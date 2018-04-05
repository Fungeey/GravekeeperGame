using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileObject))]
public class Grabbable : MonoBehaviour {
    private TileObject tileObject;

    public bool isHeld;

    public void Start() {
        tileObject = GetComponent<TileObject>();
    }

    public bool CanMove(Direction pushDir) {
        Vector3Int aheadTile = tileObject.tilePos + Utility.DirectionVector(pushDir);
        if (Utility.IsSolidAtPos(tileObject.tilemaps, aheadTile)) {
            return false;
        }
        return true;
    }

    public void Grab() {
        isHeld = true;
        tileObject.moving = true;
    }

    public void Drop() {
        tileObject.tilePos = tileObject.levelGrid.WorldToCell(transform.position);
        isHeld = false;
        tileObject.moving = false;
    }
}
