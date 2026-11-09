using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class soundStart : MonoBehaviour
{
    [SerializeField] private AudioClip collisionSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }
        else if (audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Assign a collision sound or an AudioClip to this object's AudioSource.", this);
        }
    }
}
