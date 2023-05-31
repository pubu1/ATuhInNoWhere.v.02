using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionOut : MonoBehaviour
{
    public DimensionIn BaseDimension { get; set; }
    public string OutDirection { get; set; }

    public Vector2 getBaseDimension()
    {
        return BaseDimension.transform.position;
    }

    public Vector2 getBaseDimensionEntrance()
    {
        return BaseDimension.transform.position;
    }
}
