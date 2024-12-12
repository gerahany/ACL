using UnityEngine;
using UnityEngine.UI;

public class MenuAudioManager : MonoBehaviour
{
    public GameObject[] menuPanels; // Array of menu panels
    private AudioSource audioSource;
    public Slider volumeSlider; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Set the slider value to the current audio volume
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;

            // Add a listener to handle slider value changes
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

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
     public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
