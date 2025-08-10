using System;
using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Newtonsoft.Json.Linq;

namespace Eco.Mods.MechanicExpansion;

public struct VehicleTuneData
    {
        public TuneValues MaxSpeedWeights;
        public TuneValues FuelConsumptionWeights;
        public TuneValues CO2EmissionWeights;
        public TuneValues StorageCapacityWeights;
        public TuneValues DecayMultiplierWeights;
        public TuneValues OffroadMultiplierWeights;

        public VehicleTuneData(TuneValueTemplate maxSpeedWeights, TuneValueTemplate fuelConsumptionWeights, TuneValueTemplate co2EmissionWeights, TuneValueTemplate storageCapacityWeights, TuneValueTemplate durabilityMultiplierWeight, TuneValueTemplate offroadMultiplierWeights)
        {
            MaxSpeedWeights = new TuneValues(maxSpeedWeights);
            FuelConsumptionWeights = new TuneValues(fuelConsumptionWeights, true);
            CO2EmissionWeights = new TuneValues(co2EmissionWeights, true);
            StorageCapacityWeights = new TuneValues(storageCapacityWeights);
            DecayMultiplierWeights = new TuneValues(durabilityMultiplierWeight, true);
            OffroadMultiplierWeights = new TuneValues(offroadMultiplierWeights, neutral:true);
        }

        public VehicleTuneData(JToken jObj)
        {
            
            MaxSpeedWeights = SafeGetWeightRelation(jObj, "max_speed_weights");
            FuelConsumptionWeights = SafeGetWeightRelation(jObj, "fuel_consumption_weights", true);
            CO2EmissionWeights = SafeGetWeightRelation(jObj, "co2_emission_weights", true);
            StorageCapacityWeights = SafeGetWeightRelation(jObj, "storage_capacity_weights");
            DecayMultiplierWeights = SafeGetWeightRelation(jObj, "durability_multiplier_weights", true); 
            OffroadMultiplierWeights = SafeGetWeightRelation(jObj, "offroad_multiplier_weights");
        }

        public static TuneValues SafeGetWeightRelation(JToken jObj, string key, bool isLowerBetter=false, bool neutral=false)
        {
            if (jObj[key] != null && jObj[key] is JObject obj)
            {
                return new TuneValues(obj, isLowerBetter, neutral);
            }

            return new TuneValues(1, 0.5f, 0.5f);
        }
        
        public EvaluatedData Evaluate(int[] tunes)
        {
            float[] evalutedDrags = new float[tunes.Length];
            for (int tuneInd = 0; tuneInd < tunes.Length; ++tuneInd)
            {
                for (int inputInd = 0; inputInd < tunes.Length; ++inputInd) 
                {
                    if (tuneInd == inputInd) continue;
                    if (tunes[inputInd] <= 0)
                    {
                        continue;
                    }
                    evalutedDrags[tuneInd] -= tunes[inputInd] * TuneManager.DRAGS[inputInd][tuneInd];
                }
            }
            return new EvaluatedData(
                MaxSpeedWeights.EvaluateInput(tunes[0] + evalutedDrags[0]),
                FuelConsumptionWeights.EvaluateInput(tunes[1] + evalutedDrags[1]),
                CO2EmissionWeights.EvaluateInput(tunes[2] + evalutedDrags[2]),
                (int) Math.Round(StorageCapacityWeights.EvaluateInput(tunes[3] + evalutedDrags[3])),
                DecayMultiplierWeights.EvaluateInput(tunes[4] + evalutedDrags[4]),
                (float)Math.Round(OffroadMultiplierWeights.EvaluateInput(tunes[5] + evalutedDrags[5]), 2)
            );
        }

        public TuneValues this[int i]
        {
            get => GetWeightRelation(i);
        }

        private TuneValues GetWeightRelation(int i)
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
                case 6:
                    return OffroadMultiplierWeights;
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
            jObject["offroad_multiplier_weights"] = OffroadMultiplierWeights.toJSON();
            return jObject;
        }
    }