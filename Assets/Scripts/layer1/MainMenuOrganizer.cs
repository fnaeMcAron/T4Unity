public class MainMenuOrganizer : OrganizerBase
{
    public override void Start()
    {
        if (DungeonMaster.Instance != null)
        {
            initialState = new MainMenuState(this);
            DungeonMaster.Instance.RegisterScene(this);
        }
    }
}
