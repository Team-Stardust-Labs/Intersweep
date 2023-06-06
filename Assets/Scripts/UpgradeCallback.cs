using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCallback : MonoBehaviour
{
    // Unified return value for all upgrades

    Status status = new Status();
    string message = "";

    public UpgradeCallback() { }

    public UpgradeCallback(Status _status, string _message)
    {
        status = _status;
        message = _message;
    }

    // getters
    public Status getStatus()
    {
        return status;
    }

    public string getMessage()
    {
        return message;
    }

    // setters
    public void setStatus(Status _status)
    {
        status = _status;
    }

    public void setMessage(string _message)
    {
        message = _message;
    }

}
