using UnityEngine;

public class BoyCharacter : MonoBehaviour
{
    public Sprite[] frames;
    public float framesPerSecond = 20f;
    public float moveSpeed = 10f;

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0)
            spriteRenderer.sprite = frames[0];
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(moveX, moveY).normalized;
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
}
