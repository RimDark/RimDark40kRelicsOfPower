using System;
using System.Reflection;
using Verse;

namespace Relics40k;

/// <summary>
/// The only place in this assembly that touches Mankind's Finest, and it does so purely
/// by reflection. Resolves to null when that mod is absent, so nothing here can ever
/// force the JIT to load a missing assembly.
/// </summary>
public static class MankindsFinestCompat
{
    private const string FuryboundGeneTypeName = "Genes40k.Gene_Furybound";

    private const string ThoughtStagePropertyName = "CurrentThoughtStage";

    private static bool resolved;

    private static Type furyboundGeneType;

    private static PropertyInfo thoughtStageProperty;

    private static void Resolve()
    {
        if (resolved)
        {
            return;
        }

        resolved = true;

        furyboundGeneType = GenTypes.GetTypeInAnyAssembly(FuryboundGeneTypeName);

        if (furyboundGeneType == null)
        {
            return;
        }

        thoughtStageProperty = furyboundGeneType.GetProperty(
            ThoughtStagePropertyName,
            BindingFlags.Public | BindingFlags.Instance);

        if (thoughtStageProperty == null || thoughtStageProperty.PropertyType != typeof(int))
        {
            thoughtStageProperty = null;
            Log.WarningOnce(
                "[Relics of Power] Found " + FuryboundGeneTypeName + " but no readable int " +
                ThoughtStagePropertyName + " property. Thunder warrior instability will not be preserved.",
                0x5EC7A0);
        }
    }

    /// <summary>
    /// The pawn's current Furybound instability stage (0-4), or -1 when it cannot be read
    /// (Mankind's Finest absent, gene absent, or the property has moved).
    /// </summary>
    public static int GetFuryboundStage(Pawn pawn)
    {
        Resolve();

        if (thoughtStageProperty == null || pawn?.genes == null)
        {
            return -1;
        }

        foreach (var gene in pawn.genes.GenesListForReading)
        {
            if (gene == null || !furyboundGeneType.IsInstanceOfType(gene))
            {
                continue;
            }

            try
            {
                return (int)thoughtStageProperty.GetValue(gene);
            }
            catch (Exception ex)
            {
                Log.WarningOnce(
                    "[Relics of Power] Could not read the Furybound instability stage: " + ex.Message,
                    0x5EC7A1);
                return -1;
            }
        }

        return -1;
    }
}
