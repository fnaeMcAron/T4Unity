using UnityEngine;
using UnityEngine.UI;

public class GeneMenuController : MonoBehaviour
{
    public Toggle neuroToggle;
    public Toggle frameToggle;
    public Toggle eelToggle;

    public GeneSystem geneSystem;

    GeneBits pendingBits;

    void Start()
    {
        UpdatePendingBits();
    }

    public void OnToggleChanged()
    {
        UpdatePendingBits();
    }

    void UpdatePendingBits()
    {
        pendingBits = GeneBits.None;

        if (neuroToggle.isOn)
            pendingBits |= GeneBits.Neuro;

        if (frameToggle.isOn)
            pendingBits |= GeneBits.Frame;

        if (eelToggle.isOn)
            pendingBits |= GeneBits.Eel;
    }

    public void Confirm()
    {
        geneSystem.ApplyBits(pendingBits);
    }
}
