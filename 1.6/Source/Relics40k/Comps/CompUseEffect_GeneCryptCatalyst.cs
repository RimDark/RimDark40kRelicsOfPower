using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Relics40k;

/// <summary>
/// Turns a thunder warrior into a Thunderborn Astartes. Left at the default order
/// priority of 0 so it always runs before CompUseEffect_DestroySelf, which sits at -1000.
/// </summary>
public class CompUseEffect_GeneCryptCatalyst : CompUseEffect
{
    private DefModExtension_GeneCryptCatalyst Extension =>
        parent.def.GetModExtension<DefModExtension_GeneCryptCatalyst>();

    public override AcceptanceReport CanBeUsedBy(Pawn p)
    {
        var extension = Extension;

        if (extension == null || p?.genes == null)
        {
            return "Relics40k.GeneCryptCatalyst.NotThunderWarrior".Translate();
        }

        if (extension.comaHediff != null && p.health.hediffSet.HasHediff(extension.comaHediff))
        {
            return "Relics40k.GeneCryptCatalyst.AlreadyUndergoing".Translate();
        }

        foreach (var geneDef in extension.requiredGenes)
        {
            if (!p.genes.HasActiveGene(geneDef))
            {
                return "Relics40k.GeneCryptCatalyst.NotThunderWarrior".Translate();
            }
        }

        return true;
    }

    public override TaggedString ConfirmMessage(Pawn p)
    {
        return "Relics40k.GeneCryptCatalyst.Confirm".Translate(p.Named("PAWN"));
    }

    public override void DoEffect(Pawn usedBy)
    {
        base.DoEffect(usedBy);

        var extension = Extension;

        if (extension == null || usedBy?.genes == null)
        {
            return;
        }

        // Snapshot before anything is removed - the stage lives on the Furybound gene.
        var instabilityStage = MankindsFinestCompat.GetFuryboundStage(usedBy);

        RemoveGenes(usedBy, extension.genesToRemove);
        AddGenes(usedBy, extension.genesToAdd);
        ApplyFrozenInstability(usedBy, extension, instabilityStage);
        ApplyXenotype(usedBy, extension);

        if (extension.comaHediff != null)
        {
            usedBy.health.AddHediff(extension.comaHediff);
        }

        SendLetter(usedBy);
    }

    private static void RemoveGenes(Pawn pawn, List<GeneDef> geneDefs)
    {
        if (geneDefs.NullOrEmpty())
        {
            return;
        }

        // Copy first: RemoveGene mutates the list we would otherwise be iterating.
        var toRemove = new List<Gene>();

        foreach (var gene in pawn.genes.GenesListForReading)
        {
            if (gene != null && geneDefs.Contains(gene.def))
            {
                toRemove.Add(gene);
            }
        }

        foreach (var gene in toRemove)
        {
            pawn.genes.RemoveGene(gene);
        }
    }

    private static void AddGenes(Pawn pawn, List<GeneDef> geneDefs)
    {
        if (geneDefs.NullOrEmpty())
        {
            return;
        }

        foreach (var geneDef in geneDefs)
        {
            if (!pawn.genes.HasActiveGene(geneDef))
            {
                pawn.genes.AddGene(geneDef, true);
            }
        }
    }

    private static void ApplyFrozenInstability(Pawn pawn, DefModExtension_GeneCryptCatalyst extension, int stage)
    {
        if (extension.frozenInstabilityHediff == null || stage < 0)
        {
            return;
        }

        var hediff = pawn.health.AddHediff(extension.frozenInstabilityHediff);

        if (hediff != null)
        {
            hediff.Severity = stage + 1;
        }
    }

    private static void ApplyXenotype(Pawn pawn, DefModExtension_GeneCryptCatalyst extension)
    {
        if (extension.newXenotype != null)
        {
            // Must come first: SetXenotypeDirect nulls xenotypeName.
            pawn.genes.SetXenotypeDirect(extension.newXenotype);
        }

        if (!extension.newXenotypeName.NullOrEmpty())
        {
            pawn.genes.xenotypeName = extension.newXenotypeName;
        }

        if (extension.newXenotypeIcon != null)
        {
            pawn.genes.iconDef = extension.newXenotypeIcon;
        }
    }

    private static void SendLetter(Pawn pawn)
    {
        Find.LetterStack.ReceiveLetter(
            "Relics40k.GeneCryptCatalyst.LetterLabel".Translate(pawn.Named("PAWN")),
            "Relics40k.GeneCryptCatalyst.LetterText".Translate(pawn.Named("PAWN")),
            LetterDefOf.PositiveEvent,
            pawn);
    }
}
