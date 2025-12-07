using ProjectApp.ServiceAbstractions;
using System;
using System.Globalization;

namespace ProjectApp.ConsoleApp.Helpers
{
    public static class ConsoleHelpers
    {
        public static void Pause()
        {
            Console.WriteLine("\n[ENTER]");
            Console.ReadLine();
        }

        public static int ReadIndex(string prompt, int max)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int val) && val >= 1 && val <= max)
                return val - 1;
            return -1;
        }

        public static Guid AddKursantAndReturnId(IKursantService svc)
        {
            Console.WriteLine("\n--- NOWY KURSANT ---");

            string imie;
            do
            {
                Console.Write("Imię: ");
                imie = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(imie))
                    Console.WriteLine("Błąd: Imię nie może być puste!");
            } while (string.IsNullOrWhiteSpace(imie));

            string nazwisko;
            do
            {
                Console.Write("Nazwisko: ");
                nazwisko = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nazwisko))
                    Console.WriteLine("Błąd: Nazwisko nie może być puste!");
            } while (string.IsNullOrWhiteSpace(nazwisko));

            DateTime dataUr;
            while (true)
            {
                Console.Write("Data ur. (yyyy-MM-dd): ");
                string? dataInput = Console.ReadLine();

                if (DateTime.TryParseExact(dataInput, "yyyy-MM-dd", null, DateTimeStyles.None, out dataUr))
                {
                    if (dataUr > DateTime.Now)
                    {
                        Console.WriteLine("Błąd: Data urodzenia nie może być z przyszłości.");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Błąd: Nieprawidłowy format daty (użyj RRRR-MM-DD).");
                }
            }

            var id = svc.Create(imie, nazwisko, dataUr);
            Console.WriteLine($"Dodano kursanta: {imie} {nazwisko}");
            return id;
        }
    }
}