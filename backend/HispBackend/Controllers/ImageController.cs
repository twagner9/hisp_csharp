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
        if (kernelRadius > 6)
        {
            return BadRequest("Provided kernel radius is too large.");
        }
        if (kernelRadius < 1)
        {
            return BadRequest("Provided kernel radius is 0 or less.");
        }
        Console.WriteLine("ImageController.SimpleBlur executing");
        // Java's equivalent is try-with-resource: try (BufferedWriter b = new BufferedWriter(file)), which limits
        // the BufferedWriter's scope. The using keyword is the same here; it's saying that once the scope ends,
        // the lifetime of the object should end, instead of waiting for garbage collection.
        // Basically a nice little optimizaiton that prevents complete reliance on garbage collection which might
        // live too long.
        using Stream s = image.OpenReadStream();
        byte[] res = _imageProcessingService.SimpleBlur(s, kernelRadius);
        Console.WriteLine("Completed SimpleBlur");
        return File(res, "image/png");
    }
}
