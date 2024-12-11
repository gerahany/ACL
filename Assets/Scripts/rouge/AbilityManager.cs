using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private static bool isAbilityActive = false;

    // Getter and Setter for isAbilityActive
    public static bool IsAbilityActive()
    {
        return isAbilityActive;
    }

    public static void SetAbilityActive(bool isActive)
    {
        isAbilityActive = isActive;
    }
}