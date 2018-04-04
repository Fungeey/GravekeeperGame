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

    public bool moving = false;
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

        Vector3Int aheadTile = tilePos + Utility.DirectionVector(faceDir);

        if (!tilemaps[0].HasTile(aheadTile)) {
            return false;
        }
        
        /*SoulController soul;
        if (Utility.GetSoulAtPos(tilemaps, aheadTile, out soul)) {
            return soul.Push(faceDir);
        }*/

        TileObject to;
        if (Utility.GetTileObjectAtPos(aheadTile, out to) && to.pushable) {
            return to.pushComp.Push(faceDir);
        }

        if (holdObject == null) {
            if (Utility.IsSolidAtPos(tilemaps, aheadTile)) {
                return false;
            }
        } else {
            if (faceDir == facing) { // moving forwards
                return holdObject.grabComp.CanMove(faceDir);
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
            /*SoulController soul;
            if (Utility.GetSoulAtPos(tilemaps, tilePos + Utility.DirectionVector(faceDir), out soul)) {
                soul.Grab();
                holdObject = soul.gameObject;
            }*/
            TileObject to;
            if (Utility.GetTileObjectAtPos(tilePos + Utility.DirectionVector(faceDir), out to) && to.grabbable) {
                to.grabComp.Grab();
                holdObject = to;
            }
        }
        
    }

    void DropHeld() {
        // will need to extend in future for other grabbable things?
        holdObject.grabComp.Drop();
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
