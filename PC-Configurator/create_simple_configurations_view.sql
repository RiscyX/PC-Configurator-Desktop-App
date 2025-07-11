-- Új nézet létrehozása a meglévő oszlopstruktúrával
CREATE OR ALTER VIEW [dbo].[vw_Configurations] AS
SELECT
    c.Id AS ConfigurationId,
    c.Name AS ConfigurationName,
    u.Email AS OwnerEmail,
    
    -- Komponens adatok - csak a nevüket tároljuk
    ISNULL(cpu.Name, 'Nincs megadva') AS CPU,
    ISNULL(mb.Name, 'Nincs megadva') AS Motherboard,
    ISNULL(gpu.Name, 'Nincs megadva') AS GPU,
    ISNULL(ram.Name, 'Nincs megadva') AS RAM,
    ISNULL(s.Name, 'Nincs megadva') AS Storage,
    ISNULL(psu.Name, 'Nincs megadva') AS PSU,
    ISNULL(cs.Name, 'Nincs megadva') AS CaseName
FROM
    Configurations c
LEFT JOIN
    Users u ON c.UserId = u.Id
LEFT JOIN
    ConfigurationComponents cc ON c.Id = cc.ConfigurationId
LEFT JOIN
    CPUs cpu ON cc.CPUId = cpu.Id
LEFT JOIN
    Motherboards mb ON cc.MotherboardId = mb.Id
LEFT JOIN
    GPUs gpu ON cc.GPUId = gpu.Id
LEFT JOIN
    RAMs ram ON cc.RAMId = ram.Id
LEFT JOIN
    Storages s ON cc.StorageId = s.Id
LEFT JOIN
    PSUs psu ON cc.PSUId = psu.Id
LEFT JOIN
    Cases cs ON cc.CaseId = cs.Id;
