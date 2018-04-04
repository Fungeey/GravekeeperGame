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

    public Direction facing; // represents direction player is facing
    public Direction moveDirection; // represents 
    public Vector3Int tilePos; // represents location on levelGrid
    public Vector3Int aheadTile; //represents the grid location of the tile in front

    [HideInInspector]
    public GameObject holdObject; // Object being held (if any)
    public Transform holdPosition;

    public bool moving = false;
    public Direction turning = Direction.UP;

    // Use this for initialization
    void Start() {
        tilePos = levelGrid.WorldToCell(transform.position);
        aheadTile = tilePos + Utility.DirectionVector(facing);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A) && !moving) { // turn left
            if (CanTurn(facing, Direction.LEFT)) {
                facing = Utility.DirAdd(facing, Direction.LEFT);
                turning = Direction.LEFT;
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.D) && !moving) { // turn right
            if (CanTurn(facing, Direction.RIGHT)) {
                facing = Utility.DirAdd(facing, Direction.RIGHT);
                turning = Direction.RIGHT;
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.W) && !moving) { // forwards
            if (CanMove(facing)) {
                tilePos += Utility.DirectionVector(facing);
                moving = true;
            }
        } else if (Input.GetKeyDown(KeyCode.S) && !moving) { // backwards
            if (CanMove(Utility.DirAdd(facing, Direction.DOWN))) {
                tilePos -= Utility.DirectionVector(facing);
                moving = true;
            }
        }
        if (!moving) {
            moveDirection = facing;
        }
        if (Input.GetKeyDown(KeyCode.Z)) { // toggle hold
            if (holdObject == null) {
                GrabAhead(facing);
            } else {
                DropHeld();
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

    bool CanMove(Direction faceDir) {
        // replace with smarter system later

        aheadTile = tilePos + Utility.DirectionVector(faceDir);

        if (!tilemaps[0].HasTile(aheadTile)) {
            return false;
        }
        
        SoulController soul;
        if (Utility.GetSoulAtPos(tilemaps, aheadTile, out soul)) {
            return soul.Push(faceDir);
        }

        if (holdObject == null) {
            if (Utility.IsSolidAtPos(tilemaps, aheadTile)) {
                return false;
            }
        } else {
            if (faceDir == facing) { // moving forwards
                return holdObject.GetComponent<SoulController>().CanMove(faceDir);
            } else {
                if (Utility.IsSolidAtPos(tilemaps, aheadTile)) {
                    return false;
                }
            }
        }
        
        return true;
    }

    void GrabAhead(Direction faceDir) {
        if (holdObject == null) {
            Vector2 rayDirection = (Vector3)Utility.DirectionVector(faceDir);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, 1f);
            SoulController soul;
            if (Utility.GetSoulAtPos(tilemaps, tilePos + Utility.DirectionVector(faceDir), out soul)) {
                soul.Grab();
                holdObject = soul.gameObject;
            }
        }
        
    }

    void DropHeld() {
        // will need to extend in future for other grabbable things?
        holdObject.GetComponent<SoulController>().Drop();
        holdObject = null;
    }

    bool CanTurn(Direction faceDir, Direction turnDirection) {
        //Debug.DrawLine(levelGrid.CellToWorld(tilePos), levelGrid.CellToWorld(tilePos + DirectionVector(faceDir) + DirectionVector(DirAdd(faceDir, turnDirection))), Color.red, 10000);
        if (holdObject != null) {
            if (Utility.IsSolidAtPos(tilemaps, tilePos + Utility.DirectionVector(Utility.DirAdd(faceDir, turnDirection))) || // position + relative turning direction
                Utility.IsSolidAtPos(tilemaps, tilePos + Utility.DirectionVector(faceDir) + Utility.DirectionVector(Utility.DirAdd(faceDir, turnDirection)))) { // position + forwards + relative turning direction
                return false;
            }
        }
        return true;
    }


    
}
