using UnityEngine;

public class SummonPlatform : MonoBehaviour
{

    public GameObject summonedPlatform;
    public Transform player;
    private float direction;
    public float summonPlatformCount;
    public PlayerMove playermove;
    public Sprite defaultSprite;
    public Sprite downSprite;
    public bool IsSummond;
    GrapplingHook grapplingHook;

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

        if (Input.GetKeyDown(KeyCode.LeftShift) && GameObject.Find("summonedPlatform(Clone)") == null && summonPlatformCount < 1 && playermove.isOnFloor == 0 )
        {
            summonPlatformCount++;
            IsSummond = true;
            GameObject newPlatform; 

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Vector3 spawnOffset = new Vector3(0f, -0.7f, 0f);
                Vector3 spawnPosition = player.position + spawnOffset;
                newPlatform = Instantiate(summonedPlatform, spawnPosition, Quaternion.identity); 
                SpriteRenderer sr = newPlatform.GetComponent<SpriteRenderer>();
                if (sr != null && downSprite != null)
                    sr.sprite = downSprite;
            }
            else
            {
                Vector3 spawnOffset = new Vector3(direction * 1f, 0f, 0f);
                Vector3 spawnPosition = player.position + spawnOffset;
                newPlatform = Instantiate(summonedPlatform, spawnPosition, Quaternion.Euler(0, 0, 90)); 
                SpriteRenderer sr = newPlatform.GetComponent<SpriteRenderer>();
                if (sr != null && defaultSprite != null)
                    sr.sprite = defaultSprite;
            }
        }


    }

}