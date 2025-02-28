using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

public class PathTraversalTest1 : ControllerBase
{
    [HttpGet("{path}")]
    public void Test(string path)
    {
        // High: Path traversal
        System.IO.File.Delete(path);
    }
}