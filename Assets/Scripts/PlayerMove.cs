using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float dashDistance = 1f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.2f;

    private bool isDashing = false;
    private bool canDash = true;

    //[SerializeField] private GameObject player;
    private Rigidbody2D rb;
    private Vector3 dashDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (canDash && !isDashing)
            {
                // Store the direction of dash based on player's input
                dashDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;

                // Start dashing
                StartCoroutine(Moving());
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (canDash && !isDashing)
            {
                // Store the direction of dash based on player's input
                dashDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;

                // Start dashing
                StartCoroutine(Moving());
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (canDash && !isDashing)
            {
                // Store the direction of dash based on player's input
                dashDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;

                // Start dashing
                StartCoroutine(Moving());
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (canDash && !isDashing)
            {
                // Store the direction of dash based on player's input
                dashDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;

                // Start dashing
                StartCoroutine(Moving());
            }
        }

    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Brick"))
        {
            dashDistance = 0f;
        }
    }

    private IEnumerator Moving()
    {
        isDashing = true;
        canDash = false;
        // Disable player's movement during the dash
        var originalPosition = transform.position;
        var targetPosition = originalPosition + dashDirection * dashDistance;
        var elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            // Lerp between the original position and target position based on time
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / dashDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the target position
        transform.position = targetPosition;

        // Wait for dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        isDashing = false;

    }
}
