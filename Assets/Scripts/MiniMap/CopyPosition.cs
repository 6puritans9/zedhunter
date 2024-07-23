using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z;

    [SerializeField]
    private string targetTag;

    [SerializeField]
    private LayerMask targetLayer;

    private Transform target;

    private void Start()
    {
        StartCoroutine(FindTargetCoroutine());
    }

    private IEnumerator FindTargetCoroutine()
    {
        while (target == null)
        {
            TryFindTarget();
            yield return new WaitForSeconds(0.5f); // 0.5초마다 재시도
        }
    }

    private void TryFindTarget()
    {
        Debug.Log("Searching for object with tag: " + targetTag);
        // Debug.Log("Searching for object in layer: " + LayerMask.LayerToName(Mathf.Log(targetLayer.value, 2)));

        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            target = targetObject.transform;
            Debug.Log("Target found using tag: " + target.name);
        }
        else
        {
            GameObject[] objectsInLayer = FindObjectsInLayer(targetLayer);
            Debug.Log("Objects found in layer: " + objectsInLayer.Length);
            if (objectsInLayer.Length > 0)
            {
                target = objectsInLayer[0].transform;
                Debug.Log("Target found using layer: " + target.name);
            }
            else
            {
                Debug.LogWarning("No target found with specified tag or layer.");
            }
        }
    }

    private GameObject[] FindObjectsInLayer(LayerMask layer)
    {
        GameObject[] objectsInScene = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> objectsInLayer = new List<GameObject>();

        foreach (GameObject obj in objectsInScene)
        {
            if (((1 << obj.layer) & layer) != 0)
            {
                objectsInLayer.Add(obj);
                Debug.Log("Object in target layer: " + obj.name);
            }
        }

        return objectsInLayer.ToArray();
    }

    private void Update()
    {
        if (!target) return;

        transform.position = new Vector3(
            (x ? target.position.x : transform.position.x),
            (y ? target.position.y : transform.position.y),
            (z ? target.position.z : transform.position.z)
        );
    }
}