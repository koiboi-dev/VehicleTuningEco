using Eco.Shared.Localization;
using Eco.Shared.Logging;

namespace Eco.Mods.MechanicExpansion;

public struct WeightRelationData
{
    public float initialValue;
    public float variance;

    public WeightRelationData(float initialValue, float variance)
    {
        this.initialValue = initialValue;
        this.variance = variance;
        if (this.initialValue < this.variance)
        {
            Log.WriteError(new LocString("WARNING: And initial value is greater than the variance, this can cause problems for the affected vehicle."));
        }
    }
}