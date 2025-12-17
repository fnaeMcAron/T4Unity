using TMPro;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Общие настройки")]
    public WormManager wormManager;
    public CharacterBase[] characters;
    public TMP_Text text;
    public CameraFollow cameraFollow;
    public StyleManager styleManager;

    [Header("Текущий персонаж")]
    [SerializeField] private CharacterBase currentCharacter;
    [SerializeField] private int currentCharacterIndex;

    public CharacterBase CurrentCharacter => currentCharacter;
    public int CurrentCharacterIndex => currentCharacterIndex;

    public delegate void DeathAction();
    public static event DeathAction OnDeath;

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
        text.text = "ЧЕРВЯЧКИИИ: " + wormManager.GetWorms();
        if (wormManager.GetWorms() <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        OnDeath?.Invoke();
        Destroy(this.gameObject);
    }

    public void SwitchToCharacter(int index)
    {
        if (index < 0 || index >= characters.Length) return;

        Vector3 prevPos = currentCharacter.transform.localPosition;
        Quaternion prevRot = currentCharacter.transform.localRotation;

        if (currentCharacter != null)
        {
            currentCharacter.OnCharacterDeselected();
            currentCharacter.gameObject.SetActive(false);
        }

        currentCharacterIndex = index;
        currentCharacter = characters[index];

        if (currentCharacter is RodionController)
        {
            styleManager?.SwitchToRodionStyle();
        }
        else if (currentCharacter is FinaController)
        {
            styleManager?.SwitchToFinaStyle();
        }

        currentCharacter.transform.localPosition = prevPos;
        currentCharacter.transform.localRotation = prevRot;
        currentCharacter.gameObject.SetActive(true);
        currentCharacter.OnCharacterSelected();

        cameraFollow?.SetTarget(currentCharacter.transform);

        //Debug.Log($"Переключено на: {currentCharacter.name}");
    }
}