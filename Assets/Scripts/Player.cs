using Fusion;
using UnityEngine;
using Cinemachine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;
    private CinemachineVirtualCamera _virtualCamera;
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();

    }
    private void Start()
    {
        NetworkObject _networkObject;
        _networkObject = GetComponent<NetworkObject>();
        Debug.Log($"HasInputAuthority{HasInputAuthority}");
        if (HasInputAuthority)
        {
            Debug.Log("P1_camera_set");
            _virtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = this.gameObject.transform;
            _virtualCamera.LookAt = this.gameObject.transform;
        }
        else
        {
            Debug.Log("P2_camera_set");
            _virtualCamera = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = this.gameObject.transform;
            _virtualCamera.LookAt = this.gameObject.transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
        }
    }
}