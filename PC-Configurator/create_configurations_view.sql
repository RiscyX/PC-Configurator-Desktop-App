-- Új átfogó nézet létrehozása a konfigurációkhoz
CREATE OR ALTER VIEW [dbo].[vw_FullConfigurations] AS
SELECT 
    c.Id AS ConfigurationId,
    c.Name AS ConfigurationName,
    c.UserId,
    c.CreatedAt,
    cc.CPUId,
    cpu.Name AS CPUName,
    cpu.Manufacturer AS CPUManufacturer,
    cpu.Price AS CPUPrice,
    cpu.PowerConsumption AS CPUPower,
    cc.GPUId,
    gpu.Name AS GPUName,
    gpu.Manufacturer AS GPUManufacturer,
    gpu.Price AS GPUPrice,
    gpu.PowerConsumption AS GPUPower,
    cc.RAMId,
    ram.Name AS RAMName,
    ram.CapacityGB AS RAMCapacity,
    ram.Type AS RAMType,
    ram.SpeedMHz AS RAMSpeed,
    ram.Price AS RAMPrice,
    ram.PowerConsumption AS RAMPower,
    cc.StorageId,
    s.Name AS StorageName,
    s.Type AS StorageType,
    s.CapacityGB AS StorageCapacity,
    s.Price AS StoragePrice,
    s.PowerConsumption AS StoragePower,
    cc.MotherboardId,
    mb.Name AS MotherboardName,
    mb.Manufacturer AS MotherboardManufacturer,
    mb.Price AS MotherboardPrice,
    mb.PowerConsumption AS MotherboardPower,
    cc.PSUId,
    psu.Name AS PSUName,
    psu.Wattage AS PSUWattage,
    psu.EfficiencyRating AS PSUEfficiency,
    psu.Price AS PSUPrice,
    cc.CaseId,
    cs.Name AS CaseName,
    cs.FormFactor AS CaseFormFactor,
    cs.Color AS CaseColor,
    cs.Price AS CasePrice,
    (ISNULL(cpu.Price, 0) + ISNULL(gpu.Price, 0) + ISNULL(ram.Price, 0) + 
     ISNULL(s.Price, 0) + ISNULL(mb.Price, 0) + ISNULL(psu.Price, 0) + 
     ISNULL(cs.Price, 0)) AS TotalPrice,
    (ISNULL(cpu.PowerConsumption, 0) + ISNULL(gpu.PowerConsumption, 0) + 
     ISNULL(ram.PowerConsumption, 0) + ISNULL(s.PowerConsumption, 0) + 
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
    Storages s ON cc.StorageId = s.Id
LEFT JOIN 
    Motherboards mb ON cc.MotherboardId = mb.Id
LEFT JOIN 
    PSUs psu ON cc.PSUId = psu.Id
LEFT JOIN 
    Cases cs ON cc.CaseId = cs.Id;
