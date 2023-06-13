using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{

    public int moveX = 1;
    public int moveY = 1;

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

    private void Update()
    {
        CheckTop();
        CheckLeft();
        CheckRight();
        CheckDown();
        if (Input.GetKeyDown(KeyCode.LeftArrow)&& !isLeft)
        {
            transform.Translate(new Vector3(-moveX, 0, 0));

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !isRight)
        {

            transform.Translate(new Vector3(moveX, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && !isTop)
        {

            transform.Translate(new Vector3(0, moveY, 0));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !isDown)
        {

            transform.Translate(new Vector3(0, -moveY, 0));
        }
      
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
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.tag == "Player")
    //    {
    //        Debug.Log("true");
    //        SceneManager.LoadScene("New");
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SingleMode"))
        {
            Debug.Log("Single");
            
            SceneManager.LoadScene("PlayMode");
        }
        else if (collision.CompareTag("CoopMode"))
        {
            Debug.Log("Coop");
            
           SceneManager.LoadScene("Map");
        }
    }

}
