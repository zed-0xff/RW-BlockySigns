using HarmonyLib;
using Verse;
using RimWorld;
using Blocky.Core;

namespace Blocky.Signs;

[HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
static class Patch_AllowFramesInWalls {
    static void Postfix(ref bool __result, IntVec3 c, Map map){
        if( __result ) return;

        if( Cache<Building_Frame>.Get(c, map) is Building_Frame f && !f.Occupied ){
            __result = true;
        }
    }
}
