using Unity.VisualScripting;
using UnityEngine;
using System;
using JetBrains.Annotations;
#if UNITY_EDITOR
using static UnityEditor.Searcher.SearcherWindow.Alignment;
#endif

using Unity.XR.GoogleVr;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public GameManager GameManager;
    public float maxSpeed = 5;
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
    //double jump effect
    public Transform Double;
    public bool isDoubleActivated = false;
    private bool doubleEffectPlayed = false;
    public float isOnFloor;
    //sound effect
    public AudioClip clip;
    bool speedSoundPlayed = false;
    bool jumpSoundPlayed = false;
    //summonplatform
    public SummonPlatform summonplatform;
    private GrapplingHook grapplingHook;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    GrapplingHook grappling;


    void Awake()
    {
        grapplingHook = GetComponent<GrapplingHook>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        grappling = GetComponent<GrapplingHook>();
    }
    void Update()
    {
        float h = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            h = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            h = 1f;
        }

        Debug.DrawRay(rigid.position, Vector3.right, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector3.left, new Color(0, 1, 0));
        RaycastHit2D rayDown = Physics2D.Raycast(rigid.position, Vector3.down, 0.5f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayRight = Physics2D.Raycast(rigid.position, Vector3.right, 0.5f, LayerMask.GetMask("Platform"));
        RaycastHit2D rayLeft = Physics2D.Raycast(rigid.position, Vector3.left, 0.5f, LayerMask.GetMask("Platform"));
        //jump
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount && !grappling.isAttach)
        {
            rigid.linearVelocityY = 0;
            rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            animator.SetBool("is jumping", true);
            animator.SetBool("is diving", false);
            jumpCount++;
            JumpPower = 10;
            if (jumpCount <= 1)
            {
                SoundManager.instance.PlaySFX("Jump");
            }
            if (jumpCount == 2)
            {
                SoundManager.instance.PlaySFX("Double Jump");
            }

            //wall kick
            if (rayLeft.collider != null )
            {
                animator.SetBool("is wallkick", true);
                animator.SetBool("is jumping", false);
                wallIsRight = -1;
                afterWallJumpStiff = 20;
                spriteRenderer.flipX = false;
            }
            else if (rayRight.collider != null)
            {
                animator.SetBool("is wallkick", true);
                animator.SetBool("is jumping", false);
                rigid.AddForce(Vector2.left * wallJumpPower, ForceMode2D.Impulse);
                wallIsRight = 1;
                afterWallJumpStiff = 20;
                spriteRenderer.flipX = true;
            }
        }

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

        // flip
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX=true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;
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
                    SoundManager.instance.PlaySFX("Sliding");

                    if (rayDown.collider == null)
                    {
                        SoundManager.instance.PlaySFX("Diving");
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
        // double jump effect
        if (jumpCount == 2 && !doubleEffectPlayed)
        {
            Double doubleScript = Double.GetComponent<Double>();
            if (doubleScript != null && !doubleScript.isDoubleActivated)
            {
                doubleScript.Activate();
                doubleEffectPlayed = true;
            }
        }
        if (jumpCount < 2 && doubleEffectPlayed)
        {
            doubleEffectPlayed = false;
        }




        if (grappling.isAttach)
        {
            maxSpeed = 9f;
            rigid.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * 10, ForceMode2D.Force);
        }


        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // 시각 디버깅용

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform", "Speed", "Jump"));
        RaycastHit2D rayRight = Physics2D.Raycast(rigid.position, Vector2.right, 0.5f, LayerMask.GetMask("Platform", "Speed", "Jump"));
        RaycastHit2D rayLeft = Physics2D.Raycast(rigid.position, Vector2.left, 0.5f, LayerMask.GetMask("Platform", "Speed", "Jump"));

        //move speed
        float h = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            h = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            h = 1f;
        }      
        if (grappling.isAttach)
        {
            rigid.AddForce(Vector2.right * h * 10, ForceMode2D.Force);
        }


        //platform senser
        if (rigid.linearVelocity.y < 1)
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
            }
            
        }
        if (rayHit.collider == null)
            isOnFloor = 0;
        else
            isOnFloor = 1;

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

        RaycastHit2D raySpeed = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Speed"));
        if (raySpeed.collider != null && raySpeed.distance < 0.6f)
        {
            if (!speedSoundPlayed)
            {
                SoundManager.instance.PlaySFX("String5");
                speedSoundPlayed = true;
            }
            maxSpeed = 12;
        }
        else
        {
            speedSoundPlayed = false;
        }
        RaycastHit2D SpeedHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform", "Jump"));
        if (SpeedHit.collider != null && SpeedHit.distance < 0.6f)
        {
            maxSpeed = 5;
        }


            RaycastHit2D rayJump = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Jump"));
            if (rayJump.collider != null && rayJump.distance < 0.6f)
            {
                if (!jumpSoundPlayed)
                {
                    SoundManager.instance.PlaySFX("String6");
                    jumpSoundPlayed = true;
                }
                JumpPower = 20;
            }
            else
            {
                jumpSoundPlayed = false;
                JumpPower = 10;
            }

        if (dashTime == 0)
        {
            animator.SetBool("is sliding", false);

            if (rigid.linearVelocity.x > maxSpeed)
                rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocityY);
            else if (rigid.linearVelocity.x < -maxSpeed)
                rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocityY);
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
    private IEnumerator ReactivateAfterDelay(GameObject obj, float delay)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Reset"))
        {
            summonplatform.summonPlatformCount = 0;
            StartCoroutine(ReactivateAfterDelay(collision.gameObject, 3f));
        }
        if (collision.gameObject.tag == "Item")
        {
            //point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            bool isBigGold = collision.gameObject.name.Contains("Coin");
            if (isBronze)
                GameManager.stagePoint += 100;
            else if (isSilver)
                GameManager.stagePoint += 500;
            else if (isGold)
            {
                GameManager.stagePoint += 1000;
                SoundManager.instance.PlaySFX("Gold");
            }    
            else if (isBigGold)
            {
                GameManager.stagePoint += 5000;
                SoundManager.instance.PlaySFX("Big Gold");
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //Next stage
            GameManager.NextStage();
        }
        else if (collision.gameObject.tag == "Fragment")
        {
            SoundManager.instance.PlaySFX("Fragment");
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
