using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public void Die()
    {
        GameManager.ResetScene();
    }
}