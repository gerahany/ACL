using UnityEngine;
using UnityEngine.UI;

public class SoundEffectHandler : MonoBehaviour
{
    public AudioClip shieldSound;
    public AudioClip dashSound;
    public AudioClip arrowFireSound;
    public AudioClip explosionSound;
    public AudioClip fireballSound;
    public AudioClip itemPickupSound;
    public AudioClip playerDamageSound;
    public AudioClip healingSound;
    public AudioClip playerDeathSound;
    public AudioClip enemyDeathSound;
    public AudioClip bossSummonSound;
    public AudioClip bossStompSound;
    public AudioClip bossSpellSound;
    public AudioClip bossSwingSound;
    public AudioClip bossDamageSound;
    public AudioClip bossDeathSound;

    public AudioSource audioSource;

    // Add this new public field for volume control
    public Slider volumeSlider;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Set the slider value to the current audio volume
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;

            // Add listener for volume changes
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // Method to set the audio source volume
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = GetClipByName(soundName);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetClipByName(string soundName)
    {
        switch (soundName)
        {
            case "Shield": return shieldSound;
            case "Dash": return dashSound;
            case "ArrowFire": return arrowFireSound;
            case "Explosion": return explosionSound;
            case "Fireball": return fireballSound;
            case "ItemPickup": return itemPickupSound;
            case "PlayerDamage": return playerDamageSound;
            case "Healing": return healingSound;
            case "PlayerDeath": return playerDeathSound;
            case "EnemyDeath": return enemyDeathSound;
            case "BossSummon": return bossSummonSound;
            case "BossStomp": return bossStompSound;
            case "BossSpell": return bossSpellSound;
            case "BossSwing": return bossSwingSound;
            case "BossDamage": return bossDamageSound;
            case "BossDeath": return bossDeathSound;
            default: return null;
        }
    }

    // Public methods for Animation Events
    public void PlayShieldSound() => PlaySound("Shield");
    public void PlayDashSound() => PlaySound("Dash");
    public void PlayArrowFireSound() => PlaySound("ArrowFire");
    public void PlayExplosionSound() => PlaySound("Explosion");
    public void PlayFireballSound() => PlaySound("Fireball");
    public void PlayItemPickupSound() => PlaySound("ItemPickup");
    public void PlayPlayerDamageSound() => PlaySound("PlayerDamage");
    public void PlayHealingSound() => PlaySound("Healing");
    public void PlayPlayerDeathSound() => PlaySound("PlayerDeath");
    public void PlayEnemyDeathSound() => PlaySound("EnemyDeath");
    public void PlayBossSummonSound() => PlaySound("BossSummon");
    public void PlayBossStompSound() => PlaySound("BossStomp");
    public void PlayBossSpellSound() => PlaySound("BossSpell");
    public void PlayBossSwingSound() => PlaySound("BossSwing");
    public void PlayBossDamageSound() => PlaySound("BossDamage");
    public void PlayBossDeathSound() => PlaySound("BossDeath");
}
