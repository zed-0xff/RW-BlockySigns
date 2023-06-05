using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Verse;

namespace Blocky.Signs;

static class ExpCompiler {
    public delegate object DynInvoker(object root);

    static Dictionary<string, DynInvoker> invokerCache = new Dictionary<string, DynInvoker>();

    public static DynInvoker Compile(string exp, object root){
        DynInvoker invoker;
        if( !invokerCache.TryGetValue(exp, out invoker) ){
            invoker = invokerCache[exp] = compile(exp, root);
        }
        return invoker;
    }

    public abstract class Token {
        public abstract string Original{ get; }
    }

    public class TokenList : List<Token> {
        public override string ToString(){
            return string.Join(".", this);
        }

        public string Original { get { return string.Join(".", this.Select((x) => x.Original)); }}
    }

    public class SimpleToken : Token {
        public string content;

        public override string Original { get { return content; } }

        public SimpleToken(string x) {
            content = x;
        }

        public override string ToString(){
            return "[" + content + "]";
        }
    }

    public class CallToken : Token {
        public string method;
        public TokenList[] args;

        public override string Original { get { return method + "(" + string.Join(",", args.Select((x) => x.Original)) + ")"; } }

        public CallToken(string m, TokenList[] a) {
            method = m;
            args = a == null ? new TokenList[]{} : a;
        }

        public override string ToString(){
            return "[" + method + "(" + string.Join(", ", (object[])args) + ")]";
        }
    }

    public static TokenList Tokenize(string fqmn){
        TokenList tokens = new TokenList();
        int start = 0;
        for( int i=0; i<fqmn.Length; i++ ){
            switch(fqmn[i]){
                case '.':
                    if( i != start ){
                        tokens.Add(new SimpleToken(fqmn.Substring(start, i-start)));
                    }
                    start = i+1;
                    break;
                case '(':
                    int depth = 0;
                    for( int j=i+1; j<fqmn.Length; j++ ){
                        if( fqmn[j] == '(' )
                            depth++;
                        if( fqmn[j] == ')' ){
                            if( depth > 0 ){
                                depth--;
                            } else {
                                var args = fqmn.Substring(i+1, j-i-1)
                                    .Split(',')
                                    .Select(x => x.Trim())
                                    .Where(x  => !string.IsNullOrEmpty(x))
                                    .Select(x => Tokenize(x))
                                    .ToArray();
                                tokens.Add(new CallToken(fqmn.Substring(start, i-start), args));
                                i = j;
                                start = i+1;
                                break;
                            }
                        }
                    }
                    break;
                case ')':
                    throw new ArgumentException("unexpected ')'");
            }
        }

        if( start < fqmn.Length ){
            tokens.Add(new SimpleToken(fqmn.Substring(start)));
        }

        return tokens;
    }

    static DynInvoker compile( string fqmn, object root ){
        return FastCallTokenized(Tokenize(fqmn), root);
    }

    static DynInvoker FastCallTokenized( TokenList tokens, object root){
        DynamicMethod dynMethod = new DynamicMethod("", typeof(object), new Type[]{typeof(object)}, restrictedSkipVisibility: true);
        ILGenerator il = dynMethod.GetILGenerator(256);

        // syntax sugar
        if( (tokens[0] as SimpleToken)?.content == "parent" ){
            tokens.Insert(0, new SimpleToken("this"));
        }

        Type lastValueType = FastCallTokenizedInt( tokens, root, il );

        if (lastValueType.IsValueType){
            il.Emit(OpCodes.Box, lastValueType); // convert int/float/... into an object
        }

        il.Emit(OpCodes.Ret);

        return (DynInvoker)dynMethod.CreateDelegate(typeof(DynInvoker));
    }

