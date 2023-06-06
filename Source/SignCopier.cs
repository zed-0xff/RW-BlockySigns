using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

public class SignCopier : WorldComponent {
    static CompNameable src = null;
    static List<CopyInfo> queue = new List<CopyInfo>();

    public SignCopier(World w) : base(w) {
    }

    class CopyInfo : IExposable {
        public string name;
        public Color color;
        public IntVec3 pos;
        public Map map;

        public void ExposeData() {
            Scribe_Values.Look(ref name, "name");
            Scribe_Values.Look(ref color, "color");
            Scribe_Values.Look(ref pos, "pos");
            Scribe_References.Look(ref map, "map");
        }
    }

    public override void ExposeData() {
        base.ExposeData();
        Scribe_Collections.Look(ref queue, "queue", LookMode.Deep);

        if( Scribe.mode == LoadSaveMode.PostLoadInit ){
            if( queue == null )
                queue = new List<CopyInfo>();

            queue.RemoveAll(x => x.map == null);
        }
    }

    public static void ResetState(){
        src = null;
    }

    public static void StartCopyFrom(CompNameable src0){
        src = src0;
    }

    public static void PostSpawnSetup(CompNameable dst){
        if( src != null ){
            // god mode
            dst.Name = src.Name;
            dst.color = src.color;
            return;
        }

        foreach( CopyInfo ci in queue ){
            if( dst.parent.Map == ci.map && dst.parent.Position == ci.pos ){
                dst.Name = ci.name;
                dst.color = ci.color;
                queue.Remove(ci);
                break;
            }
        }
    }

    public static void TryCleanupAt(IntVec3 pos, Map map){
        foreach( CopyInfo ci in queue ){
            if( ci.map == map && ci.pos == pos ){
                queue.Remove(ci);
                break;
            }
        }
    }

    public static void TryEnqueue(Blueprint_Sign bs){
        if(src == null){
            TryCleanupAt(bs.Position, bs.Map);
            return;
        }

        CopyInfo ci = new CopyInfo();
        ci.name = src.Name;
        ci.color = src.color;
        ci.pos = bs.Position;
        ci.map = bs.Map;

        queue.Add(ci);
    }
}
