using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;
using Blocky.Core;

namespace Blocky.Signs;

public class Patch_GenSpawn {
    static bool g_respawningAfterLoad;

    [HarmonyPatch(typeof(GenSpawn), nameof(GenSpawn.SpawnBuildingAsPossible))]
    public class NoWipeWallUnderFrameWithItem0
    {
        public static void Prefix(bool respawningAfterLoad) {
            g_respawningAfterLoad = respawningAfterLoad;
        }

        public static void Postfix(bool respawningAfterLoad) {
            g_respawningAfterLoad = false;
        }
    }

    [HarmonyPatch(typeof(GenSpawn), nameof(GenSpawn.Refund))]
    public class NoWipeWallUnderFrameWithItem1
    {
        public static bool Prefix(Thing thing, Map map)
        {
            if( !g_respawningAfterLoad ) return true;
            bool hasFrame = ( ThingCache<Building_Frame>.Get(thing.Position, map) != null );

//            if( Blocky.Core.Settings.Debug ){
//                Log.Warning("[d] Refund: " + thing + " hasFrame: " + hasFrame);
//            }

            if( !hasFrame ) return true;

            // passing non-existing wipemode to actually ignore all things in the cell
            GenSpawn.Spawn(thing, thing.Position, map, thing.Rotation, (WipeMode)111, respawningAfterLoad: true);
            return false;
        }
    }
}
