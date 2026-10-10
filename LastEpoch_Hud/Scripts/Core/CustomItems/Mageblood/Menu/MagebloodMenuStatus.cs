namespace LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

/// <summary>What the Mageblood card shows.</summary>
public readonly record struct MagebloodMenuStatus(
    bool Worn,
    int ActiveFlasks,
    MagebloodFileWarning Warning,
    bool Editable
)
{
    public static MagebloodMenuStatus From(
        bool readable,
        int problemCount,
        bool worn,
        int activeFlasks
    )
    {
        int active = worn && activeFlasks > 0 ? activeFlasks : 0;
        return new MagebloodMenuStatus(worn, active, WarningFor(readable, problemCount), readable);
    }

    private static MagebloodFileWarning WarningFor(bool readable, int problemCount)
    {
        if (!readable)
        {
            return MagebloodFileWarning.FileError;
        }

        return problemCount > 0 ? MagebloodFileWarning.SomeSkipped : MagebloodFileWarning.None;
    }
}
