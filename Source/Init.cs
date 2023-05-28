using HarmonyLib;
using RimWorld;
using Verse;

namespace Blocky.Signs;

[StaticConstructorOnStartup]
public class Init
{
    static Init()
    {
        Harmony harmony = new Harmony("Blocky.Signs");
        harmony.PatchAll();
    }
}
