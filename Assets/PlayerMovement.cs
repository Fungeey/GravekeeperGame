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

    [HideInInspector]
    public bool isHolding = false;
    public GameObject holdObject; // Object being held (if any)
    public Transform holdPosition;

    public bool moving = false;

    // Use this for initialization
    void Start() {
        tilePos = levelGrid.WorldToCell(transform.position);
        aheadTile = tilePos + DirectionVector(direction);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !moving) { // turn left
            direction = (Direction) (((int) direction + 3) % 4); // +3 rather than -1 because modulo is weird
            moving = true;
        } else if (Input.GetKeyDown(KeyCode.RightArrow) && !moving) { // turn right
            direction = (Direction) (((int) direction + 1) % 4);
            moving = true;
        } else if (Input.GetKeyDown(KeyCode.UpArrow) && !moving) { // forwards
            if (CanMove(direction)) {
                tilePos += DirectionVector(direction);
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && !moving) { // backwards
            if (CanMove((Direction) (((int) direction + 2) % 4))) {
                tilePos -= DirectionVector(direction);
                moving = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z)) { // Start holding
            isHolding = CanGrab(direction);
        } else if(Input.GetKeyUp(KeyCode.Z)) { // Stop holding
            isHolding = false;
            holdObject = null;
        }

        Debug.DrawLine(transform.position, levelGrid.CellToWorld(aheadTile) + new Vector3(0.5f, 0.5f, 0), Color.red);
    }

    void FixedUpdate() {
        if (transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);

            if (isHolding) {
                holdObject.transform.position = Vector3.MoveTowards(holdObject.transform.position, holdPosition.position, moveSpeed);
            }
        } else if (Vector3Int.RoundToInt(transform.rotation.eulerAngles) != Vector3Int.RoundToInt(DirectionQuaternion(direction).eulerAngles)) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, DirectionQuaternion(direction), pivotSpeed);

            if (isHolding) {
                Debug.Log("Turning");
                holdObject.transform.position = Vector3.MoveTowards(holdObject.transform.position, holdPosition.position, moveSpeed*3);
            }
        } else moving = false;
    }

    bool CanMove(Direction direction) {
        // replace with smarter system later
        // tile to move into

        // Check for wall tiles in Main layer
        // Raycast for interactable objects\

        aheadTile = tilePos + DirectionVector(direction);

        Vector2 rayDirection = new Vector2(DirectionVector(direction).x, DirectionVector(direction).y);
        if (tilemaps[1].HasTile(aheadTile) || (Physics2D.Raycast(transform.position, rayDirection, 1f) && !isHolding)) {
            return false;
        }

        if (tilemaps[0].HasTile(aheadTile)) { 
            return true;
        }
        
            
        return false;
    }

    bool CanGrab(Direction direction) {
        if (isHolding) {
            return true;
        }
        Vector2 rayDirection = new Vector2(DirectionVector(direction).x, DirectionVector(direction).y);
        if (Physics2D.Raycast(transform.position, rayDirection, 1f)) {
            holdObject = Physics2D.Raycast(transform.position, rayDirection, 1f).collider.gameObject;
            return true;
        }
        return false;
    }

    /*bool CanTurn(Direction direction) {
        Vector3Int dir = DirectionVector(direction);
        if (direction == Direction.LEFT) {
            if(!IsSolidAtPos(tilePos + dir))
        }else if(direction == Direction.RIGHT) {

        }
    }
    */

    bool IsSolidAtPos(Vector3Int pos) {
        Vector2 rayPos = new Vector2(pos.x, pos.y);
        if (tilemaps[1].HasTile(pos) || Physics2D.Raycast(rayPos, Vector2.up, 0.1f)) {
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
