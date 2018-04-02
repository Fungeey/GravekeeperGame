using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField]
    private float speed = 0.5f;
    private Vector3 targetPos;

	// Use this for initialization
	void Start () {
        targetPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKeyDown(KeyCode.W) && transform.position == targetPos) {
            targetPos += Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.A) && transform.position == targetPos) {
            targetPos += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S) && transform.position == targetPos) {
            targetPos += Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.D) && transform.position == targetPos) {
            targetPos += Vector3.right;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
	}
}
