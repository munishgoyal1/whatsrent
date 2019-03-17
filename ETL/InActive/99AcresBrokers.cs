using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ETL
{
    public class BrokerData
    {
        public string BrokerId;
        public string BusinessName;
        public string ContactPerson;
        public string Address;
        public string City;
        public string Phone1;
        public string Phone2;
        public string Phone3;
        public string Desc;
        public string Tags;
        public string Remarks;
        public string DealsIn;
        public string Localities;
        public string Status;
        public string Source;
        public DateTime DateAdded;
        public DateTime DateModified;
    }

    public class Acres99ETL
    {

        // Read all files in all folders
        // Read 1 file at a time and parse the fields out in the broker data class
        // Add each broker to the collection, at the end insert full collection into table
        // and also write the collection to csv
        // While inserting check if overwrite mode is enabled then update or ignore. if absent then insert

        const string SRC_PATH = @"D:\CocuData\Data\99acres\Brokers";
        const string DIR_EXCLUDE = "1_REF_FILES";
        const string BROK_TYPE1 = @"currentPageName='dif_landing_page';";
        const string BROK_TYPE2 = @"currentPageName='QS';";

        static List<BrokerData> BrokerDataList = new List<BrokerData>(25000);
        static HashSet<string> BrokerIdSet = new HashSet<string>();

        public static void ParseBrokers()
        {
            var cityPaths = Directory.EnumerateFiles(SRC_PATH, "*", SearchOption.AllDirectories);

            //var stocks = new List<string>(cityPaths);

            foreach (string brokerPath in cityPaths)
            {
                var splitPath = Path.GetDirectoryName(brokerPath).Split(Path.DirectorySeparatorChar);
                var city = splitPath[splitPath.Length - 2];

                if (city == DIR_EXCLUDE)
                    continue;

                string brokerId = Path.GetFileName(brokerPath);
                brokerId = brokerId.Remove(brokerId.IndexOf('.')); // Remove extension

                var broker = new BrokerData();
                broker.BrokerId = brokerId;

                // parse other data
                // Read the file text 
                var brokPage = File.ReadAllText(brokerPath);

                if (brokPage.Contains(BROK_TYPE1))
                    ParseBrokerType1(brokPage);
                else if (brokPage.Contains(BROK_TYPE2))
                    ParseBrokerType2(brokPage);

            }
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
