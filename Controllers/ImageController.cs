using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageDescriptionApp.Controllers
{
    public class ImageController : Controller
    {
        private readonly string _endpoint;
        private readonly string _key;
        private readonly string _storageConnectionString;
        private readonly string _containerName;
        private readonly IMemoryCache _cache;

        public ImageController(IConfiguration configuration, IMemoryCache cache)
        {
            _endpoint = configuration["AzureCognitiveServices:Endpoint"];
            _key = configuration["AzureCognitiveServices:Key"];
            _storageConnectionString = configuration["AzureStorage:ConnectionString"];
            _containerName = configuration["AzureStorage:ContainerName"];
            _cache = cache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select an image file.";
                return View("Index");
            }

            // Upload the image to Blob Storage
            var imageUrl = await UploadImageToBlobStorage(file);

            // Analyze the image using Computer Vision API
            var description = await AnalyzeImage(imageUrl);

            ViewBag.Description = description;
            ViewBag.ImageUrl = imageUrl;
            return View("Index");
        }

        private async Task<string> UploadImageToBlobStorage(IFormFile file)
        {
            var blobServiceClient = new BlobServiceClient(_storageConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Ensure the container exists
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString();
        }

        private async Task<string> AnalyzeImage(string imageUrl)
        {
            // Check if the description is in the cache
            if (_cache.TryGetValue(imageUrl, out string description))
            {
                return description;
            }

            var computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_key))
            {
                Endpoint = _endpoint
            };

            var result = await computerVision.DescribeImageAsync(imageUrl);

            if (result.Captions.Count > 0)
            {
                description = result.Captions[0].Text;
            }
            else
            {
                description = "No description available.";
            }

            // Set cache options
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30), // Cache for 30 minutes
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            // Save the description in the cache
            _cache.Set(imageUrl, description, cacheOptions);

            return description;
        }
    }
}
