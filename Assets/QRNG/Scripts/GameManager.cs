/* 
* Spooky Manufacturing, LLC hereby disclaims
* all copyright interest in the program "GameManager"
* (which allows controlling a maze game) written by
* Noah G. Wood
* Released by:
* Noah G. Wood, Founder at Spooky Manufacturing, LLC
* on: 15, May, 2022
* under GPLv3 License
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(MazeGenerator))]
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public GameObject player;
    private GameObject cPlayer;
    public GameObject startObject;
    public GameObject goalObject;
    public bool goalReached = false;
    public int score = 0;
    public TMP_Text scoreLabel;
    
    [Header("Maze Settings")]
    public int mazeWidth = 100;
    public int mazeHeight = 100;

    private MazeGenerator generator;

    void Start()
    {
        generator = gameObject.GetComponent<MazeGenerator>();
        StartNewMaze();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnStartTrigger(GameObject trigger, GameObject other)
    {
//        Debug.Log("Start trigger!");
        Destroy(trigger);
    }


/*
 * The following code was written by Joseph Hocking 2017
 * and it has been released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */
    void StartNewMaze()
    {
        generator.GenerateMaze(mazeWidth,mazeHeight);
        float x = generator.startCol * generator.wallWidth;
        float y = 1;
        float z = generator.startRow * generator.wallWidth;
        Destroy(cPlayer);
        cPlayer = Instantiate(player, new Vector3(x,y,z), Quaternion.identity);
    }

    public void OnGoalTrigger(GameObject trigger, GameObject other) {
        goalReached = true;
        score++;
        scoreLabel.text  = score.ToString();
        Destroy(trigger);
        StartNewMaze();
    }

}
