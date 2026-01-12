using System;

namespace ProjectApp.Desktop.ViewModels
{
    public class SzkolaListItemVm
    {
        public Guid Id { get; set; }
        public string Nazwa { get; set; } = "";
        public string Adres { get; set; } = "";
        public string Menadzer { get; set; } = "";
        public bool CzyAktywna { get; set; }
    }
}