
using UnityEngine;

public class RuneFragmentScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RuneFragment"))
        {
            BasePlayer player = other.GetComponent<BasePlayer>();
            if (player != null)
            {
                player.addrune();
            }
        }
    }

}

