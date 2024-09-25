using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test12_SlimeSpawner : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Slime[] slimes = FindObjectsByType<Slime>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Slime slime in slimes)
        {
            slime.Die();
        }
    }
}
