using UnityEngine;

public static class VectorUtils
{
    public static Vector3 HorizontalDirection(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        direction.y = 0;
        return direction;
    }

    public static void SetHorizontalComponent(ref Vector3 vector, Vector3 newHorizontalComponent)
    {
        vector.x = newHorizontalComponent.x;
        vector.z = newHorizontalComponent.z;
    }
}
