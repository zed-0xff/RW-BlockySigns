using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Blocky.Signs;

static class ExpCompiler {
    public delegate object DynInvoker(object root);

    static Dictionary<string, DynInvoker> invokerCache = new Dictionary<string, DynInvoker>();

    public static DynInvoker Compile(string exp, object root){
        DynInvoker invoker = null;
        if( !invokerCache.TryGetValue(exp, out invoker) ){
            invoker = invokerCache[exp] = FastCallAny(exp, root);
        }
        return invoker;
    }

    // this         this
    // this         this.parent
    // this         this.parent.DrawColor

    // field:       Find.GameInfo.RealPlayTimeInteracting
    // field+NS:    YADA.ModConfig.RootDir
    // method:      StorytellerUtilityPopulation.DebugReadout()
    // field chain: Find.CameraDriver.CurrentZoom

    // fast, because expression is compiled into bytecode
    static DynInvoker FastCallAny( string fqmn, object root, bool debug = false ){
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

}
