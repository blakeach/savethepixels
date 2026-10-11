using UnityEngine;

public class KeyTrigger : MonoBehaviour
{
    [SerializeField] private GameObject brownDoor; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (brownDoor != null)
            {
                brownDoor.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}