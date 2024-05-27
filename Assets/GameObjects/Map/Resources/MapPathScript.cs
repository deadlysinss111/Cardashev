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
        GetComponent<SpriteShapeRenderer>().color = Color.blue;
    }

    public void LockPath()
    {
        GetComponent<SpriteShapeRenderer>().color = Color.grey;
    }

    public void SetShapePosition(Vector3 start, Vector3 end)
    {
        shapeController.spline.SetPosition(0, start);

        Vector3 mid = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2);
        shapeController.spline.SetPosition(1, mid);

        shapeController.spline.SetPosition(2, end);
    }
}
