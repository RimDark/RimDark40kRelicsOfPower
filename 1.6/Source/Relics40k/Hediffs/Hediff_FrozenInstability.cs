using Verse;

namespace Relics40k;

/// <summary>
/// Carries the thunder warrior instability stage that was captured at the moment of
/// catalysis. Severity is set once, to (capturedStage + 1), and nothing ever moves it:
/// the hediff has no comps, cannot merge, and can never be removed for low severity.
/// </summary>
public class Hediff_FrozenInstability : HediffWithComps
{
    public override bool ShouldRemove => false;

    public override bool TryMergeWith(Hediff other)
    {
        return false;
    }

    /// <summary>Stage index (0-4) the thought reads back out.</summary>
    public int FrozenStage => CurStageIndex;
}
