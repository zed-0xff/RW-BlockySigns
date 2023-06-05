using System.Collections;
using System.Reflection;

using Blocky.Signs;

class Test_ExpCompiler {
    public static void Run(){
        Console.WriteLine("[.] Running Test_ExpCompiler");

        List<ExpCompiler.Token> tokens;
       
        tokens = ExpCompiler.Tokenize("");
        Expect.Eq(0, tokens.Count());

        tokens = ExpCompiler.Tokenize("a");
        Expect.Eq(1, tokens.Count());

        tokens = ExpCompiler.Tokenize("a.b");
        Expect.Eq(2, tokens.Count());

        tokens = ExpCompiler.Tokenize("aaa.bbb");
        Expect.Eq("[aaa].[bbb]", tokens.ToString());

        tokens = ExpCompiler.Tokenize("aaa.bbb().ccc");
        Expect.Eq("[aaa].[bbb()].[ccc]", tokens.ToString());

        tokens = ExpCompiler.Tokenize("aaa.bbb(this).ccc");
        Expect.Eq("[aaa].[bbb([this])].[ccc]", tokens.ToString());

        tokens = ExpCompiler.Tokenize("aaa.bbb(this.bar).ccc");
        Expect.Eq("[aaa].[bbb([this].[bar])].[ccc]", tokens.ToString());

        tokens = ExpCompiler.Tokenize("GenCelestial.CurCelestialSunGlow(Find.CurrentMap)");
        Expect.Eq("[GenCelestial].[CurCelestialSunGlow([Find].[CurrentMap])]", tokens.ToString());

        tokens = ExpCompiler.Tokenize("Gizmo_RoomStats.GetRoomToShowStatsFor(this).GetStat(RoomStatDefOf.Wealth)");
        Expect.Eq("[Gizmo_RoomStats].[GetRoomToShowStatsFor([this])].[GetStat([RoomStatDefOf].[Wealth])]", tokens.ToString());

        Expect.Eq(111, ExpCompiler.Compile("this.mi()", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(222, ExpCompiler.Compile("_Comp.smi()", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(333, ExpCompiler.Compile("this.fi", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(444, ExpCompiler.Compile("_Comp.sfi", _Comp.instance).Invoke(_Comp.instance));

        Expect.Eq(111*333, ExpCompiler.Compile("this.mi2(this.fi)", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(111*444, ExpCompiler.Compile("this.mi2(_Comp.sfi)", _Comp.instance).Invoke(_Comp.instance));

        TestExtensions.Register();

        Expect.Eq(_Room.instance, ExpCompiler.Compile("_Gizmo_RoomStats.GetRoomToShowStatsFor(this.parent)", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(42, ExpCompiler.Compile("_Gizmo_RoomStats.GetRoomToShowStatsFor(this.parent).Temperature", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(42, ExpCompiler.Compile("parent.Position.GetRoom(Find.CurrentMap).Temperature", _Comp.instance).Invoke(_Comp.instance));

        // float, parenthesis
        Expect.Eq(1, ExpCompiler.Compile("Mathf.RoundToInt(this.mf())", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(5, ExpCompiler.Compile("Mathf.RoundToInt(this.ff)", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(4, ExpCompiler.Compile("Mathf.RoundToInt(Mathf.Floor(this.ff))", _Comp.instance).Invoke(_Comp.instance));
    }
}
