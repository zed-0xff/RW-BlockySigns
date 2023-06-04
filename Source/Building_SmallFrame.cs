using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

public class Building_SmallFrame : Building_Frame {
    protected override void drawThing(Thing t){
        if( angle < -360 )
            angle += 360;

        var drawPos = Position.ToVector3ShiftedWithAltitude(def.altitudeLayer+1);

        const float offset = 0.25f;

        if( Rotation == Rot4.North ){
            drawPos.z += offset;
        } else if( Rotation == Rot4.South ){
            drawPos.z -= offset;
        } else if( Rotation == Rot4.East ){
            drawPos.x += offset;
        } else if( Rotation == Rot4.West ){
            drawPos.x -= offset;
        }

        Graphic g = t.Graphic;

        if( g is Graphic_RandomRotated ){
            // apparel, weapon
            g = (Graphic)HarmonyLib.AccessTools.Field(typeof(Graphic_RandomRotated), "subGraphic").GetValue(g);
        } else if( t is Corpse ){
            // XXX corpses are drawn non-zoomed
            t.DrawAt(drawPos);
            return;
        }

        DrawWorker_Zoomed( g, drawPos, Rot4.North, t.def, t, angle);
    }

    const float zoomFactor = 0.75f;

    // also noshadow
    void DrawWorker_Zoomed(Graphic g, Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation) {
        Mesh mesh = g.MeshAt(rot);
        Quaternion quat = Quaternion.identity;
        if (extraRotation != 0f)
        {
            quat *= Quaternion.Euler(Vector3.up * extraRotation);
        }
        loc += g.DrawOffset(rot);
        Material mat = g.MatAt(rot, thing);
        var matrix = Matrix4x4.TRS(loc, quat, new Vector3(zoomFactor, 1, zoomFactor));
        Graphics.DrawMesh(mesh, matrix, mat, 0);
    }
}
