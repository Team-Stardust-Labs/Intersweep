/*
   TimeManager
   - Tracks elapsed time, manages time-related calculations, and triggers events based on time intervals.
   - Handles the refresh rate for UI updates and other time-dependent game mechanics.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Variables
    public int    elapsed_days = 0;              // how many ingame-days have elapsed
    double elapsed_time_manual_production = 0.0; // elapsed time units since last `manual production`
    double elapsed_time_daily = 0.0;             // elapsed time units per day
    double elapsed_time = 0.0;                   // elapsed time units since scene start
    double time_scale = 1.0;                     // how many days per real second
    double refresh_rate = 2.0;                   // how often per second the UI is updated
    double DELTA = 0.0;                          // time since last frame
    public bool   can_produce_manual = true;     // if `manual production` can be used

    // Manager References
    public UIManager UI = new UIManager();

    private void FixedUpdate()
    {
        DELTA = (double)Time.deltaTime * time_scale;

        // Time Management
        elapsed_time_daily += DELTA;
        elapsed_time += DELTA;
        elapsed_time_manual_production += Time.deltaTime;               // this uses unscaled delta time to capture this in real time units
        elapsed_days = Mathf.FloorToInt( (float) elapsed_time);

        // UI Update
        if (elapsed_time_daily > (1.0 / refresh_rate) * time_scale)
        {

            elapsed_time_daily -= (1.0 / refresh_rate) * time_scale;    // reset daily elapsed time

            UI.printStatistics();
            UI.refreshUIButtons();
        }

        // Manual production nerf
        if (elapsed_time_manual_production > 0.2)                       // 5x per second is max to prevent autoclickers
        {
            can_produce_manual = true;
            elapsed_time_manual_production = 0.0;
        }
    }

    public double getDelta()
    {
        return DELTA;
    }
}
