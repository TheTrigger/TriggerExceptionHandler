using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Oibi.TestHelper;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TriggerExceptionHandler.Demo;
using Xunit;

namespace TriggerExceptionHandler.Test
{
    public class UnitTestController : IClassFixture<ServerFixture<Startup>>
	{
		private readonly ServerFixture<Startup> _testFixure;
		private readonly JsonSerializerOptions _jsonOptions;

		public UnitTestController(ServerFixture<Startup> testFixure)
		{
			_testFixure = testFixure;
			_jsonOptions = _testFixure.GetService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
		}


		[Fact]
		public void TestTextJsonInjection()
		{
			Assert.NotNull(_jsonOptions);
			Assert.Equal(JsonCommentHandling.Skip, _jsonOptions.ReadCommentHandling);
			Assert.True(_jsonOptions.PropertyNameCaseInsensitive);
		}


		[Fact]
		public async Task TestExceptionHandler()
		{
			var response = await _testFixure.Client.GetAsync("/api/demo/").ConfigureAwait(false);
			Assert.NotNull(response);
			Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

			var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			Assert.NotEmpty(content);

			var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);

			Assert.NotEmpty(problemDetails.Title);
			Assert.NotEmpty(problemDetails.Type);
			Assert.True(problemDetails.Status.HasValue);
			Assert.True(problemDetails.Status.Value == (int)response.StatusCode);
		}

		[Fact]
		public async Task TestAggregate()
		{
			var response = await _testFixure.Client.GetAsync("/api/demo/aggregate").ConfigureAwait(false);
			Assert.NotNull(response);
			Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

			var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			Assert.NotEmpty(content);

			var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);

			Assert.NotEmpty(problemDetails.Title);
			Assert.NotEmpty(problemDetails.Type);
			Assert.True(problemDetails.Status.HasValue);
			Assert.True(problemDetails.Status.Value == (int)response.StatusCode);
		}

		[Fact]
		public async Task TestASingleException()
		{
			var response = await _testFixure.Client.GetAsync("/api/demo/single").ConfigureAwait(false);
			Assert.NotNull(response);
			Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

			var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
			var parameters = new StringContent(JsonConvert.SerializeObject(new
			{
				WrongParameter = "test"
			}),
				Encoding.UTF8,
				"application/json"
			);

			var response = await _testFixure.Client.PostAsync("/api/demo/validate/", parameters).ConfigureAwait(false);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

			var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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

		[Theory]
		[InlineData("/api/wtf")]
		[InlineData("???")]
		[InlineData("hello/?query=true")]
		[InlineData("../../")]
		public async Task Test404Routes(string path)
		{
			var response = await _testFixure.Client.GetAsync(path).ConfigureAwait(false);

			Assert.NotNull(response);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}
	}
}