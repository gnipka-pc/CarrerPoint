using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Controllers;

[Route("api/tests")]
[ApiController]
public class TestController: ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTest()
    {
        var a = "test";
        return Ok(a);
    }

    [HttpGet]
    public async Task<IActionResult> GetTests()
    {
        return Ok();
    }
}