using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class GPU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public int MemoryGB { get; set; }

        public GPU() { }
        public GPU(int id, string name, string manufacturer, int memoryGB)
        {
            Id = id;
            Name = name;
            Manufacturer = manufacturer;
            MemoryGB = memoryGB;
        }

        public void SaveToDatabase()
        {
            // TODO: Implement DB save logic
        }
    }
}
