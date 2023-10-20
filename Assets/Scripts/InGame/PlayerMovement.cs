using Fusion;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField]
    private GameObject CameraFollow;
    [SerializeField]
    private float _dashSpeed = 2;
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
                velocity += transform.forward;

            if (Input.GetKey(KeyCode.S))
                velocity -= transform.forward;

            if (Input.GetKey(KeyCode.A))
                velocity -= transform.right;

            if (Input.GetKey(KeyCode.D))
                velocity += transform.right;



            velocity = velocity.normalized * Runner.DeltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
                velocity *= _dashSpeed;
            _cc.Move(5 * velocity);
        }

    }


}