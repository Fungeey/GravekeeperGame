using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SoulController : MonoBehaviour {
    [SerializeField]
    private float moveSpeed = 0.1f;

    public Tilemap[] tilemaps;
    public Vector3Int targetPos; // target position on tile grid 
    
    private bool held = false;

    private void Start() {
        targetPos = tilemaps[0].WorldToCell(transform.position);
    }

    public bool CanMove(Direction pushDir) {
        Vector3Int aheadTile = tilemaps[0].WorldToCell(transform.position) + Utility.DirectionVector(pushDir);
        if (Utility.IsSolidAtPos(tilemaps, aheadTile)) {
            return false;
        }
        return true;
    }

    public bool Push(Direction pushDir) {
        if (CanMove(pushDir)) {
            targetPos = tilemaps[0].WorldToCell(transform.position) + Utility.DirectionVector(pushDir);
            return true;
        } else {
            return false;
        }
    }

    private void FixedUpdate() {
        if (!held && transform.position != tilemaps[0].CellToWorld(targetPos) + new Vector3(0.5f, 0.5f, 0)) {
            transform.position = Vector3.MoveTowards(transform.position, tilemaps[0].CellToWorld(targetPos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);
        }
    }

    public void Grab() {
        held = true;
    }

    public void Drop() {
        targetPos = tilemaps[0].WorldToCell(transform.position);
        held = false;
    }
}
