using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class ComponentInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public int Power { get; set; }
        public string Type { get; set; }

        public ComponentInfo()
        {
        }

        public ComponentInfo(int id, string name, string details, decimal price, int power, string type = null)
        {
            Id = id;
            Name = name;
            Details = details;
            Price = price;
            Power = power;
            Type = type;
        }

        // Konverziós segédmetódusok
        public static ComponentInfo FromCPU(CPU cpu)
        {
            if (cpu == null) return null;
            
            return new ComponentInfo
            {
                Id = cpu.Id,
                Name = cpu.Name,
                Details = $"{cpu.Cores} mag / {cpu.Threads} szál, {cpu.BaseClockGHz} GHz ({cpu.BoostClockGHz} GHz)",
                Price = 0, // Az ár az adatbázisból származik
                Power = 0, // A fogyasztás az adatbázisból származik
                Type = "CPU"
            };
        }

        public static ComponentInfo FromGPU(GPU gpu)
        {
            if (gpu == null) return null;
            
            return new ComponentInfo
            {
                Id = gpu.Id,
                Name = gpu.Name,
                Details = $"{gpu.Memory}GB {gpu.MemoryType}",
                Price = 0,
                Power = 0,
                Type = "GPU"
            };
        }

        public static ComponentInfo FromRAM(RAM ram)
        {
            if (ram == null) return null;
            
            return new ComponentInfo
            {
                Id = ram.Id,
                Name = ram.Name,
                Details = $"{ram.CapacityGB}GB {ram.Type}, {ram.SpeedMHz} MHz",
                Price = 0,
                Power = 0,
                Type = "RAM"
            };
        }

        public static ComponentInfo FromMotherboard(Motherboard mb)
        {
            if (mb == null) return null;
            
            return new ComponentInfo
            {
                Id = mb.Id,
                Name = mb.Name,
                Details = $"{mb.Chipset}, {mb.Socket}",
                Price = 0,
                Power = 0,
                Type = "Motherboard"
            };
        }

        public static ComponentInfo FromStorage(Storage storage)
        {
            if (storage == null) return null;
            
            return new ComponentInfo
            {
                Id = storage.Id,
                Name = storage.Name,
                Details = $"{storage.Capacity}GB {storage.Type}",
                Price = 0,
                Power = 0,
                Type = "Storage"
            };
        }

        public static ComponentInfo FromPSU(PSU psu)
        {
            if (psu == null) return null;
            
            return new ComponentInfo
            {
                Id = psu.Id,
                Name = psu.Name,
                Details = $"{psu.Wattage}W, {psu.Efficiency}",
                Price = 0,
                Power = 0,
                Type = "PSU"
            };
        }

        public static ComponentInfo FromCase(Case caseComponent)
        {
            if (caseComponent == null) return null;
            
            return new ComponentInfo
            {
                Id = caseComponent.Id,
                Name = caseComponent.Name,
                Details = $"{caseComponent.FormFactor}, {caseComponent.Color}",
                Price = 0,
                Power = 0,
                Type = "Case"
            };
        }
    }
}
