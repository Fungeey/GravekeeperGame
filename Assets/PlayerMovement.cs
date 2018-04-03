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
    public Vector3Int aheadTile; //represents the grid location of the tile in front

    public bool moving = false;

    // Use this for initialization
    void Start() {
        tilePos = levelGrid.WorldToCell(transform.position);
    }

    void Update() {
        if (Input.GetAxisRaw("Horizontal") == -1 && !moving) { // turn left
            direction = (Direction) (((int) direction + 3) % 4); // +3 rather than -1 because modulo is weird
            moving = true;
        } else if (Input.GetAxisRaw("Horizontal") == 1 && !moving) { // turn right
            direction = (Direction) (((int) direction + 1) % 4);
            moving = true;
        } else if (Input.GetAxisRaw("Vertical") == 1 && !moving) { // forwards
            if (CanMove(direction)) {
                tilePos += DirectionVector(direction);
                moving = true;
            }
        } else if (Input.GetAxisRaw("Vertical") == -1 && !moving) { // backwards
            if (CanMove((Direction) (((int) direction + 2) % 4))) {
                tilePos -= DirectionVector(direction);
                moving = true;
            }
        }

        Debug.DrawLine(transform.position, levelGrid.CellToWorld(aheadTile) + new Vector3(0.5f, 0.5f, 0), Color.red);
    }

    void FixedUpdate() {
        if (transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);
        } else if (Vector3Int.RoundToInt(transform.rotation.eulerAngles) != Vector3Int.RoundToInt(DirectionQuaternion(direction).eulerAngles)) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, DirectionQuaternion(direction), pivotSpeed);
        } else moving = false;
    }

    bool CanMove(Direction direction) {
        // replace with smarter system later
        // tile to move into
        aheadTile = tilePos + DirectionVector(direction);

        Debug.Log(tilemaps[1].HasTile(aheadTile));

        Ray ray = new Ray(transform.position, (levelGrid.CellToWorld(aheadTile) - transform.position) + new Vector3(0.5f, 0.5f, 0));

        // Check for wall tiles in Main layer
        // Raycast for interactable objects
        if (tilemaps[1].HasTile(aheadTile) || Physics2D.Raycast(transform.position, transform.up, 0.5f)) {
            return false;
        }

        if (tilemaps[0].HasTile(aheadTile)) { 
            return true;
        }
        
            
        return false;
    }

    Vector3Int DirectionVector(Direction direction) {
        // Correctly map directions from enum
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
