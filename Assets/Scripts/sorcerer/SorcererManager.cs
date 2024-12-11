using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorcererManager : MonoBehaviour
{
    public static bool buttonPress=false;

    // Getter and Setter for isAbilityActive
     public static bool IsButton()
    {
        return buttonPress;
    }

    public static void SetButton(bool isActive)
    {
        buttonPress = isActive;
    }
}