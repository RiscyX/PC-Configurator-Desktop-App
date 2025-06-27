# PC Configurator Desktop App

## 1. Architekturális áttekintés

### Fő rétegek
- **Models**: Az adatbázis entitásokat reprezentáló osztályok (CPU, GPU, RAM, stb.)
- **Views**: A felhasználói felület XAML fájlok és azok code-behind osztályai
- **Adathozzáférési réteg**: Közvetlen ADO.NET SQL lekérdezések a modell osztályokon belül

### Architektúra jellemzői
- A projekt egyszerűsített WPF alkalmazás, amely a Model-View architektúrát követi (nem teljes MVVM)
- Az adathozzáférés közvetlenül a modell osztályokban történik, nincsenek külön repository vagy service osztályok
- Az üzleti logika a View code-behind osztályokban található

## 2. Adatbázis séma

### Táblák
- **Users**: Felhasználók tárolása (Id, Email, PasswordHash, Role, RegisteredAt)
- **Configurations**: Felhasználók konfigurációi (Id, UserId, Name, CreatedAt)
- **CPUs**: Processzorok (Id, Name, Manufacturer, Cores, Threads, BaseClockGHz, BoostClockGHz, stb.)
- **GPUs**: Videókártyák (Id, Name, Manufacturer, MemoryGB, stb.)
- **RAMs**: Memória modulok (Id, Name, CapacityGB, SpeedMHz, stb.)
- **Motherboards**: Alaplapok (Id, Name, Manufacturer, SocketTypeId, ChipsetTypeId, stb.)
- **Storages**: Tárolók (Id, Name, Manufacturer, CapacityGB, Type, stb.)
- **PSUs**: Tápegységek (Id, Name, Manufacturer, WattageW, stb.)
- **Cases**: Számítógépházak (Id, Name, Manufacturer, FormFactor, stb.)
- **SocketTypes**: CPU foglalatok (Id, Name, stb.)
- **ChipsetTypes**: Alaplap chipset típusai (Id, Name, stb.)
- **ConfigurationComponents**: Konfigurációk komponensei (ConfigurationId, ComponentType, ComponentId)

### Modell osztályok
- Az adatbázis táblák közvetlenül megfelelnek a modell osztályoknak (CPU, GPU, RAM, stb.)
- Minden modell osztály tartalmazza a táblában lévő mezőket
- A modell osztályok tartalmaznak metódusokat az adatbázisműveletekhez (SaveToDatabase, LoadFromDatabase, stb.)
- A ConfigurationModel osztály különleges, mert ez tárolja egy teljes konfiguráció összes komponensét

## 3. Felhasználói felület

### Főbb ablakok/oldalak
- **LoginWindow**: Bejelentkezés
- **RegistrationWindow**: Regisztráció
- **DashboardWindow**: Fő alkalmazás ablak, amely a következő oldalakat tartalmazza:
  - **Dashboard**: Áttekintő oldal
  - **Profile**: Felhasználó profil
  - **Components**: Komponensek böngészése és keresése
  - **ConfigBuilder**: Konfiguráció összeállítása
  - **Configs**: Mentett konfigurációk kezelése
  - **Users**: Felhasználók kezelése (csak admin)
  - **AdminSettings**: Adminisztrációs beállítások (csak admin)

### Komponens felvételi űrlapok
- CPU, GPU, RAM, Storage, Motherboard, PSU, Case - mindegyikhez külön XAML és code-behind

## 4. Adathozzáférés

Az alkalmazás közvetlenül használja az ADO.NET-et az adatbázis eléréséhez:
- Minden modell osztály tartalmaz adathozzáférési metódusokat
- Az alkalmazás a "MyDb" connection string-et használja a Web.config fájlból
- Nincsenek külön repository vagy service osztályok

## 5. Hiányzó elemek és TODO-k

- **Validáció**: Hiányzik a megfelelő bemeneti validáció több űrlapon
- **Hibakezelés**: A hibakezelés nem következetes, több helyen hiányzik a try-catch
- **Unit tesztek**: Nincsenek automatizált tesztek
- **ConfigBuilder.xaml.cs**: A fájlban sok hiba van, amelyeket javítani kell
  - Típus konverziós problémák a ComponentInfo és a konkrét komponens típusok között
  - Hiányzó vagy duplikált metódusok
  - Névtér hibák a Configuration típus használatánál

## 6. Refaktorálási javaslatok

- **MVVM bevezetése**: A kód tisztább és jobban karbantartható lenne MVVM mintával
- **Repository minta**: Adathozzáférési réteg kiemelése a modell osztályokból
- **Dependency Injection**: DI konténer bevezetése a függőségek kezelésére
- **Kódismétlés csökkentése**: Sok ismétlődő kód van a komponensek kezelésében
- **Névkonvenciók egységesítése**: A változók és osztályok elnevezése nem következetes
- **Hibaüzenetek lokalizálása**: A hibaüzenetek magyarul vannak hardkódolva

## 7. Feladatlista a projekt befejezéséhez

### Magas prioritású feladatok
1. **ConfigBuilder.xaml.cs javítása**:
   - A The type or namespace name "Configuration" not found hibák javítása
   - A type konverziós hibák javítása a ComponentInfo és a specifikus komponens típusok között
   - A duplikált metódusok összevonása/tisztítása

2. **Kompatibilitás ellenőrzés befejezése**:
   - CPU és alaplap socket kompatibilitás
   - RAM és alaplap kompatibilitás

3. **Felhasználói jogosultságkezelés javítása**:
   - Admin funkciók elrejtése normál felhasználóktól
   - Jogosultságellenőrzések következetes implementálása

### Közepes prioritású feladatok
1. **Adatvalidáció javítása**:
   - Űrlapok validációjának befejezése
   - Hibaüzenetek egységesítése

2. **Felhasználói élmény javítása**:
   - Állapotjelzők hozzáadása hosszabb műveletekhez
   - Visszajelzések javítása sikeres művelet után

3. **Adatbázis műveletek optimalizálása**:
   - Hatékonyabb lekérdezések írása
   - Tömeges műveletek támogatása

### Alacsony prioritású feladatok
1. **UI/UX fejlesztések**:
   - Reszponzív elrendezés javítása
   - Sötét/világos téma támogatás

2. **Dokumentáció**:
   - Kód dokumentáció befejezése
   - Felhasználói dokumentáció elkészítése

3. **Unit tesztek írása**:
   - Kritikus funkciók tesztelése
   - Automatizált tesztek beállítása
