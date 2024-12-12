using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TMP_Text sorcererText;
    public TMP_Text barbarianText;
    public TMP_Text rougeText;
    public TMP_Text abilitiesText;
    public Color defaultColor = Color.white;
    public Color selectionColor = Color.red; // Color when selected

    private string selectedCharacter;
    private TMP_Text currentSelectedText;

    void Start()
    {
        selectedCharacter = null;
        confirmButton.SetActive(false);
        ResetTextColors();
        Debug.Log("Initialization complete");
    }

    private void ResetTextColors()
    {
        sorcererText.color = defaultColor;
        barbarianText.color = defaultColor;
        rougeText.color = defaultColor;

        Debug.Log("Text colors reset");
    }

    private void SetCharacterTextColor(TMP_Text characterText)
    {
        // Reset text colors for all characters
        ResetTextColors();

        // Highlight the selected character's text in red
        characterText.color = selectionColor;
    }

    private void SelectCharacter(GameObject detailPanel, string characterName, TMP_Text characterText)
    {
        // Hide all detail panels
        sorcererDetailPanel.SetActive(false);
        barbarianDetailPanel.SetActive(false);
        rougeDetailPanel.SetActive(false);

        // Hide the main panel and show the detail panel
        characterClassPanel.SetActive(false);
        detailPanel.SetActive(true);

        selectedCharacter = characterName;
        confirmButton.SetActive(true);
        if(characterName=="Sorcerer"){
            abilitiesText.text="";
        }
        if(characterName=="Barbarian"){
            abilitiesText.text="";
        }
        if(characterName=="Rouge"){
            abilitiesText.text="";
        }

        // Set the selected character text color
        SetCharacterTextColor(characterText);

        Debug.Log($"Character selected: {characterName}, Confirm button visible");
    }

    public void SelectSorcerer()
    {
        SelectCharacter(sorcererDetailPanel, "Sorcerer", sorcererText);
    }

    public void SelectBarbarian()
    {
        SelectCharacter(barbarianDetailPanel, "Barbarian", barbarianText);
    }

    public void SelectRouge()
    {
        SelectCharacter(rougeDetailPanel, "Rouge", rougeText);
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

        // Hide confirm button when going back to the selection screen
        confirmButton.SetActive(false);

        Debug.Log("Back to selection screen, confirm button hidden");
    }
}
