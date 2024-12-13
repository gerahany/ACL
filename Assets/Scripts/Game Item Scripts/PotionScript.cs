using UnityEngine;

public class PotionScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BasePlayer player = other.GetComponent<BasePlayer>();
            if (player != null)
            {
                if (player.healingPotions < player.maxHealingPotions)
                {
                    

                    player.AddPotion();
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Potion not collected: Inventory full.");
                }
            }
        }
    }

}
