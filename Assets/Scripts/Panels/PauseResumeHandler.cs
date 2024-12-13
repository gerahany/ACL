using UnityEngine;using System.Collections;
using UnityEngine.SceneManagement;

public class PauseResumeHandler : MonoBehaviour
{
    public GameObject resumePanel;  // The resume panel UI
    public GameObject mainPanel;   // The main game UI panel
    public GameObject panels;      // The Panels GameObject (UI)
    public Camera mainCamera;      // Main camera for gameplay
    private bool isPaused = false; // Track the pause state

    void Start()
    {
        // Ensure the resume panel and Panels are hidden at the start
        //f (resumePanel != null) resumePanel.SetActive(false);
        if (panels != null) panels.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);

        // Set isPaused to false initially
        isPaused = false;

        // Log initial states
        Debug.Log($"PauseResumeHandler initialized. Panels hidden. isPaused: {isPaused}");
    }

    void Update()
    {
        // Listen for the ESC key press to toggle pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed. Current isPaused state: " + isPaused);

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        Debug.Log("Pausing the game...");

        // Set time scale to 0 to pause gameplay
        Time.timeScale = 0;
        isPaused = true;

        // Activate pause-related UI
        if (resumePanel != null)
        {
            resumePanel.SetActive(true);
            Debug.Log("Resume panel activated.");
        }
        if (panels != null)
        {
            panels.SetActive(true);
            Debug.Log("Panels activated.");
        }
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
            Debug.Log("Main panel deactivated.");
        }
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming the game...");

        // Set time scale back to 1 to resume gameplay
        Time.timeScale = 1;
        isPaused = false;

        // Deactivate pause-related UI
        if (resumePanel != null)
        {
            resumePanel.SetActive(false);
            Debug.Log("Resume panel deactivated.");
        }
        if (panels != null)
        {
            panels.SetActive(false);
            Debug.Log("Panels deactivated.");
        }
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
            Debug.Log("Main panel activated.");
        }
    }public void RestartLevel()
    {
        Debug.Log("Restarting the level...");

        // Get the selected character from PlayerPrefs
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "default");

        // Close all UI elements
        if (resumePanel != null)
        {
            resumePanel.SetActive(false);
            Debug.Log("Resume panel deactivated.");
        }
        if (panels != null)
        {
            panels.SetActive(false);
            Debug.Log("Panels deactivated.");
        }
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
            Debug.Log("Main panel deactivated.");
        }

        // Start the scene reload process
        StartCoroutine(ReloadScenesAndActivateCharacter(selectedCharacter));
    }

    private IEnumerator ReloadScenesAndActivateCharacter(string selectedCharacter)
    {
        // Unload the 'Panels' scene if it is already loaded
        AsyncOperation unloadPanels = SceneManager.UnloadSceneAsync("Panels");
        yield return unloadPanels;

        Debug.Log("Panels scene unloaded");

        // Load the 'newScene' and 'Panels' scenes
        AsyncOperation loadNewScene = SceneManager.LoadSceneAsync("newScene", LoadSceneMode.Additive);
        AsyncOperation loadPanels = SceneManager.LoadSceneAsync("Panels", LoadSceneMode.Additive);

        // Wait for both scenes to load
        yield return loadNewScene;
        yield return loadPanels;

        Debug.Log("newScene and Panels scenes loaded");

        // Ensure the correct scene is active (newScene should be active now)
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("newScene"));
        Debug.Log("newScene set as active scene");

        // After scenes are loaded, activate the selected character in 'newScene'
        ActivatePlayerInScene("newScene", selectedCharacter);
    }

    private void ActivatePlayerInScene(string sceneName, string characterName)
    {
        // Ensure the scene is active or properly referenced
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            Debug.LogError($"Scene '{sceneName}' is not loaded!");
            return;
        }

        // Set the scene to active (if needed)
        SceneManager.SetActiveScene(scene);

        // Find the "Characters" parent GameObject in the active scene
        GameObject characterParent = GameObject.Find("Characters");
        if (characterParent == null)
        {
            Debug.LogError("Parent 'Characters' object not found in the scene!");
            return;
        }

        // Deactivate all characters within the "Characters" parent
        foreach (Transform child in characterParent.transform)
        {
            child.gameObject.SetActive(false);
        }

        // Activate the selected character by its name (case-insensitive)
        GameObject selectedPlayer = characterParent.transform.Find(characterName.ToLower())?.gameObject;
        if (selectedPlayer != null)
        {
            selectedPlayer.SetActive(true);
            Debug.Log($"{characterName} activated in scene {sceneName}");
        }
        else
        {
            Debug.LogError($"{characterName} not found under 'Characters' object!");
        }

        // Find the "Footers" GameObject in the scene and activate it
        GameObject footers = GameObject.Find("Footers");
        if (footers != null)
        {
            footers.SetActive(true);
            Debug.Log("Footers activated in the scene.");
        }
        else
        {
            Debug.LogError("Footers object not found in the scene!");
        }
    }
}
