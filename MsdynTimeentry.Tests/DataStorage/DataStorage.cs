using MsdynTimeentry.DataStorage;
using System;
using System.Collections.Generic;

namespace MsdynTimeentry.Tests.DataStorage
{
    internal sealed class DataStorage : IDataStorage
    {
        private bool disposed = false;

        public List<DateTime> Data { get; }

        public DataStorage()
        {
            Data = new List<DateTime>();
        }

        public HashSet<string> GetDatesOfTimeentriesFromRange(DateTime beginDate, DateTime endDate, string resultDatesFormat)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(resultDatesFormat))
                throw new ArgumentException("Result dates format is empty", nameof(resultDatesFormat));

            HashSet<string> resultDates = new HashSet<string>();
            foreach (DateTime date in Data)
            {
                if (date >= beginDate && date < endDate)
                {
                    string dateToString = date.ToString(resultDatesFormat);
                    if (!resultDates.Contains(dateToString))
                        resultDates.Add(dateToString);
                }
            }

            return resultDates;
        }

        public int CreateTimeentries(IEnumerable<DateTime> dates)
        {
            CheckDisposed();
            if (dates is null)
                throw new ArgumentNullException(nameof(dates), "Dates list is null");

            int dataItemsCount = Data.Count;
            Data.AddRange(dates);

            return Data.Count - dataItemsCount;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Data.Clear();
            disposed = true;
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(DataStorage));
        }
    }
}
