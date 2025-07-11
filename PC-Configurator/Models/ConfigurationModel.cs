using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class ConfigurationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
        public int PerformanceScore { get; set; }
        public List<ComponentInfo> Components { get; set; } = new List<ComponentInfo>();

        // Komponensek azonosítói a képen látható tábla szerkezete szerint
        public int? CPUId { get; set; }
        public int? GPUId { get; set; }
        public int? RAMId { get; set; }
        public int? StorageId { get; set; }
        public int? MotherboardId { get; set; }
        public int? PSUId { get; set; }
        public int? CaseId { get; set; }

        // Komponensek objektumként
        public CPU CPU { get; set; }
        public GPU GPU { get; set; }
        public RAM RAM { get; set; }
        public Storage Storage { get; set; }
        public Motherboard Motherboard { get; set; }
        public PSU PSU { get; set; }
        public Case Case { get; set; }

        public ConfigurationModel()
        {
        }

        public ConfigurationModel(int id, string name, int userId, DateTime createdAt, decimal price, int performanceScore)
        {
            Id = id;
            Name = name;
            UserId = userId;
            CreatedAt = createdAt;
            Price = price;
            PerformanceScore = performanceScore;
            Components = new List<ComponentInfo>();
        }

        public bool SaveToDatabase()
        {
            try
            {
                // Debug: Kiírjuk a mentendő adatokat
                System.Diagnostics.Debug.WriteLine("=== KONFIGURÁCIÓ MENTÉS KEZDÉSE ===");
                System.Diagnostics.Debug.WriteLine($"Név: {Name}, UserId: {UserId}");
                System.Diagnostics.Debug.WriteLine($"Komponens ID-k: CPU={CPUId}, GPU={GPUId}, RAM={RAMId}, Storage={StorageId}, Motherboard={MotherboardId}, PSU={PSUId}, Case={CaseId}");
                
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Adatbázis kapcsolat megnyitva: " + connection.State);
                    
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Konfiguráció mentése
                            int configId;
                            using (var command = new SqlCommand(
                                @"INSERT INTO Configurations (Name, UserId, CreatedAt) 
                                VALUES (@Name, @UserId, @CreatedAt);
                                SELECT CAST(SCOPE_IDENTITY() as int)", connection, transaction))
                            {
                                System.Diagnostics.Debug.WriteLine($"SQL paraméterek: Name={Name}, UserId={UserId}, CreatedAt={DateTime.Now}");
                                
                                // Név ellenőrzése - ne legyen üres
                                if (string.IsNullOrEmpty(Name))
                                {
                                    Name = $"Új konfiguráció - {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}";
                                    System.Diagnostics.Debug.WriteLine($"A konfiguráció neve üres volt, alapértelmezett név beállítva: {Name}");
                                }
                                
                                command.Parameters.AddWithValue("@Name", Name);
                                command.Parameters.AddWithValue("@UserId", UserId);
                                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                                
                                System.Diagnostics.Debug.WriteLine("Konfiguráció INSERT parancs végrehajtása...");
                                object result = command.ExecuteScalar();
                                
                                if (result != null && result != DBNull.Value)
                                {
                                    configId = Convert.ToInt32(result);
                                    System.Diagnostics.Debug.WriteLine($"Konfiguráció ID létrehozva: {configId}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("HIBA: Nem sikerült létrehozni a konfigurációt, null értéket kaptunk vissza");
                                    throw new Exception("Nem sikerült létrehozni a konfigurációt az adatbázisban. Üres ID értéket kaptunk vissza.");
                                }
                                
                                // Konfiguráció tábla adatok ellenőrzése
                                using (var checkCommand = new SqlCommand(
                                    @"SELECT COUNT(*) FROM Configurations WHERE Id = @Id", connection, transaction))
                                {
                                    checkCommand.Parameters.AddWithValue("@Id", configId);
                                    int configExists = (int)checkCommand.ExecuteScalar();
                                    System.Diagnostics.Debug.WriteLine($"Konfiguráció létezik az adatbázisban (ID={configId}): {configExists > 0}");
                                    
                                    if (configExists == 0)
                                    {
                                        throw new Exception($"Nem található a létrehozott konfiguráció az adatbázisban (ID={configId})");
                                    }
                                }
                            }

                            // Komponensek hozzáadása a konfigurációhoz az új táblaszerkezet szerint
                            System.Diagnostics.Debug.WriteLine("ConfigurationComponents INSERT parancs előkészítése...");
                            System.Diagnostics.Debug.WriteLine($"Használni kívánt ConfigurationId: {configId}");
                            
                            using (var command = new SqlCommand(
                                @"INSERT INTO ConfigurationComponents (ConfigurationId, CPUId, GPUId, RAMId, StorageId, MotherboardId, PSUId, CaseId) 
                                VALUES (@ConfigurationId, @CPUId, @GPUId, @RAMId, @StorageId, @MotherboardId, @PSUId, @CaseId)", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ConfigurationId", configId);
                                
                                // Null értékek kezelése
                                if (CPUId.HasValue)
                                {
                                    System.Diagnostics.Debug.WriteLine($"CPU ID: {CPUId.Value} (Valid)");
                                    // CPU létezésének ellenőrzése
                                    bool cpuExists = CheckComponentExists("CPUs", CPUId.Value, connection, transaction);
                                    System.Diagnostics.Debug.WriteLine($"CPU ID {CPUId.Value} létezik az adatbázisban: {cpuExists}");
                                    command.Parameters.AddWithValue("@CPUId", CPUId.Value);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("CPU ID: NULL");
                                    command.Parameters.AddWithValue("@CPUId", DBNull.Value);
                                }

                                if (GPUId.HasValue)
                                {
                                    System.Diagnostics.Debug.WriteLine($"GPU ID: {GPUId.Value} (Valid)");
                                    // GPU létezésének ellenőrzése
                                    bool gpuExists = CheckComponentExists("GPUs", GPUId.Value, connection, transaction);
                                    System.Diagnostics.Debug.WriteLine($"GPU ID {GPUId.Value} létezik az adatbázisban: {gpuExists}");
                                    command.Parameters.AddWithValue("@GPUId", GPUId.Value);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("GPU ID: NULL");
                                    command.Parameters.AddWithValue("@GPUId", DBNull.Value);
                                }

                                if (RAMId.HasValue)
                                {
                                    System.Diagnostics.Debug.WriteLine($"RAM ID: {RAMId.Value} (Valid)");
                                    // RAM létezésének ellenőrzése
                                    bool ramExists = CheckComponentExists("RAMs", RAMId.Value, connection, transaction);
                                    System.Diagnostics.Debug.WriteLine($"RAM ID {RAMId.Value} létezik az adatbázisban: {ramExists}");
                                    command.Parameters.AddWithValue("@RAMId", RAMId.Value);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("RAM ID: NULL");
                                    command.Parameters.AddWithValue("@RAMId", DBNull.Value);
                                }

                                if (StorageId.HasValue)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Storage ID: {StorageId.Value} (Valid)");
                                    // Storage létezésének ellenőrzése
                                    bool storageExists = CheckStorageExists(StorageId.Value, connection, transaction);
                                    System.Diagnostics.Debug.WriteLine($"Storage ID {StorageId.Value} létezik az adatbázisban: {storageExists}");
                                    command.Parameters.AddWithValue("@StorageId", StorageId.Value);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Storage ID: NULL");
                                    command.Parameters.AddWithValue("@StorageId", DBNull.Value);
                                }

                                if (MotherboardId.HasValue)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Motherboard ID: {MotherboardId.Value} (Valid)");
                                    // Motherboard létezésének ellenőrzése
                                    bool motherboardExists = CheckComponentExists("Motherboards", MotherboardId.Value, connection, transaction);
                                    System.Diagnostics.Debug.WriteLine($"Motherboard ID {MotherboardId.Value} létezik az adatbázisban: {motherboardExists}");
                                    command.Parameters.AddWithValue("@MotherboardId", MotherboardId.Value);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Motherboard ID: NULL");
                                    command.Parameters.AddWithValue("@MotherboardId", DBNull.Value);
                                }

                                if (PSUId.HasValue)
                                {
                                    System.Diagnostics.Debug.WriteLine($"PSU ID: {PSUId.Value} (Valid)");
                                    // PSU létezésének ellenőrzése
                                    bool psuExists = CheckComponentExists("PSUs", PSUId.Value, connection, transaction);
                                    System.Diagnostics.Debug.WriteLine($"PSU ID {PSUId.Value} létezik az adatbázisban: {psuExists}");
                                    command.Parameters.AddWithValue("@PSUId", PSUId.Value);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("PSU ID: NULL");
                                    command.Parameters.AddWithValue("@PSUId", DBNull.Value);
                                }

                                if (CaseId.HasValue)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Case ID: {CaseId.Value} (Valid)");
                                    // Case létezésének ellenőrzése
                                    bool caseExists = CheckComponentExists("Cases", CaseId.Value, connection, transaction);
                                    System.Diagnostics.Debug.WriteLine($"Case ID {CaseId.Value} létezik az adatbázisban: {caseExists}");
                                    command.Parameters.AddWithValue("@CaseId", CaseId.Value);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Case ID: NULL");
                                    command.Parameters.AddWithValue("@CaseId", DBNull.Value);
                                }

                                System.Diagnostics.Debug.WriteLine("ConfigurationComponents INSERT parancs végrehajtása...");
                                int affectedRows = command.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine($"ConfigurationComponents INSERT sikeres, {affectedRows} sor beszúrva");
                                
                                // Ellenőrizzük, hogy valóban létrejött-e a kapcsolat
                                using (var checkCommand = new SqlCommand(
                                    @"SELECT COUNT(*) FROM ConfigurationComponents WHERE ConfigurationId = @ConfigId", 
                                    connection, transaction))
                                {
                                    checkCommand.Parameters.AddWithValue("@ConfigId", configId);
                                    int configComponentsExist = (int)checkCommand.ExecuteScalar();
                                    System.Diagnostics.Debug.WriteLine($"Komponens kapcsolatok léteznek az adatbázisban (ConfigID={configId}): {configComponentsExist > 0}");
                                    
                                    if (configComponentsExist == 0)
                                    {
                                        throw new Exception($"Nem sikerült a komponensek hozzáadása a konfigurációhoz (ID={configId})");
                                    }
                                }
                            }

                            System.Diagnostics.Debug.WriteLine("Transaction commit végrehajtása...");
                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine("Transaction commit sikeres");
                            Id = configId; // Beállítjuk az új ID-t
                            
                            // Komponens objektumok hozzárendelése a ConfigurationModel-hez (rekurzió elkerülése)
                            SimpleUpdateComponentList();
                            
                            System.Diagnostics.Debug.WriteLine($"Konfiguráció sikeresen elmentve: ID={configId}, Név={Name}");
                            System.Diagnostics.Debug.WriteLine("=== KONFIGURÁCIÓ MENTÉS BEFEJEZVE ===");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            System.Diagnostics.Debug.WriteLine($"HIBA a konfiguráció mentése közben: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                            if (ex.InnerException != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                                System.Diagnostics.Debug.WriteLine($"Belső hiba stack trace: {ex.InnerException.StackTrace}");
                            }
                            System.Diagnostics.Debug.WriteLine("=== KONFIGURÁCIÓ MENTÉS SIKERTELEN ===");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SÚLYOS HIBA a konfiguráció mentése közben: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Belső hiba stack trace: {ex.InnerException.StackTrace}");
                }
                System.Diagnostics.Debug.WriteLine("=== KONFIGURÁCIÓ MENTÉS SIKERTELEN ===");
                return false;
            }
        }

        // Komponens infó lista frissítése a komponens objektumokból
        private void UpdateComponentInfoList()
        {
            // Mentjük a régi értékeket, hogy össze tudjuk hasonlítani
            decimal oldPrice = Price;
            int oldPerformanceScore = PerformanceScore;
            bool hasComponents = Components.Any();
            
            Components.Clear();
            
            // Komponens objektumok átalakítása ComponentInfo objektumokká
            if (CPU != null)
            {
                var cpuInfo = ComponentInfo.FromCPU(CPU);
                cpuInfo.Price = CPU.Price;
                cpuInfo.Power = CPU.PowerConsumption;
                Components.Add(cpuInfo);
            }
            
            if (GPU != null)
            {
                var gpuInfo = ComponentInfo.FromGPU(GPU);
                gpuInfo.Price = GPU.Price;
                gpuInfo.Power = GPU.PowerConsumption;
                Components.Add(gpuInfo);
            }
            
            if (RAM != null)
            {
                var ramInfo = ComponentInfo.FromRAM(RAM);
                ramInfo.Price = RAM.Price;
                ramInfo.Power = RAM.PowerConsumption;
                Components.Add(ramInfo);
            }
            
            if (Storage != null)
            {
                var storageInfo = ComponentInfo.FromStorage(Storage);
                storageInfo.Price = Storage.Price;
                storageInfo.Power = Storage.PowerConsumption;
                Components.Add(storageInfo);
            }
            
            if (Motherboard != null)
            {
                var mbInfo = ComponentInfo.FromMotherboard(Motherboard);
                mbInfo.Price = Motherboard.Price;
                mbInfo.Power = Motherboard.PowerConsumption;
                Components.Add(mbInfo);
            }
            
            if (PSU != null)
            {
                var psuInfo = ComponentInfo.FromPSU(PSU);
                psuInfo.Price = PSU.Price;
                Components.Add(psuInfo);
            }
            
            if (Case != null)
            {
                var caseInfo = ComponentInfo.FromCase(Case);
                caseInfo.Price = Case.Price;
                Components.Add(caseInfo);
            }
            
            // Csak akkor számoljuk újra az árat és teljesítményt, ha még nem volt beállítva
            // vagy ha első alkalommal hívjuk ezt a metódust a komponensekkel
            if (oldPrice <= 0 || oldPerformanceScore <= 0 || !hasComponents)
            {
                // Teljes ár és teljesítmény közvetlen számítása
                // NE hívjuk meg a CalculatePriceAndPerformance metódust! (végtelen rekurzió elkerülése)
                
                // Ár számítás közvetlenül
                decimal totalPrice = 0;
                foreach (var component in Components)
                {
                    totalPrice += component.Price;
                }
                
                // Csak akkor állítjuk be, ha nincs még ára
                if (oldPrice <= 0) 
                {
                    Price = totalPrice;
                }
                
                // Teljesítmény csak egyszer
                if (oldPerformanceScore <= 0)
                {
                    // Egyszerűsített teljesítmény pontszám számítás
                    if (CPU != null) PerformanceScore += 30;
                    if (GPU != null) PerformanceScore += 30;
                    if (RAM != null) PerformanceScore += 15;
                    if (Storage != null) PerformanceScore += 10;
                    if (Motherboard != null) PerformanceScore += 10;
                    if (PSU != null) PerformanceScore += 5;
                    
                    // Limitálás 0-100 között
                    PerformanceScore = Math.Min(100, PerformanceScore);
                    PerformanceScore = Math.Max(0, PerformanceScore);
                }
            }
        }
        
        // Teljes ár és teljesítménypont kiszámítása
        private void CalculatePriceAndPerformance()
        {
            // Összegezzük a komponensek árait
            decimal totalPrice = 0;
            foreach (var component in Components)
            {
                totalPrice += component.Price;
            }
            Price = totalPrice;

            // Ha az ár még mindig 0, akkor közvetlen becsült árat számítunk komponens típusok alapján
            // Fontos: NE hívjuk meg az EstimateTotalPrice()-t, mert az visszamutat és végtelen rekurziót okoz!
            if (Price <= 0)
            {
                decimal estimatedPrice = 0;
                
                // Közvetlenül becsüljük az árakat, nem másik metóduson keresztül
                if (CPU != null && !string.IsNullOrEmpty(CPU.Name))
                {
                    // CPU árbecslés
                    if (CPU.Name.Contains("Ryzen 7") || CPU.Name.Contains("Ryzen7"))
                        estimatedPrice += 120000;
                    else if (CPU.Name.Contains("Ryzen 5") || CPU.Name.Contains("Ryzen5"))
                        estimatedPrice += 85000;
                    else if (CPU.Name.Contains("Core i7") || CPU.Name.Contains("i7"))
                        estimatedPrice += 110000;
                    else if (CPU.Name.Contains("Core i5") || CPU.Name.Contains("i5"))
                        estimatedPrice += 80000;
                    else
                        estimatedPrice += 90000; // Alapértelmezett CPU ár
                }
                
                if (GPU != null && !string.IsNullOrEmpty(GPU.Name))
                {
                    // GPU árbecslés
                    if (GPU.Name.Contains("RTX 3070"))
                        estimatedPrice += 250000;
                    else if (GPU.Name.Contains("RTX 3060"))
                        estimatedPrice += 180000;
                    else if (GPU.Name.Contains("RTX"))
                        estimatedPrice += 200000;
                    else if (GPU.Name.Contains("GTX"))
                        estimatedPrice += 150000;
                    else
                        estimatedPrice += 180000; // Alapértelmezett GPU ár
                }
                
                // Egyéb komponensek becsült árai
                if (RAM != null) estimatedPrice += 30000;
                if (Storage != null) estimatedPrice += 25000;
                if (Motherboard != null) estimatedPrice += 40000;
                if (PSU != null) estimatedPrice += 20000;
                if (Case != null) estimatedPrice += 18000;
                
                Price = estimatedPrice;
            }
            
            // Egyszerű teljesítmény számítás
            PerformanceScore = 0;
            
            // CPU pontszám
            if (CPU != null)
            {
                // Ha ismerjük a magok számát
                if (CPU.Cores > 0)
                {
                    PerformanceScore += CPU.Cores * 5; // Csökkentett szorzó
                    PerformanceScore += (int)(CPU.BaseClockGHz * 10); // Csökkentett szorzó
                }
                else
                {
                    // CPU név alapján becslés (közvetlen, nem metódushívással)
                    string cpuName = CPU.Name ?? string.Empty;
                    
                    // Egyszerűsített CPU pontszámítás
                    if (cpuName.Contains("Ryzen 7") || cpuName.Contains("i7"))
                        PerformanceScore += 35;
                    else if (cpuName.Contains("Ryzen 5") || cpuName.Contains("i5"))
                        PerformanceScore += 25;
                    else
                        PerformanceScore += 15;
                }
            }
            
            // GPU pontszám
            if (GPU != null)
            {
                // Ha ismerjük a memória méretét
                if (GPU.Memory > 0)
                {
                    PerformanceScore += GPU.Memory * 5; // Csökkentett szorzó
                }
                else
                {
                    // GPU név alapján becslés (közvetlen, nem metódushívással)
                    string gpuName = GPU.Name ?? string.Empty;
                    
                    // Egyszerűsített GPU pontszámítás
                    if (gpuName.Contains("RTX 3070"))
                        PerformanceScore += 40;
                    else if (gpuName.Contains("RTX 3060"))
                        PerformanceScore += 30;
                    else if (gpuName.Contains("RTX"))
                        PerformanceScore += 25;
                    else if (gpuName.Contains("GTX"))
                        PerformanceScore += 15;
                    else
                        PerformanceScore += 10;
                }
            }
            
            // Egyéb komponensek hatása a teljesítményre - egyszerűsített
            if (RAM != null)
            {
                PerformanceScore += 10;
            }
            
            // Korlátozás 100-ra
            PerformanceScore = Math.Min(100, PerformanceScore);
            PerformanceScore = Math.Max(0, PerformanceScore);
        }
        
        // Teljes konfigurációs ár becslése a komponensek alapján
        // FIGYELEM: Ezt a metódust ne hívjuk a CalculatePriceAndPerformance-ből,
        // mert végtelen rekurzió alakulhat ki!
        private decimal EstimateTotalPrice()
        {
            // MEGJEGYZÉS: Ezt a metódust csak a kód kiegészítés miatt hagytam meg,
            // de ne használjuk a CalculatePriceAndPerformance-ből
            decimal totalPrice = 0;
            
            // Egyszerű becsült árak komponens típusok szerint
            if (CPU != null)
            {
                totalPrice += 90000; // Átlagos CPU ár
            }
            
            if (GPU != null)
            {
                totalPrice += 180000; // Átlagos GPU ár
            }
            
            if (RAM != null)
            {
                totalPrice += 30000; // Átlagos RAM ár
            }
            
            if (Storage != null)
            {
                totalPrice += 25000; // Átlagos Storage ár
            }
            
            if (Motherboard != null)
            {
                totalPrice += 40000; // Átlagos Motherboard ár
            }
            
            if (PSU != null)
            {
                totalPrice += 20000; // Átlagos PSU ár
            }
            
            if (Case != null)
            {
                totalPrice += 18000; // Átlagos Case ár
            }
            
            return totalPrice;
        }
        
        // CPU pontszám becslése név alapján
        private int EstimateCPUScore(string cpuName)
        {
            int score = 0;
            
            if (string.IsNullOrEmpty(cpuName))
                return score;
            
            // Ryzen processzorok
            if (cpuName.Contains("Ryzen"))
            {
                score += 30;
                
                // Ryzen generációk
                if (cpuName.Contains("5") || cpuName.Contains("5000"))
                    score += 30;
                else if (cpuName.Contains("3") || cpuName.Contains("3000"))
                    score += 20;
                else if (cpuName.Contains("2") || cpuName.Contains("2000"))
                    score += 15;
                else if (cpuName.Contains("1") || cpuName.Contains("1000"))
                    score += 10;
                
                // Ryzen típus
                if (cpuName.Contains("9") || cpuName.Contains("950") || cpuName.Contains("900"))
                    score += 20;
                else if (cpuName.Contains("7") || cpuName.Contains("700"))
                    score += 15;
                else if (cpuName.Contains("5") || cpuName.Contains("500"))
                    score += 10;
                else if (cpuName.Contains("3") || cpuName.Contains("300"))
                    score += 5;
            }
            // Intel processzorok
            else if (cpuName.Contains("Intel") || cpuName.Contains("Core"))
            {
                score += 30;
                
                // Intel generációk
                if (cpuName.Contains("11th") || cpuName.Contains("11.") || cpuName.Contains("11-"))
                    score += 30;
                else if (cpuName.Contains("10th") || cpuName.Contains("10.") || cpuName.Contains("10-"))
                    score += 25;
                else if (cpuName.Contains("9th") || cpuName.Contains("9.") || cpuName.Contains("9-"))
                    score += 20;
                else if (cpuName.Contains("8th") || cpuName.Contains("8.") || cpuName.Contains("8-"))
                    score += 15;
                else if (cpuName.Contains("7th") || cpuName.Contains("7.") || cpuName.Contains("7-"))
                    score += 10;
                
                // Intel típus
                if (cpuName.Contains("i9"))
                    score += 20;
                else if (cpuName.Contains("i7"))
                    score += 15;
                else if (cpuName.Contains("i5"))
                    score += 10;
                else if (cpuName.Contains("i3"))
                    score += 5;
            }
            
            return score;
        }
        
        // GPU pontszám becslése név alapján
        private int EstimateGPUScore(string gpuName)
        {
            int score = 0;
            
            if (string.IsNullOrEmpty(gpuName))
                return score;
            
            // NVIDIA GPU-k
            if (gpuName.Contains("RTX") || gpuName.Contains("GTX") || gpuName.Contains("NVIDIA"))
            {
                score += 30;
                
                // RTX 30xx széria
                if (gpuName.Contains("RTX 30") || gpuName.Contains("3090") || gpuName.Contains("3080") || 
                    gpuName.Contains("3070") || gpuName.Contains("3060"))
                {
                    score += 30;
                    
                    // Modellek
                    if (gpuName.Contains("3090"))
                        score += 20;
                    else if (gpuName.Contains("3080 Ti") || gpuName.Contains("3080Ti"))
                        score += 18;
                    else if (gpuName.Contains("3080"))
                        score += 15;
                    else if (gpuName.Contains("3070 Ti") || gpuName.Contains("3070Ti"))
                        score += 12;
                    else if (gpuName.Contains("3070"))
                        score += 10;
                    else if (gpuName.Contains("3060 Ti") || gpuName.Contains("3060Ti"))
                        score += 8;
                    else if (gpuName.Contains("3060"))
                        score += 5;
                }
                // RTX 20xx széria
                else if (gpuName.Contains("RTX 20") || gpuName.Contains("2080") || gpuName.Contains("2070") || 
                         gpuName.Contains("2060"))
                {
                    score += 20;
                    
                    // Modellek
                    if (gpuName.Contains("2080 Ti") || gpuName.Contains("2080Ti"))
                        score += 15;
                    else if (gpuName.Contains("2080 Super") || gpuName.Contains("2080S"))
                        score += 12;
                    else if (gpuName.Contains("2080"))
                        score += 10;
                    else if (gpuName.Contains("2070 Super") || gpuName.Contains("2070S"))
                        score += 8;
                    else if (gpuName.Contains("2070"))
                        score += 7;
                    else if (gpuName.Contains("2060 Super") || gpuName.Contains("2060S"))
                        score += 6;
                    else if (gpuName.Contains("2060"))
                        score += 5;
                }
                // GTX 16xx széria
                else if (gpuName.Contains("GTX 16") || gpuName.Contains("1660") || gpuName.Contains("1650"))
                {
                    score += 15;
                    
                    if (gpuName.Contains("1660 Ti") || gpuName.Contains("1660Ti"))
                        score += 7;
                    else if (gpuName.Contains("1660 Super") || gpuName.Contains("1660S"))
                        score += 6;
                    else if (gpuName.Contains("1660"))
                        score += 5;
                    else if (gpuName.Contains("1650"))
                        score += 3;
                }
                // GTX 10xx széria
                else if (gpuName.Contains("GTX 10") || gpuName.Contains("1080") || gpuName.Contains("1070") || 
                         gpuName.Contains("1060") || gpuName.Contains("1050"))
                {
                    score += 10;
                    
                    if (gpuName.Contains("1080 Ti") || gpuName.Contains("1080Ti"))
                        score += 10;
                    else if (gpuName.Contains("1080"))
                        score += 8;
                    else if (gpuName.Contains("1070 Ti") || gpuName.Contains("1070Ti"))
                        score += 7;
                    else if (gpuName.Contains("1070"))
                        score += 6;
                    else if (gpuName.Contains("1060"))
                        score += 4;
                    else if (gpuName.Contains("1050"))
                        score += 2;
                }
            }
            // AMD GPU-k
            else if (gpuName.Contains("RX") || gpuName.Contains("Radeon") || gpuName.Contains("AMD"))
            {
                score += 30;
                
                // RX 6000 széria
                if (gpuName.Contains("RX 6") || gpuName.Contains("6900") || gpuName.Contains("6800") || 
                    gpuName.Contains("6700") || gpuName.Contains("6600"))
                {
                    score += 25;
                    
                    if (gpuName.Contains("6900 XT") || gpuName.Contains("6900XT"))
                        score += 20;
                    else if (gpuName.Contains("6800 XT") || gpuName.Contains("6800XT"))
                        score += 18;
                    else if (gpuName.Contains("6800"))
                        score += 15;
                    else if (gpuName.Contains("6700 XT") || gpuName.Contains("6700XT"))
                        score += 12;
                    else if (gpuName.Contains("6600 XT") || gpuName.Contains("6600XT"))
                        score += 8;
                    else if (gpuName.Contains("6600"))
                        score += 5;
                }
                // RX 5000 széria
                else if (gpuName.Contains("RX 5") || gpuName.Contains("5700") || gpuName.Contains("5600") || 
                         gpuName.Contains("5500"))
                {
                    score += 15;
                    
                    if (gpuName.Contains("5700 XT") || gpuName.Contains("5700XT"))
                        score += 10;
                    else if (gpuName.Contains("5700"))
                        score += 8;
                    else if (gpuName.Contains("5600 XT") || gpuName.Contains("5600XT"))
                        score += 7;
                    else if (gpuName.Contains("5600"))
                        score += 5;
                    else if (gpuName.Contains("5500"))
                        score += 3;
                }
                // RX Vega széria
                else if (gpuName.Contains("Vega"))
                {
                    score += 10;
                    
                    if (gpuName.Contains("Vega 64") || gpuName.Contains("Vega64"))
                        score += 8;
                    else if (gpuName.Contains("Vega 56") || gpuName.Contains("Vega56"))
                        score += 6;
                }
            }
            
            return score;
        }
        
        // Komponens ár becslése típus és név alapján
        private static decimal EstimateComponentPrice(string componentType, string componentName)
        {
            if (string.IsNullOrEmpty(componentName))
                return 0;
                
            decimal estimatedPrice = 0;
            
            switch (componentType)
            {
                case "CPU":
                    // Ryzen processzorok árbecslése
                    if (componentName.Contains("Ryzen"))
                    {
                        if (componentName.Contains("5950X")) estimatedPrice = 229900;
                        else if (componentName.Contains("5900X")) estimatedPrice = 179900;
                        else if (componentName.Contains("5800X")) estimatedPrice = 139900;
                        else if (componentName.Contains("5600X")) estimatedPrice = 99900;
                        else if (componentName.Contains("5600")) estimatedPrice = 79900;
                        else if (componentName.Contains("3950X")) estimatedPrice = 199900;
                        else if (componentName.Contains("3900X")) estimatedPrice = 159900;
                        else if (componentName.Contains("3800X")) estimatedPrice = 119900;
                        else if (componentName.Contains("3700X")) estimatedPrice = 99900;
                        else if (componentName.Contains("3600X")) estimatedPrice = 79900;
                        else if (componentName.Contains("3600")) estimatedPrice = 69900;
                        else estimatedPrice = 100000; // Alapértelmezett Ryzen ár
                    }
                    // Intel processzorok árbecslése
                    else if (componentName.Contains("Intel") || componentName.Contains("Core"))
                    {
                        if (componentName.Contains("i9")) estimatedPrice = 199900;
                        else if (componentName.Contains("i7")) estimatedPrice = 149900;
                        else if (componentName.Contains("i5")) estimatedPrice = 99900;
                        else if (componentName.Contains("i3")) estimatedPrice = 59900;
                        else estimatedPrice = 90000; // Alapértelmezett Intel ár
                    }
                    else
                    {
                        estimatedPrice = 90000; // Ismeretlen CPU ár
                    }
                    break;
                
                case "GPU":
                    // NVIDIA GPU-k árbecslése
                    if (componentName.Contains("RTX") || componentName.Contains("GTX"))
                    {
                        if (componentName.Contains("3090")) estimatedPrice = 799900;
                        else if (componentName.Contains("3080 Ti")) estimatedPrice = 599900;
                        else if (componentName.Contains("3080")) estimatedPrice = 499900;
                        else if (componentName.Contains("3070 Ti")) estimatedPrice = 399900;
                        else if (componentName.Contains("3070")) estimatedPrice = 299900;
                        else if (componentName.Contains("3060 Ti")) estimatedPrice = 249900;
                        else if (componentName.Contains("3060")) estimatedPrice = 199900;
                        else if (componentName.Contains("2080 Ti")) estimatedPrice = 499900;
                        else if (componentName.Contains("2080 Super")) estimatedPrice = 399900;
                        else if (componentName.Contains("2080")) estimatedPrice = 349900;
                        else if (componentName.Contains("2070 Super")) estimatedPrice = 299900;
                        else if (componentName.Contains("2070")) estimatedPrice = 249900;
                        else if (componentName.Contains("2060 Super")) estimatedPrice = 199900;
                        else if (componentName.Contains("2060")) estimatedPrice = 159900;
                        else if (componentName.Contains("1660 Ti")) estimatedPrice = 119900;
                        else if (componentName.Contains("1660 Super")) estimatedPrice = 99900;
                        else if (componentName.Contains("1660")) estimatedPrice = 89900;
                        else if (componentName.Contains("1650")) estimatedPrice = 69900;
                        else if (componentName.Contains("1080 Ti")) estimatedPrice = 299900;
                        else if (componentName.Contains("1080")) estimatedPrice = 249900;
                        else if (componentName.Contains("1070 Ti")) estimatedPrice = 199900;
                        else if (componentName.Contains("1070")) estimatedPrice = 169900;
                        else if (componentName.Contains("1060")) estimatedPrice = 99900;
                        else if (componentName.Contains("1050 Ti")) estimatedPrice = 69900;
                        else if (componentName.Contains("1050")) estimatedPrice = 59900;
                        else estimatedPrice = 200000; // Alapértelmezett NVIDIA ár
                    }
                    // AMD GPU-k árbecslése
                    else if (componentName.Contains("RX") || componentName.Contains("Radeon"))
                    {
                        if (componentName.Contains("6900 XT")) estimatedPrice = 599900;
                        else if (componentName.Contains("6800 XT")) estimatedPrice = 499900;
                        else if (componentName.Contains("6800")) estimatedPrice = 399900;
                        else if (componentName.Contains("6700 XT")) estimatedPrice = 299900;
                        else if (componentName.Contains("6600 XT")) estimatedPrice = 199900;
                        else if (componentName.Contains("6600")) estimatedPrice = 159900;
                        else if (componentName.Contains("5700 XT")) estimatedPrice = 249900;
                        else if (componentName.Contains("5700")) estimatedPrice = 199900;
                        else if (componentName.Contains("5600 XT")) estimatedPrice = 149900;
                        else if (componentName.Contains("5600")) estimatedPrice = 129900;
                        else if (componentName.Contains("5500 XT")) estimatedPrice = 99900;
                        else if (componentName.Contains("5500")) estimatedPrice = 89900;
                        else if (componentName.Contains("Vega 64")) estimatedPrice = 179900;
                        else if (componentName.Contains("Vega 56")) estimatedPrice = 149900;
                        else estimatedPrice = 180000; // Alapértelmezett AMD GPU ár
                    }
                    else
                    {
                        estimatedPrice = 180000; // Ismeretlen GPU ár
                    }
                    break;
                
                case "RAM":
                    // RAM árszámítás a kapacitás és sebesség alapján
                    int capacity = ExtractRAMCapacityGB(componentName);
                    int speed = ExtractRAMSpeedMHz(componentName);
                    
                    if (capacity > 0)
                    {
                        estimatedPrice = capacity * 5000; // 1GB RAM ~ 5000 Ft
                        
                        // Sebesség alapján módosítás
                        if (speed >= 3600) estimatedPrice *= 1.3m;
                        else if (speed >= 3200) estimatedPrice *= 1.2m;
                        else if (speed >= 3000) estimatedPrice *= 1.1m;
                        
                        // Típus alapján módosítás
                        if (componentName.Contains("DDR5")) estimatedPrice *= 1.8m;
                        else if (componentName.Contains("DDR4")) estimatedPrice *= 1.0m;
                        else if (componentName.Contains("DDR3")) estimatedPrice *= 0.8m;
                    }
                    else
                    {
                        // Ha nem sikerül a kapacitást kinyerni
                        if (componentName.Contains("32GB")) estimatedPrice = 80000;
                        else if (componentName.Contains("16GB")) estimatedPrice = 40000;
                        else if (componentName.Contains("8GB")) estimatedPrice = 20000;
                        else if (componentName.Contains("4GB")) estimatedPrice = 10000;
                        else estimatedPrice = 30000; // Alapértelmezett RAM ár
                    }
                    break;
                
                case "Storage":
                    int storageCapacityGB = ExtractStorageCapacityGB(componentName);
                    string storageType = ExtractStorageType(componentName);
                    
                    if (storageCapacityGB > 0)
                    {
                        if (storageType == "SSD" || componentName.Contains("SSD"))
                        {
                            if (componentName.Contains("NVMe") || componentName.Contains("M.2"))
                                estimatedPrice = storageCapacityGB * 150; // NVMe SSD ~ 150 Ft/GB
                            else
                                estimatedPrice = storageCapacityGB * 120; // SATA SSD ~ 120 Ft/GB
                        }
                        else
                        {
                            // Feltételezzük, hogy HDD
                            estimatedPrice = storageCapacityGB * 30; // HDD ~ 30 Ft/GB
                        }
                    }
                    else
                    {
                        // Ha nem sikerül a kapacitást kinyerni
                        if (componentName.Contains("SSD"))
                        {
                            if (componentName.Contains("2TB")) estimatedPrice = 240000;
                            else if (componentName.Contains("1TB")) estimatedPrice = 120000;
                            else if (componentName.Contains("500GB") || componentName.Contains("512GB")) estimatedPrice = 60000;
                            else if (componentName.Contains("250GB") || componentName.Contains("256GB")) estimatedPrice = 30000;
                            else estimatedPrice = 60000; // Alapértelmezett SSD ár
                        }
                        else
                        {
                            // Feltételezzük, hogy HDD
                            if (componentName.Contains("8TB")) estimatedPrice = 100000;
                            else if (componentName.Contains("6TB")) estimatedPrice = 75000;
                            else if (componentName.Contains("4TB")) estimatedPrice = 50000;
                            else if (componentName.Contains("2TB")) estimatedPrice = 25000;
                            else if (componentName.Contains("1TB")) estimatedPrice = 15000;
                            else estimatedPrice = 30000; // Alapértelmezett HDD ár
                        }
                    }
                    break;
                
                case "Motherboard":
                    // Motherboard árbecslés a chipset és gyártó alapján
                    if (componentName.Contains("X570") || componentName.Contains("Z590")) estimatedPrice = 100000;
                    else if (componentName.Contains("B550") || componentName.Contains("Z490")) estimatedPrice = 80000;
                    else if (componentName.Contains("B450") || componentName.Contains("B460")) estimatedPrice = 50000;
                    else if (componentName.Contains("A520") || componentName.Contains("H470")) estimatedPrice = 40000;
                    else if (componentName.Contains("X470") || componentName.Contains("Z390")) estimatedPrice = 70000;
                    else if (componentName.Contains("B350") || componentName.Contains("H370")) estimatedPrice = 40000;
                    else if (componentName.Contains("A320") || componentName.Contains("H310")) estimatedPrice = 30000;
                    else estimatedPrice = 60000; // Alapértelmezett alaplap ár
                    
                    // Gyártó alapján módosítás
                    if (componentName.Contains("ASUS ROG") || componentName.Contains("Maximus")) estimatedPrice *= 1.4m;
                    else if (componentName.Contains("ASUS TUF") || componentName.Contains("Gigabyte AORUS")) estimatedPrice *= 1.2m;
                    else if (componentName.Contains("MSI") || componentName.Contains("ASUS")) estimatedPrice *= 1.1m;
                    break;
                
                case "PSU":
                    int wattage = ExtractPSUWattage(componentName);
                    string efficiency = ExtractPSUEfficiency(componentName);
                    
                    if (wattage > 0)
                    {
                        estimatedPrice = 25000; // Alap ár
                        
                        // Teljesítmény alapján
                        if (wattage >= 1000) estimatedPrice = 70000;
                        else if (wattage >= 850) estimatedPrice = 60000;
                        else if (wattage >= 750) estimatedPrice = 50000;
                        else if (wattage >= 650) estimatedPrice = 40000;
                        else if (wattage >= 550) estimatedPrice = 35000;
                        else if (wattage >= 450) estimatedPrice = 30000;
                        
                        // Hatékonyság alapján módosítás
                        if (efficiency == "Titanium" || efficiency.Contains("Titanium")) estimatedPrice *= 1.5m;
                        else if (efficiency == "Platinum" || efficiency.Contains("Platinum")) estimatedPrice *= 1.3m;
                        else if (efficiency == "Gold" || efficiency.Contains("Gold")) estimatedPrice *= 1.15m;
                        else if (efficiency == "Silver" || efficiency.Contains("Silver")) estimatedPrice *= 1.05m;
                        
                        // Gyártó alapján módosítás
                        if (componentName.Contains("Seasonic") || componentName.Contains("Corsair")) estimatedPrice *= 1.2m;
                        else if (componentName.Contains("EVGA") || componentName.Contains("be quiet!")) estimatedPrice *= 1.15m;
                    }
                    else
                    {
                        // Név alapján becslés, ha nem sikerült a teljesítményt kinyerni
                        if (componentName.Contains("1000W") || componentName.Contains("1000 W")) estimatedPrice = 70000;
                        else if (componentName.Contains("850W") || componentName.Contains("850 W")) estimatedPrice = 60000;
                        else if (componentName.Contains("750W") || componentName.Contains("750 W")) estimatedPrice = 50000;
                        else if (componentName.Contains("650W") || componentName.Contains("650 W")) estimatedPrice = 40000;
                        else if (componentName.Contains("550W") || componentName.Contains("550 W")) estimatedPrice = 35000;
                        else if (componentName.Contains("450W") || componentName.Contains("450 W")) estimatedPrice = 30000;
                        else estimatedPrice = 35000; // Alapértelmezett tápegység ár
                    }
                    break;
                
                case "Case":
                    if (componentName.Contains("NZXT") || componentName.Contains("Corsair"))
                    {
                        if (componentName.Contains("Premium") || componentName.Contains("Elite"))
                            estimatedPrice = 60000;
                        else
                            estimatedPrice = 40000;
                    }
                    else if (componentName.Contains("Fractal Design") || componentName.Contains("be quiet!"))
                        estimatedPrice = 35000;
                    else if (componentName.Contains("Phanteks") || componentName.Contains("Lian Li"))
                        estimatedPrice = 30000;
                    else if (componentName.Contains("Cooler Master") || componentName.Contains("Thermaltake"))
                        estimatedPrice = 25000;
                    else
                        estimatedPrice = 20000; // Alapértelmezett ház ár
                    break;
                
                default:
                    estimatedPrice = 10000; // Ismeretlen komponens típus
                    break;
            }
            
            return estimatedPrice;
        }
        
        // Komponens fogyasztás becslése típus és név alapján
        private static int EstimateComponentPower(string componentType, string componentName)
        {
            if (string.IsNullOrEmpty(componentName))
                return 0;
                
            int powerConsumption = 0;
            
            switch (componentType)
            {
                case "CPU":
                    // Ryzen processzorok fogyasztás becslése
                    if (componentName.Contains("Ryzen"))
                    {
                        if (componentName.Contains("5950X")) powerConsumption = 105;
                        else if (componentName.Contains("5900X")) powerConsumption = 105;
                        else if (componentName.Contains("5800X")) powerConsumption = 105;
                        else if (componentName.Contains("5600X")) powerConsumption = 65;
                        else if (componentName.Contains("5600")) powerConsumption = 65;
                        else if (componentName.Contains("3950X")) powerConsumption = 105;
                        else if (componentName.Contains("3900X")) powerConsumption = 105;
                        else if (componentName.Contains("3800X")) powerConsumption = 105;
                        else if (componentName.Contains("3700X")) powerConsumption = 65;
                        else if (componentName.Contains("3600X")) powerConsumption = 95;
                        else if (componentName.Contains("3600")) powerConsumption = 65;
                        else powerConsumption = 95; // Alapértelmezett Ryzen TDP
                    }
                    // Intel processzorok fogyasztás becslése
                    else if (componentName.Contains("Intel") || componentName.Contains("Core"))
                    {
                        if (componentName.Contains("i9")) powerConsumption = 125;
                        else if (componentName.Contains("i7")) powerConsumption = 95;
                        else if (componentName.Contains("i5")) powerConsumption = 65;
                        else if (componentName.Contains("i3")) powerConsumption = 58;
                        else powerConsumption = 80; // Alapértelmezett Intel TDP
                    }
                    else
                    {
                        powerConsumption = 80; // Ismeretlen CPU TDP
                    }
                    break;
                
                case "GPU":
                    // NVIDIA GPU-k fogyasztás becslése
                    if (componentName.Contains("RTX") || componentName.Contains("GTX"))
                    {
                        if (componentName.Contains("3090")) powerConsumption = 350;
                        else if (componentName.Contains("3080 Ti")) powerConsumption = 320;
                        else if (componentName.Contains("3080")) powerConsumption = 320;
                        else if (componentName.Contains("3070 Ti")) powerConsumption = 290;
                        else if (componentName.Contains("3070")) powerConsumption = 220;
                        else if (componentName.Contains("3060 Ti")) powerConsumption = 200;
                        else if (componentName.Contains("3060")) powerConsumption = 170;
                        else if (componentName.Contains("2080 Ti")) powerConsumption = 250;
                        else if (componentName.Contains("2080 Super")) powerConsumption = 250;
                        else if (componentName.Contains("2080")) powerConsumption = 215;
                        else if (componentName.Contains("2070 Super")) powerConsumption = 215;
                        else if (componentName.Contains("2070")) powerConsumption = 175;
                        else if (componentName.Contains("2060 Super")) powerConsumption = 175;
                        else if (componentName.Contains("2060")) powerConsumption = 160;
                        else if (componentName.Contains("1660 Ti")) powerConsumption = 120;
                        else if (componentName.Contains("1660 Super")) powerConsumption = 125;
                        else if (componentName.Contains("1660")) powerConsumption = 120;
                        else if (componentName.Contains("1650")) powerConsumption = 75;
                        else if (componentName.Contains("1080 Ti")) powerConsumption = 250;
                        else if (componentName.Contains("1080")) powerConsumption = 180;
                        else if (componentName.Contains("1070 Ti")) powerConsumption = 180;
                        else if (componentName.Contains("1070")) powerConsumption = 150;
                        else if (componentName.Contains("1060")) powerConsumption = 120;
                        else if (componentName.Contains("1050 Ti")) powerConsumption = 75;
                        else if (componentName.Contains("1050")) powerConsumption = 75;
                        else powerConsumption = 180; // Alapértelmezett NVIDIA fogyasztás
                    }
                    // AMD GPU-k fogyasztás becslése
                    else if (componentName.Contains("RX") || componentName.Contains("Radeon"))
                    {
                        if (componentName.Contains("6900 XT")) powerConsumption = 300;
                        else if (componentName.Contains("6800 XT")) powerConsumption = 300;
                        else if (componentName.Contains("6800")) powerConsumption = 250;
                        else if (componentName.Contains("6700 XT")) powerConsumption = 230;
                        else if (componentName.Contains("6600 XT")) powerConsumption = 160;
                        else if (componentName.Contains("6600")) powerConsumption = 132;
                        else if (componentName.Contains("5700 XT")) powerConsumption = 225;
                        else if (componentName.Contains("5700")) powerConsumption = 180;
                        else if (componentName.Contains("5600 XT")) powerConsumption = 160;
                        else if (componentName.Contains("5600")) powerConsumption = 150;
                        else if (componentName.Contains("5500 XT")) powerConsumption = 130;
                        else if (componentName.Contains("5500")) powerConsumption = 120;
                        else if (componentName.Contains("Vega 64")) powerConsumption = 295;
                        else if (componentName.Contains("Vega 56")) powerConsumption = 210;
                        else powerConsumption = 180; // Alapértelmezett AMD GPU fogyasztás
                    }
                    else
                    {
                        powerConsumption = 150; // Ismeretlen GPU fogyasztás
                    }
                    break;
                
                case "RAM":
                    // RAM memória fogyasztás becslése (nagyon alacsony)
                    int capacity = ExtractRAMCapacityGB(componentName);
                    if (capacity > 0)
                    {
                        powerConsumption = capacity / 2; // Megközelítőleg 1W per 2GB memória
                    }
                    else
                    {
                        powerConsumption = 4; // Alapértelmezett RAM fogyasztás
                    }
                    break;
                
                case "Storage":
                    string storageType = ExtractStorageType(componentName);
                    if (storageType == "SSD" || componentName.Contains("SSD"))
                    {
                        if (componentName.Contains("NVMe") || componentName.Contains("M.2"))
                            powerConsumption = 7; // NVMe SSD átlagos fogyasztása
                        else
                            powerConsumption = 5; // SATA SSD átlagos fogyasztása
                    }
                    else
                    {
                        // HDD
                        powerConsumption = 10; // Átlagos HDD fogyasztás
                    }
                    break;
                
                case "Motherboard":
                    // Alaplap fogyasztás a chipset alapján
                    if (componentName.Contains("X570") || componentName.Contains("Z590")) powerConsumption = 30;
                    else if (componentName.Contains("B550") || componentName.Contains("Z490")) powerConsumption = 25;
                    else if (componentName.Contains("B450") || componentName.Contains("B460")) powerConsumption = 20;
                    else powerConsumption = 20; // Alapértelmezett alaplap fogyasztás
                    break;
                
                default:
                    powerConsumption = 5; // Ismeretlen komponens típus
                    break;
            }
            
            return powerConsumption;
        }
        
        // Frissíti a konfigurációt az adatbázisban
        public bool UpdateInDatabase()
        {
            try
            {
                if (Id <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Nem lehet frissíteni a konfigurációt érvénytelen ID-val.");
                    return false;
                }

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Frissítjük a konfiguráció adatait
                            using (var command = new SqlCommand(
                                @"UPDATE Configurations 
                                  SET Name = @Name, UserId = @UserId 
                                  WHERE Id = @Id", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Id", Id);
                                command.Parameters.AddWithValue("@Name", Name);
                                command.Parameters.AddWithValue("@UserId", UserId);

                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Nem található {Id} azonosítójú konfiguráció.");
                                    transaction.Rollback();
                                    return false;
                                }
                            }

                            // Frissítjük vagy beszúrjuk a komponens kapcsolatokat
                            using (var checkCommand = new SqlCommand(
                                @"SELECT COUNT(*) FROM ConfigurationComponents WHERE ConfigurationId = @ConfigId", 
                                connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@ConfigId", Id);
                                int count = (int)checkCommand.ExecuteScalar();
                                
                                if (count > 0)
                                {
                                    // Ha van már rekord, akkor frissítünk
                                    using (var updateCommand = new SqlCommand(
                                        @"UPDATE ConfigurationComponents 
                                          SET CPUId = @CPUId, 
                                              GPUId = @GPUId, 
                                              RAMId = @RAMId, 
                                              StorageId = @StorageId, 
                                              MotherboardId = @MotherboardId, 
                                              PSUId = @PSUId, 
                                              CaseId = @CaseId 
                                          WHERE ConfigurationId = @ConfigId", connection, transaction))
                                    {
                                        updateCommand.Parameters.AddWithValue("@ConfigId", Id);
                                        
                                        // Null értékek kezelése
                                        if (CPUId.HasValue)
                                            updateCommand.Parameters.AddWithValue("@CPUId", CPUId.Value);
                                        else
                                            updateCommand.Parameters.AddWithValue("@CPUId", DBNull.Value);

                                        if (GPUId.HasValue)
                                            updateCommand.Parameters.AddWithValue("@GPUId", GPUId.Value);
                                        else
                                            updateCommand.Parameters.AddWithValue("@GPUId", DBNull.Value);

                                        if (RAMId.HasValue)
                                            updateCommand.Parameters.AddWithValue("@RAMId", RAMId.Value);
                                        else
                                            updateCommand.Parameters.AddWithValue("@RAMId", DBNull.Value);

                                        if (StorageId.HasValue)
                                            updateCommand.Parameters.AddWithValue("@StorageId", StorageId.Value);
                                        else
                                            updateCommand.Parameters.AddWithValue("@StorageId", DBNull.Value);

                                        if (MotherboardId.HasValue)
                                            updateCommand.Parameters.AddWithValue("@MotherboardId", MotherboardId.Value);
                                        else
                                            updateCommand.Parameters.AddWithValue("@MotherboardId", DBNull.Value);

                                        if (PSUId.HasValue)
                                            updateCommand.Parameters.AddWithValue("@PSUId", PSUId.Value);
                                        else
                                            updateCommand.Parameters.AddWithValue("@PSUId", DBNull.Value);

                                        if (CaseId.HasValue)
                                            updateCommand.Parameters.AddWithValue("@CaseId", CaseId.Value);
                                        else
                                            updateCommand.Parameters.AddWithValue("@CaseId", DBNull.Value);

                                        updateCommand.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Ha nincs még rekord, akkor beszúrunk
                                    using (var insertCommand = new SqlCommand(
                                        @"INSERT INTO ConfigurationComponents 
                                          (ConfigurationId, CPUId, GPUId, RAMId, StorageId, MotherboardId, PSUId, CaseId) 
                                          VALUES (@ConfigId, @CPUId, @GPUId, @RAMId, @StorageId, @MotherboardId, @PSUId, @CaseId)", 
                                        connection, transaction))
                                    {
                                        insertCommand.Parameters.AddWithValue("@ConfigId", Id);
                                        
                                        // Null értékek kezelése
                                        if (CPUId.HasValue)
                                            insertCommand.Parameters.AddWithValue("@CPUId", CPUId.Value);
                                        else
                                            insertCommand.Parameters.AddWithValue("@CPUId", DBNull.Value);

                                        if (GPUId.HasValue)
                                            insertCommand.Parameters.AddWithValue("@GPUId", GPUId.Value);
                                        else
                                            insertCommand.Parameters.AddWithValue("@GPUId", DBNull.Value);

                                        if (RAMId.HasValue)
                                            insertCommand.Parameters.AddWithValue("@RAMId", RAMId.Value);
                                        else
                                            insertCommand.Parameters.AddWithValue("@RAMId", DBNull.Value);

                                        if (StorageId.HasValue)
                                            insertCommand.Parameters.AddWithValue("@StorageId", StorageId.Value);
                                        else
                                            insertCommand.Parameters.AddWithValue("@StorageId", DBNull.Value);

                                        if (MotherboardId.HasValue)
                                            insertCommand.Parameters.AddWithValue("@MotherboardId", MotherboardId.Value);
                                        else
                                            insertCommand.Parameters.AddWithValue("@MotherboardId", DBNull.Value);

                                        if (PSUId.HasValue)
                                            insertCommand.Parameters.AddWithValue("@PSUId", PSUId.Value);
                                        else
                                            insertCommand.Parameters.AddWithValue("@PSUId", DBNull.Value);

                                        if (CaseId.HasValue)
                                            insertCommand.Parameters.AddWithValue("@CaseId", CaseId.Value);
                                        else
                                            insertCommand.Parameters.AddWithValue("@CaseId", DBNull.Value);

                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                            SimpleUpdateComponentList();
                            System.Diagnostics.Debug.WriteLine($"Konfiguráció sikeresen frissítve: ID={Id}, Név={Name}");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció frissítése közben: {ex.Message}");
                            if (ex.InnerException != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció frissítése közben: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                return false;
            }
        }

    // Felhasználói konfigurációk betöltése
    // A meglévő nézetet használja, amely a képen látható oszlopokkal rendelkezik
    public static List<ConfigurationModel> LoadUserConfigurations(int userId)
    {
        System.Diagnostics.Debug.WriteLine($"===== LoadUserConfigurations kezdése userId={userId} =====");
        List<ConfigurationModel> configurations = new List<ConfigurationModel>();
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
        System.Diagnostics.Debug.WriteLine($"Csatlakozás az adatbázishoz: {connectionString}");
        
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                System.Diagnostics.Debug.WriteLine("Kapcsolódás az adatbázishoz...");
                connection.Open();
                System.Diagnostics.Debug.WriteLine($"Adatbázis kapcsolat állapota: {connection.State}");
                
                // Közvetlenül a Configurations és ConfigurationComponents táblákból lekérdezés
                string sql = @"SELECT
    c.ConfigurationId,
    c.ConfigurationName,
    c.OwnerEmail,
    c.CPU,
    c.Motherboard,
    c.GPU,
    c.RAM,
    c.Storage,
    c.PSU,
    c.CaseName
FROM dbo.vw_FullConfigurations   AS c
ORDER BY c.ConfigurationId DESC;
";
                      
                System.Diagnostics.Debug.WriteLine("SQL lekérdezés előkészítése:");
                System.Diagnostics.Debug.WriteLine(sql);
                
                using (var command = new SqlCommand(sql, connection))
                    {
                        // UserId alapján szűrünk
                        System.Diagnostics.Debug.WriteLine($"SQL paraméter beállítása: @UserId = {userId}");
                        command.Parameters.AddWithValue("@UserId", userId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            int recordCount = 0;
                            System.Diagnostics.Debug.WriteLine("Adatsorok feldolgozása kezdődik...");
                            
                            while (reader.Read())
                            {
                                recordCount++;
                                System.Diagnostics.Debug.WriteLine($"--- {recordCount}. rekord feldolgozása ---");
                                
                                try
                                {
                                    var config = new ConfigurationModel();
                                    
                                    // Alapadatok beállítása a képen látható oszlopokból
                                    config.Id = reader.GetInt32(reader.GetOrdinal("ConfigurationId"));
                                    config.Name = reader.GetString(reader.GetOrdinal("ConfigurationName"));
                                    
                                    // Email cím alapján beállítjuk a user ID-t (ez csak ideiglenes megoldás)
                                    string ownerEmail = reader.GetString(reader.GetOrdinal("OwnerEmail"));
                                    config.UserId = userId; // Átadjuk a kapott userId-t
                                    
                                    // Komponensek adatainak beállítása szöveges mezőkből
                                    System.Diagnostics.Debug.WriteLine($"Konfiguráció: {config.Name}, Email: {ownerEmail}");
                                    
                                    // CPU betöltése
                                    if (!reader.IsDBNull(reader.GetOrdinal("CPU")))
                                    {
                                        string cpuText = reader.GetString(reader.GetOrdinal("CPU"));
                                        System.Diagnostics.Debug.WriteLine($"CPU adat: {cpuText}");
                                        
                                        // CPU objektum létrehozása részletes adatokkal
                                        config.CPU = new CPU
                                        {
                                            Name = cpuText,
                                            Price = EstimateComponentPrice("CPU", cpuText),
                                            PowerConsumption = EstimateComponentPower("CPU", cpuText),
                                            Cores = ExtractCPUCores(cpuText),
                                            Threads = ExtractCPUThreads(cpuText),
                                            BaseClockGHz = ExtractCPUBaseClockGHz(cpuText),
                                            BoostClockGHz = ExtractCPUBoostClockGHz(cpuText)
                                        };
                                    }
                                    
                                    // GPU betöltése
                                    if (!reader.IsDBNull(reader.GetOrdinal("GPU")))
                                    {
                                        string gpuText = reader.GetString(reader.GetOrdinal("GPU"));
                                        System.Diagnostics.Debug.WriteLine($"GPU adat: {gpuText}");
                                        
                                        config.GPU = new GPU
                                        {
                                            Name = gpuText,
                                            Price = EstimateComponentPrice("GPU", gpuText),
                                            PowerConsumption = EstimateComponentPower("GPU", gpuText),
                                            MemoryGB = ExtractGPUMemoryGB(gpuText),
                                            MemoryType = ExtractGPUMemoryType(gpuText)
                                        };
                                    }
                                    
                                    // RAM betöltése
                                    if (!reader.IsDBNull(reader.GetOrdinal("RAM")))
                                    {
                                        string ramText = reader.GetString(reader.GetOrdinal("RAM"));
                                        System.Diagnostics.Debug.WriteLine($"RAM adat: {ramText}");
                                        
                                        config.RAM = new RAM
                                        {
                                            Name = ramText,
                                            Price = EstimateComponentPrice("RAM", ramText),
                                            PowerConsumption = EstimateComponentPower("RAM", ramText),
                                            CapacityGB = ExtractRAMCapacityGB(ramText),
                                            Type = ExtractRAMType(ramText),
                                            SpeedMHz = ExtractRAMSpeedMHz(ramText)
                                        };
                                    }
                                    
                                    // Motherboard betöltése
                                    if (!reader.IsDBNull(reader.GetOrdinal("Motherboard")))
                                    {
                                        string mbText = reader.GetString(reader.GetOrdinal("Motherboard"));
                                        System.Diagnostics.Debug.WriteLine($"Motherboard adat: {mbText}");
                                        
                                        config.Motherboard = new Motherboard
                                        {
                                            Name = mbText,
                                            Price = EstimateComponentPrice("Motherboard", mbText),
                                            PowerConsumption = EstimateComponentPower("Motherboard", mbText),
                                            FormFactor = ExtractMotherboardFormFactor(mbText),
                                            Socket = ExtractMotherboardSocket(mbText),
                                            Chipset = ExtractMotherboardChipset(mbText)
                                        };
                                    }
                                    
                                    // Storage betöltése
                                    if (!reader.IsDBNull(reader.GetOrdinal("Storage")))
                                    {
                                        string storageText = reader.GetString(reader.GetOrdinal("Storage"));
                                        System.Diagnostics.Debug.WriteLine($"Storage adat: {storageText}");
                                        
                                        config.Storage = new Storage
                                        {
                                            Name = storageText,
                                            Price = EstimateComponentPrice("Storage", storageText),
                                            PowerConsumption = EstimateComponentPower("Storage", storageText),
                                            Type = ExtractStorageType(storageText),
                                            CapacityGB = ExtractStorageCapacityGB(storageText)
                                        };
                                    }
                                    
                                    // PSU betöltése
                                    if (!reader.IsDBNull(reader.GetOrdinal("PSU")))
                                    {
                                        string psuText = reader.GetString(reader.GetOrdinal("PSU"));
                                        System.Diagnostics.Debug.WriteLine($"PSU adat: {psuText}");
                                        
                                        config.PSU = new PSU
                                        {
                                            Name = psuText,
                                            Price = EstimateComponentPrice("PSU", psuText),
                                            Wattage = ExtractPSUWattage(psuText),
                                            EfficiencyRating = ExtractPSUEfficiency(psuText)
                                        };
                                    }
                                    
                                    // Case betöltése
                                    if (!reader.IsDBNull(reader.GetOrdinal("CaseName")))
                                    {
                                        string caseText = reader.GetString(reader.GetOrdinal("CaseName"));
                                        System.Diagnostics.Debug.WriteLine($"Case adat: {caseText}");
                                        
                                        config.Case = new Case
                                        {
                                            Name = caseText,
                                            Price = EstimateComponentPrice("Case", caseText),
                                            Color = ExtractCaseColor(caseText),
                                            SupportedFormFactors = "ATX, mATX, Mini-ITX" // Alapértelmezett támogatás
                                        };
                                    }
                                    
                                    // Teljesítménypont számítás a CPU és GPU nevek alapján
                                    string cpuName = config.CPU?.Name ?? string.Empty;
                                    string gpuName = config.GPU?.Name ?? string.Empty;
                                    config.PerformanceScore = EstimatePerformanceScore(cpuName, gpuName);
                                    System.Diagnostics.Debug.WriteLine($"Teljesítménypont: {config.PerformanceScore} pont");
                                    
                                    // Becsült ár számítása
                                    config.Price = EstimateSimplePrice(config);
                                    System.Diagnostics.Debug.WriteLine($"Becsült ár: {config.Price} Ft");
                                    
                                    // Létrehozás időpontja - alapértelmezett
                                    config.CreatedAt = DateTime.Now;
                                    
                                    // ComponentInfo lista egyszerű frissítése a komponensek alapján
                                    // FONTOS: ne hívjuk a normál UpdateComponentInfoList metódust, mert rekurzív hívás lehet
                                    config.SimpleUpdateComponentList();
                                    
                                    configurations.Add(config);
                                    System.Diagnostics.Debug.WriteLine($"Konfiguráció sikeresen hozzáadva: {config.Name} (ID: {config.Id})");
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"HIBA a konfiguráció feldolgozása közben: {ex.Message}");
                                    if (ex.InnerException != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                                    }
                                }
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"Összesen {recordCount} konfiguráció betöltve.");
                        }
                    }
                }
                
                // Sikerességi összegzés
                System.Diagnostics.Debug.WriteLine($"Sikeresen betöltve {configurations.Count} konfiguráció az adatbázisból.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a felhasználói konfigurációk betöltése közben: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"Sikeresen betöltve {configurations.Count} konfiguráció az adatbázisból a {userId} azonosítójú felhasználó számára");
            return configurations;
        }
        
        // Segéd metódus a teljesítmény pontszám kiszámításához név alapján
        private static int EstimatePerformanceScore(string cpuName, string gpuName)
        {
            int score = 50; // Alapértelmezett középérték
            
            // CPU pontszám a neve alapján
            if (!string.IsNullOrEmpty(cpuName))
            {
                // CPU generáció és típus alapján becsüljük meg a teljesítményt
                if (cpuName.Contains("Ryzen"))
                {
                    score += 15; // AMD Ryzen alappontszám
                    
                    if (cpuName.Contains("5800X") || cpuName.Contains("5900X") || cpuName.Contains("5950X"))
                        score += 35; // High-end
                    else if (cpuName.Contains("5600X") || cpuName.Contains("5700X"))
                        score += 30; // Mid-range
                    else if (cpuName.Contains("3900X") || cpuName.Contains("3950X"))
                        score += 25; // Previous gen high-end
                    else if (cpuName.Contains("3600") || cpuName.Contains("3700X"))
                        score += 20; // Previous gen mid-range
                }
                else if (cpuName.Contains("Intel") || cpuName.Contains("Core"))
                {
                    score += 15; // Intel Core alappontszám
                    
                    if (cpuName.Contains("i9"))
                        score += 35; // High-end
                    else if (cpuName.Contains("i7"))
                        score += 30; // Upper mid-range
                    else if (cpuName.Contains("i5"))
                        score += 20; // Mid-range
                    else if (cpuName.Contains("i3"))
                        score += 10; // Entry level
                }
            }
            
            // GPU pontszám a neve alapján
            if (!string.IsNullOrEmpty(gpuName))
            {
                // GPU széria és típus alapján becsüljük meg a teljesítményt
                if (gpuName.Contains("RTX 30"))
                {
                    score += 15; // RTX 30xx alappontszám
                    
                    if (gpuName.Contains("3090") || gpuName.Contains("3080 Ti"))
                        score += 35; // High-end
                    else if (gpuName.Contains("3080"))
                        score += 30; // High-end
                    else if (gpuName.Contains("3070"))
                        score += 25; // Upper mid-range
                    else if (gpuName.Contains("3060"))
                        score += 20; // Mid-range
                }
                else if (gpuName.Contains("RTX 20"))
                {
                    score += 10; // RTX 20xx alappontszám
                    
                    if (gpuName.Contains("2080 Ti") || gpuName.Contains("2080 Super"))
                        score += 25; // Previous gen high-end
                    else if (gpuName.Contains("2080") || gpuName.Contains("2070 Super"))
                        score += 20; // Previous gen high-mid
                    else if (gpuName.Contains("2070") || gpuName.Contains("2060 Super"))
                        score += 15; // Previous gen mid-range
                    else if (gpuName.Contains("2060"))
                        score += 10; // Previous gen entry
                }
                else if (gpuName.Contains("GTX"))
                {
                    score += 5; // GTX alappontszám
                    
                    if (gpuName.Contains("1080 Ti") || gpuName.Contains("1080"))
                        score += 15; // Older high-end
                    else if (gpuName.Contains("1070"))
                        score += 10; // Older mid-range
                    else if (gpuName.Contains("1060") || gpuName.Contains("1650") || gpuName.Contains("1660"))
                        score += 5; // Older entry level
                }
                else if (gpuName.Contains("RX"))
                {
                    score += 10; // AMD RX alappontszám
                    
                    if (gpuName.Contains("6900 XT") || gpuName.Contains("6800 XT"))
                        score += 30; // High-end
                    else if (gpuName.Contains("6800") || gpuName.Contains("6700 XT"))
                        score += 25; // Upper mid-range
                    else if (gpuName.Contains("6600 XT") || gpuName.Contains("6600"))
                        score += 20; // Mid-range
                    else if (gpuName.Contains("5700 XT") || gpuName.Contains("5700"))
                        score += 15; // Previous gen mid-range
                }
            }
            
            // Limitáljuk a pontot 0-100 között
            score = Math.Min(100, score);
            score = Math.Max(0, score);
            
            return score;
        }

        // Komponensek betöltése egy konfigurációhoz
        private static void LoadConfigurationComponents(SqlConnection connection, ConfigurationModel config)
        {
            try
            {
                // Komponens ID-k lekérése
                using (var command = new SqlCommand(
                    @"SELECT CPUId, GPUId, RAMId, StorageId, MotherboardId, PSUId, CaseId
                      FROM ConfigurationComponents 
                      WHERE ConfigurationId = @ConfigId", connection))
                {
                    command.Parameters.AddWithValue("@ConfigId", config.Id);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Komponens ID-k kiolvasása
                            config.CPUId = reader.IsDBNull(0) ? null : (int?)reader.GetInt32(0);
                            config.GPUId = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1);
                            config.RAMId = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
                            config.StorageId = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                            config.MotherboardId = reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4);
                            config.PSUId = reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5);
                            config.CaseId = reader.IsDBNull(6) ? null : (int?)reader.GetInt32(6);
                        }
                    }
                }
                
                // Komponens objektumok betöltése az ID-k alapján
                if (config.CPUId.HasValue)
                {
                    config.CPU = LoadCPUFromDatabase(config.CPUId.Value);
                }
                
                if (config.GPUId.HasValue)
                {
                    config.GPU = LoadGPUFromDatabase(config.GPUId.Value);
                }
                
                if (config.RAMId.HasValue)
                {
                    config.RAM = LoadRAMFromDatabase(config.RAMId.Value);
                }
                
                if (config.StorageId.HasValue)
                {
                    config.Storage = LoadStorageFromDatabase(config.StorageId.Value);
                }
                
                if (config.MotherboardId.HasValue)
                {
                    config.Motherboard = LoadMotherboardFromDatabase(config.MotherboardId.Value);
                }
                
                if (config.PSUId.HasValue)
                {
                    config.PSU = LoadPSUFromDatabase(config.PSUId.Value);
                }
                
                if (config.CaseId.HasValue)
                {
                    config.Case = LoadCaseFromDatabase(config.CaseId.Value);
                }
                
                // Komponens info lista frissítése (rekurzió elkerülése)
                config.SimpleUpdateComponentList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a komponensek betöltése közben a {config.Id} azonosítójú konfigurációhoz: {ex.Message}");
            }
        }
        
        // Komponensek betöltése
        public void LoadComponents()
        {
            try
            {
                // CPU betöltése
                if (CPUId.HasValue)
                {
                    CPU = LoadCPUFromDatabase(CPUId.Value);
                }

                // GPU betöltése
                if (GPUId.HasValue)
                {
                    GPU = LoadGPUFromDatabase(GPUId.Value);
                }

                // RAM betöltése
                if (RAMId.HasValue)
                {
                    RAM = LoadRAMFromDatabase(RAMId.Value);
                }

                // Storage betöltése
                if (StorageId.HasValue)
                {
                    Storage = LoadStorageFromDatabase(StorageId.Value);
                }

                // Motherboard betöltése
                if (MotherboardId.HasValue)
                {
                    Motherboard = LoadMotherboardFromDatabase(MotherboardId.Value);
                }

                // PSU betöltése
                if (PSUId.HasValue)
                {
                    PSU = LoadPSUFromDatabase(PSUId.Value);
                }

                // Case betöltése
                if (CaseId.HasValue)
                {
                    Case = LoadCaseFromDatabase(CaseId.Value);
                }
                
                // ComponentInfo lista frissítése (rekurzió elkerülése)
                SimpleUpdateComponentList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a komponensek betöltése közben: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
            }
        }
        
        // Teljes konfiguráció betöltése ID alapján
        public static ConfigurationModel LoadFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(
                        @"SELECT c.Id, c.Name, c.UserId, c.CreatedAt
                          FROM Configurations c
                          WHERE c.Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var config = new ConfigurationModel();
                                
                                config.Id = id;
                                config.Name = reader.GetString(1);
                                config.UserId = reader.GetInt32(2);
                                
                                if (!reader.IsDBNull(3))
                                    config.CreatedAt = reader.GetDateTime(3);
                                else
                                    config.CreatedAt = DateTime.Now;

                                // Komponensek betöltése külön metódusban
                                reader.Close();
                                LoadConfigurationComponents(connection, config);
                                
                                return config;
                            }
                        }
                    }

                    return null; // Nincs ilyen azonosítójú konfiguráció
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció betöltése közben: {ex.Message}");
                return null;
            }
        }
        // Konfiguráció törlése az adatbázisból
        public static bool DeleteFromDatabase(int configId)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Először a komponens kapcsolatokat töröljük
                            using (var command = new SqlCommand(
                                @"DELETE FROM ConfigurationComponents WHERE ConfigurationId = @ConfigId", 
                                connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ConfigId", configId);
                                command.ExecuteNonQuery();
                            }

                            // Utána a konfigurációt
                            using (var command = new SqlCommand(
                                @"DELETE FROM Configurations WHERE Id = @Id", 
                                connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Id", configId);
                                int rowsAffected = command.ExecuteNonQuery();
                                
                                if (rowsAffected == 0)
                                {
                                    // Nincs mit törölni
                                    transaction.Rollback();
                                    return false;
                                }
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine($"Konfiguráció sikeresen törölve: ID={configId}");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció törlése közben: {ex.Message}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció törlése közben: {ex.Message}");
                return false;
            }
        }

        // Konfiguráció másolása
        public ConfigurationModel Clone()
        {
            var clone = new ConfigurationModel
            {
                Name = $"{Name} - másolat",
                UserId = UserId,
                CreatedAt = DateTime.Now,
                Price = Price,
                PerformanceScore = PerformanceScore,
                
                // Komponens hivatkozások másolása
                CPUId = CPUId,
                GPUId = GPUId,
                RAMId = RAMId,
                StorageId = StorageId,
                MotherboardId = MotherboardId,
                PSUId = PSUId,
                CaseId = CaseId,
                
                // Komponens objektumok másolása
                CPU = CPU,
                GPU = GPU,
                RAM = RAM,
                Storage = Storage,
                Motherboard = Motherboard,
                PSU = PSU,
                Case = Case
            };
            
            // Komponens információk másolása
            // NE hívjunk metódusokat, mert rekurzív hívást okozhatnak
            foreach (var component in Components)
            {
                clone.Components.Add(new ComponentInfo
                {
                    Id = component.Id,
                    Name = component.Name,
                    Details = component.Details,
                    Price = component.Price,
                    Power = component.Power,
                    Type = component.Type
                });
            }
            
            // Ha a lista üres, akkor töltsük fel egyszerű módon
            if (clone.Components.Count == 0)
            {
                clone.SimpleUpdateComponentList();
            }
            
            return clone;
        }
        
        // CPU hozzáadása/beállítása a konfigurációhoz
        public void SetCPU(CPU cpu)
        {
            CPU = cpu;
            CPUId = cpu?.Id;
            SimpleUpdateComponentList(); // Módosítva, hogy ne okozzon végtelen rekurziót
        }
        
        // GPU hozzáadása/beállítása a konfigurációhoz
        public void SetGPU(GPU gpu)
        {
            GPU = gpu;
            GPUId = gpu?.Id;
            SimpleUpdateComponentList(); // Módosítva, hogy ne okozzon végtelen rekurziót
        }
        
        // RAM hozzáadása/beállítása a konfigurációhoz
        public void SetRAM(RAM ram)
        {
            RAM = ram;
            RAMId = ram?.Id;
            SimpleUpdateComponentList(); // Módosítva, hogy ne okozzon végtelen rekurziót
        }
        
        // Háttértár hozzáadása/beállítása a konfigurációhoz
        public void SetStorage(Storage storage)
        {
            Storage = storage;
            StorageId = storage?.Id;
            SimpleUpdateComponentList(); // Módosítva, hogy ne okozzon végtelen rekurziót
        }
        
        // Alaplap hozzáadása/beállítása a konfigurációhoz
        public void SetMotherboard(Motherboard motherboard)
        {
            Motherboard = motherboard;
            MotherboardId = motherboard?.Id;
            SimpleUpdateComponentList(); // Módosítva, hogy ne okozzon végtelen rekurziót
        }
        
        // Tápegység hozzáadása/beállítása a konfigurációhoz
        public void SetPSU(PSU psu)
        {
            PSU = psu;
            PSUId = psu?.Id;
            SimpleUpdateComponentList(); // Módosítva, hogy ne okozzon végtelen rekurziót
        }
        
        // Ház hozzáadása/beállítása a konfigurációhoz
        public void SetCase(Case caseComponent)
        {
            Case = caseComponent;
            CaseId = caseComponent?.Id;
            SimpleUpdateComponentList(); // Módosítva, hogy ne okozzon végtelen rekurziót
        }
        
        // Teljes áramfogyasztás kiszámítása
        public int CalculateTotalPowerConsumption()
        {
            int totalPower = 0;
            
            if (CPU != null) totalPower += CPU.PowerConsumption;
            if (GPU != null) totalPower += GPU.PowerConsumption;
            if (RAM != null) totalPower += RAM.PowerConsumption;
            if (Storage != null) totalPower += Storage.PowerConsumption;
            if (Motherboard != null) totalPower += Motherboard.PowerConsumption;
            
            // A tápegység fogyasztását nem számoljuk, mert az termel, nem fogyaszt
            
            return totalPower;
        }
        
        // Elegendő-e a tápegység a konfigurációhoz
        public bool HasSufficientPower()
        {
            if (PSU == null) return false;
            
            int totalConsumption = CalculateTotalPowerConsumption();
            int recommendedPower = (int)(totalConsumption * 1.3); // 30% tartalék
            
            return PSU.Wattage >= recommendedPower;
        }
        
        // Egyszerűsített kompatibilitási ellenőrzés
        public Dictionary<string, bool> CheckCompatibility()
        {
            var results = new Dictionary<string, bool>();
            
            // Alaplap és CPU kompatibilitás
            if (Motherboard != null && CPU != null)
            {
                // Socket kompatibilitás ellenőrzése
                results["CPU_Socket"] = Motherboard.SocketTypeId == CPU.SocketTypeId;
            }
            
            return results;
        }
        
        // CPU betöltése az adatbázisból ID alapján
        private static CPU LoadCPUFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM CPUs WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var cpu = new CPU();
                                cpu.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                cpu.Name = reader.GetString(reader.GetOrdinal("Name"));
                                
                                // További mezők betöltése - ellenőrizve, hogy léteznek-e
                                if (HasColumn(reader, "Manufacturer"))
                                    cpu.Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"));
                                
                                if (HasColumn(reader, "Price") && !reader.IsDBNull(reader.GetOrdinal("Price")))
                                    cpu.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                
                                if (HasColumn(reader, "SocketTypeId") && !reader.IsDBNull(reader.GetOrdinal("SocketTypeId")))
                                    cpu.SocketTypeId = reader.GetInt32(reader.GetOrdinal("SocketTypeId"));
                                
                                if (HasColumn(reader, "Socket") && !reader.IsDBNull(reader.GetOrdinal("Socket")))
                                    cpu.Socket = reader.GetString(reader.GetOrdinal("Socket"));
                                
                                if (HasColumn(reader, "Cores") && !reader.IsDBNull(reader.GetOrdinal("Cores")))
                                    cpu.Cores = reader.GetInt32(reader.GetOrdinal("Cores"));
                                
                                if (HasColumn(reader, "Threads") && !reader.IsDBNull(reader.GetOrdinal("Threads")))
                                    cpu.Threads = reader.GetInt32(reader.GetOrdinal("Threads"));
                                
                                if (HasColumn(reader, "BaseClockGHz") && !reader.IsDBNull(reader.GetOrdinal("BaseClockGHz")))
                                    cpu.BaseClockGHz = (float)reader.GetDecimal(reader.GetOrdinal("BaseClockGHz"));
                                
                                if (HasColumn(reader, "BoostClockGHz") && !reader.IsDBNull(reader.GetOrdinal("BoostClockGHz")))
                                    cpu.BoostClockGHz = (float)reader.GetDecimal(reader.GetOrdinal("BoostClockGHz"));
                                
                                if (HasColumn(reader, "PowerConsumption") && !reader.IsDBNull(reader.GetOrdinal("PowerConsumption")))
                                    cpu.PowerConsumption = reader.GetInt32(reader.GetOrdinal("PowerConsumption"));
                                
                                return cpu;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a CPU betöltése közben: {ex.Message}");
                return null;
            }
        }
        
        // GPU betöltése az adatbázisból ID alapján
        private static GPU LoadGPUFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM GPUs WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var gpu = new GPU();
                                gpu.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                gpu.Name = reader.GetString(reader.GetOrdinal("Name"));
                                
                                // További mezők betöltése - csak a lényeges adatok
                                if (HasColumn(reader, "Price") && !reader.IsDBNull(reader.GetOrdinal("Price")))
                                    gpu.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                
                                // Memory betöltése - MemoryGB az írható tulajdonság
                                if (HasColumn(reader, "MemoryGB") && !reader.IsDBNull(reader.GetOrdinal("MemoryGB")))
                                    gpu.MemoryGB = reader.GetInt32(reader.GetOrdinal("MemoryGB"));
                                else if (HasColumn(reader, "Memory") && !reader.IsDBNull(reader.GetOrdinal("Memory")))
                                    gpu.MemoryGB = reader.GetInt32(reader.GetOrdinal("Memory"));
                                
                                if (HasColumn(reader, "MemoryType") && !reader.IsDBNull(reader.GetOrdinal("MemoryType")))
                                    gpu.MemoryType = reader.GetString(reader.GetOrdinal("MemoryType"));
                                
                                if (HasColumn(reader, "PowerConsumption") && !reader.IsDBNull(reader.GetOrdinal("PowerConsumption")))
                                    gpu.PowerConsumption = reader.GetInt32(reader.GetOrdinal("PowerConsumption"));
                                
                                return gpu;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a GPU betöltése közben: {ex.Message}");
                return null;
            }
        }
        
        // RAM betöltése az adatbázisból ID alapján
        private static RAM LoadRAMFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM RAMs WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var ram = new RAM();
                                ram.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                ram.Name = reader.GetString(reader.GetOrdinal("Name"));
                                
                                // További mezők betöltése - csak a lényeges adatok
                                if (HasColumn(reader, "Price") && !reader.IsDBNull(reader.GetOrdinal("Price")))
                                    ram.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                
                                if (HasColumn(reader, "CapacityGB") && !reader.IsDBNull(reader.GetOrdinal("CapacityGB")))
                                    ram.CapacityGB = reader.GetInt32(reader.GetOrdinal("CapacityGB"));
                                
                                if (HasColumn(reader, "Type") && !reader.IsDBNull(reader.GetOrdinal("Type")))
                                    ram.Type = reader.GetString(reader.GetOrdinal("Type"));
                                
                                if (HasColumn(reader, "SpeedMHz") && !reader.IsDBNull(reader.GetOrdinal("SpeedMHz")))
                                    ram.SpeedMHz = reader.GetInt32(reader.GetOrdinal("SpeedMHz"));
                                
                                if (HasColumn(reader, "PowerConsumption") && !reader.IsDBNull(reader.GetOrdinal("PowerConsumption")))
                                    ram.PowerConsumption = reader.GetInt32(reader.GetOrdinal("PowerConsumption"));
                                
                                return ram;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a RAM betöltése közben: {ex.Message}");
                return null;
            }
        }
        
        // Storage betöltése az adatbázisból ID alapján
        private static Storage LoadStorageFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM Storages WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var storage = new Storage();
                                
                                try
                                {
                                    // Kötelező mezők - ezekért kivételt dobhatunk, ha nincs meg
                                    storage.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                    storage.Name = reader.GetString(reader.GetOrdinal("Name"));
                                    
                                    // Type mezőt alapértékre állítjuk, ha nem létezik vagy null
                                    if (HasColumn(reader, "Type") && !reader.IsDBNull(reader.GetOrdinal("Type")))
                                        storage.Type = reader.GetString(reader.GetOrdinal("Type"));
                                    else
                                        storage.Type = "Ismeretlen"; // Alapértelmezett érték
                                    
                                    // Capacity betöltése - alapértelmezett érték: 0
                                    if (HasColumn(reader, "CapacityGB") && !reader.IsDBNull(reader.GetOrdinal("CapacityGB")))
                                        storage.CapacityGB = reader.GetInt32(reader.GetOrdinal("CapacityGB"));
                                    else if (HasColumn(reader, "Capacity") && !reader.IsDBNull(reader.GetOrdinal("Capacity")))
                                        storage.CapacityGB = reader.GetInt32(reader.GetOrdinal("Capacity"));
                                    else
                                        storage.CapacityGB = 0; // Alapértelmezett érték
                                    
                                    // Opcionális mezők
                                    if (HasColumn(reader, "Price") && !reader.IsDBNull(reader.GetOrdinal("Price")))
                                        storage.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                    
                                    if (HasColumn(reader, "PowerConsumption") && !reader.IsDBNull(reader.GetOrdinal("PowerConsumption")))
                                        storage.PowerConsumption = reader.GetInt32(reader.GetOrdinal("PowerConsumption"));
                                    
                                    System.Diagnostics.Debug.WriteLine($"Storage sikeresen betöltve: ID={storage.Id}, Név={storage.Name}, Típus={storage.Type}, Kapacitás={storage.CapacityGB}GB");
                                    return storage;
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Hiba a Storage betöltése közben (adatolvasás): {ex.Message}");
                                }
                            }
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Nem található {id} azonosítójú Storage az adatbázisban");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a Storage betöltése közben: {ex.Message}");
                return null;
            }
        }
        
        // Motherboard betöltése az adatbázisból ID alapján
        private static Motherboard LoadMotherboardFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM Motherboards WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var motherboard = new Motherboard();
                                motherboard.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                motherboard.Name = reader.GetString(reader.GetOrdinal("Name"));
                                
                                // További mezők betöltése
                                if (HasColumn(reader, "Manufacturer"))
                                    motherboard.Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"));
                                
                                if (HasColumn(reader, "Price") && !reader.IsDBNull(reader.GetOrdinal("Price")))
                                    motherboard.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                
                                if (HasColumn(reader, "ChipsetTypeId") && !reader.IsDBNull(reader.GetOrdinal("ChipsetTypeId")))
                                    motherboard.ChipsetTypeId = reader.GetInt32(reader.GetOrdinal("ChipsetTypeId"));
                                
                                if (HasColumn(reader, "Chipset") && !reader.IsDBNull(reader.GetOrdinal("Chipset")))
                                    motherboard.Chipset = reader.GetString(reader.GetOrdinal("Chipset"));
                                
                                if (HasColumn(reader, "SocketTypeId") && !reader.IsDBNull(reader.GetOrdinal("SocketTypeId")))
                                    motherboard.SocketTypeId = reader.GetInt32(reader.GetOrdinal("SocketTypeId"));
                                
                                if (HasColumn(reader, "Socket") && !reader.IsDBNull(reader.GetOrdinal("Socket")))
                                    motherboard.Socket = reader.GetString(reader.GetOrdinal("Socket"));
                                
                                if (HasColumn(reader, "FormFactor") && !reader.IsDBNull(reader.GetOrdinal("FormFactor")))
                                    motherboard.FormFactor = reader.GetString(reader.GetOrdinal("FormFactor"));
                                
                                if (HasColumn(reader, "MemorySlots") && !reader.IsDBNull(reader.GetOrdinal("MemorySlots")))
                                    motherboard.MemorySlots = reader.GetInt32(reader.GetOrdinal("MemorySlots"));
                                
                                if (HasColumn(reader, "MaxMemoryGB") && !reader.IsDBNull(reader.GetOrdinal("MaxMemoryGB")))
                                    motherboard.MaxMemoryGB = reader.GetInt32(reader.GetOrdinal("MaxMemoryGB"));
                                
                                if (HasColumn(reader, "PowerConsumption") && !reader.IsDBNull(reader.GetOrdinal("PowerConsumption")))
                                    motherboard.PowerConsumption = reader.GetInt32(reader.GetOrdinal("PowerConsumption"));
                                
                                return motherboard;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a Motherboard betöltése közben: {ex.Message}");
                return null;
            }
        }
        
        // PSU betöltése az adatbázisból ID alapján
        private static PSU LoadPSUFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM PSUs WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var psu = new PSU();
                                psu.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                psu.Name = reader.GetString(reader.GetOrdinal("Name"));
                                
                                // További mezők betöltése - csak a lényeges adatok
                                if (HasColumn(reader, "Price") && !reader.IsDBNull(reader.GetOrdinal("Price")))
                                    psu.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                
                                if (HasColumn(reader, "Wattage") && !reader.IsDBNull(reader.GetOrdinal("Wattage")))
                                    psu.Wattage = reader.GetInt32(reader.GetOrdinal("Wattage"));
                                
                                // Efficiency betöltése - EfficiencyRating az írható tulajdonság
                                if (HasColumn(reader, "EfficiencyRating") && !reader.IsDBNull(reader.GetOrdinal("EfficiencyRating")))
                                    psu.EfficiencyRating = reader.GetString(reader.GetOrdinal("EfficiencyRating"));
                                else if (HasColumn(reader, "Efficiency") && !reader.IsDBNull(reader.GetOrdinal("Efficiency")))
                                    psu.EfficiencyRating = reader.GetString(reader.GetOrdinal("Efficiency"));
                                
                                return psu;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a PSU betöltése közben: {ex.Message}");
                return null;
            }
        }
        
        // Case betöltése az adatbázisból ID alapján
        private static Case LoadCaseFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM Cases WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var caseObj = new Case();
                                caseObj.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                caseObj.Name = reader.GetString(reader.GetOrdinal("Name"));
                                
                                // További mezők betöltése - csak a lényeges adatok
                                if (HasColumn(reader, "Manufacturer"))
                                    caseObj.Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"));
                                
                                if (HasColumn(reader, "Price") && !reader.IsDBNull(reader.GetOrdinal("Price")))
                                    caseObj.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                
                                if (HasColumn(reader, "Color") && !reader.IsDBNull(reader.GetOrdinal("Color")))
                                    caseObj.Color = reader.GetString(reader.GetOrdinal("Color"));
                                
                                if (HasColumn(reader, "SupportedFormFactors") && !reader.IsDBNull(reader.GetOrdinal("SupportedFormFactors")))
                                    caseObj.SupportedFormFactors = reader.GetString(reader.GetOrdinal("SupportedFormFactors"));
                                
                                return caseObj;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a Case betöltése közben: {ex.Message}");
                return null;
            }
        }
        

        
        // Segédfüggvény annak ellenőrzésére, hogy az olvasó tartalmazza-e az adott oszlopot
        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            if (reader == null)
                return false;
                
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch
            {
                return false;
            }
        }

        // Ellenőrzi, hogy létezik-e a megadott azonosítójú komponens az adatbázisban
        private bool CheckComponentExists(string tableName, int componentId, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                using (var command = new SqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE Id = @Id", connection, transaction))
                {
                    command.Parameters.AddWithValue("@Id", componentId);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a {tableName} létezésének ellenőrzésekor (ID={componentId}): {ex.Message}");
                return false;
            }
        }
        
        // Ellenőrzi, hogy létezik-e a megadott azonosítójú Storage az adatbázisban
        private bool CheckStorageExists(int storageId, SqlConnection connection, SqlTransaction transaction)
        {
            return CheckComponentExists("Storages", storageId, connection, transaction);
        }

        // Egyszerűsített ár becslés komponens alapján
        public static decimal EstimateSimplePrice(ConfigurationModel config)
        {
            decimal totalPrice = 0;
            
            // Komponens árak egyszerű összegzése
            if (config.CPU != null) 
            {
                totalPrice += config.CPU.Price > 0 ? config.CPU.Price : EstimateComponentPrice("CPU", config.CPU.Name);
            }
            
            if (config.GPU != null)
            {
                totalPrice += config.GPU.Price > 0 ? config.GPU.Price : EstimateComponentPrice("GPU", config.GPU.Name);
            }
            
            if (config.RAM != null)
            {
                totalPrice += config.RAM.Price > 0 ? config.RAM.Price : EstimateComponentPrice("RAM", config.RAM.Name);
            }
            
            if (config.Storage != null)
            {
                totalPrice += config.Storage.Price > 0 ? config.Storage.Price : EstimateComponentPrice("Storage", config.Storage.Name);
            }
            
            if (config.Motherboard != null)
            {
                totalPrice += config.Motherboard.Price > 0 ? config.Motherboard.Price : EstimateComponentPrice("Motherboard", config.Motherboard.Name);
            }
            
            if (config.PSU != null)
            {
                totalPrice += config.PSU.Price > 0 ? config.PSU.Price : EstimateComponentPrice("PSU", config.PSU.Name);
            }
            
            if (config.Case != null)
            {
                totalPrice += config.Case.Price > 0 ? config.Case.Price : EstimateComponentPrice("Case", config.Case.Name);
            }
            
            return totalPrice;
        }
        
        // Egyszerű komponens lista frissítés rekurzív hívások nélkül
        public void SimpleUpdateComponentList()
        {
            // Komponens lista ürítése
            Components.Clear();
            
            // Komponensek egyszerű hozzáadása a listához
            if (CPU != null)
            {
                Components.Add(new ComponentInfo
                {
                    Name = CPU.Name,
                    Type = "CPU",
                    Details = $"{CPU.Cores} mag / {CPU.Threads} szál, {CPU.BaseClockGHz:0.0} GHz",
                    Price = CPU.Price > 0 ? CPU.Price : EstimateComponentPrice("CPU", CPU.Name),
                    Power = CPU.PowerConsumption
                });
            }
            
            if (GPU != null)
            {
                Components.Add(new ComponentInfo
                {
                    Name = GPU.Name,
                    Type = "GPU",
                    Details = $"{GPU.Memory} GB {GPU.MemoryType}",
                    Price = GPU.Price > 0 ? GPU.Price : EstimateComponentPrice("GPU", GPU.Name),
                    Power = GPU.PowerConsumption
                });
            }
            
            if (RAM != null)
            {
                Components.Add(new ComponentInfo
                {
                    Name = RAM.Name,
                    Type = "RAM",
                    Details = $"{RAM.CapacityGB} GB {RAM.Type} {RAM.SpeedMHz} MHz",
                    Price = RAM.Price > 0 ? RAM.Price : EstimateComponentPrice("RAM", RAM.Name),
                    Power = RAM.PowerConsumption
                });
            }
            
            if (Storage != null)
            {
                Components.Add(new ComponentInfo
                {
                    Name = Storage.Name,
                    Type = "Storage",
                    Details = $"{Storage.Type}, {Storage.CapacityGB} GB",
                    Price = Storage.Price > 0 ? Storage.Price : EstimateComponentPrice("Storage", Storage.Name),
                    Power = Storage.PowerConsumption
                });
            }
            
            if (Motherboard != null)
            {
                Components.Add(new ComponentInfo
                {
                    Name = Motherboard.Name,
                    Type = "Motherboard",
                    Details = $"{Motherboard.FormFactor}, {Motherboard.Socket}, {Motherboard.Chipset}",
                    Price = Motherboard.Price > 0 ? Motherboard.Price : EstimateComponentPrice("Motherboard", Motherboard.Name),
                    Power = Motherboard.PowerConsumption
                });
            }
            
            if (PSU != null)
            {
                Components.Add(new ComponentInfo
                {
                    Name = PSU.Name,
                    Type = "PSU",
                    Details = $"{PSU.Wattage} W, {PSU.EfficiencyRating}",
                    Price = PSU.Price > 0 ? PSU.Price : EstimateComponentPrice("PSU", PSU.Name),
                    Power = 0
                });
            }
            
            if (Case != null)
            {
                Components.Add(new ComponentInfo
                {
                    Name = Case.Name,
                    Type = "Case",
                    Details = $"{Case.Color}",
                    Price = Case.Price > 0 ? Case.Price : EstimateComponentPrice("Case", Case.Name),
                    Power = 0
                });
            }
        }
        
        // Az alábbiakban olyan helper függvények találhatóak, amelyek komponens adatok kinyerését segítik névből
        
        // CPU adatok kinyerése
        private static int ExtractCPUCores(string cpuName)
        {
            // Alap mag szám
            int cores = 4; // Alapértelmezett 4 mag
            
            try
            {
                // Ryzen processzoroknál
                if (cpuName.Contains("Ryzen"))
                {
                    if (cpuName.Contains("5950X")) cores = 16;
                    else if (cpuName.Contains("5900X")) cores = 12;
                    else if (cpuName.Contains("5800X")) cores = 8;
                    else if (cpuName.Contains("5600X")) cores = 6;
                    else if (cpuName.Contains("3950X")) cores = 16;
                    else if (cpuName.Contains("3900X")) cores = 12;
                    else if (cpuName.Contains("3800X")) cores = 8;
                    else if (cpuName.Contains("3700X")) cores = 8;
                    else if (cpuName.Contains("3600")) cores = 6;
                }
                // Intel processzoroknál
                else if (cpuName.Contains("Intel") || cpuName.Contains("Core"))
                {
                    if (cpuName.Contains("i9")) cores = 10;
                    else if (cpuName.Contains("i7")) cores = 8;
                    else if (cpuName.Contains("i5")) cores = 6;
                    else if (cpuName.Contains("i3")) cores = 4;
                }
            }
            catch
            {
                cores = 4; // Hiba esetén alapértelmezett 4 mag
            }
            
            return cores;
        }
        
        private static int ExtractCPUThreads(string cpuName)
        {
            // Alapértelmezett szál = magok száma
            int cores = ExtractCPUCores(cpuName);
            int threads = cores; // Alapértelmezettként megegyezik a magokkal
            
            try
            {
                // Ryzen processzoroknál általában kétszeres (SMT)
                if (cpuName.Contains("Ryzen"))
                {
                    threads = cores * 2;
                }
                // Intel processzoroknál 8. generáció felett általában kétszeres (Hyperthreading)
                else if ((cpuName.Contains("Intel") || cpuName.Contains("Core")) && 
                         !cpuName.Contains("F") && // F széria néha nem tartalmaz Hyperthreading-et
                         !cpuName.ToLower().Contains("kf"))
                {
                    if (cpuName.Contains("i9")) threads = cores * 2;
                    else if (cpuName.Contains("i7")) threads = cores * 2;
                    else if (cpuName.Contains("i5") && (cpuName.Contains("10") || cpuName.Contains("11"))) threads = cores * 2;
                    else threads = cores; // i3 vagy régebbi i5 esetén
                }
            }
            catch
            {
                threads = cores; // Hiba esetén ugyanannyi szál mint mag
            }
            
            return threads;
        }
        
        private static float ExtractCPUBaseClockGHz(string cpuName)
        {
            // Alapértelmezett órajel
            float baseClockGHz = 3.0f;
            
            try
            {
                // Megpróbálunk GHz értéket keresni
                if (cpuName.Contains("GHz"))
                {
                    int index = cpuName.IndexOf("GHz");
                    
                    // 5 karaktert nézünk vissza (pl. "3.8 GHz")
                    if (index >= 5)
                    {
                        string clockText = cpuName.Substring(index - 5, 5).Trim();
                        // Csak számokat és pontot hagyunk meg
                        clockText = new string(clockText.Where(c => char.IsDigit(c) || c == '.').ToArray());
                        
                        if (!string.IsNullOrEmpty(clockText) && float.TryParse(clockText, out float result))
                        {
                            baseClockGHz = result;
                        }
                    }
                }
                else
                {
                    // Típus alapján saccolás
                    if (cpuName.Contains("Ryzen"))
                    {
                        if (cpuName.Contains("5950X")) baseClockGHz = 3.4f;
                        else if (cpuName.Contains("5900X")) baseClockGHz = 3.7f;
                        else if (cpuName.Contains("5800X")) baseClockGHz = 3.8f;
                        else if (cpuName.Contains("5600X")) baseClockGHz = 3.7f;
                        else if (cpuName.Contains("3900X")) baseClockGHz = 3.8f;
                        else if (cpuName.Contains("3800X")) baseClockGHz = 3.9f;
                        else if (cpuName.Contains("3700X")) baseClockGHz = 3.6f;
                        else if (cpuName.Contains("3600")) baseClockGHz = 3.6f;
                    }
                    else if (cpuName.Contains("Intel") || cpuName.Contains("Core"))
                    {
                        if (cpuName.Contains("i9")) baseClockGHz = 3.7f;
                        else if (cpuName.Contains("i7")) baseClockGHz = 3.6f;
                        else if (cpuName.Contains("i5")) baseClockGHz = 3.3f;
                        else if (cpuName.Contains("i3")) baseClockGHz = 3.0f;
                    }
                }
            }
            catch
            {
                baseClockGHz = 3.0f; // Hiba esetén 3.0 GHz
            }
            
            return baseClockGHz;
        }
        
        private static float ExtractCPUBoostClockGHz(string cpuName)
        {
            // Alapértelmezetten 15%-kal magasabb mint az alap órajel
            float baseClockGHz = ExtractCPUBaseClockGHz(cpuName);
            float boostClockGHz = baseClockGHz * 1.15f;
            
            try
            {
                // Típus alapján becslés
                if (cpuName.Contains("Ryzen"))
                {
                    if (cpuName.Contains("5950X")) boostClockGHz = 4.9f;
                    else if (cpuName.Contains("5900X")) boostClockGHz = 4.8f;
                    else if (cpuName.Contains("5800X")) boostClockGHz = 4.7f;
                    else if (cpuName.Contains("5600X")) boostClockGHz = 4.6f;
                    else if (cpuName.Contains("3900X")) boostClockGHz = 4.6f;
                    else if (cpuName.Contains("3800X")) boostClockGHz = 4.5f;
                    else if (cpuName.Contains("3700X")) boostClockGHz = 4.4f;
                    else if (cpuName.Contains("3600")) boostClockGHz = 4.2f;
                }
                else if (cpuName.Contains("Intel") || cpuName.Contains("Core"))
                {
                    if (cpuName.Contains("i9")) boostClockGHz = 5.3f;
                    else if (cpuName.Contains("i7")) boostClockGHz = 5.0f;
                    else if (cpuName.Contains("i5")) boostClockGHz = 4.8f;
                    else if (cpuName.Contains("i3")) boostClockGHz = 4.6f;
                }
            }
            catch
            {
                boostClockGHz = baseClockGHz * 1.15f; // Hiba esetén 15% boost
            }
            
            return boostClockGHz;
        }
        
        // GPU adatok kinyerése
        private static int ExtractGPUMemoryGB(string gpuName)
        {
            // Alapértelmezett memória méret
            int memoryGB = 8; // Alapértelmezett 8GB
            
            try
            {
                // Keresünk GB-ra utaló szöveget
                if (gpuName.Contains("GB"))
                {
                    int index = gpuName.IndexOf("GB");
                    
                    // 3 karaktert nézünk vissza (pl. "8GB")
                    if (index >= 2)
                    {
                        string memoryText = gpuName.Substring(index - 3, 3).Trim();
                        // Csak számokat hagyunk meg
                        memoryText = new string(memoryText.Where(char.IsDigit).ToArray());
                        
                        if (!string.IsNullOrEmpty(memoryText) && int.TryParse(memoryText, out int result))
                        {
                            memoryGB = result;
                        }
                    }
                }
                else
                {
                    // Típus alapján saccolás
                    if (gpuName.Contains("RTX"))
                    {
                        if (gpuName.Contains("3090")) memoryGB = 24;
                        else if (gpuName.Contains("3080 Ti")) memoryGB = 12;
                        else if (gpuName.Contains("3080")) memoryGB = 10;
                        else if (gpuName.Contains("3070 Ti")) memoryGB = 8;
                        else if (gpuName.Contains("3070")) memoryGB = 8;
                        else if (gpuName.Contains("3060 Ti")) memoryGB = 8;
                        else if (gpuName.Contains("3060")) memoryGB = 12;
                        else if (gpuName.Contains("2080 Ti")) memoryGB = 11;
                        else if (gpuName.Contains("2080")) memoryGB = 8;
                        else if (gpuName.Contains("2070")) memoryGB = 8;
                        else if (gpuName.Contains("2060")) memoryGB = 6;
                    }
                    else if (gpuName.Contains("GTX"))
                    {
                        if (gpuName.Contains("1660 Ti") || gpuName.Contains("1660")) memoryGB = 6;
                        else if (gpuName.Contains("1650")) memoryGB = 4;
                        else if (gpuName.Contains("1080 Ti")) memoryGB = 11;
                        else if (gpuName.Contains("1080")) memoryGB = 8;
                        else if (gpuName.Contains("1070 Ti") || gpuName.Contains("1070")) memoryGB = 8;
                        else if (gpuName.Contains("1060")) memoryGB = 6;
                        else if (gpuName.Contains("1050 Ti")) memoryGB = 4;
                        else if (gpuName.Contains("1050")) memoryGB = 2;
                    }
                    else if (gpuName.Contains("RX") || gpuName.Contains("Radeon"))
                    {
                        if (gpuName.Contains("6900 XT")) memoryGB = 16;
                        else if (gpuName.Contains("6800 XT") || gpuName.Contains("6800")) memoryGB = 16;
                        else if (gpuName.Contains("6700 XT")) memoryGB = 12;
                        else if (gpuName.Contains("6600 XT") || gpuName.Contains("6600")) memoryGB = 8;
                        else if (gpuName.Contains("5700 XT") || gpuName.Contains("5700")) memoryGB = 8;
                        else if (gpuName.Contains("5600 XT") || gpuName.Contains("5600")) memoryGB = 6;
                        else if (gpuName.Contains("5500 XT") || gpuName.Contains("5500")) memoryGB = 4;
                        else if (gpuName.Contains("Vega 64")) memoryGB = 8;
                        else if (gpuName.Contains("Vega 56")) memoryGB = 8;
                    }
                }
            }
            catch
            {
                memoryGB = 8; // Hiba esetén 8GB
            }
            
            return memoryGB;
        }
        
        private static string ExtractGPUMemoryType(string gpuName)
        {
            // Alapértelmezett memória típus
            string memoryType = "GDDR6";
            
            try
            {
                if (gpuName.Contains("GDDR6X")) memoryType = "GDDR6X";
                else if (gpuName.Contains("GDDR6")) memoryType = "GDDR6";
                else if (gpuName.Contains("GDDR5X")) memoryType = "GDDR5X";
                else if (gpuName.Contains("GDDR5")) memoryType = "GDDR5";
                else if (gpuName.Contains("HBM2")) memoryType = "HBM2";
                else if (gpuName.Contains("HBM")) memoryType = "HBM";
                else
                {
                    // Típus alapján saccolás
                    if (gpuName.Contains("RTX 30")) memoryType = "GDDR6X";
                    else if (gpuName.Contains("RTX 20")) memoryType = "GDDR6";
                    else if (gpuName.Contains("GTX 16")) memoryType = "GDDR5";
                    else if (gpuName.Contains("GTX 10")) memoryType = "GDDR5";
                    else if (gpuName.Contains("RX 6")) memoryType = "GDDR6";
                    else if (gpuName.Contains("RX 5")) memoryType = "GDDR6";
                    else if (gpuName.Contains("Vega")) memoryType = "HBM2";
                }
            }
            catch
            {
                memoryType = "GDDR6"; // Hiba esetén GDDR6
            }
            
            return memoryType;
        }
        
        // RAM adatok kinyerése
        private static int ExtractRAMCapacityGB(string ramName)
        {
            // Alapértelmezett kapacitás
            int capacityGB = 8; // Alapértelmezett 8GB
            
            try
            {
                // Keresünk GB-ra utaló szöveget
                if (ramName.Contains("GB"))
                {
                    int index = ramName.IndexOf("GB");
                    
                    // 3 karaktert nézünk vissza (pl. "16GB")
                    if (index >= 2)
                    {
                        // Addig megyünk vissza, amíg számot találunk
                        int startPos = index - 1;
                        while (startPos > 0 && (char.IsDigit(ramName[startPos - 1]) || ramName[startPos - 1] == ' '))
                        {
                            startPos--;
                        }
                        
                        string capacityText = ramName.Substring(startPos, index - startPos).Trim();
                        // Csak számokat hagyunk meg
                        capacityText = new string(capacityText.Where(char.IsDigit).ToArray());
                        
                        if (!string.IsNullOrEmpty(capacityText) && int.TryParse(capacityText, out int result))
                        {
                            capacityGB = result;
                        }
                    }
                }
                else
                {
                    // Ha nem találtunk GB jelölést, próbáljuk megkeresni a számokat
                    if (ramName.Contains("32")) capacityGB = 32;
                    else if (ramName.Contains("16")) capacityGB = 16;
                    else if (ramName.Contains("8")) capacityGB = 8;
                    else if (ramName.Contains("4")) capacityGB = 4;
                    // Alapértelmezett: 8GB
                }
            }
            catch
            {
                capacityGB = 8; // Hiba esetén 8GB
            }
            
            return capacityGB;
        }
        
        private static string ExtractRAMType(string ramName)
        {
            // Alapértelmezett RAM típus
            string ramType = "DDR4";
            
            try
            {
                if (ramName.Contains("DDR5")) ramType = "DDR5";
                else if (ramName.Contains("DDR4")) ramType = "DDR4";
                else if (ramName.Contains("DDR3")) ramType = "DDR3";
                else
                {
                    // Ha nem találtunk explicit típust, következtetünk a sebesség említése alapján
                    if (ramName.Contains("4800") || ramName.Contains("5200") || ramName.Contains("5600")) ramType = "DDR5";
                    else if (ramName.Contains("2133") || ramName.Contains("2400") || ramName.Contains("2666") || 
                             ramName.Contains("3000") || ramName.Contains("3200") || ramName.Contains("3600")) ramType = "DDR4";
                    else ramType = "DDR4"; // Alapértelmezett marad DDR4
                }
            }
            catch
            {
                ramType = "DDR4"; // Hiba esetén DDR4
            }
            
            return ramType;
        }
        
        private static int ExtractRAMSpeedMHz(string ramName)
        {
            // Alapértelmezett sebesség
            int speedMHz = 3200; // Alapértelmezett 3200MHz
            
            try
            {
                // Keresünk MHz-re utaló szöveget
                if (ramName.Contains("MHz"))
                {
                    int index = ramName.IndexOf("MHz");
                    
                    // Max 5 karaktert nézünk vissza (pl. "3200MHz")
                    if (index >= 4)
                    {
                        // Addig megyünk vissza, amíg számot találunk
                        int startPos = index - 1;
                        while (startPos > index - 5 && startPos > 0 && (char.IsDigit(ramName[startPos - 1]) || ramName[startPos - 1] == ' '))
                        {
                            startPos--;
                        }
                        
                        string speedText = ramName.Substring(startPos, index - startPos).Trim();
                        // Csak számokat hagyunk meg
                        speedText = new string(speedText.Where(char.IsDigit).ToArray());
                        
                        if (!string.IsNullOrEmpty(speedText) && int.TryParse(speedText, out int result))
                        {
                            speedMHz = result;
                        }
                    }
                }
                else
                {
                    // Típus alapján saccolás - közvetlenül ellenőrizzük a RAM típust
                    if (ramName.Contains("DDR5")) speedMHz = 4800;
                    else if (ramName.Contains("DDR4")) speedMHz = 3200;
                    else if (ramName.Contains("DDR3")) speedMHz = 1600;
                    else speedMHz = 3200; // Alapértelmezetten 3200MHz
                }
            }
            catch
            {
                // Hiba esetén típus alapján alapértelmezett sebesség
                if (ramName.Contains("DDR5")) speedMHz = 4800;
                else if (ramName.Contains("DDR4")) speedMHz = 3200;
                else if (ramName.Contains("DDR3")) speedMHz = 1600;
                else speedMHz = 3200; // Ismeretlen típus esetén 3200MHz
            }
            
            return speedMHz;
        }
        
        // Storage adatok kinyerése
        private static int ExtractStorageCapacityGB(string storageName)
        {
            // Alapértelmezett kapacitás
            int capacityGB = 500; // Alapértelmezett 500GB
            
            try
            {
                // TB-t keresünk először (nagyobb egység)
                if (storageName.Contains("TB"))
                {
                    int index = storageName.IndexOf("TB");
                    
                    // 3 karaktert nézünk vissza (pl. "2TB")
                    if (index >= 2)
                    {
                        // Addig megyünk vissza, amíg számot vagy pontot találunk
                        int startPos = index - 1;
                        while (startPos > 0 && (char.IsDigit(storageName[startPos - 1]) || storageName[startPos - 1] == '.' || storageName[startPos - 1] == ' '))
                        {
                            startPos--;
                        }
                        
                        string capacityText = storageName.Substring(startPos, index - startPos).Trim();
                        // Csak számokat és pontot hagyunk meg
                        capacityText = new string(capacityText.Where(c => char.IsDigit(c) || c == '.').ToArray());
                        
                        if (!string.IsNullOrEmpty(capacityText) && float.TryParse(capacityText, out float result))
                        {
                            capacityGB = (int)(result * 1000); // TB -> GB konverzió
                        }
                    }
                }
                // Ha nincs TB, GB-t keresünk
                else if (storageName.Contains("GB"))
                {
                    int index = storageName.IndexOf("GB");
                    
                    // 4 karaktert nézünk vissza (pl. "500GB")
                    if (index >= 3)
                    {
                        // Addig megyünk vissza, amíg számot találunk
                        int startPos = index - 1;
                        while (startPos > 0 && (char.IsDigit(storageName[startPos - 1]) || storageName[startPos - 1] == ' '))
                        {
                            startPos--;
                        }
                        
                        string capacityText = storageName.Substring(startPos, index - startPos).Trim();
                        // Csak számokat hagyunk meg
                        capacityText = new string(capacityText.Where(char.IsDigit).ToArray());
                        
                        if (!string.IsNullOrEmpty(capacityText) && int.TryParse(capacityText, out int result))
                        {
                            capacityGB = result;
                        }
                    }
                }
                else
                {
                    // Ha explicit kapacitás jelölés nincs, akkor típus alapján tippelünk
                    string storageType = ExtractStorageType(storageName);
                    
                    if (storageType == "SSD") capacityGB = 500;
                    else capacityGB = 1000; // HDD esetén
                }
            }
            catch
            {
                capacityGB = 500; // Hiba esetén 500GB
            }
            
            return capacityGB;
        }
        
        private static string ExtractStorageType(string storageName)
        {
            // Alapértelmezett típus
            string storageType = "HDD";
            
            try
            {
                if (storageName.Contains("SSD"))
                {
                    storageType = "SSD";
                    
                    // SSD altípusok
                    if (storageName.Contains("NVMe") || storageName.Contains("M.2"))
                    {
                        storageType = "NVMe SSD";
                    }
                    else if (storageName.Contains("SATA"))
                    {
                        storageType = "SATA SSD";
                    }
                }
                else if (storageName.Contains("HDD") || storageName.Contains("Hard Drive"))
                {
                    storageType = "HDD";
                }
                else if (storageName.Contains("NVMe") || storageName.Contains("M.2"))
                {
                    storageType = "NVMe SSD";
                }
            }
            catch
            {
                storageType = "HDD"; // Hiba esetén HDD
            }
            
            return storageType;
        }
        
        // Alaplap adatok kinyerése
        private static string ExtractMotherboardFormFactor(string motherboardName)
        {
            // Alapértelmezett form factor
            string formFactor = "ATX";
            
            try
            {
                if (motherboardName.Contains("EATX") || motherboardName.Contains("E-ATX"))
                {
                    formFactor = "E-ATX";
                }
                else if (motherboardName.Contains("Micro-ATX") || motherboardName.Contains("mATX") || motherboardName.Contains("µATX"))
                {
                    formFactor = "Micro-ATX";
                }
                else if (motherboardName.Contains("Mini-ITX") || motherboardName.Contains("mITX"))
                {
                    formFactor = "Mini-ITX";
                }
                else if (motherboardName.Contains("ATX"))
                {
                    formFactor = "ATX";
                }
            }
            catch
            {
                formFactor = "ATX"; // Hiba esetén ATX
            }
            
            return formFactor;
        }
        
        private static string ExtractMotherboardSocket(string motherboardName)
        {
            // Alapértelmezett socket
            string socket = "AM4";
            
            try
            {
                if (motherboardName.Contains("AM5"))
                {
                    socket = "AM5";
                }
                else if (motherboardName.Contains("AM4"))
                {
                    socket = "AM4";
                }
                else if (motherboardName.Contains("LGA 1200") || motherboardName.Contains("LGA1200"))
                {
                    socket = "LGA1200";
                }
                else if (motherboardName.Contains("LGA 1700") || motherboardName.Contains("LGA1700"))
                {
                    socket = "LGA1700";
                }
                else if (motherboardName.Contains("LGA 1151") || motherboardName.Contains("LGA1151"))
                {
                    socket = "LGA1151";
                }
                else
                {
                    // Chipset alapján próbáljuk meghatározni
                    string chipset = ExtractMotherboardChipset(motherboardName);
                    
                    if (chipset.Contains("X570") || chipset.Contains("B550") || chipset.Contains("B450") || chipset.Contains("A520"))
                    {
                        socket = "AM4"; // AMD modern chipsets
                    }
                    else if (chipset.Contains("X670") || chipset.Contains("B650"))
                    {
                        socket = "AM5"; // AMD új chipsets
                    }
                    else if (chipset.Contains("Z590") || chipset.Contains("B560") || chipset.Contains("H510"))
                    {
                        socket = "LGA1200"; // Intel 11. gen
                    }
                    else if (chipset.Contains("Z690") || chipset.Contains("B660") || chipset.Contains("H610"))
                    {
                        socket = "LGA1700"; // Intel 12. gen
                    }
                    else if (chipset.Contains("Z490") || chipset.Contains("B460") || chipset.Contains("H470"))
                    {
                        socket = "LGA1200"; // Intel 10. gen
                    }
                    else if (chipset.Contains("Z390") || chipset.Contains("B365") || chipset.Contains("H370"))
                    {
                        socket = "LGA1151"; // Intel 8-9. gen
                    }
                }
            }
            catch
            {
                socket = "AM4"; // Hiba esetén AM4
            }
            
            return socket;
        }
        
        private static string ExtractMotherboardChipset(string motherboardName)
        {
            // Alapértelmezett chipset
            string chipset = "B550";
            
            try
            {
                // AMD chipseteket keresünk
                if (motherboardName.Contains("X670")) chipset = "X670";
                else if (motherboardName.Contains("B650")) chipset = "B650";
                else if (motherboardName.Contains("X570")) chipset = "X570";
                else if (motherboardName.Contains("B550")) chipset = "B550";
                else if (motherboardName.Contains("X470")) chipset = "X470";
                else if (motherboardName.Contains("B450")) chipset = "B450";
                else if (motherboardName.Contains("A520")) chipset = "A520";
                else if (motherboardName.Contains("A320")) chipset = "A320";
                
                // Intel chipseteket keresünk
                else if (motherboardName.Contains("Z690")) chipset = "Z690";
                else if (motherboardName.Contains("B660")) chipset = "B660";
                else if (motherboardName.Contains("H610")) chipset = "H610";
                else if (motherboardName.Contains("Z590")) chipset = "Z590";
                else if (motherboardName.Contains("B560")) chipset = "B560";
                else if (motherboardName.Contains("H510")) chipset = "H510";
                else if (motherboardName.Contains("Z490")) chipset = "Z490";
                else if (motherboardName.Contains("B460")) chipset = "B460";
                else if (motherboardName.Contains("H470")) chipset = "H470";
                else if (motherboardName.Contains("Z390")) chipset = "Z390";
                else if (motherboardName.Contains("B365")) chipset = "B365";
                else if (motherboardName.Contains("H370")) chipset = "H370";
                else if (motherboardName.Contains("B360")) chipset = "B360";
                else if (motherboardName.Contains("H310")) chipset = "H310";
            }
            catch
            {
                chipset = "B550"; // Hiba esetén B550
            }
            
            return chipset;
        }
        
        // PSU adatok kinyerése
        private static int ExtractPSUWattage(string psuName)
        {
            // Alapértelmezett teljesítmény
            int wattage = 650; // Alapértelmezett 650W
            
            try
            {
                // Keresünk W-ra utaló szöveget
                if (psuName.Contains("W") || psuName.Contains("Watt"))
                {
                    int index = -1;
                    if (psuName.Contains("W")) index = psuName.IndexOf("W");
                    else if (psuName.Contains("Watt")) index = psuName.IndexOf("Watt");
                    
                    if (index > 0)
                    {
                        // Addig megyünk vissza, amíg számot találunk
                        int startPos = index - 1;
                        while (startPos > 0 && (char.IsDigit(psuName[startPos - 1]) || psuName[startPos - 1] == ' '))
                        {
                            startPos--;
                        }
                        
                        string wattText = psuName.Substring(startPos, index - startPos).Trim();
                        // Csak számokat hagyunk meg
                        wattText = new string(wattText.Where(char.IsDigit).ToArray());
                        
                        if (!string.IsNullOrEmpty(wattText) && int.TryParse(wattText, out int result))
                        {
                            wattage = result;
                        }
                    }
                }
                else
                {
                    // Ha nincs explicit jelölés, tippelünk
                    if (psuName.Contains("1000")) wattage = 1000;
                    else if (psuName.Contains("850")) wattage = 850;
                    else if (psuName.Contains("750")) wattage = 750;
                    else if (psuName.Contains("650")) wattage = 650;
                    else if (psuName.Contains("550")) wattage = 550;
                    else if (psuName.Contains("450")) wattage = 450;
                }
            }
            catch
            {
                wattage = 650; // Hiba esetén 650W
            }
            
            return wattage;
        }
        
        private static string ExtractPSUEfficiency(string psuName)
        {
            // Alapértelmezett hatékonyság
            string efficiency = "Bronze";
            
            try
            {
                if (psuName.Contains("Titanium")) efficiency = "Titanium";
                else if (psuName.Contains("Platinum")) efficiency = "Platinum";
                else if (psuName.Contains("Gold")) efficiency = "Gold";
                else if (psuName.Contains("Silver")) efficiency = "Silver";
                else if (psuName.Contains("Bronze")) efficiency = "Bronze";
                else if (psuName.Contains("80+") || psuName.Contains("80 +") || psuName.Contains("80 Plus")) efficiency = "80 PLUS";
                else if (psuName.Contains("White")) efficiency = "White";
            }
            catch
            {
                efficiency = "Bronze"; // Hiba esetén Bronze
            }
            
            return efficiency;
        }
        
        // Case adatok kinyerése
        private static string ExtractCaseColor(string caseName)
        {
            // Alapértelmezett szín
            string color = "Fekete";
            
            try
            {
                if (caseName.Contains("Black") || caseName.Contains("Fekete")) color = "Fekete";
                else if (caseName.Contains("White") || caseName.Contains("Fehér")) color = "Fehér";
                else if (caseName.Contains("Red") || caseName.Contains("Piros")) color = "Piros";
                else if (caseName.Contains("Blue") || caseName.Contains("Kék")) color = "Kék";
                else if (caseName.Contains("Gray") || caseName.Contains("Grey") || caseName.Contains("Szürke")) color = "Szürke";
                else if (caseName.Contains("Silver") || caseName.Contains("Ezüst")) color = "Ezüst";
                else if (caseName.Contains("RGB")) color = "RGB";
            }
            catch
            {
                color = "Fekete"; // Hiba esetén fekete
            }
            
            return color;
        }
    }
}
