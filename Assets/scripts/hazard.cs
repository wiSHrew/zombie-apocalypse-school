using UnityEngine;
using UnityEngine.SceneManagement;

public class hazard : MonoBehaviour
{
    private bool isReloading;

    private void OnTriggerEnter(Collider other)
    {
        TryReloadScene(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryReloadScene(collision.collider);
    }

    private void TryReloadScene(Collider other)
    {
        if (isReloading || other.GetComponentInParent<CharacterController>() == null)
        {
            return;
        }

        isReloading = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
