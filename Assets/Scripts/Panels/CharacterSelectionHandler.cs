using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectionHandler : MonoBehaviour
{
    public GameObject sorcererDetailPanel;
    public GameObject barbarianDetailPanel;
    public GameObject currentPanel;
    public GameObject previousPanel;
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
     private string barbarianAbilities = "Barbarian Abilities:\n" +
        "• Bash: Swings the axe to attack an enemy in a small range.\n" +
        "• Shield: A protective shield lasting 3 seconds.\n" +
        "• Iron Maelstrom: Deals damage in a 360° small range circle.\n" +
        "• Charge: Rushes forward, destroying enemies/objects in the way.\n";

    private string sorcererAbilities = "Sorcerer Abilities:\n" +
        "• Fireball: Shoots a fireball to deal damage.\n" +
        "• Teleport: Instantly moves to a selected position.\n" +
        "• Clone: Creates a decoy that explodes after 5 seconds.\n" +
        "• Inferno: Creates a flame ring dealing damage over time.\n";

    private string rogueAbilities = "Rogue Abilities:\n" +
        "• Arrow: Shoots an arrow to deal damage.\n" +
        "• Smoke Bomb: Stuns enemies for 5 seconds.\n" +
        "• Dash: Quickly moves to a selected position.\n" +
        "• Shower of Arrows: Slows enemies and deals damage.\n";
    public Color defaultColor = Color.white;
    public Color selectionColor = Color.red;

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

    private void SelectCharacter(GameObject detailPanel, string characterName, TMP_Text characterText,string abilitiesDescription )
    {
        selectedCharacter = characterName;
        confirmButton.SetActive(true);
        previousPanel.SetActive(false);

        abilitiesText.text = abilitiesDescription;
        // Set the selected character text color
        SetCharacterTextColor(characterText);
        //abilitiesText.text=abilitiesText2;

        Debug.Log($"Character selected: {characterName}, Confirm button visible");
    }

    public void SelectSorcerer()
    {
        SelectCharacter(sorcererDetailPanel, "sorcerer", sorcererText, sorcererAbilities);
    }

    public void SelectBarbarian()
    {
        SelectCharacter(barbarianDetailPanel, "barbarian", barbarianText, barbarianAbilities);
    }

    public void SelectRouge()
    {
        SelectCharacter(rougeDetailPanel, "rouge", rougeText, rogueAbilities);
    }

    public void ConfirmSelection()
    {
        if (!string.IsNullOrEmpty(selectedCharacter))
        {
            GameObject panels = GameObject.Find("Panels");
            if (panels != null)
            {
                panels.SetActive(false);  // Deactivate Panels UI
                Debug.Log("Panels deactivated.");
            }
            else
            {
                Debug.LogError("Panels GameObject not found in the scene!");
            }

            Debug.Log($"Character confirmed: {selectedCharacter}");
            // Add logic to proceed with the selected character
            currentPanel.SetActive(false);

            // Switch to the new scene and activate the respective player
            ActivatePlayerInScene("newScene", selectedCharacter);
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
     public void Back()
    {
        previousPanel.SetActive(true);
        currentPanel.SetActive(false);
    }
    private void ActivatePlayerInScene(string sceneName, string characterName)
{
    // Ensure the scene is active or properly referenced
    Scene scene = SceneManager.GetSceneByName(sceneName);
    SceneManager.UnloadSceneAsync("Panels");

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
