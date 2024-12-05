using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    public int requiredRunes = 3; // Runes required to unlock the gate
    public string bossLevelSceneName = "BossLevel"; // Name of the boss level scene

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null && playerInventory.runeCount >= requiredRunes)
            {
                Debug.Log("Gate Unlocked! Loading Boss Level...");
                LoadBossLevel();
            }
            else
            {
                Debug.Log("Not enough Rune Fragments to unlock the gate!");
            }
        }
    }

    private void LoadBossLevel()
    {
        SceneManager.LoadScene(bossLevelSceneName);
    }
}
