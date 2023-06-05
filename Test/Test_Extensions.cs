using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Verse;

using Blocky.Signs;

public static class MyExtensions {
    public static _Building GetEdifice(this _IntVec3 x, _Map map) {
        return _Building.instance;
    }
}

class Test_Extensions {

    static _Building stub(_IntVec3 x){
        return x.GetEdifice(_Map.instance);
    }

    public static void Run(){
        Console.WriteLine("[.] Running Test_Extensions");

        Type t = typeof(IntVec3);

        var ct = new ExpCompiler.CallToken("GetEdifice", new[]{ ExpCompiler.Tokenize("Map")} );
        var mi = ExpCompiler.getMethodInfo(t, ct);
        
        Expect.Eq(mi.Name, "GetEdifice");

        Type[] allTypes = Assembly.GetEntryAssembly().GetTypes();
        Type[] extensionTypes = allTypes.Where(t => t.IsDefined(typeof(ExtensionAttribute))).ToArray();
        MethodInfo[] extensionMethods = extensionTypes.SelectMany(e => e.GetMethods().Where(m => m.IsDefined(typeof(ExtensionAttribute)))).ToArray();

        ExpCompiler.allExtensionMethods = extensionMethods;

        Expect.Eq(_Building.instance, ExpCompiler.Compile("this.parent.Position.GetEdifice(Find.CurrentMap)", _Comp.instance).Invoke(_Comp.instance));
    }
}
