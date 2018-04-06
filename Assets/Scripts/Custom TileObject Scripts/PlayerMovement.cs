using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : TileObject {
    [SerializeField]
    private float pivotSpeed = 15f;

    public Direction facing; // represents direction player is facing

    [HideInInspector]
    public TileObject holdObject; // Object being held (if any)
    public Transform holdPosition;
    
    public Direction turning = Direction.UP;

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
        if (Input.GetKeyDown(KeyCode.Z)) { // toggle hold
            if (holdObject == null) {
                GrabAhead(facing);
            } else {
                DropHeld();
            }
        }

        //Debug.DrawLine(transform.position, levelGrid.CellToWorld(aheadTile) + new Vector3(0.5f, 0.5f, 0));
    }

    protected override void FixedUpdate() {
        if (transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) { // Moving
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);

            if (holdObject != null) {
                holdObject.transform.position = Vector3.MoveTowards(holdObject.transform.position, holdPosition.position, moveSpeed);
            }

            IsOnExit(); // Check if you are on an exit
        } else if (Vector3Int.RoundToInt(transform.rotation.eulerAngles) != Vector3Int.RoundToInt(Utility.DirectionQuaternion(facing).eulerAngles)) { // Turning
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Utility.DirectionQuaternion(facing), pivotSpeed);

            if (holdObject != null) {
                holdObject.transform.position = Vector3.MoveTowards(holdObject.transform.position, holdPosition.position, moveSpeed * 3);
            }
        } else {
            if (holdObject != null) {
                holdPosition.transform.localPosition = Vector3Int.up;
                holdObject.tilePos = levelGrid.WorldToCell(holdPosition.transform.position);
            }
            if(moving == true) {
                gameController.saveLevelData(tilemaps[0], tilemaps[1], gameController.soulObjects, gameObject); // Hopefully should only save once
                moving = false;
            }
        }

    }

    bool CanMove(Direction moveDir) {
        Vector3Int aheadTile = tilePos + Utility.DirectionVector(moveDir);

        if (!tilemaps[0].HasTile(aheadTile)) {
            return false;
        }

        TileObject to;
        if (Utility.GetTileObjectAtPos(aheadTile, out to) && to.pushable) {
            return to.pushComp.Push(moveDir);
        }

        if (holdObject == null) {
            if (Utility.IsSolidAtPos(tilemaps, aheadTile)) {
                return false;
            }
        } else {
            if (moveDir == facing) { // moving forwards
                return holdObject.grabComp.CanMove(moveDir);
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
            TileObject to;
            if (Utility.GetTileObjectAtPos(tilePos + Utility.DirectionVector(faceDir), out to) && to.grabbable) {
                to.grabComp.Grab();
                holdObject = to;
            }
        }
        
    }

    void DropHeld() {
        holdObject.grabComp.Drop();
        holdObject = null;
    }

    bool CanTurn(Direction faceDir, Direction turnDirection) {
        //Debug.DrawLine(levelGrid.CellToWorld(tilePos), levelGrid.CellToWorld(tilePos + DirectionVector(faceDir) + DirectionVector(DirAdd(faceDir, turnDirection))), Color.red, 10000);
        if (holdObject != null) {
            if (!holdObject.grabComp.pivotable) return false;
            if (Utility.IsSolidAtPos(tilemaps, tilePos + Utility.DirectionVector(Utility.DirAdd(faceDir, turnDirection))) || // position + relative turning direction
                Utility.IsSolidAtPos(tilemaps, tilePos + Utility.DirectionVector(faceDir) + Utility.DirectionVector(Utility.DirAdd(faceDir, turnDirection)))) { // position + forwards + relative turning direction
                return false;
            }
        }
        return true;
    }

    public bool IsOnExit() { // Check if you are standing on an exit, used in GameController win checking
        if (tilemaps[0].GetTile(tilePos).name == "Exit") {
            // If you are standing on an exit
            if (gameController.win == false) {
                gameController.CheckWin();
                gameController.playerOnExit = true;
            }
            return true;
        } else {
            gameController.playerOnExit = false;
        }

        return false;
    }
    
}
