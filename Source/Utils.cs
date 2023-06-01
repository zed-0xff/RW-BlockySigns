using System;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

public static class Utils {

    public static Vector2 CalcTextSizeForFont(string text, GameFont font){
        GameFont prevFont = Text.Font;
        Text.Font = font;
        Vector2 size = Text.CalcSize(text);
        Text.Font = prevFont;
        return size;
    }

    // draw label with custom BG texture/color
    public static void DrawCustomThingLabel(Vector2 screenPos, string text, Color textColor, Color? bgColor, Texture2D bgTex, GameFont font = GameFont.Tiny, bool debug = false) {
        Text.Font = font;
        float x = Text.CalcSize(text).x;
        float pad = (Text.TinyFontSupported && font == GameFont.Tiny ? 2 : 3);
        float height = (Text.TinyFontSupported && font == GameFont.Tiny ? 13 : 16);

//        if( bgTex != null ){
//            var rect = new Rect(screenPos.x - x / 2f - pad, screenPos.y-3, x + pad * 2f, height+4);
//            GUI.color = Color.black;
//            Widgets.DrawBox(rect.ExpandedBy(1), 1);
//            GUI.color = bgColor.Value;
//            GUI.DrawTexture(rect, bgTex);
//
//            if( debug ){
//                GUI.DrawTexture(new Rect(10,110,100,100), bgTex);
//            }
//        }

        GUI.color = textColor;
        Text.Anchor = TextAnchor.UpperCenter;
        Widgets.Label(new Rect(screenPos.x - x / 2f, screenPos.y - 3f, x, 999f), text);
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;
    }

//    // draw label with custom BG color
//    public static void DrawCustomThingLabel(Vector2 screenPos, string text, Color textColor, Color? bgColor = null, GameFont font = GameFont.Tiny) {
//        Text.Font = font;
//        float x = Text.CalcSize(text).x;
//        float pad = (Text.TinyFontSupported && font == GameFont.Tiny ? 2 : 3);
//        float height = (Text.TinyFontSupported && font == GameFont.Tiny ? 13 : 16);
//        if( bgColor != null ){
//            GUI.color = bgColor.Value;
//            var rect = new Rect(screenPos.x - x / 2f - pad, screenPos.y-2, x + pad * 2f, height+2);
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
//        float pad = (Text.TinyFontSupported && font == GameFont.Tiny ? 2 : 3);
//        float height = (Text.TinyFontSupported && font == GameFont.Tiny ? 13 : 16);
//        if( bgTex != null ){
//            var rect = new Rect(screenPos.x - x / 2f - pad, screenPos.y-2, x + pad * 2f, height+2);
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
