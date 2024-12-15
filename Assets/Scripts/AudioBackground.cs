using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioClip bossLevel0Clip; // Assign audio clip for Boss Level 0 in the Inspector
    public AudioClip bossLevel1Clip; // Assign audio clip for Boss Level 1 in the Inspector
    public AudioSource audioSource; // Assign the AudioSource component in the Inspector
    private int currentBossLevel = -1; // Keeps track of the current boss level to avoid redundant updates

    void Start()
    {
        UpdateAudioClip(); // Initialize the audio clip at the start
    }

    void Update()
    {
        UpdateAudioClip(); // Check and update the audio clip dynamically
    }

    private void UpdateAudioClip()
    {
        // Get the BossLevelSelected value from PlayerPrefs
        int bossLevelSelected = PlayerPrefs.GetInt("BossLevelSelected", 0);

        // If the boss level has changed, update the audio clip
        if (bossLevelSelected != currentBossLevel)
        {
            currentBossLevel = bossLevelSelected;

            // Assign the appropriate audio clip based on the boss level
            switch (bossLevelSelected)
            {
                case 0:
                    audioSource.clip = bossLevel0Clip;
                    break;
                case 1:
                    audioSource.clip = bossLevel1Clip;
                    break;
                default:
                    Debug.LogWarning($"Boss level {bossLevelSelected} is not handled. Defaulting to level 0 clip.");
                    audioSource.clip = bossLevel0Clip;
                    break;
            }

            // Play the new audio clip
            if (audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
    }
}
