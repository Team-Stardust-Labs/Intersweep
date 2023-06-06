using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionHandler : MonoBehaviour
{
    const int LOG_CACHE = 32;                // how many messages the log caches
    string[] messageLog = new string[LOG_CACHE];



    public void log(string message, bool verbose = false)
    {
        // this adds a message to the log and prints it to the console
        // if verbose is true, it 'should' be printed ingame.


        // FIXME: implement the actual log cache




        Debug.Log(message);

    }

}
