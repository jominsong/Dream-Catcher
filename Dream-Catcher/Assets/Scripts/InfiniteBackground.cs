using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public Transform player;
    private float backgroundWidth;

    void Start()
    {
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        if (player.position.x > transform.position.x + backgroundWidth)
        {
            transform.position += new Vector3(backgroundWidth * 2f, 0, 0);
        }
        else if (player.position.x < transform.position.x - backgroundWidth)
        {
            transform.position -= new Vector3(backgroundWidth * 2f, 0, 0);
        }
    }
}
