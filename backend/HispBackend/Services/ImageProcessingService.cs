namespace HispBackend.Services;

using System.IO;
using StbImageSharp;
using StbImageWriteSharp;
using System.Runtime.InteropServices;

public class ImageProcessingService
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
		Console.WriteLine("Kernel radius: " + kernelRadius);

		byte[] src = img.Data;
		byte[] dst = new byte[src.Length];

		const int RGB_OFFSET = 3;
		for (int y = 0; y < img.Height; y++)
		{
			for (int x = 0; x < img.Width; x++)
			{
				long centralIdx = (y * img.Width + x) * RGB_OFFSET;
				int pixelSumR = 0;
				int pixelSumG = 0;
				int pixelSumB = 0;
				int addedPixels = 0;
				for (int kernelRow = -kernelRadius; kernelRow <= kernelRadius; kernelRow++)
				{
					for (int kernelCol = -kernelRadius; kernelCol <= kernelRadius; kernelCol++)
					{
						if (x + kernelCol < 0 || x + kernelCol >= img.Width || y + kernelRow < 0 || y + kernelRow >= img.Height)
						{
							continue;
						}
						long kernelIdx = ((y + kernelRow) * img.Width + x + kernelCol) * RGB_OFFSET;
						pixelSumR += img.Data[kernelIdx];
						pixelSumG += img.Data[kernelIdx + 1];
						pixelSumB += img.Data[kernelIdx + 2];
						++addedPixels;
					}
				}

				int avgR = pixelSumR;
				int avgG = pixelSumG;
				int avgB = pixelSumB;

				// addPixels could be 0 if the surrounding pixels all happen to be 0
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

	public byte[] ConvertToGrayscale(Stream imageStream)
	{
		ImageResult img = ImageResult.FromStream(imageStream);
		byte[] imgPixels = img.Data;
		byte[] dst = new byte[imgPixels.Length];

		const int RGB_OFFSET = 3;
		for (int y = 0; y < img.Height; y++)
		{
			for (int x = 0; x < img.Width; x++)
			{
				long idx = ((y * img.Width) + x) * RGB_OFFSET;
				byte avgPixelVal = (byte)(((int)imgPixels[idx] + (int)imgPixels[idx + 1] + (int)imgPixels[idx + 2]) / 3);
				dst[idx] = avgPixelVal;
				dst[idx + 1] = avgPixelVal;
				dst[idx + 2] = avgPixelVal;
			}
		}

		var writer = new ImageWriter();
		var outputStream = new MemoryStream();
		writer.WritePng(dst, img.Width, img.Height, StbImageWriteSharp.ColorComponents.RedGreenBlue, outputStream);
		return outputStream.ToArray();
	}

	public byte[] GaussianBlur(Stream s, int kernelRadius, double sigma)
	{
		ImageResult img = ImageResult.FromStream(s);
		byte[] pixelData = img.Data;
		byte[] dst = new byte[pixelData.Length];

		// First set up the Guassian weights:
		double sum = 0;
		int size = 2 * kernelRadius + 1;

		// Number of weights is NOT equivalent to the size of the kernel; it's
		// size^2 because size is merely the dimensions of the kernel. The
		// actual kernel positions are then dim * dim, or size * size in this
		// case, and must all be filled in.
		double[] weights = new double[size * size];
		for (int y = -kernelRadius; y <= kernelRadius; y++)
		{
			for (int x = -kernelRadius; x <= kernelRadius; x++)
			{
				int idx = (y + kernelRadius) * size + x + kernelRadius;
				double r2 = x * x + y * y;
				double w = Math.Exp(-r2 / (2 * (sigma * sigma)));
				// Formula: 1 / (2 * pi * sigma^2) * e^-((x^2 + y^2) / (2 * sigma^2))
				weights[idx] = w;
				sum += w;
			}
		}

		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] /= sum;
		}

		const int RGB_OFFSET = 3;
		for (int y = 0; y < img.Height; y++)
		{
			for (int x = 0; x < img.Width; x++)
			{
				long centralIdx = (y * img.Width + x) * RGB_OFFSET;
				double pixelSumR = 0;
				double pixelSumG = 0;
				double pixelSumB = 0;
				double weightSum = 0;
				for (int kernelRow = -kernelRadius; kernelRow <= kernelRadius; kernelRow++)
				{
					for (int kernelCol = -kernelRadius; kernelCol <= kernelRadius; kernelCol++)
					{
						if (x + kernelCol < 0 || x + kernelCol >= img.Width || y + kernelRow < 0 || y + kernelRow >= img.Height)
						{
							continue;
						}

						long kernelIdx = ((y + kernelRow) * img.Width + x + kernelCol) * RGB_OFFSET;
						int weightIdx = (kernelRow + kernelRadius) * size + kernelCol + kernelRadius;
						double w = weights[weightIdx];

						pixelSumR += img.Data[kernelIdx] * w;
						pixelSumG += img.Data[kernelIdx + 1] * w;
						pixelSumB += img.Data[kernelIdx + 2] * w;
						weightSum += w;
					}
				}

				int outR = (int)(pixelSumR / weightSum);
				int outG = (int)(pixelSumG / weightSum);
				int outB = (int)(pixelSumB / weightSum);

				dst[centralIdx] = (byte)outR;
				dst[centralIdx + 1] = (byte)outG;
				dst[centralIdx + 2] = (byte)outB;
			}
		}

		var writer = new ImageWriter();
		var outputStream = new MemoryStream();
		writer.WritePng(dst, img.Width, img.Height, StbImageWriteSharp.ColorComponents.RedGreenBlue, outputStream);
		return outputStream.ToArray();
	}
}