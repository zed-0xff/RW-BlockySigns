using Verse;

namespace Blocky.Signs;

public class CompProperties_Nameable: CompProperties {
    public float labelShift = 0.32f;

	public CompProperties_Nameable() {
		compClass = typeof(CompNameable);
	}
}
