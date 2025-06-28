using Microsoft.AspNetCore.Components.Forms;

namespace CarHostingWeb.Models;
public class TempImage(string imageDataUrl, IBrowserFile imageFile)
{
    public string DataUrl { get; set; } = imageDataUrl;
    public IBrowserFile File { get; set; } = imageFile;
}