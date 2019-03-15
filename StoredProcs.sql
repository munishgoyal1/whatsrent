
/* Delete existing and Insert new record into Rent Data */
USE [WhatsRent]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'RentDataDelsertCommand' AND user_name(uid) = 'dbo')
BEGIN
	DROP PROC dbo.[RentDataDelsertCommand]
END
GO

USE [WhatsRent]
GO

/****** Object:  StoredProcedure [dbo].[[RentDataDelsertCommand]]    Script Date: 3/25/2016 1:16:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RentDataDelsertCommand]
(
	@property_id varchar(50),
	@project_name varchar(50),
	@project_id varchar(50),
	@area_id varchar(50),
	@area_name varchar(50),
	@city varchar(50),
	@property_description varchar(256),
	@title varchar(100),
	@about_project varchar(100),
	@property_type varchar(50),
	@unit_type varchar(20),
	@floor_number int,
	@bed_rooms decimal(2,1),
	@bath_rooms int,
	@balconies int,
	@facing varchar(50),
	@area_carpet int,
	@area_builtup int,
	@area_unit_type varchar(50),
	@flooring_type varchar(50),
	@intent varchar(50),
	@ownership varchar(50),
	@rent_monthly int,
	@deposit_amount int,
	@is_negotiable varchar(50),
	@possession_from date,
	@property_age_years int,
	@bachelors_allowed varchar(10),
	@furnish_state varchar(50),
	@two_wheeler_parking varchar(10),
	@four_wheeler_parking varchar(10),
	@address varchar(100),
	@total_floors int,
	@source_system varchar(30),
	@record_type varchar(20),
	@added_on date,
	@contact_person_type varchar(20),
	@added_by_type varchar(20),
	@added_by_name varchar(30),
	@latitude varchar(20),
	@longitude varchar(20)
)
AS
BEGIN
	SET NOCOUNT OFF;

	DELETE FROM [rent_data] WHERE property_id=@property_id;
INSERT INTO [rent_data] ([property_id], [project_name], [project_id], [area_id], [area_name], [city], [property_description], [title], [about_project], [property_type], [unit_type], [floor_number], [bed_rooms], [bath_rooms], [balconies], [facing], [area_carpet], [area_builtup], [area_unit_type], [flooring_type], [intent], [ownership], [rent_monthly], [deposit_amount], [is_negotiable], [possession_from], [property_age_years], [bachelors_allowed], [furnish_state], [two_wheeler_parking], [four_wheeler_parking], [address], [total_floors], [source_system], [record_type], [added_on], [contact_person_type], [added_by_type], [added_by_name], [latitude], [longitude]) VALUES (@property_id, @project_name, @project_id, @area_id, @area_name, @city, @property_description, @title, @about_project, @property_type, @unit_type, @floor_number, @bed_rooms, @bath_rooms, @balconies, @facing, @area_carpet, @area_builtup, @area_unit_type, @flooring_type, @intent, @ownership, @rent_monthly, @deposit_amount, @is_negotiable, @possession_from, @property_age_years, @bachelors_allowed, @furnish_state, @two_wheeler_parking, @four_wheeler_parking, @address, @total_floors, @source_system, @record_type, @added_on, @contact_person_type, @added_by_type, @added_by_name, @latitude, @longitude);
	
SELECT property_id, project_name, project_id, area_id, area_name, city, property_description, title, about_project, property_type, unit_type, floor_number, bed_rooms, bath_rooms, balconies, facing, area_carpet, area_builtup, area_unit_type, flooring_type, intent, ownership, rent_monthly, deposit_amount, is_negotiable, possession_from, property_age_years, bachelors_allowed, furnish_state, two_wheeler_parking, four_wheeler_parking, address, total_floors, source_system, record_type, added_on, contact_person_type, added_by_type, added_by_name, latitude, longitude, last_updated FROM rent_data WHERE (property_id = @property_id)
END
GO


------------------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'NewSelectCommand' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].NewSelectCommand
GO

CREATE PROCEDURE [dbo].NewSelectCommand
AS
	SET NOCOUNT ON;
select * from rent_data
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'NewInsertCommand' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].NewInsertCommand
GO

CREATE PROCEDURE [dbo].NewInsertCommand
(
	@property_id varchar(50),
	@project_name varchar(50),
	@project_id varchar(50),
	@area_id varchar(50),
	@area_name varchar(50),
	@city varchar(50),
	@property_description varchar(256),
	@title varchar(100),
	@about_project varchar(100),
	@property_type varchar(50),
	@unit_type varchar(20),
	@floor_number int,
	@bed_rooms decimal(2,1),
	@bath_rooms int,
	@balconies int,
	@facing varchar(50),
	@area_carpet int,
	@area_builtup int,
	@area_unit_type varchar(50),
	@flooring_type varchar(50),
	@intent varchar(50),
	@ownership varchar(50),
	@rent_monthly int,
	@deposit_amount int,
	@is_negotiable varchar(50),
	@possession_from date,
	@property_age_years int,
	@bachelors_allowed varchar(10),
	@furnish_state varchar(50),
	@two_wheeler_parking varchar(10),
	@four_wheeler_parking varchar(10),
	@address varchar(100),
	@total_floors int,
	@source_system varchar(30),
	@record_type varchar(20),
	@added_on date,
	@contact_person_type varchar(20),
	@added_by_type varchar(20),
	@added_by_name varchar(30),
	@latitude varchar(20),
	@longitude varchar(20),
	@last_updated datetime
)
AS
	SET NOCOUNT OFF;
INSERT INTO [rent_data] ([property_id], [project_name], [project_id], [area_id], [area_name], [city], [property_description], [title], [about_project], [property_type], [unit_type], [floor_number], [bed_rooms], [bath_rooms], [balconies], [facing], [area_carpet], [area_builtup], [area_unit_type], [flooring_type], [intent], [ownership], [rent_monthly], [deposit_amount], [is_negotiable], [possession_from], [property_age_years], [bachelors_allowed], [furnish_state], [two_wheeler_parking], [four_wheeler_parking], [address], [total_floors], [source_system], [record_type], [added_on], [contact_person_type], [added_by_type], [added_by_name], [latitude], [longitude], [last_updated]) VALUES (@property_id, @project_name, @project_id, @area_id, @area_name, @city, @property_description, @title, @about_project, @property_type, @unit_type, @floor_number, @bed_rooms, @bath_rooms, @balconies, @facing, @area_carpet, @area_builtup, @area_unit_type, @flooring_type, @intent, @ownership, @rent_monthly, @deposit_amount, @is_negotiable, @possession_from, @property_age_years, @bachelors_allowed, @furnish_state, @two_wheeler_parking, @four_wheeler_parking, @address, @total_floors, @source_system, @record_type, @added_on, @contact_person_type, @added_by_type, @added_by_name, @latitude, @longitude, @last_updated);
	
SELECT property_id, project_name, project_id, area_id, area_name, city, property_description, title, about_project, property_type, unit_type, floor_number, bed_rooms, bath_rooms, balconies, facing, area_carpet, area_builtup, area_unit_type, flooring_type, intent, ownership, rent_monthly, deposit_amount, is_negotiable, possession_from, property_age_years, bachelors_allowed, furnish_state, two_wheeler_parking, four_wheeler_parking, address, total_floors, source_system, record_type, added_on, contact_person_type, added_by_type, added_by_name, latitude, longitude, last_updated FROM rent_data WHERE (property_id = @property_id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'NewUpdateCommand' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].NewUpdateCommand
GO

CREATE PROCEDURE [dbo].NewUpdateCommand
(
	@property_id varchar(50),
	@project_name varchar(50),
	@project_id varchar(50),
	@area_id varchar(50),
	@area_name varchar(50),
	@city varchar(50),
	@property_description varchar(256),
	@title varchar(100),
	@about_project varchar(100),
	@property_type varchar(50),
	@unit_type varchar(20),
	@floor_number int,
	@bed_rooms decimal(2,1),
	@bath_rooms int,
	@balconies int,
	@facing varchar(50),
	@area_carpet int,
	@area_builtup int,
	@area_unit_type varchar(50),
	@flooring_type varchar(50),
	@intent varchar(50),
	@ownership varchar(50),
	@rent_monthly int,
	@deposit_amount int,
	@is_negotiable varchar(50),
	@possession_from date,
	@property_age_years int,
	@bachelors_allowed varchar(10),
	@furnish_state varchar(50),
	@two_wheeler_parking varchar(10),
	@four_wheeler_parking varchar(10),
	@address varchar(100),
	@total_floors int,
	@source_system varchar(30),
	@record_type varchar(20),
	@added_on date,
	@contact_person_type varchar(20),
	@added_by_type varchar(20),
	@added_by_name varchar(30),
	@latitude varchar(20),
	@longitude varchar(20),
	@last_updated datetime,
	@Original_property_id varchar(50),
	@IsNull_project_name Int,
	@Original_project_name varchar(50),
	@IsNull_project_id Int,
	@Original_project_id varchar(50),
	@IsNull_area_id Int,
	@Original_area_id varchar(50),
	@IsNull_area_name Int,
	@Original_area_name varchar(50),
	@IsNull_city Int,
	@Original_city varchar(50),
	@IsNull_property_description Int,
	@Original_property_description varchar(256),
	@IsNull_title Int,
	@Original_title varchar(100),
	@IsNull_about_project Int,
	@Original_about_project varchar(100),
	@IsNull_property_type Int,
	@Original_property_type varchar(50),
	@IsNull_unit_type Int,
	@Original_unit_type varchar(20),
	@IsNull_floor_number Int,
	@Original_floor_number int,
	@IsNull_bed_rooms Int,
	@Original_bed_rooms int,
	@IsNull_bath_rooms Int,
	@Original_bath_rooms int,
	@IsNull_balconies Int,
	@Original_balconies int,
	@IsNull_facing Int,
	@Original_facing varchar(50),
	@IsNull_area_carpet Int,
	@Original_area_carpet int,
	@IsNull_area_builtup Int,
	@Original_area_builtup int,
	@IsNull_area_unit_type Int,
	@Original_area_unit_type varchar(50),
	@IsNull_flooring_type Int,
	@Original_flooring_type varchar(50),
	@IsNull_intent Int,
	@Original_intent varchar(50),
	@IsNull_ownership Int,
	@Original_ownership varchar(50),
	@IsNull_rent_monthly Int,
	@Original_rent_monthly int,
	@IsNull_deposit_amount Int,
	@Original_deposit_amount int,
	@IsNull_is_negotiable Int,
	@Original_is_negotiable varchar(50),
	@IsNull_possession_from Int,
	@Original_possession_from date,
	@IsNull_property_age_years Int,
	@Original_property_age_years int,
	@IsNull_bachelors_allowed Int,
	@Original_bachelors_allowed varchar(10),
	@IsNull_furnish_state Int,
	@Original_furnish_state varchar(50),
	@IsNull_two_wheeler_parking Int,
	@Original_two_wheeler_parking varchar(10),
	@IsNull_four_wheeler_parking Int,
	@Original_four_wheeler_parking varchar(10),
	@IsNull_address Int,
	@Original_address varchar(100),
	@IsNull_total_floors Int,
	@Original_total_floors int,
	@IsNull_source_system Int,
	@Original_source_system varchar(30),
	@IsNull_record_type Int,
	@Original_record_type varchar(20),
	@IsNull_added_on Int,
	@Original_added_on date,
	@IsNull_contact_person_type Int,
	@Original_contact_person_type varchar(20),
	@IsNull_added_by_type Int,
	@Original_added_by_type varchar(20),
	@IsNull_added_by_name Int,
	@Original_added_by_name varchar(30),
	@IsNull_latitude Int,
	@Original_latitude varchar(20),
	@IsNull_longitude Int,
	@Original_longitude varchar(20),
	@IsNull_last_updated Int,
	@Original_last_updated datetime
)
AS
	SET NOCOUNT OFF;
UPDATE [rent_data] SET [property_id] = @property_id, [project_name] = @project_name, [project_id] = @project_id, [area_id] = @area_id, [area_name] = @area_name, [city] = @city, [property_description] = @property_description, [title] = @title, [about_project] = @about_project, [property_type] = @property_type, [unit_type] = @unit_type, [floor_number] = @floor_number, [bed_rooms] = @bed_rooms, [bath_rooms] = @bath_rooms, [balconies] = @balconies, [facing] = @facing, [area_carpet] = @area_carpet, [area_builtup] = @area_builtup, [area_unit_type] = @area_unit_type, [flooring_type] = @flooring_type, [intent] = @intent, [ownership] = @ownership, [rent_monthly] = @rent_monthly, [deposit_amount] = @deposit_amount, [is_negotiable] = @is_negotiable, [possession_from] = @possession_from, [property_age_years] = @property_age_years, [bachelors_allowed] = @bachelors_allowed, [furnish_state] = @furnish_state, [two_wheeler_parking] = @two_wheeler_parking, [four_wheeler_parking] = @four_wheeler_parking, [address] = @address, [total_floors] = @total_floors, [source_system] = @source_system, [record_type] = @record_type, [added_on] = @added_on, [contact_person_type] = @contact_person_type, [added_by_type] = @added_by_type, [added_by_name] = @added_by_name, [latitude] = @latitude, [longitude] = @longitude, [last_updated] = @last_updated WHERE (([property_id] = @Original_property_id) AND ((@IsNull_project_name = 1 AND [project_name] IS NULL) OR ([project_name] = @Original_project_name)) AND ((@IsNull_project_id = 1 AND [project_id] IS NULL) OR ([project_id] = @Original_project_id)) AND ((@IsNull_area_id = 1 AND [area_id] IS NULL) OR ([area_id] = @Original_area_id)) AND ((@IsNull_area_name = 1 AND [area_name] IS NULL) OR ([area_name] = @Original_area_name)) AND ((@IsNull_city = 1 AND [city] IS NULL) OR ([city] = @Original_city)) AND ((@IsNull_property_description = 1 AND [property_description] IS NULL) OR ([property_description] = @Original_property_description)) AND ((@IsNull_title = 1 AND [title] IS NULL) OR ([title] = @Original_title)) AND ((@IsNull_about_project = 1 AND [about_project] IS NULL) OR ([about_project] = @Original_about_project)) AND ((@IsNull_property_type = 1 AND [property_type] IS NULL) OR ([property_type] = @Original_property_type)) AND ((@IsNull_unit_type = 1 AND [unit_type] IS NULL) OR ([unit_type] = @Original_unit_type)) AND ((@IsNull_floor_number = 1 AND [floor_number] IS NULL) OR ([floor_number] = @Original_floor_number)) AND ((@IsNull_bed_rooms = 1 AND [bed_rooms] IS NULL) OR ([bed_rooms] = @Original_bed_rooms)) AND ((@IsNull_bath_rooms = 1 AND [bath_rooms] IS NULL) OR ([bath_rooms] = @Original_bath_rooms)) AND ((@IsNull_balconies = 1 AND [balconies] IS NULL) OR ([balconies] = @Original_balconies)) AND ((@IsNull_facing = 1 AND [facing] IS NULL) OR ([facing] = @Original_facing)) AND ((@IsNull_area_carpet = 1 AND [area_carpet] IS NULL) OR ([area_carpet] = @Original_area_carpet)) AND ((@IsNull_area_builtup = 1 AND [area_builtup] IS NULL) OR ([area_builtup] = @Original_area_builtup)) AND ((@IsNull_area_unit_type = 1 AND [area_unit_type] IS NULL) OR ([area_unit_type] = @Original_area_unit_type)) AND ((@IsNull_flooring_type = 1 AND [flooring_type] IS NULL) OR ([flooring_type] = @Original_flooring_type)) AND ((@IsNull_intent = 1 AND [intent] IS NULL) OR ([intent] = @Original_intent)) AND ((@IsNull_ownership = 1 AND [ownership] IS NULL) OR ([ownership] = @Original_ownership)) AND ((@IsNull_rent_monthly = 1 AND [rent_monthly] IS NULL) OR ([rent_monthly] = @Original_rent_monthly)) AND ((@IsNull_deposit_amount = 1 AND [deposit_amount] IS NULL) OR ([deposit_amount] = @Original_deposit_amount)) AND ((@IsNull_is_negotiable = 1 AND [is_negotiable] IS NULL) OR ([is_negotiable] = @Original_is_negotiable)) AND ((@IsNull_possession_from = 1 AND [possession_from] IS NULL) OR ([possession_from] = @Original_possession_from)) AND ((@IsNull_property_age_years = 1 AND [property_age_years] IS NULL) OR ([property_age_years] = @Original_property_age_years)) AND ((@IsNull_bachelors_allowed = 1 AND [bachelors_allowed] IS NULL) OR ([bachelors_allowed] = @Original_bachelors_allowed)) AND ((@IsNull_furnish_state = 1 AND [furnish_state] IS NULL) OR ([furnish_state] = @Original_furnish_state)) AND ((@IsNull_two_wheeler_parking = 1 AND [two_wheeler_parking] IS NULL) OR ([two_wheeler_parking] = @Original_two_wheeler_parking)) AND ((@IsNull_four_wheeler_parking = 1 AND [four_wheeler_parking] IS NULL) OR ([four_wheeler_parking] = @Original_four_wheeler_parking)) AND ((@IsNull_address = 1 AND [address] IS NULL) OR ([address] = @Original_address)) AND ((@IsNull_total_floors = 1 AND [total_floors] IS NULL) OR ([total_floors] = @Original_total_floors)) AND ((@IsNull_source_system = 1 AND [source_system] IS NULL) OR ([source_system] = @Original_source_system)) AND ((@IsNull_record_type = 1 AND [record_type] IS NULL) OR ([record_type] = @Original_record_type)) AND ((@IsNull_added_on = 1 AND [added_on] IS NULL) OR ([added_on] = @Original_added_on)) AND ((@IsNull_contact_person_type = 1 AND [contact_person_type] IS NULL) OR ([contact_person_type] = @Original_contact_person_type)) AND ((@IsNull_added_by_type = 1 AND [added_by_type] IS NULL) OR ([added_by_type] = @Original_added_by_type)) AND ((@IsNull_added_by_name = 1 AND [added_by_name] IS NULL) OR ([added_by_name] = @Original_added_by_name)) AND ((@IsNull_latitude = 1 AND [latitude] IS NULL) OR ([latitude] = @Original_latitude)) AND ((@IsNull_longitude = 1 AND [longitude] IS NULL) OR ([longitude] = @Original_longitude)) AND ((@IsNull_last_updated = 1 AND [last_updated] IS NULL) OR ([last_updated] = @Original_last_updated)));
	
SELECT property_id, project_name, project_id, area_id, area_name, city, property_description, title, about_project, property_type, unit_type, floor_number, bed_rooms, bath_rooms, balconies, facing, area_carpet, area_builtup, area_unit_type, flooring_type, intent, ownership, rent_monthly, deposit_amount, is_negotiable, possession_from, property_age_years, bachelors_allowed, furnish_state, two_wheeler_parking, four_wheeler_parking, address, total_floors, source_system, record_type, added_on, contact_person_type, added_by_type, added_by_name, latitude, longitude, last_updated FROM rent_data WHERE (property_id = @property_id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'NewDeleteCommand' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].NewDeleteCommand
GO

CREATE PROCEDURE [dbo].NewDeleteCommand
(
	@Original_property_id varchar(50),
	@IsNull_project_name Int,
	@Original_project_name varchar(50),
	@IsNull_project_id Int,
	@Original_project_id varchar(50),
	@IsNull_area_id Int,
	@Original_area_id varchar(50),
	@IsNull_area_name Int,
	@Original_area_name varchar(50),
	@IsNull_city Int,
	@Original_city varchar(50),
	@IsNull_property_description Int,
	@Original_property_description varchar(256),
	@IsNull_title Int,
	@Original_title varchar(100),
	@IsNull_about_project Int,
	@Original_about_project varchar(100),
	@IsNull_property_type Int,
	@Original_property_type varchar(50),
	@IsNull_unit_type Int,
	@Original_unit_type varchar(20),
	@IsNull_floor_number Int,
	@Original_floor_number int,
	@IsNull_bed_rooms Int,
	@Original_bed_rooms int,
	@IsNull_bath_rooms Int,
	@Original_bath_rooms int,
	@IsNull_balconies Int,
	@Original_balconies int,
	@IsNull_facing Int,
	@Original_facing varchar(50),
	@IsNull_area_carpet Int,
	@Original_area_carpet int,
	@IsNull_area_builtup Int,
	@Original_area_builtup int,
	@IsNull_area_unit_type Int,
	@Original_area_unit_type varchar(50),
	@IsNull_flooring_type Int,
	@Original_flooring_type varchar(50),
	@IsNull_intent Int,
	@Original_intent varchar(50),
	@IsNull_ownership Int,
	@Original_ownership varchar(50),
	@IsNull_rent_monthly Int,
	@Original_rent_monthly int,
	@IsNull_deposit_amount Int,
	@Original_deposit_amount int,
	@IsNull_is_negotiable Int,
	@Original_is_negotiable varchar(50),
	@IsNull_possession_from Int,
	@Original_possession_from date,
	@IsNull_property_age_years Int,
	@Original_property_age_years int,
	@IsNull_bachelors_allowed Int,
	@Original_bachelors_allowed varchar(10),
	@IsNull_furnish_state Int,
	@Original_furnish_state varchar(50),
	@IsNull_two_wheeler_parking Int,
	@Original_two_wheeler_parking varchar(10),
	@IsNull_four_wheeler_parking Int,
	@Original_four_wheeler_parking varchar(10),
	@IsNull_address Int,
	@Original_address varchar(100),
	@IsNull_total_floors Int,
	@Original_total_floors int,
	@IsNull_source_system Int,
	@Original_source_system varchar(30),
	@IsNull_record_type Int,
	@Original_record_type varchar(20),
	@IsNull_added_on Int,
	@Original_added_on date,
	@IsNull_contact_person_type Int,
	@Original_contact_person_type varchar(20),
	@IsNull_added_by_type Int,
	@Original_added_by_type varchar(20),
	@IsNull_added_by_name Int,
	@Original_added_by_name varchar(30),
	@IsNull_latitude Int,
	@Original_latitude varchar(20),
	@IsNull_longitude Int,
	@Original_longitude varchar(20),
	@IsNull_last_updated Int,
	@Original_last_updated datetime
)
AS
	SET NOCOUNT OFF;
DELETE FROM [rent_data] WHERE (([property_id] = @Original_property_id) AND ((@IsNull_project_name = 1 AND [project_name] IS NULL) OR ([project_name] = @Original_project_name)) AND ((@IsNull_project_id = 1 AND [project_id] IS NULL) OR ([project_id] = @Original_project_id)) AND ((@IsNull_area_id = 1 AND [area_id] IS NULL) OR ([area_id] = @Original_area_id)) AND ((@IsNull_area_name = 1 AND [area_name] IS NULL) OR ([area_name] = @Original_area_name)) AND ((@IsNull_city = 1 AND [city] IS NULL) OR ([city] = @Original_city)) AND ((@IsNull_property_description = 1 AND [property_description] IS NULL) OR ([property_description] = @Original_property_description)) AND ((@IsNull_title = 1 AND [title] IS NULL) OR ([title] = @Original_title)) AND ((@IsNull_about_project = 1 AND [about_project] IS NULL) OR ([about_project] = @Original_about_project)) AND ((@IsNull_property_type = 1 AND [property_type] IS NULL) OR ([property_type] = @Original_property_type)) AND ((@IsNull_unit_type = 1 AND [unit_type] IS NULL) OR ([unit_type] = @Original_unit_type)) AND ((@IsNull_floor_number = 1 AND [floor_number] IS NULL) OR ([floor_number] = @Original_floor_number)) AND ((@IsNull_bed_rooms = 1 AND [bed_rooms] IS NULL) OR ([bed_rooms] = @Original_bed_rooms)) AND ((@IsNull_bath_rooms = 1 AND [bath_rooms] IS NULL) OR ([bath_rooms] = @Original_bath_rooms)) AND ((@IsNull_balconies = 1 AND [balconies] IS NULL) OR ([balconies] = @Original_balconies)) AND ((@IsNull_facing = 1 AND [facing] IS NULL) OR ([facing] = @Original_facing)) AND ((@IsNull_area_carpet = 1 AND [area_carpet] IS NULL) OR ([area_carpet] = @Original_area_carpet)) AND ((@IsNull_area_builtup = 1 AND [area_builtup] IS NULL) OR ([area_builtup] = @Original_area_builtup)) AND ((@IsNull_area_unit_type = 1 AND [area_unit_type] IS NULL) OR ([area_unit_type] = @Original_area_unit_type)) AND ((@IsNull_flooring_type = 1 AND [flooring_type] IS NULL) OR ([flooring_type] = @Original_flooring_type)) AND ((@IsNull_intent = 1 AND [intent] IS NULL) OR ([intent] = @Original_intent)) AND ((@IsNull_ownership = 1 AND [ownership] IS NULL) OR ([ownership] = @Original_ownership)) AND ((@IsNull_rent_monthly = 1 AND [rent_monthly] IS NULL) OR ([rent_monthly] = @Original_rent_monthly)) AND ((@IsNull_deposit_amount = 1 AND [deposit_amount] IS NULL) OR ([deposit_amount] = @Original_deposit_amount)) AND ((@IsNull_is_negotiable = 1 AND [is_negotiable] IS NULL) OR ([is_negotiable] = @Original_is_negotiable)) AND ((@IsNull_possession_from = 1 AND [possession_from] IS NULL) OR ([possession_from] = @Original_possession_from)) AND ((@IsNull_property_age_years = 1 AND [property_age_years] IS NULL) OR ([property_age_years] = @Original_property_age_years)) AND ((@IsNull_bachelors_allowed = 1 AND [bachelors_allowed] IS NULL) OR ([bachelors_allowed] = @Original_bachelors_allowed)) AND ((@IsNull_furnish_state = 1 AND [furnish_state] IS NULL) OR ([furnish_state] = @Original_furnish_state)) AND ((@IsNull_two_wheeler_parking = 1 AND [two_wheeler_parking] IS NULL) OR ([two_wheeler_parking] = @Original_two_wheeler_parking)) AND ((@IsNull_four_wheeler_parking = 1 AND [four_wheeler_parking] IS NULL) OR ([four_wheeler_parking] = @Original_four_wheeler_parking)) AND ((@IsNull_address = 1 AND [address] IS NULL) OR ([address] = @Original_address)) AND ((@IsNull_total_floors = 1 AND [total_floors] IS NULL) OR ([total_floors] = @Original_total_floors)) AND ((@IsNull_source_system = 1 AND [source_system] IS NULL) OR ([source_system] = @Original_source_system)) AND ((@IsNull_record_type = 1 AND [record_type] IS NULL) OR ([record_type] = @Original_record_type)) AND ((@IsNull_added_on = 1 AND [added_on] IS NULL) OR ([added_on] = @Original_added_on)) AND ((@IsNull_contact_person_type = 1 AND [contact_person_type] IS NULL) OR ([contact_person_type] = @Original_contact_person_type)) AND ((@IsNull_added_by_type = 1 AND [added_by_type] IS NULL) OR ([added_by_type] = @Original_added_by_type)) AND ((@IsNull_added_by_name = 1 AND [added_by_name] IS NULL) OR ([added_by_name] = @Original_added_by_name)) AND ((@IsNull_latitude = 1 AND [latitude] IS NULL) OR ([latitude] = @Original_latitude)) AND ((@IsNull_longitude = 1 AND [longitude] IS NULL) OR ([longitude] = @Original_longitude)) AND ((@IsNull_last_updated = 1 AND [last_updated] IS NULL) OR ([last_updated] = @Original_last_updated)))
GO

