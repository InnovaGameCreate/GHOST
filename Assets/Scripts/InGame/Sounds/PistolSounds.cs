using UnityEngine;

public class PistolSounds : MonoBehaviour
{
    public AudioClip shootingSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = shootingSound;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリックが押されたら
        {
            Shoot();
        }
    }

    void Shoot()
    { 
        // 発射音を再生
        audioSource.Play();
    }
}