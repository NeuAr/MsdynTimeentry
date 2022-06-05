using MsdynTimeentry.BL;
using System;
using Xunit;

namespace MsdynTimeentry.Tests
{
    public class BusinessLogicTest
    {
        [Fact]
        public void CreateDatesFromRange()
        {
            DataStorage dataStorage = new DataStorage();
            using BusinessLogicManager businessLogicManager = new BusinessLogicManager(dataStorage);

            int createdDatesCount = businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(2022, 3, 3), new DateTime(2022, 3, 9));
            Assert.Equal(7, createdDatesCount);
            Assert.Equal(7, dataStorage.Data.Count);

            createdDatesCount = businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(2022, 3, 5, 22, 13, 55, DateTimeKind.Utc), new DateTime(2022, 3, 11));
            Assert.Equal(2, createdDatesCount);
            Assert.Equal(9, dataStorage.Data.Count);

            dataStorage.Data.Sort();
            int currentDay = 3;
            foreach (DateTime date in dataStorage.Data)
            {
                Assert.Equal(currentDay, date.Day);
                Assert.Equal(3, date.Month);
                Assert.Equal(2022, date.Year);
                ++currentDay;
            }

            createdDatesCount = businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(2022, 3, 5), new DateTime(2022, 3, 8));
            Assert.Equal(0, createdDatesCount);
            Assert.Equal(9, dataStorage.Data.Count);

            createdDatesCount = businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(2022, 3, 1), new DateTime(2022, 3, 3, 23, 59, 59, DateTimeKind.Utc));
            Assert.Equal(2, createdDatesCount);
            Assert.Equal(11, dataStorage.Data.Count);

            createdDatesCount = businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(2023, 2, 9), new DateTime(2023, 3, 5));
            Assert.Equal(25, createdDatesCount);
            Assert.Equal(36, dataStorage.Data.Count);

            dataStorage.Data.Sort();
            currentDay = 1;
            int currentMonth = 3;
            int currentYear = 2022;
            foreach (DateTime date in dataStorage.Data)
            {
                if (currentDay == 12 && currentYear == 2022)
                {
                    currentDay = 9;
                    currentMonth = 2;
                    currentYear = 2023;
                }
                else if (currentDay == 29 && currentYear == 2023)
                {
                    currentDay = 1;
                    currentMonth = 3;
                }

                Assert.Equal(currentDay, date.Day);
                Assert.Equal(currentMonth, date.Month);
                Assert.Equal(currentYear, date.Year);
                ++currentDay;
            }
        }

        [Fact]
        public void NotCorrectDateRange()
        {
            DataStorage dataStorage = new DataStorage();
            using BusinessLogicManager businessLogicManager = new BusinessLogicManager(dataStorage);

            Assert.Throws<ArgumentOutOfRangeException>(() => businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(1902, 2, 9), new DateTime(2023, 3, 5)));
            Assert.Throws<ArgumentOutOfRangeException>(() => businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(2023, 3, 5), new DateTime(1968, 3, 5)));
            Assert.Throws<ArgumentOutOfRangeException>(() => businessLogicManager.CreateNotExistsTimeentriesFromRange(new DateTime(2023, 3, 5), new DateTime(2023, 3, 4)));
        }
    }
}
