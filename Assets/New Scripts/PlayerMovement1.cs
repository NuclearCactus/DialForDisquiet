using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canMove = true;

    // Sprite renderers
    public SpriteRenderer spriteRenderer;

    // Sprites for animations
    public Sprite frontIdle;
    public Sprite frontWalk1;
    public Sprite frontWalk2;
    public Sprite backIdle;
    public Sprite backWalk1;
    public Sprite backWalk2;
    public Sprite leftIdle;
    public Sprite leftWalk1;
    public Sprite leftWalk2;
    public Sprite rightIdle;
    public Sprite rightWalk1;
    public Sprite rightWalk2;

    private Vector2 movement;
    private bool isMoving;
    private float animationTimer;
    private int animationFrame;
    private float animationInterval = 0.2f; // Time between animation frames

    // Direction enum to track facing
    private enum Direction { Front, Back, Left, Right }
    private Direction facingDirection = Direction.Front;

    void Update()
    {
        if(!canMove)
            return;
        // Handle movement input
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        movement.y = Input.GetAxisRaw("Vertical");   // W/S or Up/Down arrows

        isMoving = movement != Vector2.zero;

        if (isMoving)
        {
            HandleMovement();
            AnimateMovement();
        }
        else
        {
            SetIdleSprite();
        }
    }

    void HandleMovement()
    {
        // Move the player
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        // Set the facing direction based on movement
        if (movement.y > 0)
        {
            facingDirection = Direction.Back;
        }
        else if (movement.y < 0)
        {
            facingDirection = Direction.Front;
        }
        else if (movement.x < 0)
        {
            facingDirection = Direction.Left;
        }
        else if (movement.x > 0)
        {
            facingDirection = Direction.Right;
        }
    }

    void AnimateMovement()
    {
        // Update animation frame based on timer
        animationTimer += Time.deltaTime;

        if (animationTimer >= animationInterval)
        {
            animationFrame = (animationFrame + 1) % 2; // Alternate between 0 and 1
            animationTimer = 0f;
        }

        // Set the walking sprite based on direction and frame
        switch (facingDirection)
        {
            case Direction.Front:
                spriteRenderer.sprite = (animationFrame == 0) ? frontWalk1 : frontWalk2;
                break;
            case Direction.Back:
                spriteRenderer.sprite = (animationFrame == 0) ? backWalk1 : backWalk2;
                break;
            case Direction.Left:
                spriteRenderer.sprite = (animationFrame == 0) ? leftWalk1 : leftWalk2;
                break;
            case Direction.Right:
                spriteRenderer.sprite = (animationFrame == 0) ? rightWalk1 : rightWalk2;
                break;
        }
    }

    void SetIdleSprite()
    {
        // Set idle sprite based on facing direction
        switch (facingDirection)
        {
            case Direction.Front:
                spriteRenderer.sprite = frontIdle;
                break;
            case Direction.Back:
                spriteRenderer.sprite = backIdle;
                break;
            case Direction.Left:
                spriteRenderer.sprite = leftIdle;
                break;
            case Direction.Right:
                spriteRenderer.sprite = rightIdle;
                break;
        }
    }
}
