using Eco.Core.Utils;
using Newtonsoft.Json.Linq;

namespace Eco.Mods.MechanicExpansion;

public struct TuneRelation
    {
        public WeightRelation MaxSpeedWeights;
        public WeightRelation FuelConsumptionWeights;
        public WeightRelation CO2EmissionWeights;
        public WeightRelation StorageCapacityWeights;
        public WeightRelation DecayMultiplierWeights;
        
        public float EfficiencyMultiplier;
        public int MountCount;
        public int StorageSlots;
        public int WeightCapacity;

        public TuneRelation(WeightRelationData maxSpeedWeights, WeightRelationData fuelConsumptionWeights, WeightRelationData co2EmissionWeights, WeightRelationData storageCapacityWeights, WeightRelationData durabilityMultiplierWeight, float efficiencyMultiplier, int mountCount, int storageSlots, int weightCapacity)
        {
            MaxSpeedWeights = new WeightRelation(maxSpeedWeights);
            FuelConsumptionWeights = new WeightRelation(fuelConsumptionWeights);
            CO2EmissionWeights = new WeightRelation(co2EmissionWeights);
            StorageCapacityWeights = new WeightRelation(storageCapacityWeights);
            DecayMultiplierWeights = new WeightRelation(durabilityMultiplierWeight);
            EfficiencyMultiplier = efficiencyMultiplier;
            MountCount = mountCount;
            StorageSlots = storageSlots;
            WeightCapacity = weightCapacity;
        }

        public TuneRelation(JToken jObj)
        {
            
            MaxSpeedWeights = SafeGetWeightRelation(jObj, "max_speed_weights");
            FuelConsumptionWeights = SafeGetWeightRelation(jObj, "fuel_consumption_weights");
            CO2EmissionWeights = SafeGetWeightRelation(jObj, "co2_emission_weights");
            StorageCapacityWeights = SafeGetWeightRelation(jObj, "storage_capacity_weights");
            DecayMultiplierWeights = SafeGetWeightRelation(jObj, "durability_multiplier_weights"); 
            EfficiencyMultiplier = (float)jObj["efficiency_multiplier"];
            MountCount = (int)jObj["mount_count"];
            StorageSlots = (int)jObj["storage_slots"];
            WeightCapacity = (int)jObj["weight_capacity"];
        }

        public static WeightRelation SafeGetWeightRelation(JToken jObj, string key)
        {
            if (jObj[key] != null && jObj[key] is JObject obj)
            {
                return new WeightRelation(obj);
            }

            return new WeightRelation(1, 0.5f);
        }
        
        public EvaluatedData Evaluate(int[] tunes)
        {
            return new EvaluatedData(
                MaxSpeedWeights.EvaluateInput(tunes[0]),
                FuelConsumptionWeights.EvaluateInput(tunes[1]),
                CO2EmissionWeights.EvaluateInput(tunes[2]),
                (int) Math.Round(StorageCapacityWeights.EvaluateInput(tunes[3])),
                DecayMultiplierWeights.EvaluateInput(tunes[4])
            );
        }

        public WeightRelation this[int i]
        {
            get => GetWeightRelation(i);
        }

        private WeightRelation GetWeightRelation(int i)
        {
            switch (i)
            {
                case 1:
                    return MaxSpeedWeights;
                case 2:
                    return FuelConsumptionWeights;
                case 3:
                    return CO2EmissionWeights;
                case 4:
                    return StorageCapacityWeights;
                case 5:
                    return DecayMultiplierWeights;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public JObject toJSON()
        {
            JObject jObject = new JObject();
            jObject["max_speed_weights"] = MaxSpeedWeights.toJSON();
            jObject["fuel_consumption_weights"] = FuelConsumptionWeights.toJSON();
            jObject["co2_emission_weights"] = CO2EmissionWeights.toJSON();
            jObject["storage_capacity_weights"] = StorageCapacityWeights.toJSON();
            jObject["durability_multiplier_weights"] = DecayMultiplierWeights.toJSON();
            jObject["efficiency_multiplier"] = EfficiencyMultiplier;
            jObject["mount_count"] = MountCount;
            jObject["storage_slots"] = StorageSlots;
            jObject["weight_capacity"] = WeightCapacity;
            return jObject;
        }
    }