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
            Console.Write("Imię: "); string imie = Console.ReadLine() ?? "";
            Console.Write("Nazwisko: "); string nazwisko = Console.ReadLine() ?? "";
            Console.Write("Data ur. (yyyy-MM-dd): ");

            if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, DateTimeStyles.None, out var data))
            {
                var id = svc.Create(imie, nazwisko, data);
                Console.WriteLine("Dodano kursanta.");
                return id;
            }

            Console.WriteLine("Błędna data (wymagany format yyyy-MM-dd).");
            return Guid.Empty;
        }
    }
}