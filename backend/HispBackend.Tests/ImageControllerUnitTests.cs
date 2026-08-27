using HispBackend.Controllers;
using HispBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HispBackend.Tests;


public class ImageControllerUnitTests
{
	[Fact]
	public void Process_InvalidImage_ReturnsBadRequest()
	{
		ImageProcessingService ips = new();
		var controller = new ImageController(ips);
		var result = controller.Grayscale(null!);
		var badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid image provided.", badRequest.Value);

		result = controller.SimpleBlur(null!, 7);
		badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid image provided.", badRequest.Value);

		result = controller.GaussianBlur(null!, 7, 5.0);
		badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid image provided.", badRequest.Value);
	}

	[Fact]
	public void Process_InvalidKernelSize_ReturnsBadRequest()
	{
		ImageProcessingService ips = new();
		ImageController controller = new(ips);
		var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "tiny_flowers.jpg");
		using var stream = File.OpenRead(filePath);
		var testImage = new FormFile(stream, 0, stream.Length, "image", "tiny_flowers.jpg");

		// SimpleBlur:
		// 1. Test just too large
		var result = controller.SimpleBlur(testImage, 15);
		var badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Provided kernel radius is too large.", badRequest.Value);

		// 2. Test negative value
		result = controller.SimpleBlur(testImage, -20);
		badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Provided kernel radius is 0 or less.", badRequest.Value);

		// 3. Test maximum value
		result = controller.SimpleBlur(testImage, -20);
		var successfulResult = Assert.IsType<IActionResult>(result, exactMatch: false);

		// 4. Test minimum value
		result = controller.SimpleBlur(testImage, 1);
		successfulResult = Assert.IsType<IActionResult>(result, exactMatch: false);

		// GaussianBlur:
		// 1. Test kernel just too large
		result = controller.GaussianBlur(testImage, 14, 0);
		badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid kernel radius; must be 13 or less.", badRequest.Value);

		// 2. Test negative kernel value
		result = controller.GaussianBlur(testImage, -4, 0);
		badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Provided kernel radius is 0 or less.", badRequest.Value);

		// 3. Test even kernel value
		result = controller.GaussianBlur(testImage, 4, 0.0);
		badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid kernelRadius; must be odd integer.", badRequest.Value);

		// 4. Test maximum kernel value
		result = controller.GaussianBlur(testImage, 13, 0.0);
		successfulResult = Assert.IsType<IActionResult>(result, exactMatch: false);

		// 5. Test minimum kernel value
		result = controller.GaussianBlur(testImage, 1, 0.0);
		successfulResult = Assert.IsType<IActionResult>(result, exactMatch: false);
	}

	[Fact]
	public void Process_InvalidGaussianBlurSigma_ReturnsBadRequest()
	{
		ImageProcessingService ips = new();
		ImageController controller = new(ips);
		var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "tiny_flowers.jpg");
		using var stream = File.OpenRead(filePath);
		var testImage = new FormFile(stream, 0, stream.Length, "image", "tiny_flowers.jpg");
		// 6. Test sigma too small
		var result = controller.GaussianBlur(testImage, 3, -0.01);
		var badRequest = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Sigma value must be 0.0 or greater.", badRequest.Value);
	}

	// TODO: move this to its own source file; this is testing the ImageProcessingService output, not the ImageController


	// [Fact]
	// public void Process_InvalidKernelSize_ReturnsBadRequest()
	// {
	// 	ImageProcessingService ips = new ImageProcessingService();
	// 	var controller = new ImageController(ips);
	// }
}
