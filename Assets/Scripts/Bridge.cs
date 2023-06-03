using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private string bridgeType;

    public bool HasWireOnBridge { get; set; }
    public bool HasWireUnderBridge { get; set; }

    void Start()
    {
        HasWireOnBridge = false;
        HasWireUnderBridge = false;
    }

    public string GetBridgeType()
    {
        return bridgeType.Trim();
    }

    public bool IsVertical()
    {
        return bridgeType == "Vertical";
    }
    public bool IsHorizontal()
    {
        return bridgeType == "Horizontal";
    }

    public bool CheckNextStep(Bridge bridge, Player player)
    {
        bool isOnBridge = false;

        if ((bridge.IsHorizontal() && (player.TempNextKey == "Left" || player.TempNextKey == "Right"))
        || (bridge.IsVertical() && (player.TempNextKey == "Up" || player.TempNextKey == "Down")))
            isOnBridge = true;

        if (isOnBridge)
        {
            if(HasWireOnBridge && !player.IsNotPickWire) return false;
            player.DefaultZAxis = 2f;
        }
        else
        {    
            if(HasWireUnderBridge && !player.IsNotPickWire) return false;   
            player.DefaultZAxis = 5f;
        }

        return true;
    }

    public bool CheckCurrentStep(Bridge bridge, Player player, string previousMove)
    {
        bool isOnBridge = false;

        if ((bridge.IsHorizontal() && (previousMove == "Left" || previousMove == "Right"))
        || (bridge.IsVertical() && (previousMove == "Up" || previousMove == "Down")))
            isOnBridge = true;

        if (isOnBridge)
        {
            if ((bridge.IsHorizontal() && (player.TempNextKey == "Up" || player.TempNextKey == "Down"))
            || (bridge.IsVertical() && (player.TempNextKey == "Left" || player.TempNextKey == "Right")))
                return false;

            if (!player.IsNotPickWire) bridge.HasWireOnBridge = true;
        }
        else
        {
            if ((bridge.IsHorizontal() && (player.TempNextKey == "Left" || player.TempNextKey == "Right"))
            || (bridge.IsVertical() && (player.TempNextKey == "Up" || player.TempNextKey == "Down")))
                return false;

            if (!player.IsNotPickWire) bridge.HasWireUnderBridge = true;
        }
        return true;
    }

    public float GetZAxisWire(string previousMove)
    {
        float wireZAxis = 0f;

        if (this.IsVertical()
        && (previousMove == "Left" || previousMove == "Right")
        && this.HasWireUnderBridge)
        {
            wireZAxis = 6f;
        }
        else if (this.IsHorizontal()
        && (previousMove == "Up" || previousMove == "Down"
        && this.HasWireUnderBridge))
        {
            wireZAxis = 6f;
        }
        else if (this.IsVertical()
        && (previousMove == "Up" || previousMove == "Down"
        && this.HasWireOnBridge))
        {
            wireZAxis = 3f;
        }
        else if (this.IsHorizontal()
        && (previousMove == "Left" || previousMove == "Right"
        && this.HasWireOnBridge))
        {
            wireZAxis = 3f;
        }
        return wireZAxis;
    }
}
