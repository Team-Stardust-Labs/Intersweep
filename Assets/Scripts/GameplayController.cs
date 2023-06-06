using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameplayController : MonoBehaviour
{


    // Time management object
    static TimeManager GameTime = new TimeManager();

    UserInterface UI = new UserInterface(GameTime);
    Energy energy = new Energy();



    // create a resource collection to hold all resources in game
    // FIXME: damn shit dosent work.
    //ResourceCollection resz = new ResourceCollection();

    // initialize resources
    Resource copper = new Resource("Kupfer", 0.0, 7.0, 0.5);
    Resource iron = new Resource("Eisen", 0.0, 5.0, 0.3);
    Resource metals = new Resource("Edelmetalle", 0.0, 0.5, 0.2);

    Dictionary<string, int> resourceCollection = new Dictionary<string, int>();

    
    resourceCollection.Add("copper", 1);


}
