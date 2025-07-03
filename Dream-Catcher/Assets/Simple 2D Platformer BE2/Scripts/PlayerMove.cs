using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMove : MonoBehaviour
{
    public GameManager GameManager;
    private float maxSpeed = 5;
    private float JumpPower = 10;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("is jumping"))
        {
            rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            animator.SetBool("is jumping", true);
        }

        //stop speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 1.5f, rigid.linearVelocity.y);
        }

        //direction sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //walk animation
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.7)
            animator.SetBool("is walking", false);
        else
            animator.SetBool("is walking", true);
        animator.speed = maxSpeed / 7f;
    }
    void FixedUpdate()
    {
        //move speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.linearVelocity = new Vector2(h * maxSpeed, rigid.linearVelocity.y);

        if (rigid.linearVelocity.x > maxSpeed)
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
        else if (rigid.linearVelocity.x < -maxSpeed)
            rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);

        //Landing Platform
        if (rigid.linearVelocity.y < 0)
        {
            Debug.DrawRay(rigid.position + Vector2.down * 0.5f, Vector2.down * 1.2f, Color.yellow); // 시각 디버깅용

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position + Vector2.down * 0.5f, Vector2.down, 1.2f, LayerMask.GetMask("Platform", "Speed", "Jump"));

            if (rayHit.collider != null && rayHit.distance < 0.6f)
                animator.SetBool("is jumping", false);
        }


        // Super Jump/Speed
        if (rigid.linearVelocity.y < 0)
        {

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Speed"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.6f)
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

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Jump"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.6f)
                    JumpPower = 20;
            }
        }
        RaycastHit2D JumpHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform", "Speed"));
        if (JumpHit.collider != null && JumpHit.distance < 0.6f)
        {
            JumpPower = 10;
        }

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
    }
    public void OnDie()
    {

    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }
    


}
