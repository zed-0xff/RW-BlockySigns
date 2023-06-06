using System.Collections.Generic;
using Verse;

namespace Blocky.Signs;

// only for 'Build copy' gizmo patching
class Building_Sign : Building {
    public override IEnumerable<Gizmo> GetGizmos() {
        CompNameable compNameable = this.TryGetComp<CompNameable>();

        var copyLabel = "CommandBuildCopy".Translate();
        foreach (Gizmo gizmo in base.GetGizmos()) {
            if( gizmo is Command_Action ca && ca.defaultLabel == copyLabel && compNameable != null ){
                var prevAction = ca.action;
                ca.action = delegate {
                    SignCopier.StartCopyFrom(compNameable);
                    prevAction();
                };
            }
            yield return gizmo;
        }
    }
}
