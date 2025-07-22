using Unity.VisualScripting;
using UnityEngine;
using System;
using JetBrains.Annotations;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Unity.XR.GoogleVr;

public class PlayerMove : MonoBehaviour
{
    public GameManager GameManager;
    private float maxSpeed = 5;
    //Jump
    private float JumpPower = 10;
    public float wallJumpPower;
    private int wallIsRight;
    public float antiGravity;
    public int jumpCount;
    public int maxJumpCount;
    public float afterWallJumpStiff;
    //Dash
    public float currentTime;
    public float lastTapTime;
    public float doubleTapTimeLimit;
    public bool firstTapDetected;
    public float dashCount;
    public float dashTime;
    public float dashCoolDown;

    public Transform Double;
    public bool isDoubleActivated = false;
    private bool doubleEffectPlayed = false;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    GrapplingHook grappling;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        grappling = GetComponent<GrapplingHook>();
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Debug.DrawRay(rigid.position, Vector3.right, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector3.left, new Color(0, 1, 0));
        RaycastHit2D rayDown = Physics2D.Raycast(rigid.position, Vector3.down, 0.5f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayRight = Physics2D.Raycast(rigid.position, Vector3.right, 0.5f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayLeft = Physics2D.Raycast(rigid.position, Vector3.left, 0.5f, LayerMask.GetMask("Platform"));
        //jump
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rigid.linearVelocityY = 0;
            rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            animator.SetBool("is jumping", true);
            animator.SetBool("is diving", false);
            jumpCount++;

            // 벽 점프 감지
            if (rayLeft.collider != null)
            {
                animator.SetBool("is wallkick", true);
                animator.SetBool("is jumping", false);
                wallIsRight = -1;
                afterWallJumpStiff = 20;
            }
            else if (rayRight.collider != null)
            {
                animator.SetBool("is wallkick", true);
                animator.SetBool("is jumping", false);
                rigid.AddForce(Vector2.left * wallJumpPower, ForceMode2D.Impulse);
                wallIsRight = 1;
                afterWallJumpStiff = 20;
            }
        }

        //벽점프&관성
        if (wallIsRight == 1)
        {
            rigid.linearVelocityX = -afterWallJumpStiff;
        }
        else if (wallIsRight == -1)
        {
            rigid.linearVelocityX = afterWallJumpStiff;
        }

        //stop speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 1.5f, rigid.linearVelocity.y);
        }

        // 방향 반전: 실제 이동 방향 기준
        if (Mathf.Abs(rigid.linearVelocity.x) > 0.01f)
        {
            spriteRenderer.flipX = rigid.linearVelocity.x < 0;
        }


