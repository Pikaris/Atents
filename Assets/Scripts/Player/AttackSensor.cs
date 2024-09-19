using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSensor : MonoBehaviour
{
    public Action<Slime> onSlimeEnter;
    public Action<Slime> onSlimeExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Slime slime = collision.GetComponent<Slime>();
        if(slime != null)
        {
            onSlimeEnter.Invoke(slime);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Slime slime = collision.GetComponent<Slime>();
        if (slime != null)
        {
            onSlimeExit.Invoke(slime);
        }
    }
}
