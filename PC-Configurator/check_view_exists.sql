-- Ellenőrzés, hogy létezik-e a nézet
SELECT 
    SCHEMA_NAME(schema_id) as schema_name,
    name as view_name
FROM 
    sys.views
WHERE 
    name = 'vw_FullConfigurations';
