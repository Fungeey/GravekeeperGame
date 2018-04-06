using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class TileObject : MonoBehaviour {
    [SerializeField]
    protected float moveSpeed = 0.1f;

    [HideInInspector]
    public Grid levelGrid;
    [HideInInspector]
    public Tilemap[] tilemaps;
    [HideInInspector]
    public GameController gameController;

    public Vector3Int tilePos;
    public bool moving;

    public bool grabbable = false;
    public bool pushable = false;
    public bool pivotable = false;

    [HideInInspector]
    public Grabbable grabComp;
    [HideInInspector]
    public Pushable pushComp;

    protected virtual void Awake() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tilemaps = levelGrid.GetComponentsInChildren<Tilemap>();
        tilePos = levelGrid.WorldToCell(transform.position);
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();


        grabComp = grabbable ? gameObject.AddComponent<Grabbable>() : null;
        pushComp = pushable ? gameObject.AddComponent<Pushable>() : null;
        if (grabbable) grabComp.pivotable = pivotable;
    }

    protected virtual void FixedUpdate() {
        if (grabbable && grabComp.isHeld) {

        } else {
            if (transform.position != levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0)) {
                transform.position = Vector3.MoveTowards(transform.position, levelGrid.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), moveSpeed);
            } else {
                moving = false;
            }
        }
    }
}

