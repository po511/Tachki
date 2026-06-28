using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    public static Dictionary<int, Transform> all = new Dictionary<int, Transform>();

    void OnEnable()
    {
        if (!all.ContainsKey(checkpointID))
            all.Add(checkpointID, transform);
    }

    void OnDisable()
    {
        all.Remove(checkpointID);
    }
}