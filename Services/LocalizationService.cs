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
    

    public string? GetLocalizedFuelType(string? fuelType)
    {
        return fuelType switch
        {
            "Petrol" => GetString("CarFuelTypePetrol"),
            "Diesel" => GetString("CarFuelTypeDiesel"),
            "Electric" => GetString("CarFuelTypeElectric"),
            "Hybrid" => GetString("CarFuelTypeHybrid"),
            "Other" => GetString("GeneralOther"),
            _ => fuelType // Fallback to original value
        };
    }

}