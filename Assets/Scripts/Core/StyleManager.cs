using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

[System.Serializable]
public class StyleLevel
{
    public string levelName;
    public int pointsRequired;
    public float damageMultiplier = 1f;
    public float moveSpeedBonus = 0f;
    public Color styleColor = Color.white;
    public GameObject visualEffect;
    public Sprite styleImage;
}

public class StyleManager : MonoBehaviour
{
    public static StyleManager Instance { get; private set; }

    [Header("Настройки стиля")]
    public List<StyleLevel> styleLevels = new List<StyleLevel>();
    public int currentStylePoints = 0;
    public StyleLevel currentStyleLevel { get; private set; }
    public float styleDecayRate = 100f; // Потеря очков стиля в секунду
    public float styleDecayDelay = 3f; // Задержка перед началом распада

    [Header("Визуальные эффекты")]
    public ParticleSystem styleParticles;
    public Light styleLight;
    public TMP_Text pointsText;
    public Image image;

    private float timeSinceLastAction = 0f;
    private bool isDecayActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStyleSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeStyleSystem()
    {
        if (styleLevels.Count == 0)
        {
            styleLevels = new List<StyleLevel>
            {
                
                new StyleLevel { levelName = "D", pointsRequired = 50, damageMultiplier = 1.1f, styleColor = Color.gray },
                new StyleLevel { levelName = "C", pointsRequired = 100, damageMultiplier = 1.2f, styleColor = Color.blue },
                new StyleLevel { levelName = "B", pointsRequired = 200, damageMultiplier = 1.4f, styleColor = Color.green },
                new StyleLevel { levelName = "A", pointsRequired = 300, damageMultiplier = 1.7f, styleColor = Color.yellow },
                new StyleLevel { levelName = "S", pointsRequired = 600, damageMultiplier = 2.0f, styleColor = Color.red }
            };
        }

        UpdateStyleLevel();
    }

    void Update()
    {
        if (isDecayActive && currentStylePoints > 0)
        {
            timeSinceLastAction += Time.deltaTime;

            if (timeSinceLastAction >= styleDecayDelay)
            {
                // Постепенная потеря очков стиля
                currentStylePoints = Mathf.Max(0, currentStylePoints - (int)(styleDecayRate * Time.deltaTime) * 10);
                UpdateStyleLevel();
            }
        }
        pointsText.text = currentStylePoints.ToString();
    }

    public void AddStylePoints(int points, string actionName = "")
    {
        currentStylePoints += points;
        timeSinceLastAction = 0f;
        isDecayActive = true;

        StyleLevel newLevel = UpdateStyleLevel();

        if (!string.IsNullOrEmpty(actionName))
        {
            Debug.Log($"Стиль +{points} за '{actionName}'. Уровень: {newLevel.levelName}");
        }

        // Визуальная обратная связь
        PlayStyleGainEffect(points);
    }

    private StyleLevel UpdateStyleLevel()
    {
        StyleLevel newLevel = styleLevels[0];

        // Находим текущий уровень стиля
        for (int i = styleLevels.Count - 1; i >= 0; i--)
        {
            if (currentStylePoints >= styleLevels[i].pointsRequired)
            {
                newLevel = styleLevels[i];
                break;
            }
        }

        // Если уровень изменился
        if (currentStyleLevel != newLevel)
        {
            currentStyleLevel = newLevel;
            OnStyleLevelChanged();
        }

        return newLevel;
    }

    private void OnStyleLevelChanged()
    {
        Debug.Log($"Уровень стиля изменен на: {currentStyleLevel.levelName}");

        // Визуальные эффекты смены уровня
        if (styleParticles != null)
        {
            var main = styleParticles.main;
            main.startColor = currentStyleLevel.styleColor;
        }

        if (styleLight != null)
        {
            styleLight.color = currentStyleLevel.styleColor;
        }

        if (currentStyleLevel.styleImage != null)
        {
            image.sprite = currentStyleLevel.styleImage;
        }

        Debug.Log(currentStyleLevel.levelName);
        // TO DO: звуковые эффекты, UI оповещения
    }

    private void PlayStyleGainEffect(int points)
    {
        // Визуальная обратная связь при получении очков стиля
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Quad);
        effect.transform.position = transform.position + Vector3.up * 2f;
        //effect.GetComponent<Renderer>().material.color = currentStyleLevel.styleColor;

        Destroy(effect, 2f);
    }

    public void ResetStyle()
    {
        currentStylePoints = 0;
        timeSinceLastAction = 0f;
        isDecayActive = false;
        UpdateStyleLevel();
    }

    public float GetDamageMultiplier()
    {
        return currentStyleLevel?.damageMultiplier ?? 1f;
    }

    public float GetMoveSpeedBonus()
    {
        return currentStyleLevel?.moveSpeedBonus ?? 0f;
    }
}