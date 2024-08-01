using System.Collections.Generic;
using UnityEngine;

public class RipplesManager : MonoBehaviour
{
    public GameObject _ripplePrefab;
    private List<GameObject> _rippleInstances = new List<GameObject>();

    public void CreateRipples(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            Vector3 ripplePosition = position;
            //ripplePosition.y += 0.51f;
            GameObject rippleInstance = Instantiate(_ripplePrefab, ripplePosition, Quaternion.identity);
            _rippleInstances.Add(rippleInstance);
        }
    }

    public void ClearRipples()
    {
        Destroy(_rippleInstances[0]);
        _rippleInstances.RemoveAt(0);
    }
}