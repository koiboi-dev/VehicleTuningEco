using System;
using Eco.Gameplay.Skills;
using Eco.Mods.MechanicExpansion.Skills;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.Mods.MechanicExpansion.Talents;

[Serialized]
[LocDisplayName("Bonus Skill Points")]
[LocDescription("Grants a 6 bonus skill points for use in the tuning table.")]
public class TunePointTalentGroup : TalentGroup
{
    public TunePointTalentGroup()
    {
        Talents = new Type[]
        {
            typeof(TunePointTalent),
        };
        OwningSkill = typeof(VehicleHandlingSkill);
        this.Level = 3;
    }
}

public class TunePointTalent : Talent
{
    public override bool Base { get { return false; } }
    public override Type TalentGroupType { get { return typeof(TunePointTalentGroup); } }
    public TunePointTalent()
    {
        Value = 6;
    }
}

[Serialized]
[LocDisplayName("Driving Efficiency")]
[LocDescription("Reduces calorie usage while driving by 25%")]
public class DrivingEfficiencyTalentGroup : TalentGroup
{
    public DrivingEfficiencyTalentGroup()
    {
        Talents = new Type[]
        {
            typeof(TunePointTalent),
        };
        OwningSkill = typeof(VehicleHandlingSkill);
        this.Level = 3;
    }
}

public class DrivingEfficiencyTalent : Talent
{
    public override bool Base { get { return false; } }
    public override Type TalentGroupType { get { return typeof(TunePointTalentGroup); } }
    public DrivingEfficiencyTalent()
    {
        Value = 0.75f;
    }
}