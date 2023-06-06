using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearUpgrade : MonoBehaviour
{
    /*
     * Linear Upgrades have a base cost and an upgrade
     * cost that scales linearly up by the upgrade level 
     */

    double baseCost = 0.0;
    double upgradeCost = 0.0;
    uint upgradeLevel = 0;
    Resource upgradeResource = new Resource();

    public LinearUpgrade(double _baseCost, double _upgradeCost,
                         uint _upgradeLevel, Resource _upgradeResource)
    {

        baseCost = _baseCost;
        upgradeCost = _upgradeCost;
        upgradeLevel = _upgradeLevel;
        upgradeResource = _upgradeResource;

    }


    public void requestUpgrade(Resource resource)
    {

        double requiredAmount = baseCost + (upgradeCost * upgradeLevel);
        if (resource.getAmount() >= requiredAmount)
        {
            upgradeLevel++;
            resource.setAmount(resource.getAmount() - requiredAmount);
            addToLog("Neues Sammlerschiff gebaut.");
        }

    }
    
 }
