using HispBackend.Controllers;
using HispBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HispBackend.Tests;


public class ImageControllerUnitTests
{
	[Fact]
	public void Process_InvalidImage_ReturnsBadRequest()
	{
		ImageProcessingService ips = new ImageProcessingService();
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

	// [Fact]
	// public void Process_InvalidKernelSize_ReturnsBadRequest()
	// {
	// 	ImageProcessingService ips = new ImageProcessingService();
	// 	var controller = new ImageController(ips);
	// }
}
