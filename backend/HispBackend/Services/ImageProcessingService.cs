namespace HispBackend.Services;
using System.IO;
using StbImageSharp;

public class ImageProcessingService
{
    private readonly IWebHostEnvironment _environment;
	private static string DEVEL_IMG_PATH;
    
    public ImageProcessingService(IWebHostEnvironment env)
    {
        _environment = env;
        DEVEL_IMG_PATH = Path.Combine(_environment.ContentRootPath, "TestData", "tuck_tuck.jpg");
    }
	public void Process(string imagePath)
	{
        using (var stream = File.OpenRead(DEVEL_IMG_PATH))
        {
            ImageInfo? info = ImageInfo.FromStream(stream);
	    	ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            string imgType = image.GetType().ToString();
            Console.WriteLine("Image width: " + image.Width);
            Console.WriteLine("Image height: " + image.Height);
            if (info.HasValue)
                Console.WriteLine("Image type: " + info.GetType());
            Console.WriteLine("Image name: " + image.ToString());
        }
		// byte[] bytes = File.ReadAllBytes(DEVEL_IMG_PATH);


		// for (int y = 0; y < image.Height; y++)
		// {
		// 	for (int x = 0; x < image.Width; x++)
		// 	{
				
		// 	}	
		// }	
	}
}