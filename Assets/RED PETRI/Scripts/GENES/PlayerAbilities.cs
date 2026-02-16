using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public StudentMovement movement;
    public GeneSystem genes;

    void Update()
    {
        ApplyGeneEffects();
    }

    void ApplyGeneEffects()
    {
        movement.ResetModifiers();

        switch (genes.currentGene)
        {
            case GeneType.VestibularControl:
                movement.airControl = true;
                break;

            case GeneType.AnchorMode:
                movement.canWallWalk = true;
                break;

            case GeneType.KineticBurst:
                movement.canDash = true;
                break;

            case GeneType.BioElectric:
                movement.canActivateElectric = true;
                break;

            case GeneType.Bioluminescence:
                movement.lightEnabled = true;
                break;

            case GeneType.AdaptiveDensity:
                movement.canChangeSize = true;
                break;

            case GeneType.SymbioticSynergy:
                movement.extraJump = true;
                break;
        }
    }
}
