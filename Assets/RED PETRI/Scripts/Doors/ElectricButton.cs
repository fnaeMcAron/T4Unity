using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricButton : MonoBehaviour, IElectricSensitive
{
    public GameObject door;

    public void OnElectricEnter(GameObject source)
    {
        door.gameObject.SetActive(false);
    }

    public void OnElectricExit(GameObject source) { }
}
