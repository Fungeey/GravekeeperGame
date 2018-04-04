using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class TileObject : MonoBehaviour {
    [SerializeField]
    protected float moveSpeed = 0.11f;

    [HideInInspector]
    public Grid levelGrid;
    [HideInInspector]
    public Tilemap[] tilemaps;

    public Vector3Int tilePos;

    public bool grabbable = false;
    public bool pushable = false;

    [HideInInspector]
    public Grabbable grabComp;
    [HideInInspector]
    public Pushable pushComp;

    protected virtual void Awake() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tilemaps = levelGrid.GetComponentsInChildren<Tilemap>();
        tilePos = levelGrid.WorldToCell(transform.position);

        grabComp = grabbable ? gameObject.AddComponent<Grabbable>() : null;
        pushComp = pushable ? gameObject.AddComponent<Pushable>() : null;
    }

    protected virtual void FixedUpdate() {
        if (!(grabbable && grabComp.isHeld) && transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) {
            transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);
        }
    }

    public void SetTilePos() {
        tilePos = levelGrid.WorldToCell(transform.position);
    }
}

