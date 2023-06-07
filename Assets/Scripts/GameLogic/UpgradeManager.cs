/*
   UpgradeManager
   - Handles upgrades for ships, ship capacity, solar panels, and space stations.
   - Manages upgrade costs, levels, and progression.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    // Manager References
    public TimeManager timeManager;
    public ResourceManager resources;
    public UIManager UI;

    public void requestBuildShip()
    {
        double required_iron = resources.ship_base_cost + (resources.ship_upgrade_cost * resources.ship_count);
        if (resources.iron >= required_iron)
        {
            resources.ship_count++;
            resources.iron -= required_iron;
            UI.addToLog("Neues Sammlerschiff gebaut.");
        }
        else
        {
            UI.addToLog("Nicht genügend Eisen! Du brauchst " + required_iron);
        }
    }

    public void requestUpgradeShips()
    {
        double required_metals = 0.0;
        if (resources.ship_capacity_level < resources.ship_capacity_upgrade_metals.Length)
            required_metals = resources.ship_capacity_upgrade_metals[resources.ship_capacity_level];
        else
        {
            UI.addToLog("Schiffkapazität vollständig geupgraded", false);
            return;
        }

        if (resources.metals >= required_metals)
        {
            resources.ship_capacity_level++;
            resources.metals -= required_metals;
            UI.addToLog("Schiffkapazität geupgraded auf Level " + resources.ship_capacity_level);
        }
        else
        {
            UI.addToLog("Nicht genug Metall! Du brauchst " + required_metals);
        }
    }

    public void requestBuildSolarPanel()
    {
        double required_copper = resources.solar_base_cost + (resources.solar_upgrade_cost * resources.solar_count);

        if (resources.solar_count + 1 > resources.max_solar_count)
        {
            UI.addToLog("Maximale Anzahl an Solarzellen bereits erreicht. Upgrade die Raumstation.");
            return;
        }

        if (resources.copper >= required_copper)
        {
            resources.solar_count++;
            resources.copper -= required_copper;
            UI.addToLog("Neue Solarzelle gebaut.");
        }
        else
        {
            UI.addToLog("Nicht genügend Kupfer! Du brauchst " + required_copper);
        }
    }

    public void requestSpaceStationUpgrade()
    {
        double required_copper = resources.space_station_base_cost + (resources.space_station_upgrade_cost * resources.space_station_level);

        if (resources.copper >= required_copper)
        {
            resources.space_station_level++;
            resources.max_solar_count = 10 + (10 * resources.space_station_level);
            resources.copper -= required_copper;
            UI.addToLog("Raumstation geupgraded!");
        }
        else
        {
            UI.addToLog("Nicht genügend Kupfer! Du brauchst " + required_copper);
        }
    }

    // Manual Production
    public void requestManualCopper()
    {
        if (!timeManager.can_produce_manual)
            return;
        resources.copper += resources.base_income_copper * resources.manual_production_effiency * (resources.space_station_level + 1) * (resources.space_station_level + 1);
        timeManager.can_produce_manual = false;
    }

    public void requestManualIron()
    {
        if (!timeManager.can_produce_manual)
            return;
        resources.iron += resources.base_income_iron * resources.manual_production_effiency * (resources.space_station_level + 1) * (resources.space_station_level + 1);
        timeManager.can_produce_manual = false;
    }

    public void requestManualMetals()
    {
        if (!timeManager.can_produce_manual)
            return;
        resources.metals += resources.base_income_metals * resources.manual_production_effiency * (resources.space_station_level + 1) * (resources.space_station_level + 1);
        timeManager.can_produce_manual = false;
    }
}
