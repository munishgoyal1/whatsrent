select count(*) as count,  bed_rooms, avg(rent_monthly) avg_rent, avg(deposit_amount) avg_deposit from rent_data 
group by bed_rooms
order by bed_rooms asc


select avg(rent_monthly) rent, unit_type from rent_data where possession_from between '2016-04-01' and '2016-04-30' and city = 'Pune' group by unit_type


select count(*) from rent_data where project_id is null  -- 1500

select count(*) from rent_data where area_name is null or area_name is null -- 0

select * from rent_data where project_id is null 

select * from rent_data where project_id is null 

select distinct area_name from rent_data

select distinct area_id from rent_data
