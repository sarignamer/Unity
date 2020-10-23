using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector2Extensions
    {
        public static Vector3 Vector3XZ(this Vector2 vec)
        {
            return new Vector3(vec.x, 0, vec.y);
        }
    }
}
