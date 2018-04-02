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
            targetPos += transform.up;
        }
        if (Input.GetKeyDown(KeyCode.A) && transform.position == targetPos) {
            transform.Rotate(new Vector3(0, 0, 90));
        }
        if (Input.GetKeyDown(KeyCode.S) && transform.position == targetPos) {
            targetPos -= transform.up;
        }
        if (Input.GetKeyDown(KeyCode.D) && transform.position == targetPos) {
            transform.Rotate(new Vector3(0, 0, -90));
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
	}
}
