using UnityEngine;

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

    void ResumeGame()
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
    }
}
