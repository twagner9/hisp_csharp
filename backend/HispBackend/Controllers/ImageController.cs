using Microsoft.AspNetCore.Mvc;
using HispBackend.Services;

namespace HispBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly ImageProcessingService _imageProcessingService;

    public ImageController(ImageProcessingService imageProcessingService)
    {
        _imageProcessingService = imageProcessingService;
    }

    [HttpGet]
    public IActionResult Test()
    {
        return Ok("Image API is working");
    }

    [HttpGet("test")]
    public IActionResult TestProcessing()
    {
        _imageProcessingService.Process("");

        return Ok("Testing complete.");
    }
}
