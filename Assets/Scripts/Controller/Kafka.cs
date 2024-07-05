using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kafka : MonoBehaviour {
    public Animator anim;
    public CharacterController characterController;

    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;
    
    
    // Start is called before the first frame update
    void Start() {
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            anim.SetTrigger("Walking");            
        }
        
        // Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // if (direction.sqrMagnitude > 0.01f) {
        //     Vector3 forward = Vector3.Slerp(transform.forward, direction,
        //         rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction));
        //     transform.LookAt(transform.position + forward);
        // }
        //
        // characterController.Move(direction * moveSpeed * Time.deltaTime);
        // anim.SetFloat("Speed", characterController.velocity.magnitude);
    }
}
