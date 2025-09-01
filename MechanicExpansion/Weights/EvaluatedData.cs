using Eco.Core.Controller;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.Mods.MechanicExpansion
{
    [Serialized]
    public struct EvaluatedData
    {
        public EvaluatedData(float maxSpeedValue, float fuelConsumptionValue, float co2EmissionValue,
            int storageCapacityValue, float decayMultiplier, float offroadMultiplier)
        {
            MaxSpeedValue = maxSpeedValue;
            FuelConsumptionValue = fuelConsumptionValue;
            CO2EmissionValue = co2EmissionValue;
            StorageCapacityValue = storageCapacityValue;
            DecayMultiplier = decayMultiplier;
            OffroadMultiplier = offroadMultiplier;
        }

        [Serialized]
        public float MaxSpeedValue { get; set; }
        [Serialized]
        public float FuelConsumptionValue { get; set; }
        [Serialized]
        public float CO2EmissionValue { get; set; }
        [Serialized]
        public int StorageCapacityValue { get; set; }
        [Serialized]
        public float DecayMultiplier { get; set; }
        [Serialized]
        public float OffroadMultiplier { get; set; }
        
        public string GetShortString(VehicleTuneData data)
        {
            return $"""
                    Tune Values:
                    <color={data.MaxSpeedWeights.GetColorFromEvaluated(MaxSpeedValue)}>Max Speed: {MaxSpeedValue:0.0}</color> kmph
                    <color={data.FuelConsumptionWeights.GetColorFromEvaluated(FuelConsumptionValue)}>Fuel Consumption: {FuelConsumptionValue:0}</color> joules/s
                    <color={data.CO2EmissionWeights.GetColorFromEvaluated(CO2EmissionValue)}>Emissions: {CO2EmissionValue:0.00}</color> ppm/hour
                    <color={data.StorageCapacityWeights.GetColorFromEvaluated(StorageCapacityValue)}>Storage Capacity: {StorageCapacityValue / 1000:0}</color> kg
                    <color={data.DecayMultiplierWeights.GetColorFromEvaluated(DecayMultiplier)}>Decay Multiplier: {DecayMultiplier * 100:0.0}</color>
                    <color={data.OffroadMultiplierWeights.GetColorFromEvaluated(OffroadMultiplier)}>Road Speed Multiplier: {OffroadMultiplier*100:0.0}</color> %
                    """;
        }
    }
}