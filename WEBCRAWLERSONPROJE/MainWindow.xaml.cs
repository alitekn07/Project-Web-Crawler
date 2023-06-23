using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Markup;
using System.Windows.Threading;
using static WEBCRAWLERSONPROJE.cs_public_functions;
using static WEBCRAWLERSONPROJE.cs_Global_Variables;
using System.Diagnostics;
using System.Security.Policy;
using static WEBCRAWLERSONPROJE.csHelperMethods;
using System.Collections.ObjectModel;

namespace WEBCRAWLERSONPROJE
{

    public partial class MainWindow : Window
    {

        private static int _irNumberOfTotalConcurrentCrawling = 20;
        private static int _irMaximumTryCount = 3;

        private ObservableCollection<string> _Results = new ObservableCollection<string>();
        public ObservableCollection<string> UserLogs
        {
            get { return _Results; }
            set
            {
                _Results = value;
            }
        }



        public MainWindow()
        {
            System.Threading.Thread.Sleep(2000);

            InitializeComponent();
            ThreadPool.SetMaxThreads(100000, 100000);
            ThreadPool.SetMinThreads(100000, 100000);
            cs_public_functions.refMainWindow = this;
            ServicePointManager.DefaultConnectionLimit = 1000; //this increases your number of connections to per host at the same time
            listBoxResults.ItemsSource = UserLogs;

            for (int i = 0; i < cs_public_functions.csStatistics.irHowManyStatistics; i++)
            {
                lstboxStatistics.Items.Add("A Statistic Will Be Here");
            }

            if (cs_Global_Variables.blSaveHtmlSource)
                chkBoxSaveSourceCode.IsChecked = true;
        }

        DateTime dtStartDate;

        // Events (2022110842)

