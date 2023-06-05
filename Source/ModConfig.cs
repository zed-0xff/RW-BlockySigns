using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace Blocky.Signs;

public class Settings : Verse.ModSettings {
    public int maxZoom;
    public bool useCustomLabelDraw = true;
    public int fontSize;
    public bool respectLight = true;
    public bool fixFramesCategory = true;

    public bool parseExpressions = false;
    public int expInterval;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref maxZoom, "maxZoom", 30);
        Scribe_Values.Look(ref useCustomLabelDraw, "useCustomLabelDraw", true);
        Scribe_Values.Look(ref fontSize, "fontSize", 0);
        Scribe_Values.Look(ref respectLight, "respectLight", true);
        Scribe_Values.Look(ref fixFramesCategory, "fixFramesCategory", true);
        Scribe_Values.Look(ref parseExpressions, "parseExpressions", false);
        Scribe_Values.Look(ref expInterval, "expInterval", 100);
        base.ExposeData();
    }
}

public class SettingsTab : Blocky.Core.SettingsTabBase {
    public override string Title => "Signs";

    string tmpBuf;

    public override void Draw(Listing_Standard l){
        l.Label("Max zoom level: " + ModConfig.Settings.maxZoom);
        ModConfig.Settings.maxZoom = (int)l.Slider(ModConfig.Settings.maxZoom, 1, 40);

        l.Gap();
        l.CheckboxLabeled("Use custom label draw method", ref ModConfig.Settings.useCustomLabelDraw);

        l.Label("Font size: " + ModConfig.Settings.fontSize);
        ModConfig.Settings.fontSize = (int)l.Slider(ModConfig.Settings.fontSize, 0, 2);
        l.CheckboxLabeled("Respect light level", ref ModConfig.Settings.respectLight);

        l.Gap();
        bool b0 = ModConfig.Settings.fixFramesCategory;
        l.CheckboxLabeled("Move Frames back to \"Blocky\" Architect menu category after Deep Storage moves them to \"Storage\"",
                ref ModConfig.Settings.fixFramesCategory);
        if( b0 != ModConfig.Settings.fixFramesCategory && ModConfig.Settings.fixFramesCategory ){
            Init.fixFramesCategory();
        }

        l.Gap();
        l.CheckboxLabeled("Parse ${} expressions in labels", ref ModConfig.Settings.parseExpressions);
        l.TextFieldNumericLabeled("Ticks between calls", ref ModConfig.Settings.expInterval, ref tmpBuf);
    }

    public override void Write(){
        ModConfig.Settings.Write();
    }
}

public class ModConfig : Mod {
    public static Settings Settings { get; private set; }

    public ModConfig(ModContentPack content) : base(content) {
        Settings = GetSettings<Settings>();
    }
}
