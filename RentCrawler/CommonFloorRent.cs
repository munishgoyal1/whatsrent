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
using System.Web;
using WhatsRent.DomainModel;
using System.Web.Script.Serialization;
using System.Configuration;
using WhatsRent.Common.Utilities;
using log4net;

namespace WhatsRent.Crawler.RentPageFetcher
{
    public class CommonFloorRent
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(CommonFloorRent));
        const string BASE_URL = @"http://www.commonfloor.com";
        const string CF_CITY_RENT_PROPERTY_LIST_URL = @"https://www.commonfloor.com/listing-search?search_intent=rent&page={0}&city={1}&srtby={2}&page_size=30";
        private static readonly string[] SrtBy = new string[] { "old", "recency", "image_count", "bestquality", "hightolow", "lowtohigh" };
        const string EndOfCityIndicator = "href=\"https://www.addtoany.com/share_save\"></a></li></ul></div></div></div></div></div><div class=\"row notification\"><div class=\"col-md-11 col-sm-11 col-xs-11\"><div style=\"display:none;font-weight:bold;text-align:center;\"> Oops!";


        static string ROOT_DIR;
        static string ROOT_PROPERTIES_DIR;
        static string REF_FILES_DIR;
        const string CITIES_REF_FILE = "AllCities.txt";
        const string CITYWISE_PROPERTY_URLS_REF_FILE = "NewPropertyURLs-{0}.txt";
        const string CITYWISE_PROPERTY_FILEPATHS_REF_FILE = "NewPropertyFilePaths-{0}.txt";
        const string CITIES_PROPERTY_SKIP_FILE = "1_SkipCities_properties.txt";
        const string CITIES_PROJECT_ERR_FILE = "1_{0}_CitiesErr_properties.txt";
        const string PROJECTS_ERR_FILE = "1_{0}_Errors_projects.txt";
        const string PROPERTY_CITY_ERR_FILE = "1_{0}_City_Errors_properties.txt";
        const string CITIES_PROPERTY_PAGEMAP_FILE = "1_{0}_CitiesPageMap_properties.txt";
        const string START_AFTER_CITY_PROPERTY_FILE = "1_Property_StartAfter_City.txt";

        static int webCallCntr = 0;
        static int newProjectsCntr = 0;
        static int newPropertiesCntr = 0;
        static int repeatPropertiesCntr = 0;
        static int newListingDerivedProjectsCntr = 0;
        static int repeatListingDerivedProjectsCntr = 0;

        static readonly string TODAYDATE = DateTime.Now.ToString("yyyymmdd");

        static CookieContainer mCookieContainer = new CookieContainer();
        static JavaScriptSerializer json_serializer = new JavaScriptSerializer();

        public static void RunCommonFloor()
        {
            ROOT_DIR = Config.ConfigMap["rootdir"];

            ROOT_PROPERTIES_DIR = ROOT_DIR + @"Properties\";
            REF_FILES_DIR = ROOT_DIR + @"1_REF_FILES\";

            if (!Directory.Exists(ROOT_DIR))
                Directory.CreateDirectory(ROOT_DIR);

            GetRentProperties(null);

            Log.Info(string.Format("Finished (Press any key to exit...)\nNumber of webcalls: {0}\nNew Projects: {1}\nNew Properties: {2}\nRepeat Properties: {3}",
              webCallCntr, newProjectsCntr, newPropertiesCntr, repeatPropertiesCntr));
            //Console.ReadKey();

            return;

            ParameterizedThreadStart ptsb = new ParameterizedThreadStart(GetRentProperties);
            Thread tb = new Thread(ptsb);
            tb.Start();

            //ParameterizedThreadStart ptsp = new ParameterizedThreadStart(GetProjects);
            //Thread tp = new Thread(ptsp);
            //tp.Start();

            tb.Join();
            //tp.Join();


        }

        // ====================================== PROJECTS =============================================

        static void GetRentProperties(object obj)
        {
            if (!Directory.Exists(ROOT_PROPERTIES_DIR))
                Directory.CreateDirectory(ROOT_PROPERTIES_DIR);

            if (!Directory.Exists(REF_FILES_DIR))
                Directory.CreateDirectory(REF_FILES_DIR);

            //var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            //var cities = File.ReadAllLines(citiesFile);

            var cities = Config.ConfigMap["cities"].Split(',');

            var skipCitiesFile = REF_FILES_DIR + CITIES_PROPERTY_SKIP_FILE;
            if (!File.Exists(skipCitiesFile))
                File.WriteAllText(skipCitiesFile, "");

            var skipCities = new HashSet<string>(File.ReadAllLines(skipCitiesFile));

            var startAfterCityFile = REF_FILES_DIR + START_AFTER_CITY_PROPERTY_FILE;
            if (!File.Exists(startAfterCityFile))
                File.WriteAllText(startAfterCityFile, "");

            var startAfterCity = File.ReadAllText(startAfterCityFile);
            bool isCityFound = false;
            List<string> AllCitiesNewPropertyUrls = new List<string>(50000);

            foreach (var city in cities)
            {
                if (!string.IsNullOrEmpty(startAfterCity) && city != startAfterCity && !isCityFound)
                    continue;
                else if (city == startAfterCity)
                    isCityFound = true;

                if (skipCities.Contains(city))
                    continue;

                List<string> newPropertyUrls;
                List<string> newPropertyFilePaths;

                GetCityRentData(city, out newPropertyUrls, out newPropertyFilePaths);

                AllCitiesNewPropertyUrls.AddRange(newPropertyUrls);

                // Write the urls of new properties found for this city
                var cityNewPropertyUrlsFile = REF_FILES_DIR + string.Format(CITYWISE_PROPERTY_URLS_REF_FILE, city);
                File.WriteAllText(cityNewPropertyUrlsFile, string.Join("\n", newPropertyUrls.ToArray()));

                // Write the file paths of new properties found for this city
                var cityNewPropertyFilePathsFile = REF_FILES_DIR + string.Format(CITYWISE_PROPERTY_FILEPATHS_REF_FILE, city);
                File.WriteAllText(cityNewPropertyFilePathsFile, string.Join("\n", newPropertyFilePaths.ToArray()));
            }

            var allCitiesNewPropertyUrlsFile = REF_FILES_DIR + string.Format(CITYWISE_PROPERTY_URLS_REF_FILE, "All");
            File.WriteAllText(allCitiesNewPropertyUrlsFile, string.Join("\n", AllCitiesNewPropertyUrls.ToArray()));
        }

        static void GetCityRentData(string city, out List<string> newPropertyUrls, out List<string> newPropertyFilePaths)
        {
            newPropertyUrls = new List<string>(1000);
            newPropertyFilePaths = new List<string>(1000);
            HashSet<string> projectIDs = new HashSet<string>();

            Log.Info(string.Format("Doing Rental Properties for {0}", city) + "....");

            // create city dir if absent
            var cityDir = Path.Combine(ROOT_PROPERTIES_DIR, city);
            if (!Directory.Exists(cityDir))
                Directory.CreateDirectory(cityDir);

            // create listings dir if absent
            var listingsDir = Path.Combine(cityDir, "Listings");
            if (!Directory.Exists(listingsDir))
                Directory.CreateDirectory(listingsDir);


            // create projects dir if absent
            var projectsDir = Path.Combine(cityDir, "Projects");
            if (!Directory.Exists(projectsDir))
                Directory.CreateDirectory(projectsDir);


            // create city standalone properties dir if absent
            var standalonePropDir = Path.Combine(listingsDir, "1-Standalone");
            if (!Directory.Exists(standalonePropDir))
                Directory.CreateDirectory(standalonePropDir);

            string allPropertiesMixPageData = null;
            var projectInListStart = "data-tracking-id=\"2039\"><a href=\"";
            var projectInListEnd = "\" target=\"_blank\">";

            var propInProjStart = "<a class=\"col-xs-12 cf-tracking-enabled\" href=\"";
            var propInProjEnd = "\" data-tracking-id=\"listing-snippet\" data-listingid";

            var propInListStart = "<div class=\"col-md-12 col-sm-12 col-xs-12 listing-title\"><h4><a href=\"";
            var propInListEnd = "\" target=\"_blank\">";

            foreach (var srtBy in SrtBy)
            {
                Log.Info(string.Format("Doing Rental Properties for {0} with srtby {1}", city, srtBy) + "....");
                var pageNum = 0;
                while (true)
                {
                    var cityPropPageUrl = string.Format(CF_CITY_RENT_PROPERTY_LIST_URL, ++pageNum, city, srtBy);

                    allPropertiesMixPageData = GetWebPage(cityPropPageUrl, null);

                    if (string.IsNullOrEmpty(allPropertiesMixPageData))
                    {
                        File.AppendAllText(REF_FILES_DIR + string.Format(PROPERTY_CITY_ERR_FILE, TODAYDATE), city + "\n");
                        Log.InfoFormat("Property page {0} empty for {1}. Finishing the city here with srtby {2}",
                            pageNum, city, srtBy);
                        break;
                    }

                    if (allPropertiesMixPageData.Contains(EndOfCityIndicator))
                    {
                        Log.InfoFormat(
                            "Property page {0} indicates end of city data for {1}. Finishing the city here with srtby {2}",
                            pageNum, city, srtBy);
                        break;
                    }

                    var tmpProjListPageIdx = 0;

                    // get projects on a page. prns-
                    while (true)
                    {
                        var propProjectRelativeLink = StringParser.GetStringBetween(allPropertiesMixPageData,
                            tmpProjListPageIdx, projectInListStart,
                            projectInListEnd, null, out tmpProjListPageIdx);

                        if (tmpProjListPageIdx < 0 || string.IsNullOrEmpty(propProjectRelativeLink))
                        {
                            Log.InfoFormat("Property page {0} finished for city {1}", pageNum, city);
                            break;
                        }

                        var fullProjectUrl = BASE_URL + propProjectRelativeLink;
                        var projectId = fullProjectUrl.Substring(fullProjectUrl.LastIndexOf("/") + 1);
                        var listingsProjectDir = Path.Combine(listingsDir, projectId.Last().ToString(), projectId);

                        Log.InfoFormat("Project #{3}: {0} on Page {1} for city {2}", projectId, pageNum, city, newProjectsCntr);

                        if (Directory.Exists(listingsProjectDir))
                        {
                            Log.InfoFormat("Project exists: {0} {1}", projectId, city);
                            continue;
                        }
                        else
                            Interlocked.Increment(ref newProjectsCntr);

                        var projPropertiesPageData = GetWebPage(fullProjectUrl, null);

                        Directory.CreateDirectory(listingsProjectDir);
                       
                        var tmpProjPageIdx = 0;

                        // get individual properties inside project
                        while (true)
                        {
                            var propRelativeLink = StringParser.GetStringBetween(projPropertiesPageData, tmpProjPageIdx,
                                propInProjStart,
                                propInProjEnd, null, out tmpProjPageIdx);

                            if (tmpProjPageIdx < 0 || string.IsNullOrEmpty(propRelativeLink))
                                break;

                            var fullPropertyUrl = BASE_URL + propRelativeLink;

                            var propertyId = fullPropertyUrl.Substring(fullPropertyUrl.LastIndexOf("/") + 1);
                            Log.InfoFormat("Property #{4}: {0} in Project {1} on Page {2} for city {3}", propertyId,
                                projectId, pageNum, city, newPropertiesCntr);
                            var fileName = Path.Combine(listingsProjectDir, propertyId + ".html");

                            if (File.Exists(fileName))
                            {
                                Interlocked.Increment(ref repeatPropertiesCntr);
                                Log.Info("Repeat property, total repetitions so far: " + repeatPropertiesCntr);
                                continue;
                            }

                            var propertyPageData = GetWebPage(fullPropertyUrl, null);

                            if (!string.IsNullOrEmpty(propertyPageData))
                            {
                                File.WriteAllText(fileName, propertyPageData);
                                Interlocked.Increment(ref newPropertiesCntr);
                                GetProjectPage(projectsDir, propertyPageData);
                                Log.Info("New property added, total new entries so far: " + newPropertiesCntr);
                            }

                            newPropertyFilePaths.Add(fileName);
                            newPropertyUrls.Add(fullPropertyUrl);
                        }
                    }

                    var tmpPropListPageIdx = 0;
                    // get individual properties on list page itself
                    while (true)
                    {
                        var propRelativeLink = StringParser.GetStringBetween(allPropertiesMixPageData,
                            tmpPropListPageIdx, propInListStart,
                            propInListEnd, null, out tmpPropListPageIdx);

                        if (tmpPropListPageIdx < 0 || string.IsNullOrEmpty(propRelativeLink))
                            break;

                        var fullPropertyUrl = BASE_URL + propRelativeLink;

                        var propertyId = fullPropertyUrl.Substring(fullPropertyUrl.LastIndexOf("/") + 1);
                        Log.InfoFormat("Standalone Property #{3}: {0} on Page {1} for city {2}", propertyId, pageNum,
                            city, newPropertiesCntr);

                        var standalonePropAlphabetDir = Path.Combine(standalonePropDir, propertyId.Substring(propertyId.Length - 2));
                        if (!Directory.Exists(standalonePropAlphabetDir))
                            Directory.CreateDirectory(standalonePropAlphabetDir);

                        var fileName = Path.Combine(standalonePropAlphabetDir, propertyId + ".html");

                        if (File.Exists(fileName))
                        {
                            Interlocked.Increment(ref repeatPropertiesCntr);
                            Log.Info("Repeat property, total repetitions so far: " + repeatPropertiesCntr);
                            continue;
                        }

                        var propertyPageData = GetWebPage(fullPropertyUrl, null);

                        if (!string.IsNullOrEmpty(propertyPageData))
                        {
                            File.WriteAllText(fileName, propertyPageData);
                            Interlocked.Increment(ref newPropertiesCntr);
                            GetProjectPage(projectsDir, propertyPageData);
                            Log.Info("New property added, total new entries so far: " + newPropertiesCntr);
                        }

                        newPropertyFilePaths.Add(fileName);
                        newPropertyUrls.Add(fullPropertyUrl);
                    }
                }
            }
        }

        static void GetProjectPage(string projectsDir, string propertyPageData)
        {
            var tmpIdx = 0;
            const string PropDescStart = "{\"flpBody\":";
            const string PropDescEnd = ",\"url\":";

            var propDescJsonBody = StringParser.GetStringBetween(propertyPageData,
                tmpIdx,
                PropDescStart,
                PropDescEnd, null, out tmpIdx);

            if (tmpIdx > 0 && !string.IsNullOrEmpty(propDescJsonBody))
            {
                propDescJsonBody += "}";

                var jsonDto = json_serializer.Deserialize<PropertyRentCFJsonDto>(propDescJsonBody);

                if (string.IsNullOrEmpty(jsonDto.project_url))
                    return;

                var fullProjectUrl = BASE_URL + jsonDto.project_url;

                var projectAlphabetDir = Path.Combine(projectsDir, jsonDto.property_id.Last().ToString());

                if (!Directory.Exists(projectAlphabetDir))
                    Directory.CreateDirectory(projectAlphabetDir);

                var fileName = Path.Combine(projectAlphabetDir, jsonDto.property_id + ".html");

                if (File.Exists(fileName))
                {
                    Interlocked.Increment(ref repeatListingDerivedProjectsCntr);
                    Log.Info("Repeat listing derived project, total repetitions so far: " + repeatListingDerivedProjectsCntr);
                    return;
                }

                var projectPageData = GetWebPage(fullProjectUrl, null);

                if (!string.IsNullOrEmpty(projectPageData))
                {
                    File.WriteAllText(fileName, projectPageData);
                    Interlocked.Increment(ref newListingDerivedProjectsCntr);
                    Log.Info("New listing derived project added, total new entries so far: " + newListingDerivedProjectsCntr);
                }
            }
        }

        static string GetWebPage(string url, string postdata = null)
        {
            var res = Interlocked.Increment(ref webCallCntr);
            Log.Info(webCallCntr + " Webcall Counter at: " + DateTime.Now.ToLongTimeString() + "\n" + url);

            int retryCnt = 0;
            string response = "";
            do
            {
                response = HttpHelper.GetWebPageResponse(url, postdata, null, mCookieContainer);
                retryCnt++;
            } while (string.IsNullOrEmpty(response) && retryCnt < 2);

            return response;
        }

        public static void Experiment(string[] args)
        {
            var base_url = @"http://www.commonfloor.com/apartments";
            var POSTDATA = "";
            //POSTDATA = POST_DATA_RAW.Replace("\r\n", "&").Replace(" ", "");

            var pgNum = 1;
            while (true)
            {
                var city = System.Web.HttpUtility.UrlEncode("Dadra & Nagar Haveli");
                var postdata = string.Format(POSTDATA, city, pgNum++);
                var data = HttpHelper.GetWebPageResponse(base_url, postdata, null, mCookieContainer);
            }
        }
    }
}