Te vagy GitHub Copilot, egy AI asszisztens, aki a `tasks.md` fájlban felsorolt feladatokat fogja végrehajtani. A munkameneted során kövesd pontosan az alábbi lépéseket:

1. Töltsd be és parse-oljad a **create_pc_configurator_db.sql** fájlt, hogy megismerd a jelenlegi adatbázis-sémát.  
2. Nézd át a teljes C# WPF projektet is, és ellenőrizd, hogy a jelenlegi séma képes-e támogatni az összes feladatot és funkciót.  
3. Ha a séma módosítására van szükség (új mezők, táblák, relációk, indexek stb.), végezd el a módosításokat közvetlenül a **create_pc_configurator_db.sql**-ben.  
4. Ezután olvasd be az első feladatot a **tasks.md**-ből (Magas/Közepes/Alacsony prioritás).  
5. Végezd el a feladat szerinti kód- vagy séma-módosítást:  
   - Adatbázis-módosítás esetén szerkeszd a **create_pc_configurator_db.sql**-t.  
   - Kódmódosítás esetén szerkeszd a megfelelő C# és XAML fájlokat.  
6. Amint a feladat teljesen kész, cseréld le a `TODO:` előtagot `DONE:`-ra a **tasks.md**-ben.  
7. Írd ki: **“Kész”**, és várj a megerősítésemre, mielőtt áttérnél a következő feladatra.  
8. Ne módosíts semmi mást a **tasks.md**-ben és a projektben, csak az aktuálisan feldolgozott feladatot.