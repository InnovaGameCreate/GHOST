using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerSight : NetworkBehaviour
{
    [SerializeField]
    private GameObject _cameraPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HasStateAuthority)

        { // �}�E�X�̈ړ��ʂ��擾
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            // X�����Ɉ��ʈړ����Ă���Ή���]
            if (Mathf.Abs(mx) > 0.001f)
            {
                // ��]���̓��[���h���W��Y��
                transform.RotateAround(transform.position, Vector3.up, mx);
            }

            // Y�����Ɉ��ʈړ����Ă���Ώc��]
            if (Mathf.Abs(my) > 0.001f)
            {
                // ��]���̓J�������g��X��
                _cameraPosition.transform.RotateAround(transform.position, -transform.right, my);
            }
        }
    }
    public override void FixedUpdateNetwork()
    {
        
    }
}
