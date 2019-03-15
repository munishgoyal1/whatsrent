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
    public class PropTiger
    {
        const string ROOT_DIR = @"D:\CocuData\Data\PropTiger\";
        const string CITIES_REF_FILE = "AllCities.txt";
        const string VILLA = "villas-sale";
        const string APT = "apartments-flats-sale";
        const string PLOT = "sites-plots-sale";

        static Dictionary<string, string> PropTypeMap = new Dictionary<string, string> {
            {VILLA, "Villa"}, {APT, "Apartment"}, {PLOT, "Plot"}};

        const string BASE_URL = @"http://www.proptiger.com/";
        const string PT_PROJECT_LIST_MAIN_URL = @"http://www.proptiger.com/{0}/{1}#start={2}";
        const string PT_PROJECT_LIST_URL = @"http://www.proptiger.com/search_list.php?city[]={0}&bed=0&budget=0,0&q=CITY:{0}&propertyType[]={1}&start={2}";
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
        static readonly string TODAYDATE = DateTime.Now.ToString("yyyymmdd");


        static CookieContainer mCookieContainer = new CookieContainer();

        static string GetWebPage(string url)
        {
            var res = Interlocked.Increment(ref webCallCntr);
            Console.WriteLine(webCallCntr + " Webcall Counter at: " + DateTime.Now.ToLongTimeString() + "\n" + url);

            int retryCnt = 0;
            string response = "";
            do
            {
                response = HttpHelper.GetWebPageResponse(url, null, null, mCookieContainer);
                retryCnt++;
            } while (string.IsNullOrEmpty(response) && retryCnt < 2);

            return response;
        }

        public static void RunPropTiger(string[] args)
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
            //var cities = GetMBCities();
            //var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            //File.WriteAllText(citiesFile, string.Join("\n", cities.ToArray()));
            //return;

            GetProjects(null);
            return;

            ParameterizedThreadStart ptsb = new ParameterizedThreadStart(GetProjects);
            Thread tb = new Thread(ptsb);
            tb.Start();

            //ParameterizedThreadStart ptsp = new ParameterizedThreadStart(GetProjects);
            //Thread tp = new Thread(ptsp);
            //tp.Start();

            tb.Join();
            //tp.Join();

            Console.WriteLine(string.Format("Finished (Press any key to exit...)\nNumber of webcalls: {0}\nNew Brokers: {1}\nNew Projects: {2}",
                webCallCntr, newBrokersCntr, newProjectsCntr));
            Console.ReadKey();
        }

        // ====================================== PROJECTS =============================================

        static void GetProjects(object obj)
        {
            if (!Directory.Exists(ROOT_PROJECT_DIR))
                Directory.CreateDirectory(ROOT_PROJECT_DIR);

            if (!Directory.Exists(REF_FILES_PROJECT_DIR))
                Directory.CreateDirectory(REF_FILES_PROJECT_DIR);

            var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            var cities = File.ReadAllLines(citiesFile);

            var skipCitiesFile = REF_FILES_PROJECT_DIR + CITIES_PROJECT_SKIP_FILE;
            if (!File.Exists(skipCitiesFile))
                File.WriteAllText(skipCitiesFile, "");

            var skipCities = new HashSet<string>(File.ReadAllLines(skipCitiesFile));

            var startAfterCityFile = REF_FILES_PROJECT_DIR + START_AFTER_CITY_PROJECT_FILE;
            if (!File.Exists(startAfterCityFile))
                File.WriteAllText(startAfterCityFile, "");

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

                var projectsVilla = GetCityProjectData(city, VILLA);
                var projectsApt = GetCityProjectData(city, APT);
                var projectsPlot = GetCityProjectData(city, PLOT);

                ProjectUrls.AddRange(projectsVilla);
                ProjectUrls.AddRange(projectsApt);
                ProjectUrls.AddRange(projectsPlot);

                AllProjectUrls.AddRange(projectsVilla);
                AllProjectUrls.AddRange(projectsApt);
                AllProjectUrls.AddRange(projectsPlot);

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

            Console.WriteLine(string.Format("Doing {0} Projects for {1}", type, city) + "....");

            var cityInUrl = city.Replace(" ", "-");

            // create city dir if absent
            var cityDir = Path.Combine(ROOT_PROJECT_DIR, city, PropTypeMap[type]);
            if (!Directory.Exists(cityDir))
                Directory.CreateDirectory(cityDir);

            var cityUrl = string.Format(PT_PROJECT_LIST_MAIN_URL, cityInUrl, type, 1).ToLower();
            var citypgdata = GetWebPage(cityUrl);

            if (string.IsNullOrEmpty(citypgdata))
            {
                File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(PROJECTS_CITY_ERR_FILE, TODAYDATE), city + "\n");
                Console.WriteLine(string.Format("Project page {0} empty for {1}. Skipping city", 1, city));
                return projectUrls;
            }

            //const string PAGE_NOT_FOUND_ERR = "Sorry! No properties were found for the filters applied by you";
            //if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
            //    return projectUrls;


            // Get last pagenumber
            //</strong> out of <strong>9319</strong>
            string pgNumStart = "</strong> out of <strong>";
            string pgNumEnd = "</strong>";
            var tmpIdx = 0;
            var pgIdx = 0;
            var tmpStr = StringParser.GetStringBetween(citypgdata, pgIdx, pgNumStart, pgNumEnd, null, out tmpIdx);

            if (tmpIdx > 0 && !string.IsNullOrEmpty(tmpStr))
            {
                //var lastPageNum = int.Parse(tmpStr.Substring(tmpStr.IndexOf("page-") + 5)) + 1;
                var totalPages = int.Parse(tmpStr);
                var lastPageNum = totalPages == 0 ? 0 : totalPages / 100 + 1;
                int pageNum = 1;

                while (pageNum <= lastPageNum)
                {
                    Console.WriteLine(string.Format("Projects:{0} Page{1}", city, pageNum));
                    cityUrl = string.Format(PT_PROJECT_LIST_URL, cityInUrl, PropTypeMap[type], pageNum++);//.ToLower();
                    citypgdata = GetWebPage(cityUrl);

                    if (string.IsNullOrEmpty(citypgdata))
                    {
                        File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_ERR_FILE, TODAYDATE), cityUrl + "\n");
                        Console.WriteLine(string.Format("Project page {0} empty for {1}. Skipping page", pageNum, city));
                        continue;
                    }

                    //if (citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                    //{
                    //    File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, projectIDs.Count));
                    //    break;
                    //}

                    // Get project url
                    ///do/dealer_profile/searchProfile?
                    //const string projNear1 = "PROJECT_URL\":\"";
                    const string projStart = "PROJECT_URL\":\"";
                    const string projEnd = "\"";

                    var projectUrl = "dummy";
                    var projIdx = 0;
                    var projRunIdx = 1;

                    while (projRunIdx > 0)
                    {
                        //var projNearIdx = citypgdata.IndexOf(projNear1, projIdx);
                        //if (projNearIdx < 0) break;

                        //projIdx = projNearIdx;
                        projectUrl = StringParser.GetStringBetween(citypgdata, projIdx, projStart, projEnd, null, out projRunIdx);

                        if (projRunIdx < 0 || string.IsNullOrEmpty(projectUrl))
                            break;

                        projectUrl = BASE_URL + projectUrl.Replace("\\","");
                        projIdx = projRunIdx;

                        // Get project id
                        var projectId = projectUrl.Substring(projectUrl.LastIndexOf("-") + 1);

                        // Get project page and save it
                        tmpIdx = 0;
                        if (!projectIDs.Contains(projectId))
                        {
                            projectIDs.Add(projectId);
                            var projDir = Path.Combine(cityDir, projectId);
                            if (!Directory.Exists(projDir))
                                Directory.CreateDirectory(projDir);

                            var fileName = projDir + "\\" + projectId + ".html";

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
                File.AppendAllText(REF_FILES_PROJECT_DIR + string.Format(CITIES_PROJECT_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, projectIDs.Count));
            }
            return projectUrls;
        }

        static IEnumerable<string> GetMBCities()
        {
            const string citiesUrl = @"http://www.proptiger.com/all-cities";
            var pgData = GetWebPage(citiesUrl);
        
            File.WriteAllText(ROOT_DIR + "CitiesPage.html", pgData);
            //rgb(236, 236, 236)"><a href=  "/agra-real-estate">Agra Real Estate</a></td>
            //  "/agra-real-estate">Agra Real Estate
            var str1 = "rgb(236, 236, 236)\"><a href=";
            var strEnd = "</a></td>";
            var cityStart = ">";
            var cityEnd = " Real Estate";
            int pgRunIdx = 1;
            int pgIdx = 0;
            List<string> cities = new List<string>(500);

            do
            {
                string cityData = StringParser.GetStringBetween(pgData, pgIdx, str1, strEnd, null, out pgRunIdx);

                if (pgRunIdx > 0 && !String.IsNullOrEmpty(cityData))
                {
                    var grpRunIdx = 1;
                    var city = StringParser.GetStringBetween(cityData, 0, cityStart, cityEnd, null, out grpRunIdx);

                    if (grpRunIdx < 0 || string.IsNullOrEmpty(city))
                        break;

                    cities.Add(city);
                }
                pgIdx = pgRunIdx+1;

            } while (pgRunIdx > 0);

            return cities;
        }

        public static void Experiment(string[] args)
        {
            var base_url = @"http://www.proptiger.com/search_list.php?city[]={0}&bed=0&budget=0,0&q=CITY:{0}&propertyType[]={1}&start={2}";

            var pgNum = 1;
            while (true)
            {
                var url = string.Format(base_url, "gurgaon", "Apartment", pgNum++);
                var data = GetWebPage(url);
            }
        }
    }
}