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

    public void Shoot()
    { 
        // 発射音を再生
        audioSource.Play();
    }
}