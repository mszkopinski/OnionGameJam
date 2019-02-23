using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemiesConfig", menuName = "Configs/Create Enemies Config")]
    public class EnemiesConfig : SerializedScriptableObject
    {
        public static EnemiesConfig Instance => 
            instance ? instance : instance = Instantiate(Resources.Load<EnemiesConfig>("EnemiesConfig"));

        static EnemiesConfig instance;

        [SerializeField] List<Enemy> enemies = new List<Enemy>();

        public GameObject GetEnemyPrefab(TileType tileType)
        {
            return enemies.FirstOrDefault(e => e.Type == tileType)?.Prefab;
        }
    }

    [Serializable]
    class Enemy
    {
        public GameObject Prefab;
        public TileType Type;
    }
}