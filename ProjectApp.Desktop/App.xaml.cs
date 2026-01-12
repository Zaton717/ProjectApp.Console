using Microsoft.Extensions.DependencyInjection;
using ProjectApp.Abstractions;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Desktop.Services;
using ProjectApp.Desktop.ViewModels;
using ProjectApp.Desktop.Windows;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System;
using System.Windows;
using ProjectApp.Desktop.Views; // To naprawi błąd Views

namespace ProjectApp.Desktop
{
    // POCZĄTEK KLASY APP
    public partial class App : Application
    {
        // Metoda musi być W ŚRODKU klasy
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // 1. REJESTRACJA SERWISÓW DANYCH
            services.AddSingleton<MemoryDbContext>();
            services.AddSingleton<IUnitOfWork, UnitOfWorkMemory>();
            services.AddSingleton<ISzkolaRepository, SzkolaRepositoryMemory>();
            services.AddSingleton<IKursantRepository, KursantRepositoryMemory>();

            // 2. LOGIKA BIZNESOWA
            services.AddSingleton<ISzkolaService, SzkolaService>();
            services.AddSingleton<IPojazdService, PojazdService>();
            services.AddSingleton<IKursService, KursService>();
            services.AddSingleton<IInstruktorService, InstruktorService>();
            services.AddSingleton<IKursantService, KursantService>();
            services.AddSingleton<IDataSeeder, DataSeeder>();

            // 3. VIEWMODELS
            services.AddTransient<SchoolSelectionViewModel>();
            services.AddTransient<ManagerPanelViewModel>();
            services.AddSingleton<MainViewModel>();

            // 4. OKNA I WIDOKI
            services.AddSingleton<MainWindow>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ISzkolaDialogService, SzkolaDialogService>();

            // Rejestracja okienek dialogowych
            services.AddTransient<AddSzkolaWindow>();
            services.AddTransient<EditSzkolaWindow>();
            services.AddTransient<Func<AddSzkolaWindow>>(sp => () => sp.GetRequiredService<AddSzkolaWindow>());
            services.AddTransient<Func<EditSzkolaWindow>>(sp => () => sp.GetRequiredService<EditSzkolaWindow>());
            services.AddTransient<AddSzkolaViewModel>();
            services.AddTransient<EditSzkolaViewModel>();

            var provider = services.BuildServiceProvider();

            // SEED DANYCH
            provider.GetRequiredService<IDataSeeder>().Seed();

            // --- NAWIGACJA ---
            var selectionVm = provider.GetRequiredService<SchoolSelectionViewModel>();
            var mainVm = new MainViewModel(selectionVm);

            // Logika przełączania ekranów
            selectionVm.RequestOpenManager = (id, nazwa) =>
            {
                var managerVm = provider.GetRequiredService<ManagerPanelViewModel>();
                managerVm.ZaladujDane(id, nazwa);
                managerVm.RequestGoBack = () => mainVm.CurrentViewModel = selectionVm;
                mainVm.CurrentViewModel = managerVm;
            };

            // START APLIKACJI
            var window = provider.GetRequiredService<MainWindow>();
            window.DataContext = mainVm;
            window.Show();
        }
    } // KONIEC KLASY APP
}