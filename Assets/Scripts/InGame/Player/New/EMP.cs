using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Player
{
    public class EMP : MonoBehaviour
    {
        [SerializeField]
        PlayerStatus _playerStatus;
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse2))
            {
                _playerStatus.UseEMP(true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                _playerStatus.UseEMP(false);
            }
        }
    }
}