using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class Motherboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Chipset { get; set; }
        public string Socket { get; set; }

        public Motherboard() { }
        public Motherboard(int id, string name, string manufacturer, string chipset, string socket)
        {
            Id = id;
            Name = name;
            Manufacturer = manufacturer;
            Chipset = chipset;
            Socket = socket;
        }

        public void SaveToDatabase()
        {
            // TODO: Implement DB save logic
        }
    }
}
