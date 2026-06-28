using UnityEngine;
using System.Collections.Generic;

public class TreeSpawner : MonoBehaviour
{
    public GameObject groundPlane;
    public GameObject[] firPrefabs;
    public GameObject[] treePrefabs;
    public int treeCount = 30;
    public float edgeWidth = 4f;
    public float minDistance = 3f;
    public float heightOffset = 0.1f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    void Start()
    {
        SpawnTrees();
    }

    void SpawnTrees()
    {
        if (groundPlane == null) return;

        Vector3 pos = groundPlane.transform.position;
        Vector3 scale = groundPlane.transform.localScale;
        float halfW = 5f * scale.x;
        float halfH = 5f * scale.z;

        List<Vector3> spawned = new List<Vector3>();

        for (int i = 0; i < treeCount; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                Vector3 point = RandomPointOnEdge(pos, halfW, halfH);

                bool tooClose = false;
                foreach (Vector3 s in spawned)
                {
                    if (Vector3.Distance(point, s) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    spawned.Add(point);
                    SpawnOneTree(point);
                    break;
                }
            }
        }
    }

    Vector3 RandomPointOnEdge(Vector3 center, float halfW, float halfH)
    {
        float x = 0f, z = 0f;

        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // верх
                x = Random.Range(-halfW, halfW);
                z = Random.Range(halfH - edgeWidth, halfH);
                break;
            case 1: // низ
                x = Random.Range(-halfW, halfW);
                z = Random.Range(-halfH, -halfH + edgeWidth);
                break;
            case 2: // лево
                x = Random.Range(-halfW, -halfW + edgeWidth);
                z = Random.Range(-halfH, halfH);
                break;
            case 3: // право
                x = Random.Range(halfW - edgeWidth, halfW);
                z = Random.Range(-halfH, halfH);
                break;
        }

        Vector3 world = new Vector3(center.x + x, center.y + 50f, center.z + z);

        if (Physics.Raycast(world, Vector3.down, out RaycastHit hit, 100f))
        {
            if (hit.collider.gameObject == groundPlane)
                return hit.point + Vector3.up * heightOffset;
        }

        return new Vector3(center.x + x, center.y + heightOffset, center.z + z);
    }

    void SpawnOneTree(Vector3 position)
    {
        GameObject prefab = null;

        if (firPrefabs.Length > 0 && treePrefabs.Length > 0)
            prefab = Random.value < 0.5f ? firPrefabs[Random.Range(0, firPrefabs.Length)] : treePrefabs[Random.Range(0, treePrefabs.Length)];
        else if (firPrefabs.Length > 0)
            prefab = firPrefabs[Random.Range(0, firPrefabs.Length)];
        else if (treePrefabs.Length > 0)
            prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        else
            return;

        float rotation = Random.Range(0f, 360f);
        float s = Random.Range(minScale, maxScale);
        GameObject tree = Instantiate(prefab, position, Quaternion.Euler(0, rotation, 0));
        tree.transform.localScale = Vector3.one * s;
    }
}
