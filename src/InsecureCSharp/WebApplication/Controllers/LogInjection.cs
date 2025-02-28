using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

// C#12 Feature: Primary constructor
[Route("api/[controller]")]
[ApiController]
public class LogInjection(ILogger<LogInjection> logger) : ControllerBase
{
    [HttpGet("{userInfo}")]
    public void injectLog(string userInfo)
    {
        // Low: Log forging
        logger.LogError("error!! " + userInfo);
    }
}