USE where_is_my_bus;
SET time_zone = '+00:00';
START TRANSACTION;
INSERT INTO app_roles(id,name) VALUES(1,'Passenger'),(2,'Driver'),(3,'Administrator');
-- Sample catalog only. No accounts, passwords or live location are seeded.
INSERT INTO bus_master(id,registration_number,operator_name,display_name) VALUES
(1,'TS09AB1234','Pilot Transport','Hyderabad to Warangal'),(2,'TS09CD5678','Pilot Transport','Hyderabad to Warangal Express');
INSERT INTO location_master(id,name,latitude,longitude) VALUES(1,'Hyderabad',17.3850,78.4867),(2,'Bhongir',17.5154,78.8853),(3,'Warangal',17.9689,79.5941);
INSERT INTO route_master(id,name,source_id,destination_id) VALUES(1,'Hyderabad to Warangal',1,3);
INSERT INTO route_stops(route_id,location_id,stop_order,distance_from_source_km) VALUES(1,1,1,0),(1,2,2,50),(1,3,3,145);
INSERT INTO bus_route_assignment(bus_id,route_id) VALUES(1,1),(2,1);
COMMIT;
