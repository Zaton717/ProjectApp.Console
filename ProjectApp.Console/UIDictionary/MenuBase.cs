using System;
using System.Collections.Generic;
using ProjectApp.ConsoleApp.Helpers;

namespace ProjectApp.ConsoleApp.UIDictionary
{
    public abstract class MenuBase
    {
        protected abstract string Title { get; }
        protected abstract Dictionary<char, MenuOption> Options { get; }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== {Title} ===\n");

                foreach (var kv in Options)
                    Console.WriteLine($"{kv.Key}) {kv.Value.Description}");

                Console.Write("\nOpcja: ");
                var key = Console.ReadKey().KeyChar;
                Console.WriteLine();

                if (Options.TryGetValue(key, out var opt))
                {
                    if (opt.Action == null) return; // Wyjście

                    try
                    {
                        opt.Action();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd: {ex.Message}");
                        ConsoleHelpers.Pause();
                    }
                }
                else
                {
                    Console.WriteLine("Nieznana opcja.");
                    ConsoleHelpers.Pause();
                }
            }
        }
    }
}