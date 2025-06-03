using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class PSU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Wattage { get; set; }
        public string EfficiencyRating { get; set; }

        public PSU() { }
        public PSU(int id, string name, int wattage, string efficiencyRating)
        {
            Id = id;
            Name = name;
            Wattage = wattage;
            EfficiencyRating = efficiencyRating;
        }

        public void SaveToDatabase()
        {
            // TODO: Implement DB save logic
        }
    }
}
