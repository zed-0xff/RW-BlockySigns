using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;
using Blocky.Core;

namespace Blocky.Signs;

// copying signs content & color
class Patch__Designator_Build {
    [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.Deselected))]
    class SignCopier_ResetState {
        static void Postfix() {
            SignCopier.ResetState();
        }
    }
}
