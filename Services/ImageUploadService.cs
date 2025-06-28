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
    }
}