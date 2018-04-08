using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : TileObject {
    [SerializeField]
    private float pivotSpeed = 15f;
    public Direction facing = Direction.UP;
    public Direction turning = Direction.UP;

    [Tooltip("Player sprite, then playerGrabbing sprite")]
    public Sprite[] playerSprites; // Holds default and grabbing sprites

    [HideInInspector]
    public TileObject holdObject; // Object being held (if any)
    public Transform holdPosition;

    void Start() {
        transform.rotation = Utility.DirectionQuaternion(facing);
    }

    void Update() {
        if (!moving) {
            if (Input.GetKeyDown(KeyCode.A)) { // turn left
                if (CanTurn(facing, Direction.LEFT)) {
                    gameController.SaveLevelData();
                    facing = Utility.DirAdd(facing, Direction.LEFT);
                    turning = Direction.LEFT;
                    moving = true;
                }
            } else if (Input.GetKeyDown(KeyCode.D)) { // turn right
                if (CanTurn(facing, Direction.RIGHT)) {
                    gameController.SaveLevelData();
                    facing = Utility.DirAdd(facing, Direction.RIGHT);
                    turning = Direction.RIGHT;
                    moving = true;
                }
            } else if (Input.GetKeyDown(KeyCode.W)) { // forwards
                if (CanMove(facing)) {
                    gameController.SaveLevelData();
                    TileObject to;
                    if (Utility.GetTileObjectAtPos(tilePos + Utility.DirectionVector(facing), out to) && to.pushable) {
                        to.pushComp.Push(facing);
                    }
                    tilePos += Utility.DirectionVector(facing);
                    moving = true;
                }
            } else if (Input.GetKeyDown(KeyCode.S)) { // backwards
                if (CanMove(Utility.DirAdd(facing, Direction.DOWN))) {
                    gameController.SaveLevelData();
                    TileObject to;
                    if (Utility.GetTileObjectAtPos(tilePos + Utility.DirectionVector(Utility.DirAdd(facing, Direction.DOWN)), out to) && to.pushable) {
                        to.pushComp.Push(Utility.DirAdd(facing, Direction.DOWN));
                    }
                    tilePos -= Utility.DirectionVector(facing);
                    moving = true;
                }
            }
        }
        if (!moving) {
            //if (Input.GetKey(KeyCode.Z)) { // toggle hold
            if (Input.GetMouseButton(0)) {
                if (holdObject == null)
                    GrabAhead(facing);
                GetComponent<SpriteRenderer>().sprite = playerSprites[1];
            } else {
                if (holdObject != null)
                    DropHeld();
                GetComponent<SpriteRenderer>().sprite = playerSprites[0];
            }
        }
    }

    protected override void FixedUpdate() {
        Camera.main.gameObject.transform.position = transform.position - new Vector3(0, 0, 1);

        if (transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) { // Moving
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);

            if (holdObject != null) {
                holdObject.transform.position = Vector3.MoveTowards(holdObject.transform.position, holdPosition.position, moveSpeed);
            }

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
            if (moving == true) {
                gameController.CheckWin();
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
            return to.pushComp.CanPush(moveDir);
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

    public void DropHeld() {
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

    public override TileObjectState GetState() {
        Dictionary<string, int> dict = new Dictionary<string, int> {
            { "facing", (int) facing }
        };
        return new TileObjectState(tilePos, dict);
    }

    public override void SetState(TileObjectState state) {
        base.SetState(state);
        this.facing = (Direction)state.additionalData["facing"];
        transform.rotation = Utility.DirectionQuaternion(facing);
        if (holdObject != null) DropHeld();
    }
}
