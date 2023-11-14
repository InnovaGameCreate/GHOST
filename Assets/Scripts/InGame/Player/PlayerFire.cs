using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerFire : NetworkBehaviour
{
    private GunBase _gunBase;
    public void setGunBase(GunBase _base)
    {
        _gunBase = _base;
    }
    void Update()
    {
        if(HasStateAuthority && _gunBase != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
                _gunBase.OpenFire();
            else
                _gunBase.StopFire();
        }
    }
}
