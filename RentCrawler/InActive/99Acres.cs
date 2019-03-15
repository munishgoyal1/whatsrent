//#define TRACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpLibrary;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using HtmlParsingLibrary;

namespace WhatsRent.Crawler.RentPageFetcher
{
    class Acres99
    {
        const string ROOT_DIR = @"D:\CocuData\Data\99acres\";
        const string CITIES_REF_FILE = "AllCities.txt";
        const string ALL_RES = "";
        const string ALL_COM = "commercial-";

        //res- http://www.99acres.com/real-estate-agents-in-delhi-ncr-ffid?search_type=QS&search_location=HP&lstAcn=HP_C&src=L5&np_search_type=NP%2CR2M&isvoicesearch=N&price_min=null&price_max=null&property_type=1%2C4%2C2%2C3%2C90%2C5%2C22%2C80
        //resrent- http://www.99acres.com/rent-real-estate-agents-in-delhi-ncr-ffid
        //com- http://www.99acres.com/commercial-real-estate-agents-in-delhi-ncr-ffid
        //comlease- http://www.99acres.com/rent-commercial-real-estate-agents-in-delhi-ncr-ffid?search_type=QS&search_location=SH&lstAcn=DEALER_SEARCH&lstAcnId=4114727384176614&src=L5&np_search_type=NP%2CR2M&area_unit=1&price_min=null&price_max=null&property_type=6%2C82%2C7%2C9%2C10%2C83%2C11%2C12%2C13%2C14%2C15%2C17%2C18%2C19%2C21%2C81

        const string A99_AGENT_LIST_URL = @"http://www.99acres.com/{0}real-estate-agents-in-{1}-ffid-page-{2}";
        const string A99_AGENT_URL = @"http://www.99acres.com/do/dealer_profile/searchProfile?profileid={0}";
        const string ROOT_BROKER_DIR = ROOT_DIR + @"Brokers\";
        const string REF_FILES_BROKER_DIR = ROOT_DIR + @"1_BROKER_REF_FILES\";
        const string CITYWISE_BROKER_URLS_REF_FILE = "BrokerURLs-{0}.txt";
        const string CITIES_BROKER_SKIP_FILE = "1_SkipCities_brokers.txt";
        const string CITIES_BROKER_ERR_FILE = "1_{0}_CitiesErr_brokers.txt";
        const string CITIES_BROKER_PAGEMAP_FILE = "1_{0}_CitiesPageMap_brokers.txt";
        const string BROKERS_CITY_ERR_FILE = "1_{0}_City_Errors_brokers.txt";
        const string BROKERS_ERR_FILE = "1_{0}_Errors_brokers.txt";
        const string START_AFTER_CITY_BROKER_FILE = "1_Brokers_StartAfter_City.txt";


        //res- http://www.99acres.com/new-projects-in-delhi-ncr-ffid?search_type=QS&search_location=HP&lstAcn=HP_R&src=L5&np_search_type=NP%2CR2M&isvoicesearch=N
        //com- http://www.99acres.com/new-commercial-projects-in-delhi-ncr-ffid?search_type=QS&search_location=SH&lstAcn=NPSEARCH&lstAcnId=4114467392113489&src=L5&np_search_type=NP%2CR2M&isvoicesearch=N&area_unit=1
        //res- http://www.99acres.com/new-projects-in-mumbai-ffid?search_type=QS&search_location=SH&lstAcn=NPSEARCH&lstAcnId=4114284797098355&src=L5&np_search_type=NP%2CR2M&isvoicesearch=N

        //
        //http://www.99acres.com/new-projects-in-delhi-ncr-ffid-page-1
        const string A99_PROJECT_LIST_URL = @"http://www.99acres.com/new-{0}projects-in-{1}-ffid-page-{2}";
        const string ROOT_PROJECT_DIR = ROOT_DIR + @"Projects\";
        const string REF_FILES_PROJECT_DIR = ROOT_DIR + @"1_PROJECT_REF_FILES\";
        const string CITYWISE_PROJECT_URLS_REF_FILE = "ProjectURLs-{0}.txt";
        const string CITIES_PROJECT_SKIP_FILE = "1_SkipCities_projects.txt";
        const string CITIES_PROJECT_ERR_FILE = "1_{0}_CitiesErr_projects.txt";
        const string PROJECTS_ERR_FILE = "1_{0}_Errors_projects.txt";
        const string PROJECTS_CITY_ERR_FILE = "1_{0}_City_Errors_projects.txt";
        const string CITIES_PROJECT_PAGEMAP_FILE = "1_{0}_CitiesPageMap_projects.txt";
        const string START_AFTER_CITY_PROJECT_FILE = "1_Projects_StartAfter_City.txt";

