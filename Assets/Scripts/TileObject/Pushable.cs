﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileObject))]
public class Pushable : MonoBehaviour {
    private TileObject tileObject;
    public bool canPushOffEdge = false;

    public void Start() {
        tileObject = GetComponent<TileObject>();
    }

    public bool CanPush(Direction pushDir) {
        Vector3Int aheadTile = tileObject.tilePos + Utility.DirectionVector(pushDir);
        if (Utility.IsSolidAtPos(tileObject.tilemaps, aheadTile)) {
            return false;
        }
        if (!canPushOffEdge && !tileObject.tilemaps[0].HasTile(aheadTile)) {
            return false;
        }
        return true;
    }

    public bool Push(Direction pushDir) {
        if (CanPush(pushDir)) {
            tileObject.moving = true;
            tileObject.tilePos += Utility.DirectionVector(pushDir);
            return true;
        } else {
            return false;
        }
    }
}
