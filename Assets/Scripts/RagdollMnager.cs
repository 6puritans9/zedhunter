using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollMnager : MonoBehaviour
{
    Rigidbody[] rbs;

    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs) rb.isKinematic = true;
    }

	public void TriggerRagdoll()
    {
		foreach (Rigidbody rb in rbs) rb.isKinematic = false;
	}
}
