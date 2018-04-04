using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileObject : MonoBehaviour {
    [SerializeField]
    private float moveSpeed = 0.1f;

    public Grid levelGrid;
    public Tilemap[] tilemaps;
    public Vector3Int tilePos;

    public bool grabbable = false;
    public bool pushable = false;

    public Grabbable grabComp;
    public Pushable pushComp;

    private void Awake() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tilemaps = levelGrid.GetComponentsInChildren<Tilemap>();
        tilePos = levelGrid.WorldToCell(transform.position);

        grabComp = grabbable ? gameObject.AddComponent<Grabbable>() : null;
        pushComp = grabbable ? gameObject.AddComponent<Pushable>() : null;
    }

    private void FixedUpdate() {
        if (!(grabbable && grabComp.isHeld) && transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);
        }
    }

    public void SetTilePos() {
        tilePos = levelGrid.WorldToCell(transform.position);
    }
}

