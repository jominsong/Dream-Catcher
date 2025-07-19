using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public LineRenderer line;
    public Transform hook;
    public bool isHookActivate;
    public bool isLineMax;
    public bool isAttach;

    Vector2 hookDir;
    Transform targetRing;

    void Start()
    {
        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.05f;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);
        line.useWorldSpace = true;
        isAttach = false;
    }

    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);

        if (Input.GetKeyUp(KeyCode.E) && !isHookActivate)
        {
            targetRing = FindNearestRing();

            if (targetRing != null)
            {
                hook.position = transform.position;
                hookDir = ((Vector2)targetRing.position - (Vector2)transform.position).normalized;
                isHookActivate = true;
                isLineMax = false;
                hook.gameObject.SetActive(true);
            }
        }

        if (isHookActivate && !isLineMax && !isAttach)
        {
            hook.Translate(hookDir * Time.deltaTime * 30);

            if (Vector2.Distance(transform.position, hook.position) > 5)
            {
                isLineMax = true;
            }
        }
        else if (isHookActivate && isLineMax && !isAttach)
        {
            hook.position = Vector2.MoveTowards(hook.position, transform.position, Time.deltaTime * 30);
            if (Vector2.Distance(transform.position, hook.position) < 0.1f)
            {
                isHookActivate = false;
                isLineMax = false;
                hook.gameObject.SetActive(false);
            }
        }
        else if (isAttach)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isAttach = false;
                isHookActivate = false;
                isLineMax = false;
                hook.GetComponent<Hooking>().springJoint.enabled = false;
                hook.gameObject.SetActive(false);
            }
        }
    }

    // 가장 가까운 Ring 오브젝트 찾기
    Transform FindNearestRing()
    {
        GameObject[] rings = GameObject.FindGameObjectsWithTag("Ring");
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject ring in rings)
        {
            float dist = Vector2.Distance(transform.position, ring.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = ring.transform;
            }
        }

        return nearest;
    }
}
