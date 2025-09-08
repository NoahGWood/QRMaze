/*
 * This source file contains mixed code from
 * two generally compatible licenses: MIT &
 * GPLv3.
 * The bulk of this code is licensed under MIT
 * code which is licensed under GPLv3 is bound
 * inside two comments denoting where the start
 * of the GPLv3 code is and the end of the GPLv3
 * code is.
 *
 * Spooky Manufacturing, LLC hereby disclaims
 * all copyright interest in the source code that
 * resides within between the comment sections that
 * state "This portion..." and "End modified portion."
 * written by Noah G. Wood
 * Noah G. Wood, 15, May, 2022
 * Released under GPLv3 License
 *
 * This program is free software; you can 
 * redistribute it and/or modify it under the terms
 * of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of
 * the License,  or (at your option) any later version.
 *
 * This program is distributed in the hope th that it
 * will be useful, but WITHOUT ANY WARRANTY; without
 * even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General
 * Public License with this program. If not, see
 * <https://www.gnu.org/licenses/>
 */


/*
 * The following code was written by Joseph Hocking 2017
 * and it has been released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */
using System.Collections.Generic;
using UnityEngine;
using spookymfg;

[RequireComponent(typeof(GameManager))]
public class MazeGenerator : MonoBehaviour
{
     /*
      * This portion of the code was modifed by
      * Noah Wood 2022 and it has been released
      * under GPLv3.
      */ 
    public bool showDebug = true;
    [SerializeField]
    private Material floorMat;
    [SerializeField]
    private Material wallMat;
    [SerializeField]

    [Header("Maze Mesh Settings")]
    public float wallWidth = 3.75f;
    public float wallHeight = 3.5f;
    public int[,] mazeData;

    [Range(0, 1)] public float placementThreshold = 0.1f;

    public int startCol;
    public int startRow;
    private GameManager gameManager;
    private QRNG qrng;
    /* 
     * End Modified Portion.
     */
    void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        qrng = GetComponent<QRNG>();
        mazeData = new int[,] {
                        {1,1,1},
                        {1,0,1},
                        {1,1,1}
                        };
    }

    public void GenerateMaze(int rows, int cols)
    {
        DisposeOldMaze();
        FindStartPosition();
        FindGoalPosition();
        mazeData = new int[rows, cols];
        int rMax = mazeData.GetUpperBound(0);
        int cMax = mazeData.GetUpperBound(1);
        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (i == 0 || j == 0 || i == rMax || j == cMax)
                {
                    mazeData[i, j] = 1;
                }
                else if (i % 2 == 0 && j % 2 == 0)
                {
                    /*
                     * This portion of the code was modifed by
                     * Noah Wood 2022 and it has been released
                     * under GPLv3.
                     */              
                    if (qrng.RandomNumber() > placementThreshold)
                    {          
                        mazeData[i, j] = 1;
                        int a = qrng.RandomNumber() < .5 ? 0 : (qrng.RandomNumber() < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (qrng.RandomNumber() < .5 ? -1 : 1);
                        mazeData[i + a, j + b] = 1;
                    }
                    /* 
                     * End Modified Portion.
                     */
                }
            }
        }

        DisplayMaze();
    }

