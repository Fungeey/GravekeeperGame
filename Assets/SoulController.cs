using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SoulController : MonoBehaviour {

    public Tilemap[] tilemaps;

    bool CanMove(Directions faceDir) {

        Vector3Int aheadTile = tilemaps[0].WorldToCell(transform.position) + Utility.DirectionVector(faceDir);
        
        if (Utility.IsSolidAtPos(tilemaps, aheadTile)) {
            return false;
        }

        return true;
    }
}
