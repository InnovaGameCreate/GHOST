using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioClip[] walkSounds;      // 通常の足音の効果音の配列
    public AudioClip[] runSounds;       // ダッシュ時の足音の効果音の配列
    public float walkStepInterval = 0.5f; // 通常の足音の間隔
    public float runStepInterval = 0.3f;  // ダッシュ時の足音の間隔

    private float stepTimer = 0f;
    private CharacterController characterController;
    private AudioSource audioSource;
    private bool isMoving = false;
    private bool isRunning = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // AudioSourceがアタッチされていない場合は追加する
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 3D音響を有効にする
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        // キャラクターが移動しているかどうかをチェック
        if (characterController.isGrounded && characterController.velocity.magnitude > 0)
        {
            isMoving = true;
            // SHIFTキーが押されているかどうかで走り状態を切り替え
            isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // 足音の間隔を管理
            stepTimer += Time.deltaTime;
            float stepInterval = isRunning ? runStepInterval : walkStepInterval;

            if (stepTimer >= stepInterval)
            {
                // 足音をランダムに選んで再生
                PlayRandomFootstepSound();
                stepTimer = 0f; // タイマーリセット
            }
        }
        else
        {
            isMoving = false;
        }

        // キャラクターが止まっている場合は足音を停止
        if (!isMoving)
        {
            audioSource.Stop();
        }
    }

    void PlayRandomFootstepSound()
    {
        AudioClip[] footstepSounds = isRunning ? runSounds : walkSounds;

        if (footstepSounds != null && footstepSounds.Length > 0)
        {
            // ランダムに足音を選択
            int randomIndex = Random.Range(0, footstepSounds.Length);
            AudioClip selectedFootstep = footstepSounds[randomIndex];

            // 足音を再生
            if (selectedFootstep != null)
            {
                audioSource.PlayOneShot(selectedFootstep);
            }
        }
    }
}
