# Feladatlista

## Magas prioritású feladatok
- DONE: Jogosultság alapú gombmegjelenítés  
  - A komponensek lista oldalán csak admin felhasználók lássák a "Törlés" és "Szerkesztés" gombokat az alkatrészek neve mellett  
  - A nem-admin felhasználók ne lássák ezeket a gombokat  
  - ApplyAdminButtonsVisibility metódus implementálva a Components.xaml.cs fájlban
- DONE: Alkatrész szerkesztés nézet  
  - DONE: A "Szerkesztés" gombra kattintva töltse be a megfelelő alkatrésztípust reprezentáló formot a meglévő adatokkal  
  - DONE: Az egyes formokban implementálni kell a LoadForEdit metódust
    - DONE: CPU form szerkesztési funkcionalitás
    - DONE: GPU form szerkesztési funkcionalitás
    - DONE: RAM form szerkesztési funkcionalitás
    - DONE: Storage form szerkesztési funkcionalitás
    - DONE: PSU form szerkesztési funkcionalitás
    - DONE: Motherboard form szerkesztési funkcionalitás
    - DONE: Case form szerkesztési funkcionalitás
  - DONE: A "Mentés" gomb mentse el a módosításokat az adatbázisba
  - DONE: Ablakkezelés javítása (UserControl vs. Window)

## Magas priori- DONE: Jo- DONE: Jogosultság alapú gombmegjelenítés  
  - A komponensek lista oldalán csak admin felhasználók lássák a "Törlés" és "Szerkesztés" gombokat az alkatrészek neve mellett  
  - A nem-admin felhasználók ne lássák ezeket a gombokat  
  - ApplyAdminButtonsVisibility metódus implementálva a Components.xaml.cs fájlban
- WIP: Alkatrész szerkesztés nézet  
  - DONE: A "Szerkesztés" gombra kattintva töltse be a megfelelő alkatrésztípust reprezentáló formot a meglévő adatokkal  
  - PART: Az egyes formokban implementálni kell a LoadForEdit metódust
    - DONE: CPU form szerkesztési funkcionalitás
    - TODO: További komponens formok (GPU, RAM, Storage, PSU, Motherboard, Case) 
  - DONE: A "Mentés" gomb mentse el a módosításokat az adatbázisba (CPU esetén)
  - DONE: Ablakkezelés javítása (UserControl vs. Window)ság alapú gombmegjelenítés  
  - A komponensek lista oldalán csak admin felhasználók lássák a "Törlés" és "Szerkesztés" gombokat az alkatrészek neve mellett  
  - A nem-admin felhasználók ne lássák ezeket a gombokat  
  - ApplyAdminButtonsVisibility metódus implementálva a Components.xaml.cs fájlban
- WIP: Alkatrész szerkesztés nézet  
  - DONE: A "Szerkesztés" gombra kattintva töltse be a megfelelő alkatrésztípust reprezentáló formot a meglévő adatokkal  
  - PART: Az egyes formokban implementálni kell a LoadForEdit metódust
    - DONE: CPU form szerkesztési funkcionalitás
    - TODO: További komponens formok (GPU, RAM, Storage, PSU, Motherboard, Case)
  - DONE: A "Mentés" gomb mentse el a módosításokat az adatbázisba (CPU esetén)
  - DONE: Ablakkezelés javítása (UserControl vs. Window)feladatok
- DONE: Javítsd a **ConfigBuilder.xaml.cs** fájl hibáit  
  - Javítsd a `Configuration` típus vagy névtér hiányzó definícióját  
  - Oldd meg a `ComponentInfo` és az egyes komponensosztályok közötti típuskonverziós problémákat  
  - Egyesítsd és tisztítsd meg a duplikált metódusokat  
- DONE: Fejezd be a **kompatibilitás-ellenőrzést**  
  - Implementáld a CPU és az alaplap foglalat kompatibilitásának vizsgálatát  
  - Implementáld a RAM és az alaplap kompatibilitásának vizsgálatát  
- DONE: Javítsd a **jogosultságkezelést**  
  - Rejtsd el az admin funkciókat nem-admin felhasználók elől  
  - Valósítsd meg a jogosultságellenőrzéseket minden admin művelet előtt

## Közepes prioritású feladatok
- DONE: Építsd be és egységesítsd az **adatvalidációt** az űrlapokon  
  - DONE: Központosított ValidationHelper osztály létrehozása különböző validációs szabályokkal
  - DONE: ValidationHelper osztály hozzáadása a projekthez (.csproj)
  - DONE: Validáció beépítése a CPU, GPU, Motherboard és RAM alkatrész űrlapokra
  - DONE: További alkatrész űrlapok (PSU, Storage, Case) validációjának beépítése
  - DONE: Egységesített hibaüzenet stílus létrehozása  
- DONE: Egységesítsd a **hibakezelést**  
  - DONE: Try-catch blokkok hozzáadása a formok mentési műveleteihez
  - DONE: ErrorHandler segédosztály létrehozása a hibakezeléshez
  - DONE: ErrorHandler hozzáadása a projekthez (.csproj)
  - DONE: Felhasználóbarát hibaüzenetek megjelenítése