        static int webCallCntr = 0;
        static int newBrokersCntr = 0;
        static int newProjectsCntr = 0;
        static readonly string TODAYDATE = DateTime.Now.ToString("YYYYmmdd");


        static CookieContainer mCookieContainer = new CookieContainer();

        static string GetWebPage(string url)
        {
            var res = Interlocked.Increment(ref webCallCntr);
            Console.WriteLine(webCallCntr + " Webcall Counter at: " + DateTime.Now.ToLongTimeString());

            int retryCnt = 0;
            string response = "";
            do
            {
                response = HttpHelper.GetWebPageResponse(url, null, null, mCookieContainer);
                retryCnt++;
            } while (string.IsNullOrEmpty(response) && retryCnt < 3);

            return response;
        }

        public static void Run99Acres(string[] args)
        {
            //bool doBrokers = true;
            //if (args.Length > 1)
            //    doBrokers = int.Parse(args[0]) == 0 ? false : true;

            if (!Directory.Exists(ROOT_DIR))
                Directory.CreateDirectory(ROOT_DIR);

            //if (doBrokers)
            //    GetBrokers();
            //else
            //    GetProjects();
            //var cities = GetMBCities();
            //var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            //File.WriteAllText(citiesFile, string.Join("\n", cities.ToArray()));
            //return;

            //GetProjects(null);
            //return;


            ParameterizedThreadStart ptsb = new ParameterizedThreadStart(GetBrokers);
            Thread tb = new Thread(ptsb);
            tb.Start();

            ParameterizedThreadStart ptsp = new ParameterizedThreadStart(GetProjects);
            Thread tp = new Thread(ptsp);
            tp.Start();

            tb.Join();
            tp.Join();

            Console.WriteLine(string.Format("Finished (Press any key to exit...)\nNumber of webcalls: {0}\nNew Brokers: {1}\nNew Projects: {2}",
                webCallCntr, newBrokersCntr, newProjectsCntr));
            Console.ReadKey();
        }

        static void GetBrokers(object obj)
        {
            var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            var cities = File.ReadAllLines(citiesFile);

            var skipCitiesFile = REF_FILES_BROKER_DIR + CITIES_BROKER_SKIP_FILE;
            var skipCities = new HashSet<string>(File.ReadAllLines(skipCitiesFile));

            var startAfterCityFile = REF_FILES_BROKER_DIR + START_AFTER_CITY_BROKER_FILE;
            var startAfterCity = File.ReadAllText(startAfterCityFile);
            bool isCityFound = false;
            List<string> AllBrokerUrls = new List<string>(50000);

            foreach (var city in cities)
            {
                if (!string.IsNullOrEmpty(startAfterCity) && city != startAfterCity && !isCityFound)
                    continue;
                else if (city == startAfterCity)
                    isCityFound = true;

                if (skipCities.Contains(city))
                    continue;

                List<string> BrokerUrls = new List<string>(5000);

                var brokersRes = GetCityBrokerData(city, ALL_RES);
                var brokersCom = GetCityBrokerData(city, ALL_COM);
                BrokerUrls.AddRange(brokersRes);
                BrokerUrls.AddRange(brokersCom);
                AllBrokerUrls.AddRange(brokersRes);
                AllBrokerUrls.AddRange(brokersCom);

                // Write the urls file and cities
                var cityBrokerUrlsFile = REF_FILES_BROKER_DIR + string.Format(CITYWISE_BROKER_URLS_REF_FILE, city);
                File.WriteAllText(cityBrokerUrlsFile, string.Join("\n", BrokerUrls.ToArray()));
            }

            var allBrokerUrlsFile = REF_FILES_BROKER_DIR + string.Format(CITYWISE_BROKER_URLS_REF_FILE, "All");
            File.WriteAllText(allBrokerUrlsFile, string.Join("\n", AllBrokerUrls.ToArray()));
        }

