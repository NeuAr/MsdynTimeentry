using System;

namespace MsdynTimeentry.DataStorage.Dataverse
{
    internal class DataStorageFactory : IDataStorageFactory
    {
        private readonly string connectionString;

        public DataStorageFactory(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Сonnection string is empty", nameof(connectionString));

            this.connectionString = connectionString;
        }

        public IDataStorage Create()
        {
            return new DataStorage(connectionString);
        }
    }
}
