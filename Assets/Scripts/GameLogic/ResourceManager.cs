/*
   ResourceManager
   - Manages resources (copper, iron, metals) and their income/production rates.
   - Updates resource values based on various factors (recycling, solar panels, manual production) and provides methods to access resource data.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    /*
     * The Resources could be in a standalone class but we
     * decided to leave them separate as there is no plan
     * to add further resources and this makes the code more
     * readable and easy to understand.
     * 
     * also setting these all to public could create protection
     * issues but I will just ignore these today because getters
     * and setters are a pain to work with.
     */


    // resources
    public double copper = 0.0;
    public double iron = 0.0;
    public double metals = 0.0;

    // resource processing income per second
    public readonly double base_income_copper = 7.0;
    public readonly double base_income_iron = 5.0;
    public readonly double base_income_metals = 0.35;

    // resource income palceholders [ used mostly intern and for display on UI ]
    public double income_copper = 0.0;
    public double income_iron = 0.0;
    public double income_metals = 0.0;

    // resource recycling energy management values
    public float recycling_copper = 0.5f;
    public float recycling_iron = 0.3f;
    public float recycling_metals = 0.2f;


    // solar panels
    public uint solar_count = 1;
    public uint max_solar_count = 10;
    public double solar_energy_generation = 1.0;                // 1 kW * solar count / day

    public readonly double solar_base_cost = 100.0;             // how much copper the first solar panel costs
    public readonly double solar_upgrade_cost = 100.0;          // how much additional copper per ship the next one costs

    // sector
    public uint sector = 1;
    public readonly uint max_sector = 20;

    // ships
    public double ship_capacity = 0.0;
    public int ship_count = 10;

    // ship upgrades
    public int ship_capacity_level = 0;
    public readonly double[] ship_capacity_upgrade_metals = { 10, 30, 50, 80, 120, 140, 170, 180, 200, 220, 250, 280, 300, 400, 500 };
    public readonly double[] ship_capacity_levels = { 1.0, 1.8, 2.3, 2.5, 3.0, 3.5, 3.8, 4.3, 4.7, 5.0, 5.5, 6.0, 7.5, 9.0, 12.0, 15.0 }; // has to be 1 element bigger than *_*_upgrade_metals

    public readonly double ship_base_cost = 100.0;              // how much iron the first new ship costs
    public readonly double ship_upgrade_cost = 20.0;            // how much additional iron per ship the next one costs

    // space station
    public uint space_station_level = 0;                        // affects max solar count and fuel ration on next sector
    public readonly double space_station_base_cost = 1000.0;    // copper
    public readonly double space_station_upgrade_cost = 1500.0;

    // waste
    public double waste = 20000;                       // waste 20 000 to fuel 10 000 is good combo for 10min gameplay
    public double waste_collection = 1.0;

    // fuel
    public double fuel = 10000.0;                       // 5000 makes for a real challenge

    // fuel consumption
    public double consumption_space_station = 2.0;
    public double consumption_collector_ships = 0.5;
    public double consumption_processing = 1.0;
    public double consumption_general = 0.0;        // all consumption stats summed up

    // manual production
    public readonly float manual_production_effiency = 0.5f;

    public void updateResources(double DELTA)
    {
        // update income based on recycling values and energy available
        double energy = solar_count * solar_energy_generation; // calculate total energy

        // income = recycling slider value * energy * base values
        income_copper = recycling_copper * energy * base_income_copper;
        income_iron = recycling_iron * energy * base_income_iron;
        income_metals = recycling_metals * energy * base_income_metals;

        // resource income
        copper += income_copper * DELTA;
        iron += income_iron * DELTA;
        metals += income_metals * DELTA;
    }


    // compute consumption and recalculate fuel
    public void updateConsumption(double DELTA)
    {

        consumption_general = (consumption_collector_ships * ship_count) + consumption_processing + consumption_space_station;
        fuel -= consumption_general * DELTA;
        if (fuel < 0.0)
            fuel = 0.0;
    }


    // compute the waste collected from ships every day and recalculate remaining waste
    public void updateWaste(double DELTA)
    {
        waste_collection = ship_count * ship_capacity;
        waste -= waste_collection * DELTA;
        if (waste < 0.0) // clamp waste at 0
            waste = 0.0;
    }

    public void updateUpgrades()
    {
        // Ship capacity
        if (ship_capacity_level < ship_capacity_levels.Length)
            ship_capacity = ship_capacity_levels[ship_capacity_level];
    }


    // Overcomplicated validation of recycling values

    public void validateRecyclingValues(int current_stat = 0)
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
    }

    public void validateRecyclingCopperValue(float value)
    {
        // check if all sliders combined stay under 100%
        float sliderValues = (value + recycling_iron + recycling_metals);
        if (sliderValues > 1.0)
        {
            float deltaValue = (sliderValues - 1.0f) / 2.0f;
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
    }

    public void validateRecyclingIronValue(float value)
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
    }

    public void validateRecyclingMetalsValue(float value)
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
    }

    public double getWaste()
    {
        return waste;
    }

    public double getFuel()
    {
        return fuel;
    }

}