- DONE: Javítsd a **felhasználói élményt**  
  - TODO: (Alacsony prioritás) Adj hozzá betöltő animációkat vagy spinner komponenseket hosszabb műveletekhez  
  - DONE: Biztosíts visszajelzést sikeres vagy sikertelen műveletek után (ErrorHandler segédosztály)
  - DONE: Jogosultság alapú gomb megjelenítés a Components.xaml oldalon
- TODO: Implementáld a **komponens-hozzáadás és eltávolítás** funkciót  
  - A “Alkatrészek” oldalon kattintásra add hozzá a kiválasztott alkatrészt a “Gépépítés” nézethez  
  - A “Gépépítés” nézetben kattintásra távolítsa el a korábban hozzáadott alkatrészt  
- TODO: Jogosultság alapú gombmegjelenítés  
  - A komponensek lista oldalán csak admin felhasználók lássák a “Törlés” és “Szerkesztés” gombokat az alkatrészek neve mellett  
  - A nem-admin felhasználók ne lássák ezeket a gombokat  
- TODO: Alkatrész szerkesztés nézet  
  - A “Szerkesztés” gombra kattintva töltse be a megfelelő alkatrésztípust reprezentáló formot a meglévő adatokkal  
  - A “Mentés” gomb mentse el a módosításokat az adatbázisba  

## Alacsony prioritású feladatok
- TODO: Optimalizáld az **adatbázis-lekérdezéseket**  
  - Írj hatékonyabb SQL lekérdezéseket a jobb teljesítményért  
  - Támogasd a tömeges műveleteket, ahol lehetséges  
- TODO: Fejleszd tovább az **UI/UX-et**  
  - Javítsd a reszponzív elrendezést különböző ablakméretekhez  
  - Adj hozzá sötét/világos téma váltást

## Bugfixes

- DONE: XAML hibák javítása
  - ValidationErrorTextBlock hivatkozások javítása ValidationErrorText-re a Case.xaml és Storage.xaml fájlokban
  - Egységes hibaüzenet stílusok használata az összes komponens űrlapban
- DONE: Runtime hibák kezelése
  - InvalidCastException hibák javítása a LoadComponentById metódusban a Components.xaml.cs fájlban
  - IndexOutOfRangeException hibák javítása HasColumn segédfüggvény bevezetésével
  - Double-to-float konverziós hibák javítása explicit castolással a CPU formban
- DONE: Ablakkezelés hibáinak javítása
  - UserControl-ok megfelelő betöltése a szerkesztés ablakba
  - ScrollViewer hozzáadása a nagyméretű űrlapokhoz
  - Ablak átméretezés engedélyezése a helyes megjelenítéshez

## Összegzés - Jelenlegi haladás

### Teljesített feladatok:
1. Típushibák javítása és modellek kialakítása
2. Kompatibilitás-ellenőrzés implementálása adatbázis lekérdezések használatával
3. Jogosultságkezelés megvalósítása
   - PermissionManager segédosztály
   - Admin funkciók elrejtése / megjelenítése jogosultság alapján
4. Validáció egységesítése
   - ValidationHelper segédosztály
   - Minden űrlap validáció implementálása
5. Hibakezelés egységesítése
   - ErrorHandler segédosztály
   - Felhasználóbarát hibaüzenetek
6. XAML stílusok egységesítése
   - ValidationErrorText stílus használata minden komponens formban
   - Javítva a ValidationErrorTextBlock nem létező stílusra hivatkozás a Case.xaml és Storage.xaml fájlokban

### Folyamatban lévő feladatok:
1. Komponens szerkesztés
   - CPU komponensek szerkesztési modellje elkészült
   - GPU komponensek szerkesztési modellje elkészült
   - RAM komponensek szerkesztési modellje elkészült  
   - Storage komponensek szerkesztési modellje elkészült
   - PSU komponensek szerkesztési modellje elkészült
   - Motherboard komponensek szerkesztési modellje elkészült
   - Case komponensek szerkesztési modellje elkészült

### Hátralévő feladatok:
1. Komponens hozzáadás és eltávolítás a konfigurációból
2. Alacsony prioritású feladatok:
   - Betöltő animációk (spinner) hosszabb műveletekhez
   - Adatbázis lekérdezések optimalizálása
   - UI/UX fejlesztések (reszponzivitás, témaváltás)

### Javaslat a következő lépésekre:
1. A létrehozott CPU minta alapján implementálni a többi komponens szerkesztési logikáját
2. A komponens építő logika kiegészítése, hogy lehetőséget adjon komponensek hozzáadására és eltávolítására

## Hibák javítása (2025-06-27)
A következő javítások kerültek bevezetésre:

### Helper osztályok kiegészítése
- `ErrorHandler`: Hozzáadva a `ShowError` és `LogMessage` metódusok
- `PermissionManager`: Hozzáadva az `IsAdmin` property egyszerűbb hozzáféréshez
- `ValidationHelper`: Hozzáadva a hiányzó validációs metódusok

### Modell osztályok kiegészítése
- `CPU`: Hozzáadva a `Socket` property
- `Motherboard`: Hozzáadva a `FormFactor` property

### UI komponensek kiegészítése
- `CPU.xaml`: Hozzáadva a hiányzó mezők (Socket, Price, FormTitle)
- `Components.xaml.cs`: Javítva az ablakkezelés a szerkesztő form megnyitásakor (UserControl vs Window)
- `CPU.xaml.cs`: Javítva a szerkesztési logika és a LoadForEdit metódus

### További hibajavítások (2025-06-27)
- Javítva a double -> float konverziós hibák a `CPU.xaml.cs` fájlban (explicit cast hozzáadása)
- Hozzáadva a hiányzó `FormFactor` property a `Motherboard` osztályhoz
- Implementálva a GPU form szerkesztési funkciója (LoadForEdit, SaveChangesToDatabase)
- Implementálva a RAM form szerkesztési funkciója (LoadForEdit, SaveChangesToDatabase) 
- Implementálva a Storage form szerkesztési funkciója (LoadForEdit, SaveChangesToDatabase)
- Implementálva a PSU form szerkesztési funkciója (LoadForEdit, SaveChangesToDatabase)
- Hozzáadva a FormTitle és PriceTextBox a GPU, RAM, Storage és PSU XAML fájlokhoz
- Hozzáadva a hiányzó `Price` property a `RAM`, `Storage` és `PSU` modell osztályokhoz
- Javítva a LoadComponentById metódus, hogy ellenőrizze a tábla létezését és részletes hibaüzenetet adjon
- Javítva a típuskonverziós hiba a komponensek betöltésénél (DataColumn helyett egyedi HasColumn segédfüggvény)
- Javítva a hiányzó oszlopok kezelése (CPU-nál Socket, stb.), alapértelmezett értékek biztosítása
- Javítva a szerkesztő ablak méretezhetősége és görgethetősége (ScrollViewer hozzáadása az ablakhoz)

### Tesztelt funkcionalitás (2025-06-27)
- CPU komponens szerkesztése, mentése
- GPU komponens szerkesztése, mentése
- RAM komponens szerkesztése, mentése
- Storage komponens szerkesztése, mentése
- PSU komponens szerkesztése, mentése
- Motherboard komponens szerkesztése, mentése
- Case komponens szerkesztése, mentése
- Jogosultság alapú gombmegjelenítés admin és nem-admin felhasználók számára
- Az összes komponens típus teljes kezelése adatbázis oszlopok ellenőrzésével

### Legújabb fejlesztések és hibjavítások (2025-06-27)
- Implementálva a Motherboard form szerkesztési funkciója (LoadForEdit, SaveChangesToDatabase, UpdateInDatabase)
- Implementálva a Case form szerkesztési funkciója (LoadForEdit, SaveChangesToDatabase, UpdateInDatabase)
- Javítva a Components.xaml.cs OpenEditFormByType metódusa, hogy kezelje az összes komponens típust
- Kiegészítve a LoadComponentById metódus a Motherboard és Case komponensek betöltéséhez
- Hozzáadva a Price TextBox és FormTitle mindkét új komponens XAML-jához
- Frissítve a ValidationHelper használata a Price mező validálásához
- Javítva a feladatlista a haladás pontos követéséhez
- Hozzáadva a hiányzó `ValidateDecimal` metódus a ValidationHelper osztályhoz a decimális számok ellenőrzéséhez

### Adatbázis és modell frissítések (2025-06-27)
- Egységesítve az űrlapok megjelenítése és működése:
  - Power Consumption (fogyasztás) mező hozzáadva az összes komponens űrlaphoz
  - Formázási korrekciók a feliratokban (Ft -> HUF)
  - Validation Style egységesítése (ValidationErrorText használata mindenhol)
- Adatbázis frissítés:
  - Hiányzó oszlopok hozzáadása minden komponens táblához (update_database_tables.sql)
  - Price (ár) oszlop minden komponens táblához
  - PowerConsumption (fogyasztás) oszlop minden komponens táblához
  - Manufacturer, MemoryType, Socket, stb. specifikus oszlopok a megfelelő táblákhoz
- Modell osztályok és űrlapok egységesítése:
  - Minden komponens tartalmazza az összes előírt tulajdonságot
  - PowerConsumption mező hozzáadva minden űrlaphoz és modell osztályhoz

### Legújabb bugfixek (2025-06-27)
- Javítva a PowerConsumption mező kezelése a CPU formban:
  - PowerConsumption érték mentése az adatbázisba (SaveToDatabase és UpdateInDatabase metódusok)
  - PowerConsumption validáció hozzáadása
  - PowerConsumption betöltése a LoadForEdit metódusban
  - ResetForm metódus frissítése a PowerConsumption mező törlésével
