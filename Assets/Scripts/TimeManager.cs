using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    // Variables
    int    elapsedDays = 0;
    double elapsedTimeManualProduction = 0.0;
    double elapsedTimeDaily = 0.0;
    double elapsedTime = 0.0;

    double timeScale = 1.0;

    double DELTA = 0.0;

    // Delegates for update Triggers
    // delegate void newUpdate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        DELTA = (double)Time.deltaTime * timeScale;             // scaled value of deltaTime

        elapsedTimeDaily += DELTA;
        elapsedTime      += DELTA;
        elapsedTimeManualProduction += Time.deltaTime;          // real deltaTime for manual production ( independent of timeScale )
        elapsedDays = Mathf.FloorToInt((float) elapsedTime);
    }

    // Getters

    public int getElapsedDays()
    {
        return elapsedDays;
    }

    public double getElapsedTime()
    {
        return elapsedTime;
    }

    public double getElapsedTimeDaily()
    {
        return elapsedTimeDaily;
    }

    public double getElapsedTimeManualProduction()
    {
        return elapsedTimeManualProduction;
    }

    public double deltaTime()
    {
        return DELTA;
    }


    // Setters

    public void resetElapsedTimeManualProduction()
    {
        elapsedTimeManualProduction = 0;
    }

}
