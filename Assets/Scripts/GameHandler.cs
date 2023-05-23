using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class GameHandler : MonoBehaviour
{

	/* IDEAS
	 * 
	 * UPGRADE instead of showing more numbers, lay progress bars behind the
	 * upgrade and build buttons in the UI that fill up if you have the resource
	 * with a small icon beside it.
	 * 
	 */

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
    double refreshRate = 2.0;			// how often per second the UI is updated


	// time
	int    elapsed_days = 0;
	double elapsed_time_manual_production = 0.0;
	double elapsed_time_daily = 0.0;    // intern
	double elapsed_time = 0.0;
	double time_scale = 1.0;            // how many days per second

	// general
	bool game_running = true;
	bool infinite_mode = false;
	int targetFrameRate = 60;
    bool printDebugLogs = false;
    float manual_production_effiency = 0.5f;
	bool can_produce_manual = true;

    // sector
    uint sector = 1;
	const uint max_sector = 20;

	// resources
	double copper = 0.0;
	double iron = 0.0;
	double metals = 0.0;

	// resource processing income per second
	const double base_income_copper = 7.0;
	const double base_income_iron = 5.0;
	const double base_income_metals = 0.35;

	// resource income palceholders [ intern ]
	double income_copper = 0.0;
	double income_iron = 0.0;
	double income_metals = 0.0;

	// resource recycling energy management values
	float recycling_copper = 0.5f;
	float recycling_iron = 0.3f;
	float recycling_metals = 0.2f;

	// solar panels
	uint solar_count = 1;
	uint max_solar_count = 10;
	double solar_energy_generation = 1.0;   // 1 kW * solar count / day

	double solar_base_cost = 100.0;				// how much copper the first solar panel costs
	double solar_upgrade_cost = 100.0;			// how much additional copper per ship the next one costs

	// ships
	double ship_capacity = 0.0;
	int    ship_count = 10;

	// ship upgrades
	int ship_capacity_level = 0;
	double[] ship_capacity_upgrade_metals = {10, 30, 50, 80, 120, 140, 170, 180, 200, 220, 250, 280, 300, 400, 500}; 
	double[] ship_capacity_levels = { 1.0, 1.8, 2.3, 2.5, 3.0, 3.5, 3.8, 4.3, 4.7, 5.0, 5.5, 6.0, 7.5, 9.0, 12.0, 15.0}; // has to be 1 element bigger than *_*_upgrade_metals

	double ship_base_cost = 100.0;				// how much iron the first new ship costs
	double ship_upgrade_cost = 20.0;            // how much additional iron per ship the next one costs

	// space station
	uint space_station_level = 0;				// affects max solar count and fuel ration on next sector
	double space_station_base_cost = 1000.0;	// copper
	double space_station_upgrade_cost = 1500.0;

	// waste
	double waste = 20000;						// waste 20 000 to fuel 10 000 is good combo for 10min gameplay
	double waste_collection = 1.0;          

	// fuel
	double fuel = 5000.0;

	// fuel consumption
	double consumption_space_station = 2.0;
	double consumption_collector_ships = 0.5;
	double consumption_processing = 1.0;
	double consumption_general = 0.0;        // all consumption stats summed up

	// storage
	double storage = 0.0;

	private void Start()
	{
		QualitySettings.vSyncCount = 2;
		Application.targetFrameRate = targetFrameRate;
		Screen.SetResolution(1920, 1080, true);

		updateRecyclingUI();

		winScreenUI.SetActive(false);
		loseScreenUI.SetActive(false);
	}

	void FixedUpdate()
	{
		double DELTA = (double) Time.deltaTime * time_scale;

		// check for game running
		if (!game_running)
		{
			return;
		}

		// updates
		updateResources(DELTA);
		updateWaste(DELTA);
		updateConsumption(DELTA);
		updateUpgrades();

		// Time Management
		elapsed_time_daily += DELTA;
		elapsed_time += DELTA;
		elapsed_time_manual_production += Time.deltaTime; // real delta time for manual production
		elapsed_days = Mathf.FloorToInt((float) elapsed_time);

		// Game Objectives
		checkGameEnd();

		// UI Update
		if (elapsed_time_daily > (1.0 / refreshRate) * time_scale) {

			elapsed_time_daily -= (1.0 / refreshRate) * time_scale;    // reset elapsed time

			printStatistics();
			refreshUIButtons();
		}

		// manual production update nerf
		if (elapsed_time_manual_production > 0.2) // 5x per second is max to prevent autoclickers
		{
			can_produce_manual = true;
			elapsed_time_manual_production = 0.0;
		}


	}

	void updateResources(double DELTA)
	{
		// update income based on recycling values and energy available
		double energy = solar_count * solar_energy_generation; // calculate total energy

		// income = recycling slider value * energy * base values
		income_copper = recycling_copper * energy * base_income_copper;
		income_iron = recycling_iron * energy * base_income_iron;
		income_metals = recycling_metals * energy * base_income_metals;

		// resource income
		copper  += income_copper    * DELTA;
		iron    += income_iron      * DELTA;
		metals  += income_metals    * DELTA;
	}


	// compute consumption and recalculate fuel
	void updateConsumption(double DELTA)
	{
		
		consumption_general = (consumption_collector_ships * ship_count) + consumption_processing + consumption_space_station;
		fuel -= consumption_general * DELTA;
		if (fuel < 0.0)
			fuel = 0.0;
	}

	// compute the waste collected from ships every day and recalculate remaining waste
	void updateWaste(double DELTA)
	{
		waste_collection = ship_count * ship_capacity;
		waste -= waste_collection * DELTA;
		if (waste < 0.0) // clamp waste at 0
			waste = 0.0;

		// update storage
		storage += waste_collection * DELTA;

    }

	// check for win and lose 
	void checkGameEnd()
	{

		// dont check for win if infinite mode
		if (infinite_mode)
			return;

		// Win ( Waste collected )
		if (waste <= 0.0)
		{
			print("You Win!");
			game_running = false;
			printStatistics();
			winScreenUI.SetActive(true);
		}
		// Lose ( Fuel empty )
		else if (fuel <= 0.0)
		{
			print("You Lose.");
			game_running = false;
			printStatistics();
			loseScreenUI.SetActive(true);
		}
	}

	void updateUpgrades()
	{
		// Ship capacity
		if (ship_capacity_level < ship_capacity_levels.Length)
			ship_capacity = ship_capacity_levels[ship_capacity_level];
	}

    // ##########################
    // ##     UI TRIGGERS      ##
    // ##########################

    public void requestBuildShip()
    {
        double required_iron = ship_base_cost + (ship_upgrade_cost * ship_count);
        if (iron >= required_iron)
        {
            ship_count++;
            iron -= required_iron;
            addToLog("Neues Sammlerschiff gebaut.");
        }
        else
        {
            addToLog("Nicht genügend Eisen! Du brauchst " + required_iron);
        }
    }

    public void requestUpgradeShips()
	{
		double required_metals = 0.0;
		if (ship_capacity_level < ship_capacity_upgrade_metals.Length)
			required_metals = ship_capacity_upgrade_metals[ship_capacity_level];
		else
		{
			addToLog("Schiffkapazität vollständig geupgraded", false);
			return;
		}

		if (metals >= required_metals)
		{
			ship_capacity_level++;
			metals -= required_metals;
			addToLog("Schiffkapazität geupgraded auf Level " + ship_capacity_level);
		}
		else
		{
			addToLog("Nicht genug Metall! Du brauchst " + required_metals);
		}
	}

	public void requestBuildSolarPanel()
	{
        double required_copper = solar_base_cost + (solar_upgrade_cost * solar_count);

		if (solar_count+1 > max_solar_count)
		{
			addToLog("Maximale Anzahl an Solarzellen bereits erreicht. Upgrade die Raumstation.");
			return;
		}

        if (copper >= required_copper)
        {
            solar_count++;
            copper -= required_copper;
            addToLog("Neue Solarzelle gebaut.");
        }
        else
        {
            addToLog("Nicht genügend Kupfer! Du brauchst " + required_copper);
        }
    }

	public void requestSpaceStationUpgrade()
    {
        double required_copper = space_station_base_cost + (space_station_upgrade_cost * space_station_level);

        if (copper >= required_copper)
        {
            space_station_level++;
			max_solar_count = 10 + (10 * space_station_level);
            copper -= required_copper;
            addToLog("Raumstation geupgraded!");
        }
        else
        {
            addToLog("Nicht genügend Kupfer! Du brauchst " + required_copper);
        }
    }

	// Manual Production
	public void requestManualCopper()
	{
		if (!can_produce_manual)
			return;
		copper += base_income_copper * manual_production_effiency * (space_station_level + 1) * (space_station_level + 1);
        can_produce_manual = false;
    }

    public void requestManualIron()
    {
        if (!can_produce_manual)
            return;
        iron += base_income_iron * manual_production_effiency * (space_station_level + 1) * (space_station_level + 1);
        can_produce_manual = false;
    }

    public void requestManualMetals()
    {
        if (!can_produce_manual)
            return;
		metals += base_income_metals * manual_production_effiency * (space_station_level + 1) * (space_station_level + 1);
        can_produce_manual = false;
	}

    // Recycling Energy Management Sliders

    public void setRecyclingValueCopper(float value)
	{
		// check if all sliders combined stay under 100%
		float sliderValues = (value + recycling_iron + recycling_metals);
		if (sliderValues > 1.0)
		{
			float deltaValue =  (sliderValues - 1.0f) / 2.0f;
			if (recycling_iron - deltaValue >= 0.0 && recycling_metals - deltaValue >= 0.0)
			{
				recycling_iron -= deltaValue;
				recycling_metals -= deltaValue;
			}
			else if (recycling_iron - (2.0f * deltaValue) >= 0.0)
			{
				recycling_iron -= 2.0f * deltaValue;
			}
			else
			{
				recycling_metals -= 2.0f * deltaValue;
			}
			
		}
		recycling_copper = value;
		addToLog(recycling_copper.ToString(), true);
		updateRecyclingUI(0);
	}

	public void setRecyclingValueIron(float value)
	{
		// check if all sliders combined stay under 100%
		float sliderValues = (recycling_copper + value + recycling_metals);
		if (sliderValues > 1.0)
		{
			float deltaValue = (sliderValues - 1.0f) / 2.0f;
			if (recycling_copper - deltaValue >= 0.0 && recycling_metals - deltaValue >= 0.0)
			{
				recycling_copper -= deltaValue;
				recycling_metals -= deltaValue;
			}
			else if (recycling_copper - (2.0f * deltaValue) >= 0.0)
			{
				recycling_copper -= 2.0f * deltaValue;
			}
			else
			{
				recycling_metals -= 2.0f * deltaValue;
			}
		}
		recycling_iron = value;
		addToLog(recycling_iron.ToString(), true);
		updateRecyclingUI(1);
	}

	public void setRecyclingValueMetals(float value)
	{
		// check if all sliders combined stay under 100%
		float sliderValues = (recycling_copper + recycling_iron + value);
		if (sliderValues > 1.0)
		{
			float deltaValue = (sliderValues - 1.0f) / 2.0f;
			if (recycling_iron - deltaValue >= 0.0 && recycling_copper - deltaValue >= 0.0)
			{
				recycling_iron -= deltaValue;
				recycling_copper -= deltaValue;
			}
			else if (recycling_iron - (2.0f * deltaValue) >= 0.0)
			{
				recycling_iron -= 2.0f * deltaValue;
			}
			else
			{
				recycling_copper -= 2.0f * deltaValue;
			}
		}
		recycling_metals = value;
		addToLog(recycling_metals.ToString(), true);
		updateRecyclingUI(2);
	}

	void updateRecyclingUI(int current_stat = 0)
	{
		// clamp values
		recycling_copper = Mathf.Clamp(recycling_copper, 0.0f, 1.0f);
		recycling_iron = Mathf.Clamp(recycling_iron, 0.0f, 1.0f);
		recycling_metals = Mathf.Clamp(recycling_metals, 0.0f, 1.0f);

		// check if values are over max
		float valuesCombined = (recycling_copper + recycling_iron + recycling_metals);
		if (valuesCombined > 1.0)
		{
			float deltaValue = valuesCombined - 1.0f;
			// find biggest stat and subtract from it
			if (recycling_copper > (recycling_iron + recycling_metals))
				recycling_copper -= deltaValue;
			else if (recycling_iron > (recycling_copper + recycling_metals))
				recycling_iron -= deltaValue;
			else
				recycling_metals -= deltaValue;
		}

		// check if not all energy is used, then find lowest value and add it
		// current stat argument 0 = copper, 1 = iron, 2 = metals
		valuesCombined = (recycling_copper + recycling_iron + recycling_metals);
		if (valuesCombined < 1.0)
		{
			float deltaValue = 1.0f - valuesCombined;
			if (recycling_copper <= (recycling_iron + recycling_metals) && current_stat != 0)
				recycling_copper += deltaValue;
			else if (recycling_iron <= (recycling_copper + recycling_metals) && current_stat != 1)
				recycling_iron += deltaValue;
			else
				recycling_metals += deltaValue;
		}

		// SLIDERS
		recyclingCopperSlider.value = recycling_copper;
		recyclingIronSlider.value = recycling_iron;
		recyclingMetalsSlider.value = recycling_metals;

		// LABELS
		recyclingCopperLabel.text = "Kupfer\n"      + (Mathf.Ceil(recycling_copper * 100.0f)).ToString() + "%";
		recyclingIronLabel.text = "Eisen\n"         + (Mathf.Floor(recycling_iron * 100.0f)).ToString() + "%";
		recyclingMetalsLabel.text = "Edelmetalle\n" + (Mathf.Floor(recycling_metals * 100.0f)).ToString() + "%";

	}


	// ##########################
	// ##    USER INTERFACE    ##
	// ##########################

	// this function adds the statistics to the UI
	void printStatistics()
	{
		statsText = "";

		// time info
		addStatText("----- GENERAL", true);
		addStatText("DAY: " + elapsed_days);
		addStatText("SECTOR: " + sector + " / " + max_sector);
        addStatText("RAUMSTATIONMODULE: " + space_station_level);
        // print resources
        addStatText("----- RESOURCES", true);
		addStatText("KUPFER: \t\t"        + string.Format("{0:000.00} | {1:00.00}", copper, income_copper));
		addStatText("EISEN: \t\t"         + string.Format("{0:000.00} | {1:00.00}", iron, income_iron));
		addStatText("EDELMETALLE: \t"     + string.Format("{0:000.00} | {1:00.00}", metals, income_metals));
        // print energy
        addStatText("----- ENERGY", true);
        addStatText("SOLARZELLEN: \t" + string.Format("{0} / {1}", solar_count, max_solar_count));
        addStatText("SOLAREFFIZIENZ: \t" + string.Format("{0:000.00}", solar_energy_generation));
        addStatText("STROMGENERATION: \t" + string.Format("{0:000.00}", solar_count * solar_energy_generation));
        // print fuel and waste
        addStatText("----- WASTE", true);
		addStatText("SCHROTT: \t\t"       + string.Format("{0:0.00}", waste));
		addStatText("SCHROTTSAMMLUNG: \t"   + string.Format("{0:0.00}", waste_collection));
        addStatText("LAGER: \t\t" + string.Format("{0:0.00}", storage));
        // print fuel consumption
        addStatText("----- CONSUMPTION", true);
		addStatText("KRAFTSTOFF: \t"      + string.Format("{0:0.00}", fuel));
		addStatText("SCHIFFE: \t\t"       + string.Format("{0:0.00}", consumption_collector_ships));
		addStatText("RAUMSTATION: \t"     + string.Format("{0:0.00}", consumption_space_station));
		addStatText("VERARBEITUNG: \t"      + string.Format("{0:0.00}", consumption_processing));
		// print ship stats
		addStatText("----- SHIPS", true);
		addStatText("SCHIFFE: \t\t"       + ship_count);
		addStatText("KAPAZITÄT: \t"       + ship_capacity);

		applyToStatUI();
	}


	// this function applies the statistics to the user interface
	void applyToStatUI()
	{
		statisticsText.text = statsText;
	}

	// this function refreshes the Button text on the UI
	void refreshUIButtons()
	{
		// update buy ship progress
		shipProgress.value = Mathf.Clamp((float)(iron / (ship_base_cost + (ship_upgrade_cost * ship_count))), 0.0f, 1.0f);

		// update upgrade ship capacity progress
		shipCapacityProgress.value = Mathf.Clamp((float) (metals / (ship_capacity_upgrade_metals[ship_capacity_level])), 0.0f, 1.0f);

        // update buy solar panel progress
        solarProgress.value = Mathf.Clamp((float)(copper / (solar_base_cost + (solar_upgrade_cost * solar_count))), 0.0f, 1.0f);

        // update upgrade solar panel progress
        spaceStationProgress.value = Mathf.Clamp((float)(copper / (space_station_base_cost + (space_station_upgrade_cost * space_station_level))), 0.0f, 1.0f);
    }

	// adds the string s to the statsText variable for debug UI
	void addStatText(string s, bool category = false)
	{
		if (category)
			statsText += "\n";
		statsText += s + "\n";
	}

	// debug Log
	void addToLog(string s, bool debug = false)
	{
		if ((debug == true && printDebugLogs == true) || (debug == false))
		{
			logText.text = logText.text.Insert(0, ">> " + s + "\n");
		}
	}

}