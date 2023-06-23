using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WEBCRAWLERSONPROJE
{

    // Static and non-static class usage (2022110808)
    // Public and private class, variable, and method usage (2022110809)

    public static class cs_Global_Variables
    {
        public static int irMax_Concurrent_Task_Count = 10; // Kullanımı eşzamanlı Crawl yapma, default olarak 10 ayarladım, arttırıp azaltabiliyorum uygulamadan
        public static int irPerThreadStartMiliSeconds = 1;
        public static int irMaxRetyCount = 3; // --> Kullanımı Max deneme adedi url'yi
        public static int irMaxWaitHours = 24; // --> Kullanımı Crawling'den sonra beklenmesi gereken süre tekrar Crawling yapmaması için

        // Kod Düzeni Temizliği bozulmasın diye diğer class'larda kullandığım kodları buraya derliyorum hocam, aşağıda belirttiğim kodlarda yukarıdaki kullanım başlığına giriyor

        // public static bool blCrawlingStopped = false; = Bulunduğu CLASS (cs_public_functions) - Uygulamada ki kullanımı ise Crawling'i durdurmak




        // Generic List, Generic Dictionary, Generic SortedList, Generic SortedDictionary, Generic Stack, Generic Queue (2022110850)

        public static object _obj_DicCrwalingUrls_lock = new object();
        public static Dictionary<string, per_Crawl_URL> dicCrawlingURLs = new Dictionary<string, per_Crawl_URL>();
        public static List<Task> lstRunningTasks = new List<Task>();
        public static HashSet<string> hsCrawledUrls = new HashSet<string>();
        public static HashSet<string> hsNewUrls = new HashSet<string>();
        public static HashSet<string> hsCurrentlyCrawlingUrl = new HashSet<string>();
        public static bool blSaveHtmlSource = false;
    }
}



