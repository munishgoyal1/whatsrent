using System;
using System.Collections.Generic;

namespace WhatsRent.DomainModel
{
    public class CityMetadataDto
    {
        public string city { get; set; }
        public IEnumerable<string> areanames { get; set; }
        public IEnumerable<string> projectnames { get; set; }
    }

    public class PropertyAverageRentDto
    {
        public string unit_type { get; set; }
        public int? avg_rent_monthly { get; set; }
    }

    public class PropertyRentServiceTrimmedDto
    {
        public string property_id { get; set; }
        public int? floor_number { get; set; }
        public double? bed_rooms { get; set; }
        public DateTime? possession_from { get; set; }
        public DateTime? added_on { get; set; }
        public DateTime? last_updated { get; set; }
    }
    public class PropertyRentServiceDto
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
        public int? on_floor { get; set; } // not in table
        public string block { get; set; }// not in table
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
        public DateTime? last_updated { get; set; }
    }

    public class RentDtoMapper
    {
        public static PropertyRentServiceDto ConvertStorageDtoToServiceDto(PropertyRentStorageDto src)
        {
            var dest = new PropertyRentServiceDto();

            dest.source_system = src.source_system;
            dest.added_by_type = src.added_by_type;
            dest.added_by_name = src.added_by_name;
            dest.record_type = src.record_type;

            dest.property_id = src.property_id;
            dest.project_name = src.project_name;
            dest.project_id = src.project_id;
            dest.area_id = src.area_id;
            dest.area_name = src.area_name;
            dest.city = src.city;
            dest.property_type = src.property_type;
            dest.property_description = src.description_by_user;
            dest.title = src.title;
            dest.about_project = src.about_project;
            dest.unit_type = src.unit_type;
            dest.floor_number = src.floor_number;
            dest.bed_rooms = src.bed_rooms;
            dest.bath_rooms = src.bath_rooms;
            dest.balconies = src.balconies;
            dest.facing = src.facing;
            dest.area_carpet = src.area_carpet;
            dest.area_builtup = src.area_builtup;
            dest.area_unit_type = src.area_unit_type;
            dest.flooring_type = src.flooring_type;
            dest.intent = src.intent;
            dest.ownership = src.ownership;
            dest.rent_monthly = src.rent_monthly;
            dest.deposit_amount = src.deposit_amount;
            dest.is_negotiable = src.is_negotiable;
            dest.possession_from = src.possession_from;
            dest.property_age_years = src.property_age_years;
            dest.bachelors_allowed = src.bachelors_allowed;
            dest.is_dining_space_available = src.is_dining_space_available;
            dest.furnish_state = src.furnish_state;
            dest.two_wheeler_parking = src.two_wheeler_parking;
            dest.four_wheeler_parking = src.four_wheeler_parking;
            dest.address = src.address;
            dest.total_floors = src.total_floors;
            dest.on_floor = src.on_floor;
            dest.block = src.block;
            dest.contact_name = src.contact_name;
            dest.contact_person_type = src.contact_person_type;
            dest.added_on = src.added_on;
            dest.state = src.state;
            dest.pause_mask = src.pause_mask;
            dest.is_physically_verified = src.is_physically_verified;
            dest.brokerage_terms = src.brokerage_terms;
            dest.description_by_user = src.description_by_user;
            //dest.amenities = src.amenities;
            dest.registration_charges = src.registration_charges;
            dest.car_parking = src.car_parking;
            dest.project_url = src.project_url;
            dest.url = src.url;
            dest.location_url = src.location_url;
            dest.locality_url = src.locality_url;
            dest.contact_person_profile_url = src.contact_person_profile_url;
            dest.latitude = src.latitude;
            dest.longitude = src.longitude;

            return dest;
        }
    }
}
