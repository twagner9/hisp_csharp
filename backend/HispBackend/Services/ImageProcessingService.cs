namespace HispBackend.Services;

using System.IO;
using StbImageSharp;
using StbImageWriteSharp;
using System.Runtime.InteropServices;

/*
	FIXME: Images taken from phones have EXIF data; this means there is a tag that tells the device that the image should be rotated.
	StbImage does not account for this, and just takes raw PNG data. When I pass the data back after writing through StbImage, the code
	does not account for this EXIF, which means the pixel data is typically rotated 90 degrees. The only solution to this is reading the
	EXIF orientation tag, and then swapping width and height for some of the values (specifically if the tag is:
	transpose (5), rotate 90 degrees clockwise (6), transverse (7), rotate 90 counter clockwise (8)), ensuring whole pixels (i.e. all channels)
	are moved rather than individual values, which could scramble RGB vals
*/
public partial class ImageProcessingService
{
	private readonly IWebHostEnvironment? _environment;
	private static string? DEVEL_IMG_PATH;

	public ImageProcessingService(IWebHostEnvironment env)
	{
		_environment = env;
		DEVEL_IMG_PATH = Path.Combine(_environment.ContentRootPath, "TestData", "tuck_tuck.jpg");
	}

	public ImageProcessingService()
	{
	}

	[LibraryImport("libHISPImageProcessing.so", SetLastError = true)]
	private static partial int image_blur([In] byte[] input, [Out] byte[] output, int width, int height, int kernelRadius);

	[LibraryImport("libHISPImageProcessing.so", SetLastError = true)]
	private static partial int image_gaussian_blur([In] byte[] input, [Out] byte[] output, int width, int height, int kernelRadius);

	[LibraryImport("libHISPImageProcessing.so", SetLastError = true)]
	private static partial int image_grayscale([In] byte[] input, [Out] byte[] output, int width, int height);

	[LibraryImport("libHISPImageProcessing.so", SetLastError = true)]
	private static partial int image_adjust_brightness([In] byte[] input, [Out] byte[] output, int width, int height, int interval);

	[LibraryImport("libHISPImageProcessing.so", SetLastError = true)]
	private static partial int image_simple_threshold([In] byte[] input, [Out] byte[] output, int width, int height, int threshold);

	public void Process(Stream imageStream)
	{
		ImageResult image = ImageResult.FromStream(imageStream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

		string imgType = image.GetType().ToString();
	}

	public byte[] SimpleBlur(Stream imageStream, int kernelRadius)
	{

		ImageResult img = ImageResult.FromStream(imageStream, StbImageSharp.ColorComponents.RedGreenBlue);
		int width = img.Width;
		int height = img.Height;

		byte[] output = new byte[width * height * 3];
		int result = image_blur(img.Data, output, width, height, kernelRadius);
		int centralIdx = ((2 * width) + 2) * 3;
		if (result != 0)
		{
			throw new InvalidOperationException($"Native blur failed with error code {result}");
		}

		return WriteFile(output, img.Width, img.Height);
	}

	public byte[] ConvertToGrayscale(Stream imageStream)
	{

		ImageResult img = ImageResult.FromStream(imageStream, StbImageSharp.ColorComponents.RedGreenBlue);
		int width = img.Width;
		int height = img.Height;

		byte[] output = new byte[width * height * 3];
		int result = image_grayscale(img.Data, output, width, height);
		if (result != 0)
		{
			throw new InvalidOperationException($"Native blur failed with error code {result}");
		}

		return WriteFile(output, img.Width, img.Height);
		// byte[] imgPixels = img.Data;
		// byte[] dst = new byte[imgPixels.Length];

		// const int RGB_OFFSET = 3;
		// for (int y = 0; y < img.Height; y++)
		// {
		// 	for (int x = 0; x < img.Width; x++)
		// 	{
		// 		long idx = ((y * img.Width) + x) * RGB_OFFSET;
		// 		byte avgPixelVal = (byte)(((int)imgPixels[idx] + (int)imgPixels[idx + 1] + (int)imgPixels[idx + 2]) / 3);
		// 		dst[idx] = avgPixelVal;
		// 		dst[idx + 1] = avgPixelVal;
		// 		dst[idx + 2] = avgPixelVal;
		// 	}
		// }

		// var writer = new ImageWriter();
		// var outputStream = new MemoryStream();
		// writer.WritePng(dst, img.Width, img.Height, StbImageWriteSharp.ColorComponents.RedGreenBlue, outputStream);
		// return outputStream.ToArray();
	}

	public byte[] GaussianBlur(Stream imageStream, int kernelRadius, double sigma)
	{

		ImageResult img = ImageResult.FromStream(imageStream, StbImageSharp.ColorComponents.RedGreenBlue);
		int width = img.Width;
		int height = img.Height;

		byte[] output = new byte[width * height * 3];
		int result = image_gaussian_blur(img.Data, output, width, height, kernelRadius);
		if (result != 0)
		{
			throw new InvalidOperationException($"Native blur failed with error code {result}");
		}

		return WriteFile(output, img.Width, img.Height);
	}

	public byte[] AdjustBrightness(Stream imageStream, int interval)
	{
		ImageResult img = ImageResult.FromStream(imageStream, StbImageSharp.ColorComponents.RedGreenBlue);
		int width = img.Width;
		int height = img.Height;

		byte[] output = new byte[width * height * 3];
		int result = image_adjust_brightness(img.Data, output, width, height, interval);
		if (result != 0)
		{
			throw new InvalidOperationException($"Brightness adjustment failed with error code {result}");
		}

		return WriteFile(output, img.Width, img.Height);
	}

	public byte[] SimpleThreshold(Stream imageStream, int threshold)
	{
		ImageResult img = ImageResult.FromStream(imageStream, StbImageSharp.ColorComponents.RedGreenBlue);
		int width = img.Width;
		int height = img.Height;

		byte[] output = new byte[width * height * 3];
		int result = image_simple_threshold(img.Data, output, width, height, threshold);
		if (result != 0)
		{
			throw new InvalidOperationException($"Brightness adjustment failed with error code {result}");
		}
		return WriteFile(output, img.Width, img.Height);
	}

	private static byte[] WriteFile(byte[] output, int width, int height)
	{
		using var outputStream = new MemoryStream();
		var writer = new ImageWriter();
		writer.WritePng(output, width, height, StbImageWriteSharp.ColorComponents.RedGreenBlue, outputStream);

		return outputStream.ToArray();
	}
}