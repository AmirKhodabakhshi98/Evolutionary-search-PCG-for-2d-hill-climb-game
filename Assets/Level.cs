using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level 
{

    public int fitnessValue;
    public int[] heightArray;
    
    public int highestDrop;
    public int dropSpread;
    public int nbrOfDrops;
    public List<int> dropsHeight;

    public Level(int size, int maxHeight, int maxDifferenceBetweenPoints)
    {
        heightArray = new int[size];
        nbrOfDrops = 0;
        dropSpread = 0;
        highestDrop = 0;
        fitnessValue = 0;

        RandomizeStartingValues(maxHeight, maxDifferenceBetweenPoints);

    }

    private void RandomizeStartingValues(int maxHeight, int maxDifferenceBetweenPoints)
    {
        heightArray[0] = 0; //NOTE - potentiellt ändra så att den kan starta var som helst? borde det inte va nån skillnad?

        for(int i=1; i<heightArray.Length; i++)
        {

            //loop that randomises value of a point.
            //it performs a check so that difference between a point and its previous point isnt more than max allowed.
            //Prevents 'exaggerated' up/down slopes
            do
            {
                heightArray[i] = Random.Range(0, maxHeight);
            } while (Mathf.Abs(heightArray[i] - heightArray[i - 1]) > maxDifferenceBetweenPoints); 

        }

        //    string str = "{";
       //    foreach(int e in heightArray)
      //     {
     //            str += e + ",";
      //     }
     //        str += "}";
    //         Debug.Log("initial value" + str);

    }


}
