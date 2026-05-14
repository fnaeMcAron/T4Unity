using UnityEngine;

public class SubGameOrganizer : OrganizerBase
{
    public StudentMovement stdmove;
    public DocController docmenu;

    public override void Start()
    {
        if (DungeonMaster.Instance != null)
        {
            //initialState = new sub_TerminalState();
            DungeonMaster.Instance.RegisterSubscene(this);
        }
    }
}
