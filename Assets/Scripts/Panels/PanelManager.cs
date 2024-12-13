using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // Panels
    public GameObject mainMenuPanel;
    public GameObject currentPanel;
    public GameObject choosePlayerPanel;
    public GameObject bossLevelPanel;

    private bool isMainLevelSelected = false;
    private bool isBossLevelSelected = false;

public void SelectMainLevel()
{
    isMainLevelSelected = true;
    isBossLevelSelected = false;
    Debug.Log("hey main");
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