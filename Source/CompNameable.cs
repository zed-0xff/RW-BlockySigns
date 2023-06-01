using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

[StaticConstructorOnStartup]
public class CompNameable : ThingComp {
    private string name;

    public string Name {
        get { return name; }
        set { name = value; }
    }

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
                Find.WindowStack.Add( new Blocky.Props.Dialog_ColorPicker(color, delegate(Color newColor)
                    {
                    color = newColor;
                    lastSelectedColor = newColor;
                    }));
            }
        };
    }

    private Graphic cachedLeftGraphic;
    private Graphic LeftGraphic {
        get {
            if (cachedLeftGraphic == null ) {
                cachedLeftGraphic = GraphicDatabase.Get<Graphic_Single>(Props.leftTexPath,
                        ShaderDatabase.Cutout,
                        new Vector2(texWidth, 1),
                        parent.DrawColor
                        );
            }
            return cachedLeftGraphic;
        }
    }

    private Graphic cachedCenterGraphic;
    private Graphic CenterGraphic {
        get {
            if (cachedCenterGraphic == null ) {
                cachedCenterGraphic = GraphicDatabase.Get<Graphic_Single>(Props.centerTexPath,
                        ShaderDatabase.Cutout,
                        new Vector2(texWidth, 1),
                        parent.DrawColor
                        );
            }
            return cachedCenterGraphic;
        }
    }

    private Graphic cachedRightGraphic;
    private Graphic RightGraphic {
        get {
            if (cachedRightGraphic == null ) {
                cachedRightGraphic = GraphicDatabase.Get<Graphic_Single>(Props.rightTexPath,
                        ShaderDatabase.Cutout,
                        new Vector2(texWidth, 1),
                        parent.DrawColor
                        );
            }
            return cachedRightGraphic;
        }
    }

    const float texWidth = 0.5f;
    const float defaultCapacity = 0.75f; // width capacity if non-custom sign

    public override void PostDraw(){
		base.PostDraw();
        if( !ModConfig.Settings.useCustomLabelDraw ) return;
        if ((int)Find.CameraDriver.CurrentZoom > ModConfig.Settings.maxZoom ) return;

        var textSize = Utils.CalcTextSizeForFont(name, (GameFont)ModConfig.Settings.fontSize);
        float nCells = textSize.x / Find.CameraDriver.CellSizePixels;

        // 0.05f to prevent flickering
        if( nCells < defaultCapacity - 0.05f ) return;

        int nDraws = (int)Mathf.Ceil(nCells / texWidth);

        float x = -nDraws * texWidth / 2 + texWidth/2;
        LeftGraphic.Draw(parent.DrawPos + new Vector3(x-texWidth, 1, 0), parent.Rotation, parent);
        for( int i=0; i<nDraws; i++ ){
            CenterGraphic.Draw(parent.DrawPos + new Vector3(x, 1, 0), parent.Rotation, parent);
            x += texWidth;
        }
        RightGraphic.Draw(parent.DrawPos + new Vector3(x, 1, 0), parent.Rotation, parent);
    }

    public override void DrawGUIOverlay(){
        if( name == "" || name == null ) return;

        if ((int)Find.CameraDriver.CurrentZoom <= ModConfig.Settings.maxZoom ){
            if( ModConfig.Settings.useCustomLabelDraw ){
                Utils.DrawCustomThingLabel(GenMapUI.LabelDrawPosFor(parent, Props.labelShift), name, color,
                        debug: Find.Selector.SingleSelectedObject == parent,
                        bgTex: null,
                        bgColor: parent.DrawColor,
                        font: (GameFont)ModConfig.Settings.fontSize );
            } else {
                GenMapUI.DrawThingLabel(GenMapUI.LabelDrawPosFor(parent, Props.labelShift), name, color);
            }
        }
    }

    public override void PostExposeData() {
        Scribe_Values.Look(ref name, "name");
        Scribe_Values.Look(ref color, "color", GenMapUI.DefaultThingLabelColor);
    }
}
