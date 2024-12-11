using UnityEngine;

public class CreditsPanelController : MonoBehaviour
{
    public GameObject teamCreditsPanel;
    public GameObject assetCreditsPanel;
    public GameObject optionsPanel;

    // Show the Team Credits panel
    public void ShowTeamCredits()
    {
        // Ensure only the Team Credits panel is active
        optionsPanel.SetActive(false);
        teamCreditsPanel.SetActive(true);
        assetCreditsPanel.SetActive(false);
    }

    // Show the Asset Credits panel
    public void ShowAssetCredits()
    {
        // Ensure only the Asset Credits panel is active
        optionsPanel.SetActive(false);
        assetCreditsPanel.SetActive(true);
        teamCreditsPanel.SetActive(false);
    }

    // Hide all credits panels and return to Options
    public void HideCreditsPanels()
    {
        // Disable credits panels
        teamCreditsPanel.SetActive(false);
        assetCreditsPanel.SetActive(false);

        // Re-enable the Options panel
        optionsPanel.SetActive(true);
    }
}
