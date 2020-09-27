using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    public Vector3 Offset() {
        return target.position - transform.position;
    }
}
