using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace Blocky.Signs;

public class Settings : Verse.ModSettings {
    public int maxZoom;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref maxZoom, "maxZoom", 0);
        base.ExposeData();
    }
}

public class SettingsTab : Blocky.Props.SettingsTabBase {
    public override string Title => "Signs";

    public override void Draw(Listing_Standard l){
        l.Label("Max zoom level: " + ModConfig.Settings.maxZoom);
        ModConfig.Settings.maxZoom = (int)l.Slider(ModConfig.Settings.maxZoom, 0, 5);
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
