using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel; // Reference to the Main Menu Panel
    public GameObject optionsPanel; // Reference to the Options Panel

    public GameObject charactersPanel;

    public void PlayNewGame()
    {
        // Load the Character Class Screen
        //SceneManager.LoadScene("CharacterClassScene");
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        charactersPanel.SetActive(true);
    }

    public void QuitGame()
{
    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the Editor
    #else
    Application.Quit(); // Quits the built game
    #endif
    Debug.Log("Game Quit");
}


    public void ShowOptionsPanel()
    {
        // Show the Options Panel and hide the Main Menu Panel
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        charactersPanel.SetActive(false);
    }

    public void HideOptionsPanel()
    {
        // Show the Main Menu Panel and hide the Options Panel
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        charactersPanel.SetActive(false);
    }
}
