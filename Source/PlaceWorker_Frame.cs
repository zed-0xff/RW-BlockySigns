using System.Linq;
using RimWorld;
using Verse;

namespace Blocky.Signs;

public class PlaceWorker_Frame : PlaceWorker {
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null) {
        foreach( Thing t in loc.GetThingList(map) ){
            if( t == thingToIgnore ) continue;

            if( t is Building_Frame )
                return new AcceptanceReport("IdenticalThingExists".Translate());

            if( t is Blueprint b && b.def.entityDefToBuild is ThingDef td && typeof(Building_Frame).IsAssignableFrom(td.thingClass) )
                return new AcceptanceReport("IdenticalBlueprintExists".Translate());
        }
        return AcceptanceReport.WasAccepted;
    }
}

