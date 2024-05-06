using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(AudioSource))]
public class GamingAudio : NetworkBehaviour
{
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip banana;
    [SerializeField] private AudioClip idle;
    [Networked] private float _volume { get; set; }

    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = idle;
        audioSource.loop = true;
        audioSource.Play();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data))
        {
            // _volume = 0.3f;
            // if (data.vrRightControllerTrigger > 0f)
            // {
            //     _volume += data.vrRightControllerTrigger / 2;
            // }
            // audioSource.volume = _volume;
        }
    }

    // Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            audioSource.PlayOneShot(coin);
        }
        if (other.gameObject.tag == "Banana")
        {
            audioSource.PlayOneShot(banana);
        }
    }
}
