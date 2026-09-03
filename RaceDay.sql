CREATE DATABASE RACEDAY
CREATE TABLE Users(
user_id INT IDENTITY(1,1) PRIMARY KEY,
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    first_name NVARCHAR(100) NOT NULL,
    last_name NVARCHAR(100) NOT NULL,
    role NVARCHAR(20) NOT NULL CHECK (role IN ('organiser', 'participant', 'admin')),
    phone NVARCHAR(20) NULL,
    id_number NVARCHAR(20) NULL,
    date_of_birth DATE NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- ============================================
-- 2. EVENTS TABLE
-- ============================================
CREATE TABLE Events (
    event_id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX) NULL,
    date_time DATETIME NOT NULL,
    location NVARCHAR(255) NOT NULL,
    max_participants INT NOT NULL,
    entry_fee DECIMAL(10,2) NOT NULL DEFAULT 0,
    early_bird_fee DECIMAL(10,2) NULL,
    early_bird_date DATETIME NULL,
    status NVARCHAR(20) DEFAULT 'upcoming' CHECK (status IN ('upcoming', 'ongoing', 'completed', 'cancelled')),
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
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255) NULL,
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
    status NVARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'cancelled', 'completed', 'DNS', 'DNF')),
    entry_date DATETIME DEFAULT GETDATE(),
    
    -- Payment fields
    payment_status NVARCHAR(20) DEFAULT 'pending' CHECK (payment_status IN ('pending', 'paid', 'refunded', 'failed')),
    payment_method NVARCHAR(50) NULL CHECK (payment_method IN ('card', 'eft', 'snapscan', 'zapper', 'cash', 'voucher')),
    payment_ref NVARCHAR(100) NULL,
    amount_paid DECIMAL(10,2) DEFAULT 0,
    payment_date DATETIME NULL,
    invoice_number NVARCHAR(50) UNIQUE NULL,
    
    -- Participant details
    participant_name NVARCHAR(255) NOT NULL,
    participant_user_id INT NULL FOREIGN KEY REFERENCES Users(user_id),
    participant_id_number NVARCHAR(20) NOT NULL,
    participant_dob DATE NULL,
    gender NVARCHAR(10) NULL CHECK (gender IN ('male', 'female', 'other')),
    
    -- Emergency contact
    emergency_contact_name NVARCHAR(255) NULL,
    emergency_contact_phone NVARCHAR(20) NULL,
    medical_conditions NVARCHAR(MAX) NULL,
    
    -- Race day
    bib_number NVARCHAR(20) UNIQUE NULL,
    wave_start TIME NULL,
    age_category NVARCHAR(20) NULL,
    
    updated_at DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT UC_Enrollment UNIQUE (user_id, event_id, category_id)
);
