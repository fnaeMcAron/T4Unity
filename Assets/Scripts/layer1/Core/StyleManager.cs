using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    public float resourceCostModifier = 1f;
}

public class StyleManager : MonoBehaviour
{
    public static StyleManager Instance { get; private set; }

    public enum StyleMode { Rodion, Fina, None }

    [Header("Настройки стиля Родиона (положительные значения)")]
    public List<StyleLevel> rodionStyleLevels = new List<StyleLevel>();

    [Header("Настройки стиля Фины (отрицательные значения)")]
    public List<StyleLevel> finaStyleLevels = new List<StyleLevel>();

    [Header("Общие настройки")]
    public StyleMode currentStyleMode = StyleMode.None;
    public int currentStylePoints = 0;
    public float styleDecayRate = 100f;
    public float styleDecayDelay = 3f;

    [Header("UI элементы")]
    public TMP_Text stylePointsText;
    public Image rodionStyleImage;
    public Image finaStyleImage;
    public GameObject styleUI;

    [Header("Визуальные эффекты")]
    public ParticleSystem styleParticles;
    public Light styleLight;
    public Camera cam;
    public RenderTexture[] resolutionRenderTextures;
    public RawImage resolutionTexture;
    public GameObject bandicam;

    StyleLevel currentRodionLevel;
    StyleLevel currentFinaLevel;
    float timeSinceLastAction = 0f;
    float elapsed = 0f;
    bool isDecayActive = false;
    bool isInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            InitializeStyleSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeStyleSystem()
    {
        if (isInitialized) return;

        // уровни Родиона (от 0 до int)
        if (rodionStyleLevels.Count == 0)
        {
            rodionStyleLevels = new List<StyleLevel>
            {
                new StyleLevel {
                    levelName = "1080p",
                    pointsRequired = 50,
                    styleColor = Color.white,
                    resourceCostModifier = 1.0f
                },
                new StyleLevel {
                    levelName = "4K",
                    pointsRequired = 100,
                    styleColor = Color.cyan,
                    resourceCostModifier = 0.95f
                },
                new StyleLevel {
                    levelName = "16K",
                    pointsRequired = 200,
                    styleColor = Color.green,
                    resourceCostModifier = 0.9f
                },
                new StyleLevel {
                    levelName = "64K",
                    pointsRequired = 400,
                    styleColor = Color.yellow,
                    resourceCostModifier = 0.8f
                },
                new StyleLevel {
                    levelName = "666K",
                    pointsRequired = 800,
                    styleColor = Color.red,
                    resourceCostModifier = 0.5f
                }
            };
        }

        // уровни Фины (от -int до 0)
        if (finaStyleLevels.Count == 0)
        {
            finaStyleLevels = new List<StyleLevel>
            {
                new StyleLevel {
                    levelName = "Pathetic...",
                    pointsRequired = -50,
                    damageMultiplier = 1.1f,
                    styleColor = Color.gray,
                },
                new StyleLevel {
                    levelName = "Is that all?",
                    pointsRequired = -100,
                    damageMultiplier = 1.2f,
                    styleColor = Color.blue,
                },
                new StyleLevel {
                    levelName = "Try harder.",
                    pointsRequired = -200,
                    damageMultiplier = 1.4f,
                    styleColor = Color.green,
                },
                new StyleLevel {
                    levelName = "Good boy~",
                    pointsRequired = -400,
                    damageMultiplier = 1.7f,
                    styleColor = Color.magenta,
                },
                new StyleLevel {
                    levelName = "Yes, my master~",
                    pointsRequired = -800,
                    damageMultiplier = 2f,
                    styleColor = Color.red,
                }
            };
        }

        UpdateStyleLevels();
        isInitialized = true;
    }

