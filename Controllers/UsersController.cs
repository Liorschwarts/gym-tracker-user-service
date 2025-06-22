using Microsoft.AspNetCore.Mvc;

namespace GymTracker.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("User Service is working!");
    }
}