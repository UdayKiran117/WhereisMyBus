-- Read-only smoke checks after 001 through 004 on a fresh MySQL instance.
USE where_is_my_bus;
SELECT COUNT(*) AS TableCount FROM information_schema.tables WHERE table_schema=DATABASE() AND table_type='BASE TABLE'; -- 15
SELECT COUNT(*) AS BusCount FROM bus_master; -- 2
SELECT COUNT(*) AS LocationCount FROM location_master; -- 3
SELECT COUNT(*) AS RouteCount FROM route_master; -- 1
SELECT b.registration_number FROM bus_master b
JOIN bus_route_assignment a ON a.bus_id=b.id AND a.is_active=1
JOIN route_master r ON r.id=a.route_id AND r.is_active=1
JOIN route_stops f ON f.route_id=r.id AND f.location_id=1
JOIN route_stops t ON t.route_id=r.id AND t.location_id=3
WHERE b.is_active=1 AND f.stop_order<t.stop_order; -- 2 buses
SELECT COUNT(*) AS ActiveSessionCount FROM tracking_sessions WHERE ended_at IS NULL; -- 0
