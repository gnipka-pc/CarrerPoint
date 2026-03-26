using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Controllers;

[Route("api/tests")]
[ApiController]
public class TestController: ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTest()
    {
        return Ok();
    }
}