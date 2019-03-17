using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using HtmlParsingLibrary;
using log4net;
using WhatsRent.Common.Database;
using WhatsRent.Common.Utilities;
using WhatsRent.DomainModel;

namespace WhatsRent.ETL.CommonFloor
{
    public class CF_RentPageETL
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(CF_RentPageETL));

        static string CONNECTION_STRING;
        static string ROOT_DIR;

        static string ROOT_PROPERTIES_DIR = ROOT_DIR + @"Properties\";
        const string CITYWISE_PROPERTY_FILEPATHS_REF_FILE = "NewPropertyFilePaths-{0}.txt";
        static string REF_FILES_DIR = ROOT_DIR + @"1_REF_FILES\";
        const string CITIES_REF_FILE = "AllCities.txt";

        const string PropDescStart = "{\"flpBody\":";
        const string PropDescEnd = ",\"url\":";

        static int newPropertiesInsertedCntr = 0;
        static int totalPropertiesCntr = 0;
        public static void RunOnRentPages()
        {
            int runType = int.Parse(Config.ConfigMap["runtype"]);
            int minutesold = int.Parse(Config.ConfigMap["minutesold"]);
            ROOT_DIR = Config.ConfigMap["rootdir"];
            CONNECTION_STRING = Config.ConfigMap["connectionstring"];

            DateTime etlStartTime = DateTime.UtcNow;

            ROOT_PROPERTIES_DIR = ROOT_DIR + @"Properties\";
            REF_FILES_DIR = ROOT_DIR + @"1_REF_FILES\";

            //var citiesFile = ROOT_DIR + CITIES_REF_FILE;
            //var cities = File.ReadAllLines(citiesFile);

            var cities = Config.ConfigMap["cities"].Split(',');

            //Data 
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            DBLibrary db = new DBLibrary(CONNECTION_STRING);

            foreach (var city in cities)
            {
                Log.Info("Starting to do city: " + city);

                var cityNewPropertyFilePathsFile = REF_FILES_DIR + string.Format(CITYWISE_PROPERTY_FILEPATHS_REF_FILE, city);

                IEnumerable<string> filePaths;

                var cityDir = Path.Combine(ROOT_PROPERTIES_DIR, city);

                var listingsDir = Path.Combine(cityDir, "Listings");

                if (runType == 0)
                    filePaths = File.ReadAllLines(cityNewPropertyFilePathsFile);
                else
                    filePaths = Directory.EnumerateFiles(listingsDir, "*", SearchOption.AllDirectories);


                foreach (var propertyFile in filePaths)
                {
                    //Log.InfoFormat("#{0} - {1}", totalPropertiesCntr, propertyFile);

                    Interlocked.Increment(ref totalPropertiesCntr);
                    if (runType == 2)
                    {
                        if ((new FileInfo(propertyFile).LastWriteTimeUtc.Date < etlStartTime.Date))
                            continue;
                    }
                    if (runType == 3)
                    {
                        if ((new FileInfo(propertyFile).LastWriteTimeUtc < etlStartTime.AddMinutes(-minutesold)))
                            continue;
                    }
                    Log.InfoFormat("#{0} Inserting - {1}", newPropertiesInsertedCntr, propertyFile);
                    Interlocked.Increment(ref newPropertiesInsertedCntr);

                    var propertyData = File.ReadAllText(propertyFile);
                    var tmpIdx = 0;

                    var propDescJsonBody = StringParser.GetStringBetween(propertyData,
                        tmpIdx,
                        PropDescStart,
                        PropDescEnd, null, out tmpIdx);

                    if (tmpIdx > 0 && !string.IsNullOrEmpty(propDescJsonBody))
                    {
                        propDescJsonBody += "}";

                        var jsonDto = json_serializer.Deserialize<PropertyRentCFJsonDto>(propDescJsonBody);

                        var storageDto = ConvertCFJsonDtoToStorageDto(jsonDto);

                        db.ExecuteProcedure("dbo.RentDataDelsertCommand", storageDto.DbParamListForInsert());
                        // save the json
                    }
                }
            }

            Log.InfoFormat("Properties ETL stats - new loaded: {0}, total files available: {1}", newPropertiesInsertedCntr, totalPropertiesCntr);
        }

        public static PropertyRentStorageDto ConvertCFJsonDtoToStorageDto(PropertyRentCFJsonDto src)
        {
            var dest = new PropertyRentStorageDto();

            dest.source_system = "CF";
            dest.added_by_type = src.contact_person_type;
            dest.added_by_name = src.contact_name;
            dest.record_type = "advt";

            dest.property_id = src.id;
            dest.project_name = src.name;
            dest.project_id = GetProjectId(src.project_url);
            dest.area_id = src.area_id;
            dest.area_name = src.area_name;
            dest.city = src.city;
            dest.property_type = src.type;
            dest.property_description = src.description_by_user;
            dest.title = src.title;
            dest.about_project = src.about_project;
            string unitType;
            var bedrooms = GetBedrooms(src.bed_rooms, src.title, out unitType);
            dest.unit_type = unitType;
            dest.floor_number = Utilities.GetNullableInt(src.on_floor);
            dest.bed_rooms = bedrooms;
            dest.bath_rooms = Utilities.GetNullableInt(src.bath_rooms);
            dest.balconies = Utilities.GetNullableInt(src.balconies);
            dest.facing = src.facing;
            dest.area_carpet = Utilities.GetNullableInt(src.area_carpet);
            dest.area_builtup = Utilities.GetNullableInt(src.area_builtup);
            dest.area_unit_type = src.area_unit_type;
            dest.flooring_type = src.flooring_type;
            dest.intent = src.intent;
            dest.ownership = src.ownership;
            dest.rent_monthly = GetExpectedAmount(src.expected_amount);
            dest.deposit_amount = Utilities.GetNullableInt(src.deposit_amount);
            dest.is_negotiable = src.is_negotiable;
            dest.possession_from = Utilities.GetNullableDateTime(src.possession_from);
            dest.property_age_years = Utilities.GetNullableInt(src.property_age);
            dest.bachelors_allowed = src.bachelors_allowed;
            dest.is_dining_space_available = src.is_dining_space_available;
            dest.furnish_state = src.furnish_state;
            dest.two_wheeler_parking = src.two_wheeler_parking;
            dest.four_wheeler_parking = src.four_wheeler_parking;
            dest.address = src.address;
            dest.total_floors = Utilities.GetNullableInt(src.total_floors);
            dest.on_floor = Utilities.GetNullableInt(src.on_floor);
            dest.block = src.block;
            dest.contact_name = src.contact_name;
            dest.contact_person_type = src.contact_person_type;
            dest.added_on = GetAddedOnDate(src.added_on);
            dest.state = src.state;
            dest.pause_mask = src.pause_mask;
            dest.is_physically_verified = src.is_physically_verified;
            dest.brokerage_terms = src.brokerage_terms;
            dest.description_by_user = src.description_by_user;
            //dest.amenities = src.amenities;
            dest.registration_charges = src.registration_charges;
            dest.car_parking = Utilities.GetNullableInt(src.car_parking);
            dest.project_url = src.project_url;
            dest.url = src.url;
            dest.location_url = src.location_url;
            dest.locality_url = src.locality_url;
            dest.contact_person_profile_url = src.contact_person_profile_url;
            dest.latitude = src.lat;
            dest.longitude = src.lng;

            return dest;
        }

        public static DateTime? GetAddedOnDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            DateTime result;
            if (!DateTime.TryParse(input, out result))
                return null;

            if (result.Date > DateTime.Today)
                result = result.AddYears(-1);

            return result;
        }

        public static int? GetExpectedAmount(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Remove(0, 4).Replace(",", "");

            int result;
            if (!int.TryParse(input, out result))
                return null;

            return result;
        }

        public static double? GetBedrooms(string bed_rooms, string title, out string unit_type)
        {
            unit_type = null;

            if (!string.IsNullOrWhiteSpace(title))
            {
                var bhkStr = title.Substring(0, title.IndexOf("K") + 1);
                unit_type = bhkStr.Replace(" ", "");
            }

            if (!string.IsNullOrWhiteSpace(bed_rooms))
            {
                double result;

                if (double.TryParse(bed_rooms, out result))
                {
                    if (unit_type == "BHK")
                    {
                        int integral = (int)result;
                        if (result - integral == 0)
                            unit_type = integral + unit_type;
                        else
                            unit_type = result + unit_type;
                    }
                    return result;
                }

                else if (!string.IsNullOrWhiteSpace(unit_type))
                {
                    var temp = unit_type.Replace("BHK", "").Replace("RK", "");

                    if (double.TryParse(temp, out result))
                        return result;
                }
            }

            return null;
        }

        public static string GetProjectId(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return input.Substring(input.LastIndexOf("/") + 1);
        }
    }
}
