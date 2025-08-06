using UnityEngine;

public class Destroyer : MonoBehaviour
{

    void Awake()
    {
        Destroy(GameObject.Find("summonedPlatform(Clone)"), 1f);
    }
}