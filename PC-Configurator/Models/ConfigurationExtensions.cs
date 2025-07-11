using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public static class ConfigurationExtensions
    {
        // ConfigurationModel átalakítása a nézet által használt ConfigurationItem formátumba
        public static PC_Configurator.Views.App.Configs.ConfigurationItem ToConfigurationItem(this ConfigurationModel config)
        {
            // Komponensek adatainak előkészítése a megjelenítéshez
            string cpuText = config.CPU != null ? $"{config.CPU.Manufacturer} {config.CPU.Name}" : "Nincs kiválasztva";
            string gpuText = config.GPU != null ? $"{config.GPU.Manufacturer} {config.GPU.Name}" : "Nincs kiválasztva";
            string ramText = config.RAM != null ? $"{config.RAM.CapacityGB}GB {config.RAM.Type} {config.RAM.SpeedMHz}MHz" : "Nincs kiválasztva";
            string mbText = config.Motherboard != null ? $"{config.Motherboard.Manufacturer} {config.Motherboard.Name}" : "Nincs kiválasztva";
            string storageText = config.Storage != null ? $"{config.Storage.CapacityGB}GB {config.Storage.Type}" : "Nincs kiválasztva";
            string caseText = config.Case != null ? $"{config.Case.Name}" : "Nincs kiválasztva";
            
            return new PC_Configurator.Views.App.Configs.ConfigurationItem
            {
                Id = config.Id,
                Name = config.Name,
                SaveDate = config.CreatedAt.ToString("yyyy.MM.dd. HH:mm"),
                CPU = cpuText,
                GPU = gpuText,
                RAM = ramText,
                Motherboard = mbText,
                Storage = storageText,
                Case = caseText,
                Price = (int)config.Price,
                PerformanceScore = config.PerformanceScore
            };
        }
    }
}
