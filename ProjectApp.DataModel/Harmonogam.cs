using System.Collections.Generic;

namespace ProjectApp.DataModel
{
    public class Harmonogram
    {
        public uint IdHarmonogramu { get; set; }
        // Przechowujemy jako string dla uproszczenia, w GUI można to zmienić na DateTime
        public List<string> Terminy { get; set; } = new List<string>();
        // Referencja zwrotna może być przydatna, ale nie jest konieczna w prostym modelu
        public uint IdKursu { get; set; }
    }
}