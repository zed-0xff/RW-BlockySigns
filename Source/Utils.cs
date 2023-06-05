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
