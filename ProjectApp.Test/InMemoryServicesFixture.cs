using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Abstractions;
using ProjectApp.Services;
using ProjectApp.ServiceAbstractions;
using System;

namespace ProjectApp.Test
{
    public class InMemoryServicesFixture : IDisposable
    {
        public ISzkolaService SzkolaService { get; }
        public IInstruktorService InstruktorService { get; }
        public IPojazdService PojazdService { get; }
        public IKursService KursService { get; }
        public IKursantService KursantService { get; }

        public InMemoryServicesFixture()
        {
            var db = new MemoryDbContext();


            ISzkolaRepository szkolaRepo = new SzkolaRepositoryMemory(db);
            IKursantRepository kursantRepo = new KursantRepositoryMemory(db);


            IUnitOfWork uow = new UnitOfWorkMemory(db);


            SzkolaService = new SzkolaService(szkolaRepo, kursantRepo, uow);
            InstruktorService = new InstruktorService(szkolaRepo, uow);
            PojazdService = new PojazdService(szkolaRepo, uow);
            KursService = new KursService(szkolaRepo, kursantRepo, uow);
            KursantService = new KursantService(kursantRepo, uow);


            var seeder = new DataSeeder(SzkolaService, InstruktorService, PojazdService, KursService, KursantService);
            seeder.Seed();
        }

        public void Dispose()
        {
         
        }
    }
}