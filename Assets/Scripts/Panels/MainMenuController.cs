using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel; // Reference to the Main Menu Panel
    public GameObject optionsPanel; // Reference to the Options Panel

    public void PlayNewGame()
    {
        // Load the Character Class Screen
        SceneManager.LoadScene("CharacterClassScene");
    }

    public void QuitGame()
    {
        // Exit the game
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void ShowOptionsPanel()
    {
        // Show the Options Panel and hide the Main Menu Panel
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void HideOptionsPanel()
    {
        // Show the Main Menu Panel and hide the Options Panel
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }
}
