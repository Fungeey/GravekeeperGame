using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public enum Direction {
        UP, RIGHT, DOWN, LEFT
    }

    [SerializeField]
    private float moveSpeed = 0.1f;
    [SerializeField]
    private float pivotSpeed = 15f;

    public Grid levelGrid;

    public Direction direction; // represents direction player is facing
    public Vector3Int targetPos; // represents location on levelGrid

    private bool moving = false;

    // Use this for initialization
    void Start() {
        targetPos = levelGrid.WorldToCell(transform.position);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) { // turn left
            direction = (Direction) (((int) direction + 3) % 4); // +3 rather than -1 because modulo is weird
        } else if (Input.GetKeyDown(KeyCode.D)) { // turn right
            direction = (Direction) (((int) direction + 1) % 4); 
        } else if (Input.GetKeyDown(KeyCode.W)) { // forwards
            targetPos += DirectionVector(direction);
        } else if (Input.GetKeyDown(KeyCode.S)) { // backwards
            targetPos -= DirectionVector(direction);
        }
    }

    void FixedUpdate() {
        if (transform.position != levelGrid.CellToWorld(targetPos)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(targetPos), moveSpeed);
        } else if (transform.rotation.eulerAngles != DirectionVector(direction)) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, DirectionQuaternion(direction), pivotSpeed);
        }

        //transform.position = levelGrid.CellToWorld(targetPos);
    }

    Vector3 SnapToGrid(Vector3 position) {
        return levelGrid.CellToWorld(levelGrid.WorldToCell(position));
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

    bool CanMove(Vector3 direction) {

        return false;
    }

    Vector3Int DirectionVector(Direction direction) {
        switch (direction) {
            case Direction.UP:
                return Vector3Int.up;
            case Direction.RIGHT:
                return Vector3Int.right;
            case Direction.DOWN:
                return Vector3Int.down;
            case Direction.LEFT:
                return Vector3Int.left;
            default:
                return Vector3Int.zero;
        }
    }

    Quaternion DirectionQuaternion(Direction direction) {
        return Quaternion.Euler(0, 0, ((int) direction * -90) % 360);
    }
}
