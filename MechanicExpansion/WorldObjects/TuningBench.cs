using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System;
using System.Collections.Generic;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Modules;
using Eco.Gameplay.Occupancy;
using Eco.Shared.Math;

namespace Eco.Mods.MechanicExpansion
{
    [Serialized]
    [Tag("Usable")]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(TuningComponent))]
    public partial class TuningBenchObject : WorldObject, IRepresentsItem, IOperatingWorldObjectComponent
    {
        public virtual Type RepresentedItemType => typeof(TuningBenchItem);

        static TuningBenchObject()
        {
            var BlockOccupancyList = new List<BlockOccupancy>
            {
                new BlockOccupancy(new Vector3i(0, 0, 0))
            };
            WorldObject.AddOccupancy<TuningBenchObject>(BlockOccupancyList);
        }
        
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            base.Initialize();
            GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Crafting"));
            GetComponent<LinkComponent>().Initialize(12f);
            GetComponent<TuningComponent>().Initialize();
            this.ModsPostInitialize();
        }
        
        /// <summary>Hook for mods to customize WorldObject before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize WorldObject after initialization.</summary>
        partial void ModsPostInitialize();
    }
    
    [Serialized]
    [LocDisplayName("Tuning Bench")]
    [LocDescription("Tunes shit innit.")]
    [IconGroup("World Object Minimap")]
    /*[AllowPluginModules(ItemTypes = new Type[]
    {
        typeof(SteamTruckItem),
        typeof(TruckItem)
    })]*/
    public partial class TuningBenchItem : WorldObjectItem<TuningBenchObject>, IPersistentData
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext( 0  | DirectionAxisFlags.Down , WorldObject.GetOccupancyInfo(this.WorldObjectType));

        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
    }

}