using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TriggerExceptionHandler.Attributes;
using TriggerExceptionHandler.Demo.Exceptions;

namespace TriggerExceptionHandler.Demo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DemoController : Controller
	{
		public IActionResult Index()
		{
			throw new ExpectedException("Expected exception");
		}

		[HttpGet("aggregate")]
		public Task<IActionResult> Aggregate()
		{
			var ex1 = new ExpectedException("Expected exception");
			var ex2 = new NullReferenceException("Null reference");
			var ex3 = new ApplicationException("Hey no balance");

			throw new AggregateException(ex1, ex2, ex3);
		}

		[HttpGet("single")]
		public Task<IActionResult> Single()
		{
			throw new ApplicationException("Hey no balance");
		}

		[HttpPost("validate")]
		[ValidateModelState]
		public IActionResult ValidateModelState([FromBody] DtoParameters p)
		{
			return Ok(p);
		}

		public class DtoParameters
		{
			[Required]
			public string Name { get; set; }
		}
	}
}