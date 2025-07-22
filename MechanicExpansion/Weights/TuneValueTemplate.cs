using Eco.Shared.Localization;
using Eco.Shared.Logging;

namespace Eco.Mods.MechanicExpansion;

public struct TuneValueTemplate
{
    public float initialValue;
    public float lower;
    public float upper;

    public TuneValueTemplate(float initialValue, float lower, float upper)
    {
        this.initialValue = initialValue;
        this.lower = lower;
        this.upper = upper;
        if (this.initialValue < this.lower)
        {
            Log.WriteError(new LocString("WARNING: An initial value is less than lower, this can cause problems for the affected vehicle."));
        }
    }
    public TuneValueTemplate(float initialValue, float variance)
    {
        this.initialValue = initialValue;
        this.lower = variance;
        this.upper = variance;
        if (this.initialValue < this.lower)
        {
            Log.WriteError(new LocString("WARNING: An initial value is less than lower, this can cause problems for the affected vehicle."));
        }
    }
}