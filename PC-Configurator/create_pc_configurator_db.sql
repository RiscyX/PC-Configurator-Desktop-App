-- Adatbázis létrehozása (ha még nincs)
CREATE DATABASE PCConfiguratorDB;
GO

USE PCConfiguratorDB;
GO

-- Felhasználók tábla
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('user', 'admin')),
    RegisteredAt DATETIME DEFAULT GETDATE()
);

-- Konfigurációk tábla
CREATE TABLE Configurations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Processzorok (CPU) tábla
CREATE TABLE CPUs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Manufacturer NVARCHAR(100),
    Cores INT,
    Threads INT,
    BaseClockGHz FLOAT,
    BoostClockGHz FLOAT
);

-- Videókártyák (GPU) tábla
CREATE TABLE GPUs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Manufacturer NVARCHAR(100),
    MemoryGB INT
);

-- Memória (RAM) tábla
CREATE TABLE RAMs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CapacityGB INT,
    SpeedMHz INT,
    Type NVARCHAR(50)
);

-- Háttértárak (Storage) tábla
CREATE TABLE Storages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(50), -- HDD, SSD, NVMe
    CapacityGB INT
);

-- Alaplapok (Motherboard) tábla
CREATE TABLE Motherboards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Manufacturer NVARCHAR(100),
    Chipset NVARCHAR(50),
    Socket NVARCHAR(50)
);

-- Tápegységek (PSU) tábla
CREATE TABLE PSUs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Wattage INT,
    EfficiencyRating NVARCHAR(20)
);

-- Gépházak (Case) tábla
CREATE TABLE Cases (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    FormFactor NVARCHAR(50)
);

-- Konfigurációk és komponensek összekapcsolása
CREATE TABLE ConfigurationComponents (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ConfigurationId INT NOT NULL,
    CPUId INT,
    GPUId INT,
    RAMId INT,
    StorageId INT,
    MotherboardId INT,
    PSUId INT,
    CaseId INT,
    FOREIGN KEY (ConfigurationId) REFERENCES Configurations(Id),
    FOREIGN KEY (CPUId) REFERENCES CPUs(Id),
    FOREIGN KEY (GPUId) REFERENCES GPUs(Id),
    FOREIGN KEY (RAMId) REFERENCES RAMs(Id),
    FOREIGN KEY (StorageId) REFERENCES Storages(Id),
    FOREIGN KEY (MotherboardId) REFERENCES Motherboards(Id),
    FOREIGN KEY (PSUId) REFERENCES PSUs(Id),
    FOREIGN KEY (CaseId) REFERENCES Cases(Id)
);