namespace HispBackend.Services;
using System.IO;
using StbImageSharp;

public class ImageProcessingService
{
	private static string DEVEL_IMG_PATH = "/home/twagner/Pictures/Tucker_Dog_In_The_Grass.jpg";
	public void Process(string imagePath)
	{
		byte[] bytes = File.ReadAllBytes(DEVEL_IMG_PATH);
		ImageResult image = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha);
		string imgType = image.GetType().ToString();
		Console.WriteLine("Image width: " + image.Width);
		Console.WriteLine("Image height: " + image.Height);
		Console.WriteLine("Image type: " + imgType);
		Console.WriteLine("Image name: " + image.ToString());
		for (int y = 0; y < image.Height; y++)
		{
			for (int x = 0; x < image.Width; x++)
			{
				
			}	
		}	
	}
}