using System;
using System.Data.SqlClient;

namespace PC_Configurator.Models
{
    /// <summary>
    /// Extension metódusok a SqlDataReader osztályhoz
    /// </summary>
    public static class SqlDataReaderExtensions
    {
        /// <summary>
        /// Ellenőrzi, hogy a megadott oszlop létezik-e a SqlDataReader eredményhalmazában
        /// </summary>
        /// <param name="reader">A SqlDataReader példány</param>
        /// <param name="columnName">A keresett oszlop neve</param>
        /// <returns>True, ha az oszlop létezik, egyébként false</returns>
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            if (reader == null) return false;
            
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (string.Equals(reader.GetName(i), columnName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
