using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class InputManager
{
    public string fileName = "input.txt";
    public string[,] inputMap;
    private List<string[,]> listMap;

    public List<string[,]> LoadGridFromFile()
    {
        listMap = new List<string[,]>();
        string folderPath = Path.Combine(Application.dataPath, "..", "CustomMap");
        string filePath = Path.Combine(folderPath, fileName);
        string[] lines = File.ReadAllLines(filePath);

        //count number of Map
        int currentLineIndex = 0;
        int mapCnt = int.Parse(lines[currentLineIndex++]);

/*        foreach (string line in lines)
        {
            Debug.Log(line);
        }*/
        //read all Maps
        while(mapCnt -- > 0)
        {
            // Read the dimensions of the current map
            string[] dimensions = lines[currentLineIndex].Split(' ');
            //Debug.Log(dimensions.Length);
            int rows = int.Parse(dimensions[0]);
            int columns = int.Parse(dimensions[1]);

            //Debug.Log(rows + " - " + columns);

            string[,] grid = new string[rows, columns];

            // Populate the grid with values from the remaining lines
            for (int i = 0; i < rows; i++)
            {
                string[] values = lines[currentLineIndex + 1 + i].Split(' ');
                for (int j = 0; j < columns; j++)
                {
                    grid[i, j] = values[j];
                }
            }

            listMap.Add(grid);

            // Move to the next map's data
            currentLineIndex += 1 + rows;
        }
        //Check other component
/*        while (currentLineIndex < lines.Length)
        {
            ++currentLineIndex;
        }*/

        return listMap;
        //return inputMap;
    }
}
