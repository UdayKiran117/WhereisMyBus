-- DESTRUCTIVE: manually execute only for a disposable database, after backups.
USE where_is_my_bus;
DROP TABLE IF EXISTS application_audit_logs,user_refresh_tokens,passenger_favorites,bus_latest_location,bus_location_history;
DROP TABLE IF EXISTS tracking_sessions,bus_route_assignment,route_stops,route_master,location_master,bus_driver_assignment,driver_master,bus_master,app_users,app_roles;
