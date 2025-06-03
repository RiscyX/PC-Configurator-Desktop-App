using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class Case
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FormFactor { get; set; }
        
        public Case() { }
        public Case(int id, string name, string formFactor)
        {
            Id = id;
            Name = name;
            FormFactor = formFactor;
        }

        public void SaveToDatabase()
        {
            // TODO: Implement DB save logic
        }
    }
}
