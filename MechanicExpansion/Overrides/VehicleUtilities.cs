using Eco.Gameplay.Items;

namespace Eco.Mods.TechTree
{
    public class CustomVehicleUtilities
    {
        static CustomVehicleUtilities()
        {
            var tractorMap = new StackLimitTypeRestriction();
            tractorMap.AddListRestriction(ItemUtils.GetItemsByTag("Seeds", "Crop"), 500);
            VehicleUtilities.AdvancedVehicleStackSizeMap.Add(typeof(SteamTractorObject), tractorMap);
        }
    }
}