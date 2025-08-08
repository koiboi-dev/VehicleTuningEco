using System;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.Shared.Utils;
using Newtonsoft.Json.Linq;

namespace Eco.Mods.MechanicExpansion;

public struct TuneValues
    {
        public readonly float initialValue;
        public readonly float lower;
        public readonly float upper;
        public readonly bool isLowerBetter;
        public readonly bool Neutral;

        public TuneValues(float initialValue, float lower, float upper)
        {
            this.initialValue = initialValue;
            this.lower = lower;
            this.upper = upper;
        }

        public TuneValues(TuneValueTemplate data, bool isLowerBetter = false, bool neutral = false)
        {
            initialValue = data.initialValue;
            lower = data.lower;
            upper = data.upper;
            this.isLowerBetter = isLowerBetter;
            Neutral = neutral;
        }
        
        public TuneValues(JObject jObject, bool isLowerBetter = false, bool neutral = false)
        {
            initialValue = (float) jObject["initial_value"];
            lower = (float) jObject["lower"];
            upper = (float) jObject["upper"];
            this.isLowerBetter = isLowerBetter;
            Neutral = neutral;
        }

        public float EvaluateInput(float level)
        {
            if (isLowerBetter)
            {
                level *= -1;
            }
            float mult = Math.Clamp(level / 10f, -1, 1);
            return initialValue + mult * (mult > 0 ? upper : lower);
        }

        public int GetPointCost(int level)
        {
            if (Neutral)
            {
                int output = 0;
                int lowerBound = Math.Min(Math.Abs(level), 6);
                output += lowerBound;
                int upperBound = Math.Abs(level) - lowerBound;
                output += upperBound * 2;
                return output;
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
            float diff = evaluated - initialValue;
            if (Neutral)
            {
                diff = Math.Abs(diff);
            }
            switch (diff / (diff > 0 ? upper : lower) * (isLowerBetter ? -1 : 1))
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
            jObj["lower"] = lower;
            jObj["upper"] = upper;
            return jObj;
        }
    }