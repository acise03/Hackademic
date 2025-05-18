using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BoyCharacter : MonoBehaviour
{
    public int maxHealth = 100;
    public TMP_Text healthText;
    private int currentHealth;

    public GameObject paperPrefab;
    public float paperThrownSpeed = 10f; private float damageCooldown = 1f;
    private float lastDamageTime = -Mathf.Infinity;


    public Sprite[] frames;
    public float framesPerSecond = 20f;
    public float moveSpeed = 10f;

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    private Vector2 lastMoveDirection = Vector2.right;

    public HealthBar healthBar;
    void UpdateHealthText(int health)
    {
        healthText.text = "Health: " + health.ToString() + "/100";

    }


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0)
            spriteRenderer.sprite = frames[0];

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy") && Time.time - lastDamageTime >= damageCooldown)
        {
            Debug.Log("character colliddeeeeee");
            TakeDamage(10);
            lastDamageTime = Time.time;
        }
    }

    void Update()
    {
        Debug.Log(currentHealth + "/" + maxHealth);
        HandleMovement();
        HandleAnimation();

        if (Input.GetKeyDown(KeyCode.Space) && Points.rating > 0)
        {
            ShootPaper();
            Points.rating--;
        }

        if (currentHealth == 0)
        {
            SceneManager.LoadScene("GameOver");

        }


    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(moveX, moveY).normalized;
        if (move != Vector2.zero)
        {
            lastMoveDirection = move;
        }

        transform.Translate(move * moveSpeed * Time.deltaTime);

        if (moveX != 0)
        {
            spriteRenderer.flipX = moveX < 0;
        }
    }

    void HandleAnimation()
    {
        if (frames.Length == 0) return;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / framesPerSecond)
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                spriteRenderer.sprite = frames[currentFrame];
                timer = 0f;
            }
        }
        else
        {
            currentFrame = 0;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }

    void ShootPaper()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        GameObject paper = Instantiate(paperPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = paper.GetComponent<Rigidbody2D>();
        rb.velocity = direction * paperThrownSpeed;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        UpdateHealthText(currentHealth);
    }

}
