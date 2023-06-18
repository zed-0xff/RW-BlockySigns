using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;
using Blocky.Core;

namespace Blocky.Signs;

// Frame: allow rotating small corpses
// small corpses are also "downed pawns" ...
[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.BodyAngle))]
class Patch__PawnRenderer__InFrameAngle
{
    static void Postfix(ref float __result,  Pawn ___pawn)
    {
        if( ___pawn == null ) return;
        if( !___pawn.Dead ) return;

        var corpse = ___pawn.Corpse;
        if( corpse == null ) return;

        if( ThingCache<Building_Frame>.Get(corpse.Position, corpse.Map) is Building_Frame f ){
            __result = f.angle;
        }
    }
}
