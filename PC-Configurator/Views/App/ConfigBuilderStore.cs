using System;
using System.Collections.Generic;
using PC_Configurator.Models;

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// A singleton osztály a komponensek átmeneti tárolására a Components és ConfigBuilder oldalak között
    /// </summary>
    public class ConfigBuilderStore
    {
        // Singleton instance
        private static ConfigBuilderStore _instance;
        
        // A tárolt komponensek
        private Dictionary<string, ComponentInfo> _components = new Dictionary<string, ComponentInfo>();
        
        // Private konstruktor a singleton mintához
        private ConfigBuilderStore() { }
        
        // Singleton instance lekérése
        public static ConfigBuilderStore GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ConfigBuilderStore();
            }
            return _instance;
        }
        
        // Van-e tárolt komponens
        public bool HasStoredComponents => _components.Count > 0;
        
        // Komponens hozzáadása
        public void AddComponent(string type, ComponentInfo component)
        {
            if (string.IsNullOrEmpty(type) || component == null)
                return;
                
            _components[type] = component;
        }
        
        // Komponens törlése
        public void RemoveComponent(string type)
        {
            if (string.IsNullOrEmpty(type))
                return;
                
            if (_components.ContainsKey(type))
            {
                _components.Remove(type);
            }
        }
        
        // Összes komponens törlése
        public void ClearComponents()
        {
            _components.Clear();
        }
        
        // Komponens lekérdezése típus alapján
        public ComponentInfo GetComponent(string type)
        {
            if (string.IsNullOrEmpty(type) || !_components.ContainsKey(type))
                return null;
                
            return _components[type];
        }
        
        // Összes komponens lekérdezése
        public IEnumerable<KeyValuePair<string, ComponentInfo>> GetComponents()
        {
            return _components;
        }
    }
}