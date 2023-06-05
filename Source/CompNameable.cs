using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

[StaticConstructorOnStartup]
public class CompNameable : ThingComp {
    private string name;

    public string Name {
        get { return name; }
        set { name = value; cachedText = null; }
    }

    private static readonly Texture2D texAtlas = ContentFinder<Texture2D>.Get("Blocky/Signs/Sign_Atlas");

    public Color color = GenMapUI.DefaultThingLabelColor;

    static Color lastSelectedColor = ColorLibrary.Black;

	CompProperties_Nameable Props => (CompProperties_Nameable)props;

    public override void PostSpawnSetup(bool respawningAfterLoad) {
        if( !respawningAfterLoad ){
            name = parent.thingIDNumber.ToString();
            color = lastSelectedColor;
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra() {
        yield return new Command_Action {
            defaultLabel = "Rename".Translate(),
            icon = TexButton.Rename,
            hotKey = KeyBindingDefOf.Misc3,
            action = delegate{
                Find.WindowStack.Add(new Dialog_Rename(this));
            }
        };
        yield return new Command_ColorIcon {
            defaultLabel = "GlowerChangeColor".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/ChangeColor"),
            color = color,
            hotKey = KeyBindingDefOf.Misc2,
            action = delegate
            {
                Find.WindowStack.Add( new Blocky.Core.Dialog_ColorPicker(color, delegate(Color newColor)
                    {
                    color = newColor;
                    lastSelectedColor = newColor;
                    }));
            }
        };
    }

    Color shadeColor(){
        if( ModConfig.Settings.respectLight ){
            float glow = parent.Map.glowGrid.GameGlowAt(parent.Position);
            return new Color(0.333f, 0.243f, 0.18f) * (1-glow);
        } else {
            return Color.clear;
        }
    }

    string parseExpressions(string text){
        return Regex.Replace(text, @"\$\{([a-zA-Z0-9_.:()]+)\}", delegate(Match match) {
                string fqmn = match.Groups[1].Captures[0].Value;
                return ExpCompiler.Compile(fqmn, this).Invoke(this).ToString();
        });
    }

    string cachedText = null;
    bool wasError = false;

    public override void DrawGUIOverlay(){
        if( name == "" || name == null ) return;

        string text = name;
        if( ModConfig.Settings.parseExpressions && text.Contains("$") ){
            if( cachedText == null || parent.IsHashIntervalTick(wasError ? 1000 : ModConfig.Settings.expInterval) ){
                wasError = false;
                try {
                    cachedText = parseExpressions(text);
                } catch( Exception ex ){
                    wasError = true;
                    Log.Error(text + ": " + ex);
                    cachedText = ex.Message.Colorize(Color.red);
                }
            }
            text = cachedText;
        }

        if( Find.CameraDriver.CellSizePixels >= ModConfig.Settings.maxZoom ){
            if( ModConfig.Settings.useCustomLabelDraw ){
                Color shade = shadeColor();

                Utils.DrawThingLabelAtlas(GenMapUI.LabelDrawPosFor(parent, Props.labelShift), text, (color-shade).ToOpaque(),
                        bgColor: (parent.DrawColor - shade).ToOpaque(),
                        atlasTex: texAtlas,
                        minWidth: 30f,
                        font: (GameFont)ModConfig.Settings.fontSize );
            } else {
                GenMapUI.DrawThingLabel(GenMapUI.LabelDrawPosFor(parent, Props.labelShift - 0.05f), text, color);
            }
        }
    }

    public override string CompInspectStringExtra() {
        return name;
    }

    public override void PostExposeData() {
        Scribe_Values.Look(ref name, "name");
        Scribe_Values.Look(ref color, "color", GenMapUI.DefaultThingLabelColor);
    }
}
