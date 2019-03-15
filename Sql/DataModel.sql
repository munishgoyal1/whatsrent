 DROP TABLE [WhatsRent].[dbo].[rent_data]
 GO

CREATE TABLE WhatsRent.dbo.rent_data
(
	property_id				varchar(50) not null primary key,
	project_name			varchar(50) null,
	project_url				varchar(100) null,
	project_id				varchar(50) null,
	area_id					varchar(50) null,
	area_name				varchar(50) null,
	city					varchar(50) null,
	property_description	varchar(256) null,
	title					varchar(100) null,
	about_project			varchar(100) null,
	property_type			varchar(50) null,
	unit_type				varchar(20) null,
	floor_number			int null,
	bed_rooms				decimal(2,1) null,
	bath_rooms				int null,
	balconies				int null,
	facing					varchar(50) null,
	area_carpet				int null,
	area_builtup			int null,
	area_unit_type			varchar(50) null,
	flooring_type			varchar(50) null,
	intent					varchar(50) null,
	ownership				varchar(50) null,
	rent_monthly			int null,
	deposit_amount			int null,
	is_negotiable			varchar(50) null,
	possession_from			date null,   -- rent valid start date
	property_age_years		int null,
	bachelors_allowed		varchar(10) null,
	furnish_state			varchar(50) null,
	servant_accomodation	varchar(10) null,
	two_wheeler_parking		varchar(10) null,
	four_wheeler_parking	varchar(10) null,
	address					varchar(100) null,
	total_floors			int null,
	source_system			varchar(30) null, -- CF, MB, PT, 99, user, admin etc.
	record_type				varchar(20) null, -- advertisement or real data
	added_on				date null,  -- record posted on
	contact_person_type		varchar(20) null, -- broker, owner
	added_by_type			varchar(20) null, -- broker, user, admin
	added_by_name			varchar(30) null,
	latitude				varchar(20) null, -- property location
    longitude				varchar(20) null,
	last_updated			datetime null default(getdate())
)
GO

/*
ALTER TABLE WhatsRent.dbo.rent_data ADD CONSTRAINT
rent_data_Inserted DEFAULT GETDATE() FOR last_updated
GO
*/

/*

use WhatsRent
go
alter table rent_data 
add  servant_accomodation varchar(10) null
*/