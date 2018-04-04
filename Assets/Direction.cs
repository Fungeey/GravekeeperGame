using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Directions {
    UP, RIGHT, DOWN, LEFT
}

public static class Utility {
    public static Vector3Int DirectionVector(Directions direction) {
        // Correctly map directions from enum
        switch (direction) {
            case Directions.UP:
                return Vector3Int.up;
            case Directions.RIGHT:
                return Vector3Int.right;
            case Directions.DOWN:
                return Vector3Int.down;
            case Directions.LEFT:
                return Vector3Int.left;
            default:
                return Vector3Int.zero;
        }
    }

    public static Directions DirAdd(Directions dir1, int dir2) {
        return (Directions)(((int)dir1 + dir2) % 4);
    }

    public static Directions DirAdd(Directions dir1, Directions dir2) {
        return (Directions)(((int)dir1 + (int)dir2) % 4);
    }

    public static Quaternion DirectionQuaternion(Directions direction) {
        return Quaternion.Euler(0, 0, ((int)direction * -90) % 360);
    }

    public static bool IsSolidAtPos(Tilemap[] tilemaps, Vector3Int pos) {
        // Debug.DrawLine(transform.position, levelGrid.CellToWorld(pos) + new Vector3(0.5f, 0.5f), Color.blue, 2);
        Collider2D coll = Physics2D.OverlapArea(tilemaps[0].CellToWorld(pos) + new Vector3(.1f, .1f, 0), tilemaps[0].CellToWorld(pos + new Vector3Int(1, 1, 0)) - new Vector3(.1f, .1f, 0));
        if (tilemaps[1].HasTile(pos) || coll != null) {
            return true;
        }
        return false;
    }
}