        static List<string> GetCityBrokerData(string city, string type)
        {
            List<string> brokerUrls = new List<string>(1000);
            HashSet<string> brokerIDs = new HashSet<string>();

            Console.WriteLine("Doing Brokers for " + city + "....");

            var cityInUrl = city.Replace(" ", "-");
            // create city dir if absent
            var cityDir = ROOT_BROKER_DIR + city + "/";
            if (!Directory.Exists(cityDir))
                Directory.CreateDirectory(cityDir);

            var cityUrl = string.Format(A99_AGENT_LIST_URL, type, cityInUrl, 1).ToLower();
            var citypgdata = GetWebPage(cityUrl);

            if (string.IsNullOrEmpty(citypgdata))
            {
                File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(BROKERS_CITY_ERR_FILE, TODAYDATE), city + "\n");
                Console.WriteLine(string.Format("Broker page {0} empty for {1}. Skipping city", 1, city));
                return brokerUrls;
            }

            const string PAGE_NOT_FOUND_ERR = "No Dealers Currently available for";
            if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                return brokerUrls;

            // Get last pagenumber
            //</a>  ....  <a   href="/commercial-real-estate-agents-in-gurgaon-ffid-page-12?
            string pgNumStart = "</a>  ....  <a";
            string pgNumEnd = "?";
            string pgOther = "name=\"page\"";

            var pgIdx = citypgdata.LastIndexOf(pgOther);

            var tmpIdx = 0;
            //var tmpStr = StringParser.GetStringBetween(citypgdata, 0, pgNumStart, pgNumEnd, null, out tmpIdx);
            var tmpStr = StringParser.GetStringBetween(citypgdata, pgIdx, ">", "<", null, out tmpIdx);

            if (tmpIdx > 0 && !string.IsNullOrEmpty(tmpStr))
            {
                //var lastPageNum = int.Parse(tmpStr.Substring(tmpStr.IndexOf("page-") + 5)) + 1;
                var lastPageNum = int.Parse(tmpStr);
                int pageNum = 1;

                while (pageNum <= lastPageNum)
                {
                    Console.WriteLine(string.Format("Brokers:{0} Page{1}", city, pageNum));
                    cityUrl = string.Format(A99_AGENT_LIST_URL, type, cityInUrl, pageNum++).ToLower();
                    citypgdata = GetWebPage(cityUrl);

                    if (string.IsNullOrEmpty(citypgdata))
                    {
                        File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(CITIES_BROKER_ERR_FILE, TODAYDATE), cityUrl + "\n");
                        Console.WriteLine(string.Format("Broker page {0} empty for {1}. Skipping page", pageNum, city));
                        continue;
                    }

                    if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                    {
                        File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(CITIES_BROKER_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, brokerIDs.Count));
                        break;
                    }

                    // Get broker url
                    ///do/dealer_profile/searchProfile?
                    const string brokStart = "window.open('";
                    const string brokEnd = "lstAcn";
                    //window.open('
                    //lstAcn
                    var brokerUrl = "dummy";
                    var brokIdx = 0;
                    var brokRunIdx = 1;

