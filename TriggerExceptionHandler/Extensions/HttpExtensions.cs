using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;

namespace TriggerExceptionHandler.Extensions
{
    public static class HttpExtensions
    {
        private const string DefaultContentType = "application/json";

        /// <summary>
        /// Write json <see langword="async"/> to <see cref="HttpContext"/>
        /// </summary>
        internal static async Task WriteJsonAsync<T>([NotNull] this HttpResponse response, [NotNull] T obj, string contentType = null)
        {
            response.ContentType = contentType ?? DefaultContentType;

            var result = JsonSerializer.Serialize(obj);
            await response.WriteAsync(result).ConfigureAwait(false); // aspnetcore 3.0 requires WriteAsync on response
        }
    }
}