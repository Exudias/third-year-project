using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormKiller : MonoBehaviour
{
    [SerializeField] private PlayerFormSwitcher.PlayerForm formToKill;

    public PlayerFormSwitcher.PlayerForm GetForm()
    {
        return formToKill;
    }
}
