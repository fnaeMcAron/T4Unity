using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public GameObject DDistance;
    public GameObject DFirst;

    [Header("Настройки интеракции")]
    [SerializeField] bool isDestroy;
    public UnityEvent onInteractEvent;

    void Start()
    {
        DeactivateDebug();
    }

    public void DrawGUI(float distance, bool first)
    {
        DDistance.SetActive(true);
        DDistance.GetComponent<TMP_Text>().text = distance.ToString();
        if (first)
        {
            DFirst.SetActive(true);
        }
        else
        {
            DFirst.SetActive(false);
        }
    }

    public void DeactivateDebug()
    {
        DDistance.SetActive(false);
        DFirst.SetActive(false);
    }

    public void OnInteract()
    {
        onInteractEvent?.Invoke();
        if (isDestroy)
            Destroy(this.gameObject);
    }
}
