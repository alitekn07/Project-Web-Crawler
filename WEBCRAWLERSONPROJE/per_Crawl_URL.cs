using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WEBCRAWLERSONPROJE
{

    // Static and non-static class usage (2022110808)
    // Public and private class, variable, and method usage (2022110809)

    public class per_Crawl_URL
    {
        public string srURL { get; set; }
        public string srNormalizedURL { get; set; }
        public string srKey { get; set; }
        public DateTime dtCrawlingStarted;
        public DateTime dtCrawlingEnded;
        public string srCrawledSource { get; set; }
        public string srRootSite { get; set; }
        public bool blCurrentlyCrawling = false;
        public int irCrawlRetryCount = 0;
        public static List<Task> lstRunningTasks = new List<Task>();
        public int irCrawlRetyCount = 0;
        public bool blCrawled = false;
    }
}