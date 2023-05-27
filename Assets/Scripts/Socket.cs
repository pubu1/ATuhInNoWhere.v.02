using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{
    public string Color { get; set; }

    public bool IsConnect { get; set; }

    void Start()
    {
        IsConnect = false;
    }

    public string GetColorType()
    {
        return Color;
    }

    public bool CheckNextStep(bool isNotPickWire, string handleWireColor)
    {
        if ((!isNotPickWire && handleWireColor != this.Color) || (!isNotPickWire && this.IsConnect))
            return false;
        else
            return true;
    }
}
