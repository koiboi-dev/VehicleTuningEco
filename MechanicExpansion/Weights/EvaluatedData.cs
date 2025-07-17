using Eco.Shared.Serialization;

namespace Eco.Mods.MechanicExpansion
{
    [Serialized]
    public struct EvaluatedData
    {
        public EvaluatedData(float maxSpeedValue, float fuelConsumptionValue, float co2EmissionValue,
            int storageCapacityValue, float decayMultiplier)
        {
            MaxSpeedValue = maxSpeedValue;
            FuelConsumptionValue = fuelConsumptionValue;
            CO2EmissionValue = co2EmissionValue;
            StorageCapacityValue = storageCapacityValue;
            DecayMultiplier = decayMultiplier;
        }

        public float MaxSpeedValue { get; set; }
        public float FuelConsumptionValue { get; set; }
        public float CO2EmissionValue { get; set; }
        public int StorageCapacityValue { get; set; }
        public float DecayMultiplier { get; set; }

        public void AddFromIndex(int index, float value)
        {
            switch (index)
            {
                case 0:
                    MaxSpeedValue += value;
                    break;
                case 1:
                    FuelConsumptionValue += value;
                    break;
                case 2:
                    CO2EmissionValue += value;
                    break;
                case 3:
                    StorageCapacityValue += (int) value;
                    break;
                case 4:
                    DecayMultiplier += value;
                    break;
            }
        }

        public EvaluatedData AddEvaluatedWeights(int[] inputs, TuneRelation relation)
        {
            EvaluateWeight(inputs[0], relation.MaxSpeedWeights);
            EvaluateWeight(inputs[1], relation.FuelConsumptionWeights);
            EvaluateWeight(inputs[2], relation.CO2EmissionWeights);
            EvaluateWeight(inputs[3], relation.StorageCapacityWeights);
            EvaluateWeight(inputs[4], relation.DecayMultiplierWeights);
            return this;
        }

        private void EvaluateWeight(int input, WeightRelation relation)
        {
            for (int i = 0; i < relation.weights.Length; i++)
            {
                AddFromIndex(i, relation.weights[i] * input);
            }
        }
    }
}