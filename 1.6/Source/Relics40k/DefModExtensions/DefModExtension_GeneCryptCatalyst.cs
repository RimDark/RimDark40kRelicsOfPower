using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Relics40k;

/// <summary>
/// All Mankind's Finest defs the catalyst touches are injected through here, so the
/// assembly never names a BEWH_ def and never references Genes40k.dll.
/// </summary>
public class DefModExtension_GeneCryptCatalyst : DefModExtension
{
    // Every one of these must be active on the pawn for the item to be usable.
    public List<GeneDef> requiredGenes = [];

    public List<GeneDef> genesToRemove = [];

    public List<GeneDef> genesToAdd = [];

    public XenotypeDef newXenotype = null;

    public string newXenotypeName = null;

    public XenotypeIconDef newXenotypeIcon = null;

    public HediffDef comaHediff = null;

    public HediffDef frozenInstabilityHediff = null;
}
