using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{

    public int moveX = 1;
    public int moveY = 1;
    public int moveSpeed = 10;

    //private bool isGrounded =false;
    [SerializeField] Transform checkTop;
    [SerializeField] Transform checkLeft;
    [SerializeField] Transform checkRight;
    [SerializeField] Transform checkDown;

    [SerializeField] LayerMask brickLayer;
    [SerializeField]
    private bool isTop;
    [SerializeField]
    private bool isLeft;
    [SerializeField]
    private bool isRight;
    [SerializeField]
    private bool isDown;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private Vector2 startPosition ;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        CheckTop();
        CheckLeft();
        CheckRight();
        CheckDown();
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !isLeft)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(-moveX, 0f);
            StartCoroutine(Move());
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !isRight)
        {
             startPosition = rb.position;
            targetPosition = startPosition + new Vector2(moveX, 0f);
            StartCoroutine(Move());
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && !isTop)
        {
             startPosition = rb.position;
            targetPosition = startPosition + new Vector2(0f, moveY);
            StartCoroutine(Move());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !isDown)
        {
             startPosition = rb.position;
            targetPosition = startPosition + new Vector2(0f, -moveY);
            StartCoroutine(Move());
        }

    }
    private IEnumerator Move()
    {
        // Calculate the distance to dash
        float distanceToMove = Vector2.Distance(startPosition, targetPosition);

        // Calculate the time it will take to complete the dash
        float moveTime = distanceToMove / moveSpeed;

        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            // Move the player towards the target position
            rb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Snap to the target position to ensure accuracy
        rb.position = targetPosition;
    }

    void CheckTop()
    {
        isTop = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkTop.position, 0.2f, brickLayer);
        if (colliders.Length > 0)
            isTop = true;
    }
    void CheckLeft()
    {
        isLeft = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkLeft.position, 0.2f, brickLayer);
        if (colliders.Length > 0)
            isLeft = true;
    }
    void CheckRight()
    {
        isRight = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkRight.position, 0.2f, brickLayer);
        if (colliders.Length > 0)
            isRight = true;
    }
    void CheckDown()
    {
        isDown = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkDown.position, 0.2f, brickLayer);
        if (colliders.Length > 0)
            isDown = true;
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
