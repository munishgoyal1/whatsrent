using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            const string CONNECTION_STRING = @"Data Source = MUNISH-LENOVO; Initial Catalog = WhatsRent; Integrated Security = True";
            var svc = new WhatsRent.DataServices.RentService.RentService(CONNECTION_STRING);

            var results = svc.GetCityRents("Pune", null, null, "1000");
        }
    }
}
