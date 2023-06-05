using System.Reflection;
using System.Runtime.CompilerServices;

using Blocky.Signs;

public static class TestExtensions {
    public static _Building GetEdifice(this _IntVec3 x, _Map map) {
        return _Building.instance;
    }

    public static _Room GetRoom(this _IntVec3 x, _Map map) {
        return _Room.instance;
    }

    // needed only for tests
    public static void Register(){
        Type[] allTypes = Assembly.GetEntryAssembly().GetTypes();
        Type[] extensionTypes = allTypes.Where(t => t.IsDefined(typeof(ExtensionAttribute))).ToArray();
        MethodInfo[] extensionMethods = extensionTypes.SelectMany(e => e.GetMethods().Where(m => m.IsDefined(typeof(ExtensionAttribute)))).ToArray();

        ExpCompiler.allExtensionMethods = extensionMethods;
    }

    public static void Unregister(){
        ExpCompiler.allExtensionMethods = null;
    }
}

