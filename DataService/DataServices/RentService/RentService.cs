using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using WhatsRent.Common.Database;
using WhatsRent.Common.Utilities;
using WhatsRent.DomainModel;

namespace WhatsRent.DataServices.RentService
{
    public class RentService : IRentService
    {
        string _connectionString;
        public RentService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<PropertyRentServiceDto> GetRents(string city, string numrec)
        {
            DBLibrary db = new DBLibrary(_connectionString);

            var query = string.Format("select top {1} * from rent_data where city = '{0}'", city, numrec);
            var ds = db.ExecuteQuery(query);


            //var list = ds.Tables["Table"].ToList<PropertyRentServiceDto>();
            List<PropertyRentServiceDto> list = new List<PropertyRentServiceDto>(ds.Tables["Table"].Rows.Count);
            foreach (DataRow dr in ds.Tables["Table"].Rows)
            {
                var dto = ConvertPropertyRentDataRowToServiceDto(dr);
                list.Add(dto);
            }
            return list;
            //var only5 = list.Take(1);
            //return only5;
        }

        public IEnumerable<PropertyAverageRentDto> GetCurrentRents(string city)
        {
            DBLibrary db = new DBLibrary(_connectionString);

            Thread.Sleep(100);

            var dateToday = DateTime.Today;
            var dateStart = dateToday.AddDays(-100);
            var dateEnd = dateToday.AddDays(100);

            var dateRange = string.Format("possession_from between '{0}' and '{1}'", dateStart.ToString("yyyy-MM-dd"), dateEnd.ToString("yyyy-MM-dd"));
            var query = string.Format("select avg(rent_monthly) rent, unit_type from rent_data where {0} and city = '{1}' group by unit_type", dateRange, city);
            var ds = db.ExecuteQuery(query);

            List<PropertyAverageRentDto> list = new List<PropertyAverageRentDto>(ds.Tables["Table"].Rows.Count);
            list = ds.Tables["Table"].AsEnumerable().Select(r =>
                    new PropertyAverageRentDto
                    {
                        unit_type = r["unit_type"].ToString(),
                        avg_rent_monthly = Utilities.GetNullableInt(r["rent"].ToString())
                    }
            ).ToList();

            return list;
        }


        public IEnumerable<PropertyRentServiceTrimmedDto> TestRents(string numrec)
        {
            DBLibrary db = new DBLibrary(_connectionString);

            var query = string.Format("select top {1} * from rent_data where city = '{0}'", "Pune", numrec);
            var ds = db.ExecuteQuery(query);


            //var list = ds.Tables["Table"].ToList<PropertyRentServiceDto>();
            List<PropertyRentServiceDto> list = new List<PropertyRentServiceDto>(ds.Tables["Table"].Rows.Count);
            foreach (DataRow dr in ds.Tables["Table"].Rows)
            {
                var dto = ConvertPropertyRentDataRowToServiceDto(dr);
                list.Add(dto);
            }

            var testList = list.Select(i => new PropertyRentServiceTrimmedDto
            {  property_id = i.property_id, bed_rooms = i.bed_rooms, floor_number = null, possession_from = i.possession_from,
            added_on = i.added_on, last_updated = i.last_updated});

            return testList;
        }

        private PropertyRentServiceDto ConvertPropertyRentDataRowToServiceDto(DataRow dr)
        {

            var dto = new PropertyRentServiceDto();

            dto.source_system = dr["source_system"].ToString();
            dto.added_by_type = dr["added_by_type"].ToString();
            dto.added_by_name = dr["added_by_name"].ToString();
            dto.record_type = dr["record_type"].ToString();

            dto.property_id = dr["property_id"].ToString();
            dto.project_name = dr["project_name"].ToString();
            dto.project_id = dr["project_id"].ToString();
            dto.area_id = dr["area_id"].ToString();
            dto.area_name = dr["area_name"].ToString();
            dto.city = dr["city"].ToString();
            dto.property_type = dr["property_type"].ToString();
            dto.property_description = dr["property_description"].ToString();
            dto.title = dr["title"].ToString();
            dto.about_project = dr["about_project"].ToString();
            dto.unit_type = dr["unit_type"].ToString();
            dto.floor_number = Utilities.GetNullableInt(dr["floor_number"].ToString());
            dto.bed_rooms = Utilities.GetNullableDouble(dr["bed_rooms"].ToString());
            dto.bath_rooms = Utilities.GetNullableInt(dr["bath_rooms"].ToString());
            dto.balconies = Utilities.GetNullableInt(dr["balconies"].ToString());
            dto.facing = dr["facing"].ToString();
            dto.area_carpet = Utilities.GetNullableInt(dr["area_carpet"].ToString());
            dto.area_builtup = Utilities.GetNullableInt(dr["area_builtup"].ToString());
            dto.area_unit_type = dr["area_unit_type"].ToString();
            dto.flooring_type = dr["flooring_type"].ToString();
            dto.intent = dr["intent"].ToString();
            dto.ownership = dr["ownership"].ToString();
            dto.rent_monthly = Utilities.GetNullableInt(dr["rent_monthly"].ToString());
            dto.deposit_amount = Utilities.GetNullableInt(dr["deposit_amount"].ToString());
            dto.is_negotiable = dr["is_negotiable"].ToString();
            dto.possession_from = Utilities.GetNullableDateTime(dr["possession_from"].ToString());
            dto.property_age_years = Utilities.GetNullableInt(dr["property_age_years"].ToString());
            dto.bachelors_allowed = dr["bachelors_allowed"].ToString();
            //dto.is_dining_space_available = dr["is_dining_space_available"].ToString();
            dto.furnish_state = dr["furnish_state"].ToString();
            dto.two_wheeler_parking = dr["two_wheeler_parking"].ToString();
            dto.four_wheeler_parking = dr["four_wheeler_parking"].ToString();
            dto.address = dr["address"].ToString();
            dto.total_floors = Utilities.GetNullableInt(dr["total_floors"].ToString());
            dto.on_floor = Utilities.GetNullableInt(dr["floor_number"].ToString());
            //dto.block = dr["block"].ToString();
            dto.contact_name = dr["added_by_name"].ToString();
            dto.contact_person_type = dr["contact_person_type"].ToString();
            dto.added_on = Utilities.GetNullableDateTime(dr["added_on"].ToString());
            dto.last_updated = Utilities.GetNullableDateTime(dr["last_updated"].ToString());
            //dto.state = dr["state"].ToString();
            //dto.pause_mask = dr["pause_mask"].ToString();
            //dto.is_physically_verified = dr["is_physically_verified"].ToString();
            //dto.brokerage_terms = dr["brokerage_terms"].ToString();
            dto.description_by_user = dr["property_description"].ToString();
            //dto.amenities = src.amenities;
            //dto.registration_charges = dr["registration_charges"].ToString();
            //dto.car_parking = Utilities.GetNullableInt(dr["car_parking"].ToString());
            //dto.project_url = dr["project_url"].ToString();
            //dto.url = dr["url"].ToString();
            //dto.location_url = dr["location_url"].ToString();
            //dto.locality_url = dr["locality_url"].ToString();
            //dto.contact_person_profile_url = dr["contact_person_profile_url"].ToString();
            dto.latitude = dr["latitude"].ToString();
            dto.longitude = dr["longitude"].ToString();

            return dto;
        }
    }
}
