using CarHostingWeb.Models;
using Microsoft.Extensions.Localization;

namespace CarHostingWeb.Services;

public class LocalizationService
{
    private readonly IStringLocalizer _localizer;
    public LocalizationService(IStringLocalizerFactory factory)
    {
        var type = typeof(LocalizationService); // or use a shared `Resources` class
        _localizer = factory.Create("Strings", type.Assembly.GetName().Name!);
    }

    public string? GetString(string key) => _localizer[key];
    

    public string? GetLocalizedFuelType(FuelType? fuelType)
    {
        return fuelType switch
        {
            FuelType.Petrol => GetString("CarFuelTypePetrol"),
            FuelType.Diesel => GetString("CarFuelTypeDiesel"),
            FuelType.Electric => GetString("CarFuelTypeElectric"),
            FuelType.Hybrid => GetString("CarFuelTypeHybrid"),
            FuelType.PlugInHybrid => GetString("CarFuelTypePlugInHybrid"), // Add this if needed
            FuelType.LPG => GetString("CarFuelTypeLPG"), // Add this if needed
            FuelType.CNG => GetString("CarFuelTypeCNG"), // Add this if needed
            null => null,
            _ => fuelType.ToString() // Fallback to enum name
        };
    }

}