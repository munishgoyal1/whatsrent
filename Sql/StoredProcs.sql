
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
	@project_url varchar(100),
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
INSERT INTO [rent_data] ([property_id], [project_name], [project_id], [project_url], [area_id], [area_name], [city], [property_description], [title], [about_project], [property_type], [unit_type], [floor_number], [bed_rooms], [bath_rooms], [balconies], [facing], [area_carpet], [area_builtup], [area_unit_type], [flooring_type], [intent], [ownership], [rent_monthly], [deposit_amount], [is_negotiable], [possession_from], [property_age_years], [bachelors_allowed], [furnish_state], [two_wheeler_parking], [four_wheeler_parking], [address], [total_floors], [source_system], [record_type], [added_on], [contact_person_type], [added_by_type], [added_by_name], [latitude], [longitude]) VALUES (@property_id, @project_name, @project_id, @project_url, @area_id, @area_name, @city, @property_description, @title, @about_project, @property_type, @unit_type, @floor_number, @bed_rooms, @bath_rooms, @balconies, @facing, @area_carpet, @area_builtup, @area_unit_type, @flooring_type, @intent, @ownership, @rent_monthly, @deposit_amount, @is_negotiable, @possession_from, @property_age_years, @bachelors_allowed, @furnish_state, @two_wheeler_parking, @four_wheeler_parking, @address, @total_floors, @source_system, @record_type, @added_on, @contact_person_type, @added_by_type, @added_by_name, @latitude, @longitude);
	
SELECT property_id, project_name, project_id, project_url, area_id, area_name, city, property_description, title, about_project, property_type, unit_type, floor_number, bed_rooms, bath_rooms, balconies, facing, area_carpet, area_builtup, area_unit_type, flooring_type, intent, ownership, rent_monthly, deposit_amount, is_negotiable, possession_from, property_age_years, bachelors_allowed, furnish_state, two_wheeler_parking, four_wheeler_parking, address, total_floors, source_system, record_type, added_on, contact_person_type, added_by_type, added_by_name, latitude, longitude, last_updated FROM rent_data WHERE (property_id = @property_id)
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
	@project_url varchar(100),
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
	@bed_rooms decimal(2, 1),
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
	@servant_accomodation varchar(10),
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
INSERT INTO [rent_data] ([property_id], [project_name], [project_url], [project_id], [area_id], [area_name], [city], [property_description], [title], [about_project], [property_type], [unit_type], [floor_number], [bed_rooms], [bath_rooms], [balconies], [facing], [area_carpet], [area_builtup], [area_unit_type], [flooring_type], [intent], [ownership], [rent_monthly], [deposit_amount], [is_negotiable], [possession_from], [property_age_years], [bachelors_allowed], [furnish_state], [servant_accomodation], [two_wheeler_parking], [four_wheeler_parking], [address], [total_floors], [source_system], [record_type], [added_on], [contact_person_type], [added_by_type], [added_by_name], [latitude], [longitude], [last_updated]) VALUES (@property_id, @project_name, @project_url, @project_id, @area_id, @area_name, @city, @property_description, @title, @about_project, @property_type, @unit_type, @floor_number, @bed_rooms, @bath_rooms, @balconies, @facing, @area_carpet, @area_builtup, @area_unit_type, @flooring_type, @intent, @ownership, @rent_monthly, @deposit_amount, @is_negotiable, @possession_from, @property_age_years, @bachelors_allowed, @furnish_state, @servant_accomodation, @two_wheeler_parking, @four_wheeler_parking, @address, @total_floors, @source_system, @record_type, @added_on, @contact_person_type, @added_by_type, @added_by_name, @latitude, @longitude, @last_updated);
	
SELECT property_id, project_name, project_url, project_id, area_id, area_name, city, property_description, title, about_project, property_type, unit_type, floor_number, bed_rooms, bath_rooms, balconies, facing, area_carpet, area_builtup, area_unit_type, flooring_type, intent, ownership, rent_monthly, deposit_amount, is_negotiable, possession_from, property_age_years, bachelors_allowed, furnish_state, servant_accomodation, two_wheeler_parking, four_wheeler_parking, address, total_floors, source_system, record_type, added_on, contact_person_type, added_by_type, added_by_name, latitude, longitude, last_updated FROM rent_data WHERE (property_id = @property_id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'NewUpdateCommand' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].NewUpdateCommand
GO

CREATE PROCEDURE [dbo].NewUpdateCommand
(
	@property_id varchar(50),
	@project_name varchar(50),
	@project_url varchar(100),
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
	@bed_rooms decimal(2, 1),
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
	@servant_accomodation varchar(10),
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
	@Original_property_id varchar(50)
)
AS
	SET NOCOUNT OFF;
UPDATE [rent_data] SET [property_id] = @property_id, [project_name] = @project_name, [project_url] = @project_url, [project_id] = @project_id, [area_id] = @area_id, [area_name] = @area_name, [city] = @city, [property_description] = @property_description, [title] = @title, [about_project] = @about_project, [property_type] = @property_type, [unit_type] = @unit_type, [floor_number] = @floor_number, [bed_rooms] = @bed_rooms, [bath_rooms] = @bath_rooms, [balconies] = @balconies, [facing] = @facing, [area_carpet] = @area_carpet, [area_builtup] = @area_builtup, [area_unit_type] = @area_unit_type, [flooring_type] = @flooring_type, [intent] = @intent, [ownership] = @ownership, [rent_monthly] = @rent_monthly, [deposit_amount] = @deposit_amount, [is_negotiable] = @is_negotiable, [possession_from] = @possession_from, [property_age_years] = @property_age_years, [bachelors_allowed] = @bachelors_allowed, [furnish_state] = @furnish_state, [servant_accomodation] = @servant_accomodation, [two_wheeler_parking] = @two_wheeler_parking, [four_wheeler_parking] = @four_wheeler_parking, [address] = @address, [total_floors] = @total_floors, [source_system] = @source_system, [record_type] = @record_type, [added_on] = @added_on, [contact_person_type] = @contact_person_type, [added_by_type] = @added_by_type, [added_by_name] = @added_by_name, [latitude] = @latitude, [longitude] = @longitude, [last_updated] = @last_updated WHERE (([property_id] = @Original_property_id));
	
SELECT property_id, project_name, project_url, project_id, area_id, area_name, city, property_description, title, about_project, property_type, unit_type, floor_number, bed_rooms, bath_rooms, balconies, facing, area_carpet, area_builtup, area_unit_type, flooring_type, intent, ownership, rent_monthly, deposit_amount, is_negotiable, possession_from, property_age_years, bachelors_allowed, furnish_state, servant_accomodation, two_wheeler_parking, four_wheeler_parking, address, total_floors, source_system, record_type, added_on, contact_person_type, added_by_type, added_by_name, latitude, longitude, last_updated FROM rent_data WHERE (property_id = @property_id)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'NewDeleteCommand' AND user_name(uid) = 'dbo')
	DROP PROCEDURE [dbo].NewDeleteCommand
GO

CREATE PROCEDURE [dbo].NewDeleteCommand
(
	@Original_property_id varchar(50)
)
AS
	SET NOCOUNT OFF;
DELETE FROM [rent_data] WHERE (([property_id] = @Original_property_id))
GO

