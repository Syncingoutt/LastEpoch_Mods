using Il2Cpp;
using LastEpoch_Hud.Scripts.Core.CustomItems;
using LastEpoch_Hud.Scripts.Core.CustomItems.Affixes;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items.CustomItems.Mageblood;

[RegisterTypeInIl2Cpp]
public class Items_Mageblood : MonoBehaviour
{
    private static readonly CustomUniqueRegistrar _registrar = new(CreateDefinition());

    public Items_Mageblood(System.IntPtr ptr)
        : base(ptr) { }

    private void Awake()
    {
        MagebloodConfigLoader.Load();
    }

    private void Update()
    {
        _registrar.Update();
        MagebloodConfigLoader.ReloadIfChanged(Time.unscaledTime);
    }

    private static CustomUniqueDefinition CreateDefinition()
    {
        return new CustomUniqueDefinition
        {
            Spec = CustomUniqueSpecs.Mageblood,
            SubtypeNameKey = CustomItemLocaleKeys.MagebloodSubtype,
            UniqueNameKey = CustomItemLocaleKeys.MagebloodName,
            LoreKey = CustomItemLocaleKeys.MagebloodLore,
            Description = MagebloodDescription.Text,
            Flags = () => CustomUniqueFlags.NoSettings,
            Implicits = CustomUniqueAffixes.MagebloodImplicits,
            Mods = CustomUniqueAffixes.MagebloodMods,
            TooltipEntries = () => CustomUniqueAffixes.MagebloodTooltip,
        };
    }
}