    void Update()
    {
        if (!isDecayActive) return;

        timeSinceLastAction += Time.deltaTime;

        if (timeSinceLastAction >= styleDecayDelay)
        {
            switch (currentStyleMode)
            {
                case StyleMode.Rodion:
                    if (currentStylePoints > 0)
                    {
                        currentStylePoints = Mathf.Max(0, currentStylePoints - (int)(styleDecayRate * Time.deltaTime));
                        UpdateRodionStyleLevel();
                    }
                    break;

                case StyleMode.Fina:
                    if (currentStylePoints < 0)
                    {
                        currentStylePoints = Mathf.Min(0, currentStylePoints + (int)(styleDecayRate * Time.deltaTime));
                        UpdateFinaStyleLevel();
                    }
                    break;
            }
            UpdateUI();
        }
        UpdateResolutionEffect();
    }

    public void AddStylePoints(int points, string actionName = "")
    {
        switch (currentStyleMode)
        {
            case StyleMode.Rodion:
                currentStylePoints = currentStylePoints + points;
                UpdateRodionStyleLevel();
                break;

            case StyleMode.Fina:
                currentStylePoints = currentStylePoints - points;
                UpdateFinaStyleLevel();
                break;

            case StyleMode.None:
                return;
        }

        timeSinceLastAction = 0f;
        isDecayActive = true;

        if (!string.IsNullOrEmpty(actionName))
        {
            Debug.Log($"{currentStyleMode}: {points} за '{actionName}'");
        }
        UpdateUI();
    }

    public void SwitchToRodionStyle()
    {
        if (currentStyleMode == StyleMode.Rodion) return;

        currentStyleMode = StyleMode.Rodion;

        if (styleUI != null) styleUI.SetActive(true);

        UpdateRodionStyleLevel();
        UpdateUI();
    }

    public void SwitchToFinaStyle()
    {
        if (currentStyleMode == StyleMode.Fina) return;

        ResetCurrentMode();
        currentStyleMode = StyleMode.Fina;

        if (styleUI != null) styleUI.SetActive(true);

        UpdateFinaStyleLevel();
        UpdateUI();
    }

    private void ResetCurrentMode()
    {
        currentStylePoints = 0;

        switch (currentStyleMode)
        {
            case StyleMode.Rodion:
                UpdateRodionStyleLevel();
                break;

            case StyleMode.Fina:
                UpdateFinaStyleLevel();
                break;
        }
    }

    private void UpdateRodionStyleLevel()
    {
        StyleLevel newLevel = rodionStyleLevels[0];

        for (int i = rodionStyleLevels.Count - 1; i >= 0; i--)
        {
            if (currentStylePoints >= rodionStyleLevels[i].pointsRequired)
            {
                newLevel = rodionStyleLevels[i];
                break;
            }
        }

        if (currentRodionLevel != newLevel)
        {
            currentRodionLevel = newLevel;
            OnStyleLevelChanged();
        }
    }

    private void UpdateResolutionEffect()
    {
        switch (CurrentStyleLevel.levelName)
        {
            case "1080p":
                //стандартное 1920x1080
                cam.targetTexture = resolutionRenderTextures[0];
                resolutionTexture.texture = resolutionRenderTextures[0];
                Application.targetFrameRate = 60;
                bandicam.SetActive(false);
                break;
            case "4K":
                cam.targetTexture = resolutionRenderTextures[1];
                resolutionTexture.texture = resolutionRenderTextures[1];
                Application.targetFrameRate = 30;
                bandicam.SetActive(false);
                break;
            case "8K":
                cam.targetTexture = resolutionRenderTextures[2];
                resolutionTexture.texture = resolutionRenderTextures[2];
                Application.targetFrameRate = 24;
                bandicam.SetActive(false);
                break;
            case "64K":
                cam.targetTexture = resolutionRenderTextures[3];
                resolutionTexture.texture = resolutionRenderTextures[3];
                Application.targetFrameRate = 15;
                break;
            case "666K":
                cam.targetTexture = resolutionRenderTextures[4];
                resolutionTexture.texture = resolutionRenderTextures[4];
                bandicam.SetActive(true);
                Application.targetFrameRate = 6;
                break;
            default:
                cam.targetTexture = resolutionRenderTextures[0];
                resolutionTexture.texture = resolutionRenderTextures[0];
                Application.targetFrameRate = 60;
                bandicam.SetActive(false);
                break;
        }
    }

