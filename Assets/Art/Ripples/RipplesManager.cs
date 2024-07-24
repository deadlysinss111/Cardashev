using System.Collections.Generic;
using UnityEngine;

public class RipplesManager : MonoBehaviour
{
    public GameObject ripplePrefab; // Assign the ripple prefab in the Inspector
    private List<GameObject> rippleInstances = new List<GameObject>();

    public void CreateRipples(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            Vector3 ripplePosition = position;
            ripplePosition.y += 0.51f;
            GameObject rippleInstance = Instantiate(ripplePrefab, ripplePosition, Quaternion.identity);
            rippleInstances.Add(rippleInstance);
        }
    }

    public void ClearRipples()
    {
        foreach (GameObject rippleInstance in rippleInstances)
        {
            Destroy(rippleInstance);
        }
        rippleInstances.Clear();
    }
}