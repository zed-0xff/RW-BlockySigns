using HarmonyLib;
using RimWorld;
using Verse;

namespace Blocky.Signs;

static class Patch_Building_Storage {
    [HarmonyPatch(typeof(Building_Storage), nameof(Building_Storage.Accepts))]
    static class FrameAcceptSingleItem {
        static void Postfix(ref bool __result, Building_Storage __instance, Thing t){
            if( __result == false ) return;
            if( !(__instance is Building_Frame f) ) return;

            __result = !f.Occupied ||
                t.Position == __instance.Position; // already in
        }
    }
}
