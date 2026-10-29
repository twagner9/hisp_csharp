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

		///////////////////// 1. Simple blur //////////////////////////////////////////////////////////////////////////////////////
		var result = controller.SimpleBlur(testImg, 1);
		byte[] pixelResults = ReturnByteArrayFromIActionResult(result);

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


		///////////////////// 2. Gaussian blur //////////////////////////////////////////////////////////////////////////////////////
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
		pixelResults = ReturnByteArrayFromIActionResult(result);

		// a. Test first pixel
		Assert.Equal((expectedData[0], expectedData[1], expectedData[2]), (pixelResults[0], pixelResults[1], pixelResults[2]));

		// b. Test final pixel
		Assert.Equal((expectedData[^3], expectedData[^2], expectedData[^1]), (pixelResults[^3], pixelResults[^2], pixelResults[^1]));

		// c. Test center (image size 125x125 -- center == 62 * 3 = 186)
		Assert.Equal((expectedData[186], expectedData[187], expectedData[188]), (pixelResults[186], pixelResults[187], pixelResults[188]));

		// d. Test random pixel (75; 75 * 3 == 225)
		Assert.Equal((expectedData[225], expectedData[226], expectedData[227]), (pixelResults[225], pixelResults[226], pixelResults[227]));

		///////////////////// 3. Grayscale //////////////////////////////////////////////////////////////////////////////////////
		s.Position = 0;
		result = controller.Grayscale(testImg);
		pixelResults = ReturnByteArrayFromIActionResult(result);


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

		// 4. Adjust image brightness //////////////////////////////////////////////////////////////////////////////////////
		// a. Adjust by 1
		stream.Position = 0;
		byte[] originalData = ImageResult.FromStream(stream).Data;
		int interval = 1;

		result = controller.AdjustBrightness(loadedTestImg, interval);
		pixelResults = ReturnByteArrayFromIActionResult(result);

		var expectedPixelVals = (Math.Clamp(originalData[0] + interval, 0, 255), Math.Clamp(originalData[1] + interval, 0, 255), Math.Clamp(originalData[2] + interval, 0, 255));
		var actualPixelVals = (pixelResults[0], pixelResults[1], pixelResults[2]);
		Assert.Equal(expectedPixelVals, actualPixelVals);

		// b. Adjust by 50
		interval = 50;
		result = controller.AdjustBrightness(loadedTestImg, interval);
		pixelResults = ReturnByteArrayFromIActionResult(result);

		expectedPixelVals = (Math.Clamp(originalData[0] + interval, 0, 255), Math.Clamp(originalData[1] + interval, 0, 255), Math.Clamp(originalData[2] + interval, 0, 255));
		actualPixelVals = (pixelResults[0], pixelResults[1], pixelResults[2]);
		Assert.Equal(expectedPixelVals, actualPixelVals);

		// c. Adjust by -50
		interval = -50;
		result = controller.AdjustBrightness(loadedTestImg, interval);
		pixelResults = ReturnByteArrayFromIActionResult(result);

		expectedPixelVals = (Math.Clamp(originalData[0] + interval, 0, 255), Math.Clamp(originalData[1] + interval, 0, 255), Math.Clamp(originalData[2] + interval, 0, 255));
		actualPixelVals = (pixelResults[0], pixelResults[1], pixelResults[2]);
		Assert.Equal(expectedPixelVals, actualPixelVals);

		// d. Adjust by 255
		interval = 255;
		result = controller.AdjustBrightness(loadedTestImg, interval);
		pixelResults = ReturnByteArrayFromIActionResult(result);

		expectedPixelVals = (Math.Clamp(originalData[0] + interval, 0, 255), Math.Clamp(originalData[1] + interval, 0, 255), Math.Clamp(originalData[2] + interval, 0, 255));
		actualPixelVals = (pixelResults[0], pixelResults[1], pixelResults[2]);
		Assert.Equal(expectedPixelVals, actualPixelVals);

		// e. Adjust by -255
		interval = -255;
		result = controller.AdjustBrightness(loadedTestImg, interval);
		pixelResults = ReturnByteArrayFromIActionResult(result);

		expectedPixelVals = (Math.Clamp(originalData[0] + interval, 0, 255), Math.Clamp(originalData[1] + interval, 0, 255), Math.Clamp(originalData[2] + interval, 0, 255));
		actualPixelVals = (pixelResults[0], pixelResults[1], pixelResults[2]);
		Assert.Equal(expectedPixelVals, actualPixelVals);

		// TODO: for AdjustBrightness function, test the following; should be placed in its own function
		// f. Test interval too big
		// g. Test interval too small
	}

	[Fact]
	public void TestSimpleThreshold()
	{
		ImageProcessingService ips = new();
		ImageController controller = new(ips);
		// First just write the tests
		// 3x3 image will be 3x3x3 for the 3 color channels, so 27 pixel vals
		byte[] input = new byte[27];

		// Do RGB
		for (int i = 0; i < 5; i++)
		{
			input[i * 3] = 50;
			input[i * 3 + 1] = 50;
			input[i * 3 + 2] = 50;
		}
		for (int i = 5; i < 9; i++)
		{
			input[i * 3] = 175;
			input[i * 3 + 1] = 175;
			input[i * 3 + 2] = 175;
		}

		byte[] trueInput = EncodePng(input, 3, 3);

		// 1. Test half of the image set to 0, half to 255
		int testThreshold = 125;
		using Stream s = new MemoryStream(trueInput);
		var inImg = new FormFile(s, 0, s.Length, "image", "test.png");
		var controllerResult = controller.SimpleThreshold(inImg, testThreshold);
		byte[] res = ReturnByteArrayFromIActionResult(controllerResult);

		byte[] expected = new byte[input.Length];
		for (int i = 0; i < 5; i++)
		{
			expected[i * 3] = 0;
			expected[i * 3 + 1] = 0;
			expected[i * 3 + 2] = 0;
		}
		Assert.Equal(expected, res);

		// 2. Test all below threshold, set to 0
		testThreshold = 200;
		controllerResult = controller.SimpleThreshold(inImg, testThreshold);
		res = ReturnByteArrayFromIActionResult(controllerResult); ;

		expected = [0, 0, 0, 0, 0, 0, 0, 0, 0];
		Assert.Equal(expected, res);

		// 3. Test all values below threshold, set to 255
		expected = [255, 255, 255, 255, 255, 255, 255, 255, 255];
		testThreshold = 25;
		controllerResult = controller.SimpleThreshold(inImg, testThreshold);
		res = ReturnByteArrayFromIActionResult(controllerResult);
		Assert.Equal(expected, res);

		// 4. Test at threshold
		expected = [0, 0, 0, 0, 0, 0, 0, 0, 0];
		testThreshold = 200;
		controllerResult = controller.SimpleThreshold(inImg, testThreshold);
		res = ReturnByteArrayFromIActionResult(controllerResult);
		Assert.Equal(expected, res);

		// 5. Test just above threshold
		testThreshold = 176;
		// expected still all 0s
		controllerResult = controller.SimpleThreshold(inImg, testThreshold);
		res = ReturnByteArrayFromIActionResult(controllerResult);
		Assert.Equal(expected, res);

		// 6. Test just below threshold
		testThreshold = 24;
		expected = [255, 255, 255, 255, 255, 255, 255, 255, 255];
		controllerResult = controller.SimpleThreshold(inImg, testThreshold);
		res = ReturnByteArrayFromIActionResult(controllerResult);
		Assert.Equal(expected, res);
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

	private static byte[] ReturnByteArrayFromIActionResult(IActionResult res)
	{
		FileContentResult fileResult = Assert.IsType<FileContentResult>(res, exactMatch: false);
		MemoryStream resultStream = new(fileResult.FileContents);
		ImageResult resImg = ImageResult.FromStream(resultStream, StbImageSharp.ColorComponents.RedGreenBlue);
		return resImg.Data;
	}
}