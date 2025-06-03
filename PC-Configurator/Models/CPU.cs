using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class CPU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public int Cores { get; set; }
        public int Threads { get; set; }
        public float BaseClockGHz { get; set; }
        public float BoostClockGHz { get; set; }

        public CPU() { }
        public CPU(int id, string name, string manufacturer, int cores, int threads, float baseClockGHz, float boostClockGHz)
        {
            Id = id;
            Name = name;
            Manufacturer = manufacturer;
            Cores = cores;
            Threads = threads;
            BaseClockGHz = baseClockGHz;
            BoostClockGHz = boostClockGHz;
        }

        public void SaveToDatabase()
        {
            // TODO: Implement DB save logic
        }
    }
}
