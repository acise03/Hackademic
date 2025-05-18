using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public Sprite[] frames;
    public float framesPerSecond = 5f;
    public float moveSpeed;

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    private Transform player;
    private bool isFrozen = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0)
            spriteRenderer.sprite = frames[0];

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isFrozen) return;

        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        if (player == null) return;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    void HandleAnimation()
    {
        if (frames.Length == 0 || player == null) return;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position);

        if (direction.magnitude > 0.01f)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / framesPerSecond)
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                spriteRenderer.sprite = frames[currentFrame];
                timer = 0f;
            }
            return;
        }

        spriteRenderer.sprite = frames[currentFrame];
    }

    private IEnumerator FadeAndDestroy()
    {
        isFrozen = true;

        if (TryGetComponent<Collider2D>(out var collider))
            collider.enabled = false;

        Color originalColor = spriteRenderer.color;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isFrozen) return;

        if (other.CompareTag("paper"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeAndDestroy());
        }
    }
}
