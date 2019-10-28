using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
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

        [HttpPost("validate")]
        [ValidateModelState]
        public IActionResult ValidateModelState([FromBody]DtoParameters p)
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