    static Type FastCallTokenizedInt( TokenList tokens, object root, ILGenerator il ){
        if( tokens.Count() < 1 ){
            throw new ArgumentException("don't know how to parse " + tokens.Count() + " arg(s)");
        }

        Type lastValueType;
        object obj;
    
        if( (tokens[0] as SimpleToken)?.content == "this" ){
            tokens.RemoveAt(0);
            obj = root;
            lastValueType = root.GetType();
            il.Emit(OpCodes.Ldarg_0);
        } else {
            List<string> typeParts = new List<string>();
            Type t = null;
            while( t == null && tokens[0] is SimpleToken st ){
                typeParts.Add(st.content);
                tokens.RemoveAt(0);
                t = AccessTools.TypeByName(string.Join(".", typeParts));
            }
            if( t == null ){
                throw new ArgumentException("cannot resolve root type");
            }
            obj = t;
            lastValueType = t;
        }

        while( tokens.Any() ){
            Token token = tokens[0];
            tokens.RemoveAt(0);

            if( token is SimpleToken st ){
                // field
                FieldInfo fi = AccessTools.Field((obj is Type) ? (Type)obj : obj.GetType(), st.content);
                if( fi != null ){
                    if( !fi.IsStatic && (obj == null || obj is Type) )
                        throw new ArgumentException("non-static field " + fi + " on no object");

                    obj = fi.GetValue(obj);
                    lastValueType = fi.FieldType;
                    il.Emit(fi.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, fi);
                    
                    continue;
                }
                // fi == null, try as getter
                // intentional fallback to CallToken check
                token = new CallToken("get_" + st.content, null);
            }

            if( token is CallToken ct ){
                MethodInfo mi = getMethodInfo((obj is Type) ? (Type)obj : obj.GetType(), ct);
                if( mi == null )
                    throw new ArgumentException("no method " + ct.Original + " on " + (obj == null ? "Null" : obj.GetType()));
                if( !mi.IsStatic && (obj == null || obj is Type) )
                    throw new ArgumentException("non-static method " + mi + " on no object");

                DynInvoker invoker = null;
                if( ct.args.Any() ){
                    invoker = Compile( ct.args[0].Original, root ); // XXX only a single arg now
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, invoker.GetInvocationList()[0].Method); // Call or Callvirt ?

                    // this.parent.Position.GetEdifice(Find.CurrentMap)
                    //                      ^^^^^^^^^^ extension method
                    int pidx = mi.IsDefined(typeof(ExtensionAttribute)) ? 1 : 0;
                    Type ptype = mi.GetParameters()[pidx].ParameterType;
                    if( ptype.IsValueType ){
                        il.Emit(OpCodes.Unbox_Any, ptype);
                    }
                }

                il.Emit(mi.IsStatic ? OpCodes.Call : OpCodes.Callvirt, mi);

                lastValueType = mi.ReturnType;

                if( tokens.Count() == 0 ){
                    // skip last real call, only emit as opcode
                    break;
                }

                if( invoker == null ){
                    obj = mi.Invoke(obj, null);
                } else if( !mi.IsDefined(typeof(ExtensionAttribute) )){
                    obj = mi.Invoke(obj, new[]{ invoker.Invoke(root) } );
                } else {
                    obj = mi.Invoke(obj, new[]{ obj, invoker.Invoke(root) } );
                }
            } else {
                throw new ArgumentException("unexpected token: " + token);
            }
        }
        return lastValueType;
    }

    public static MethodInfo getMethodInfo(Type t, CallToken ct){
        MethodInfo mi = AccessTools.Method(t, ct.method); // does not return extension methods!
        if( mi != null ) return mi;

        // try to find extension method
        foreach( MethodInfo mi2 in extensionMethodsForType(t) ){
            if( mi2.Name == ct.method && mi2.GetParameters().Length == ct.args.Length + 1 ){
                // XXX possible ambiguations like this:
                // IntVec3 RotatedBy(IntVec3, RotationDirection)
                // IntVec3 RotatedBy(IntVec3, Rot4)
                return mi2;
            }
        }

        return null;
    }

    public static MethodInfo[] allExtensionMethods = null;

    public static MethodInfo[] extensionMethodsForType(Type t){
        if( allExtensionMethods == null ){
            allExtensionMethods = GenTypes.AllTypes
                .Where(t => t.IsDefined(typeof(ExtensionAttribute)))
                .SelectMany(e => e.GetMethods().Where(m => m.IsDefined(typeof(ExtensionAttribute)))).ToArray();
        }

        return allExtensionMethods.Where(m => m.GetParameters()[0].ParameterType == t).ToArray();
    }
}
