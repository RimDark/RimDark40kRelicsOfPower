using RimWorld;
using Verse;

namespace Relics40k;

/// <summary>
/// Re-hosts the mood effect that BEWH_ThunderWarriorInstability used to provide, at the
/// stage frozen by the gene-crypt catalyst. Deliberately looks the hediff up by class
/// rather than by DefOf, so this worker needs no def that might not exist.
/// </summary>
public class ThoughtWorker_FrozenInstability : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        var hediffs = p?.health?.hediffSet?.hediffs;

        if (hediffs == null)
        {
            return false;
        }

        foreach (var hediff in hediffs)
        {
            if (hediff is Hediff_FrozenInstability frozenInstability)
            {
                return ThoughtState.ActiveAtStage(frozenInstability.FrozenStage);
            }
        }

        return false;
    }
}
