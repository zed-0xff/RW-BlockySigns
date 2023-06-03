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

    bool canDrawCustomTex(){
        //return Find.CameraDriver.CurrentZoom <= CameraZoomRange.Close && ModConfig.Settings.fontSize <= 1;
        return true;
    }

    int prevStateHash;
    Vector2 cachedTextSize;

    // prevent flickering
    float calcNCells(){
        int stateHash = Gen.HashCombine(ModConfig.Settings.fontSize, name);
        if( stateHash != prevStateHash ){
            prevStateHash = stateHash;
            cachedTextSize = Utils.CalcTextSizeForFont(name, (GameFont)ModConfig.Settings.fontSize);
        }
        return cachedTextSize.x / Find.CameraDriver.CellSizePixels;
    }

//    public override void PostDraw(){
//		base.PostDraw();
//        if( !ModConfig.Settings.useCustomLabelDraw ) return;
//        if (Find.CameraDriver.CellSizePixels < ModConfig.Settings.maxZoom ) return;
//        if (!canDrawCustomTex()) return; // no custom texture draw, just text on bg
//
//        float nCells = calcNCells();
//
//        // 0.05f to prevent flickering
//        if( nCells < defaultCapacity - 0.05f ) return;
//
//        int nDraws = (int)Mathf.Ceil(nCells / texWidth);
//
//        float x = -nDraws * texWidth / 2 + texWidth/2;
////        LeftGraphic.Draw(parent.DrawPos + new Vector3(x-texWidth, 1, 0), parent.Rotation, parent);
////        for( int i=0; i<nDraws; i++ ){
////            CenterGraphic.Draw(parent.DrawPos + new Vector3(x, 1, 0), parent.Rotation, parent);
////            x += texWidth;
////        }
////        RightGraphic.Draw(parent.DrawPos + new Vector3(x, 1, 0), parent.Rotation, parent);
//
////        DrawStencilCell(
////                parent.DrawPos + new Vector3(0, 1, 0),
////                CenterGraphic.MatAt(parent.Rotation, parent),
////                nCells + 0.05f);
//    }

//    public static void DrawStencilCell(Vector3 c, Material material, float width = 1f, float height = 1f){
//        Matrix4x4 matrix = default(Matrix4x4);
//        matrix.SetTRS(c, Quaternion.identity, new Vector3(width, 1f, height));
//        Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
//    }

    Color shadeColor(){
        if( ModConfig.Settings.respectLight ){
            float glow = parent.Map.glowGrid.GameGlowAt(parent.Position);
            return new Color(0.333f, 0.243f, 0.18f) * (1-glow);
        } else {
            return Color.clear;
        }
    }

    public override void DrawGUIOverlay(){
        if( name == "" || name == null ) return;

        if (Find.CameraDriver.CellSizePixels >= ModConfig.Settings.maxZoom ){
            if( ModConfig.Settings.useCustomLabelDraw ){
                Color shade = shadeColor();

                Utils.DrawThingLabelAtlas(GenMapUI.LabelDrawPosFor(parent, Props.labelShift), name, color,
                        bgColor: (parent.DrawColor - shade).ToOpaque(),
                        atlasTex: texAtlas,
                        minWidth: 30f,
                        font: (GameFont)ModConfig.Settings.fontSize );
            } else {
                GenMapUI.DrawThingLabel(GenMapUI.LabelDrawPosFor(parent, Props.labelShift - 0.05f), name, color);
            }
        }
    }

    public override void PostExposeData() {
        Scribe_Values.Look(ref name, "name");
        Scribe_Values.Look(ref color, "color", GenMapUI.DefaultThingLabelColor);
    }
}
