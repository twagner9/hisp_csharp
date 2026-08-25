using HispBackend.Controllers;
using HispBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StbImageSharp;
using StbImageWriteSharp;
using Xunit;

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

	[Fact]
	public void Process_ValidInput_ReturnsValidResult()
	{
		ImageProcessingService ips = new();
		ImageController controller = new(ips);
		// var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "tiny_flowers.jpg");
		// using var stream = File.OpenRead(filePath);
		// var testImage = new FormFile(stream, 0, stream.Length, "image", "tiny_flowers.jpg");

		// Create a 5x5 test image
		const int imgSize = 5;
		const int numChannels = 3;
		const int numPixels = imgSize * imgSize * numChannels;

		byte[] pixels = GenerateTestImg(imgSize);
		Stream s = new MemoryStream(pixels);
		var testImg = new FormFile(s, 0, imgSize * imgSize * numChannels, "image", "test.png");


		///////////////////// 1. Simple blur
		var result = controller.SimpleBlur(testImg, 1);
		var fileResult = Assert.IsType<FileContentResult>(result, exactMatch: false);
		byte[] pixelResults = fileResult.FileContents;

		// a. test first pixel (0, 0)
		var expectedRgb = (rValue: 35, gValue: 33, bValue: 36);
		var actual = (rValue: pixelResults[0], gValue: pixelResults[1], bValue: pixelResults[2]);
		Assert.Equal(expectedRgb, actual);

		// b. test last pixel (4, 4)
		actual = (rValue: pixelResults[numPixels - 3], gValue: pixelResults[numPixels - 2], bValue: pixelResults[numPixels - 1]);
		Assert.Equal(expectedRgb, actual);

		// c. test central pixel (2, 2) in this case
		int testIdx = imgSize / 2 * imgSize + imgSize / 2;
		expectedRgb = (106, 102, 111);
		actual = (rValue: pixelResults[testIdx], gValue: pixelResults[testIdx + 1], bValue: pixelResults[testIdx + 2]);
		Assert.Equal(expectedRgb, actual);

		// d. test random intermediate pixel (1, 1)
		testIdx = imgSize + 1;
		expectedRgb = (37, 36, 39);
		actual = (rValue: pixelResults[testIdx], gValue: pixelResults[testIdx + 1], bValue: pixelResults[testIdx + 2]);
		Assert.Equal(expectedRgb, actual);


		///////////////////// 2. Gaussian blur
		s.Position = 0;
		result = controller.GaussianBlur(testImg, 7, 5.5);
		// successfulResult = Assert.IsType<IActionResult>(result, exactMatch: false);


		///////////////////// 3. Grayscale
		s.Position = 0;
		result = controller.Grayscale(testImg);
		fileResult = Assert.IsType<FileContentResult>(result, exactMatch: false);
		pixelResults = fileResult.FileContents;


		// a. Test first pixel
		byte expectedAvg = 0;
		byte actualAvg = pixelResults[0];
		Assert.Equal(expectedAvg, actualAvg);

		// b. Test last pixel
		expectedAvg = 0;
		actualAvg = pixelResults[imgSize - 1];
		Assert.Equal(expectedAvg, actualAvg);

		// c. Test central pixel
		expectedAvg = 0;
		testIdx = imgSize / 2 * imgSize + imgSize / 2;
		actualAvg = pixelResults[testIdx];
		Assert.Equal(expectedAvg, actualAvg);

		// d. Test (1,1)
		expectedAvg = 140;
		testIdx = imgSize + 1;
		actualAvg = pixelResults[testIdx];
		Assert.Equal(expectedAvg, actualAvg);

		// successfulResult = Assert.IsType<IActionResult>(result, exactMatch: false);
	}

	private static byte[] GenerateTestImg(int imgSize)
	{
		const int numChannels = 3;
		byte[] res = new byte[imgSize * imgSize * numChannels];
		for (int y = 0; y < imgSize; y++)
		{
			for (int x = 0; x < imgSize; x++)
			{
				int idx = (y * imgSize + x) * numChannels;
				byte valueR;
				byte valueG;
				byte valueB;
				if (x == 0 || x == imgSize - 1 || y == 0 || y == imgSize - 1)
				{
					valueR = valueG = valueB = 0;
				}
				else
				{
					double dx = x - 2;
					double dy = y - 2;
					double distance = Math.Sqrt(dx * dx + dy * dy) * 10.0;

					valueR = (byte)(Math.Round(distance) * 10);
					valueG = (byte)Math.Max(0, valueR - 5);
					valueB = (valueR != 0) ? (byte)Math.Min(255, valueR + 5) : (byte)0;
				}

				res[idx] = valueR;
				res[idx + 1] = valueG;
				res[idx + 2] = valueB;
			}
		}

		return EncodePng(res, imgSize, imgSize);
	}

	private static byte[] EncodePng(byte[] pixels, int width, int height)
	{
		var writer = new ImageWriter();

		using var stream = new MemoryStream();

		writer.WritePng(
			pixels,
			width,
			height,
			StbImageWriteSharp.ColorComponents.RedGreenBlue,
			stream);

		return stream.ToArray();
	}

	// [Fact]
	// public void Process_InvalidKernelSize_ReturnsBadRequest()
	// {
	// 	ImageProcessingService ips = new ImageProcessingService();
	// 	var controller = new ImageController(ips);
	// }
}
