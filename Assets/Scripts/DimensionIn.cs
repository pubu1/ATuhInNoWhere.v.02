using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionIn : MonoBehaviour
{
    public bool HasEntranceTop { get; set; }
    private DimensionOut exitTop;


    public bool HasEntranceRight { get; set; }
    private DimensionOut exitRight;


    public bool HasEntranceBottom { get; set; }
    private DimensionOut exitBottom;


    public bool HasEntranceLeft { get; set; }
    public DimensionOut exitLeft{get;set;}

    // [SerializeField]
    // private Camera previousBaseCam;
    // [SerializeField]
    // private Camera targetBaseCam;

    void Start()
    {
        HasEntranceLeft = true;
    }

    public bool CheckNextStep(Player player)
    {
        if (player.TempNextKey == "Right" && HasEntranceLeft)
        {          
            /*if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire)
            {
                StopStepOnPool();
                return false;
            }
            else if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint")
            {
                if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire)
                {
                    StopStepOnPool();
                    return false;
                }
            }*/

            Vector3 entrancePosition = new Vector3(exitLeft.transform.position.x+1, exitLeft.transform.position.y, player.transform.position.z);

            player.CurrentPosition = player.transform.position;
            player.TargetPosition = entrancePosition;
            player.transform.position = entrancePosition;

            /*if (!playerScript.IsNotPickWire)
            {
                Vector2 ladder = new Vector2(entranceTeleporterPosition.x-1, entranceTeleporterPosition.y);
                RenderPipe(ladder, 0, 0);
            }*/
        }
        /*else if (player.TempNextKey == "Left" && dimension.GetTargetTeleporterList().ContainsKey("Right"))
        {
            Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Right"];
            if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire)
            {
                StopStepOnPool();
                return false;
            }
            else if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint")
            {
                if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire)
                {
                    StopStepOnPool();
                    return false;
                }
            }

            playerScript.TempCurrentPosition = player.transform.position;
            playerScript.TempTargetPosition = dimension.GetTargetTeleporterList()["Right"];
            entranceDimensionPosition = dimension.transform.position;
            player.transform.position = playerScript.TempTargetPosition;

            if (!playerScript.IsNotPickWire)
            {
                Vector2 ladder = new Vector2(entranceTeleporterPosition.x + 4, entranceTeleporterPosition.y);
                RenderPipe(ladder, 0, 0);
            }
        }
        else if (player.TempNextKey == "Up" && dimension.GetTargetTeleporterList().ContainsKey("Bottom"))
        {
            Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Bottom"];
            if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire)
            {
                StopStepOnPool();
                return false;
            }
            else if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint")
            {
                if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire)
                {
                    StopStepOnPool();
                    return false;
                }
            }

            playerScript.TempCurrentPosition = player.transform.position;
            playerScript.TempTargetPosition = dimension.GetTargetTeleporterList()["Bottom"];
            entranceDimensionPosition = dimension.transform.position;
            player.transform.position = playerScript.TempTargetPosition;

            if (!playerScript.IsNotPickWire)
            {
                Vector2 ladder = new Vector2(entranceTeleporterPosition.x, entranceTeleporterPosition.y - 4);
                RenderPipe(ladder, 0, 1);
            }
        }
        else if (player.TempNextKey == "Down" && dimension.GetTargetTeleporterList().ContainsKey("Top"))
        {
            Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Top"];
            if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire)
            {
                StopStepOnPool();
                return false;
            }
            else if (obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint")
            {
                if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire)
                {
                    StopStepOnPool();
                    return false;
                }
            }

            playerScript.TempCurrentPosition = player.transform.position;
            playerScript.TempTargetPosition = dimension.GetTargetTeleporterList()["Top"];
            entranceDimensionPosition = dimension.transform.position;
            player.transform.position = playerScript.TempTargetPosition;

            if (!playerScript.IsNotPickWire)
            {
                Vector2 ladder = new Vector2(entranceTeleporterPosition.x, entranceTeleporterPosition.y + 4);
                RenderPipe(ladder, 0, 1);
            }
        }*/
        return true;

        //dimension.SetTargetBaseCamera();

    }

    // public void SetTargetBaseCamera(){
    //     targetBaseCam.enabled = true;
    //     previousBaseCam.enabled = false;
    // }

    // public void SetPreviousBaseCamera(){
    //     previousBaseCam.enabled = true;
    //     targetBaseCam.enabled = false;
    // }

    void Update()
    {

    }
}
