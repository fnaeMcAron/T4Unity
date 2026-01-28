using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine;

public class WMWindow : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] RectTransform windowRect;


    public void OnClose()
    {
        Destroy(window);
    }
    public void OnMaximaze()
    {
        print(windowRect.sizeDelta);
    }
    public void OnMinimaze()
    {
        window.SetActive(false);
    }
    public void OnDrag()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
