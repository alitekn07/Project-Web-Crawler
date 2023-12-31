﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Web;
using System.Windows.Controls;
using System.Windows.Threading;
using static WEBCRAWLERSONPROJE.cs_Global_Variables;


namespace WEBCRAWLERSONPROJE
{
    public static class cs_public_functions
    {
        public static MainWindow refMainWindow;
        private static readonly string srDicFilePath = "crawling_dictionary.txt";
        public static StreamWriter swLogs = new StreamWriter("logs.txt", true);
        public static StreamWriter swNewFoundUrls = new StreamWriter("new_url_logs.txt", true);
        public static StreamWriter swCrawledUrls = new StreamWriter("crawled_urls.txt", true);
        private static object _lock_swLogs = new object();
        private static object _lock_swNewFoundUrls = new object();
        private static int _irThisSessionNewLinksCount = 0;
        private static int _irThisSessionCrawledCount = 0;
        public static DateTime dtCrawlingStartDate = DateTime.Now;
        static cs_public_functions()
        {
            swLogs.AutoFlush = true;
            swNewFoundUrls.AutoFlush = true;
            swCrawledUrls.AutoFlush = true;
        }
        private enum enLogType
        {
            ByPassedUrl, FoundUrl
        }
        private static void writeToLogsFile(string srLog, enLogType whichLog)
        {
            switch (whichLog)
            {
                case enLogType.ByPassedUrl:

                    // Lock usage(2022110827)

                    lock (_lock_swLogs)
                    {
                        swLogs.WriteLine(srLog + "\t" + DateTime.Now + "\r\n");
                    }
                    break;
                case enLogType.FoundUrl:

                    // Lock usage(2022110827)

                    lock (_lock_swNewFoundUrls)
                    {
                        swNewFoundUrls.WriteLine(srLog + "\t" + DateTime.Now + "\r\n");
                    }
                    break;
                default:
                    break;
            }
        }
        public static void loadCrawlingDictionary()
        {
            lock (_obj_DicCrwalingUrls_lock)
            {
                if (File.Exists(srDicFilePath))
                {
                    dicCrawlingURLs = JsonConvert.DeserializeObject<Dictionary<string, per_Crawl_URL>>(File.ReadAllText(srDicFilePath));
                }
                if (File.Exists("root_urls.txt"))
                {
                    foreach (var vrLine in File.ReadLines("root_urls.txt"))
                    {
                        addToDictionary(vrLine);
                    }
                }
                foreach (var vrPerUrl in dicCrawlingURLs)
                {
                    if (vrPerUrl.Value.blCrawled == false)
                    {
                        if (vrPerUrl.Value.irCrawlRetryCount >= irMaxRetyCount)
                        {
                            if (vrPerUrl.Value.dtCrawlingStarted.AddHours(irMaxWaitHours) > DateTime.Now)
                                continue;
                        }
                        hsNewUrls.Add(vrPerUrl.Value.srURL);
                    }
                    else
                        hsCrawledUrls.Add(vrPerUrl.Value.srURL);
                }
            }
        }
        public static void addToDictionary(string srUrl)
        {

            // Lock usage(2022110827)
            lock (_obj_DicCrwalingUrls_lock)
            {
                per_Crawl_URL myUrl = new per_Crawl_URL();
                myUrl.srNormalizedURL = srUrl.NormalizeURL();
                myUrl.srURL = srUrl;
                myUrl.srKey = srUrl.HashURL();
                myUrl.srRootSite = srUrl.GetRootURL();
                if (!dicCrawlingURLs.ContainsKey(myUrl.srKey))
                {
                    Interlocked.Increment(ref _irThisSessionNewLinksCount);
                    dicCrawlingURLs.Add(myUrl.srKey, myUrl);
                }
            }
        }
        public static void saveCrawlingDictionary()
        {

            // Lock usage(2022110827)
            lock (_obj_DicCrwalingUrls_lock)
            {
                string json = JsonConvert.SerializeObject(dicCrawlingURLs, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(srDicFilePath, json);
            }
        }
        public static void updateListStatusBox(string srMsg)
        {
            refMainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                refMainWindow.lstStatusBox.Items.Insert(0, $"{srMsg}\t{DateTime.Now}");
            }));
        }
        public static bool blCrawlingStopped = false;
        public static bool checkCrawlingCanBeStarted()
        {

            // Lock usage(2022110827)
            lock (lstRunningTasks)
            {
                foreach (var vrTask in lstRunningTasks.ToList())
                {
                    if (vrTask.Status == TaskStatus.RanToCompletion | vrTask.Status == TaskStatus.Faulted | vrTask.Status == TaskStatus.Canceled)
                    {
                        lstRunningTasks.Remove(vrTask);
                    }
                }
                if (irMax_Concurrent_Task_Count > lstRunningTasks.Count)
                    return true;
                return false;
            }
        }
        public static void crawlURL(string srUrl)
        {
            updateListStatusBox("Starting To Crawl\t" + srUrl);
            addToDictionary(srUrl);
            int irRetryCount = 0;
            var vrUrlKey = srUrl.HashURL();

            // Lock usage(2022110827)
            lock (_obj_DicCrwalingUrls_lock)
            {
                var vrCurrent = dicCrawlingURLs[vrUrlKey];
                vrCurrent.blCurrentlyCrawling = true;
                vrCurrent.dtCrawlingStarted = DateTime.Now;
                vrCurrent.irCrawlRetryCount++;
                irRetryCount = vrCurrent.irCrawlRetryCount;
            }
            var vrFetchResult = page_fetcher.fetch_a_page(srUrl);
            lock (swCrawledUrls)
                swCrawledUrls.WriteLine(vrFetchResult.fetchStatusCode + "\t" + DateTime.Now + "\t" + srUrl);

            // Ref keyword in methods(2022110834)

            Interlocked.Increment(ref _irThisSessionCrawledCount);
            if (vrFetchResult.fetchStatusCode == System.Net.HttpStatusCode.OK)
            {

                // Lock usage(2022110827)
                lock (_obj_DicCrwalingUrls_lock)
                {
                    var vrCurrent = dicCrawlingURLs[vrUrlKey];
                    vrCurrent.blCurrentlyCrawling = false;
                    vrCurrent.dtCrawlingEnded = DateTime.Now;
                    if (cs_Global_Variables.blSaveHtmlSource)
                        vrCurrent.srCrawledSource = vrFetchResult.srFetchSource;
                    vrCurrent.blCrawled = true;
                }

                // Lock usage(2022110827)
                lock (hsCrawledUrls)
                    hsCrawledUrls.Add(srUrl);
                var vrNewUrls = returnNewUrls(vrFetchResult.srFetchSource, srUrl);
                addNewUrlsToQueue(vrNewUrls);
            }
            else
            {
                if (irRetryCount < irMaxRetyCount)
                {

                    // Lock usage(2022110827)
                    lock (hsNewUrls)
                        hsNewUrls.Add(srUrl);
                }
            }

            // Lock usage(2022110827)
            lock (hsCurrentlyCrawlingUrl)
                hsCurrentlyCrawlingUrl.Remove(srUrl);
        }
        private static void addNewUrlsToQueue(List<string> lstNewUrls)
        {

            // Lock usage(2022110827)
            lock (hsCrawledUrls)
            {
                foreach (var vrUrl in lstNewUrls.ToList())
                {
                    if (hsCrawledUrls.Contains(vrUrl))
                        lstNewUrls.Remove(vrUrl);
                }
                foreach (var vrNewUrl in lstNewUrls)
                {
                    addToDictionary(vrNewUrl);
                }
                if (blCrawlingStopped == false)

                    // Lock usage(2022110827)
                    lock (hsNewUrls)
                    {
                        foreach (var vrNewUrl in lstNewUrls)
                        {
                            hsNewUrls.Add(vrNewUrl);
                        }
                    }
            }
        }
        public static List<string> returnNewUrls(string srSource, string srCrawledUrl)
        {
            // Generic List, Generic Dictionary, Generic SortedList, Generic SortedDictionary, Generic Stack, Generic Queue (2022110850)
            List<string> lstFoundNewUrls = new List<string>();
            HtmlDocument hdDoc = new HtmlDocument();
            hdDoc.LoadHtml(srSource);
            var vrNodes = hdDoc.DocumentNode.SelectNodes("//a[@href]");
            if (vrNodes != null)
                foreach (HtmlNode link in vrNodes)
                {
                    var vrHrefVal = link.Attributes["href"].Value.ToString();
                    var baseUrl = new Uri(srCrawledUrl);
                    Uri newUrl;
                    if (Uri.TryCreate(baseUrl, vrHrefVal, out newUrl))
                    {
                        string srNewUrl = newUrl.AbsoluteUri.ToString().urlNormalize();
                        if (checkIfUrlToBeCrawled(srCrawledUrl, srNewUrl, false))
                        {
                            writeToLogsFile(srNewUrl, enLogType.FoundUrl);
                            lstFoundNewUrls.Add(srNewUrl);
                        }
                    }
                }
            return lstFoundNewUrls.Distinct().ToList();
        }
        private static string urlNormalize(this string srUrl)
        {
            return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(srUrl)).Split('#').FirstOrDefault();
        }
        private static readonly List<string> lst_not_allowed_uri_extensions = new List<string>
        {
          ".png",".jpg",".jpeg",".css",".js",".pdf","docx","doc"
};
        private static readonly List<string> lst_not_allowed_urls = new List<string>
  {
   "https://www.kodevreni.com/"
     };
        private static bool checkIfUrlToBeCrawled(string srCrawledUrl, string srNewUrl, bool blAllowExternalUrls = true)
        {
            Uri orgUrl = new Uri(srCrawledUrl);
            Uri newUrl = new Uri(srNewUrl);
            if (newUrl.Scheme != Uri.UriSchemeHttp && newUrl.Scheme != Uri.UriSchemeHttps)
            {
                writeToLogsFile($"Scheme {newUrl.Scheme.ToString()} not allowed url : {srNewUrl}", enLogType.ByPassedUrl);
                return false;
            }
            if (blAllowExternalUrls == false)
            {
                if (orgUrl.Host.ToString() != newUrl.Host.ToString())
                {
                    writeToLogsFile($"external links not allowed url : {srNewUrl}", enLogType.ByPassedUrl);
                    return false;
                }
            }
            foreach (var vrExtension in lst_not_allowed_uri_extensions)
            {
                if (srNewUrl.ToLowerInvariant().EndsWith(vrExtension))
                {
                    writeToLogsFile($"extension {vrExtension} not allowed url : {srNewUrl}", enLogType.ByPassedUrl);
                    return false;
                }
            }
            foreach (var vrNotAllowedUrl in lst_not_allowed_urls)
            {
                if (srNewUrl.Contains(vrNotAllowedUrl))
                {
                    writeToLogsFile($"crawling url {vrNotAllowedUrl} is not allowed url : {srNewUrl}", enLogType.ByPassedUrl);
                    return false;
                }
            }
            return true;
        }

        // Static and non-static class usage (2022110808)
        // Public and private class, variable, and method usage (2022110809)

        public class csStatistics
        {
            public static int irHowManyStatistics = 7;
            public int irCurrentlyCrawlingUrlsCount { get; set; }
            public int irCrawlingCompletedThisSessionCount { get; set; }
            public int irTotalCrawledUrlsCount { get; set; }
            public int irThisSessionNewLinksCount { get; set; }
            public int irTotalNewLinksCount { get; set; }
            public int irTotalUnCrawledUrlsCount { get; set; }
            public int irAllTimeUrlsCount { get; set; }
            public int irHowManyCrawledPerMinute { get; set; }
        }
        public static csStatistics returnStatistic()
        {
            csStatistics myTempStatistics = new csStatistics();
            myTempStatistics.irCurrentlyCrawlingUrlsCount = hsCurrentlyCrawlingUrl.Count;
            myTempStatistics.irThisSessionNewLinksCount = _irThisSessionNewLinksCount;
            myTempStatistics.irTotalCrawledUrlsCount = hsCrawledUrls.Count;
            myTempStatistics.irCrawlingCompletedThisSessionCount = _irThisSessionCrawledCount;
            myTempStatistics.irTotalUnCrawledUrlsCount = hsNewUrls.Count;
            myTempStatistics.irAllTimeUrlsCount = dicCrawlingURLs.Count;
            myTempStatistics.irHowManyCrawledPerMinute = Convert.ToInt32(_irThisSessionCrawledCount / ((DateTime.Now - dtCrawlingStartDate).TotalMinutes));
            return myTempStatistics;
        }
    }
}
