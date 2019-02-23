using System;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static readonly Color SelectedTile = Color.green;

        public static Color SelectedAndHighlightedTile
        {
            get
            {
                var newColor = Color.green;
                newColor.g = 0.7f;
                return newColor;
            }
        }

        public static readonly Color HighlightedTile = Color.red;
        
        public static void SetPosition(this Transform transform, float? newX, float? newY, float? newZ)
        {
            var newPosition = transform.position;
            newPosition.x = newX ?? newPosition.x;
            newPosition.y = newY ?? newPosition.y;
            newPosition.z = newZ ?? newPosition.z;
            transform.position = newPosition;
        }

        public static void ForEach(this TileType[,] array, Action<TileType> actionToInvoke)
        {
            if (array == null) return;
            
            int width = array.GetLength(0),
                height = array.GetLength(1);
            
            for (var x = 0; x < width; ++x) 
            {
                for (var y = 0; y < height; ++y)
                {
                    var tileType = array[x, y];
                    actionToInvoke?.Invoke(tileType);
                }
            }
        }
        
        public static void ForEach(this TileType[,] array, Action<TileType, Vector2Int> actionToInvoke)
        {
            if (array == null) return;
            
            int width = array.GetLength(0),
                height = array.GetLength(1);
            
            for (var x = 0; x < width; ++x) 
            {
                for (var y = 0; y < height; ++y)
                {
                    var tileType = array[x, y];
                    actionToInvoke?.Invoke(tileType, new Vector2Int(x, y));
                }
            }
        }
        
        public static int Count(this TileType[,] array, Predicate<TileType> predicate)
        {
            if (array == null) return 0;

            int count = 0;
            int width = array.GetLength(0),
                height = array.GetLength(1);
            
            for (var x = 0; x < width; ++x) 
            {
                for (var y = 0; y < height; ++y)
                {
                    if (predicate != null && predicate.Invoke(array[x, y]))
                    {
                        ++count;
                    }
                }
            }

            return count;
        }
    }
}