using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour {
    public enum Direction {
        UP, RIGHT, DOWN, LEFT
    }

    [SerializeField]
    private float moveSpeed = 0.1f;
    [SerializeField]
    private float pivotSpeed = 15f;

    public Grid levelGrid;
    public Tilemap[] tilemaps; // 0, 1: ground, main

    public Direction direction; // represents direction player is facing
    public Vector3Int tilePos; // represents location on levelGrid

    public bool moving = false;

    // Use this for initialization
    void Start() {
        tilePos = levelGrid.WorldToCell(transform.position);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A) && !moving) { // turn left
            direction = (Direction) (((int) direction + 3) % 4); // +3 rather than -1 because modulo is weird
            moving = true;
        } else if (Input.GetKeyDown(KeyCode.D) && !moving) { // turn right
            direction = (Direction) (((int) direction + 1) % 4);
            moving = true;
        } else if (Input.GetKeyDown(KeyCode.W) && !moving) { // forwards
            if (CanMove(direction)) {
                tilePos += DirectionVector(direction);
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.S) && !moving) { // backwards
            if (CanMove((Direction) (((int) direction + 2) % 4))) {
                tilePos -= DirectionVector(direction);
                moving = true;
            }
        }
    }

    void FixedUpdate() {
        if (transform.position != levelGrid.CellToWorld(tilePos)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos), moveSpeed);
        } else if (Vector3Int.RoundToInt(transform.rotation.eulerAngles) != Vector3Int.RoundToInt(DirectionQuaternion(direction).eulerAngles)) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, DirectionQuaternion(direction), pivotSpeed);
        } else moving = false;

        //transform.position = levelGrid.CellToWorld(targetPos);
    }

    bool CanMove(Direction direction) {
        // replace with smarter system later
        // tile to move into
        Vector3Int aheadTile = tilePos + DirectionVector(direction);
        Debug.Log(tilemaps[1].HasTile(aheadTile));
        if (tilemaps[1].HasTile(aheadTile)) {
            return false;
        }

        if (tilemaps[0].HasTile(aheadTile)) {
            return true;
        }
        
            
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
