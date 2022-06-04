using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;

namespace MsdynTimeentry.DataStorage.Dataverse
{
    internal sealed class DataStorage : IDataStorage
    {
        private const string EntityName = "msdyn_timeentry";
        private const string StartDateFieldName = "msdyn_start";
        private const string EndDateFieldName = "msdyn_end";

        private readonly ServiceClient serviceClient;
        private bool disposed = false;

        public DataStorage(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Сonnection string is empty", nameof(connectionString));

            serviceClient = new ServiceClient(connectionString);
        }

        public HashSet<string> GetDatesOfTimeentriesFromRange(DateTime beginDate, DateTime endDate, string resultDatesFormat)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(resultDatesFormat))
                throw new ArgumentException("Result dates format is empty", nameof(resultDatesFormat));

            FilterExpression datesRangeFilter = new FilterExpression(LogicalOperator.Or);
            datesRangeFilter.AddFilter(MakeRangeFilter(StartDateFieldName, beginDate, endDate));
            datesRangeFilter.AddFilter(MakeRangeFilter(EndDateFieldName, beginDate, endDate));

            QueryExpression queryExpression = new QueryExpression()
            {
                ColumnSet = new ColumnSet(StartDateFieldName, EndDateFieldName),
                EntityName = EntityName,
                Criteria = datesRangeFilter
            };

            EntityCollection entityCollection = serviceClient.RetrieveMultiple(queryExpression);

            HashSet<string> resultDates = new HashSet<string>();
            foreach (Entity entity in entityCollection.Entities)
            {
                string entityStartDate = entity.GetAttributeValue<DateTime>(StartDateFieldName).ToString(resultDatesFormat);
                if (!resultDates.Contains(entityStartDate))
                    resultDates.Add(entityStartDate);

                string entityEndDate = entity.GetAttributeValue<DateTime>(EndDateFieldName).ToString(resultDatesFormat);
                if (!resultDates.Contains(entityEndDate))
                    resultDates.Add(entityEndDate);
            }

            return resultDates;
        }

        public int CreateTimeentries(IEnumerable<DateTime> dates)
        {
            CheckDisposed();
            if (dates is null)
                throw new ArgumentNullException(nameof(dates), "Dates list is null");

            ExecuteTransactionRequest createTimeentriesRequest = new ExecuteTransactionRequest()
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };

            foreach (DateTime date in dates)
            {
                Entity timeentry = new Entity(EntityName);
                timeentry[StartDateFieldName] = date;
                timeentry[EndDateFieldName] = date;

                CreateRequest createTimeentryRequest = new CreateRequest
                {
                    Target = timeentry
                };

                createTimeentriesRequest.Requests.Add(createTimeentryRequest);
            }

            if (createTimeentriesRequest.Requests.Count == 0)
                throw new ArgumentException("Dates list is empty", nameof(dates));

            ExecuteTransactionResponse createTimeentriesResponse = (ExecuteTransactionResponse)serviceClient.Execute(createTimeentriesRequest);
            return createTimeentriesResponse.Responses.Count;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            serviceClient.Dispose();
            disposed = true;
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(DataStorage));
        }

        private FilterExpression MakeRangeFilter(string fieldName, DateTime beginDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name is empty", nameof(fieldName));

            return new FilterExpression
            {
                FilterOperator = LogicalOperator.And,
                Conditions =
                {
                    new ConditionExpression
                    {
                        AttributeName = fieldName,
                        Operator = ConditionOperator.GreaterEqual,
                        Values = { beginDate },
                    },
                    new ConditionExpression
                    {
                        AttributeName = fieldName,
                        Operator = ConditionOperator.LessThan,
                        Values = { endDate.AddDays(1) }
                    }
                }
            };
        }
    }
}
