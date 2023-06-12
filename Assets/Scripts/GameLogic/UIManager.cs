/*
   UIManager
   - Handles UI elements, updates UI based on game events, and receives user input.
   - Manages references to UI elements (e.g., TextMeshProUGUI, Sliders) and updates them accordingly.
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{


    // User Interface References
    public string statsText = "";
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
    public AudioSource winSound;
    public AudioSource loseSound;
    public Slider shipProgress;
    public Slider shipCapacityProgress;
    public Slider solarProgress;
    public Slider spaceStationProgress;
    public GameObject pauseMenuUI;

    public bool print_debug_logs = false;

    // Manager References
    public TimeManager timeManager;
    public ResourceManager resources;

    // shows or hides the Win Screen
    public void setWinScreen(bool active)
    {
        winScreenUI.SetActive(active);

        if(active)
        {
            winSound.Play();
        }
        
    }

    // shows or hides the Lose Screen
    public void setLoseScreen(bool active)
    {
        loseScreenUI.SetActive(active);

        if(active)
        {
            loseSound.Play();
        }
    }

    // applies the statistics to the user interface
    public void applyToStatUI()
    {
        statisticsText.text = statsText;
    }

    // refreshes the Button text on the UI
    public void refreshUIButtons()
    {
        // update buy ship progress
        shipProgress.value = Mathf.Clamp((float)(resources.iron / (resources.ship_base_cost + (resources.ship_upgrade_cost * resources.ship_count))), 0.0f, 1.0f);

        // update upgrade ship capacity progress
        shipCapacityProgress.value = Mathf.Clamp((float)(resources.metals / (resources.ship_capacity_upgrade_metals[resources.ship_capacity_level])), 0.0f, 1.0f);

        // update buy solar panel progress
        solarProgress.value = Mathf.Clamp((float)(resources.copper / (resources.solar_base_cost + (resources.solar_upgrade_cost * resources.solar_count))), 0.0f, 1.0f);

        // update upgrade solar panel progress
        spaceStationProgress.value = Mathf.Clamp((float)(resources.copper / (resources.space_station_base_cost + (resources.space_station_upgrade_cost * resources.space_station_level))), 0.0f, 1.0f);
    }

    // adds the string s to the statsText variable for debug UI
    public void addStatText(string s, bool category = false)
    {
        if (category)
            statsText += "\n";
        statsText += s + "\n";
    }

    // debug Log
    public void addToLog(string s, bool debug = false)
    {
        if ((debug == true && print_debug_logs == true) || (debug == false))
        {
            logText.text = logText.text.Insert(0, ">> " + s + "\n");
        }
    }


    // this function adds the statistics to the UI
    public void printStatistics()
    {
        statsText = "";

        // time info
        addStatText("----- GENERAL", true);
        addStatText("DAY: " + timeManager.elapsed_days);
        addStatText("SECTOR: " + resources.sector + " / " + resources.max_sector);
        addStatText("RAUMSTATIONMODULE: " + resources.space_station_level);
        // print resources
        addStatText("----- RESOURCES", true);
        addStatText("KUPFER: \t\t" + string.Format("{0:000.00} | {1:00.00}", resources.copper, resources.income_copper));
        addStatText("EISEN: \t\t" + string.Format("{0:000.00} | {1:00.00}", resources.iron, resources.income_iron));
        addStatText("EDELMETALLE: \t" + string.Format("{0:000.00} | {1:00.00}", resources.metals, resources.income_metals));
        // print energy
        addStatText("----- ENERGY", true);
        addStatText("SOLARZELLEN: \t" + string.Format("{0} / {1}", resources.solar_count, resources.max_solar_count));
        addStatText("SOLAREFFIZIENZ: \t" + string.Format("{0:000.00}", resources.solar_energy_generation));
        addStatText("STROMGENERATION: \t" + string.Format("{0:000.00}", resources.solar_count * resources.solar_energy_generation));
        // print fuel and waste
        addStatText("----- WASTE", true);
        addStatText("SCHROTT: \t\t" + string.Format("{0:0.00}", resources.waste));
        addStatText("SCHROTTSAMMLUNG: \t" + string.Format("{0:0.00}", resources.waste_collection));
        // print fuel consumption
        addStatText("----- CONSUMPTION", true);
        addStatText("KRAFTSTOFF: \t" + string.Format("{0:0.00}", resources.fuel));
        addStatText("SCHIFFE: \t\t" + string.Format("{0:0.00}", resources.consumption_collector_ships));
        addStatText("RAUMSTATION: \t" + string.Format("{0:0.00}", resources.consumption_space_station));
        addStatText("VERARBEITUNG: \t" + string.Format("{0:0.00}", resources.consumption_processing));
        // print ship stats
        addStatText("----- SHIPS", true);
        addStatText("SCHIFFE: \t\t" + resources.ship_count);
        addStatText("KAPAZITÄT: \t" + resources.ship_capacity);

        applyToStatUI();
    }


    public void updateRecyclingUI(int current_stat = 0)
    {
        resources.validateRecyclingValues(current_stat);

        // SLIDERS
        recyclingCopperSlider.value = resources.recycling_copper;
        recyclingIronSlider.value = resources.recycling_iron;
        recyclingMetalsSlider.value = resources.recycling_metals;

        // LABELS
        recyclingCopperLabel.text = (Mathf.Ceil(resources.recycling_copper * 100.0f)).ToString() + "%";
        recyclingIronLabel.text = (Mathf.Floor(resources.recycling_iron * 100.0f)).ToString() + "%";
        recyclingMetalsLabel.text = (Mathf.Floor(resources.recycling_metals * 100.0f)).ToString() + "%";

    }


    // Recycling Energy Management Sliders ( yes it would make more sense in a class, sliders are complicated )

    public void setRecyclingValueCopper(float value)
    {
        resources.validateRecyclingCopperValue(value);
        addToLog(resources.recycling_copper.ToString(), true);
        updateRecyclingUI(0); // 0 is for copper
    }

    public void setRecyclingValueIron(float value)
    {
        resources.validateRecyclingIronValue(value);
        addToLog(resources.recycling_iron.ToString(), true);
        updateRecyclingUI(1); // 1 is for iron
    }

    public void setRecyclingValueMetals(float value)
    {
        resources.validateRecyclingMetalsValue(value);
        addToLog(resources.recycling_metals.ToString(), true);
        updateRecyclingUI(2); // 2 is for metals
    }

    public void toggleInGamePause()
    {
        if (pauseMenuUI.activeSelf)
        {
            pauseMenuUI.SetActive(false);
        } 
        else
        {
            pauseMenuUI.SetActive(true);
        }
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
