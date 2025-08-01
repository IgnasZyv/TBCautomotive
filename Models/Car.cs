using Google.Cloud.Firestore;
using System.Text.Json.Serialization;

namespace CarHostingWeb.Models;

// Enums for data consistency
public enum VehicleCondition
{
    New,
    Used,
    CertifiedPreOwned
}

public enum TransmissionType
{
    Manual,
    Automatic,
    CVT,
    SemiAutomatic
}

public enum FuelType
{
    Petrol,
    Diesel,
    Electric,
    Hybrid,
    PlugInHybrid
}

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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FuelType? FuelType { get; set; }

    [FirestoreProperty]
    public int Year { get; set; }

    [FirestoreProperty]
    public double Price { get; set; }
    
    [FirestoreProperty]
    public bool IsSold { get; set; }
    
    [FirestoreProperty]
    public bool IsReserved { get; set; }
    
    [FirestoreProperty]
    public string? Description { get; set; }
    
    [FirestoreProperty]
    public string? ImagePath { get; set; }
    
    [FirestoreProperty]
    public List<string>? SubImages { get; set; }

    // Enhanced properties with enums
    [FirestoreProperty]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VehicleCondition? Condition { get; set; }
    
    [FirestoreProperty]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransmissionType? Transmission { get; set; }
    
    [FirestoreProperty]
    public string? EngineSize { get; set; } // "2.0L", "3.5L V6", etc.
    
    [FirestoreProperty]
    public int? EngineCylinders { get; set; } // 4, 6, 8, etc.
    
    [FirestoreProperty]
    public int? Horsepower { get; set; } // Engine power in HP
    
    [FirestoreProperty]
    public List<string>? Features { get; set; } // ["Leather Seats", "Sunroof", "GPS Navigation", etc.]

    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }
}