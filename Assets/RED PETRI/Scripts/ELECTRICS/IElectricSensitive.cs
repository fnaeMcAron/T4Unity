using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IElectricSensitive
{
    public void OnElectricEnter(GameObject source);
    public void OnElectricExit(GameObject source);
}