    private void UpdateFinaStyleLevel()
    {
        StyleLevel newLevel = finaStyleLevels[0];

        for (int i = finaStyleLevels.Count - 1; i >= 0; i--)
        {
            if (currentStylePoints <= finaStyleLevels[i].pointsRequired)
            {
                newLevel = finaStyleLevels[i];
                break;
            }
        }

        if (currentFinaLevel != newLevel)
        {
            currentFinaLevel = newLevel;
            OnStyleLevelChanged();
        }
    }

    private void UpdateStyleLevels()
    {
        UpdateRodionStyleLevel();
        UpdateFinaStyleLevel();
    }

    private void OnStyleLevelChanged()
    {
        // todo визуальные эффекты здесь потом добавить и нармальна
        if (styleParticles != null)
        {
            var main = styleParticles.main;
            main.startColor = GetCurrentStyleColor();
        }

        if (styleLight != null)
        {
            styleLight.color = GetCurrentStyleColor();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (stylePointsText != null)
        {
            switch (currentStyleMode)
            {
                case StyleMode.Rodion:
                    rodionStyleImage.enabled = true;
                    finaStyleImage.enabled = false;
                    stylePointsText.text = $"{currentStylePoints}";
                    break;

                case StyleMode.Fina:
                    rodionStyleImage.enabled = false;
                    finaStyleImage.enabled = true;
                    stylePointsText.text = $"{Mathf.Abs(currentStylePoints)}";
                    break;

                case StyleMode.None:
                    rodionStyleImage.enabled = false;
                    finaStyleImage.enabled = true;
                    stylePointsText.text = "потом уберу";
                    stylePointsText.color = Color.gray;
                    break;
            }
        }

        if (rodionStyleImage != null)
        {
            switch (currentStyleMode)
            {
                case StyleMode.Rodion:
                    if (currentRodionLevel?.styleImage != null)
                    {
                        rodionStyleImage.sprite = currentRodionLevel.styleImage; //todo плавное изменение координат по X от 200 до -100 (на 300) через цикл и deltatime
                    }
                    break;

                case StyleMode.Fina:
                    if (currentFinaLevel?.styleImage != null)
                    {
                        finaStyleImage.sprite = currentFinaLevel.styleImage; //todo плавное изменение координат по Y от 850 до 250 (на 600) через цикл и deltatime
                    }
                    break;
            }
        }
    }

    public void ResetAllStyles()
    {
        currentStylePoints = 0;
        currentStyleMode = StyleMode.None;
        timeSinceLastAction = 0f;
        isDecayActive = false;

        if (styleUI != null) styleUI.SetActive(false);

        UpdateStyleLevels();
        UpdateUI();
    }

    // геттеры для других систем
    public float GetCurrentDamageMultiplier()
    {
        return currentStyleMode switch
        {
            StyleMode.Rodion => currentRodionLevel?.damageMultiplier ?? 1f,
            StyleMode.Fina => currentFinaLevel?.damageMultiplier ?? 1f,
            _ => 1f
        };
    }

    public float GetCurrentResourceCostModifier()
    {
        return currentStyleMode switch
        {
            StyleMode.Rodion => currentRodionLevel?.resourceCostModifier ?? 1f,
            StyleMode.Fina => currentFinaLevel?.resourceCostModifier ?? 1f,
            _ => 1f
        };
    }

    public string GetCurrentStyleLevelName()
    {
        return currentStyleMode switch
        {
            StyleMode.Rodion => currentRodionLevel?.levelName ?? "None",
            StyleMode.Fina => currentFinaLevel?.levelName ?? "None",
            _ => "None"
        };
    }

    public Color GetCurrentStyleColor()
    {
        return currentStyleMode switch
        {
            StyleMode.Rodion => currentRodionLevel?.styleColor ?? Color.white,
            StyleMode.Fina => currentFinaLevel?.styleColor ?? Color.white,
            _ => Color.gray
        };
    }

    public bool IsStyleActive()
    {
        return currentStyleMode != StyleMode.None;
    }

    public StyleLevel CurrentStyleLevel
    {
        get
        {
            return currentStyleMode switch
            {
                StyleMode.Rodion => currentRodionLevel,
                StyleMode.Fina => currentFinaLevel,
                _ => null
            };
        }
    }
}