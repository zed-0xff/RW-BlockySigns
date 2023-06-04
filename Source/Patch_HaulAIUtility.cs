using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Blocky.Core;

namespace Blocky.Signs;

[HarmonyPatch(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToCellStorageJob))]
public class Patch__HaulAIUtility__FrameAcceptSingleItem
{
    public static void Postfix(ref Job __result,  IntVec3 storeCell, Pawn p)
    {
        if( __result == null ) return;

        if( Cache<Building_Frame>.Get(storeCell, p.Map) is Building_Frame f ){
            if( f.Occupied ){
                __result = null;
            } else {
                __result.count = 1;
            }
        }
    }
}
