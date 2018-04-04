using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TileObject : MonoBehaviour {
    [SerializeField]
    private float moveSpeed = 0.1f;

    public Grid levelGrid;
    public Tilemap[] tilemaps;
    public Vector3Int tilePos;

    public Grabbable grabComp;
    public Pushable pushComp;

    private void Awake() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tilemaps = levelGrid.GetComponentsInChildren<Tilemap>();
        tilePos = levelGrid.WorldToCell(transform.position);
    }

    private void FixedUpdate() {
        if (transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);
        }
    }

    public void SetTilePos() {
        tilePos = levelGrid.WorldToCell(transform.position);
    }
}

