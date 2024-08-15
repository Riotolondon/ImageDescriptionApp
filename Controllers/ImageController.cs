using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageDescriptionApp.Models;

namespace ImageDescriptionApp.Controllers
{
    public class ImageController : Controller
    {
        private readonly string _endpoint;
        private readonly string _key;
        private readonly string _storageConnectionString;
        private readonly string _containerName;
        private readonly IMemoryCache _cache;
        private const string PastUploadsKey = "PastUploads";

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
            var pastUploads = GetPastUploads();
            ViewBag.PastUploads = pastUploads;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select an image file.";
                ViewBag.PastUploads = GetPastUploads();
                return View("Index");
            }

            string uniqueId = Guid.NewGuid().ToString();
            var imageUrl = await UploadImageToBlobStorage(file, uniqueId);
            var analysisResult = await AnalyzeImageAndCache(imageUrl);

            AddToPastUploads(uniqueId, imageUrl, file.FileName, analysisResult.Tags);

            ViewBag.Description = analysisResult.Description;
            ViewBag.ImageUrl = imageUrl;
            ViewBag.UniqueId = uniqueId;
            ViewBag.Tags = analysisResult.Tags;
            ViewBag.PastUploads = GetPastUploads();

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzePastImage(string uniqueId)
        {
            var pastUploads = GetPastUploads();
            var pastUpload = pastUploads.FirstOrDefault(p => p.UniqueId == uniqueId);

            if (pastUpload == null)
            {
                ViewBag.Message = "Image not found.";
                ViewBag.PastUploads = pastUploads;
                return View("Index");
            }

            var analysisResult = await AnalyzeImageAndCache(pastUpload.ImageUrl);

            ViewBag.Description = analysisResult.Description;
            ViewBag.ImageUrl = pastUpload.ImageUrl;
            ViewBag.UniqueId = uniqueId;
            ViewBag.Tags = analysisResult.Tags;
            ViewBag.PastUploads = pastUploads;

            return View("Index");
        }

        private async Task<string> UploadImageToBlobStorage(IFormFile file, string uniqueId)
        {
            var blobServiceClient = new BlobServiceClient(_storageConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString();
        }

        private async Task<(string Description, List<string> Tags)> AnalyzeImageAndCache(string imageUrl)
        {
            if (!_cache.TryGetValue(imageUrl, out (string Description, List<string> Tags) analysisResult))
            {
                var computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_key))
                {
                    Endpoint = _endpoint
                };

                var features = new List<VisualFeatureTypes?>
                {
                    VisualFeatureTypes.Description,
                    VisualFeatureTypes.Tags
                };

                var result = await computerVision.AnalyzeImageAsync(imageUrl, features);

                analysisResult = (
                    Description: result.Description.Captions.FirstOrDefault()?.Text ?? "No description available.",
                    Tags: result.Tags.Select(t => t.Name).ToList()
                );

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1))
                    .SetAbsoluteExpiration(TimeSpan.FromDays(7));

                _cache.Set(imageUrl, analysisResult, cacheEntryOptions);
            }

            return analysisResult;
        }

        private List<PastUpload> GetPastUploads()
        {
            if (!_cache.TryGetValue(PastUploadsKey, out List<PastUpload> pastUploads))
            {
                pastUploads = new List<PastUpload>();
                _cache.Set(PastUploadsKey, pastUploads, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return pastUploads;
        }

        private void AddToPastUploads(string uniqueId, string imageUrl, string fileName, List<string> tags)
        {
            var pastUploads = GetPastUploads();
            pastUploads.Insert(0, new PastUpload { UniqueId = uniqueId, ImageUrl = imageUrl, FileName = fileName, Tags = tags });
            if (pastUploads.Count > 10) // Keep only the last 10 uploads
            {
                pastUploads.RemoveAt(pastUploads.Count - 1);
            }
            _cache.Set(PastUploadsKey, pastUploads, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30)));
        }
    }
}