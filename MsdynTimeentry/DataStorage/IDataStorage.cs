using System;
using System.Collections.Generic;

namespace MsdynTimeentry.DataStorage
{
    public interface IDataStorage : IDisposable
    {
        HashSet<string> GetDatesOfTimeentriesFromRange(DateTime beginDate, DateTime endDate, string resultDatesFormat);

        int CreateTimeentries(IEnumerable<DateTime> dates);
    }
}
