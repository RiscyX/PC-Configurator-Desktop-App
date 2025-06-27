using System;
using System.Windows;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace PC_Configurator.Helpers
{
    /// <summary>
    /// Központi hibakezelő osztály az alkalmazás számára
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// A hiba logolási alapkönytára
        /// </summary>
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PC-Configurator",
            "Logs");

        /// <summary>
        /// Inicializálja a hibakezelőt, létrehozza a log könyvtárakat
        /// </summary>
        static ErrorHandler()
        {
            try
            {
                // Létrehozzuk a log könyvtárat, ha még nem létezik
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
            }
            catch (Exception)
            {
                // Csak csendes hiba, ha nem sikerül a könyvtárat létrehozni
            }
        }

        /// <summary>
        /// Hiba kezelése és megfelelő hibaüzenet megjelenítése
        /// </summary>
        /// <param name="ex">A kivétel, ami a hibát okozta</param>
        /// <param name="context">A hiba kontextusa, pl. "CPU mentése", "Alkatrész törlése", stb.</param>
        /// <param name="showToUser">Jelzi, hogy megjelenítsen-e felhasználói felületen üzenetet</param>
        /// <returns>Felhasználói interfészhez formázott hibaüzenet</returns>
        public static string HandleError(Exception ex, string context, bool showToUser = true)
        {
            // Naplózzuk a hibát
            LogError(ex, context);

            // Felhasználóbarát hibaüzenet generálása
            string userMessage = GetUserFriendlyErrorMessage(ex, context);

            // Ha kell, akkor megjelenítjük a hibaüzenetet
            if (showToUser)
            {
                MessageBox.Show(
                    userMessage,
                    "Hiba történt",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return userMessage;
        }

        /// <summary>
        /// Adatbázis hibák kezelése és felhasználóbarát hibaüzenet megjelenítése
        /// </summary>
        /// <param name="ex">A kivétel, ami a hibát okozta</param>
        /// <param name="operation">Az adatbázisművelet, pl. "mentés", "törlés", stb.</param>
        /// <param name="objectName">Az objektum neve, amire a művelet vonatkozik, pl. "CPU", "RAM", stb.</param>
        /// <param name="showToUser">Jelzi, hogy megjelenítsen-e felhasználói felületen üzenetet</param>
        /// <returns>Felhasználói interfészhez formázott hibaüzenet</returns>
        public static string HandleDatabaseError(Exception ex, string operation, string objectName, bool showToUser = true)
        {
            string context = $"Adatbázisművelet: {operation} - {objectName}";
            
            // Ha a hibaüzenet specifikus adatbázissal kapcsolatos, akkor egyedi kezelést kaphat
            string userMessage = ex.Message.Contains("FK_")
                ? $"A(z) {objectName} nem {operation}hető, mert más objektumok hivatkoznak rá."
                : $"Hiba történt a(z) {objectName} {operation}e közben. Kérjük, próbálja újra később.";

            // Naplózzuk a hibát
            LogError(ex, context);

            // Ha kell, akkor megjelenítjük a hibaüzenetet
            if (showToUser)
            {
                MessageBox.Show(
                    userMessage,
                    $"{objectName} {operation} hiba",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return userMessage;
        }

        /// <summary>
        /// Egyszerű hibaüzenet megjelenítése a felhasználónak
        /// </summary>
        /// <param name="message">A megjelenítendő hibaüzenet</param>
        /// <param name="title">A hibaüzenet címe</param>
        /// <returns>A megjelenített üzenet</returns>
        public static string ShowError(string message, string title = "Hiba történt")
        {
            // Naplózzuk a hibát
            LogMessage($"Hibaüzenet megjelenítve: {title} - {message}");
            
            // Megjelenítjük a hibaüzenetet
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            
            return message;
        }

        /// <summary>
        /// Felhasználóbarát hibaüzenet generálása
        /// </summary>
        private static string GetUserFriendlyErrorMessage(Exception ex, string context)
        {
            // Alapértelmezett hibaüzenet
            string message = "Sajnos hiba történt az alkalmazásban.";

            // Különböző típusú hibák egyedi kezelése
            if (ex is NullReferenceException)
            {
                message = "Az alkalmazás váratlan hibába ütközött az adatok feldolgozása során.";
            }
            else if (ex is System.Data.SqlClient.SqlException sqlEx)
            {
                message = "Adatbázis hiba történt. Kérjük ellenőrizze a kapcsolatot vagy próbálja újra később.";
                
                // Egyedi kezelés a leggyakoribb SQL hibakódokra
                switch (sqlEx.Number)
                {
                    case 4060: // Nincs jogosultság
                        message = "Nincs jogosultsága az adatbázis eléréséhez.";
                        break;
                    case 18456: // Helytelen bejelentkezési adatok
                        message = "Helytelen bejelentkezési adatok az adatbázishoz.";
                        break;
                    case 2627: // Elsődleges kulcs ütközés
                    case 2601: // Egyedi index ütközés
                        message = "Ez az elem már létezik az adatbázisban.";
                        break;
                    case 547: // Idegen kulcs ütközés
                        message = "A művelet nem hajtható végre, mert más elemek függnek tőle.";
                        break;
                    case 53: // Szerver nem található
                        message = "Nem sikerült kapcsolódni az adatbázisszerverhez. Ellenőrizze a hálózati kapcsolatot.";
                        break;
                }
            }
            else if (ex is IOException)
            {
                message = "Fájlművelet közben hiba történt. Ellenőrizze a jogosultságokat és a szabad tárhelyet.";
            }
            else if (ex is OutOfMemoryException)
            {
                message = "Az alkalmazás nem tud több memóriát lefoglalni. Kérjük, zárjon be más programokat és próbálja újra.";
            }

            // Hozzáadjuk a kontextust
            if (!string.IsNullOrWhiteSpace(context))
            {
                message = $"{message} ({context})";
            }

            // Fejlesztői környezetben részletesebb hibaüzenetet adunk
            if (IsDebugMode())
            {
                message += $"\n\nHiba részletek: {ex.Message}";
                if (ex.InnerException != null)
                {
                    message += $"\nBelső kivétel: {ex.InnerException.Message}";
                }
            }

            return message;
        }

        /// <summary>
        /// Hiba naplózása fájlba
        /// </summary>
        private static void LogError(Exception ex, string context)
        {
            try
            {
                // Létrehozzuk a log fájl nevét a mai dátummal
                string logFileName = Path.Combine(
                    LogDirectory,
                    $"error_log_{DateTime.Now:yyyy-MM-dd}.txt");

                // Hozzáfűzzük a hibainformációkat a log fájlhoz
                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{context}]");
                    writer.WriteLine($"Kivétel: {ex.GetType().Name}");
                    writer.WriteLine($"Üzenet: {ex.Message}");
                    writer.WriteLine($"Forrás: {ex.Source}");
                    writer.WriteLine($"StackTrace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        writer.WriteLine($"Belső kivétel: {ex.InnerException.Message}");
                    }

                    writer.WriteLine(new string('-', 80));
                }
            }
            catch (Exception)
            {
                // Csak csendes hiba, ha nem sikerül a log fájlba írni
            }
        }

        /// <summary>
        /// Üzenet naplózása a log fájlba
        /// </summary>
        /// <param name="message">A naplózandó üzenet</param>
        private static void LogMessage(string message)
        {
            try
            {
                string logFilePath = Path.Combine(LogDirectory, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                
                // Hozzáfűzzük a log fájlhoz az új bejegyzést
                File.AppendAllText(logFilePath, logEntry);
                
                // Debug kimenetre is írjuk
                Debug.WriteLine(logEntry);
            }
            catch
            {
                // Logging a loggolás hibájáról nem igazán lehetséges...
                Debug.WriteLine("Nem sikerült a log üzenetet kiírni.");
            }
        }

        /// <summary>
        /// Ellenőrzi, hogy az alkalmazás fejlesztői módban fut-e
        /// </summary>
        private static bool IsDebugMode()
        {
            // Fejlesztői környezetben részletes hibainformációkat adunk
            bool isDebug = false;
            
#if DEBUG
            isDebug = true;
#endif

            return isDebug;
        }
    }
}
