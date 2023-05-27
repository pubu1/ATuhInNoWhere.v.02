using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    private static float[] wireRotation = { 0f, 90.0f, 180.0f, 270.0f };   
    public float wireZAxis{get; set;}

    [SerializeField]
    private Sprite[] wireSprites;


    // Start is called before the first frame update
    public void Start()
    {
        wireZAxis = 7f;
        wireSprites = new Sprite[3];       
        wireSprites[0] = Resources.Load<Sprite>("straight");
        wireSprites[1] = Resources.Load<Sprite>("curve");
        wireSprites[2] = Resources.Load<Sprite>("cap");      
    }

    public void GenerateWire(Player player, string previousMove)
    {
        /*chuyen vi tri trong array thanh wire*/

        /*===Code here===*/
        /*obstaclePosition[player.CurrentPosition] = "Pipe";
        if(bridgeType.ContainsKey(player.CurrentPosition)) 
            obstaclePosition[player.CurrentPosition] = "Bridge";*/
        if (player.TempNextKey == "Right")
        {
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                RenderWire(player.CurrentPosition, 2, 0, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                RenderWire(player.TargetPosition, 2, 2, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (previousMove == "Right")
                {
                    RenderWire(player.CurrentPosition, 0, 0, player.HandleWireColor);
                }
                else if (previousMove == "Down")
                {
                    RenderWire(player.CurrentPosition, 1, 0, player.HandleWireColor);
                }
                else if (previousMove == "Up")
                {
                    RenderWire(player.CurrentPosition, 1, 3, player.HandleWireColor);
                }
            }
        }
        if (player.TempNextKey == "Left")
        {
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                RenderWire(player.CurrentPosition, 2, 2, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                RenderWire(player.TargetPosition, 2, 0, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (previousMove == "Left")
                {
                    RenderWire(player.CurrentPosition, 0, 0, player.HandleWireColor);
                }
                else if (previousMove == "Down")
                {
                    RenderWire(player.CurrentPosition, 1, 1, player.HandleWireColor);
                }
                else if (previousMove == "Up")
                {
                    RenderWire(player.CurrentPosition, 1, 2, player.HandleWireColor);
                }
            }
        }
        if (player.TempNextKey == "Up")
        {
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                RenderWire(player.CurrentPosition, 2, 1, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                RenderWire(player.TargetPosition, 2, 3, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (previousMove == "Up")
                {
                    RenderWire(player.CurrentPosition, 0, 1, player.HandleWireColor);
                }
                else if (previousMove == "Left")
                {
                    RenderWire(player.CurrentPosition, 1, 0, player.HandleWireColor);
                }
                else if (previousMove == "Right")
                {
                    RenderWire(player.CurrentPosition, 1, 1, player.HandleWireColor);
                }
            }
        }
        if (player.TempNextKey == "Down")
        {
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                RenderWire(player.CurrentPosition, 2, 3, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                RenderWire(player.TargetPosition, 2, 1, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (previousMove == "Down")
                {
                    RenderWire(player.CurrentPosition, 0, 1, player.HandleWireColor);
                }
                else if (previousMove == "Left")
                {
                    RenderWire(player.CurrentPosition, 1, 3, player.HandleWireColor);
                }
                else if (previousMove == "Right")
                {
                    RenderWire(player.CurrentPosition, 1, 2, player.HandleWireColor);
                }
            }
        }
    }

    public void RenderWire(Vector2 renderPosition, int pipeTypeIndex, int wireRotationIndex, string handleWireColor)
    {
        GameObject pipeClone = new GameObject();
        pipeClone.AddComponent<SpriteRenderer>();
        pipeClone.AddComponent<ChangeColor>();
        pipeClone.name = "Pipe" + handleWireColor;
        //pipes.Add(pipeClone);

        SpriteRenderer spriteRenderer = pipeClone.GetComponent<SpriteRenderer>();
        Transform transform = pipeClone.GetComponent<Transform>();
        ChangeColor changeColor = pipeClone.GetComponent<ChangeColor>();

        changeColor.Start();
        changeColor.ChangeSpriteColor(pipeClone, handleWireColor);

        spriteRenderer.sprite = wireSprites[pipeTypeIndex];
        transform.Rotate(0f, 0f, wireRotation[wireRotationIndex]);

        transform.position = new Vector3(renderPosition.x, renderPosition.y, wireZAxis);
    }
}
