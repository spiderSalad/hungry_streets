public partial class gridCharSkills : StatGridDisplay
{
    protected override V5Stat[] statArray
    {
        get => Utils.Transpose1dGridArray<V5Stat>(Cfg.Skills, Cfg.Skills.Length / 3);
    }

    protected override System.Collections.Generic.Dictionary<string, V5Stat> statDict
    {
        get => Cfg.SkillsDict;
    }

    protected override int GetStatValue(V5Entity currentChar, V5Stat stat)
    {
        return currentChar.Block.GetSkill(stat.Id);
    }
}
