-- vw_FullConfigurations nézet létrehozása
CREATE OR ALTER VIEW vw_FullConfigurations
AS
SELECT
    c.Id AS ConfigurationId,
    c.Name AS ConfigurationName,
    c.UserId,
    c.CreatedAt,
    
    -- CPU adatok
    cc.CPUId,
    cpu.Name AS CPUName,
    cpu.Manufacturer AS CPUManufacturer,
    cpu.Price AS CPUPrice,
    cpu.PowerConsumption AS CPUPower,
    
    -- GPU adatok
    cc.GPUId,
    gpu.Name AS GPUName,
    gpu.Manufacturer AS GPUManufacturer,
    gpu.Price AS GPUPrice,
    gpu.PowerConsumption AS GPUPower,
    
    -- RAM adatok
    cc.RAMId,
    ram.Name AS RAMName,
    ram.CapacityGB AS RAMCapacity,
    ram.Type AS RAMType,
    ram.SpeedMHz AS RAMSpeed,
    ram.Price AS RAMPrice,
    ram.PowerConsumption AS RAMPower,
    
    -- Storage adatok
    cc.StorageId,
    storage.Name AS StorageName,
    storage.Type AS StorageType,
    storage.CapacityGB AS StorageCapacity,
    storage.Price AS StoragePrice,
    storage.PowerConsumption AS StoragePower,
    
    -- Motherboard adatok
    cc.MotherboardId,
    mb.Name AS MotherboardName,
    mb.Manufacturer AS MotherboardManufacturer,
    mb.Price AS MotherboardPrice,
    mb.PowerConsumption AS MotherboardPower,
    
    -- PSU adatok
    cc.PSUId,
    psu.Name AS PSUName,
    psu.Wattage AS PSUWattage,
    psu.EfficiencyRating AS PSUEfficiency,
    psu.Price AS PSUPrice,
    
    -- Case adatok
    cc.CaseId,
    cs.Name AS CaseName,
    cs.FormFactor AS CaseFormFactor,
    cs.Color AS CaseColor,
    cs.Price AS CasePrice,
    
    -- Összesített adatok
    (ISNULL(cpu.Price, 0) + ISNULL(gpu.Price, 0) + ISNULL(ram.Price, 0) + 
     ISNULL(storage.Price, 0) + ISNULL(mb.Price, 0) + 
     ISNULL(psu.Price, 0) + ISNULL(cs.Price, 0)) AS TotalPrice,
    
    (ISNULL(cpu.PowerConsumption, 0) + ISNULL(gpu.PowerConsumption, 0) + 
     ISNULL(ram.PowerConsumption, 0) + ISNULL(storage.PowerConsumption, 0) + 
     ISNULL(mb.PowerConsumption, 0)) AS TotalPowerConsumption
FROM
    Configurations c
LEFT JOIN
    ConfigurationComponents cc ON c.Id = cc.ConfigurationId
LEFT JOIN
    CPUs cpu ON cc.CPUId = cpu.Id
LEFT JOIN
    GPUs gpu ON cc.GPUId = gpu.Id
LEFT JOIN
    RAMs ram ON cc.RAMId = ram.Id
LEFT JOIN
    Storages storage ON cc.StorageId = storage.Id
LEFT JOIN
    Motherboards mb ON cc.MotherboardId = mb.Id
LEFT JOIN
    PSUs psu ON cc.PSUId = psu.Id
LEFT JOIN
    Cases cs ON cc.CaseId = cs.Id;
