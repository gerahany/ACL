using UnityEngine;

public class CampManager : MonoBehaviour
{
    public GameObject runeFragment; // Assign the Rune Fragment prefab
    private int remainingEnemies; // Tracks the number of enemies in the camp

    void Start()
    {
        // Count all child enemies under this camp manager
        remainingEnemies = CountEnemies();
        if (runeFragment != null)
        {
            runeFragment.SetActive(false); // Ensure Rune Fragment is hidden initially
        }
    }

    public void EnemyDefeated()
    {
        remainingEnemies--;
        Debug.Log("Enemy defeated. Remaining enemies: " + remainingEnemies);

        if (remainingEnemies <= 0 && runeFragment != null)
        {
            runeFragment.GetComponent<RuneFragmentScript>().ActivateFragment();
        }
    }

    private int CountEnemies()
    {
        // Count all enemies directly under this GameObject
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Enemy")) // Ensure enemies are tagged correctly
            {
                count++;
            }
        }
        return count;
    }
}
