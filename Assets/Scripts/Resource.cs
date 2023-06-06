using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{

    string displayName = "Resource";    // what the resource is called in the UI
    double amount = 0.0;                // the amount of the resource you got
    double scarcity = 0.5;              // the scarcity of the resource
    double recyclingPriority = 0.1;     // the recycling priority set with the sliders
    double produced = 0.0;              // the amount which is produced every time unit



    public Resource() {}

    public Resource(string _displayName, double _amount, double _scarcity, double _recyclingPriority)
    {
        displayName = _displayName;
        amount = _amount;
        scarcity = _scarcity;
        recyclingPriority = _recyclingPriority;
    }


    // Getters
    public double getAmount()
    {
        return amount;
    }


    // Setters

    public void setAmount(double _amount)
    {
        amount = _amount;
    }

    public void setProduction(double _amount)
    {
        amount = _amount;
    }



    // Functions


    public void updateProduction(double deltaTime, double energy)
    {
        produced = scarcity * recyclingPriority * energy;
        amount += produced * deltaTime;
    }
    
}
