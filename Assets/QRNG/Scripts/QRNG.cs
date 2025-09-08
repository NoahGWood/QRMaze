/* 
 * Spooky Manufacturing, LLC hereby disclaims
 * all copyright interest in the program "GameManager"
 * (which allows interfacing between the QRNGv1
 * board and Unity3D games) written by Noah G. Wood
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
using System.IO.Ports;
/* Note:
 * If you get an error stating the namespace Ports does
 * not exist, make sure you have set your API Compatibility
 * level to ".NET Standard"
 * (check edit/project settings/player/API Compatibility Level)
 */

namespace spookymfg
{
    public class QRNG : MonoBehaviour
    {
        [SerializeField]
        private bool showRandomness = false;
        [SerializeField]
        private int baudrate = 9600;
        [SerializeField]
        private string port = "COM3";
        [SerializeField]
        [Tooltip("Maximum number of random to use per float generation.")]
        private int maxNumToUse = 5;
        [SerializeField]
        [Tooltip("Maximum amount of random numbers to cache.")]
        private int maxNumbers = 5000;
        private SerialPort stream;
        private List<float> randomNums = new List<float>();
        void Awake()
        {
            stream = new SerialPort(port, baudrate);
            stream.Open();
        }

        void AddNumber(float n)
        {
            if (randomNums.Count > maxNumbers)
            {
                // Shift numbers down
                ReduceNums(1);
//                List<float> temp = randomNums.GetRange(1,maxNumbers-1);
//                randomNums = temp;
            }
            randomNums.Add(n);
        }
        // Update is called once per frame
        void FixedUpdate()
        {
            if (stream.BytesToRead >= 4)
            {
                string rxData = stream.ReadLine();
                float num = float.Parse(rxData);
                AddNumber(num);
            }
        }


        void OnGUI()
        {
            if (showRandomness)
            {
                int PrintLabelPosition = 200;
                string toPrint = "";
                foreach (float num in randomNums)
                {
                    toPrint += num.ToString();
                    toPrint += ", ";
                }

                GUI.Label(new Rect(10, PrintLabelPosition, 200, 20), toPrint);

            }
        }

        private void ReduceNums(int n)
        {
            //Reduce the randomNums list by n numbers.
            if (n < 0)
            {
                n = 0;
            }
            List<float> temp = randomNums.GetRange(n,randomNums.Count-n);
            randomNums = temp;
        }

        float SumArray(int n)
        {
            // If we don't have enough numbers in our array,
            // return a pseudo random number
            float x = 0f;
            if (randomNums.Count < n){
                n = randomNums.Count;
                Debug.Log("Not ours");
                return Random.value;
            }
            // Returns the average of n numbers in randomNums starting from index 0
            for (int i = 0; i < n; i++)
            {
                x += randomNums[i];
            }
            // We used some randomness, now we should
            // reduce the list
            ReduceNums(n);
            return Mathf.Abs(x/n);

        }

        public float RandomNumber()
        {
            // Grab a random length of numbers
            int i = Random.Range(0, maxNumToUse);
            float j = SumArray(i);
            return j;
        }
    }
}
