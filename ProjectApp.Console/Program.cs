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
        var db = new MemoryDbContext();

        ISzkolaRepository sRepo = new SzkolaRepositoryMemory(db);
        IKursantRepository kRepo = new KursantRepositoryMemory(db);
        IUnitOfWork uow = new UnitOfWorkMemory(db);

        ISzkolaService sSvc = new SzkolaService(sRepo, kRepo, uow);
        IInstruktorService iSvc = new InstruktorService(sRepo, uow);
        IPojazdService pSvc = new PojazdService(sRepo, uow);
        IKursService kSvc = new KursService(sRepo, kRepo, uow);
        IKursantService studentSvc = new KursantService(kRepo, uow);

        new DataSeeder(sSvc, iSvc, pSvc, kSvc, studentSvc).Seed();

        Console.WriteLine(">>> SYSTEM GOTOWY <<<");

        new MainMenu(sSvc, iSvc, pSvc, kSvc, studentSvc).Run();
    }
}