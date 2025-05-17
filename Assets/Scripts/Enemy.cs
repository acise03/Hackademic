using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Sprite[] frames;
    public float framesPerSecond = 5f;
    public float moveSpeed = 0.0f;
    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    private Transform player;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0)
            spriteRenderer.sprite = frames[0];

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    void HandleAnimation()
    {
        if (frames.Length == 0) return;

        if (player != null)
        {
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
        }
        currentFrame = 0;
        spriteRenderer.sprite = frames[currentFrame];
    }
}
