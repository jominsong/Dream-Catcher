using UnityEngine;

public class Destroyer : MonoBehaviour
{
    SummonPlatform summonplatform;

    void Awake()
    {
        Destroy(GameObject.Find("summonedPlatform(Clone)"), 1f);
        if (summonplatform != null)
            summonplatform.summonPlatformCount = 0f;
    }
}