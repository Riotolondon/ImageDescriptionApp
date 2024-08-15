using System;

namespace ImageDescriptionApp.Models
{
    public class PastUpload
    {
        public string UniqueId { get; set; }
        public string ImageUrl { get; set; }
        public string FileName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}