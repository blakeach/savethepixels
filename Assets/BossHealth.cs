using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            other.gameObject.SetActive(false); 
            health--;
            Debug.Log($"Boss hit! Health remaining: {health}");

            if (health <= 0)
            {
                Debug.Log("Boss defeated! You Win!");
                SceneManager.LoadScene("WinScene"); 
            }
        }
    }
}