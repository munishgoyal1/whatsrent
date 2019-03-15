using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using HttpLibrary;

namespace ImageFetcher
{
    public class CF_Builders
    {
        // Read all files in all folders
        // Read 1 file at a time and parse the fields out in the project data class
        // Add each project to the collection, at the end insert full collection into table
        // and also write the collection to csv
        // While inserting check if overwrite mode is enabled then update or ignore. if absent then insert
        const string CF_BASE_URL = @"http://www.commonfloor.com";
        const string SRC_PATH = @"D:\CocuData\Data\CommonFloor\Projects";
        const string BASE_SITE_PATH = @"D:\CocuData\Data\CommonFloor";
        const string BUILDERS_PATH = @"D:\CocuData\Data\CommonFloor\Builders";
        //const string DIR_EXCLUDE = "1_PROJECT_REF_FILES";
        const string BUILDERS_REF_DIR = "1_BUILDER_REF_FILES";
        const string UNKNOWN_BUILDER_DIR = "UnknownBuilder";
        const string UNKNOWN_PROJECT_DIR = "UnknownProject";
        static int projCounter = 0;
        static int bldrCounter = 0;

        public static void ParseProjects()
        {
            var cityPaths = Directory.EnumerateFiles(SRC_PATH, "*", SearchOption.AllDirectories);
            HashSet<string> IgnoreImageSet = new HashSet<string>();
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

                    string pattern = @"href='\/([A-Za-z0-9\-\/]+)\/builder\/([A-Za-z0-9]+)'";
                    Regex rgx = new Regex(pattern);

                    string projDir = null;

                    Console.WriteLine(string.Format("{0}: Doing {1} project in {2} city", ++projCounter, projectId, city));
                    var matches = rgx.Matches(projPage);

                    if (matches.Count == 0)
                        continue;

                    var match = matches[0];

                    var uri = match.Value;
                    uri = uri.TrimEnd(new char[] { '\'' });
                    uri = uri.Replace("href='", CF_BASE_URL);

                    var split = uri.Split(new char[] { '/' });

                    if (split.Length < 4)
                    {
                        IgnoreImageSet.Add(uri);
                        Console.WriteLine(string.Format("Ignored: {0}", uri));
                        continue;
                    }

                    var builderName = split[split.Length - 3];
                    var builderId = split[split.Length - 1];


                    projDir = Path.Combine(BUILDERS_PATH, city, builderId);
                    if (!Directory.Exists(projDir))
                        Directory.CreateDirectory(projDir);

                    var filePath = Path.Combine(projDir, builderId + ".html");
                    var info = string.Format("{0}: Getting {1} Builder Page for {2}:{3}:{4}:{5} project", ++bldrCounter, uri, projectId, builderName, builderId, city);
                    Console.WriteLine(info);

                    try
                    {
                        if (!File.Exists(filePath))
                            HttpHelper.DownloadRemoteFile(uri, filePath);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Path.Combine(BASE_SITE_PATH, BUILDERS_REF_DIR, "AllExceptions.txt"), DateTime.Now.ToLongTimeString() + info + "\n" + ex.Message + "\n" + ex.StackTrace);
                    }

                    var builderPage = File.ReadAllText(filePath);
                    pattern = @"\/public\/images\/builder\/([A-Za-z0-9\-\/]+)\.(gif|jpg)";
                    rgx = new Regex(pattern);
                    matches = rgx.Matches(builderPage);

                    if (matches.Count == 0)
                        continue;
                    match = matches[0];
                    uri = CF_BASE_URL + match.Value;
                    var extn = Path.GetExtension(uri);
                    filePath = Path.Combine(projDir, builderId + extn);
                    try
                    {
                        if (!File.Exists(filePath))
                            HttpHelper.DownloadRemoteFile(uri, filePath);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Path.Combine(BASE_SITE_PATH, BUILDERS_REF_DIR, "AllExceptions.txt"), DateTime.Now.ToLongTimeString() + info + "\n" + ex.Message + "\n" + ex.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Path.Combine(BASE_SITE_PATH, BUILDERS_REF_DIR, "AllExceptions.txt"), DateTime.Now.ToLongTimeString() + projectPath + "\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }

            File.WriteAllLines(Path.Combine(BASE_SITE_PATH, BUILDERS_REF_DIR, "IgnoredImages.txt"), IgnoreImageSet);
            File.WriteAllLines(Path.Combine(BASE_SITE_PATH, BUILDERS_REF_DIR, "CitywiseProjectCounter.txt"), cityCounter.ToList().Select(kv => kv.Key + " : " + kv.Value));
        }

        public static void RunCFBuilders()
        {
            try
            {
                ParseProjects();
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(BASE_SITE_PATH, BUILDERS_REF_DIR, "TerminatingException.txt"), ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