                    while (brokRunIdx > 0)
                    {
                        var brokerId = StringParser.GetStringBetween(citypgdata, brokIdx, brokStart, brokEnd, null, out brokRunIdx);
                        brokerId = StringParser.GetStringBetween(brokerId ?? "", 0, "profileid=", "&", null, out tmpIdx);

                        if (brokRunIdx > 0 && !string.IsNullOrEmpty(brokerId))
                        {
                            brokerUrl = string.Format(A99_AGENT_URL, brokerId);
                            brokIdx = brokRunIdx;

                            // Get broker page and save it
                            tmpIdx = 0;
                            if (!brokerIDs.Contains(brokerId))
                            {
                                brokerIDs.Add(brokerId);
                                var fileName = cityDir + brokerId + ".html";

                                if (!File.Exists(fileName))
                                {
                                    var brokerPage = GetWebPage(brokerUrl);
                                    if (string.IsNullOrEmpty(brokerPage))
                                        File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(BROKERS_ERR_FILE, TODAYDATE), brokerUrl + "\n");
                                    File.WriteAllText(fileName, brokerPage);
                                    Interlocked.Increment(ref newBrokersCntr);
                                    Console.WriteLine("New Broker added, total new entries so far: " + newBrokersCntr);
                                }
                                brokerUrls.Add(brokerUrl);
                            }
                        }
                    }
                }
                File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(CITIES_BROKER_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, brokerIDs.Count));
            }
            return brokerUrls;
        }

        // ====================================== PROJECTS =============================================



        static void GetProjects(object obj)
        {
            //var cities = GetMBCities();
            var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            //File.WriteAllText(citiesFile, string.Join("\n", cities.ToArray()));
            var cities = File.ReadAllLines(citiesFile);

            var skipCitiesFile = REF_FILES_PROJECT_DIR + CITIES_PROJECT_SKIP_FILE;
            var skipCities = new HashSet<string>(File.ReadAllLines(skipCitiesFile));

            var startAfterCityFile = REF_FILES_PROJECT_DIR + START_AFTER_CITY_PROJECT_FILE;
            var startAfterCity = File.ReadAllText(startAfterCityFile);
            bool isCityFound = false;
            List<string> AllProjectUrls = new List<string>(50000);


            foreach (var city in cities)
            {
                if (!string.IsNullOrEmpty(startAfterCity) && city != startAfterCity && !isCityFound)
                    continue;
                else if (city == startAfterCity)
                    isCityFound = true;

                if (skipCities.Contains(city))
                    continue;

                List<string> ProjectUrls = new List<string>(5000);

                var projectsRes = GetCityProjectData(city, ALL_RES);
                var projectsCom = GetCityProjectData(city, ALL_COM);
                ProjectUrls.AddRange(projectsRes);
                ProjectUrls.AddRange(projectsCom);
                AllProjectUrls.AddRange(projectsRes);
                AllProjectUrls.AddRange(projectsCom);

                // Write the urls file and cities
                var cityProjectUrlsFile = REF_FILES_PROJECT_DIR + string.Format(CITYWISE_PROJECT_URLS_REF_FILE, city);
                File.WriteAllText(cityProjectUrlsFile, string.Join("\n", ProjectUrls.ToArray()));
            }

            var brokerUrlsFile = REF_FILES_PROJECT_DIR + string.Format(CITYWISE_PROJECT_URLS_REF_FILE, "All");
            File.WriteAllText(brokerUrlsFile, string.Join("\n", AllProjectUrls.ToArray()));
        }

        static List<string> GetCityProjectData(string city, string type)
        {
            List<string> projectUrls = new List<string>(1000);
            HashSet<string> projectIDs = new HashSet<string>();

            Console.WriteLine("Doing Projects for " + city + "....");

            var cityInUrl = city.Replace(" ", "-");

            // create city dir if absent
            var cityDir = ROOT_PROJECT_DIR + city + "/";
            if (!Directory.Exists(cityDir))
                Directory.CreateDirectory(cityDir);

            var cityUrl = string.Format(A99_PROJECT_LIST_URL, type, cityInUrl, 1).ToLower();
            var citypgdata = GetWebPage(cityUrl);

            if (string.IsNullOrEmpty(citypgdata))
            {
                File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(PROJECTS_CITY_ERR_FILE, TODAYDATE), city + "\n");
                Console.WriteLine(string.Format("Project page {0} empty for {1}. Skipping city", 1, city));
                return projectUrls;
            }

            const string PAGE_NOT_FOUND_ERR = "No New Projects Currently available for";
            if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                return projectUrls;


            // Get last pagenumber
            //</a>  ....  <a   href="/commercial-real-estate-agents-in-gurgaon-ffid-page-12?
            string pgNumStart = "</a>  ....  <a";
            string pgNumEnd = "?";
            string pgOther = "name=\"page\"";

            //if (!citypgdata.Contains(pgNumStart))
            //{
            //}

            var pgIdx = citypgdata.LastIndexOf(pgOther);

            var tmpIdx = 0;
            //var tmpStr = StringParser.GetStringBetween(citypgdata, 0, pgNumStart, pgNumEnd, null, out tmpIdx);
            var tmpStr = StringParser.GetStringBetween(citypgdata, pgIdx, ">", "<", null, out tmpIdx);

            if (tmpIdx > 0 && !string.IsNullOrEmpty(tmpStr))
            {
                //var lastPageNum = int.Parse(tmpStr.Substring(tmpStr.IndexOf("page-") + 5)) + 1;
                var lastPageNum = int.Parse(tmpStr);
                int pageNum = 1;

                while (pageNum <= lastPageNum)
                {
                    Console.WriteLine(string.Format("Projects:{0} Page{1}", city, pageNum));
                    cityUrl = string.Format(A99_PROJECT_LIST_URL, type, cityInUrl, pageNum++).ToLower();
                    citypgdata = GetWebPage(cityUrl);

                    if (string.IsNullOrEmpty(citypgdata))
                    {
                        File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_ERR_FILE, TODAYDATE), cityUrl + "\n");
                        Console.WriteLine(string.Format("Project page {0} empty for {1}. Skipping page", pageNum, city));
                        continue;
                    }

                    if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                    {
                        File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, projectIDs.Count));
                        break;
                    }

                    // Get project url
                    ///do/dealer_profile/searchProfile?
                    const string projStart = "window.open('";
                    const string projEnd = "'+'?";

                    var projectUrl = "dummy";
                    var projIdx = 0;
                    var projRunIdx = 1;

                    while (projRunIdx > 0)
                    {
                        projectUrl = StringParser.GetStringBetween(citypgdata, projIdx, projStart, projEnd, null, out projRunIdx);

                        if (projRunIdx > 0 && !string.IsNullOrEmpty(projectUrl))
                        {
                            projIdx = projRunIdx;

                            // Get project id
                            var projectId = projectUrl.Substring(projectUrl.LastIndexOf("-") + 1); 

                            // Get project page and save it
                            tmpIdx = 0;
                            if (!projectIDs.Contains(projectId))
                            {
                                projectIDs.Add(projectId);
                                var fileName = cityDir + projectId + ".html";

                                if (!File.Exists(fileName))
                                {
                                    var projectPage = GetWebPage(projectUrl);
                                    if (string.IsNullOrEmpty(projectPage))
                                        File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(PROJECTS_ERR_FILE, TODAYDATE), projectUrl + "\n");
                                    File.WriteAllText(fileName, projectPage);
                                    Interlocked.Increment(ref newProjectsCntr);
                                    Console.WriteLine("New Project added, total new entries so far: " + newProjectsCntr);
                                }
                                projectUrls.Add(projectUrl);
                            }
                        }
                    }
                }
                File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, projectIDs.Count));
            }
            return projectUrls;
        }

        static IEnumerable<string> GetMBCities()
        {
            const string citiesUrl = @"http://www.99acres.com/new-commercial-projects-in-delhi-ncr-ffid";
            var pgdata = GetWebPage(citiesUrl);

            File.WriteAllText(ROOT_DIR + "CitiesPage.html", pgdata);
            //<option class="boldclass" value="1" selected>
            //">Mumbai (All)</option>
            var strStart = "<option class=\"boldclass\" value=\"1\" selected>";
            var strEnd = "International</option>";
            var cityStart = "\">";
            var cityEnd = "</option>";
            int pgRunIdx = 1;
            int pgIdx = 0;
            List<string> cities = new List<string>(500);

            string grpData = StringParser.GetStringBetween(pgdata, pgIdx, strStart, strEnd, null, out pgRunIdx);

            if (pgRunIdx > 0)
            {
                var grpRunIdx = 1;

                while (grpRunIdx > 0 && !String.IsNullOrEmpty(grpData))
                {
                    var city = StringParser.GetStringBetween(grpData, pgIdx, cityStart, cityEnd, null, out grpRunIdx);

                    if (grpRunIdx < 0 || string.IsNullOrEmpty(city))
                        break;

                    pgIdx = grpRunIdx;
                    cities.Add(city);
                }
            }
            return cities;
        }
    }
}