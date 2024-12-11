using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private static bool isAbilityActive = false;
    public static bool buttonPress=false;

    // Getter and Setter for isAbilityActive
    public static bool IsAbilityActive()
    {
        return isAbilityActive;
    }

    public static void SetAbilityActive(bool isActive)
    {
        isAbilityActive = isActive;
    }

     public static bool IsButton()
    {
        return buttonPress;
    }

    public static void SetButton(bool isActive)
    {
        buttonPress = isActive;
    }
}