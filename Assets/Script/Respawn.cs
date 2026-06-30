using UnityEngine;

public class Respawn : MonoBehaviour
{
    public LapCounter lapCounter;
    public Vector3 startPos;
    public Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConsTextKey.TegRespawnZone))
        {
            Vector3 targetPos;
            Quaternion targetRot;

            if (lapCounter != null && lapCounter.lastCheckpoint != null)
            {
                targetPos = lapCounter.lastCheckpoint.position;
                targetRot = lapCounter.lastCheckpoint.rotation;
            }
            else
            {
                targetPos = startPos;
                targetRot = startRot;
            }

            transform.position = targetPos + Vector3.up * 2f;
            transform.rotation = targetRot;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
