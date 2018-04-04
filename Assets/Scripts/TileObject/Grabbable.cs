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

    public void Grab() {
        isHeld = true;
    }

    public void Drop() {
        tileObject.SetTilePos();
        isHeld = false;
    }
}
