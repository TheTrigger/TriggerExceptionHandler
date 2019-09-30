using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TriggerExceptionHandler.Models;
using Xunit;

namespace TriggerExceptionHandler.Test
{
    public class UnitTestController
    {
        [Fact]
        public async Task TestExceptionHandler()
        {
            using var fixture = new TestServerFixture();
            var response = await fixture.Client.GetAsync("/api/demo/");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content);

            var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);

            Assert.NotEmpty(problemDetails.Title);
            Assert.NotEmpty(problemDetails.Type);
            Assert.True(problemDetails.Status.HasValue);
            Assert.True(problemDetails.Status.Value == (int)response.StatusCode);
        }

        [Fact]
        public async Task TestModelStateHandler()
        {
            using var fixture = new TestServerFixture();
            var parameters = new StringContent(JsonConvert.SerializeObject(new
            {
                WrongParameter = "test"
            }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await fixture.Client.PostAsync("/api/demo/validate/", parameters);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content);

            var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);

            Assert.NotEmpty(problemDetails.Title);
            Assert.NotEmpty(problemDetails.Detail);
            Assert.NotEmpty(problemDetails.Type);
            Assert.True(problemDetails.Status.HasValue);
            Assert.True(problemDetails.Status.Value == (int)response.StatusCode);
            Assert.NotNull(problemDetails.Errors);
            Assert.NotEmpty(problemDetails.Errors);
        }
    }
}