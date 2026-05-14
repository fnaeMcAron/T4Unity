using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class RadialMenu : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] float radius = 150f;

    [Header("Input")]
    [SerializeField] InputActionReference pointAction;
    [SerializeField] InputActionReference navigateAction;

    RectTransform rect;
    List<RectTransform> items = new();
    int currentIndex = -1;

    float angleStep;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        items.Clear();
        foreach (Transform child in transform)
            items.Add(child as RectTransform);

        Rebuild();
    }

    void OnEnable()
    {
        pointAction.action.Enable();
        navigateAction.action.Enable();
    }

    void OnDisable()
    {
        pointAction.action.Disable();
        navigateAction.action.Disable();
    }

    public void Rebuild()
    {
        int count = items.Count;
        if (count == 0) return;

        angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            items[i].anchoredPosition = pos;
        }
    }

    void Update()
    {
        Vector2 dir = GetInputDirection();
        if (dir.sqrMagnitude < 0.01f)
        {
            SetIndex(-1);
            return;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        int index = Mathf.FloorToInt(angle / angleStep);
        SetIndex(index);
    }

    Vector2 GetInputDirection()
    {
        // Геймпад приоритетнее
        Vector2 stick = navigateAction.action.ReadValue<Vector2>();
        if (stick.sqrMagnitude > 0.01f)
            return stick.normalized;

        // Мышь
        Vector2 mousePos = pointAction.action.ReadValue<Vector2>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            mousePos,
            null,
            out Vector2 localMouse
        );

        return localMouse.normalized;
    }

    void SetIndex(int index)
    {
        if (currentIndex == index) return;

        currentIndex = index;

        for (int i = 0; i < items.Count; i++)
        {
            items[i].localScale = (i == index)
                ? Vector3.one * 1.2f
                : Vector3.one;
        }
    }

    public int GetSelectedIndex() => currentIndex;
}
