using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Services;
using ProjectApp.ConsoleApp.UIDictionary;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Abstractions;
using ProjectApp.ConsoleApp.UI;
using System;

class Program
{
    static void Main(string[] args)
    {
        // 1. Database
        var db = new MemoryDbContext();

        // 2. Repositories
        ISzkolaRepository sRepo = new SzkolaRepositoryMemory(db);
        IKursantRepository kRepo = new KursantRepositoryMemory(db);
        IUnitOfWork uow = new UnitOfWorkMemory(db);

        // 3. Services (5 serwisów)
        ISzkolaService sSvc = new SzkolaService(sRepo, kRepo, uow);
        IInstruktorService iSvc = new InstruktorService(sRepo, uow);
        IPojazdService pSvc = new PojazdService(sRepo, uow);
        IKursService kSvc = new KursService(sRepo, kRepo, uow);
        IKursantService studentSvc = new KursantService(kRepo, uow);

        // 4. Seed
        new DataSeeder(sSvc, iSvc, pSvc, kSvc, studentSvc).Seed();

        Console.WriteLine(">>> SYSTEM GOTOWY <<<");

        // 5. Run UI
        new MainMenu(sSvc, iSvc, pSvc, kSvc, studentSvc).Run();
    }
}