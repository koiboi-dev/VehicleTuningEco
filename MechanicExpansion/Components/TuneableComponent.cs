using System.ComponentModel;
using Eco.Core.Controller;
using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;

namespace Eco.Mods.MechanicExpansion
{
    [Serialized]
    [CreateComponentTabLoc("Tunable", true)]
    [LocDescription("CONFIGURE EVERYTHING !!!!!")]
    [HasIcon(null)]
    [RequireComponent(typeof(VehicleComponent))]
    public class TuneableComponent : WorldObjectComponent, IPersistentData
    {
        
        public object PersistentData
        {
            get => tuneData;
            set
            {
                if (value is EvaluatedData controller)
                {
                    tuneData = controller;
                }
            }
        }

        [Serialized]
        public EvaluatedData tuneData;

        [Serialized] [Eco] [SyncToView] 
        public int Tune { get; set; }

        public void Initialize(float efficiencyMultiplier, int mountCount, int storageSlots, int weightCapacity)
        {
            Log.WriteLine(Localizer.Do($"Started Tuneable Component!"));
            Parent.GetComponent<VehicleComponent>().Initialize(tuneData.MaxSpeedValue, efficiencyMultiplier, mountCount);
            if (Parent.HasComponent<FuelConsumptionComponent>())
            {
                Parent.GetComponent<FuelConsumptionComponent>().Initialize(tuneData.FuelConsumptionValue);
            }

            if (Parent.HasComponent<AirPollutionComponent>())
            {
                Parent.GetComponent<AirPollutionComponent>().Initialize(tuneData.CO2EmissionValue);
            }

            if (Parent.HasComponent<PublicStorageComponent>())
            {
                float kgPerSlot = (float)weightCapacity / storageSlots;
                float weightDiff = tuneData.StorageCapacityValue - weightCapacity;
                int slotChange = (int)Math.Ceiling(weightDiff / kgPerSlot);
                Log.WriteLine(Localizer.Do($"Component Data, {weightCapacity} : {storageSlots} - kgPerSlot : {kgPerSlot} - weightDiff : {weightDiff} - slotChange : {slotChange} - {tuneData.StorageCapacityValue}"));
                Parent.GetComponent<PublicStorageComponent>()
                    .Initialize(storageSlots + slotChange, tuneData.StorageCapacityValue);
            }

            if (Parent.HasComponent<PartsComponent>())
            {
                Parent.GetComponent<PartsComponent>().DecayMultiplier = tuneData.DecayMultiplier;
            }

            base.Initialize();
        }
    }

    /*public interface IEvaluatedData
    {
        float MaxSpeedValue { get; set; }
        float FuelConsumptionValue { get; set; }
        float CO2EmissionValue { get; set; }
        int StorageCapacityValue { get; set; }
        float DecayMultiplier { get; set; }

        public EvaluatedDataController GenerateDataController()
        {
            return new EvaluatedDataController(MaxSpeedValue, FuelConsumptionValue, CO2EmissionValue, StorageCapacityValue, DecayMultiplier);
        }
    }*/
}