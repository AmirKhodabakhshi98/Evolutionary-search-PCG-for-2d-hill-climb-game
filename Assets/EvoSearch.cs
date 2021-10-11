using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EvoSearch : MonoBehaviour
{

    public int nbrOfPopulation;
    public int nbrOfPoints;
    public int maxRuns;
    public int highestDrop;
    public int MaxHeight;

    int[] height = new int[10]; //kan gö så d ändras via inspector sen

    int[] fitnessArray;
    int[][] populationArray;

    int runs = 0;


    public void Start()
    {
        populationArray = new int[nbrOfPopulation][];
        fitnessArray = new int[nbrOfPopulation];


        Instantiate();
    }

    private void Instantiate()
    {
        for (int n = 0; n < fitnessArray.Length; n++)
        {
            fitnessArray[n] = 0; //initialize fitnessarray to 0 fitness for each candidate
            populationArray[n] = new int[nbrOfPoints]; //initialize each candidate map with x number of points
        }


        for (int i = 0; i < populationArray.Length; i++)
        {
            populationArray[i][0] = 0;
            for (int j = 1; j < 10; j++) //startpunkt 0 börjar alltid på samma koordinater. 
            {
                populationArray[i][j] = 0; //instantiera alla kandidat levels till 0 överlag
            }
        }

        FitnessCheck();
    }



    private void FitnessCheck()
    {

        int max = 0;
        int difference = 0;


        //checks biggest down-slope for each map
        for (int i = 0; i < populationArray.Length; i++)
        {
            max = 0;
            for (int j = 0; j < nbrOfPoints-1; j++) //1 less than max so its not out of bounds. //HARDCODED CHANGE LATER
            {
                difference = populationArray[i][j] - populationArray[i][j + 1]; //check diff between 2 points, looking for sharp cliff down
                if (difference > max)
                {
                    max = difference; // set new best cliff.
                    fitnessArray[i] = max; //set fitness value for that candidate
                }
            }
        }

        int bestCandidatePos = 0; //position of the best candidate in array - starts at 0 so its not out of bounds later, better solution?
        bool bestFound = false;
        //kan optimeras sen med return statement i ovanstående loop.
        //loop to find best candidate
        
        max = 0;
        for (int x = 0; x < fitnessArray.Length; x++)
        {
            
            if (fitnessArray[x] > max)
            {
                max = fitnessArray[x];
                bestCandidatePos = x; //sets which candidate has best score - remark: wont rewrite if 2nd candidate has same score
            }
        }

        //checks if best candidate limit found
        if (fitnessArray[bestCandidatePos] >= highestDrop) //MAKE PUBLIC VARIABLE LATER
        {
            bestFound = true;

            //debug: write out population
            for (int z = 0; z < populationArray[bestCandidatePos].Length; z++)
            {
                print(populationArray[bestCandidatePos][z]);
            }
            print("runs: " + runs);
            BuildLevel(bestCandidatePos);

        }

        //mutation step
        else if (runs < maxRuns) {

            string str = "";
            for (int z = 0; z < populationArray[bestCandidatePos].Length; z++)
            {
                str += populationArray[bestCandidatePos][z] + ", ";
            }
            print("run: "+ runs + ", fitness:" + fitnessArray[bestCandidatePos] + ", best candidate: " + str );


            for (int p = 0; p < populationArray.Length; p++)
            {
                if (p != bestCandidatePos)
                { //so we don't rewrite best with mutation

                    int rnd = Random.Range(1, 10);
                    populationArray[p] = populationArray[bestCandidatePos]; //replaces all with best cand
                    populationArray[p][rnd] = Random.Range(0, 10); //mutates a random spot - mutates difference so not too big an upward slope

                    //step to make sure upward slope isnt too large. OPTIMIZE??
                    while(populationArray[p][rnd] - populationArray[p][rnd-1] > 5)
                    {
                        populationArray[p][rnd] = Random.Range(0, 10); 

                    }

                }
            }
            runs++;
            FitnessCheck();
            
        }
        else //i.e runs 100 while fitness goal not reached
        {

            print(runs);
            //prints best pop
            string str = "";
            for (int z = 0; z < populationArray[bestCandidatePos].Length; z++)
            {
                str+=populationArray[bestCandidatePos][z] + ", ";
            }
            print("runs: " + runs + ", fitness:" + fitnessArray[bestCandidatePos] + ", pos: " + str  );
            BuildLevel(bestCandidatePos);

        }


    }



    //method to build the level based on the search method
    private void BuildLevel(int bestCandidatePos)
    {

        GameObject square = GameObject.Find("Square");
        SpriteShapeController spriteShapeController = square.GetComponent<SpriteShapeController>();
        Spline spline = spriteShapeController.spline;

        for (int i = 1; i < populationArray[bestCandidatePos].Length + 1; i++) //spline 0 is bottom left corner. 
        {
            spline.SetPosition(i, new Vector3(spline.GetPosition(i).x, spline.GetPosition(i).y + populationArray[bestCandidatePos][i-1], spline.GetPosition(i).z)); 
      //      spline.SetRightTangent(i, )
            spline.SetTangentMode(i, ShapeTangentMode.Broken);
        }

       spriteShapeController.RefreshSpriteShape();

    }



    private void Selection()
    {

    }




}
