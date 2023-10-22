using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerFire : NetworkBehaviour
{
    [SerializeField]
    private GunBase _gunBase; 
    void Update()
    {
        if(HasStateAuthority)
        {
            if (Input.GetKey(KeyCode.Mouse0))
                _gunBase.OpenFire();
            else
                _gunBase.StopFire();
        }
    }
}
