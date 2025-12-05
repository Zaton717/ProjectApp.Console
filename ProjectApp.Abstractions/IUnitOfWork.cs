namespace ProjectApp.Abstractions
{
    public interface IUnitOfWork
    {
        int SaveChanges();
    }
}