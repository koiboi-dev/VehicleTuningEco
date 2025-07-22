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
    }
}