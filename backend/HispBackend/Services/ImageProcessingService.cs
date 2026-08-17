namespace HispBackend.Services;

using System.IO;
using StbImageSharp;
using StbImageWriteSharp;
using System.Runtime.InteropServices;

public class ImageProcessingService
{
	private readonly IWebHostEnvironment _environment;
	private static string? DEVEL_IMG_PATH;

	public ImageProcessingService(IWebHostEnvironment env)
	{
		_environment = env;
		DEVEL_IMG_PATH = Path.Combine(_environment.ContentRootPath, "TestData", "tuck_tuck.jpg");
	}

	public void Process(Stream imageStream)
	{
		ImageResult image = ImageResult.FromStream(imageStream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

		string imgType = image.GetType().ToString();
		Console.WriteLine("Image width: " + image.Width);
		Console.WriteLine("Image height: " + image.Height);
		Console.WriteLine("Image type: " + image.GetType());
		Console.WriteLine("Image name: " + image.ToString());

		// byte[] bytes = File.ReadAllBytes(DEVEL_IMG_PATH);


		// for (int y = 0; y < image.Height; y++)
		// {
		// 	for (int x = 0; x < image.Width; x++)
		// 	{

		// 	}	
		// }	
	}

	public byte[] SimpleBlur(Stream imageStream, int kernelRadius)
	{
		ImageResult img = ImageResult.FromStream(imageStream);
		byte[] src = img.Data;
		byte[] dst = new byte[src.Length];

		// const int RGB_OFFSET = 3;
		for (int y = 0; y < img.Height; y++)
		{
			for (int x = 0; x < img.Width; x++)
			{
				long centralIdx = y * img.Width + x;
				int pixelSumR = 0;
				int pixelSumG = 0;
				int pixelSumB = 0;
				int addedPixels = 0;
				for (int kernelRow = -kernelRadius; kernelRow < kernelRadius; kernelRow++)
				{
					for (int kernelCol = -kernelRadius; kernelCol < kernelRadius; kernelCol++)
					{
						if (x + kernelCol < 0 || x + kernelCol > img.Width || y + kernelRow < 0 || y + kernelRow > img.Height)
						{
							continue;
						}
						long kernelIdx = (y + kernelRow) * img.Width + x + kernelCol;
						pixelSumR += img.Data[kernelIdx];
						pixelSumG += img.Data[kernelIdx + 1];
						pixelSumB += img.Data[kernelIdx + 2];
						++addedPixels;
					}
				}

				int avgR = pixelSumR;
				int avgG = pixelSumG;
				int avgB = pixelSumB;

				// addedPixels should never really be 0 here...why is this necessary?
				if (addedPixels > 0)
				{
					avgR /= addedPixels;
					avgG /= addedPixels;
					avgB /= addedPixels;
				}

				dst[centralIdx] = (byte)avgR;
				dst[centralIdx + 1] = (byte)avgG;
				dst[centralIdx + 2] = (byte)avgB;
			}
		}

		// dst is a byte[], but it lacks the formatting required to return an appropriate file
		// from controller to the frontend. Use StbImageWriteSharp to add this necessary data,
		// and return this instead.
		var writer = new ImageWriter();
		var outputStream = new MemoryStream();
		writer.WritePng(dst, img.Width, img.Height, StbImageWriteSharp.ColorComponents.RedGreenBlue, outputStream);

		return outputStream.ToArray();
	}
}