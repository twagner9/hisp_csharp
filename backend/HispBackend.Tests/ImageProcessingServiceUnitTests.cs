using HispBackend.Controllers;
using HispBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StbImageSharp;
using StbImageWriteSharp;
using Xunit;
using Xunit.Abstractions;

public class ImageProcessingServiceUnitTests
{
	public ITestOutputHelper output;

	public ImageProcessingServiceUnitTests(ITestOutputHelper output)
	{
		this.output = output;
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
		var testImg = new FormFile(s, 0, s.Length, "image", "test.png");

		///////////////////// 1. Simple blur
		var result = controller.SimpleBlur(testImg, 1);
		var fileResult = Assert.IsType<FileContentResult>(result, exactMatch: false);
		var resultStream = new MemoryStream(fileResult.FileContents);
		var resImg = ImageResult.FromStream(resultStream, StbImageSharp.ColorComponents.RedGreenBlue);
		byte[] pixelResults = resImg.Data;

		// a. test first pixel (0, 0)
		var expectedRgb = (rValue: 35, gValue: 34, bValue: 37);
		var actual = (rValue: pixelResults[0], gValue: pixelResults[1], bValue: pixelResults[2]);
		Assert.Equal(expectedRgb, actual);

		// b. test last pixel (4, 4)
		actual = (rValue: pixelResults[numPixels - 3], gValue: pixelResults[numPixels - 2], bValue: pixelResults[numPixels - 1]);
		Assert.Equal(expectedRgb, actual);

		// c. test central pixel (2, 2) in this case
		const int center = imgSize / 2;
		int testIdx = (center * imgSize + center) * 3;
		expectedRgb = (rValue: 107, gValue: 103, bValue: 112);
		actual = (rValue: pixelResults[testIdx], gValue: pixelResults[testIdx + 1], bValue: pixelResults[testIdx + 2]);
		Assert.Equal(expectedRgb, actual);

		// d. test random intermediate pixel (1, 1)
		testIdx = (imgSize + 1) * 3;
		expectedRgb = (rValue: 38, gValue: 37, bValue: 40);
		actual = (rValue: pixelResults[testIdx], gValue: pixelResults[testIdx + 1], bValue: pixelResults[testIdx + 2]);
		Assert.Equal(expectedRgb, actual);


		///////////////////// 2. Gaussian blur
		// TODO: instead of testing this in this manner, write a Python script that will apply Gaussian blur to the test image
		// I already include, and then compare the result of this operation to that one; they should be similar.
		var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gaussian_tiny_flowers.jpg");
		using var expectedStream = File.OpenRead(filePath);
		ImageResult expectedImg = ImageResult.FromStream(expectedStream);
		byte[] expectedData = expectedImg.Data;

		filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "tiny_flowers.jpg");
		using var stream = File.OpenRead(filePath);
		var loadedTestImg = new FormFile(stream, 0, stream.Length, "image", "tiny_flowers.jpg");
		result = controller.GaussianBlur(loadedTestImg, 1, 5);
		fileResult = Assert.IsType<FileContentResult>(result, exactMatch: false);
		resultStream = new MemoryStream(fileResult.FileContents);
		resImg = ImageResult.FromStream(resultStream);
		pixelResults = resImg.Data;

		// a. Test first pixel
		Assert.Equal((expectedData[0], expectedData[1], expectedData[2]), (pixelResults[0], pixelResults[1], pixelResults[2]));

		// b. Test final pixel
		Assert.Equal((expectedData[^3], expectedData[^2], expectedData[^1]), (pixelResults[^3], pixelResults[^2], pixelResults[^1]));

		// c. Test center (image size 125x125 -- center == 62 * 3 = 186)
		Assert.Equal((expectedData[186], expectedData[187], expectedData[188]), (pixelResults[186], pixelResults[187], pixelResults[188]));

		// d. Test random pixel (75; 75 * 3 == 225)
		Assert.Equal((expectedData[225], expectedData[226], expectedData[227]), (pixelResults[225], pixelResults[226], pixelResults[227]));

		///////////////////// 3. Grayscale
		s.Position = 0;
		result = controller.Grayscale(testImg);
		fileResult = Assert.IsType<FileContentResult>(result, exactMatch: false);
		resultStream = new MemoryStream(fileResult.FileContents);
		resImg = ImageResult.FromStream(resultStream, StbImageSharp.ColorComponents.RedGreenBlue);
		pixelResults = resImg.Data;


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
		testIdx = center * imgSize + center;
		actualAvg = pixelResults[testIdx];
		Assert.Equal(expectedAvg, actualAvg);

		// d. Test (1,1)
		expectedAvg = 140;
		testIdx = (imgSize + 1) * 3;
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
}