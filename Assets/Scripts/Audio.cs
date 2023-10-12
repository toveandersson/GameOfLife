using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    AudioSource audioSource;
    private float audioPitch = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            audioPitch += 0.008f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            audioPitch -= 0.008f;
        }
        audioSource.pitch = audioPitch;
    }
    
}

