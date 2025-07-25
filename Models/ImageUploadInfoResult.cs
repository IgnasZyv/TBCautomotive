namespace CarHostingWeb.Models;

public class ImageUploadInfoResult
{
    public string? Url { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}