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
        if (collision.CompareTag("Ring"))
        {
            Debug.Log("Ring Ãæµ¹ °¨ÁöµÊ");

            springJoint.connectedBody = playerRb;
            springJoint.autoConfigureDistance = false;
            springJoint.distance = Vector2.Distance(transform.position, playerRb.position);
            springJoint.dampingRatio = 0.7f;
            springJoint.frequency = 2.0f;
            springJoint.enabled = true;
            grappling.isAttach = true;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}
