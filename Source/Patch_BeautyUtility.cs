using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;
using Blocky.Core;

namespace Blocky.Signs;

[HarmonyPatch(typeof(BeautyUtility), nameof(BeautyUtility.CellBeauty))]
public class Patch__BeautyUtility__FrameBeauty
{
    public static void Postfix(ref float __result, IntVec3 c, Map map)
    {
        if( ThingCache<Building_Frame>.Get(c, map) is Building_Frame f ){
            __result += f.InnerBeauty();
        }
    }
}
