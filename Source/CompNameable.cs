using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

[StaticConstructorOnStartup]
public class CompNameable : ThingComp {
    public string name;
    public Color color = GenMapUI.DefaultThingLabelColor;

	CompProperties_Nameable Props => (CompProperties_Nameable)props;

    private static List<Color> cachedColors = new List<Color>();

    public override void PostSpawnSetup(bool respawningAfterLoad) {
        if( !respawningAfterLoad ){
            name = parent.thingIDNumber.ToString();
        }
        if( cachedColors.Count == 0 ){
            cachedColors = DefDatabase<ColorDef>.AllDefsListForReading.Select((ColorDef c) => c.color).ToList();
            cachedColors.SortByColor((Color c) => c);
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra() {
        yield return new Command_Action {
            action = delegate{
                Find.WindowStack.Add(new Dialog_Rename(this));
            },
            defaultLabel = "Rename".Translate(),
            icon = TexButton.Rename
        };
        yield return new Command_ColorIcon {
            defaultLabel = "GlowerChangeColor".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/ChangeColor"),
            color = color,
            action = delegate
            {
                Find.WindowStack.Add(new Dialog_ChooseColor("GlowerChangeColor".Translate(), color, cachedColors, delegate(Color newColor)
                    {
                    color = newColor;
                    }));
            }
        };
    }

    public override void DrawGUIOverlay(){
        if( name == "" || name == null ) return;

        if ((int)Find.CameraDriver.CurrentZoom <= ModConfig.Settings.maxZoom ){
            GenMapUI.DrawThingLabel(GenMapUI.LabelDrawPosFor(parent, Props.labelShift), name, color);
        }
    }

    public override void PostExposeData() {
        Scribe_Values.Look(ref name, "name");
        Scribe_Values.Look(ref color, "color", GenMapUI.DefaultThingLabelColor);
    }
}
