using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MsdynTimeentry.Http
{
    public static class RequestsConverter
    {
        public static async Task<TTarget> ConvertBodyTo<TTarget>(HttpRequest req, string jsonSchema = null)
        {
            if (req is null)
                throw new ArgumentNullException(nameof(req), "HTTP request is null");

            string requestBodyJson = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(requestBodyJson))
                throw new ArgumentException("HTTP request body is empty", nameof(req));

            JsonSerializer serializer = new JsonSerializer();
            JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(requestBodyJson));

            if (!string.IsNullOrEmpty(jsonSchema))
            {
                JSchemaValidatingReader jsonSchemaValidatingReader = new JSchemaValidatingReader(jsonTextReader)
                {
                    Schema = JSchema.Parse(jsonSchema)
                };

                return serializer.Deserialize<TTarget>(jsonSchemaValidatingReader);
            }
            else
            {
                return serializer.Deserialize<TTarget>(jsonTextReader);
            }
        }
    }
}
