using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Controllers;

[Route("api/tests")]
[ApiController]
public class TestController: ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTest()
    {
        var b = "test2";
        return Ok(b);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTest()
    {
        return Ok();
    }
}