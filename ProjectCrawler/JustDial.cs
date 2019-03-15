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

namespace PageFetcher
{
    public class JustDial
    {
        const string ROOT_DIR = @"D:\CocuData\Data\justdial\";
        const string CITIES_REF_FILE = "AllCities.txt";
        const string ALL_RES = "";
        const string ALL_COM = "commercial-";

        const string A99_AGENT_LIST_URL = @"http://www.justdial.com/{0}/estate-agents/page-{1}";
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

        public static void RunJustDial(string[] args)
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

            //ParameterizedThreadStart ptsp = new ParameterizedThreadStart(GetProjects);
            //Thread tp = new Thread(ptsp);
            //tp.Start();

            tb.Join();
            //tp.Join();

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
                //var brokersCom = GetCityBrokerData(city, ALL_COM);
                BrokerUrls.AddRange(brokersRes);
                //BrokerUrls.AddRange(brokersCom);
                AllBrokerUrls.AddRange(brokersRes);
                //AllBrokerUrls.AddRange(brokersCom);

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

            var cityUrl = string.Format(A99_AGENT_LIST_URL, cityInUrl, 1).ToLower();
            var citypgdata = GetWebPage(cityUrl);

            if (string.IsNullOrEmpty(citypgdata))
            {
                File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(BROKERS_CITY_ERR_FILE, TODAYDATE), city + "\n");
                Console.WriteLine(string.Format("Broker page {0} empty for {1}. Skipping city", 1, city));
                return brokerUrls;
            }

            const string PAGE_NOT_FOUND_ERR = "Estd.";
            if (!citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                return brokerUrls;

            int pageNum = 1;

            while (true)
            {
                Console.WriteLine(string.Format("Brokers:{0} Page{1}", city, pageNum));
                cityUrl = string.Format(A99_AGENT_LIST_URL, cityInUrl, pageNum++).ToLower();
                citypgdata = GetWebPage(cityUrl);

                if (string.IsNullOrEmpty(citypgdata))
                {
                    File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(CITIES_BROKER_ERR_FILE, TODAYDATE), cityUrl + "\n");
                    Console.WriteLine(string.Format("Broker page {0} empty for {1}. Skip to next city", pageNum, city));
                    break;
                }

                if (!citypgdata.Contains(PAGE_NOT_FOUND_ERR))
                {
                    File.AppendAllText(REF_FILES_BROKER_DIR + string.Format(CITIES_BROKER_PAGEMAP_FILE, TODAYDATE), string.Format("{0};{1};{2}\n", city, pageNum - 1, brokerIDs.Count));
                    break;
                }

                // Get broker url
                const string brokBlockStart = "jcnwrp";
                const string brokBlockEnd = "jrcw";
                const string brokStart = "a href=\"";
                const string brokEnd = "\" title";
                var brokIdx = 0;
                var brokRunIdx = 1;
                var tmpIdx = 0;

                while (brokRunIdx > 0)
                {
                    var brokerUrl = StringParser.GetStringBetween(citypgdata, brokIdx, brokBlockStart, brokBlockEnd, null, out brokRunIdx);
                    brokerUrl = StringParser.GetStringBetween(brokerUrl ?? "", 0, brokStart, brokEnd, null, out tmpIdx);

                    if (brokRunIdx > 0 && !string.IsNullOrEmpty(brokerUrl))
                    {
                        var brokerId = brokerUrl.Substring(brokerUrl.LastIndexOf("/")+1).Split(new char[] { '_' })[0];
                        //var brokerId = StringParser.GetStringBetween(brokerUrl ?? "", 0, "XX11-", "-", null, out tmpIdx);
                        brokIdx = brokRunIdx;
                        //brokerId = fwdSlashIdx;
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
            return brokerUrls;
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