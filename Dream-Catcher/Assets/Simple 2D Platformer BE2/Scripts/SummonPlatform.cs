using UnityEngine;

public class SummonPlatform : MonoBehaviour
{

    public GameObject summonedPlatform;
    public Transform player;
    private float direction;
    public float summonPlatformCount;
    public PlayerMove playermove;
    void Start()
    {
    }

    void Update()
    {
        bool isFlipped = player.GetComponent<SpriteRenderer>().flipX;
        if (isFlipped == true)
            direction = -1;
        else
            direction = 1;

        if (Input.GetKeyDown(KeyCode.LeftShift) && GameObject.Find("summonedPlatform(Clone)") == null && summonPlatformCount < 1 && playermove.isOnFloor == 0)
        {
            summonPlatformCount++;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Vector3 spawnOffset = new Vector3(0f, -0.5f, 0f);
                Vector3 spawnPosition = player.position + spawnOffset;
                Instantiate(summonedPlatform, spawnPosition, Quaternion.Euler(0, 0, 90));
            }
            else
            {
                Vector3 spawnOffset = new Vector3(direction * 0.3f, 0f, 0f);
                Vector3 spawnPosition = player.position + spawnOffset;
                Instantiate(summonedPlatform, spawnPosition, Quaternion.identity);
            }
        }

    }

}
