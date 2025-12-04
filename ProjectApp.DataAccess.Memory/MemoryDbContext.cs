using ProjectApp.DataModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectApp.DataAccess.Memory
{
    internal class MemoryDbContext
    {
        public List<Szkola> Szkoly { get; set; } = new List<Szkola>();

        private const string FilePath = "szkola_baza.json";

        // Konfiguracja JSON, aby radziła sobie z relacjami (Kurs ma Kursanta, Kursant ma Kurs...)
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        public void SaveChanges()
        {
            string jsonString = JsonSerializer.Serialize(Szkoly, _jsonOptions);
            File.WriteAllText(FilePath, jsonString);
        }

        public void LoadData()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(FilePath);
                    // Jeśli plik jest pusty, stwórz nową listę, w przeciwnym razie deserializuj
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        Szkoly = JsonSerializer.Deserialize<List<Szkola>>(jsonString, _jsonOptions) ?? new List<Szkola>();
                    }
                }
                catch
                {
                    // W razie błędu pliku (np. uszkodzony JSON), startujemy z pustą bazą
                    Szkoly = new List<Szkola>();
                }
            }
            else
            {
                Szkoly = new List<Szkola>();
            }
        }
    }
}