USE where_is_my_bus;
SET time_zone = '+00:00';
CREATE TABLE app_roles (
 id SMALLINT PRIMARY KEY, name VARCHAR(30) NOT NULL UNIQUE
) ENGINE=InnoDB;
CREATE TABLE app_users (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, role_id SMALLINT NOT NULL,
 mobile_number VARCHAR(20) NOT NULL UNIQUE, display_name VARCHAR(120) NOT NULL,
 password_hash VARCHAR(512) NOT NULL, is_active BOOLEAN NOT NULL DEFAULT TRUE,
 created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), updated_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_user_role FOREIGN KEY(role_id) REFERENCES app_roles(id)
) ENGINE=InnoDB;
CREATE TABLE bus_master (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, registration_number VARCHAR(15) CHARACTER SET ascii COLLATE ascii_bin NOT NULL UNIQUE,
 operator_name VARCHAR(160) NOT NULL, display_name VARCHAR(160) NOT NULL, is_active BOOLEAN NOT NULL DEFAULT TRUE,
 created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), updated_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)
) ENGINE=InnoDB;
CREATE TABLE driver_master (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, user_id BIGINT NOT NULL UNIQUE, license_number VARCHAR(60) NOT NULL UNIQUE,
 is_active BOOLEAN NOT NULL DEFAULT TRUE, created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_driver_user FOREIGN KEY(user_id) REFERENCES app_users(id)
) ENGINE=InnoDB;
CREATE TABLE bus_driver_assignment (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, bus_id BIGINT NOT NULL, driver_id BIGINT NOT NULL,
 valid_from DATETIME(6) NOT NULL, valid_to DATETIME(6) NULL, is_active BOOLEAN NOT NULL DEFAULT TRUE,
 created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_assignment_bus FOREIGN KEY(bus_id) REFERENCES bus_master(id),
 CONSTRAINT fk_assignment_driver FOREIGN KEY(driver_id) REFERENCES driver_master(id),
 CONSTRAINT ck_assignment_dates CHECK(valid_to IS NULL OR valid_to > valid_from)
) ENGINE=InnoDB;
CREATE TABLE location_master (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(160) NOT NULL UNIQUE,
 latitude DOUBLE NULL, longitude DOUBLE NULL, is_active BOOLEAN NOT NULL DEFAULT TRUE,
 created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT ck_location_lat CHECK(latitude IS NULL OR latitude BETWEEN -90 AND 90),
 CONSTRAINT ck_location_lon CHECK(longitude IS NULL OR longitude BETWEEN -180 AND 180)
) ENGINE=InnoDB;
CREATE TABLE route_master (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(160) NOT NULL, source_id BIGINT NOT NULL, destination_id BIGINT NOT NULL,
 is_active BOOLEAN NOT NULL DEFAULT TRUE, created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_route_source FOREIGN KEY(source_id) REFERENCES location_master(id),
 CONSTRAINT fk_route_destination FOREIGN KEY(destination_id) REFERENCES location_master(id),
 CONSTRAINT ck_route_endpoints CHECK(source_id <> destination_id),
 UNIQUE KEY uq_route_name(name,source_id,destination_id)
) ENGINE=InnoDB;
CREATE TABLE route_stops (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, route_id BIGINT NOT NULL, location_id BIGINT NOT NULL, stop_order INT NOT NULL,
 distance_from_source_km DECIMAL(9,3) NULL,
 CONSTRAINT fk_stop_route FOREIGN KEY(route_id) REFERENCES route_master(id),
 CONSTRAINT fk_stop_location FOREIGN KEY(location_id) REFERENCES location_master(id),
 CONSTRAINT ck_stop_order CHECK(stop_order>0), UNIQUE KEY uq_stop_order(route_id,stop_order), UNIQUE KEY uq_stop_location(route_id,location_id)
) ENGINE=InnoDB;
CREATE TABLE bus_route_assignment (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, bus_id BIGINT NOT NULL, route_id BIGINT NOT NULL, is_active BOOLEAN NOT NULL DEFAULT TRUE,
 created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_bus_route_bus FOREIGN KEY(bus_id) REFERENCES bus_master(id),
 CONSTRAINT fk_bus_route_route FOREIGN KEY(route_id) REFERENCES route_master(id), UNIQUE KEY uq_bus_route(bus_id,route_id)
) ENGINE=InnoDB;
CREATE TABLE tracking_sessions (
 id CHAR(36) CHARACTER SET ascii COLLATE ascii_bin PRIMARY KEY, bus_id BIGINT NOT NULL, driver_id BIGINT NOT NULL, route_id BIGINT NOT NULL,
 started_at DATETIME(6) NOT NULL, ended_at DATETIME(6) NULL, consent_at DATETIME(6) NOT NULL,
 active_bus_id BIGINT GENERATED ALWAYS AS (CASE WHEN ended_at IS NULL THEN bus_id ELSE NULL END) STORED,
 active_driver_id BIGINT GENERATED ALWAYS AS (CASE WHEN ended_at IS NULL THEN driver_id ELSE NULL END) STORED,
 CONSTRAINT fk_session_bus FOREIGN KEY(bus_id) REFERENCES bus_master(id),
 CONSTRAINT fk_session_driver FOREIGN KEY(driver_id) REFERENCES driver_master(id),
 CONSTRAINT fk_session_route FOREIGN KEY(route_id) REFERENCES route_master(id),
 CONSTRAINT ck_session_dates CHECK(ended_at IS NULL OR ended_at>=started_at),
 UNIQUE KEY uq_active_bus(active_bus_id), UNIQUE KEY uq_active_driver(active_driver_id), UNIQUE KEY uq_session_bus(id,bus_id)
) ENGINE=InnoDB;
CREATE TABLE bus_location_history (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, update_id CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL UNIQUE,
 session_id CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL, bus_id BIGINT NOT NULL,
 latitude DOUBLE NOT NULL, longitude DOUBLE NOT NULL, speed_mps DOUBLE NULL, heading DOUBLE NULL, accuracy_meters DOUBLE NOT NULL,
 gps_timestamp DATETIME(6) NOT NULL, received_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_history_session FOREIGN KEY(session_id,bus_id) REFERENCES tracking_sessions(id,bus_id),
 CONSTRAINT ck_history_lat CHECK(latitude BETWEEN -90 AND 90), CONSTRAINT ck_history_lon CHECK(longitude BETWEEN -180 AND 180),
 CONSTRAINT ck_history_speed CHECK(speed_mps IS NULL OR speed_mps>=0), CONSTRAINT ck_history_heading CHECK(heading IS NULL OR (heading>=0 AND heading<360)),
 CONSTRAINT ck_history_accuracy CHECK(accuracy_meters>=0)
) ENGINE=InnoDB;
CREATE TABLE bus_latest_location (
 bus_id BIGINT PRIMARY KEY, session_id CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
 update_id CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
 latitude DOUBLE NOT NULL, longitude DOUBLE NOT NULL, speed_mps DOUBLE NULL, heading DOUBLE NULL, accuracy_meters DOUBLE NOT NULL,
 gps_timestamp DATETIME(6) NOT NULL, received_at DATETIME(6) NOT NULL,
 CONSTRAINT fk_latest_session FOREIGN KEY(session_id,bus_id) REFERENCES tracking_sessions(id,bus_id),
 CONSTRAINT ck_latest_lat CHECK(latitude BETWEEN -90 AND 90), CONSTRAINT ck_latest_lon CHECK(longitude BETWEEN -180 AND 180),
 CONSTRAINT ck_latest_speed CHECK(speed_mps IS NULL OR speed_mps>=0), CONSTRAINT ck_latest_heading CHECK(heading IS NULL OR (heading>=0 AND heading<360)),
 CONSTRAINT ck_latest_accuracy CHECK(accuracy_meters>=0)
) ENGINE=InnoDB;
CREATE TABLE passenger_favorites (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, user_id BIGINT NOT NULL, bus_id BIGINT NOT NULL,
 created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_favorite_user FOREIGN KEY(user_id) REFERENCES app_users(id), CONSTRAINT fk_favorite_bus FOREIGN KEY(bus_id) REFERENCES bus_master(id),
 UNIQUE KEY uq_user_favorite(user_id,bus_id)
) ENGINE=InnoDB;
CREATE TABLE user_refresh_tokens (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, user_id BIGINT NOT NULL, family_id CHAR(36) NOT NULL,
 token_hash BINARY(32) NOT NULL UNIQUE, expires_at DATETIME(6) NOT NULL, revoked_at DATETIME(6) NULL, replaced_by_hash BINARY(32) NULL,
 created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), CONSTRAINT fk_token_user FOREIGN KEY(user_id) REFERENCES app_users(id)
) ENGINE=InnoDB;
CREATE TABLE application_audit_logs (
 id BIGINT PRIMARY KEY AUTO_INCREMENT, user_id BIGINT NULL, action VARCHAR(80) NOT NULL, entity_type VARCHAR(60) NOT NULL,
 entity_id VARCHAR(60) NULL, trace_id VARCHAR(80) NULL, occurred_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
 CONSTRAINT fk_audit_user FOREIGN KEY(user_id) REFERENCES app_users(id)
) ENGINE=InnoDB;
