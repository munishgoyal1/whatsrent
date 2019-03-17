using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsRent.Common.Database;

namespace WhatsRent.DomainModel
{
    public class PropertyRentCFJsonDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string bed_rooms { get; set; }
        public string bath_rooms { get; set; }
        public string balconies { get; set; }
        public string facing { get; set; }
        public string area_carpet { get; set; }
        public string area_builtup { get; set; }
        public string area_unit_type { get; set; }
        public string flooring_type { get; set; }
        public string intent { get; set; }
        public string ownership { get; set; }
        public string servant_accommodation { get; set; }
        public string expected_amount { get; set; }
        public string deposit_amount { get; set; }
        public string is_negotiable { get; set; }
        public string possession_from { get; set; }
        public string property_age { get; set; }
        public string bachelors_allowed { get; set; }
        public string is_dining_space_available { get; set; }
        public string furnish_state { get; set; }
        public string two_wheeler_parking { get; set; }
        public string four_wheeler_parking { get; set; }
        public string area_name { get; set; }
        public string area_id { get; set; }
        public string city { get; set; }
        public string property_id { get; set; }
        public string address { get; set; }
        public string total_floors { get; set; }
        public string on_floor { get; set; }
        public string block { get; set; }
        public string contact_name { get; set; }
        public string contact_person_type { get; set; }
        public string added_by { get; set; }
        public string added_on { get; set; }
        public string state { get; set; }
        public string pause_mask { get; set; }
        public string is_physically_verified { get; set; }
        public string brokerage_terms { get; set; }
        public string description_by_user { get; set; }
        public Dictionary<string, List<string>> amenities { get; set; }
        public string about_project { get; set; }
        public string registration_charges { get; set; }
        public string car_parking { get; set; }
        public string sale_type { get; set; }
        public string is_shortlisted { get; set; }
        public string project_url { get; set; }
        public string url { get; set; }
        public string location_url { get; set; }
        public string locality_url { get; set; }
        public string contact_person_profile_url { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class PropertyRentStorageDto
    {
        public string property_id { get; set; }
        public string project_name { get; set; }
        public string project_id { get; set; }
        public string area_id { get; set; }
        public string area_name { get; set; }
        public string city { get; set; }
        public string property_description { get; set; }
        public string title { get; set; }
        public string property_type { get; set; }  // new  "Residential Apartment"
        public string servant_accomodation { get; set; }// new  Yes No
        public string unit_type { get; set; }
        public int? floor_number { get; set; }
        public double? bed_rooms { get; set; }
        public int? bath_rooms { get; set; }
        public int? balconies { get; set; }
        public string facing { get; set; }
        public int? area_carpet { get; set; }
        public int? area_builtup { get; set; }
        public string area_unit_type { get; set; }
        public string flooring_type { get; set; }
        public string intent { get; set; }
        public string ownership { get; set; }
        public int? rent_monthly { get; set; }
        public int? deposit_amount { get; set; }
        public string is_negotiable { get; set; }
        public DateTime? possession_from { get; set; }
        public int? property_age_years { get; set; }
        public string bachelors_allowed { get; set; }
        public string is_dining_space_available { get; set; }
        public string furnish_state { get; set; }
        public string two_wheeler_parking { get; set; }
        public string four_wheeler_parking { get; set; }
        public string address { get; set; }
        public int? total_floors { get; set; }
        public int? on_floor { get; set; }
        public string block { get; set; }
        public string contact_name { get; set; }
        public string contact_person_type { get; set; }
        public string added_by_type { get; set; }
        public string added_by_name { get; set; }
        public DateTime? added_on { get; set; }
        public string state { get; set; }
        public string pause_mask { get; set; }
        public string is_physically_verified { get; set; }
        public string brokerage_terms { get; set; }
        public string description_by_user { get; set; }
        public string amenities { get; set; }
        public string about_project { get; set; }
        public string registration_charges { get; set; }
        public int? car_parking { get; set; }
        public string is_shortlisted { get; set; }
        public string project_url { get; set; }
        public string url { get; set; }
        public string location_url { get; set; }
        public string locality_url { get; set; }
        public string contact_person_profile_url { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string source_system { get; set; }
        public string record_type { get; set; }
        public DateTime last_updated { get; set; }

        public ArrayList DbParamListForInsert()
        {
            ArrayList paramList = new ArrayList();

            DBUtilities dbUtilities = new DBUtilities();

            paramList.Add(dbUtilities.CreateSqlParamater("@property_id", SqlDbType.NVarChar, 50, ParameterDirection.Input, this.property_id));
            paramList.Add(dbUtilities.CreateSqlParamater("@project_name", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.project_name)));
            paramList.Add(dbUtilities.CreateSqlParamater("@project_id", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.project_id)));
            paramList.Add(dbUtilities.CreateSqlParamater("@project_url", SqlDbType.NVarChar, 100, ParameterDirection.Input, GetDBValue(this.project_url)));
            paramList.Add(dbUtilities.CreateSqlParamater("@area_id", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.area_id)));
            paramList.Add(dbUtilities.CreateSqlParamater("@area_name", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.area_name)));
            paramList.Add(dbUtilities.CreateSqlParamater("@city", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.city)));
            paramList.Add(dbUtilities.CreateSqlParamater("@property_description", SqlDbType.NVarChar, 256, ParameterDirection.Input, GetDBValue(this.property_description)));
            paramList.Add(dbUtilities.CreateSqlParamater("@title", SqlDbType.NVarChar, 100, ParameterDirection.Input, GetDBValue(this.title)));
            paramList.Add(dbUtilities.CreateSqlParamater("@about_project", SqlDbType.NVarChar, 100, ParameterDirection.Input, GetDBValue(this.about_project)));
            paramList.Add(dbUtilities.CreateSqlParamater("@property_type", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.property_type)));
            paramList.Add(dbUtilities.CreateSqlParamater("@unit_type", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.unit_type)));

            
            paramList.Add(dbUtilities.CreateSqlParamater("@floor_number", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.floor_number)));
            paramList.Add(dbUtilities.CreateSqlParamater("@bed_rooms", SqlDbType.Decimal, ParameterDirection.Input, GetDBValue(this.bed_rooms)));
            paramList.Add(dbUtilities.CreateSqlParamater("@bath_rooms", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.bath_rooms)));
            paramList.Add(dbUtilities.CreateSqlParamater("@balconies", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.balconies)));
            paramList.Add(dbUtilities.CreateSqlParamater("@area_carpet", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.area_carpet)));
            paramList.Add(dbUtilities.CreateSqlParamater("@area_builtup", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.area_builtup)));
            paramList.Add(dbUtilities.CreateSqlParamater("@rent_monthly", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.rent_monthly)));
            paramList.Add(dbUtilities.CreateSqlParamater("@deposit_amount", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.deposit_amount)));
            paramList.Add(dbUtilities.CreateSqlParamater("@property_age_years", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.property_age_years)));
            paramList.Add(dbUtilities.CreateSqlParamater("@total_floors", SqlDbType.Int, ParameterDirection.Input, GetDBValue(this.total_floors)));

            paramList.Add(dbUtilities.CreateSqlParamater("@facing", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.facing)));
            paramList.Add(dbUtilities.CreateSqlParamater("@area_unit_type", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.area_unit_type)));
            paramList.Add(dbUtilities.CreateSqlParamater("@flooring_type", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.flooring_type)));
            paramList.Add(dbUtilities.CreateSqlParamater("@intent", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.intent)));
            paramList.Add(dbUtilities.CreateSqlParamater("@ownership", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.ownership)));
            paramList.Add(dbUtilities.CreateSqlParamater("@is_negotiable", SqlDbType.NVarChar, 10, ParameterDirection.Input, GetDBValue(this.is_negotiable)));
            paramList.Add(dbUtilities.CreateSqlParamater("@bachelors_allowed", SqlDbType.NVarChar, 10, ParameterDirection.Input, GetDBValue(this.bachelors_allowed)));
            paramList.Add(dbUtilities.CreateSqlParamater("@furnish_state", SqlDbType.NVarChar, 50, ParameterDirection.Input, GetDBValue(this.furnish_state)));
            paramList.Add(dbUtilities.CreateSqlParamater("@two_wheeler_parking", SqlDbType.NVarChar, 10, ParameterDirection.Input, GetDBValue(this.two_wheeler_parking)));
            paramList.Add(dbUtilities.CreateSqlParamater("@four_wheeler_parking", SqlDbType.NVarChar, 10, ParameterDirection.Input, GetDBValue(this.four_wheeler_parking)));
            paramList.Add(dbUtilities.CreateSqlParamater("@address", SqlDbType.NVarChar, 100, ParameterDirection.Input, GetDBValue(this.address)));
            paramList.Add(dbUtilities.CreateSqlParamater("@source_system", SqlDbType.NVarChar, 30, ParameterDirection.Input, GetDBValue(this.source_system)));
            paramList.Add(dbUtilities.CreateSqlParamater("@record_type", SqlDbType.NVarChar, 20, ParameterDirection.Input, GetDBValue(this.record_type)));
            paramList.Add(dbUtilities.CreateSqlParamater("@added_by_type", SqlDbType.NVarChar, 20, ParameterDirection.Input, GetDBValue(this.added_by_type)));
            paramList.Add(dbUtilities.CreateSqlParamater("@added_by_name", SqlDbType.NVarChar, 20, ParameterDirection.Input, GetDBValue(this.added_by_name)));
            paramList.Add(dbUtilities.CreateSqlParamater("@contact_person_type", SqlDbType.NVarChar, 20, ParameterDirection.Input, GetDBValue(this.contact_person_type)));
            paramList.Add(dbUtilities.CreateSqlParamater("@latitude", SqlDbType.NVarChar, 20, ParameterDirection.Input, GetDBValue(this.latitude)));
            paramList.Add(dbUtilities.CreateSqlParamater("@longitude", SqlDbType.NVarChar, 20, ParameterDirection.Input, GetDBValue(this.longitude)));

            //contact_person_type

            paramList.Add(dbUtilities.CreateSqlParamater("@possession_from", SqlDbType.DateTime, ParameterDirection.Input, GetDBValue(this.possession_from)));
            paramList.Add(dbUtilities.CreateSqlParamater("@added_on", SqlDbType.DateTime, ParameterDirection.Input, GetDBValue(this.added_on)));

            return paramList;
        }

        private object GetDBValue(object input)
        {
            return input ?? (object)DBNull.Value;
        }
    }
}
