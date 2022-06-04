namespace MsdynTimeentry.DataStorage
{
    public interface IDataStorageFactory
    {
        IDataStorage Create();
    }
}
