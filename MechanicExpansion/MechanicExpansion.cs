using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.Shared.Logging;

namespace Eco.Mods.MechanicExpansion
{

    public class MechanicExpansionMod : IModInit
    {
        public static ModRegistration Register() => new()
        {
            ModName = "MechanicExpansion",
            ModDescription = "Adds some tunin stuff",
            ModDisplayName = "Mechanic Expansion"
        };
    }
    
    public class MechanicExpansion : IModKitPlugin, IInitializablePlugin, IShutdownablePlugin
    {
        public string status = "Uninitialized";
		
        public string GetStatus()
        {
            return status;
        }

        public string GetCategory()
        {
            return "Mechanic Expansion";
        }

        public async Task ShutdownAsync()
        {
            TuneManager.Deinitalize();
        }

        public async void Initialize(TimedTask timer)
        {
	        Log.WriteLine(new LocString("Initializing Mechanic Expansion"));
            
            TuneManager.Initalize();

            TuneManager.AddVehicle<SteamTruckItem>(
                new TuneValueTemplate(18, 4, 3), // Max speed, variance makes no difference
                new TuneValueTemplate(300, 100, 150), // Fuel consumption, higher variance preferred
                new TuneValueTemplate(0.2f, 0.1f, 0.2f), // Co2 Emissions, very high profile preferred
                new TuneValueTemplate(5000000, 2500000), // Storage capacity, variance makes no difference
                new TuneValueTemplate(1, 0.8f), // Durability weights, higher variance preferred
                new TuneValueTemplate(1, 0.4f, 0.8f)
            );

            TuneManager.AddVehicle<SteamTractorItem>(
                new TuneValueTemplate(12, 4),
                new TuneValueTemplate(225, 100, 120),
                new TuneValueTemplate(0.07f, 0.055f),
                new TuneValueTemplate(2500000, 100000), // Applies to attached objects as well
                new TuneValueTemplate(1, 0.4f), 
                new TuneValueTemplate(1, 0.4f, 0.8f)
            );
            
            TuneManager.AddVehicle<PoweredCartItem>(
                new TuneValueTemplate(12, 8),
                new TuneValueTemplate(110, 40, 90),
                new TuneValueTemplate(0.1f, 0.09f),
                new TuneValueTemplate(3500000, 100000),
                new TuneValueTemplate(1, 0.8f),
                new TuneValueTemplate(1, 0.4f, 0.8f)
            );
            
            TuneManager.AddVehicle<TruckItem>(
                new TuneValueTemplate(20, 8, 4),
                new TuneValueTemplate(250, 50, 100),
                new TuneValueTemplate(0.5f, 0.2f, 1f),
                new TuneValueTemplate(8000000, 3000000),
                new TuneValueTemplate(1, 0.8f),
                new TuneValueTemplate(4, 2f, 1.5f)
            );
            
            TuneManager.AddVehicle<SmallWoodCartItem>(
                new TuneValueTemplate(10, 2, 3),
                new TuneValueTemplate(0,0),
                new TuneValueTemplate(0,0),
                new TuneValueTemplate(1400000, 800000),
                new TuneValueTemplate(0,0),
                new TuneValueTemplate(1, 0.25f, 0.5f)
            );
            TuneManager.AddVehicle<WoodCartItem>(
                new TuneValueTemplate(12, 4, 2),
                new TuneValueTemplate(0,0),
                new TuneValueTemplate(0,0),
                new TuneValueTemplate(1400000, 800000),
                new TuneValueTemplate(1, 0.8f),
                new TuneValueTemplate(1, 0.35f, 0.5f)
            );
            
            status = "Running";
        }
    }
}