using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components.Forms;

namespace CarHostingWeb.Services
{
    public class ImageUploadService(Cloudinary cloudinary)
    {
        public async Task<string?> UploadImageAsync(IBrowserFile? file)
        {
            if (file == null)
                return null;

            // Limit file size to 10MB for this example
            var maxSize = 10 * 1024 * 1024; 
    
            await using var stream = file.OpenReadStream(maxSize);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Create upload parameters with compression settings
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.Name, memoryStream),
                Folder = "car-listings",
                UseFilename = true,
                UniqueFilename = true,
        
                // Add compression and optimization settings
                Transformation = new Transformation()
                    .Quality("auto")             // Automatic quality - Cloudinary determines optimal compression
                    .FetchFormat("auto")         // Automatically deliver in the best format (WebP for browsers that support it)
                    // For low bandwidth situations
                    .Quality("auto:low")    // Prioritize file size over quality 

                    // Resize images to reasonable dimensions
                    .Width(1200)           // Set max width
                    .Crop("limit")         // Only resize if larger than specified dimensions
            };

            // If you want more specific compression level, use this instead:
            // .Quality(80)  // 80% quality, adjust to your needs (lower = smaller file size)

            // Upload to Cloudinary
            var result = await cloudinary.UploadAsync(uploadParams);

            // Check if upload was successful
            if (result.Error != null)
            {
                throw new Exception(result.Error.Message);
            }

            // Return the secure URL of the uploaded image
            return result.SecureUrl.ToString();
        }
    }
}