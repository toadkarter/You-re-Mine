using System;
using UnityEditor.Animations;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpSpeed = 10;
    [SerializeField] private float fallSpeed = 5;
    
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animatorController;
    private bool isGrounded = false;
    private bool isDead = false;
    private bool fallDamageIsActive = false;

    private const string isRunningAnimation = "isRunning";
    private const string hasDiedAnimation = "hasDied";


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animatorController = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Die()) return;
        
        Move();
        Jump();
        Fall();
    }

    private bool Die()
    {
        if (!isDead) return false;
        _animatorController.SetTrigger(hasDiedAnimation);
        return true;
    }

    private void Move()
    {
        float currentDirection = Input.GetAxis("Horizontal") * moveSpeed;
        _rigidbody2D.velocity = new Vector2(currentDirection, _rigidbody2D.velocity.y);
        _spriteRenderer.flipX = currentDirection < 0;
        PlayRunningAnimation(currentDirection);
    }

    private void PlayRunningAnimation(float currentDirection)
    {
        if (currentDirection == 0)
        {
            _animatorController.ResetTrigger(isRunningAnimation);
        }
        else
        {
            _animatorController.SetTrigger(isRunningAnimation);
        }
    }

    private void Jump()
    {
        bool isJumping = Input.GetButtonDown("Jump");
        if (isJumping && isGrounded)
        {
            _rigidbody2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
        };
    }

    private void Fall()
    {
        if (!isFalling()) return;
        
        _rigidbody2D.AddForce(Vector2.down * fallSpeed);
        if (_rigidbody2D.velocity.y < -30.0)
        {
            fallDamageIsActive = true;
        }
    }

    private bool isFalling()
    {
        return _rigidbody2D.velocity.y < 0;
    }

    // Added new thing
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            isDead = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Ground")) return;
        
        isGrounded = true;
        if (fallDamageIsActive)
        {
            isDead = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
