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

    [HttpPost("process")]
    public IActionResult ProcessImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("No image provided");
        }

        Console.WriteLine("Received image: " + image.FileName);
        Console.WriteLine("Content Type: " + image.ContentType);
        Console.WriteLine("Size: " + image.Length);
        using Stream stream = image.OpenReadStream();
        _imageProcessingService.Process(stream);

        return Ok(new
        {
            fileName = image.FileName
        });
    }

    // Navigate to localhost:5192/api/Image/process/blur
    [HttpPost("process/blur")]
    public IActionResult SimpleBlur(IFormFile image, int kernelRadius)
    {
        Console.WriteLine("Image received.");
        if (image == null || image.Length == 0)
        {
            return BadRequest("Invalid image provided.");
        }
        Console.WriteLine("ImageController.SimpleBlur executing");
        Stream s = image.OpenReadStream();
        byte[] res = _imageProcessingService.SimpleBlur(s, kernelRadius);
        Console.WriteLine("Completed SimpleBlur");
        return File(res, "image/png");
    }
}
