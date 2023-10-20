using Fusion;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField]
    private GameObject CameraFollow;
    [Networked]
    public Vector3 moveVelocity { get; set; }
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();

    }
    private void Start()
    {
        NetworkObject _networkObject;
        _networkObject = GetComponent<NetworkObject>();
        if (HasStateAuthority)
        {
            Debug.Log("P1_camera_set");
            _virtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = CameraFollow.gameObject.transform;
            _virtualCamera.LookAt = CameraFollow.gameObject.transform;
        }
        else
        {
            Debug.Log("P2_camera_set");
            _virtualCamera = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = CameraFollow.gameObject.transform;
            _virtualCamera.LookAt = CameraFollow.gameObject.transform;
        }

        if(HasInputAuthority)
        {
            //RPC_RequestStateAuthority(GetComponent<NetworkObject>());
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            Vector3 velocity = Vector3.zero;
            //ˆÚ“®ŠÖŒW
            if (Input.GetKey(KeyCode.W))
                velocity += Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                velocity += Vector3.back;

            if (Input.GetKey(KeyCode.A))
                velocity += Vector3.left;

            if (Input.GetKey(KeyCode.D))
                velocity += Vector3.right;

            velocity = velocity.normalized * Runner.DeltaTime;
            moveVelocity = velocity;
            _cc.Move(5 * velocity);
        }

    }


}