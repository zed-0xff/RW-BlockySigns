using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;

namespace Blocky.Signs;

// account room with Frames as a Museum
public static class Patch__BillDoors__Museum
{
    public static void HandlePatch(Harmony harmony) {
        Type t = AccessTools.TypeByName("BillDoorsLootsAndShelves.RoomRoleWorker_Museum");
        if( t == null ) return;

        harmony.Patch(AccessTools.Method(t, "GetScore"),
                postfix: new HarmonyMethod(
                    MethodBase.GetCurrentMethod().DeclaringType.GetMethod("Postfix", BindingFlags.Static | BindingFlags.Public)
                    )
                );
    }

    public static void Postfix(ref float __result, Room room){
        if( room == null ) return;

        int num = 0;
        List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
        for (int i = 0; i < containedAndAdjacentThings.Count; i++) {
            if (containedAndAdjacentThings[i] is Building_Frame) {
                num++;
            }
        }
        __result += 50 * num;
    }
}
