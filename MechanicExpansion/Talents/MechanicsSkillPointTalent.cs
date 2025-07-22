using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.Mods.MechanicExpansion.Talents;

[Serialized]
[LocDisplayName("Bonus Skill Points")]
[LocDescription("Grants a 6 bonus skill points for use in the tuning table.")]
public class MechanicsSkillPointTalentGroup : TalentGroup
{
    public MechanicsSkillPointTalentGroup()
    {
        Talents = new Type[]
        {
            typeof(MechanicsSkillPointTalent),
        };
        OwningSkill = typeof(MechanicsSkill);
        this.Level = 4;
    }
}

public class MechanicsSkillPointTalent : Talent
{
    public override bool Base { get { return false; } }
    public override Type TalentGroupType { get { return typeof(MechanicsParallelProcessingTalentGroup); } }
    public MechanicsSkillPointTalent()
    {
        Value = 6;
    }
}