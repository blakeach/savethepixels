using UnityEngine;
using System.Collections.Generic;

public class ProjectilePooler : MonoBehaviour
{
    public static ProjectilePooler Instance;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 3;
    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledProjectile()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}