using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace Blocky.Signs;

[HarmonyPatch(typeof(BeautyUtility), nameof(BeautyUtility.CellBeauty))]
public class Patch__BeautyUtility__FrameBeauty
{
    public static void Postfix(ref float __result, IntVec3 c, Map map)
    {
        if( c == null || map == null ) return;

        foreach( Thing container in map.thingGrid.ThingsListAt(c) ){
            if( container is Building_Frame b ){
                __result += b.InnerBeauty();
            }
        }
    }
}
