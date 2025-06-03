using System;

namespace PC_Configurator.Models
{
    internal class Components
    {
        public CPU Cpu { get; set; }
        public GPU Gpu { get; set; }
        public Motherboard Motherboard { get; set; }
        public RAM Ram { get; set; }
        public Storage Storage { get; set; }
        public PSU Psu { get; set; }
        public Case Case { get; set; }

        public Components() { }
        public Components(CPU cpu, GPU gpu, Motherboard motherboard, RAM ram, Storage storage, PSU psu, Case @case)
        {
            Cpu = cpu;
            Gpu = gpu;
            Motherboard = motherboard;
            Ram = ram;
            Storage = storage;
            Psu = psu;
            Case = @case;
        }
    }
}
