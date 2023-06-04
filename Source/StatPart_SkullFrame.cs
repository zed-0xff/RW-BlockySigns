using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Blocky.Signs;

public class StatPart_SkullFrame : StatPart
{
	public int offsetOccupied;

	public List<ThingDef> thingDefs;

	public override void TransformValue(StatRequest req, ref float val)
	{
		if (ApplyTo(req)) {
			val += offsetOccupied;
		} else {
            val = 0;
        }
	}

	public override string ExplanationPart(StatRequest req)
	{
		if (!ApplyTo(req))
		{
			return null;
		}
		return (string)("StatsReport_OccupiedCorpseCasket".Translate() + ": ") + offsetOccupied;
	}

	private bool ApplyTo(StatRequest req)
	{
		if (req.Thing is Building_Frame f && (f.HasCorpse() || f.HasSkull()) && offsetOccupied != 0)
		{
			if (thingDefs != null)
			{
				return thingDefs.Contains(req.Thing.def);
			}
			return true;
		}
		return false;
	}
}
