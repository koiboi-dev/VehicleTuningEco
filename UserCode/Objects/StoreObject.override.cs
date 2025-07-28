namespace Eco.Mods.TechTree
{
    using Eco.Core.Controller;
    using Eco.Core.Utils;
    using Eco.Gameplay.Aliases;
    using Eco.Gameplay.Property;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.GameActions;
    using Eco.Gameplay.Objects;
    using Eco.Shared.Networking;
    using Eco.Gameplay.Components.Store;
        
    //Attributes must remain in this order: (SharedLinkComponent, StoreComponent) to avoid double Update calls for notification messages
    [RequireComponent(typeof(SharedLinkComponent))]
    [RequireComponent(typeof(StoreComponent))]
    public partial class StoreObject : WorldObject, INullCurrencyAllowed
    {
        protected override void OnCreatePostInitialize()
        {
            base.OnCreatePostInitialize();
            this.GetComponent<PropertyAuthComponent>().SetPublic(); // so everyone can acess store by default
        }
    }
}
