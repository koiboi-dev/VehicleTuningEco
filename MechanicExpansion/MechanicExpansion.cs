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
            TuneRelationManager.Deinitalize();
        }

        public async void Initialize(TimedTask timer)
        {
	        Log.WriteLine(new LocString("Initializing Mechanic Expansion"));
            
            TuneRelationManager.Initalize();

            TuneRelationManager.AddVehicle<SteamTruckItem>(
                new WeightRelationData(18, 10),
                new WeightRelationData(300, 40),
                new WeightRelationData(0.2f, 0.15f),
                new WeightRelationData(5000000, 1000000),
                new WeightRelationData(1, 0.6f),
                3,2, 24, 5000000
            );

            /*TuneRelationManager.AddVehicle<SteamTractorItem>(
                new WeightRelationData(12, 3),
                new WeightRelationData(225, 60),
                new WeightRelationData(0.07f, 0.025f),
                new WeightRelationData(0, 0),
                new WeightRelationData(1, 0.4f),
                1, 1, 0, 0
            );*/
            status = "Running";
            // TODO Test all the systems, make sure that durability weights is properly added.
        }
    }
}