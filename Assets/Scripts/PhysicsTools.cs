using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTools : MonoBehaviour
{
    public static Vector3 GetRandomPositionBetweenVectors(Vector3 first, Vector3 second)
    {
        return new Vector3(Random.Range(first.x, second.x), Random.Range(first.y, second.y), Random.Range(first.z, second.z));
    }
}