using UnityEngine;

public class Hooking : MonoBehaviour
{
    GrapplingHook grappling;
    public SpringJoint2D springJoint;
    private Rigidbody2D playerRb;
    

    void Start()
    {
        grappling = GameObject.Find("player").GetComponent<GrapplingHook>();
        playerRb = GameObject.Find("player").GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        springJoint.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ring 태그에만 반응
        if (collision.CompareTag("Ring"))
        {
            Debug.Log("Ring 충돌 감지됨");

            // 플레이어를 연결
            springJoint.connectedBody = playerRb;
            springJoint.autoConfigureDistance = false;
            springJoint.distance = Vector2.Distance(transform.position, playerRb.position);
            springJoint.dampingRatio = 0.7f;
            springJoint.frequency = 2.0f;
            springJoint.enabled = true;
            

            // Grappling 상태 설정
            grappling.isAttach = true;

            // Hook 이동 정지
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}
