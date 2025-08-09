using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int dreamPoint;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public TextMeshProUGUI stagePointText;
    public TextMeshProUGUI totalPointText;

    void Update()
    {
        // UI 업데이트
        stagePointText.text = "Stage Score: " + stagePoint.ToString();
        totalPointText.text = "Total Score: " + totalPoint.ToString();
    }

    public void NextStage()
    {
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        else
        {
            Time.timeScale = 0;
            Debug.Log("게임클리어");
        }

        totalPoint += stagePoint;
        stagePoint = 0;
        dreamPoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
            health--;
        else
        {
            player.OnDie();
            Debug.Log("죽었습니다");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
            HealthDown();

        PlayerReposition();
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 1, -1);
        player.VelocityZero();
        player.summonplatform.summonPlatformCount = 0;
    }
}
