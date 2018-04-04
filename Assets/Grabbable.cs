using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileObject))]
public class Grabbable : MonoBehaviour {
    public bool held;
    public TileObject tileObject;

    public void Start() {
        tileObject = GetComponent<TileObject>();
    }

    public void Grab() {
        held = true;
    }

    public void Drop() {
        tileObject.SetTilePos();
        held = false;
    }
}
