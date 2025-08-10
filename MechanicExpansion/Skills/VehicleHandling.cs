using Eco.Mods.TechTree;

namespace Eco.Mods.MechanicExpansion.Skills;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Core.Utils.AtomicAction;
using Eco.Gameplay.Blocks;
using Eco.Gameplay.Components;
using Eco.Gameplay.DynamicValues;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Gameplay.Property;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Systems;
using Eco.Gameplay.Systems.TextLinks;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Services;
using Eco.Shared.Utils;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Core.Controller;
using Eco.Gameplay.Items.Recipes;

[Serialized]
[LocDisplayName("Vehicle Handling")]
[LocDescription(
    "Vehicles power our modern world and with experience and knowledge its possible to improve them for the betterment of all.")]
[Ecopedia("Professions", "Scientist", createAsSubPage: true)]
[RequiresSkill(typeof(ScientistSkill), 0), Tag("Scientist Specialty"), Tier(2)]
[Tag("Specialty")]
[Tag("Teachable")]
public partial class VehicleHandlingSkill : Skill
{
    public override void OnLevelUp(User user)
    {
        user.Skillset.AddExperience(typeof(SelfImprovementSkill), 20,
            Localizer.DoStr("for leveling up another specialization."));
    }


    public static MultiplicativeStrategy MultiplicativeStrategy =
        new MultiplicativeStrategy(new float[]
        {
            1,
            1 - 0.2f,
            1 - 0.25f,
            1 - 0.3f,
            1 - 0.35f,
            1 - 0.4f,
            1 - 0.45f,
            1 - 0.5f,
        });

    public override MultiplicativeStrategy MultiStrategy => MultiplicativeStrategy;

    public static AdditiveStrategy AdditiveStrategy =
        new AdditiveStrategy(new float[]
        {
            0,
            0.5f,
            0.55f,
            0.6f,
            0.65f,
            0.7f,
            0.75f,
            0.8f,
        });

    public override AdditiveStrategy AddStrategy => AdditiveStrategy;

    public override int MaxLevel
    {
        get { return 7; }
    }

    public override int Tier
    {
        get { return 2; }
    }
}

[Serialized]
[Weight(1000)]
[LocDisplayName("Vehicle Handling Skill Book")]
[Ecopedia("Items", "Skill Books", createAsSubPage: true)]
public partial class VehicleHandlingSkillBook : SkillBook<VehicleHandlingSkill, VehicleHandlingSkillScroll>
{
}

[Serialized]
[Weight(100)]
[LocDisplayName("Vehicle Handling Skill Scroll")]
public partial class VehicleHandlingSkillScroll : SkillScroll<VehicleHandlingSkill, VehicleHandlingSkillBook>
{
}

/// <summary>
/// <para>Server side recipe definition for "OilDrilling".</para>
/// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
/// </summary>
/// <remarks>
/// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization. 
/// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
/// </remarks>
[RequiresSkill(typeof(MechanicsSkill), 1)]
[Ecopedia("Professions", "Scientist", subPageName: "Vehicle Handling Skill Book Item")]
public partial class VehicleHandlingSkillBookRecipe : RecipeFamily
{
    public VehicleHandlingSkillBookRecipe()
    {
        var recipe = new Recipe();
        recipe.Init(
            name: "VehicleHandling", //noloc
            displayName: Localizer.DoStr("Vehicle Handling Skill Book"),

            // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
            // type of the item, the amount of the item, the skill required, and the talent used.
            ingredients: new List<IngredientElement>
            {
                new IngredientElement(typeof(EngineeringResearchPaperAdvancedItem), 6, typeof(MechanicsSkill)),
                new IngredientElement("Basic Research", 16, typeof(MechanicsSkill)), //noloc
            },

            // Define our recipe output items.
            // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
            // to create.
            items: new List<CraftingElement>
            {
                new CraftingElement<VehicleHandlingSkillBook>()
            });
        this.Recipes = new List<Recipe> { recipe };

        // Defines the amount of labor required and the required skill to add labor
        this.LaborInCalories = CreateLaborInCaloriesValue(6000, typeof(MechanicsSkill));

        // Defines our crafting time for the recipe
        this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(VehicleHandlingSkillBookRecipe), start: 30,
            skillType: typeof(MechanicsSkill));

        // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Oil Drilling Skill Book"
        this.ModsPreInitialize();
        this.Initialize(displayText: Localizer.DoStr("Vehicle Handling Skill Book"),
            recipeType: typeof(VehicleHandlingSkillBookRecipe));
        this.ModsPostInitialize();

        // Register our RecipeFamily instance with the crafting system so it can be crafted.
        CraftingComponent.AddRecipe(tableType: typeof(ResearchTableObject), recipeFamily: this);
    }

    /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
    partial void ModsPreInitialize();

    /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
    partial void ModsPostInitialize();
}