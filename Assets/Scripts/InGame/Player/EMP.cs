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
            if(Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(EMPFinish());
            }
        }

        IEnumerator EMPFinish()
        {
            _playerStatus.UseEMP(true);
            yield return new WaitForSeconds(5);
            _playerStatus.UseEMP(false);
        }
    }
}