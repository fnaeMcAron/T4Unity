using UnityEngine;

public class MainGameOrganizer : OrganizerBase, IPlayerControllable
{
    public CharacterManager charman;


    public override void Start()
    {
        if (DungeonMaster.Instance != null)
        {
            initialState = new ShardsState(this);
            DungeonMaster.Instance.RegisterScene(this);
        }
    }

    public void StopPlayer()
    {
        charman.currentCharacter.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ResumePlayer()
    {
        charman.currentCharacter.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}
