using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace Blocky.Signs;

[HarmonyPatch(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToCellStorageJob))]
public class Patch__HaulAIUtility__FrameAcceptSingleItem
{
    public static void Postfix(ref Job __result,  IntVec3 storeCell, Pawn p)
    {
        if( __result == null ) return;

        foreach( Thing container in p.Map.thingGrid.ThingsListAt(storeCell) ){
            if( container is Building_Frame b ){
                if( b.slotGroup.HeldThings.Any() ){
                    __result = null;
                } else {
                    __result.count = 1;
                }
            }
        }
    }
}
