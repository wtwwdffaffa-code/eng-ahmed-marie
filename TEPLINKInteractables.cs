using UnityEngine;

public class TEPLINKComputer : MonoBehaviour
{
    public int computerNumber = 1;
}
public class TEPLINKFoodSpot : MonoBehaviour { }
public class TEPLINKDrinkSpot : MonoBehaviour { }
public class TEPLINKBed : MonoBehaviour { }

public class TEPLINKPedestrian : MonoBehaviour
{
    private Vector3 target;
    private float changeTargetAt;
    private void Start() { PickTarget(); }
    private void Update()
    {
        if (Time.time > changeTargetAt || Vector3.Distance(transform.position, target) < .4f) PickTarget();
        Vector3 flat = target - transform.position; flat.y = 0;
        if (flat.sqrMagnitude > .01f)
        {
            transform.position += flat.normalized * 0.65f * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flat), Time.deltaTime * 3f);
        }
    }
    private void PickTarget()
    {
        target = new Vector3(Random.Range(-7.2f, 7.2f), 1f, Random.Range(-31f, 31f));
        changeTargetAt = Time.time + Random.Range(4f, 9f);
    }
}
