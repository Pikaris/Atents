using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Singleton<Factory>
{
    SlimePool slime;

    protected override void OnInitialize()
    {
        slime = GetComponentInChildren<SlimePool>();
        if (slime != null)
        {
            slime.Initialized();
        }
    }

    public Slime GetSlime(Vector3? position)
    {
        return slime.GetObject(position);
    }
}
