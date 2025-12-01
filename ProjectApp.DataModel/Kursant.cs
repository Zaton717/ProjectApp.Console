namespace ProjectApp.DataModel
{
    public class Kursant : Osoba
    {
        public uint IdKursanta { get; set; }
        public KategoriaPrawaJazdy Kategoria { get; set; }
        public bool Oplacony { get; set; }
    }
}