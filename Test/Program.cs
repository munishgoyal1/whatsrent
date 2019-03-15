using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            const string CONNECTION_STRING = @"Data Source = MUNISH-LENOVO; Initial Catalog = WhatsRent; Integrated Security = True";
            // var svc = new WhatsRent.DataServices.RentService.RentService(CONNECTION_STRING);

            //  var results = svc.GetRents("Pune", "1000");
            MoveToDoubleLetterSubDirectories();

        }

        static void MoveToDoubleLetterSubDirectories()
        {
            return;
            var srclistingsDir = @"D:\WhatsRent\Data\Rent\CommonFloor\Properties\*\Listings\1-Standalone";

            //var listingsDir = @"D:\WhatsRent\Data\Rent\CommonFloor\Properties\Pune\Listings";
            //var standaloneListingsDir = @"D:\WhatsRent\Data\Rent\CommonFloor\Properties\Pune\Listings\1-Standalone";

            string pattern = srclistingsDir;
            var matches = Directory.GetFiles(@"D:\WhatsRent\Data\Rent\CommonFloor\Properties", "*.*", SearchOption.AllDirectories)
                .Where(path => Regex.Match(path, ".*1-Standalone.*").Success);

            //var filePaths = Directory.EnumerateFiles(srclistingsDir, "*", SearchOption.AllDirectories);

            foreach (var filePath in matches)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var dirname = Path.GetDirectoryName(filePath);
                var parentDir = Directory.GetParent(dirname);

                var twochardirname = fileName.Substring(fileName.Length - 2, 2);
                var dir = Path.Combine(parentDir.FullName, twochardirname);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var destFilePath = Path.Combine(dir, Path.GetFileName(filePath));
                File.Move(filePath, destFilePath);

                if (Directory.GetFiles(dirname).Length == 0 && Directory.GetDirectories(dirname).Length == 0)
                {
                    Directory.Delete(dirname, false);
                }
            }
        }

        static void MoveToSingleLetterSubDirectories()
        {
            return;
            var srclistingsDir = @"D:\WhatsRent\Data\Rent\CommonFloor\Properties\Pune1\Listings";

            var listingsDir = @"D:\WhatsRent\Data\Rent\CommonFloor\Properties\Pune\Listings";
            var standaloneListingsDir = @"D:\WhatsRent\Data\Rent\CommonFloor\Properties\Pune\Listings\1-Standalone";

            var filePaths = Directory.EnumerateFiles(srclistingsDir, "*", SearchOption.AllDirectories);

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                string lastFolderName = Path.GetFileName(Path.GetDirectoryName(filePath));
                string dir;
                if (filePath.Contains("1-Standalone"))
                    dir = Path.Combine(standaloneListingsDir, fileName.Last().ToString());
                else
                    dir = Path.Combine(listingsDir, lastFolderName.Last().ToString(), lastFolderName);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var destFilePath = Path.Combine(dir, Path.GetFileName(filePath));

                File.Copy(filePath, destFilePath);
            }
        }
    }
}
