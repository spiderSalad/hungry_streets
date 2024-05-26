public partial class gridCharAttrs : StatGridDisplay
{
    protected override V5Stat[] statArray
    {
        get => Utils.Transpose1dGridArray<V5Stat>(Cfg.Attrs, Cfg.Attrs.Length / 3);
    }

    protected override System.Collections.Generic.Dictionary<string, V5Stat> statDict
    {
        get => Cfg.AttrsDict; 
    }

    protected override int GetStatValue(V5Entity currentChar, V5Stat stat)
    {
        return currentChar.Block.GetAttr(stat.Id);
    }
}