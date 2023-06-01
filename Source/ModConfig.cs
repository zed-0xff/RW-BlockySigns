using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace Blocky.Signs;

public class Settings : Verse.ModSettings {
    public int maxZoom;
    public bool useCustomLabelDraw = true;
    public int fontSize;
    public float tintR = 0.74f;
    public float tintG = 0.74f;
    public float tintB = 0.74f;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref maxZoom, "maxZoom", 0);
        Scribe_Values.Look(ref useCustomLabelDraw, "useCustomLabelDraw", true);
        Scribe_Values.Look(ref fontSize, "fontSize", 0);
        Scribe_Values.Look(ref tintR, "tintR", 0.74f);
        Scribe_Values.Look(ref tintG, "tintG", 0.74f);
        Scribe_Values.Look(ref tintB, "tintB", 0.74f);
        base.ExposeData();
    }
}

public class SettingsTab : Blocky.Props.SettingsTabBase {
    public override string Title => "Signs";

    public override void Draw(Listing_Standard l){
        l.Label("Max zoom level: " + ModConfig.Settings.maxZoom);
        ModConfig.Settings.maxZoom = (int)l.Slider(ModConfig.Settings.maxZoom, 0, 5);

        l.Gap();
        l.CheckboxLabeled("Use custom label draw method", ref ModConfig.Settings.useCustomLabelDraw);

        l.Label("Font size: " + ModConfig.Settings.fontSize);
        ModConfig.Settings.fontSize = (int)l.Slider(ModConfig.Settings.fontSize, 0, 2);

        ModConfig.Settings.tintR = l.Slider(ModConfig.Settings.tintR, 0, 1.0f);
        ModConfig.Settings.tintG = l.Slider(ModConfig.Settings.tintG, 0, 1.0f);
        ModConfig.Settings.tintB = l.Slider(ModConfig.Settings.tintB, 0, 1.0f);
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
