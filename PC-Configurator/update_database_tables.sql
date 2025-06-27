-- Add missing columns to component tables

-- CPUs - Add Price and Socket
ALTER TABLE CPUs ADD
    Socket NVARCHAR(50),
    Price DECIMAL(10, 2) DEFAULT 0,
    PowerConsumption INT DEFAULT 0;

-- GPUs - Add Price, MemoryType and PowerConsumption
ALTER TABLE GPUs ADD
    MemoryType NVARCHAR(50) DEFAULT 'GDDR6',
    Price DECIMAL(10, 2) DEFAULT 0,
    PowerConsumption INT DEFAULT 0;

-- RAMs - Add Price
ALTER TABLE RAMs ADD
    Price DECIMAL(10, 2) DEFAULT 0,
    PowerConsumption INT DEFAULT 0;

-- Storages - Add Price and PowerConsumption
ALTER TABLE Storages ADD
    Price DECIMAL(10, 2) DEFAULT 0,
    PowerConsumption INT DEFAULT 0;

-- Motherboards - Add Price, PowerConsumption and FormFactor
ALTER TABLE Motherboards ADD
    FormFactor NVARCHAR(50),
    Price DECIMAL(10, 2) DEFAULT 0,
    PowerConsumption INT DEFAULT 0;

-- PSUs - Add Price
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PSUs') AND name = 'Price')
BEGIN
    ALTER TABLE PSUs ADD Price DECIMAL(10, 2) DEFAULT 0;
END

-- Cases - Add Price and Color
ALTER TABLE Cases ADD
    Color NVARCHAR(50) DEFAULT 'Black',
    Price DECIMAL(10, 2) DEFAULT 0;

-- Note: The script uses ALTER TABLE ADD without checking if columns exist
-- In a production environment, you should check if columns exist before adding them
-- For each table, you might want to use:
-- IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('table_name') AND name = 'column_name')
-- BEGIN
--    ALTER TABLE table_name ADD column_name data_type;
-- END
