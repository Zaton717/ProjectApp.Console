namespace ProjectApp.Abstractions
{
    public interface IUnitOfWork
    {
        ISzkolaRepository SzkolaRepository { get; }
        void Save();
    }
}