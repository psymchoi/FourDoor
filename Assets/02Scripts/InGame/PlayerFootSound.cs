using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootSound : MonoBehaviour
{
    public AudioClip footSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            AudioSource.PlayClipAtPoint(footSound, transform.position);
        }
    }
}
