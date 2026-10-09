using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;

/// <summary>One game-ready flask: name, icon name and resolved stat rows.</summary>
public sealed class MagebloodFlask
{
    public MagebloodFlask(string name, string iconName, IReadOnlyList<MagebloodBuffStat> stats)
    {
        Name = name;
        IconName = iconName;
        Stats = stats;
    }

    public string Name { get; }
    public string IconName { get; }
    public IReadOnlyList<MagebloodBuffStat> Stats { get; }
}
