using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace InGame.Player
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [SerializeField]
        private Camera _camera;
        private PlayerStatus _status;
        // Start is called before the first frame update
        void Start()
        {
            _status = gameObject.transform.root.GetComponent<PlayerStatus>();
            if (_status.isLocalPlayer)
            {
                _camera.rect = new Rect(0, 0, 1, 0.5f);
            }
            else
            {
                _camera.rect = new Rect(0, 0.5f, 1, 0.5f);
            }
        }
    }
}