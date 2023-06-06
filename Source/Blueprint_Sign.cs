using RimWorld;
using UnityEngine;
using Verse;

namespace Blocky.Signs;

// only for copying a sign
public class Blueprint_Sign : Blueprint_Build {
    public override void SpawnSetup(Map map, bool respawningAfterLoad) {
        base.SpawnSetup(map, respawningAfterLoad);

        if( !respawningAfterLoad ){
            SignCopier.TryEnqueue(this);
        }
    }
}
