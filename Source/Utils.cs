using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

public static class Utils {

    public delegate object DynInvoker(object root);

    // this         this
    // this         this.parent
    // this         this.parent.DrawColor

    // field:       Find.GameInfo.RealPlayTimeInteracting
    // field+NS:    YADA.ModConfig.RootDir
    // method:      StorytellerUtilityPopulation.DebugReadout()
    // field chain: Find.CameraDriver.CurrentZoom

    // fast, because expression is compiled into bytecode
    public static DynInvoker FastCallAny( string fqmn, object root, bool debug = false ){
        var a = fqmn.Split(new char[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        if( a.Count() < 1 ){
            throw new ArgumentException("don't know how to parse " + a.Count() + " arg(s)");
        }

        DynamicMethod dynMethod = new DynamicMethod(fqmn, typeof(object), new Type[]{typeof(object)}, restrictedSkipVisibility: true);
        ILGenerator il = dynMethod.GetILGenerator(256);

        Type lastValueType;
        object obj;
    
        if( a[0] == "this" ){
            a.RemoveAt(0);
            obj = root;
            lastValueType = root.GetType();
            il.Emit(OpCodes.Ldarg_0);
        } else {
            List<string> typeParts = new List<string>();
            Type t = null;
            while( t == null && a.Any() ){
                typeParts.Add(a[0]);
                a.RemoveAt(0);
                t = AccessTools.TypeByName(string.Join(".", typeParts));
            }
            if( t == null ){
                throw new ArgumentException("cannot resolve root type");
            }
            obj = t;
            lastValueType = t;
        }

        while( a.Any() ){
            string chunk = a[0];
            a.RemoveAt(0);

            if( !chunk.EndsWith("()") ){
                // field
                FieldInfo fi = AccessTools.Field((obj is Type) ? (Type)obj : obj.GetType(), chunk);
                if( fi != null ){
                    if( !fi.IsStatic && (obj == null || obj is Type) )
                        throw new ArgumentException("non-static field " + fi + " on no object");

                    obj = fi.GetValue(obj);
                    lastValueType = fi.FieldType;
                    {
                        var opcode = fi.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld;
                        if( debug ) Log.Message("[d] " + opcode + " " + fi);
                        il.Emit(opcode, fi);
                    }
                    continue;
                }
                // fi == null, try as getter
                //throw new ArgumentException("no field " + chunk + " on " + (obj == null ? "Null" : obj));
                chunk = "get_" + chunk;
            }

            // method
            MethodInfo mi = AccessTools.Method((obj is Type) ? (Type)obj : obj.GetType(), chunk.Replace("()", ""));
            if( mi == null )
                throw new ArgumentException("no method " + chunk + " on " + (obj == null ? "Null" : obj));
            if( !mi.IsStatic && (obj == null || obj is Type) )
                throw new ArgumentException("non-static method " + mi + " on no object");

            {
                var opcode = mi.IsStatic ? OpCodes.Call : OpCodes.Callvirt;
                if( debug ) Log.Message("[d] " + opcode + " " + mi);
                il.Emit(opcode, mi);
            }

            lastValueType = mi.ReturnType;

            if( a.Count() == 0 ){
                // skip last real call, only emit as opcode
                break;
            }
            obj = mi.Invoke(obj, null);
        }

        if (lastValueType.IsValueType){
            il.Emit(OpCodes.Box, lastValueType); // convert int/float/... into an object
        }

        il.Emit(OpCodes.Ret);

        return (DynInvoker)dynMethod.CreateDelegate(typeof(DynInvoker));
    }

    // XXX slow, because method resolving is not cached
    // field:       GameInfo.RealPlayTimeInteracting
    // field+NS:    YADA.ModConfig.RootDir
    // method:      StorytellerUtilityPopulation.DebugReadout()
    // field chain: Find.CameraDriver.CurrentZoom
    public static string CallAny( string fqmn ){
        var a = fqmn.Split(new char[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        if( a.Count() < 2 ){
            throw new ArgumentException("don't know how to parse " + a.Count() + " arg(s)");
        }
    
        List<string> typeParts = new List<string>();
        Type t = null;
        while( t == null && a.Any() ){
            typeParts.Add(a[0]);
            a.RemoveAt(0);
            t = AccessTools.TypeByName(string.Join(".", typeParts));
        }
        if( t == null ){
            throw new ArgumentException("cannot resolve root type");
        }

        object obj = t;
        while( a.Any() ){
            string chunk = a[0];
            a.RemoveAt(0);

            if( !chunk.EndsWith("()") ){
                // field
                FieldInfo fi = AccessTools.Field((obj is Type) ? (Type)obj : obj.GetType(), chunk);
                if( fi != null ){
                    if( !fi.IsStatic && (obj == null || obj is Type) )
                        throw new ArgumentException("non-static field " + fi + " on no object");

                    obj = fi.GetValue(obj);
                    continue;
                }
                // fi == null, try as getter
                //throw new ArgumentException("no field " + chunk + " on " + (obj == null ? "Null" : obj));
                chunk = "get_" + chunk;
            }

            // method
            MethodInfo mi = AccessTools.Method((obj is Type) ? (Type)obj : obj.GetType(), chunk.Replace("()", ""));
            if( mi == null )
                throw new ArgumentException("no method " + chunk + " on " + (obj == null ? "Null" : obj));
            if( !mi.IsStatic && (obj == null || obj is Type) )
                throw new ArgumentException("non-static method " + mi + " on no object");

            obj = mi.Invoke(obj, null);
        }

        return obj.ToString();
    }

    public static Vector2 CalcTextSizeForFont(string text, GameFont font){
        GameFont prevFont = Text.Font;
        Text.Font = font;
        Vector2 size = Text.CalcSize(text);
        Text.Font = prevFont;
        return size;
    }

    // draw label with custom BG texture/color
    public static void DrawThingLabel(Vector2 screenPos, string text, Color textColor, Color? bgColor, Texture2D bgTex = null, GameFont font = GameFont.Tiny, ScaleMode scaleMode = ScaleMode.StretchToFill) {
        Text.Font = font;
        var textSize = Text.CalcSize(text);
        float padX = 4;

        if( bgColor != null ){
            var rect = new Rect(screenPos.x - textSize.x / 2 - padX, screenPos.y, textSize.x + padX * 2, textSize.y);
            Widgets.DrawBoxSolid(rect.ExpandedBy(1), Color.black);
            GUI.color = bgColor.Value;
            GUI.DrawTexture(rect, bgTex ?? BaseContent.WhiteTex, scaleMode);
        }

        GUI.color = textColor;
        Text.Anchor = TextAnchor.UpperCenter;
        Widgets.Label(new Rect(screenPos.x - textSize.x / 2, screenPos.y, textSize.x, 999f), text);
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;
    }

    public static void DrawThingLabelAtlas(Vector2 screenPos, string text, Color textColor, Color? bgColor, Texture2D atlasTex, GameFont font = GameFont.Tiny, float minWidth = 0) {
        Text.Font = font;
        var textSize = Text.CalcSize(text);
        float padX = 4;

        if( bgColor != null ){
            float padY = 2; // need addtional padY because atlas might have transparent edges
            float w = Mathf.Max(minWidth, textSize.x);
            var rect = new Rect(screenPos.x - w/2 - padX, screenPos.y-padY, w + padX * 2, textSize.y + padY+1);
            GUI.color = bgColor.Value;
            Widgets.DrawAtlas(rect, atlasTex);
        }

        GUI.color = textColor;
        Text.Anchor = TextAnchor.UpperCenter;
        Widgets.Label(new Rect(screenPos.x - textSize.x/2, screenPos.y, textSize.x, 999f), text);
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;
    }

//    // draw label with custom BG color
//    public static void DrawCustomThingLabel(Vector2 screenPos, string text, Color textColor, Color? bgColor = null, GameFont font = GameFont.Tiny) {
//        Text.Font = font;
//        float x = Text.CalcSize(text).x;
//        float padX = (Text.TinyFontSupported && font == GameFont.Tiny ? 2 : 3);
//        float height = (Text.TinyFontSupported && font == GameFont.Tiny ? 13 : 16);
//        if( bgColor != null ){
//            GUI.color = bgColor.Value;
//            var rect = new Rect(screenPos.x - x / 2f - padX, screenPos.y-2, x + padX * 2f, height+2);
//            GUI.DrawTexture(rect, BaseContent.WhiteTex);
//        }
//        GUI.color = textColor;
//        Text.Anchor = TextAnchor.UpperCenter;
//        Widgets.Label(new Rect(screenPos.x - x / 2f, screenPos.y - 3f, x, 999f), text);
//        GUI.color = Color.white;
//        Text.Anchor = TextAnchor.UpperLeft;
//        Text.Font = GameFont.Small;
//    }
//
//    // draw label with custom BG texture
//    public static void DrawCustomThingLabel(Vector2 screenPos, string text, Color textColor, Texture2D bgTex = null, GameFont font = GameFont.Tiny) {
//        Text.Font = font;
//        float x = Text.CalcSize(text).x;
//        float padX = (Text.TinyFontSupported && font == GameFont.Tiny ? 2 : 3);
//        float height = (Text.TinyFontSupported && font == GameFont.Tiny ? 13 : 16);
//        if( bgTex != null ){
//            var rect = new Rect(screenPos.x - x / 2f - padX, screenPos.y-2, x + padX * 2f, height+2);
//            GUI.DrawTexture(rect, bgTex);
//        }
//        GUI.color = textColor;
//        Text.Anchor = TextAnchor.UpperCenter;
//        Widgets.Label(new Rect(screenPos.x - x / 2f, screenPos.y - 3f, x, 999f), text);
//        GUI.color = Color.white;
//        Text.Anchor = TextAnchor.UpperLeft;
//        Text.Font = GameFont.Small;
//    }
}
