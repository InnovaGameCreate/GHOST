using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Player
{
    public class NetworkAnimEventReceiver : MonoBehaviour
    {
        [SerializeField] private NetworkFPSController controller;

        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<NetworkFPSController>();
            }
        }

        public void SetActionActive(int isActive)
        {
            controller.SetActionActive(isActive);
        }

        public void ChangeWeapon()
        {
            controller.EquipWeapon();
        }

        public void RefreshStagedState()
        {
            controller.RefreshStagedState();
        }

        public void ResetStagedState()
        {
            controller.ResetStagedState();
        }
    }
}