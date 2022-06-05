using Microsoft.AspNetCore.Http;
using MsdynTimeentry.Http;
using MsdynTimeentry.Models;
using MsdynTimeentry.Properties;
using Newtonsoft.Json.Schema;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsdynTimeentry.Tests
{
    public class HttpRequestsConverterTest
    {
        private const string BeginDateFieldName = "StartOn";
        private const string EndDateFieldName = "EndOn";
        private const string DateFormat = "yyyy-MM-dd";

        [Fact]
        public async Task ConvertCreateMsdynTimeentriesRequests()
        {
            DateTime beginDate = new DateTime(2019, 7, 23);
            DateTime endDate = new DateTime(2023, 9, 7);
            DateRange dateRange = await RequestsConverter.ConvertBodyTo<DateRange>(
                MakeHttpRequest(beginDate, endDate), Encoding.UTF8.GetString(Resources.CreateMsdynTimeentriesRequestSchema));

            Assert.Equal(beginDate.Day, dateRange.StartOn.Day);
            Assert.Equal(beginDate.Month, dateRange.StartOn.Month);
            Assert.Equal(beginDate.Year, dateRange.StartOn.Year);

            Assert.Equal(endDate.Day, dateRange.EndOn.Day);
            Assert.Equal(endDate.Month, dateRange.EndOn.Month);
            Assert.Equal(endDate.Year, dateRange.EndOn.Year);
        }

        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData("{}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2018-02-29\", \"" + EndDateFieldName + "\": \"2018-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2017-04-16\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + EndDateFieldName + "\": \"2017-04-16\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2022-04-29\", \"" + EndDateFieldName + "\": \"2023-13-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"NoCorrectName\": \"2022-04-29\", \"" + EndDateFieldName + "\": \"2023-12-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2022-04-29\", \"NoCorrectName\": \"2023-12-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2016+02-12\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2016-02-12T03:00:00\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2016-02-12T03:00:00Z\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2016-02-12 03:00:00\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2016-02-12t\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2016/02-12\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"02-12\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        [InlineData("{\"" + BeginDateFieldName + "\": \"2016-12\", \"" + EndDateFieldName + "\": \"2016-03-29\"}", typeof(JSchemaValidationException))]
        public async Task NotCorrectMsdynTimeentriesRequests(string body, Type exceptionType)
        {
            await Assert.ThrowsAsync(
                exceptionType, 
                () => RequestsConverter.ConvertBodyTo<DateRange>(MakeHttpRequest(body), Encoding.UTF8.GetString(Resources.CreateMsdynTimeentriesRequestSchema)));
        }

        private HttpRequest MakeHttpRequest(DateTime beginDate, DateTime endDate)
        {
            string body = "{\"" + BeginDateFieldName + "\": \"" + beginDate.ToString(DateFormat) + "\", \"" + EndDateFieldName + "\": \"" + endDate.ToString(DateFormat) + "\"}";
            return MakeHttpRequest(body);
        }

        private HttpRequest MakeHttpRequest(string body)
        {
            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/json";

            if (!string.IsNullOrEmpty(body))
            {
                MemoryStream bodyStream = new MemoryStream();
                StreamWriter bodyWriter = new StreamWriter(bodyStream);
                bodyWriter.Write(body);
                bodyWriter.Flush();
                bodyStream.Position = 0;
                httpContext.Request.Body = bodyStream;
            }

            return httpContext.Request;
        }
    }
}
