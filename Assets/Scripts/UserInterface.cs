using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{

    // UI References
    string statsText = "";
    public TextMeshProUGUI statisticsText;
    public TextMeshProUGUI logText;
    public Slider recyclingCopperSlider;
    public Slider recyclingIronSlider;
    public Slider recyclingMetalsSlider;
    public TextMeshProUGUI recyclingCopperLabel;
    public TextMeshProUGUI recyclingIronLabel;
    public TextMeshProUGUI recyclingMetalsLabel;
    public GameObject winScreenUI;
    public GameObject loseScreenUI;
    public Slider shipProgress;
    public Slider shipCapacityProgress;
    public Slider solarProgress;
    public Slider spaceStationProgress;

    // UI update rate
    public double refreshRate = 2.0;			// how often per second the UI is updated

    // Time reference
    TimeManager GameTime = new TimeManager();

    // manual production
    bool canProduceManual = true;


    public UserInterface(TimeManager time)
    {
        GameTime = time;
    }

    // Start is called before the first frame update
    void Start()
    {
        showWinScreen(false);
        showLoseScreen(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        double oneSecond = ((1.0 / refreshRate) * GameTime.getTimeScale());

        // UI Update
        if (GameTime.getElapsedTimeDaily() > oneSecond)
        {
            GameTime.setElapsedTimeDaily(GameTime.getElapsedTimeDaily() - oneSecond); // reset elapsed time

            //printStatistics();
            //refreshUIButtons();
        }

        // manual production update nerf
        if (GameTime.getElapsedTimeManualProduction() > 0.2) // 5x per second is max to prevent autoclickers
        {
            canProduceManual = true;
            GameTime.resetElapsedTimeManualProduction();
        }
    }


    // Functions
    public void showWinScreen(bool active)
    {
        winScreenUI.SetActive(active);
    }

    public void showLoseScreen(bool active)
    {
        loseScreenUI.SetActive(active);
    }
}
