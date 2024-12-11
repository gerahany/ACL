using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionHandler : MonoBehaviour
{
    public GameObject sorcererDetailPanel;
    public GameObject barbarianDetailPanel;
    public GameObject rougeDetailPanel;
    public GameObject characterClassPanel;
    public GameObject confirmButton;

    public Button sorcererButton;
    public Button barbarianButton;
    public Button rougeButton;

    public Color defaultColor = Color.white;
    public Color highlightColor = Color.green;

    private string selectedCharacter;
    private Button currentSelectedButton;

    void Start()
    {
        selectedCharacter = null;
        confirmButton.SetActive(false);
        ResetButtonColors();
        Debug.Log("Initialization complete");
    }

    private void ResetButtonColors()
    {
        sorcererButton.image.color = defaultColor;
        barbarianButton.image.color = defaultColor;
        rougeButton.image.color = defaultColor;
        Debug.Log("Button colors reset");
    }

    private void HighlightSelectedButton()
    {
        if (currentSelectedButton != null)
        {
            currentSelectedButton.image.color = highlightColor;
        }
    }

    private void SelectCharacter(Button selectedButton, GameObject detailPanel, string characterName)
    {
        ResetButtonColors();
        selectedButton.image.color = highlightColor;
        currentSelectedButton = selectedButton;

        // Hide all panels
        sorcererDetailPanel.SetActive(false);
        barbarianDetailPanel.SetActive(false);
        rougeDetailPanel.SetActive(false);

        // Hide the main panel and show the detail panel
        characterClassPanel.SetActive(false);
        detailPanel.SetActive(true);

        selectedCharacter = characterName;
        confirmButton.SetActive(true);

        Debug.Log($"Character selected: {characterName}, Confirm button visible");
    }

    public void SelectSorcerer()
    {
        SelectCharacter(sorcererButton, sorcererDetailPanel, "Sorcerer");
    }

    public void SelectBarbarian()
    {
        SelectCharacter(barbarianButton, barbarianDetailPanel, "Barbarian");
    }

    public void SelectRouge()
    {
        SelectCharacter(rougeButton, rougeDetailPanel, "Rouge");
    }

    public void ConfirmSelection()
    {
        if (!string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.Log($"Character confirmed: {selectedCharacter}");
            // Add logic to proceed with the selected character
        }
        else
        {
            Debug.LogWarning("No character selected!");
        }
    }

    public void BackToSelection()
    {
        // Hide all detail panels
        sorcererDetailPanel.SetActive(false);
        barbarianDetailPanel.SetActive(false);
        rougeDetailPanel.SetActive(false);

        // Show the character selection panel
        characterClassPanel.SetActive(true);

        // Keep confirm button visible
        confirmButton.SetActive(true);

        // Highlight the previously selected button
        HighlightSelectedButton();

        Debug.Log("Back to selection screen, confirm button remains visible");
    }
}
