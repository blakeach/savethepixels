using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 12f;

    private Vector2 moveInput;
    private bool isGrounded;

    private float elapsedTime = 0f;
    private bool isTiming = true;
    private float bestTime; 

    private int level2SquaresCollected = 0;
    private bool canShootBoss = false;

    private void Start()
    {
        bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
    }

    private void OnEnable()
    {
        if (moveAction != null && moveAction.action != null) moveAction.action.Enable();
        if (jumpAction != null && jumpAction.action != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += OnJumpPerformed;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null) moveAction.action.Disable();
        if (jumpAction != null && jumpAction.action != null)
        {
            jumpAction.action.Disable();
            jumpAction.action.performed -= OnJumpPerformed;
        }
    }

    void Update()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveInput = moveAction.action.ReadValue<Vector2>();
        }

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        if (isTiming)
        {
            elapsedTime += Time.deltaTime;
        }

        if (timerText != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Level2")
            {
                if (canShootBoss)
                {
                    timerText.text = $"Squares: {level2SquaresCollected}/3\n<color=yellow>Press Q to Fire!</color>";
                }
                else
                {
                    timerText.text = $"Squares: {level2SquaresCollected}/3";
                }
            }
            else
            {
                // Level 1: Hide squares count, show timer and best time
                if (canShootBoss)
                {
                    timerText.text = "<color=yellow>Press Q to Fire!</color>";
                }
                else
                {
                    string bestDisplay = (bestTime == float.MaxValue) ? "--" : bestTime.ToString("F2") + "s";
                    timerText.text = $"Time: {elapsedTime:F2}s\nBest: {bestDisplay}";
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && canShootBoss)
        {
            FirePooledProjectile();
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FirePooledProjectile()
    {
        if (ProjectilePooler.Instance != null)
        {
            GameObject proj = ProjectilePooler.Instance.GetPooledProjectile();
            if (proj != null)
            {
                if (firePoint != null)
                    proj.transform.position = firePoint.position;
                else
                    proj.transform.position = transform.position;

                proj.SetActive(true);
                
                Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
                if (projRb != null)
                {
                    projRb.linearVelocity = Vector2.right * projectileSpeed;
                }
            }
        }
        else
        {
            Debug.LogWarning("ProjectilePooler instance not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            Debug.Log("Hit hazard! Restarting...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (other.CompareTag("Goal"))
        {
            isTiming = false; 

            float currentBest = PlayerPrefs.GetFloat("BestTime", float.MaxValue);

            if (elapsedTime < currentBest)
            {
                PlayerPrefs.SetFloat("BestTime", elapsedTime);
                PlayerPrefs.Save();
                bestTime = elapsedTime; 
                Debug.Log($"<color=green>New Best Time!</color> {elapsedTime:F2} seconds!");
            }
            else
            {
                Debug.Log($"Level Complete! Time: {elapsedTime:F2}s | Best Time: {currentBest:F2}s");
            }

            Debug.Log("Goal reached! Loading Level 2...");
            SceneManager.LoadScene("Level2");
        }
        else if (other.CompareTag("Level2Square"))
        {
            level2SquaresCollected++;
            Debug.Log($"Square collected in Level 2! Total: {level2SquaresCollected}/3");
            other.gameObject.SetActive(false);

            if (level2SquaresCollected >= 3)
            {
                canShootBoss = true;
                Debug.Log("<color=green>Boss weapon unlocked! Press Q to fire!</color>");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}