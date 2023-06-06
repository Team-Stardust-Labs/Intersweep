using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Upgrades : MonoBehaviour
{

    LinearUpgrade ShipUpgrade = new LinearUpgrade(100.0, 100.0, 1, iron);
    static ExceptionHandler upgradeExceptions = new ExceptionHandler();


    public void requestBuildShipIron(Resource iron)
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

        if (solar_count + 1 > max_solar_count)
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
}
