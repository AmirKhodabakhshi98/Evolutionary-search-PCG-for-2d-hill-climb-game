using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CreateLevel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public CreateLevel(int[] heightArray, float distance, float tangentLength)
    {
        Generate(heightArray, distance, tangentLength);
    }
   
    public void Generate(int[]array, float distance, float tangentLength)
    {
        GameObject square = GameObject.Find("Square");
        SpriteShapeController spriteShapeController = square.GetComponent<SpriteShapeController>();
        Spline spline = spriteShapeController.spline;
        

        spline.SetPosition(0, new Vector3(0, -200));
        spline.SetPosition(1, new Vector3(0, 0));

        spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        spline.SetRightTangent(0, new Vector3(tangentLength, 0, 0));
        spline.SetLeftTangent(0, new Vector3(-tangentLength, 0, 0));
        spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        spline.SetRightTangent(1, new Vector3(tangentLength, 0, 0));
        spline.SetLeftTangent(1, new Vector3(-tangentLength, 0, 0));

        spline.SetPosition(2, new Vector3(distance , array[0]));
        spline.SetPosition(3, new Vector3(distance * 2, array[1]));

        spline.SetTangentMode(2, ShapeTangentMode.Continuous);
        spline.SetRightTangent(2, new Vector3(tangentLength, 0, 0));
        spline.SetLeftTangent(2, new Vector3(-tangentLength, 0, 0));

        spline.SetTangentMode(3, ShapeTangentMode.Continuous);
        spline.SetRightTangent( 3, new Vector3(tangentLength, 0, 0));
        spline.SetLeftTangent( 3, new Vector3(-tangentLength, 0, 0));


        for (int i = 2; i<array.Length; i++)
        {
            
            spline.InsertPointAt(i+2, new Vector3(distance*(i+1), array[i]));
            
            spline.SetTangentMode(i+2, ShapeTangentMode.Continuous);
            spline.SetRightTangent(i+2, new Vector3(tangentLength, 0,0));
            spline.SetLeftTangent(i+2, new Vector3(-tangentLength, 0, 0));
        }

        spline.SetTangentMode((array.Length + 1), ShapeTangentMode.Linear);
        spline.InsertPointAt(array.Length + 2, new Vector3(spline.GetPosition(array.Length+1).x, -200));
        spline.SetTangentMode(array.Length + 2, ShapeTangentMode.Linear);
     //   spline.SetRightTangent(array.Length + 2, new Vector3(0.1f, 0, 0));
     //     spline.SetLeftTangent(array.Length + 2, new Vector3(-0.1f, 0, 0));

        string str = "{";
            foreach(int e in array)
             {
                    str += e + ",";
             }
                str += "}";
                 Debug.Log("building array: " + str);


    }

    /*
    private void BuildLevel(int bestCandidatePos)
    {

        GameObject square = GameObject.Find("Square");
        SpriteShapeController spriteShapeController = square.GetComponent<SpriteShapeController>();
        Spline spline = spriteShapeController.spline;

        for (int i = 1; i < populationArray[bestCandidatePos].Length + 1; i++) //spline 0 is bottom left corner. 
        {
            spline.SetPosition(i, new Vector3(spline.GetPosition(i).x, spline.GetPosition(i).y + populationArray[bestCandidatePos][i - 1], spline.GetPosition(i).z));
            //      spline.SetRightTangent(i, )
            spline.SetTangentMode(i, ShapeTangentMode.Broken);
        }

        spriteShapeController.RefreshSpriteShape();
    */


}
