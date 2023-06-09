using System.Reflection;
using System.Runtime.CompilerServices;
using Verse;

using Blocky.Signs;

class Test_Null {
    public static void Run(){
        Console.WriteLine("[.] Running Test_Null");

        Expect.Eq(null, ExpCompiler.Compile("_Find.anotherMap", _Comp.instance).Invoke(_Comp.instance));

        // should not throw
        var invoker = ExpCompiler.Compile("_Find.anotherMap.loadedFullThings[0]", _Comp.instance);
        _Find.anotherMap = _Find.CurrentMap;
        Expect.Eq(_Thing.instance, invoker.Invoke(null));

//        _Find.anotherMap = null;
//        Expect.Eq(null, invoker.Invoke(null));
    }
}
