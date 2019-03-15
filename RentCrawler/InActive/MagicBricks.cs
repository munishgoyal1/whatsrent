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
    class MagicBricks
    {
        const string ROOT_DIR = @"D:\CocuData\Data\magicbricks\";
        const string CITIES_REF_FILE = "AllCities.txt";
        const string ALL_RES = "ALL-RESIDENTIAL";
        const string ALL_COM = "ALL-COMMERCIAL";

        const string MB_AGENT_BASE_URL = @"http://www.magicbricks.com/Real-estate-property-agents/{0}-agent-in-{1}/Page-{2}";
        const string ROOT_BROKER_DIR = ROOT_DIR + @"Brokers\";
        const string REF_FILES_BROKER_DIR = ROOT_DIR + @"1_BROKER_REF_FILES\";
        const string CITYWISE_BROKER_URLS_REF_FILE = "BrokerURLs-{0}.txt";
        const string CITIES_BROKER_SKIP_FILE = "1_SkipCities_brokers.txt";
        const string CITIES_BROKER_ERR_FILE = "1_{0}_CitiesErr_brokers.txt";
        const string CITIES_BROKER_PAGEMAP_FILE = "1_{0}_CitiesPageMap_brokers.txt";
        const string BROKERS_ERR_FILE = "1_{0}_Errors_brokers.txt";
        const string START_AFTER_CITY_BROKER_FILE = "1_Brokers_StartAfter_City.txt";

        const string MB_PROJECT_BASE_URL = @"http://www.magicbricks.com/Real-estate-projects-search/{0}-new-project-{1}/Page-{2}";
        const string ROOT_PROJECT_DIR = ROOT_DIR + @"Projects\";
        const string REF_FILES_PROJECT_DIR = ROOT_DIR + @"1_PROJECT_REF_FILES\";
        const string CITYWISE_PROJECT_URLS_REF_FILE = "ProjectURLs-{0}.txt";
        const string CITIES_PROJECT_SKIP_FILE = "1_SkipCities_projects.txt";
        const string CITIES_PROJECT_ERR_FILE = "1_{0}_CitiesErr_projects.txt";
        const string PROJECTS_ERR_FILE = "1_{0}_Errors_projects.txt";
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
            Console.WriteLine(webCallCntr + " Webcall Counter");
            return HttpHelper.GetWebPageResponse(url, null, null, mCookieContainer);
        }

        public static void RunMagicBricks(string[] args)
        {
            //bool doBrokers = true;
            //if (args.Length > 1)
            //    doBrokers = int.Parse(args[0]) == 0 ? false : true;

            //if (doBrokers)
            //    GetBrokers();
            //else
            //    GetProjects();

            if (!Directory.Exists(ROOT_DIR))
                Directory.CreateDirectory(ROOT_DIR);

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
            //var cities = GetMBCities();
            var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            //File.WriteAllText(citiesFile, string.Join("\n", cities.ToArray()));

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

            // Error string 
            const string PAGE_NOT_FOUND_ERR = "We don't have agents matching your search";

            // create city dir if absent
            var cityDir = ROOT_BROKER_DIR + city + "/";
            if (!Directory.Exists(cityDir))
                Directory.CreateDirectory(cityDir);

            int pageNum = 1;

            while (true)
            {
                Console.WriteLine(string.Format("Brokers:{0} Page{1}", city, pageNum));
                var cityUrl = string.Format(MB_AGENT_BASE_URL, type, city, pageNum++);
                var citypgdata = GetWebPage(cityUrl);

                if (string.IsNullOrEmpty(citypgdata))
                {
                    File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(CITIES_BROKER_ERR_FILE, TODAYDATE), cityUrl + "\n");
                    continue;
                }

                if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                {
                    File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(CITIES_BROKER_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, brokerIDs.Count));
                    break;
                }

                // Get broker url
                const string BROK_URL_BASE = @"http://www.magicbricks.com/property-agent-details/";
                const string brokStart = "href=\"/property-agent-details/";
                const string brokEnd = "\">";

                var brokerUrl = "dummy";
                var brokIdx = 0;
                var brokRunIdx = 1;
                while (brokRunIdx > 0)
                {
                    brokerUrl = StringParser.GetStringBetween(citypgdata, brokIdx, brokStart, brokEnd, null, out brokRunIdx);

                    if (brokRunIdx > 0)
                    {
                        brokerUrl = BROK_URL_BASE + brokerUrl;
                        brokIdx = brokRunIdx;
                        // Get broker page and save it
                        // Get broker ID and city
                        var tmpIdx = 0;
                        var brokId = StringParser.GetStringBetween(brokerUrl, 0, "agentid-", "&operating", null, out tmpIdx);
                        if ((!string.IsNullOrEmpty(brokId) || tmpIdx > 0) && !brokerIDs.Contains(brokId))
                        {
                            brokerIDs.Add(brokId);
                            var fileName = cityDir + brokId + ".html";

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

            return brokerUrls;
        }

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

            var allProjectUrlsFile = REF_FILES_PROJECT_DIR + string.Format(CITYWISE_PROJECT_URLS_REF_FILE, "All");
            File.WriteAllText(allProjectUrlsFile, string.Join("\n", AllProjectUrls.ToArray()));
        }

        static List<string> GetCityProjectData(string city, string type)
        {
            List<string> projectUrls = new List<string>(1000);
            HashSet<string> projectIDs = new HashSet<string>();

            Console.WriteLine("Doing Projects for " + city + "....");

            // Error string 
            const string PAGE_NOT_FOUND_ERR = "We don't have projects matching your search";

            // create city dir if absent
            var cityDir = ROOT_PROJECT_DIR + city + "/";
            if (!Directory.Exists(cityDir))
                Directory.CreateDirectory(cityDir);

            int pageNum = 1;

            while (true)
            {
                Console.WriteLine(string.Format("Projects:{0} Page{1}", city, pageNum));
                var cityUrl = string.Format(MB_PROJECT_BASE_URL, type, city, pageNum++);
                var citypgdata = GetWebPage(cityUrl);

                if (string.IsNullOrEmpty(citypgdata))
                {
                    File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_ERR_FILE, TODAYDATE), cityUrl + "\n");
                    continue;
                }

                if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                {
                    File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, projectIDs.Count));
                    break;
                }

                // Get project url
                const string PROJ_URL_BASE = @"http://www.magicbricks.com";
                const string projStart = "onclick=\"openProjectDetailPage(event, '";
                const string projEnd = "');\"";

                var projectUrl = "dummy";
                var projIdx = 0;
                var projRunIdx = 1;
                while (projRunIdx > 0)
                {
                    projectUrl = StringParser.GetStringBetween(citypgdata, projIdx, projStart, projEnd, null, out projRunIdx);

                    if (projRunIdx > 0)
                    {
                        projectUrl = PROJ_URL_BASE + projectUrl;
                        projIdx = projRunIdx;
                        // Get project page and save it
                        // Get project ID and city
                        var tmpIdx = 0;
                        //pdpid-4d4235303230363534?
                        var projId = StringParser.GetStringBetween(projectUrl, 0, "pdpid-", "?", null, out tmpIdx);
                        if ((!string.IsNullOrEmpty(projId) || tmpIdx > 0) && !projectIDs.Contains(projId))
                        {
                            projectIDs.Add(projId);
                            var fileName = cityDir + projId + ".html";

                            if (!File.Exists(fileName) || (new FileInfo(fileName).Length == 0))
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

            return projectUrls;
        }

        static IEnumerable<string> GetMBCities()
        {
            const string citiesUrl = @"http://www.magicbricks.com/Real-estate-property-agents/ALL-RESIDENTIAL-agent-in-Mumbai";
            var pgdata = GetWebPage(citiesUrl);

            File.WriteAllText(ROOT_BROKER_DIR + "CitiesPage.html", pgdata);

            var strStart = "<optgroup label=\"";
            var strEnd = "</optgroup>";
            var cityGrp = "dummy";
            var headingDD = "label=\"heading-dd\">";
            var headingDDEnd = "</option>";
            var cityStart = "\">";
            var cityEnd = "</option>";
            int pgRunIdx = 1;
            int pgIdx = 0;
            List<string> cities = new List<string>(50);
            while (pgRunIdx > 0)  //!String.IsNullOrEmpty(cityGrp) || 
            {

                //                <optgroup label="">
                //    <option value="4442" label="heading-dd">Thane</option>
                //    <option value="4442-1202032">Thane - Beyond Thane</option>
                //</optgroup>
                //<optgroup label="Andhra Pradesh">
                //    <option value="2058">Guntur</option>
                //    <option value="2060">Hyderabad</option>
                //    <option value="2121">Nellore</option>
                //    <option value="2145">Rajahmundry</option>
                //    <option value="2165">Secunderabad</option>
                //    <option value="2188">Tirupathi</option>
                //    <option value="2200">Vijayawada</option>
                //    <option value="2202">Visakhapatnam</option>
                //</optgroup>

                string grpData = StringParser.GetStringBetween(pgdata, pgIdx, strStart, strEnd, null, out pgRunIdx);
                pgIdx = pgRunIdx;

                // if heading-dd then take only one city, else take each city
                // Parse grpData
                var grpRunIdx = 1;

                if (pgRunIdx < 0)
                    break;

                // advance to >
                int idx1 = grpData.IndexOf(">");
                grpData = grpData.Substring(idx1, grpData.Length - idx1);

                if (grpData.Contains(headingDD))
                {
                    var city = StringParser.GetStringBetween(grpData, 0, headingDD, headingDDEnd, null, out grpRunIdx);
                    if (grpRunIdx > 0)
                        cities.Add(city);
                }
                else
                {
                    var city = "dummy";
                    var grpIdx = 0;
                    var idx = grpData.IndexOf("Select City", grpIdx);
                    if (idx > 0)
                        grpIdx = idx;

                    while (grpRunIdx > 0)
                    {
                        // Go past Select City
                        city = StringParser.GetStringBetween(grpData, grpIdx, cityStart, cityEnd, null, out grpRunIdx);

                        if (grpRunIdx > 0)
                        {
                            cities.Add(city);
                            grpIdx = grpRunIdx;
                            if (idx > 0)
                                break;
                        }
                    }
                    //var count Regex.Matches( input,  "true" ).Count
                }
            }
            return cities;
        }
    }
}