
using UnityEngine;

static class Extend
{

    public static float DistanceSquare(this Vector3 vector3,  Vector3 p1, Vector3 p2)
    {
        return (p1.x - p2.x) * (p1.x - p2.x) + (p1.z - p2.z) * (p1.z - p2.z);
    }

}