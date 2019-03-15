using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using HttpLibrary;

namespace ImageFetcher
{
    public class PT_Images
    {
        // Read all files in all folders
        // Read 1 file at a time and parse the fields out in the project data class
        // Add each project to the collection, at the end insert full collection into table
        // and also write the collection to csv
        // While inserting check if overwrite mode is enabled then update or ignore. if absent then insert

        const string SRC_PATH = @"D:\CocuData\Data\PropTiger\Projects";
        const string BASE_SITE_PATH = @"D:\CocuData\Data\PropTiger";
        const string IMAGES_PATH = @"D:\CocuData\Data\PropTiger\Images";
        const string IMG_BASE_PATH = @"http://cdn.proptiger.com/images/";
        static readonly int IMG_BASE_PATH_LEN = IMG_BASE_PATH.Length;
        //const string DIR_EXCLUDE = "1_PROJECT_REF_FILES";
        const string IMAGES_REF_DIR = "2_PROJECT_IMAGES_REF_FILES";
        const string UNKNOWN_BUILDER_DIR = "UnknownBuilder";
        const string UNKNOWN_PROJECT_DIR = "UnknownProject";
        static int projCounter = 0;
        static int imgCounter = 0;

        public static void ParseProjects()
        {
            var cityPaths = Directory.EnumerateFiles(SRC_PATH, "*", SearchOption.AllDirectories);
            HashSet<string> IgnoreImageSet = new HashSet<string>();
            //var stocks = new List<string>(cityPaths);
            Dictionary<string, int> cityCounter = new Dictionary<string, int>(100);
            foreach (string projectPath in cityPaths)
            {
                try
                {
                    var splitPath = Path.GetDirectoryName(projectPath).Split(Path.DirectorySeparatorChar);
                    var city = splitPath[5];
                    //if (city == DIR_EXCLUDE)
                    //    continue;
                    if (!cityCounter.ContainsKey(city))
                        cityCounter.Add(city, 0);
                    cityCounter[city] += 1;

                    //if (cityCounter[city] > 5)
                    //    continue;
                    var type = splitPath.Length > 6 ? splitPath[6] : "";
                    var projectId = splitPath.Length > 7 ? splitPath[7] : Path.GetFileName(projectPath);

                    projectId = projectId.Contains(".") ? projectId.Remove(projectId.IndexOf('.')) : projectId;

                    // Read the file text 
                    var projPage = File.ReadAllText(projectPath);

                    string pattern = @"http:\/\/cdn\.proptiger\.com\/images([A-Za-z0-9\-\/]+)\.jpg";
                    Regex rgx = new Regex(pattern);

                    string projName = null;
                    string builderName = null;
                    string projDir = null;

                    string projNameThisPage = null;
                    string builderNameThisPage = null;
                    string projDirThisPage = null;

                    Console.WriteLine(string.Format("{0}: Doing {1} project in {2} city", ++projCounter, projectId, city));

                    foreach (Match match in rgx.Matches(projPage))
                    {

                        // Console.WriteLine("Found '{0}' at position {1}", match.Value, match.Index);
                        var uri = match.Value;

                        if (IgnoreImageSet.Contains(uri))
                            continue;

                        var fileName = Path.GetFileName(uri);
                        var partPath = uri.Substring(IMG_BASE_PATH_LEN);
                        var split = partPath.Split(new char[] { '/' });

                        if (split.Length < 3)
                        {
                            IgnoreImageSet.Add(uri);
                            continue;
                        }

                        builderName = split[0];
                        projName = split[1];
                        if (string.IsNullOrEmpty(builderName))
                            builderName = UNKNOWN_BUILDER_DIR;
                        if (string.IsNullOrEmpty(projName))
                            projName = UNKNOWN_PROJECT_DIR;
                        projDir = Path.Combine(IMAGES_PATH, builderName, projName);
                        if (!Directory.Exists(projDir))
                            Directory.CreateDirectory(projDir);

                        // Happens once per project page

                        if (string.IsNullOrEmpty(projDirThisPage))
                        {
                            if (string.IsNullOrEmpty(builderNameThisPage))
                                builderNameThisPage = split[0];
                            if (string.IsNullOrEmpty(projNameThisPage))
                                projNameThisPage = split[1];

                            if (string.IsNullOrEmpty(builderNameThisPage))
                                builderNameThisPage = UNKNOWN_BUILDER_DIR;

                            if (string.IsNullOrEmpty(projNameThisPage))
                                projNameThisPage = UNKNOWN_PROJECT_DIR;

                            projDirThisPage = Path.Combine(IMAGES_PATH, builderNameThisPage, projNameThisPage);

                            if (!Directory.Exists(projDirThisPage))
                                Directory.CreateDirectory(projDirThisPage);
                            var projFileName = Path.Combine(projDirThisPage, string.Format("{0}_{1}.txt", city, projectId));
                            File.WriteAllText(projFileName, city + ":" + projectId);
                        }

                        var filePath = Path.Combine(projDir, fileName);

                        var info = string.Format("{0}: Getting {1} image for {2}:{3}:{4} project", ++imgCounter, uri, projName, builderName, city);
                        Console.WriteLine(info);

                        try
                        {
                            if (!File.Exists(filePath))
                                HttpHelper.DownloadRemoteFile(uri, filePath);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Path.Combine(BASE_SITE_PATH, IMAGES_REF_DIR, "AllExceptions.txt"), DateTime.Now.ToLongTimeString() + info + "\n" + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Path.Combine(BASE_SITE_PATH, IMAGES_REF_DIR, "AllExceptions.txt"), DateTime.Now.ToLongTimeString() + projectPath + "\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }

            File.WriteAllLines(Path.Combine(BASE_SITE_PATH, IMAGES_REF_DIR, "IgnoredImages.txt"), IgnoreImageSet);
            File.WriteAllLines(Path.Combine(BASE_SITE_PATH, IMAGES_REF_DIR, "CitywiseProjectCounter.txt"), cityCounter.ToList().Select(kv => kv.Key + " : " + kv.Value));
        }

        public static void RunPTImages()
        {
            try
            {
                ParseProjects();
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(BASE_SITE_PATH, IMAGES_REF_DIR, "TerminatingException.txt"), DateTime.Now.ToString() + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
