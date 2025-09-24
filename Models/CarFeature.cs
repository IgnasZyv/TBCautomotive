using System;
using System.ComponentModel;

public class CategoryAttribute : Attribute
{
    public string Name { get; }
    
    public CategoryAttribute(string name)
    {
        Name = name;
    }
}

public enum CarFeature
{
    // Safety & Security
    [Category("Safety & Security")]
    [Description("ABS (Anti-lock Braking System)")]
    ABS,
    
    [Category("Safety & Security")]
    [Description("Electronic Stability Control (ESC)")]
    ElectronicStabilityControl,
    
    [Category("Safety & Security")]
    [Description("Front Airbags")]
    FrontAirbags,
    
    [Category("Safety & Security")]
    [Description("Side Airbags")]
    SideAirbags,
    
    [Category("Safety & Security")]
    [Description("Backup Camera")]
    BackupCamera,
    
    [Category("Safety & Security")]
    [Description("Blind Spot Monitoring")]
    BlindSpotMonitoring,
    
    [Category("Safety & Security")]
    [Description("Lane Keep Assist")]
    LaneKeepAssist,
    
    [Category("Safety & Security")]
    [Description("Automatic Emergency Braking")]
    AutomaticEmergencyBraking,
    
    [Category("Safety & Security")]
    [Description("Adaptive Cruise Control")]
    AdaptiveCruiseControl,
    
    [Category("Safety & Security")]
    [Description("Keyless Start/Stop")]
    KeylessStartStop,

    // Comfort & Convenience
    [Category("Comfort & Convenience")]
    [Description("Air Conditioning")]
    AirConditioning,
    
    [Category("Comfort & Convenience")]
    [Description("Heated Front Seats")]
    HeatedFrontSeats,
    
    [Category("Comfort & Convenience")]
    [Description("Power Windows")]
    PowerWindows,
    
    [Category("Comfort & Convenience")]
    [Description("Power Mirrors")]
    PowerMirrors,
    
    [Category("Comfort & Convenience")]
    [Description("Cruise Control")]
    CruiseControl,
    
    [Category("Comfort & Convenience")]
    [Description("Remote Start")]
    RemoteStart,
    
    [Category("Comfort & Convenience")]
    [Description("Power Tailgate")]
    PowerTailgate,
    
    [Category("Comfort & Convenience")]
    [Description("Automatic Headlights")]
    AutomaticHeadlights,

    // Technology
    [Category("Technology")]
    [Description("8-inch Touchscreen")]
    Touchscreen8Inch,
    
    [Category("Technology")]
    [Description("10-inch Touchscreen")]
    Touchscreen10Inch,
    
    [Category("Technology")]
    [Description("12-inch Touchscreen")]
    Touchscreen12Inch,
    
    [Category("Technology")]
    [Description("Apple CarPlay")]
    AppleCarPlay,
    
    [Category("Technology")]
    [Description("Android Auto")]
    AndroidAuto,
    
    [Category("Technology")]
    [Description("Bluetooth Connectivity")]
    BluetoothConnectivity,
    
    [Category("Technology")]
    [Description("USB Ports")]
    USBPorts,
    
    [Category("Technology")]
    [Description("Wireless Charging Pad")]
    WirelessChargingPad,
    
    [Category("Technology")]
    [Description("Built-in Navigation System")]
    NavigationSystem,

    // Audio & Entertainment
    [Category("Audio & Entertainment")]
    [Description("Premium Audio System")]
    PremiumAudioSystem,
    
    [Category("Audio & Entertainment")]
    [Description("Satellite Radio (SiriusXM)")]
    SatelliteRadio,

    // Exterior Features
    [Category("Exterior Features")]
    [Description("Sunroof")]
    Sunroof,
    
    [Category("Exterior Features")]
    [Description("Alloy Wheels")]
    AlloyWheels,
    
    [Category("Exterior Features")]
    [Description("LED Headlights")]
    LEDHeadlights,
    
    [Category("Exterior Features")]
    [Description("Dynamic Lighting")]
    DynamicLighting,

    // Performance & Drivetrain
    [Category("Performance & Drivetrain")]
    [Description("All-Wheel Drive (AWD)")]
    AllWheelDrive,
    
    [Category("Performance & Drivetrain")]
    [Description("Automatic Transmission")]
    AutomaticTransmission,
    
    [Category("Performance & Drivetrain")]
    [Description("Sport Mode")]
    SportMode,
    
    [Category("Performance & Drivetrain")]
    [Description("Turbocharger")]
    Turbocharger,

    // Seating & Interior
    [Category("Seating & Interior")]
    [Description("Leather Seats")]
    LeatherSeats,
    
    [Category("Seating & Interior")]
    [Description("Power Driver Seat")]
    PowerDriverSeat,
    
    [Category("Seating & Interior")]
    [Description("Heated Steering Wheel")]
    HeatedSteeringWheel,
    
    [Category("Seating & Interior")]
    [Description("Third Row Seating")]
    ThirdRowSeating
}

// Extension methods to easily get categories and descriptions
public static class CarFeatureExtensions
{
    public static string GetCategory(this CarFeature feature)
    {
        var fieldInfo = feature.GetType().GetField(feature.ToString());
        var categoryAttribute = (CategoryAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(CategoryAttribute));
        return categoryAttribute?.Name ?? "Unknown";
    }
    
    public static string GetDescription(this CarFeature feature)
    {
        var fieldInfo = feature.GetType().GetField(feature.ToString());
        var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
        return descriptionAttribute?.Description ?? feature.ToString();
    }
    
    public static IEnumerable<CarFeature> GetFeaturesByCategory(string category)
    {
        return Enum.GetValues<CarFeature>()
            .Where(f => f.GetCategory() == category);
    }
    
    public static IEnumerable<string> GetAllCategories()
    {
        return Enum.GetValues<CarFeature>()
            .Select(f => f.GetCategory())
            .Distinct()
            .OrderBy(c => c);
    }
}