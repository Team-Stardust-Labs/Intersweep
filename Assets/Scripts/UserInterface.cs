using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
