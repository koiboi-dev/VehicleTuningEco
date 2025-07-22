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
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Modules;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
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
    [Ecopedia("Work Stations", "Craft Tables", subPageName: "Tuning Bench")]
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
    [LocDescription("Tunes vehicles.")]
    [IconGroup("World Object Minimap")]
    [Weight(2000)]
    [MaxStackSize(2)]
    [Ecopedia("Work Stations", "Craft Tables", createAsSubPage: true)]
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
    
    [RequiresSkill(typeof(MechanicsSkill), 1)]
    [Ecopedia("Work Stations", "Craft Tables", subPageName: "Hydrotable Item")]
    public partial class TuningBenchRecipe : RecipeFamily
    {
        public TuningBenchRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "TuningBench",  //noloc
                displayName: Localizer.DoStr("Tuning Bench"),
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement("IronBar", 12, typeof(MechanicsSkill), typeof(MechanicsLavishResourcesTalent)),
                    new IngredientElement("CastIronStove", 1.5f, typeof(MechanicsSkill), typeof(MechanicsLavishResourcesTalent)),
                },
                items: new List<CraftingElement>
                {
                    new CraftingElement<TuningBenchItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 30; 
            
            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(600, typeof(MechanicsSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(TuningBenchRecipe), start: 8, skillType: typeof(MechanicsSkill), typeof(MechanicsFocusedSpeedTalent), typeof(MechanicsParallelSpeedTalent));
            
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Tuning Table"), recipeType: typeof(TuningBenchRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(CarpentryTableObject), recipeFamily: this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}