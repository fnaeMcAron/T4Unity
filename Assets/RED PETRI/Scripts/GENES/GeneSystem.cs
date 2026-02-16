using UnityEngine;

[System.Flags]
public enum GeneBits
{
    None = 0, // 000
    Neuro = 1, // 001
    Frame = 2, // 010
    Eel = 4  // 100
}

public enum GeneType
{
    Base,
    VestibularControl,
    AnchorMode,
    KineticBurst,
    BioElectric,
    Bioluminescence,
    AdaptiveDensity,
    SymbioticSynergy
}

public class GeneSystem : MonoBehaviour
{
    public GeneBits currentBits;
    public GeneType currentGene;

    public void ApplyBits(GeneBits bits)
    {
        currentBits = bits;
        currentGene = ResolveGene(bits);
    }

    GeneType ResolveGene(GeneBits bits)
    {
        switch (bits)
        {
            case GeneBits.None:
                return GeneType.Base;

            case GeneBits.Neuro:
                return GeneType.VestibularControl;

            case GeneBits.Frame:
                return GeneType.AnchorMode;

            case GeneBits.Neuro | GeneBits.Frame:
                return GeneType.KineticBurst;

            case GeneBits.Eel:
                return GeneType.BioElectric;

            case GeneBits.Neuro | GeneBits.Eel:
                return GeneType.Bioluminescence;

            case GeneBits.Frame | GeneBits.Eel:
                return GeneType.AdaptiveDensity;

            case GeneBits.Neuro | GeneBits.Frame | GeneBits.Eel:
                return GeneType.SymbioticSynergy;
        }

        return GeneType.Base;
    }
}
