using UnityEngine;using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI;  // For NavMesh related classes, including NavMeshSurface


public class PauseResumeHandler : MonoBehaviour
{
    public GameObject resumePanel;  // The resume panel UI
    public GameObject mainPanel;   // The main game UI panel
    public GameObject panels;      // The Panels GameObject (UI)
    public GameObject gameAudio; 
    public GameObject sorcererPlayer ;
     public   GameObject barbarianPlayer ;
     public   GameObject roguePlayer ;
    public Camera mainCamera;      // Main camera for gameplay
    private bool isPaused = false; // Track the pause state
    public AudioSource backgroundAudioSource;
    private string selectedCharacter;
    

void Start()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
    // Ensure the character is activated properly after the scene has loaded
    if (PlayerPrefs.GetInt("isRestart", 0) == 1)
    {
        Time.timeScale = 1;
        Debug.Log("Scene is restarting...");
        PlayerPrefs.SetInt("isRestart", 0); // Reset restart flag
        PlayerPrefs.Save(); // Save PlayerPrefs
        
        // Deactivate panels and activate character after restart
        if (panels != null)
        {
            panels.SetActive(false);  // Deactivate Panels UI
            Debug.Log("Panels deactivated.");
        }
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", ""); // Get the selected character from PlayerPrefs
  Debug.Log("slected");
  Debug.Log(selectedCharacter);
    // Based on the selected character, activate the corresponding player in the scene
    if (!string.IsNullOrEmpty(selectedCharacter))
{
    switch (selectedCharacter.ToLower())
    {
        case "sorcerer":
            if (sorcererPlayer != null) 
            {
                sorcererPlayer.SetActive(true);
                
            }
            break;
        case "barbarian":
            if (barbarianPlayer != null) 
            {
                barbarianPlayer.SetActive(true);
                
            }
            break;
        case "rouge":
            if (roguePlayer != null) 
            {
                roguePlayer.SetActive(true);
      
            }
            break;
    }
    if (sorcererPlayer != null)
    {
        NavMeshAgent agent = sorcererPlayer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // Disable the agent temporarily
            agent.enabled = true;  // Re-enable it to reset movement
            Debug.Log("NavMeshAgent reinitialized for Sorcerer.");
        }
    }
    if (barbarianPlayer != null)
    {
        NavMeshAgent agent = barbarianPlayer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // Disable the agent temporarily
            agent.enabled = true;  // Re-enable it to reset movement
            Debug.Log("NavMeshAgent reinitialized for barb.");
        }
    }
     if (roguePlayer != null)
    {
        NavMeshAgent agent = roguePlayer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // Disable the agent temporarily
            agent.enabled = true;  // Re-enable it to reset movement
            Debug.Log("NavMeshAgent reinitialized for rouge.");
        }
        if (Input.GetMouseButtonDown(0)) // Left mouse click
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Ensure the ray is hitting the ground and moving the agent
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Ground clicked at: " + hit.point);
           
            if (agent != null)
            {
                agent.SetDestination(hit.point);
            }
        }
    }
    }
}

        // Wait for the scene to load completely before activating the character
        
    }
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // Ensure this is only called once
    SceneManager.sceneLoaded -= OnSceneLoaded;

    // Ensure the NavMesh Agent is properly reinitialized after scene reload
    if (sorcererPlayer != null)
    {
        NavMeshAgent agent = sorcererPlayer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // Disable the agent temporarily
            agent.enabled = true;  // Re-enable it to reset movement
            Debug.Log("NavMeshAgent reinitialized for Sorcerer.");
        }
    }
    if (barbarianPlayer != null)
    {
        NavMeshAgent agent = barbarianPlayer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // Disable the agent temporarily
            agent.enabled = true;  // Re-enable it to reset movement
            Debug.Log("NavMeshAgent reinitialized for barb.");
        }
    }
     if (roguePlayer != null)
    {
        NavMeshAgent agent = roguePlayer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // Disable the agent temporarily
            agent.enabled = true;  // Re-enable it to reset movement
            Debug.Log("NavMeshAgent reinitialized for rouge.");
        }
    }
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
    public void Quit()
    {
        string currentScene = SceneManager.GetActiveScene().name;

    // Reload the current scene
    SceneManager.LoadScene(currentScene);

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
        if (gameAudio != null)
        {
            gameAudio.SetActive(false);
            Debug.Log("gameAudio activated.");
        }
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
            Debug.Log("Main panel deactivated.");
        }
        backgroundAudioSource.Pause();
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming the game...");

        // Set time scale back to 1 to resume gameplay
       
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
         if (gameAudio != null)
        {
            gameAudio.SetActive(true);
            Debug.Log("gameAudio activated.");
        }
        backgroundAudioSource.UnPause();
    }
  public void RestartLevel()
{
    // Set the restart flag to true before reloading the scene
    PlayerPrefs.SetInt("isRestart", 1);
    PlayerPrefs.Save();  // Make sure changes are saved
    
    // Get the current scene's name
    string currentScene = SceneManager.GetActiveScene().name;

    // Reload the current scene
    SceneManager.LoadScene(currentScene);
}
}
