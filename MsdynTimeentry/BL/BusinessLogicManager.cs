using MsdynTimeentry.DataStorage;
using System;
using System.Collections.Generic;

namespace MsdynTimeentry.BL
{
    internal class BusinessLogicManager : IDisposable
    {
        private const string DateFormat = "%yyyy-%MM-%dd";

        private readonly DateTime dateMinValue = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IDataStorage dataStorage;
        private bool disposed = false;

        BusinessLogicManager(IDataStorage dataStorage)
        {
            if (dataStorage is null)
                throw new ArgumentNullException(nameof(dataStorage), "Data storage is null");

            this.dataStorage = dataStorage;
        }

        public int CreateNotExistsTimeentriesFromRange(DateTime beginDate, DateTime endDate)
        {
            if (beginDate < dateMinValue)
                throw new ArgumentOutOfRangeException(nameof(beginDate), beginDate, "Begin date is less of minimum value (" + dateMinValue.ToString(DateFormat) + ")");

            if (endDate < dateMinValue)
                throw new ArgumentOutOfRangeException(nameof(endDate), endDate, "End date is less of minimum value (" + dateMinValue.ToString(DateFormat) + ")");

            if (endDate < beginDate)
                throw new ArgumentOutOfRangeException(nameof(endDate), endDate, "End date is less of begin date (" + beginDate.ToString(DateFormat) + ")");

            HashSet<string> existedDates = dataStorage.GetDatesOfTimeentriesFromRange(beginDate, endDate, DateFormat);
            List<DateTime> createdDates = new List<DateTime>();
            while (beginDate <= endDate)
            {
                if (!existedDates.Contains(beginDate.ToString(DateFormat)))
                    createdDates.Add(beginDate);

                beginDate = beginDate.AddDays(1);
            }

            return dataStorage.CreateTimeentries(createdDates);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            dataStorage.Dispose();
            disposed = true;
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicManager));
        }
    }
}
