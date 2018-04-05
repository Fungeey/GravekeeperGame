using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction {
    UP, RIGHT, DOWN, LEFT
}

public static class Utility {
    public static Vector3Int DirectionVector(Direction direction) {
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

    public static Direction DirAdd(Direction dir1, int dir2) {
        return (Direction)(((int)dir1 + dir2) % 4);
    }

    public static Direction DirAdd(Direction dir1, Direction dir2) {
        return (Direction)(((int)dir1 + (int)dir2) % 4);
    }

    public static Quaternion DirectionQuaternion(Direction direction) {
        return Quaternion.Euler(0, 0, ((int)direction * -90) % 360);
    }

    public static bool IsSolidAtPos(Tilemap[] tilemaps, Vector3Int pos) {
        Grid levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        MarkPoint(levelGrid.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0), Color.blue, 2);
        Collider2D coll = Physics2D.OverlapArea(tilemaps[0].CellToWorld(pos) + new Vector3(.1f, .1f, 0), tilemaps[0].CellToWorld(pos + new Vector3Int(1, 1, 0)) - new Vector3(.1f, .1f, 0));
        if (tilemaps[1].HasTile(pos) || coll != null) {
            return true;
        }
        return false;
    }

    public static bool GetTileObjectAtPos(Vector3Int pos, out TileObject tileObject) {
        Grid levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        Collider2D coll = Physics2D.OverlapArea(levelGrid.CellToWorld(pos) + new Vector3(.1f, .1f, 0), levelGrid.CellToWorld(pos + new Vector3Int(1, 1, 0)) - new Vector3(.1f, .1f, 0));
        if (coll != null && coll.GetComponent<TileObject>() != null) {
            tileObject = coll.GetComponent<TileObject>();
            return true;
        }
        tileObject = null;
        return false;
    }

    public static void MarkPoint(Vector3 point, Color color, float seconds) {
        Debug.DrawLine(point + new Vector3(-0.1f, -0.1f), point + new Vector3(0.1f, 0.1f), color, seconds);
        Debug.DrawLine(point + new Vector3(-0.1f, 0.1f), point + new Vector3(0.1f, -0.1f), color, seconds);
    }
}
