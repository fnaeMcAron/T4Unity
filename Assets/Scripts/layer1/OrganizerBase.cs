using UnityEngine;

public abstract class OrganizerBase : MonoBehaviour
{
    [Header("Настройки контекста сцены")]
    public Camera cam;
    public IGameState initialState { get; set; }

    public virtual void Start()
    {
        if (DungeonMaster.Instance != null)
        {
            DungeonMaster.Instance.RegisterScene(this);
        }
    }
}
