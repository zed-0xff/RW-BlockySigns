using System.Reflection;
using System.Runtime.CompilerServices;
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

        Expect.Eq(_Map.instance, ExpCompiler.Compile("_Find.CurrentMap", _Comp.instance).Invoke(_Comp.instance));

        Expect.Eq(0, ExpCompiler.Compile("0", _Comp.instance).Invoke(_Comp.instance));
        Expect.Eq(_Map.instance.LoadedFullThings[0], ExpCompiler.Compile("_Find.CurrentMap.loadedFullThings[0]", _Comp.instance).Invoke(_Comp.instance));
    }
}
