CREATE DATABASE RACEDAY
CREATE TABLE Users(
user_id INT IDENTITY(1,1) PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('organiser', 'participant', 'admin')),
    phone VARCHAR(20) NULL,
    id_number VARCHAR(20) NULL,
    date_of_birth DATE NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- ============================================
-- 2. EVENTS TABLE
-- ============================================
CREATE TABLE Events (
    event_id INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description VARCHAR(MAX) NULL,
    date_time DATETIME NOT NULL,
    location VARCHAR(255) NOT NULL,
    max_participants INT NOT NULL,
    entry_fee DECIMAL(10,2) NOT NULL DEFAULT 0,
    early_bird_fee DECIMAL(10,2) NULL,
    early_bird_date DATETIME NULL,
    status VARCHAR(20) DEFAULT 'upcoming' CHECK (status IN ('upcoming', 'ongoing', 'completed', 'cancelled')),
    created_by INT NOT NULL FOREIGN KEY REFERENCES Users(user_id),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- ============================================
-- 3. CATEGORIES TABLE
-- ============================================
CREATE TABLE Categories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL FOREIGN KEY REFERENCES Events(event_id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(255) NULL,
    entry_fee DECIMAL(10,2) NOT NULL,
    start_time TIME NULL,
    distance_km DECIMAL(5,2) NOT NULL,
    capacity INT NULL,
    created_at DATETIME DEFAULT GETDATE()
);

-- ============================================
-- 4. ENROLLMENTS TABLE
-- ============================================
CREATE TABLE Enrollments (
    enrollment_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES Users(user_id),
    event_id INT NOT NULL FOREIGN KEY REFERENCES Events(event_id),
    category_id INT NOT NULL FOREIGN KEY REFERENCES Categories(category_id),
    
    -- Registration status
    status VARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'cancelled', 'completed', 'DNS', 'DNF')),
    entry_date DATETIME DEFAULT GETDATE(),
    
    -- Payment fields
    payment_status VARCHAR(20) DEFAULT 'pending' CHECK (payment_status IN ('pending', 'paid', 'refunded', 'failed')),
    payment_method VARCHAR(50) NULL CHECK (payment_method IN ('card', 'eft', 'snapscan', 'zapper', 'cash', 'voucher')),
    payment_ref VARCHAR(100) NULL,
    amount_paid DECIMAL(10,2) DEFAULT 0,
    payment_date DATETIME NULL,
    invoice_number NVARCHAR(50) UNIQUE NULL,
    
    -- Participant details
    participant_name VARCHAR(255) NOT NULL,
    participant_user_id INT NULL FOREIGN KEY REFERENCES Users(user_id),
    participant_id_number NVARCHAR(20) NOT NULL,
    participant_dob DATE NULL,
    gender VARCHAR(10) NULL CHECK (gender IN ('male', 'female', 'other')),
    
    -- Emergency contact
    emergency_contact_name VARCHAR(255) NULL,
    emergency_contact_phone VARCHAR(20) NULL,
    medical_conditions VARCHAR(MAX) NULL,
    
    -- Race day
    bib_number VARCHAR(20) UNIQUE NULL,
    wave_start TIME NULL,
    age_category VARCHAR(20) NULL,
    
    updated_at DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT UC_Enrollment UNIQUE (user_id, event_id, category_id)
);

-- ============================================
-- 5. RESULTS TABLE
-- ============================================
CREATE TABLE Results (
    result_id INT IDENTITY(1,1) PRIMARY KEY,
    enrollment_id INT NOT NULL FOREIGN KEY REFERENCES Enrollments(enrollment_id) UNIQUE,
    finish_time TIME NOT NULL,
    position INT NOT NULL,
    overall_rank INT NOT NULL,
    category_rank INT NOT NULL,
    certified BIT DEFAULT 0,
    certificate_url VARCHAR(500) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- ============================================
-- 6. WEATHERINFO TABLE
-- ============================================
CREATE TABLE WeatherInfo (
    weather_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL FOREIGN KEY REFERENCES Events(event_id) UNIQUE,
    forecast_date DATE NOT NULL,
    temperature DECIMAL(5,2) NULL,
    conditions VARCHAR(100) NULL,
    wind_speed DECIMAL(5,2) NULL,
    humidity DECIMAL(5,2) NULL,
    updated_at DATETIME DEFAULT GETDATE()
);

-- ============================================
-- SEED DATA
-- ============================================

-- Insert Organisers
INSERT INTO Users (email, password_hash, first_name, last_name, role, phone, id_number)
VALUES 
('sandra@comrades.co.za', 'hashed_password_1', 'Sandra', 'Mkhize', 'organiser', '+27821234567', '7501011234081'),
('michael@twooceans.co.za', 'hashed_password_2', 'Michael', 'van der Merwe', 'organiser', '+27829876543', '8005155678082'
);

-- Insert Participants
INSERT INTO Users (email, password_hash, first_name, last_name, role, phone, id_number, date_of_birth)
VALUES 
('thabo@email.com', 'hashed_password_3', 'Thabo', 'Ndlovu', 'participant', '+27831234567', '9001011234081', '1990-01-01'),
('linda@email.com', 'hashed_password_4', 'Linda', 'Botha', 'participant', '+27837654321', '8505155678082', '1985-05-15'),
('sipho@email.com', 'hashed_password_5', 'Sipho', 'Zulu', 'participant', '+27835551111', '9208089876083', '1992-08-08'),
('johannes@email.com', 'hashed_password_6', 'Johannes', 'Smit', 'participant', '+27834442222', '7802123456084', '1978-02-12');

-- Insert Events
INSERT INTO Events (title, description, date_time, location, max_participants, entry_fee, early_bird_fee, early_bird_date, created_by)
VALUES 
('Comrades Marathon', 'The Ultimate Human Race - 90km between Pietermaritzburg and Durban. The world''s oldest and largest ultra marathon.', '2026-06-07 05:30:00', 'Pietermaritzburg to Durban', 20000, 1200.00, 850.00, '2026-03-31 23:59:00', 1),
('Two Oceans Marathon', '56km Ultra Marathon around the Cape Peninsula. Known as the world''s most beautiful marathon.', '2026-04-04 06:00:00', 'Cape Town', 12000, 1000.00, 750.00, '2026-02-28 23:59:00', 2),
('Soweto Marathon', 'Race through the streets of Soweto, celebrating South African heritage and community spirit.', '2026-11-01 05:45:00', 'Soweto, Johannesburg', 10000, 750.00, 550.00, '2026-08-31 23:59:00', 1);
