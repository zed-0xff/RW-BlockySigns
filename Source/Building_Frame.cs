using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

[StaticConstructorOnStartup]
public class Building_Frame : Building_Storage {
    static readonly Texture2D RotateIcon = ContentFinder<Texture2D>.Get("Blocky/UI/Rotate", true);

    const float itemWealthMultiplier = 0.25f; // jsut hardcode for now

    public float angle = 0;

    public override void Draw() {
        base.Draw();

        foreach (Thing t in slotGroup.HeldThings) {
            drawThing(t);
            break;
        }
    }

    void drawThing(Thing t){
        if( angle < -360 )
            angle += 360;

        var drawPos = Position.ToVector3ShiftedWithAltitude(def.altitudeLayer+1);
        Graphic g = t.Graphic;

        if( g is Graphic_RandomRotated ){
            // apparel, weapon
            g = (Graphic)HarmonyLib.AccessTools.Field(typeof(Graphic_RandomRotated), "subGraphic").GetValue(g);
        } else if( t is Corpse ){
            t.DrawAt(drawPos);
            return;
        }

        g.DrawWorker( drawPos, Rot4.North, t.def, t, angle);
    }

    public float InnerBeauty(){
        float r = 0;
        foreach (Thing thing in slotGroup.HeldThings) {
            r += Mathf.Max(thing.MarketValue, thing.GetStatValue(StatDefOf.Beauty));
            break;
        }
        return r*itemWealthMultiplier;
    }

    public override IEnumerable<Gizmo> GetGizmos() {
        foreach (Gizmo gizmo in base.GetGizmos()) {
            yield return gizmo;
        }
        yield return new Command_Action() {
            action = delegate{ angle -= 45; },
                   hotKey = KeyBindingDefOf.Designator_RotateLeft, // Q
                   defaultLabel = "Rotate",
                   icon = RotateIcon
        };
    }

    public override string GetInspectString() {
        string text = base.GetInspectString();
        text += "\n" + "Beauty".Translate() + ": " + this.GetStatValue(StatDefOf.Beauty);
        if( slotGroup.HeldThings.Any() ){
            text += " + " + InnerBeauty();
        }
        return text;
    }

    public override void ExposeData() {
        base.ExposeData();
        Scribe_Values.Look(ref angle, "angle");
    }
}
