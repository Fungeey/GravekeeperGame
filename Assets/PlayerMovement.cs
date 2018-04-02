using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField]
    private float movSpeed = 0.1f;
    [SerializeField]
    private float rotSpeed = 15f;

    private Vector3 targetPos;
    private Quaternion targetRot;
    
    private bool up, down, left, right;

    // Use this for initialization
    void Start () {
        targetPos = transform.position;
        targetRot = transform.rotation;
	}

    private void Update() {
        up = Input.GetKeyDown(KeyCode.W);
        left = Input.GetKeyDown(KeyCode.A);
        down = Input.GetKeyDown(KeyCode.S);
        right = Input.GetKeyDown(KeyCode.D);
    }
    
    void FixedUpdate () {
        bool canMove = transform.position == targetPos && transform.rotation == targetRot;

        if (left && canMove) {
            left = false;
            RoundTransform();
            targetRot.eulerAngles += new Vector3(0, 0, 90);
        } else if (right && canMove) {
            right = false;
            RoundTransform();
            targetRot.eulerAngles += new Vector3(0, 0, -90);
        } else if (up && canMove) {
            up = false;
            RoundTransform();
            targetPos += transform.up.normalized;
        } else if (down && canMove) {
            down = false;
            RoundTransform();
            targetPos -= transform.up.normalized;
        }
        
        transform.position = Vector3.MoveTowards(transform.position, targetPos, movSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed);

	}

    Vector3 SnapToGrid(Vector3 position) {
        return new Vector3Int (Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
    }

    Quaternion SnapToRight(Quaternion rotation) {
        Vector3 angles = rotation.eulerAngles;
        angles = new Vector3Int(Mathf.RoundToInt(angles.x), Mathf.RoundToInt(angles.y), Mathf.RoundToInt(angles.z));
        return Quaternion.Euler(angles);
    }

    void RoundTransform() {
        transform.position = SnapToGrid(transform.position);
        transform.rotation = SnapToRight(transform.rotation);
    }
}
