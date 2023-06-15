/*
   GameManager (top-level manager)
   - Responsible for initializing the game, managing game flow, and coordinating interactions between different components.
   - Delegates specific responsibilities to other classes.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{



    // Variables
    public bool game_running = true;
    bool infinite_mode = false;
    int target_frame_rate = 60;

    // Manager References
    public UIManager UI;
    public TimeManager timeManager;
    public ResourceManager resources;

    private void Start()
    {
        QualitySettings.vSyncCount = 2;                     // enable vsync to counter screen tearing
        Application.targetFrameRate = target_frame_rate;    // the game dosen't need to run at 5000 fps so cap it reasonably
        Screen.SetResolution(1920, 1080, true);             // the UI is not fully responsive so set it to a widely accepted standard resolution
    
        UI.updateRecyclingUI();
        UI.setWinScreen(false);
        UI.setLoseScreen(false);

        UI.pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        //checks for Escape presses to pause/resume gameplay
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UI.pauseMenuUI.activeSelf)
            {
                resume();
            }
            else
            {
                pause();
            }
        }

    }
    void FixedUpdate()
    {

        double DELTA = timeManager.getDelta();

        // check for game running, if not discard further method calls
        if (!game_running) { return; }

        // Update Calls
        resources.updateResources(DELTA);
        resources.updateWaste(DELTA);
        resources.updateConsumption(DELTA);
        resources.updateUpgrades();

        // Game Objectives
        checkGameEnd();
    }


    // check for Win or Lose 
    void checkGameEnd()
    {

        // dont check for win if in infinite mode
        if (infinite_mode)
            return;

        // Win ( All Waste collected )
        if (resources.getWaste() <= 0.0)
        {
            print("You Win!");
            game_running = false;
            UI.printStatistics();
            UI.setWinScreen(true);
        }
        // Lose ( Fuel empty )
        else if (resources.getFuel() <= 0.0)
        {
            print("You Lose.");
            game_running = false;
            UI.printStatistics();
            UI.setLoseScreen(true);
        }
    }

    //pauses gameplay and opens pause menu
    public void pause()
    {
        game_running = false;
        UI.toggleInGamePause();
    }

    //closes pause menu and resumes gameplay
    public void resume()
    {
        game_running = true;
        UI.toggleInGamePause();
    }
}