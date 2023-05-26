using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{

    uint solarPanelCount = 1;
    double solarPanelCapacity = 1.0;


    public double getEnergy()
    {
        return solarPanelCount * solarPanelCapacity;
    }
}
