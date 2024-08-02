using System.Collections.Generic;
using UnityEngine;

public class JackHammer : MonoBehaviour
{
    Dictionary<int, Vector3> _intToVector = new Dictionary<int, Vector3>()
    {
        {0, new Vector3(0, 0, 2) },
        {1, new Vector3(2, 0, 0) },
        {2, new Vector3(0, 0, -2) },
        {3, new Vector3(-2, 0, 0) },
    };

    [SerializeField] float _fuel = 35;
    [SerializeField] int _dmg = 25;

    private void Update()
    {
        _fuel -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider target)
    {
        if(target.gameObject.TryGetComponent(out StatManager statManager))
        {
            statManager.TakeDamage(_dmg);
        }
    }

    public void Operating(bool state)
    {
        HierarchySearcher.FindChildRecursively(transform, "Animator").GetComponent<Animator>().SetBool("Bounce", state);
    }

    public void Move()
    {
        Vector3 target = transform.position + _intToVector[UnityEngine.Random.Range(0, 4)];
        Physics.Raycast(target + new Vector3(0, 2, 0), Vector3.down, out RaycastHit hit);
        if (hit.transform == null) return;
        //print(hit.transform.gameObject.name);
        if (hit.transform.gameObject.CompareTag("TMTopology"))
            //GetComponent<Rigidbody>().AddForce(_intToVector[UnityEngine.Random.Range(0, 4)]);
            //GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -200));
            transform.position = target;
        CheckForFuel();
    }

    void CheckForFuel()
    {
        if (_fuel <= 0)
        {
            HierarchySearcher.FindChildRecursively(transform, "Animator").GetComponent<Animator>().SetInteger("Debris", UnityEngine.Random.Range(0, 2)==0? 90 : -90);
            Operating(false);
            Destroy(GetComponent<StatManager>());
        }
    }
}
