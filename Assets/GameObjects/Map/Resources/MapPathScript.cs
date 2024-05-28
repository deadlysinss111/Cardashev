using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MapPathScript : MonoBehaviour
{
    SpriteShapeController shapeController;

    private void Awake()
    {
        shapeController = GetComponent<SpriteShapeController>();
    }

    private void Start()
    {
        GetComponent<SpriteShapeRenderer>().color = Color.blue;
    }

    public void SetPathPoints(GameObject point1, GameObject point2)
    {
        SetShapePosition(point1.transform.position, point2.transform.position);

    }

    public void LockPath()
    {
        GetComponent<SpriteShapeRenderer>().color = Color.grey;
    }

    void SetShapePosition(Vector3 start, Vector3 end)
    {
        shapeController.spline.SetPosition(0, start);

        Vector3 mid = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2);
        transform.localPosition = mid;
        shapeController.spline.SetPosition(1, mid);

        shapeController.spline.SetPosition(2, end);
    }
}
