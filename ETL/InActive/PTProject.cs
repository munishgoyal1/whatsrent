using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;

namespace ETL
{
    public class ETLPTProject
    {

        // Read all files in all folders
        // Read 1 file at a time and parse the fields out in the project data class
        // Add each project to the collection, at the end insert full collection into table
        // and also write the collection to csv
        // While inserting check if overwrite mode is enabled then update or ignore. if absent then insert

        const string SRC_PATH = @"D:\CocuData\Data\PropTiger\Projects";
        const string DIR_EXCLUDE = "1_REF_FILES";

        static List<BrokerData> BrokerDataList = new List<BrokerData>(25000);
        static HashSet<string> BrokerIdSet = new HashSet<string>();

        public static void ParseProjects()
        {
            var cityPaths = Directory.EnumerateFiles(SRC_PATH, "*", SearchOption.AllDirectories);

            //var stocks = new List<string>(cityPaths);
            Dictionary<string, int> cityCounter = new Dictionary<string, int>(100);
            foreach (string projectPath in cityPaths)
            {
                var splitPath = Path.GetDirectoryName(projectPath).Split(Path.DirectorySeparatorChar);
                var city = splitPath[5];
                if (city == DIR_EXCLUDE)
                    continue;
                if (!cityCounter.ContainsKey(city))
                    cityCounter.Add(city, 0);
                cityCounter[city] += 1;

                if (cityCounter[city] > 5)
                    continue;
                var type = splitPath.Length > 6 ? splitPath[6] : "";
                var projectId = splitPath.Length > 7 ? splitPath[7] : Path.GetFileName(projectPath);


                projectId = projectId.Contains(".") ? projectId.Remove(projectId.IndexOf('.')) : projectId;

                // Read the file text 
                var projPage = File.ReadAllText(projectPath);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(projPage);

                //var root = doc.DocumentNode;
                //var aTags = doc.DocumentNode.SelectNodes("//a");

                // Name, Area, City
                var res = doc.DocumentNode.SelectSingleNode("//div[@class='flt newLaunchProject']");
                //var content1 = firstDiv.ChildNodes[0].InnerText.Trim();
                var text = res.InnerText;
                //var split = text.Split(new char []{'\n', '\r'});
                var split = text.Split(new string[] { "\r\n", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                split[4] = split[4].Trim();
                split[8] = split[8].Replace(',',' ').Trim();
                split[9] = split[9].Trim();
                // split[4] = project name, split[8] = area, split[9] = city

                // Project Area, type of units, construction status, availability, launch, possession dates
                res = doc.DocumentNode.SelectSingleNode("//div[@class='detailnewtext clear paddingTop10']");
                text = res.InnerText;
                split = text.Split(new string[] { "\r\n", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);


                //
                res = doc.DocumentNode.SelectSingleNode("//div[@class='projectDetailText clear']");
                text = res.InnerText;
                split = text.Split(new string[] { "\r\n", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                var desc = string.Join(" ", split).Replace("Less view ...Read More", "").Trim();


                var firstDiv = res.SelectSingleNode("div");
                //var content2 = firstDiv.ChildNodes[1].InnerText.Trim();

                

                //int counter = 1;
                //string label = "";
                //if (aTags != null)
                //{
                //    foreach (var aTag in aTags)
                //    {
                //        try
                //        {
                //            label = counter + ". " + aTag.InnerHtml + " - " + aTag.Attributes["href"].Value + "\t" + "<br />";
                //            counter++;
                //            Console.WriteLine(label);
                //        }
                //        catch (Exception ex)
                //        {
                //            Console.WriteLine(ex.Message);
                //        }
                //    }

            }
            Console.ReadKey();
        }

        public static string ParseBrokerType1(string brokPage)
        {
            return null;

        }

        public static string ParseBrokerType2(string brokPage)
        {
            return null;

        }


    }
}