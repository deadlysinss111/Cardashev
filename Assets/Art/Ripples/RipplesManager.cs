using System.Collections.Generic;
using UnityEngine;

public class RipplesManager : MonoBehaviour
{
    public GameObject ripplePrefab;
    private List<GameObject> rippleInstances = new List<GameObject>();

    public void CreateRipples(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            print("hh");
            Vector3 ripplePosition = position;
            ripplePosition.y += 0.51f;
            GameObject rippleInstance = Instantiate(ripplePrefab, ripplePosition, Quaternion.identity);
            rippleInstances.Add(rippleInstance);
        }
    }

    public void ClearRipples()
    {
        Destroy(rippleInstances[0]);
        rippleInstances.RemoveAt(0);
    }
}