/*
 * The following code was written by Joseph Hocking 2017
 * and it has been released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */
    Mesh FromData(int[,] data)
    {
        Mesh maze = new Mesh();
        //3
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();
        maze.subMeshCount = 2;
        List<int> floorTriangles = new List<int>();
        List<int> wallTriangles = new List<int>();
        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        float halfH = wallHeight * .5f;
        //4
        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (data[i, j] != 1)
                {
                    // floor
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * wallWidth, 0, i * wallWidth),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(wallWidth, wallWidth, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);
                    // ceiling
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * wallWidth, wallHeight, i * wallWidth),
                        Quaternion.LookRotation(Vector3.down),
                        new Vector3(wallWidth, wallWidth, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);
                    // walls on sides next to blocked grid cells
                    if (i - 1 < 0 || data[i - 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * wallWidth, halfH, (i - .5f) * wallWidth),
                            Quaternion.LookRotation(Vector3.forward),
                            new Vector3(wallWidth, wallHeight, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }
                    if (j + 1 > cMax || data[i, j + 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j + .5f) * wallWidth, halfH, i * wallWidth),
                            Quaternion.LookRotation(Vector3.left),
                            new Vector3(wallWidth, wallHeight, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }
                    if (j - 1 < 0 || data[i, j - 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j - .5f) * wallWidth, halfH, i * wallWidth),
                            Quaternion.LookRotation(Vector3.right),
                            new Vector3(wallWidth, wallHeight, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }
                    if (i + 1 > rMax || data[i + 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * wallWidth, halfH, (i + .5f) * wallWidth),
                            Quaternion.LookRotation(Vector3.back),
                            new Vector3(wallWidth, wallHeight, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }
                }
            }
        }
        maze.vertices = newVertices.ToArray();
        maze.uv = newUVs.ToArray();
        maze.SetTriangles(floorTriangles.ToArray(), 0);
        maze.SetTriangles(wallTriangles.ToArray(), 1);
        //5
        maze.RecalculateNormals();
        return maze;
    }
    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
    ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        // corners before transforming
        Vector3 vert1 = new Vector3(-.5f, -.5f, 0);
        Vector3 vert2 = new Vector3(-.5f, .5f, 0);
        Vector3 vert3 = new Vector3(.5f, .5f, 0);
        Vector3 vert4 = new Vector3(.5f, -.5f, 0);

        newVertices.Add(matrix.MultiplyPoint3x4(vert1));
        newVertices.Add(matrix.MultiplyPoint3x4(vert2));
        newVertices.Add(matrix.MultiplyPoint3x4(vert3));
        newVertices.Add(matrix.MultiplyPoint3x4(vert4));

        newUVs.Add(new Vector2(1, 0));
        newUVs.Add(new Vector2(1, 1));
        newUVs.Add(new Vector2(0, 1));
        newUVs.Add(new Vector2(0, 0));

        newTriangles.Add(index + 2);
        newTriangles.Add(index + 1);
        newTriangles.Add(index);

        newTriangles.Add(index + 3);
        newTriangles.Add(index + 2);
        newTriangles.Add(index);
    }


    void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "qrng Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = FromData(mazeData);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] { floorMat, wallMat };
    }

    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }

    public void FindStartPosition()
    {
        int[,] maze = mazeData;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i=0; i<= rMax; i++){
            for (int j=0; j<= cMax; j++){
                if (maze[i,j] == 0){
                    /*
                     * This portion of the code was modifed by
                     * Noah Wood 2022 and it has been released
                     * under GPLv3.
                     */
                    GameObject go = Instantiate(gameManager.startObject, 
                    new Vector3(j * wallWidth, 0.5f, i * wallWidth),
                    Quaternion.identity);
                    TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
                    tc.callback = gameManager.OnStartTrigger;
                    startCol = j;
                    startRow = i;
                    /*
                     * End modified portion.
                     */
                    return;
                }
            }
        }
    }

    public void FindGoalPosition()
    {
        int[,] maze = mazeData;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        for (int i=rMax; i >= 0; i--){
            for (int j= cMax; j >=0; j--){
                if (maze[i,j] == 0)
                {
                    /*
                     * This portion of the code was modifed by
                     * Noah Wood 2022 and it has been released
                     * under GPLv3.
                     */
                    GameObject go = Instantiate(gameManager.goalObject, 
                    new Vector3(j * wallWidth, 0.5f, i * wallWidth),
                    Quaternion.identity);
                    TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
                    tc.callback = gameManager.OnGoalTrigger;
                    /*
                     * End modified portion.
                     */
                    return;
                }
            }
        }
    }
}

