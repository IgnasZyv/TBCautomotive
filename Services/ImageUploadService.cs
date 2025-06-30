using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components.Forms;

namespace CarHostingWeb.Services
{
    public class ImageUploadService(Cloudinary cloudinary)
    {
        public async Task<List<string>> UploadImagesAsync(IEnumerable<IBrowserFile> files)
        {
            var urls = new List<string>();
    
            foreach (var file in files)
            {
                try
                {
                    var maxSize = 10 * 1024 * 1024;

                    await using var stream = file.OpenReadStream(maxSize);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, memoryStream),
                        Folder = "car-listings",
                        UseFilename = true,
                        UniqueFilename = true,
                        Transformation = new Transformation()
                            .Quality("auto")
                            .FetchFormat("auto")
                            .Quality("auto:low")
                            .Width(1200)
                            .Crop("limit")
                    };

                    var result = await cloudinary.UploadAsync(uploadParams);

                    if (result.Error != null)
                    {
                        throw new Exception(result.Error.Message);
                    }

                    urls.Add(result.SecureUrl.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($@"Failed to upload {file.Name}: {ex.Message}");
                }
            }
    
            return urls;
        }

        // Convenience method for single file (returns single string or null)
        public async Task<string?> UploadImageAsync(IBrowserFile? file)
        {
            if (file == null) return null;

            var results = await UploadImagesAsync([file]); // Wrap in array
            return results.FirstOrDefault();
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                var publicId = ExtractPublicIdFromUrl(imageUrl);

                if (string.IsNullOrEmpty(publicId)) return false;

                var deletionParams = new DeletionParams(publicId);
                var result = await cloudinary.DestroyAsync(deletionParams);

                return result.Result == "ok";
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Error deleting image: {e.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMultipleImagesAsync(IEnumerable<string> imageUrls)
        {
            try
            {
                var publicIds = imageUrls
                    .Select(ExtractPublicIdFromUrl)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();

                if (publicIds.Count == 0) return true; // Nothing to delete

                var deletionParams = new DelResParams()
                {
                    PublicIds = publicIds,
                    Type = "upload",
                    ResourceType = ResourceType.Image
                };

                var result = await cloudinary.DeleteResourcesAsync(deletionParams);

                return result.Deleted?.Count == publicIds.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Error deleting multiple images: {e.Message}");
                return false;
            }
        }

        private string? ExtractPublicIdFromUrl(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.Segments;

                // Find the upload segment and get everything after it
                var uploadIndex = Array.FindIndex(segments, s => s.Contains("upload"));
                if (uploadIndex == -1) return null;
            
                // Get path after upload/version
                var pathParts = segments.Skip(uploadIndex + 2).ToArray(); // Skip upload/ and version/
                var fullPath = string.Join("", pathParts).TrimEnd('/');
            
                // Remove file extension
                var lastDotIndex = fullPath.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    fullPath = fullPath.Substring(0, lastDotIndex);
                }

                return fullPath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            
        }
    }
}