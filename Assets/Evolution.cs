using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public int populationSize; // size of population - EVEN NUMBER
    public int fitnessTarget; // fitness value to strive for
    public int generationsLimit; // max nbr of generations allowed
    public int individSize; // how many points each member of pop should have
    public int mutationIntervalSize; //size of the random interval of points that is to be mutated
    public int maxHeight;  //highest allowed point that terrain can be
    public int maxDifferenceBetweenPoints; //max allowed delta between height of 2 points

    public int weightDrop;
    public int weightNbrOfFlatTerrain; // multiplier to give weighted value

    public int nbrOfDropsToLookFor; // how mayn drops max the fitness function wants to look for

    public float distanceBetweenPoints; // for map generation
    public float tangentLength; // for map generation

    private string path = @"E:\backup ssd\downloads\MAU HT 20\PCG\Assignment 1\output.txt";
    private string path2 = @"E:\backup ssd\downloads\MAU HT 20\PCG\Assignment 1\outputFitness.txt";
    private string path3 = @"E:\backup ssd\downloads\MAU HT 20\PCG\Assignment 1\outputExpressiveRange.txt";

    //private List<Level> population;




    public void Start()
    {
              Search();

    }

    private string str = "";
    private string str2 = "";
    private void Search() 
    {
        List<Level> population = new List<Level>();
        int nbrOfGenerations = 0;

        population = InitializePopulation(population); //initialize a population with random values


        
        while (nbrOfGenerations < generationsLimit)
        {
            
            population = Fitness(population);   // assign them all a fitness value
            var (pop, mostFit, leastFit, avgFitness) = SortByFitness(population);  //sort based on fitness value
            population = pop;
            str2 += "Generation: " + nbrOfGenerations + ", Most fit: " + mostFit + ", Least Fit: " + leastFit + ", Average: " + avgFitness +  "\n";

            //if target fitness has been reached search is stopped
            if (mostFit >= fitnessTarget)
            {
                
                break; }

            population = NaturalSelection(population);     //replace bottom half with copies of the top half
            population = MutatePopulation(population);     //mutate the copied members of population

            nbrOfGenerations++;
        }

        string str3 = "Nbr of flat: " +  population[0].nbrOfFlat + ", Totaldrop: " + population[0].totalDrop + ", max drop: " + population[0].maxDrop;

        File.WriteAllText(path, str);
        File.WriteAllText(path2, str2);
        File.WriteAllText(path3, str3);
        GenerateLevel(population[0].getHeightArray()); //sends in top ranked candidate for level generation after search is finished
      //  TotalDropFinder(population[0].heightArray);

    }

    public List<Level> InitializePopulation(List<Level> pop)
    {

        for(int i=0; i<populationSize; i++)
        {
            pop.Add(new Level(individSize, maxHeight, maxDifferenceBetweenPoints));
        }

        return pop;
    }


    //method to assign fitness score to each member of population
    public List<Level> Fitness(List<Level> pop)
    {

        int dropSum = 0 ;
        int nbrOfFlatTerrain = 0;
        int fit = 0;
        int tooSteep = 0;

        //loop through population and assign total score based on the various fitness tests.
        for(int i=0; i<pop.Count; i++)
        {
            dropSum = 0;
            nbrOfFlatTerrain = 0;
            fit = 0;
            tooSteep = 0;

            nbrOfFlatTerrain = NbrOfFlatTerrainFinder(pop[i].getHeightArray());

            var (sum, max) = TotalDropFinder(pop[i].getHeightArray());  //sort based on fitness value

            dropSum = sum;

            pop[i].maxDrop = max;
            pop[i].totalDrop = dropSum; //for exp range
            pop[i].nbrOfFlat= nbrOfFlatTerrain; //for exp range

            fit = dropSum * weightDrop - nbrOfFlatTerrain * weightNbrOfFlatTerrain;
            pop[i].setFitnessValue(fit);
        }
        
        return pop;
    }

  //  (List<Level> pop, int mostFit, int leastFit, int avgFitness)
    //takes a height array and finds all the drops in the level. A drop is a downward slope up until flat ground or the ground rises again.
    public (int sum, int max) TotalDropFinder(int[] heighArray)
    {

        int indexTop = 0;
        int indexBottom = 0;
        int max = 0; //for exp range

        List<int> dropList = new List<int>();
        int runs = 0;


        while (indexBottom < heighArray.Length - 1)
        {
            
            //if the next point is lower than current point keep searching for end of slope
            if (heighArray[indexBottom] > heighArray[indexBottom + 1])
            {
                indexBottom++;

                //code for the case that we have reached the end of the array, so the last drop doesnt get lost. 
                if (indexBottom == heighArray.Length - 1)
                {
                    dropList.Add(heighArray[indexTop] - heighArray[indexBottom]);
                }
            }
            else //if downward slope has ended add slope to list. 
            {
                //if statement to make sure we arent adding an upward slope or flat ground
                if (heighArray[indexTop] - heighArray[indexBottom] > 0)
                {
                    dropList.Add(heighArray[indexTop] - heighArray[indexBottom]);
                }
                indexBottom++;
                indexTop = indexBottom; //set top of slope to next point and start searching from there again.
            }

            runs++; //debug value.

        }

        dropList.Sort((x, y) => y.CompareTo(x)); // desc

        if(dropList[0] != null)
        {

        
        max = dropList[0];
        }

        int count = 0;
        int sum = 0;



        while(count < nbrOfDropsToLookFor && count < dropList.Count)
        {
            sum += dropList[count];
            count++;
        }
        return (sum, max);
    }




    //returns total 'amount' of terrain in level that is flat.
    private int NbrOfFlatTerrainFinder(int[] heightArray)
    {
        int totalFlatTerrain = 0;

        for(int i=0; i<heightArray.Length-1; i++)
        {
            if (heightArray[i] == heightArray[i+1])
            {
                totalFlatTerrain++;
            }
        }
        
        return totalFlatTerrain;
    }



    //sorts population in list based on fitness
    public (List<Level> pop, int mostFit, int leastFit, int avgFitness) SortByFitness(List<Level> pop)
    {
        str += "pre sort: {";
        for(int i=0; i<pop.Count; i++)
        {
            
            str += pop[i].getFitnessValue() + "," ;
        }
        str += "\n";


        pop.Sort((x, y) => y.getFitnessValue().CompareTo(x.getFitnessValue())); //sort list descending order based on fitness value

        str += "postsort: {";
        for (int i = 0; i < pop.Count; i++)
        {

            str += pop[i].getFitnessValue() + ",";
        }
        str += "\n";

        int mostFit = pop[0].getFitnessValue();
        int leastFit = pop[pop.Count-1].getFitnessValue();
        int avgFitness = 0;

        //code section that calcs avg fitness
        for(int i=0; i<pop.Count; i++)
        {
            avgFitness += pop[i].getFitnessValue();
        }
        avgFitness /= pop.Count;

        return (pop, mostFit, leastFit, avgFitness);
    } 



    //replaces bottom half of population with top half
    public List<Level> NaturalSelection(List<Level> pop)
    {
        
        for(int i= individSize-1; i>=(individSize/2); i--)
        {
            pop.RemoveAt(i);
         //   pop[pop.Count / 2 + i] = pop[i];
        }

        for(int j=0; j<(individSize/2); j++)
        {
            Level lvl = new Level(individSize,maxHeight,maxDifferenceBetweenPoints);
            
            
            int fit = pop[j].getFitnessValue();
            lvl.setFitnessValue( fit);

            int[] tempArray = new int[individSize];
            int[] copyArray = new int[individSize];
            copyArray = pop[j].getHeightArray();

            for(int x=0; x<tempArray.Length; x++)
            {
                tempArray[x] = copyArray[x];
            }

            lvl.setHeightArray(tempArray);

            pop.Add(lvl);
        }

        return pop;
    }





    public List<Level> MutatePopulation(List<Level> pop)
    {


        for (int i = (pop.Count / 2); i < pop.Count; i++)
        {

            int mutationPoint = Random.Range(1, pop[i].getHeightArray().Length - 1);
            int mutationsLeft = mutationIntervalSize;

            while (mutationsLeft > 0 && mutationPoint < pop[i].getHeightArray().Length)
            {


                //roll a mutated value thats within range of previous point
                int value = Random.Range(pop[i].getHeightArray()[mutationPoint - 1] - maxDifferenceBetweenPoints, pop[i].getHeightArray()[mutationPoint - 1] + maxDifferenceBetweenPoints);

                

                //perform corrections if value is out of bounds
                if (value < 0)
                {
                    value = 0;
                }
                if (value > maxHeight)
                {
                    value = maxHeight;
                }

                //checks if its the last point. Means we can set the value without worrying about the point after.
                if ((mutationPoint + 1) >= pop[i].getHeightArray().Length)
                {
                    pop[i].getHeightArray()[mutationPoint] = value;
                }

                //if its the last mutation spot we need to check that it's within range of next point
                else if (mutationsLeft == 1)
                {
                    int absoluteDifference = Mathf.Abs(value - pop[i].getHeightArray()[mutationPoint + 1]);
                    //if out of range of next spot
                    if (absoluteDifference > maxDifferenceBetweenPoints)
                    {
                        //if it's too low compared to next value, correct it to lowest possible 
                        if (value < pop[i].getHeightArray()[mutationPoint + 1])
                        {
                            value = pop[i].getHeightArray()[mutationPoint + 1] - maxDifferenceBetweenPoints;

                        }
                        //else correct value to highest difference the other way
                        else if (value > pop[i].getHeightArray()[mutationPoint + 1])
                        { value = pop[i].getHeightArray()[mutationPoint + 1] + maxDifferenceBetweenPoints; }
                    }

                    pop[i].getHeightArray()[mutationPoint] = value;
                }

                else
                {
                    pop[i].getHeightArray()[mutationPoint] = value;
                }


                mutationsLeft--;
                mutationPoint++;

            }
        }
        return pop;
    }



    public void GenerateLevel(int[] heightArray)
    {
        new CreateLevel(heightArray, distanceBetweenPoints, tangentLength);
    }


}




