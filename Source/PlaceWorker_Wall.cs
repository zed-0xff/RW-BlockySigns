using System.Linq;
using Verse;

namespace Blocky.Signs;

public class PlaceWorker_Wall : PlaceWorker {
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null) {
        if (loc.GetThingList(map)
                .Where(t => t.def.category == ThingCategory.Building)
                .Where(t => t.def.building != null)
                .Where(t => t.def.passability == Traversability.Impassable)
                .Any())
        {
            return AcceptanceReport.WasAccepted;
        }
        else
        {
            return new AcceptanceReport("WorkPlacer_CannotPlaceHere".Translate());
        }
    }
}

