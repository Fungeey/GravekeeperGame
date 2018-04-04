using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour {
    [SerializeField]
    private float moveSpeed = 0.1f;
    [SerializeField]
    private float pivotSpeed = 15f;

    public Grid levelGrid;
    public Tilemap[] tilemaps; // 0, 1: ground, main

    public Directions facing; // represents direction player is facing
    public Directions moveDirection; // represents 
    public Vector3Int tilePos; // represents location on levelGrid
    public Vector3Int aheadTile; //represents the grid location of the tile in front

    [HideInInspector]
    public GameObject holdObject; // Object being held (if any)
    public Transform holdPosition;

    public bool moving = false;
    public Directions turning = Directions.UP;

    // Use this for initialization
    void Start() {
        tilePos = levelGrid.WorldToCell(transform.position);
        aheadTile = tilePos + Utility.DirectionVector(facing);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A) && !moving) { // turn left
            if (CanTurn(facing, Directions.LEFT)) {
                facing = Utility.DirAdd(facing, Directions.LEFT);
                turning = Directions.LEFT;
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.D) && !moving) { // turn right
            if (CanTurn(facing, Directions.RIGHT)) {
                facing = Utility.DirAdd(facing, Directions.RIGHT);
                turning = Directions.RIGHT;
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.W) && !moving) { // forwards
            if (CanMove(facing)) {
                tilePos += Utility.DirectionVector(facing);
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.S) && !moving) { // backwards
            if (CanMove(Utility.DirAdd(facing, Directions.DOWN))) {
                tilePos -= Utility.DirectionVector(facing);
                moving = true;
            }
        }
        if (!moving) {
            moveDirection = facing;
        }
        if (Input.GetKeyDown(KeyCode.Z)) { // toggle hold
            if (holdObject == null) {
                Grab(facing);
            } else {
                Drop();
            }
        }

        //Debug.DrawLine(transform.position, levelGrid.CellToWorld(aheadTile) + new Vector3(0.5f, 0.5f, 0));
    }

    void FixedUpdate() {
        if (transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);

            if (holdObject != null) {
                holdObject.transform.position = Vector3.MoveTowards(holdObject.transform.position, holdPosition.position, moveSpeed);
            }
        } else if (Vector3Int.RoundToInt(transform.rotation.eulerAngles) != Vector3Int.RoundToInt(Utility.DirectionQuaternion(facing).eulerAngles)) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Utility.DirectionQuaternion(facing), pivotSpeed);

            if (holdObject != null) {
                Debug.Log("Turning");
                holdObject.transform.position = Vector3.MoveTowards(holdObject.transform.position, holdPosition.position, moveSpeed * 3);
            }
        } else moving = false;
    }

    bool CanMove(Directions faceDir) {
        // replace with smarter system later
        // tile to move into

        // Check for wall tiles in Main layer
        // Raycast for interactable objects

        aheadTile = tilePos + Utility.DirectionVector(faceDir);

        if (!tilemaps[0].HasTile(aheadTile)) {
            return false;
        }

        if (holdObject == null) {
            if (Utility.IsSolidAtPos(tilemaps, aheadTile)) {
                return false;
            }
        } else {
            if (Utility.IsSolidAtPos(tilemaps, aheadTile + Utility.DirectionVector(faceDir))) {
                return false;
            }
        }
        
        return true;
    }

    void Grab(Directions faceDir) {
        if (holdObject == null) {
            Vector2 rayDirection = (Vector3)Utility.DirectionVector(faceDir);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, 1f);
            if (hit.collider != null) {
                holdObject = hit.collider.gameObject;
            }
        }
        
    }

    void Drop() {
        holdObject = null;
    }

    bool CanTurn(Directions faceDir, Directions turnDirection) {
        //Debug.DrawLine(levelGrid.CellToWorld(tilePos), levelGrid.CellToWorld(tilePos + DirectionVector(faceDir) + DirectionVector(DirAdd(faceDir, turnDirection))), Color.red, 10000);
        if (holdObject != null) {
            if (Utility.IsSolidAtPos(tilemaps, tilePos + Utility.DirectionVector(Utility.DirAdd(faceDir, turnDirection))) || Utility.IsSolidAtPos(tilemaps, tilePos + Utility.DirectionVector(faceDir) + Utility.DirectionVector(Utility.DirAdd(faceDir, turnDirection)))) {
                return false;
            }
        }
        return true;

        /*
        Vector3Int dir = DirectionVector(direction);
        if (turnDirection == Direction.LEFT) {
            if (!IsSolidAtPos(tilePos + DirectionVector((Direction)(((int)direction + 3) % 4)) + DirectionVector(direction))) { // Up and left (relative)
                if (tilemaps[0].HasTile((tilePos + DirectionVector((Direction)(((int)direction + 3) % 4))))) { // Left (relative)
                    return true;
                }
            }
        } else if (turnDirection == Direction.RIGHT) {
            if (!IsSolidAtPos(tilePos + DirectionVector((Direction)(((int)direction + 1) % 4)))) {
                Vector3 pos = levelGrid.CellToWorld(tilePos + DirectionVector(DirAdd(direction, 1)));
                Debug.DrawLine(pos, pos + new Vector3(0.2f, 0, 0), Color.red, 10f);
                if (tilemaps[0].HasTile(tilePos + DirectionVector(DirAdd(direction, 3)) + DirectionVector(direction))) {
                    Vector3 pos2 = levelGrid.CellToWorld(tilePos + DirectionVector(DirAdd(direction, 1)));
                    Debug.DrawLine(pos2, pos2 + new Vector3(0.2f, 0, 0), Color.green, 10f);
                    return true;
                }
            }
        }
        return false;
        */
    }


    
}
