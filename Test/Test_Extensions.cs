using Verse;

using Blocky.Signs;

class Test_Extensions {
    public static void Run(){
        Console.WriteLine("[.] Running Test_Extensions");

        Type t = typeof(IntVec3);

        var ct = new ExpCompiler.CallToken("GetEdifice", new[]{ ExpCompiler.Tokenize("Map")} );
        TestExtensions.Unregister();
        var mi = ExpCompiler.getMethodInfo(t, ct);
        
        Expect.Eq(mi.Name, "GetEdifice");

        TestExtensions.Register();
        Expect.Eq(_Building.instance, ExpCompiler.Compile("this.parent.Position.GetEdifice(Find.CurrentMap)", _Comp.instance).Invoke(_Comp.instance));
    }
}
