using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class Barrel : MonoBehaviour
{
    int _angle;
    [NonSerialized] public GameObject _targetedBy = null;

    Dictionary<int, Vector3> _angleToVect = new Dictionary<int, Vector3>()
    {
        {-90, new Vector3(-2, 0, 0) },
        {-45, new Vector3(-2, 0, 2) },
        {0, new Vector3(0, 0, 2) },
        {45, new Vector3(2, 0, 2) },
        {90, new Vector3(2, 0, 0) },
        {135, new Vector3(2, 0, -2) },
        {180, new Vector3(0, 0, -2) },
        {225, new Vector3(-2, 0, -2) },
        {270, new Vector3(-2, 0, 0) },
        {315, new Vector3(-2, 0, 2) },
        {360, new Vector3(0, 0, 2) },
        {405, new Vector3(2, 0, 2) },
    };

    public void Spill(Vector3 direction)
    {
        _angle = (int)Vector2.Angle(new Vector2(1, 1), new Vector2(direction.x, direction.z));
        int offset = _angle % 45;

        if (Mathf.Abs(offset) < 23)
            _angle -= offset;
        else
            _angle += 45 - offset;

        gameObject.GetComponent<Animator>().SetTrigger("Spill");
        gameObject.GetComponent<Animator>().SetInteger("Xangle", _angle);        
    }

    public void Propagate()
    {
        Vector3 pointer;
        switch (_angle % 90)
        {
            // horizontal / vertical
            case 0:
                pointer = transform.position + _angleToVect[_angle];
                SpawnAcidOnTile(pointer);
                SpawnAcidOnTile(pointer+ _angleToVect[_angle]);
                SpawnAcidOnTile(pointer+ _angleToVect[_angle-90]);
                SpawnAcidOnTile(pointer+ _angleToVect[_angle+90]);
                

                break;

            // diagonal
            case 45:
                pointer = transform.position;
                SpawnAcidOnTile(pointer + _angleToVect[_angle]);
                SpawnAcidOnTile(pointer + _angleToVect[_angle - 45]);
                SpawnAcidOnTile(pointer + _angleToVect[_angle + 45]);
                break;

            default:
                print("SUSSY ANGLE THERE");
                break;
        }
    }

    void SpawnAcidOnTile(Vector3 pos)
    {
        if(Physics.Raycast(pos, Vector3.down, out RaycastHit hit))
            Instantiate(Resources.Load("Radioactive Zone/Interactibles/Prefabs/Acid"), hit.transform.position + new Vector3(0, 1.1f, 0), Quaternion.identity);
    }
}