        //walk animation
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.7)
            animator.SetBool("is walking", false);
        else
            animator.SetBool("is walking", true);
        animator.speed = maxSpeed / 7f;

        //Dash
        currentTime = Time.time;
        if (Input.GetButtonDown("Horizontal"))
        {
            if (currentTime - lastTapTime <= doubleTapTimeLimit && firstTapDetected)
            {
                firstTapDetected = false;
                if (dashCount < 1 && dashCoolDown == 0)
                {
                    animator.SetBool("is sliding", true);
                    
                    if (rayDown.collider == null)
                    {
                        animator.SetBool("is diving", true);
                        rigid.linearVelocity = new Vector2(h * 12, 5);
                    }
                    else
                        rigid.linearVelocityX = h * 12;
                    dashCount++;
                    dashTime = 20;
                    dashCoolDown = 30;
                }
            }
            else
            {
                firstTapDetected = true;
                lastTapTime = currentTime;
            }
        }
        if (currentTime - lastTapTime > doubleTapTimeLimit)
        {
            lastTapTime = -1f;
            firstTapDetected = false;
        }

    }

    void FixedUpdate()
    {
        // 더블점프 이펙트
        if (jumpCount == 2 && !doubleEffectPlayed)
        {
            Double doubleScript = Double.GetComponent<Double>();
            if (doubleScript != null && !doubleScript.isDoubleActivated)
            {
                doubleScript.Activate();
                doubleEffectPlayed = true; // 한 번만 실행되게
            }
        }

        // jumpCount가 초기화되면 다시 false로
        if (jumpCount < 2 && doubleEffectPlayed)
        {
            doubleEffectPlayed = false;
        }





        //  Grappling 상태에 따라 속도 변경
        if (grappling.isAttach)
            {
                maxSpeed = 7f; // 줄에 매달렸을 때 속도 증가 (원하는 값으로 조정 가능)
                rigid.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * 10, ForceMode2D.Force);
            }
            else
            {
                maxSpeed = 5f; // 기본 속도
            }


        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // 시각 디버깅용

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform", "Speed", "Jump"));
        RaycastHit2D rayRight = Physics2D.Raycast(rigid.position, Vector2.right, 0.5f, LayerMask.GetMask("Platform", "Speed", "Jump"));
        RaycastHit2D rayLeft = Physics2D.Raycast(rigid.position, Vector2.left, 0.5f, LayerMask.GetMask("Platform", "Speed", "Jump"));


        //move speed
        float h = Input.GetAxisRaw("Horizontal");
        if (afterWallJumpStiff < 0)
        {
             h = Input.GetAxisRaw("Horizontal");
        }
        
        if (grappling.isAttach)
        {
            rigid.AddForce(Vector2.right * h * 10, ForceMode2D.Force);
        }

            


        if (dashTime == 0)
        {
            animator.SetBool("is sliding", false);
            if (rigid.linearVelocity.x > maxSpeed)
                rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocityY);
            else if (rigid.linearVelocity.x < maxSpeed * (-1))
                rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocityY);
        }

        //플렛폼 감지
        if (rigid.linearVelocity.y < 0)
        {
            if (rayHit.collider != null)
            {
                animator.SetBool("is diving", false);
                animator.SetBool("is wallkick", false);
                animator.SetBool("is jumping", false);


            }
            if (rayHit.collider != null || rayRight.collider != null || rayLeft.collider != null)
            {
                animator.SetBool("is diving", false);
                animator.SetBool("is wallkick", false);
                jumpCount = 0;
                dashCount = 0;
                Debug.Log("초기화됨");
            }

        }
        if (afterWallJumpStiff == 0 && dashTime == 0)
        {
            if (rayRight.collider == null && rayLeft.collider == null)
            {
                rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
                animator.SetBool("is climbing", false);
            }
            else
            {
                animator.SetBool("is climbing", true);
                if (rigid.linearVelocityY < 0)
                    rigid.AddForce(Vector2.up * antiGravity, ForceMode2D.Impulse);
                if (rayRight.collider != null && Input.GetKey(KeyCode.LeftArrow))
                    rigid.AddForce(Vector2.left, ForceMode2D.Impulse);
                if (rayLeft.collider != null && Input.GetKey(KeyCode.RightArrow))
                    rigid.AddForce(Vector2.right, ForceMode2D.Impulse);
            }
        }

        // Super Jump/Speed
        if (rigid.linearVelocity.y < 0)
        {

            RaycastHit2D raySpeed = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Speed"));

            if (raySpeed.collider != null)
            {
                if (raySpeed.distance < 0.6f)
                    maxSpeed = 12;
            }
        }
        RaycastHit2D SpeedHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform","Jump"));
        if (SpeedHit.collider != null && SpeedHit.distance < 0.6f)
        {
            maxSpeed = 5;
        }


        if (rigid.linearVelocity.y < 0)
        {

            RaycastHit2D rayJump = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Jump"));

            if (rayJump.collider != null)
            {
                if (rayJump.distance < 0.6f)
                    JumpPower = 20;
            }
        }
        RaycastHit2D JumpHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform", "Speed"));
        if (JumpHit.collider != null && JumpHit.distance < 0.6f)
        {
            JumpPower = 10;
        }


        
        if (dashTime > 0)
            dashTime--;
        if (dashCoolDown > 0)
            dashCoolDown--;
        if (afterWallJumpStiff > 0)
            afterWallJumpStiff--;
        if (afterWallJumpStiff == 0)
            wallIsRight = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            if (isBronze)
                GameManager.stagePoint += 100;
            else if (isSilver)
                GameManager.stagePoint += 500;
            else if (isGold)
                GameManager.stagePoint += 1000;
            //Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //Next stage
            GameManager.NextStage();
        }
        else if (collision.gameObject.tag == "Fragment")
        {
            //Deactive Item
            collision.gameObject.SetActive(false);
            GameManager.dreamPoint += 1;
            if (GameManager.dreamPoint == 5)
                GameManager.NextStage(); 
        }
    }
    public void OnDie()
    {

    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }

    public void ResetDouble()
    {
        isDoubleActivated = false;
    }

}
