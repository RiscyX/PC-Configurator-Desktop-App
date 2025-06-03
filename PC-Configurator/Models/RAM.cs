using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class RAM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CapacityGB { get; set; }
        public int SpeedMHz { get; set; }

        public RAM() { }
        public RAM(int id, string name, int capacityGB, int speedMHz)
        {
            Id = id;
            Name = name;
            CapacityGB = capacityGB;
            SpeedMHz = speedMHz;
        }

        public void SaveToDatabase()
        {
            // TODO: Implement DB save logic
        }
    }
}
