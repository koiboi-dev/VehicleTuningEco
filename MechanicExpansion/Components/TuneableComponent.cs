using System.ComponentModel;
using Eco.Core.Controller;
using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.DynamicValues;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Mods.MechanicExpansion.Skills;
using Eco.Mods.MechanicExpansion.Talents;
using Eco.Mods.TechTree;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;

namespace Eco.Mods.MechanicExpansion
{
    [Serialized]
    [CreateComponentTabLoc("Tunable", true)]
    [LocDescription("Tuned vehicle with adjusted values, see below.")]
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

        [Autogen]
        [SyncToView]
        [UITypeName("StringTitle")]
        [PropReadOnly]
        public string DataString => GetDataString();
        
        [SyncToView(null, false)]
        [NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)]
        public string ShortString => GetShortString();

        private float InitalHumanPoweredValue = 0.5f;

        public void Initialize(int mountCount, int storageSlots, int weightCapacity, float initalHumanPowered, params object[] args)
        {
            if (tuneData.MaxSpeedValue == 0)
            {
                tuneData = TuneManager.GetDefaultData(((IRepresentsItem) Parent).RepresentedItemType);
                this.Changed(nameof(tuneData));
            }
            Parent.GetComponent<VehicleComponent>().Initialize(tuneData.MaxSpeedValue, tuneData.OffroadMultiplier, mountCount);
            if (Parent.HasComponent<FuelConsumptionComponent>())
            {
                Parent.GetComponent<FuelConsumptionComponent>().Initialize(tuneData.FuelConsumptionValue);
            }

            if (Parent.HasComponent<AirPollutionComponent>())
            {
                Parent.GetComponent<AirPollutionComponent>().Initialize(tuneData.CO2EmissionValue);
            }

            float kgPerSlot = (float)weightCapacity / storageSlots;
            float weightDiff = tuneData.StorageCapacityValue - weightCapacity;
            int slotChange = (int)Math.Ceiling(weightDiff / kgPerSlot);
            
            if (Parent.HasComponent<PublicStorageComponent>())
            {
                Parent.GetComponent<PublicStorageComponent>()
                    .Initialize(storageSlots + slotChange, tuneData.StorageCapacityValue);
            } else if (Parent.HasComponent<VehicleToolComponent>())
            { 
                Parent.GetComponent<VehicleToolComponent>().Initialize(storageSlots + slotChange, tuneData.StorageCapacityValue,
                    (float)args[0], (float)args[1], (float)args[2], (bool)args[3], (InventoryRestriction[])args[4]);
            }

            if (Parent.HasComponent<PartsComponent>())
            {
                Parent.GetComponent<PartsComponent>().DecayMultiplier = tuneData.DecayMultiplier;
            }

            InitalHumanPoweredValue = initalHumanPowered;

            base.Initialize();
        }
        
        public static SkillModifiedValue MechanicSkillBonusPoint = new SkillModifiedValue(1f, VehicleHandlingSkill.MultiplicativeStrategy, typeof(VehicleHandlingSkill), typeof(TuneableComponent), new LocString("Lets you drive more calorie efficiently"), DynamicValueType.LaborEfficiency);
        public static TalentModifiedValue TalentBonus = new TalentModifiedValue(typeof(TuneableComponent), typeof(DrivingEfficiencyTalent), 1F);
        public static MultiDynamicValue EfficiencyValue = new MultiDynamicValue(MultiDynamicOps.Multiply, MechanicSkillBonusPoint, TalentBonus);
        private Player? prevDriver = null;
        public override void Tick()
        {
            Player driver = Parent.GetComponent<VehicleComponent>().Driver;
            if (prevDriver == null && driver != null)
            {
                Parent.GetComponent<VehicleComponent>().HumanPowered(EfficiencyValue.GetCurrentValue(driver.User) * InitalHumanPoweredValue);
                prevDriver = driver;
            } else if (driver == null && prevDriver != null)
            {
                Parent.GetComponent<VehicleComponent>().HumanPowered(InitalHumanPoweredValue);
                prevDriver = null;
            }
            
            
            base.Tick();
        }

        public string GetDataString()
        {
            VehicleTuneData data = TuneManager.GetTuneRelation(((IRepresentsItem) Parent).RepresentedItemType);
            return $"""
                    Tune Values:
                    <color={data.MaxSpeedWeights.GetColorFromEvaluated(tuneData.MaxSpeedValue)}>Max Speed: {tuneData.MaxSpeedValue:0.0}</color> kmph
                    <color={data.FuelConsumptionWeights.GetColorFromEvaluated(tuneData.FuelConsumptionValue)}>Fuel Consumption: {tuneData.FuelConsumptionValue:0}</color> joules/s
                    <color={data.CO2EmissionWeights.GetColorFromEvaluated(tuneData.CO2EmissionValue)}>Emissions: {tuneData.CO2EmissionValue:0.00}</color> ppm/hour
                    <color={data.StorageCapacityWeights.GetColorFromEvaluated(tuneData.StorageCapacityValue)}>Storage Capacity: {tuneData.StorageCapacityValue / 1000:0}</color> kg
                    <color={data.DecayMultiplierWeights.GetColorFromEvaluated(tuneData.DecayMultiplier)}>Decay Multiplier: {tuneData.DecayMultiplier * 100:0.0}</color>
                    <color={data.OffroadMultiplierWeights.GetColorFromEvaluated(tuneData.OffroadMultiplier)}>Road Speed Multiplier: {tuneData.OffroadMultiplier*100:0.0}</color> %)
                    
                    Road Speed Multiplier determines how much the vehicle will be affected by the road type, lower values means off-roading will be less effective<br>. However, better terrain wont speed up the vehicle as much. Higher numbers will correspond to higher speeds on roads.
                    """;
        }

        public string GetShortString()
        {
            VehicleTuneData data = TuneManager.GetTuneRelation(((IRepresentsItem) Parent).RepresentedItemType);
            return $"""
                    Tune Values:
                    <color={data.MaxSpeedWeights.GetColorFromEvaluated(tuneData.MaxSpeedValue)}>Max Speed: {tuneData.MaxSpeedValue:0.0}</color> kmph
                    <color={data.FuelConsumptionWeights.GetColorFromEvaluated(tuneData.FuelConsumptionValue)}>Fuel Consumption: {tuneData.FuelConsumptionValue:0}</color> joules/s
                    <color={data.CO2EmissionWeights.GetColorFromEvaluated(tuneData.CO2EmissionValue)}>Emissions: {tuneData.CO2EmissionValue:0.00}</color> ppm/hour
                    <color={data.StorageCapacityWeights.GetColorFromEvaluated(tuneData.StorageCapacityValue)}>Storage Capacity: {tuneData.StorageCapacityValue / 1000:0}</color> kg
                    <color={data.DecayMultiplierWeights.GetColorFromEvaluated(tuneData.DecayMultiplier)}>Decay Multiplier: {tuneData.DecayMultiplier * 100:0.0}</color>
                    <color={data.OffroadMultiplierWeights.GetColorFromEvaluated(tuneData.OffroadMultiplier)}>Road Speed Multiplier: {tuneData.OffroadMultiplier*100:0.0}</color> %)
                    """;
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