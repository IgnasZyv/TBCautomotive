using Google.Cloud.Firestore;

namespace CarHostingWeb.Models;

[FirestoreData]
public class Car
{
    [FirestoreDocumentId]
    public string? Id { get; set; }
    
    [FirestoreProperty]
    public string? Make { get; set; }

    [FirestoreProperty]
    public string? Model { get; set; }
    
    [FirestoreProperty]
    public double? Kilometres { get; set; }
    
    [FirestoreProperty]
    public string? FuelType { get; set; }

    [FirestoreProperty]
    public int Year { get; set; }

    [FirestoreProperty]
    public double Price { get; set; }
    
    [FirestoreProperty]
    public string? Description { get; set; }
    
    [FirestoreProperty]
    public string? ImagePath { get; set; }
    
    [FirestoreProperty]
    public List<string>? SubImages { get; set; } 

    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }
}
