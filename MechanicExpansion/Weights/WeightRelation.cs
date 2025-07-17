using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.Shared.Utils;
using Newtonsoft.Json.Linq;

namespace Eco.Mods.MechanicExpansion;

public struct WeightRelation
    {
        public readonly float initialValue;
        public readonly float variance;
        public bool isLowerBetter;

        public WeightRelation(float initialValue, float variance, bool isLowerBetter = false)
        {
            this.initialValue = initialValue;
            this.variance = variance;
            this.isLowerBetter = isLowerBetter;
        }

        public WeightRelation(WeightRelationData data)
        {
            initialValue = data.initialValue;
        }
        
        public WeightRelation(JObject jObject)
        {
            initialValue = (float) jObject["initial_value"];
            variance = (float) jObject["variance"];
            isLowerBetter = (bool)jObject["is_lower_better"];
        }

        public float EvaluateInput(int level)
        {
            return initialValue + Math.Clamp(level / 10f, -1, 1) * variance;
        }

        public int GetPointCost(int level)
        {
            if (isLowerBetter)
            {
                level = -level;
            }
            if (level > 0)
            {
                int output = 0;
                int lowerBound = Math.Min(level, 6);
                output += lowerBound;
                int upperBound = level - lowerBound;
                output += upperBound * 2;
                return output;
            }

            return level;
        }

        public string GetColorFromEvaluated(float evaluated)
        {
            switch ((evaluated - initialValue)/variance * (isLowerBetter ? -1 : 1))
            {
                case < -0.75f:
                    return "#ff0000";
                case < -0.5f:
                    return "#ff8800";
                case < -0.25f:
                    return "#fff200";
                case < 0.25f:
                    return "#c3ff00";
                case < 0.5f:
                    return "#b3ff00";
                case < 0.75f:
                    return "#48ff00";
                default:
                    return "#001eff";
            }
            return "#ff0000";
        }

        public JObject toJSON()
        {
            JObject jObj = new JObject();
            jObj["initial_value"] = initialValue;
            jObj["variance"] = variance;
            jObj["is_lower_better"] = isLowerBetter;
            return jObj;
        }
    }