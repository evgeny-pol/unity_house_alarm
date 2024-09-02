using System.Linq;
using UnityEngine;

public static class GameObjectUtils
{
    public static Transform[] GetChildren(Transform obj)
    {
        return obj.Cast<Transform>().ToArray();
    }
}
