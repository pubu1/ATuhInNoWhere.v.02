using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID { get; set; }
    public string HandleWireColor{get; set;}
    public bool IsNotPickWire{get; set;}
    public Vector2 CurrentPosition{get; set;}
    public Vector2 TargetPosition{get; set;}
    public Vector2 TempCurrentPosition{get; set;}
    public Vector2 TempTargetPosition{get; set;}

    public string TempNextKey{get; set;}

    public float DefaultZAxis{get; set;}

    public bool IsAtSocket{get; set;}
    // Start is called before the first frame update
    void Start()
    {
        IsNotPickWire = true;
        IsAtSocket = false;
        IsNotPickWire = true;
        CurrentPosition = this.transform.position;
        TargetPosition = this.transform.position;
        TempCurrentPosition = this.transform.position;
        TempTargetPosition = this.transform.position;
        DefaultZAxis = 6f;
        TempNextKey = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
