using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // Panels
    public GameObject mainMenuPanel;
    public GameObject currentPanel;
    public GameObject choosePlayerPanel;
    public GameObject bossLevelPanel;

    // Flags to determine the chosen level
    private bool isMainLevelSelected = false;
    private bool isBossLevelSelected = false;

    // Stack to track panel shistory
    //private Stack<GameObject> panelHistory = new Stack<GameObject>();

public void SelectMainLevel()
{
    isMainLevelSelected = true;
    isBossLevelSelected = false;

    // Navigate to Choose Player Panel
    mainMenuPanel.SetActive(false);
    choosePlayerPanel.SetActive(true);
}

public void SelectBossLevel()
{
    isBossLevelSelected = true;
    isMainLevelSelected = false;

    // Navigate to Choose Player Panel
    mainMenuPanel.SetActive(false);
    choosePlayerPanel.SetActive(true);
}


public void Back()
{
    mainMenuPanel.SetActive(true);
    currentPanel.SetActive(false);
    choosePlayerPanel.SetActive(false);
    bossLevelPanel.SetActive(false);
}

}