        private void btnSingleUrlCrawl_Click(object sender, RoutedEventArgs e)
        {
            doSingleCrawl(txtUrl.Text);
        }
        private void doSingleCrawl(string urlAddress)
        {
            System.Threading.Thread.Sleep(10 * 1000);
            var vrResult = page_fetcher.fetch_a_page(urlAddress);
            if (vrResult.fetchStatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show((int)vrResult.fetchStatusCode + "\n" +
                    vrResult.fetchStatusCode.ToString() +
                    "\n" +
                    vrResult.fetchStatusDescription);
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                txtSourceCode.Text = vrResult.srFetchSource;
            }));
        }

        // Events (2022110842)
        private void btnTimerStart_Click(object sender, RoutedEventArgs e)
        {
            startTimer();
        }

        // Timers (2022110841)
        private void startTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(update_timer_label);
            dispatcherTimer.Tick += new EventHandler(updateStatistics);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        // Timers (2022110841)
        private void update_timer_label(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblTimer.Content = $"Time: {DateTime.Now.ToString()} :{DateTime.Now.Millisecond}";
            }));
        }



        private void updateStatistics(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var vrStatistics = cs_public_functions.returnStatistic();

                var vrThisSessionNewLinksCount = "New Links Found This Session: " + vrStatistics.irThisSessionNewLinksCount.ToString("N0");
                var vrCrawlingCompletedThisSessionCount = "Crawling Completed Link Count This Session: " + vrStatistics.irCrawlingCompletedThisSessionCount.ToString("N0");
                var vrCurrentlyCrawlingUrlsCount = "Currently Crawling Urls Count: " + vrStatistics.irCurrentlyCrawlingUrlsCount.ToString("N0");
                var vrTotalCrawlingCount = "All Time Crawling Completed Urls Count: " + vrStatistics.irTotalCrawledUrlsCount.ToString("N0");
                var vrTotalUnCrawledUrlsCount = "All Time Waiting To Be Crawling Urls Count: " + vrStatistics.irTotalUnCrawledUrlsCount.ToString("N0");
                var vrAllTimeUrlsCount = "All Time Urls Count: " + vrStatistics.irAllTimeUrlsCount.ToString("N0");
                var vrPerMinuteCount = "Per Minute Urls Crawling Count: " + vrStatistics.irHowManyCrawledPerMinute.ToString("N0");



                lstboxStatistics.Items[0] = vrThisSessionNewLinksCount;
                lstboxStatistics.Items[1] = vrCrawlingCompletedThisSessionCount;
                lstboxStatistics.Items[2] = vrCurrentlyCrawlingUrlsCount;
                lstboxStatistics.Items[3] = vrTotalCrawlingCount;
                lstboxStatistics.Items[4] = vrTotalUnCrawledUrlsCount;
                lstboxStatistics.Items[5] = vrAllTimeUrlsCount;
                lstboxStatistics.Items[6] = vrPerMinuteCount;

            }));
        }

        // Events (2022110842)
        private void btnTaskBasedCrawl_Click(object sender, RoutedEventArgs e)
        {
            string srUrl = txtUrl.Text;
            Task VrStartedTask = Task.Factory.StartNew(() => { doSingleCrawl(srUrl); }).ContinueWith(task =>
            {
                MessageBox.Show("Task Has Finished: " + task.Status + " " + DateTime.Now);
            });
            VrStartedTask.Wait();
            MessageBox.Show("Task Started But Not Finished Yet: " + VrStartedTask.Status + " " + DateTime.Now);
        }

        // Events (2022110842)
        private void ThreadCountTest_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1000; i++)
            {
                Task.Factory.StartNew(() => { tempTask(); }, TaskCreationOptions.LongRunning);
            }
        }

        // Events (2022110842)
        private void ThreadCountTestDefault_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1000; i++)
            {
                Task.Factory.StartNew(() => { tempTask(); });
            }
        }
        // Events (2022110842)
        private void tempTask()
        {
            System.Threading.Thread.Sleep(100 * 1000);
        }
        DispatcherTimer dispatcherCrawling = new DispatcherTimer();

        // Events (2022110842)
        private void btnStartMainCrawling_Click(object sender, RoutedEventArgs e)
        {

            txtNewMaxConcurrent.Text = cs_Global_Variables.irMax_Concurrent_Task_Count.ToString();

            cs_public_functions.dtCrawlingStartDate = DateTime.Now;
            startTimer();
            cs_public_functions.loadCrawlingDictionary();
            dispatcherCrawling.Tick += new EventHandler(doMainCrawling);
            dispatcherCrawling.Interval = new TimeSpan(0, 0, 0, 0, cs_Global_Variables.irPerThreadStartMiliSeconds);
            dispatcherCrawling.Start();
        }

        // Events (2022110842)
        private void doMainCrawling(object sender, EventArgs e)
        {

            for (int i = 0; i < cs_Global_Variables.irMax_Concurrent_Task_Count; i++)
            {

                Debug.WriteLine($"doMainCrawling called " + DateTime.Now.ToLocalTime() + " " + DateTime.Now.Millisecond);

                if (!checkCrawlingCanBeStarted())
                {
                    return;
                }
                string srNewUrl = null;
                lock (hsNewUrls)
                {
                    if (hsNewUrls.Count == 0)
                    {
                        lock (hsCurrentlyCrawlingUrl)
                            if (hsCurrentlyCrawlingUrl.Count == 0)
                            {
                                dispatcherCrawling.Stop();
                                saveCrawlingDictionary();
                                MessageBox.Show("Crawling Completed");
                                return;
                            }
                    }
                    foreach (var vrNewUrl in hsNewUrls)
                    {
                        if (hsCurrentlyCrawlingUrl.Contains(vrNewUrl))
                            continue;
                        srNewUrl = vrNewUrl;
                        break;
                    }
                    if (string.IsNullOrEmpty(srNewUrl))
                        return;
                    hsNewUrls.Remove(srNewUrl);

                    lock (hsCurrentlyCrawlingUrl)
                    {
                        hsCurrentlyCrawlingUrl.Add(srNewUrl);
                        var vrTask = Task.Factory.StartNew(() => { cs_public_functions.crawlURL(srNewUrl); });

                        lock (lstRunningTasks)
                            lstRunningTasks.Add(vrTask);
                    }
                }
            }
        }


        // Events (2022110842)
        private void btnStopCrawling_Click(object sender, RoutedEventArgs e)
        {
            blCrawlingStopped = true;
            lock (hsCrawledUrls)
            {
                hsNewUrls = new HashSet<string>();
            }
            cs_public_functions.updateListStatusBox("Crawling Stopped By Manually.");
        }


        // Events (2022110842)
        private void chkBoxSaveSourceCode_Click(object sender, RoutedEventArgs e)
        {
            if (chkBoxSaveSourceCode.IsChecked == true)
                blSaveHtmlSource = true;
            else
                blSaveHtmlSource = false;
        }


        // Events (2022110842)
        private void btnSetNewMaxCount_Click(object sender, RoutedEventArgs e)
        {
            int irmaxnewconcurrent = 0;
            Int32.TryParse(txtNewMaxConcurrent.Text, out irmaxnewconcurrent);
            if (irmaxnewconcurrent > 0)
                cs_Global_Variables.irMax_Concurrent_Task_Count = irmaxnewconcurrent;
        }


        // Events (2022110842)
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        // Events (2022110842)
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Events (2022110842)
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
           
            using (DBCrawling db = new DBCrawling())
            {
                db.tblMainUrls.RemoveRange(db.tblMainUrls);
                db.SaveChanges();

                db.tblMainUrls.Add(new tblMainUrl { Url = "www.toros.edu.tr", ParentUrlHash = "www.toros.edu.tr", SourceCode = "gg", UrlHash = "ww" });

                db.SaveChanges();
            }
        }



        // Events (2022110842)
        private void clearDBandStart(object sender, RoutedEventArgs e)
        {
            dtStartDate = DateTime.Now;
            // clearDatabase();
            crawlPage(txtInputUrl.Text.normalizeUrl(), 0, txtInputUrl.Text.normalizeUrl(), DateTime.Now);
            checkingTimer();
        }

        private void checkingTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(startPollingAwaitingURLs);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dispatcherTimer.Start();
        }


        private static object _lock_CrawlingSync = new object();
        private static bool blBeingProcessed = false;
        private static List<Task> lstCrawlingTasks = new List<Task>();
        private static List<string> lstCurrentlyCrawlingUrls = new List<string>();

        private void startPollingAwaitingURLs(object sender, EventArgs e)
        {

            lock (UserLogs)
            {
                string srPerMinCrawlingspeed = (irCrawledUrlCount.ToDouble() / (DateTime.Now - dtStartDate).TotalMinutes).ToString("N2");

                string srPerMinDiscoveredLinkSpeed = (irDiscoveredUrlCount.ToDouble() / (DateTime.Now - dtStartDate).TotalMinutes).ToString("N2");

                string srPassedTime = (DateTime.Now - dtStartDate).TotalMinutes.ToString("N2");

                UserLogs.Insert(0, $"{DateTime.Now} polling awaiting urls \t processing: {blBeingProcessed} \t number of crawling tasks: {lstCrawlingTasks.Count}");

                UserLogs.Insert(0, $"Total Time: {srPassedTime} Minutes \t Total Crawled Links Count: {irCrawledUrlCount.ToString("N0")} \t Crawling Speed Per Minute: {srPerMinCrawlingspeed} \t Total Discovered Links : {irDiscoveredUrlCount.ToString("N0")} \t Discovered Url Speed: {srPerMinDiscoveredLinkSpeed} ");
            }

            logMesssage($"polling awaiting urls \t processing: {blBeingProcessed} \t number of crawling tasks: {lstCrawlingTasks.Count}");




            if (blBeingProcessed)
                return;

            //Lock usage (2022110827)
            lock (_lock_CrawlingSync)
            {
                blBeingProcessed = true;

                lstCrawlingTasks = lstCrawlingTasks.Where(pr => pr.Status != TaskStatus.RanToCompletion && pr.Status != TaskStatus.Faulted).ToList();

                int irTasksCountToStart = _irNumberOfTotalConcurrentCrawling - lstCrawlingTasks.Count;

                if (irTasksCountToStart > 0)
                    using (DBCrawling db = new DBCrawling())
                {
                    var vrReturnedList = db.tblMainUrls.Where(x => x.IsCrawled == false && x.CrawlTryCounter < _irMaximumTryCount).OrderBy(pr => pr.DiscoverDate).Select(x => new
                    {
                        x.Url,
                        x.LinkDepthLevel
                    }).Take(irTasksCountToStart * 2).ToList();

                    logMesssage(string.Join(" , ", vrReturnedList.Select(pr => pr.Url)));

                    foreach (var vrPerReturned in vrReturnedList)
                    {
                        var vrUrlToCrawl = vrPerReturned.Url;
                        int irDepth = vrPerReturned.LinkDepthLevel;
                        lock (lstCurrentlyCrawlingUrls)
                        {
                            if (lstCurrentlyCrawlingUrls.Contains(vrUrlToCrawl))
                            {
                                logMesssage($"bypass url since already crawling: \t {vrUrlToCrawl}");
                                continue;
                            }
                            lstCurrentlyCrawlingUrls.Add(vrUrlToCrawl);
                        }

                        logMesssage($"starting crawling url: \t {vrUrlToCrawl}");

                        lock (UserLogs)
                        {
                            UserLogs.Insert(0, $"{DateTime.Now} starting crawling url: \t {vrUrlToCrawl}");
                        }

                        var vrStartedTask = Task.Factory.StartNew(() => { crawlPage(vrUrlToCrawl, irDepth, null, DateTime.MinValue); }).ContinueWith((pr) =>
                        {

                            lock (lstCurrentlyCrawlingUrls)
                            {
                                lstCurrentlyCrawlingUrls.Remove(vrUrlToCrawl);
                                logMesssage($"removing url from list since task completed: \t {vrUrlToCrawl}");
                            }

                        });
                        lstCrawlingTasks.Add(vrStartedTask);

                        if (lstCrawlingTasks.Count > _irNumberOfTotalConcurrentCrawling)
                            break;
                    }
                }
                blBeingProcessed = false;
            }
        }
    }
}