using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Общие настройки персонажей")]
    public WormManager wormManager;
    public CharacterBase[] characters;
    public TMP_Text text;
    public CameraFollow cameraFollow;

    [Header("Текущий персонаж")]
    [SerializeField] private CharacterBase currentCharacter;
    [SerializeField] private int currentCharacterIndex;

    public CharacterBase CurrentCharacter => currentCharacter;
    public int CurrentCharacterIndex => currentCharacterIndex;
    public delegate void DeathAction();
    public static event DeathAction OnDeath; // Событие при смерти игрока

    void Start()
    {
        if (characters.Length > 0)
        {
            SwitchToCharacter(0);
        }
        Application.targetFrameRate = 8000;
    }

    private void LateUpdate()
    {
        text.text = "ЧЕРВЯЧКИИИИ: " + wormManager.GetWorms();
        if (wormManager.GetWorms() <= 0f)
        {
            Die(); // Вызываем метод при смерти игрока
        }
    }
    void Die()
    {
        if (OnDeath != null)
        {
            OnDeath(); // Вызываем событие при смерти игрока
        }

        Destroy(this.gameObject); // Уничтожаем игрока
    }

    public void SwitchToCharacter(int index)
    {
        Vector3 prevPos = new Vector3();
        Quaternion prevRot = new Quaternion();
        if (index < 0 || index >= characters.Length) return;

        currentCharacter.OnCharacterDeselected();
        currentCharacter.gameObject.SetActive(false);
        currentCharacter.transform.GetLocalPositionAndRotation(out prevPos, out prevRot);

        currentCharacterIndex = index;
        currentCharacter = characters[index];
        currentCharacter.OnCharacterSelected();
        currentCharacter.transform.SetLocalPositionAndRotation(prevPos, prevRot);
        currentCharacter.gameObject.SetActive(true);
        cameraFollow.SetTarget(currentCharacter.transform);
        Debug.Log($"Переключено на: {currentCharacter.name}");
    }
}