﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Text;

namespace TriggerExceptionHandler.Extensions
{
    public static class HttpExtensions
    {
        private static readonly Encoding _encoding = Encoding.UTF8;
        private static readonly string _defaultContentType = "application/json";

        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static void WriteJson<T>(this HttpResponse response, T obj, string contentType = null)
        {
            response.ContentType = contentType ?? _defaultContentType;
            using (var writer = new HttpResponseStreamWriter(response.Body, _encoding))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.CloseOutput = false;
                    jsonWriter.AutoCompleteOnClose = false;

                    Serializer.Serialize(jsonWriter, obj);
                }
            }
        }
    }
}
