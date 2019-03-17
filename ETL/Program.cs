using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ETL;
using log4net;
using WhatsRent.Common.Utilities;

namespace WhatsRent.ETL
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

                var logDir = Path.Combine(Config.ConfigMap["rootdir"], "etllogs");

                var log4netconfig = Path.Combine(Config.ConfigMap["rootdir"], "log4net.config");

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                var logfile = Path.Combine(logDir, "ETL");

                log4net.GlobalContext.Properties["logfile"] = logfile;
                log4net.Config.XmlConfigurator.Configure(new FileInfo(log4netconfig));

                Log.Info("Started ETL process");

                Log.Info(" =============== Configuration ================ ");
                Log.Info(configString);
                Log.Info(" =============== Configuration ================ ");

                CommonFloor.CF_RentPageETL.RunOnRentPages();
            }
            catch(Exception ex)
            {
                Log.Error("Error: " + ex.Message);
                Log.Error("Stacktrace: " + ex.StackTrace);
            }
        }
    }
}
