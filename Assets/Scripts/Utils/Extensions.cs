using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static void SetPosition(this Transform transform, float? newX, float? newY, float? newZ)
        {
            var newPosition = transform.position;
            newPosition.x = newX ?? newPosition.x;
            newPosition.y = newY ?? newPosition.y;
            newPosition.z = newZ ?? newPosition.z;
            transform.position = newPosition;
        }
    }
}