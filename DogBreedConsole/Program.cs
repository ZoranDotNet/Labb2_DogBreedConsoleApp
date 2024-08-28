using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.Drawing;

namespace DogBreedConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool loop = false;
            string imagePath;

            Console.WriteLine("Dog Analyzer\n\n");

            Console.WriteLine("Upload an Image of your DOG and I will tell you what breed it is.");

            do
            {
                Console.Write("Enter the full path to the image: ");
                imagePath = Console.ReadLine().Trim('"');

                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                {
                    Console.WriteLine($"\nFile not found: {imagePath}\nTry again");
                    loop = true;
                }
                else
                {
                    loop = false;
                }
            } while (loop);



            string predictionEndpoint = "https://labb2dog-prediction.cognitiveservices.azure.com/";
            string predictionKey = "b4bcabdcd7c44f938158a1787f650bd0";
            string projectId = "7fe574ec-6751-43ba-b430-40f939a0fbc2";
            string publishedModelName = "Iteration2";


            try
            {
                // Create prediction client
                CustomVisionPredictionClient predictionClient = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
                {
                    Endpoint = predictionEndpoint
                };

                // Load image
                using (var imageStream = new FileStream(imagePath, FileMode.Open))
                {
                    var result = await predictionClient.ClassifyImageAsync(new Guid(projectId), publishedModelName, imageStream);

                    foreach (var prediction in result.Predictions)
                    {
                        Console.WriteLine($"\nBreed: {prediction.TagName}, Probability: {prediction.Probability:P1}");
                    }
                }

            }
            catch (CustomVisionErrorException ex)
            {
                Console.WriteLine($"Custom Vision API Error: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.Response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            int width;
            int height;
            Console.WriteLine("\nA Thumbnail will be saved in Root directory.\nEnter width in pixels");
            while (!int.TryParse(Console.ReadLine(), out width))
            {
                Console.WriteLine("Try again");
            }
            Console.WriteLine("Enter height");
            while (!int.TryParse(Console.ReadLine(), out height))
            {
                Console.WriteLine("Try again");
            }

            SaveThumbnail(imagePath, width, height);

            static void SaveThumbnail(string imagePath, int width, int height)
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string thumbnailPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"thumbnail_{timestamp}.jpg");

                using (Image image = Image.FromFile(imagePath))
                {

                    Image thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                    thumbnail.Save(thumbnailPath);
                }

                Console.WriteLine($"Thumbnail saved to: {thumbnailPath}");
                Console.ReadKey();
            }


        }
    }
}
