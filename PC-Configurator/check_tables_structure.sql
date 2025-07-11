-- Konfiguráció tábla szerkezetének ellenőrzése
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'Configurations';

-- Komponens kapcsolatok tábla szerkezetének ellenőrzése
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'ConfigurationComponents';

-- CPU tábla szerkezetének ellenőrzése
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'CPUs';

-- Próba lekérdezés, hogy létezik-e megfelelő adat
SELECT TOP 5 * FROM Configurations;
SELECT TOP 5 * FROM ConfigurationComponents;
