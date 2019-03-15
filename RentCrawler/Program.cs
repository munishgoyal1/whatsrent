using System;
using System.Configuration;
using System.IO;
using System.Threading;
using log4net;
using WhatsRent.Common.Utilities;
using WhatsRent.Crawler.RentPageFetcher;

namespace Runner
{
    class Program
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            try
            {
                Config.ConfigMap = Utilities.GetConfig(args[0]);
                var configString = File.ReadAllText(args[0]);

                var logDir = Path.Combine(Config.ConfigMap["rootdir"], "crawlerlogs");

                var log4netconfig = Path.Combine(Config.ConfigMap["rootdir"], "log4net.config");

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                var logfile = Path.Combine(logDir, "RentPageFetcher");

                log4net.GlobalContext.Properties["logfile"] = logfile;
                log4net.Config.XmlConfigurator.Configure(new FileInfo(log4netconfig));

                Log.Info("Data Fetcher process started");

                Log.Info(" =============== Configuration ================ ");
                Log.Info(configString);
                Log.Info(" =============== Configuration ================ ");

                RunCrawl();

                Log.Info("Data Fetcher process ended");
            }
            catch(Exception ex)
            {
                Log.Error("Error: " + ex.Message);
                Log.Error("Stacktrace: " + ex.StackTrace);
            }
        }

        //static void RunImages(string[] args)
        //{
        //    if(args.Length == 0)
        //        CF_Builders.RunCFBuilders();

        //    else
        //    {
        //        switch (int.Parse(args[0]))
        //        {
        //            case 0: PT_Images.RunPTImages(); break;
        //            case 1: CF_Images.RunCFImages(); break;
        //            case 2: CF_Builders.RunCFBuilders(); break;
        //        }
        //    }
        //}

        static void RunETL()
        {
            //Acres99ETL.ParseBrokers();
            //ETLPTProject.ParseProjects();

        }

        static void RunCrawl()
        {
            //MagicBricks.RunMagicBricks(args);
            //Acres99.Run99Acres(args);
            //JustDial.RunJustDial(args);
            //PropTiger.RunPropTiger(args);
         
            CommonFloorRent.RunCommonFloor();
            //CommonFloor.Experiment(args);
        }
    }
}
