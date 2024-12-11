using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    public GameObject[] menuPanels; // Array of menu panels
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        CheckMenuState(); // Check the initial state
    }

    void Update()
    {
        CheckMenuState(); // Continuously check the panel state
    }

    private void CheckMenuState()
    {
        bool isAnyMenuActive = false;

        // Check if any menu panel is active
        foreach (GameObject panel in menuPanels)
        {
            if (panel.activeSelf)
            {
                isAnyMenuActive = true;
                break;
            }
        }

        // Play or stop music based on the menu state
        if (isAnyMenuActive)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
