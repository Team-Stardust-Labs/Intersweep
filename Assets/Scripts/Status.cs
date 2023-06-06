using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    // Statuses for exception handling
    public enum STATUS_CODE {
        OK,
        ERROR,
        UNDEFINED
    }

    STATUS_CODE status = STATUS_CODE.UNDEFINED;

    public Status() {}

    public Status(STATUS_CODE _status)
    {
        status = _status;
    }

    public STATUS_CODE getStatus()
    {
        return status;
    }

    public void setStatus(STATUS_CODE _status)
    {
        status = _status;
    }

}
