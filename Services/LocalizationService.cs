using System.Globalization;
using CarHostingWeb.Models;
using Microsoft.Extensions.Localization;

namespace CarHostingWeb.Services;

public class LocalizationService
{
    private readonly IStringLocalizer _localizer;
    
    // Add this property
    public string CurrentCulture => CultureInfo.CurrentUICulture.Name;
    public string CurrentLanguage => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    
    public LocalizationService(IStringLocalizerFactory factory)
    {
        var type = typeof(LocalizationService); // or use a shared `Resources` class
        _localizer = factory.Create("Strings", type.Assembly.GetName().Name!);
    }

    public string? GetString(string key) => _localizer[key];
    
    // New method to get localized car feature name
    public string? GetLocalizedCarFeature(CarFeature carFeature)
    {
        var key = $"CarFeature{carFeature}";
        return GetString(key);
    }
    
    // New method to get localized car feature category
    public string? GetLocalizedCarFeatureCategory(string category)
    {
        var key = category switch
        {
            "Safety & Security" => "CarFeatureCategorySafetySecurity",
            "Comfort & Convenience" => "CarFeatureCategoryComfortConvenience",
            "Technology" => "CarFeatureCategoryTechnology",
            "Audio & Entertainment" => "CarFeatureCategoryAudioEntertainment",
            "Exterior Features" => "CarFeatureCategoryExteriorFeatures",
            "Performance & Drivetrain" => "CarFeatureCategoryPerformanceDrivetrain",
            "Seating & Interior" => "CarFeatureCategorySeatingInterior",
            _ => null
        };
        
        return key != null ? GetString(key) : category;
    }
    
    // New method to get grouped features by localized category
    public Dictionary<string, List<(CarFeature Feature, string LocalizedName)>> GetGroupedLocalizedFeatures()
    {
        var groupedFeatures = new Dictionary<string, List<(CarFeature, string)>>();
        
        foreach (var feature in Enum.GetValues<CarFeature>())
        {
            var category = feature.GetCategory();
            var localizedCategory = GetLocalizedCarFeatureCategory(category) ?? category;
            var localizedFeature = GetLocalizedCarFeature(feature) ?? feature.ToString();
            
            if (!groupedFeatures.ContainsKey(localizedCategory))
            {
                groupedFeatures[localizedCategory] = new List<(CarFeature, string)>();
            }
            
            groupedFeatures[localizedCategory].Add((feature, localizedFeature));
        }
        
        return groupedFeatures;
    }
    
    // New method to get all localized features as a flat list
    public List<(CarFeature Feature, string LocalizedName, string LocalizedCategory)> GetAllLocalizedFeatures()
    {
        var features = new List<(CarFeature, string, string)>();
        
        foreach (var feature in Enum.GetValues<CarFeature>())
        {
            var category = feature.GetCategory();
            var localizedCategory = GetLocalizedCarFeatureCategory(category) ?? category;
            var localizedFeature = GetLocalizedCarFeature(feature) ?? feature.ToString();
            
            features.Add((feature, localizedFeature, localizedCategory));
        }
        
        return features.OrderBy(f => f.Item3).ThenBy(f => f.Item2).ToList();
    }

    public string? GetLocalizedFuelType(FuelType? fuelType)
    {
        return fuelType switch
        {
            FuelType.Petrol => GetString("CarFuelTypePetrol"),
            FuelType.Diesel => GetString("CarFuelTypeDiesel"),
            FuelType.Electric => GetString("CarFuelTypeElectric"),
            FuelType.Hybrid => GetString("CarFuelTypeHybrid"),
            FuelType.PlugInHybrid => GetString("CarFuelTypePlugInHybrid"), // Add this if needed
            null => null,
            _ => fuelType.ToString() // Fallback to enum name
        };
    }

    public string? GetLocalizedCondition(VehicleCondition? condition)
    {
        return condition switch
        {
            VehicleCondition.New => GetString("CarConditionNew"),
            VehicleCondition.Used => GetString("CarConditionUsed"),
            VehicleCondition.CertifiedPreOwned => GetString("CarConditionPreOwned"),
            null => null,
            _ => condition.ToString()
        };
    }

    public string? GetLocalizedTransmission(TransmissionType? transmission)
    {
        return transmission switch
        {
            TransmissionType.Manual => GetString("CarTransmissionManual"),
            TransmissionType.Automatic => GetString("CarTransmissionAutomatic"),
            TransmissionType.SemiAutomatic => GetString("CarTransmissionSemiAutomatic"),
            TransmissionType.CVT => GetString("CarTransmissionAutomaticCVT"),
            null => null,
            _ => transmission.ToString()
        };
    }
}