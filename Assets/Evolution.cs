using System.Collections;
using System.Collections.Generic;
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

    //private List<Level> population;

    public Evolution(){

    //    Search();

    }


    public void Start()
    {
        Search();

    }

    private void Search() //NOOOOOOOOOOOOOOOOOOOOTEEEEE här lägga till o skriva ut siffror etc
    {
        List<Level> population = new List<Level>();
        int nbrOfGenerations = 0;

        population = InitializePopulation(population); //initialize a population with random values
        int most=0;
        int least=0;
        int avg=0;

        while (nbrOfGenerations < generationsLimit)
        {

            population = Fitness(population);   // assign them all a fitness value
            var (pop, mostFit, leastFit, avgFitness) = SortByFitness(population);  //sort based on fitness value

            //if target fitness has been reached search is stopped
            if (mostFit >= fitnessTarget)
            { break; }

            population = NaturalSelection(pop);     //replace bottom half with copies of the top half
            population = MutatePopulation(population);     //mutate the copied members of population


            //kod för printa most, least, avg till fil.

            most = mostFit;
            least = leastFit;
            avg = avgFitness;

            nbrOfGenerations++;
        }

        Debug.Log("mostfit: " + most + "\n" + "Least:"  + least);
        Debug.Log("avg: " + avg);   
  //      GenerateLevel(population[0].heightArray); //sends in top ranked candidate for level generation after search is finished

        //create level



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
        //  int populationMaxFitness = 0; ?

        int highestDrop = 0 ;
        int nbrOfFlatTerrain = 0;

        //loop through population and assign total score based on the various fitness tests.
        for(int i=0; i<pop.Count; i++)
        {
            highestDrop = 0;
            nbrOfFlatTerrain = 0;


            highestDrop = HighestDropFinder(pop[i].heightArray);
            nbrOfFlatTerrain = NbrOfFlatTerrain(pop[i].heightArray);

            pop[i].fitnessValue = highestDrop - nbrOfFlatTerrain;

        //    Debug.Log("highest= " + highestDrop + ", flat= " + nbrOfFlatTerrain + "fitness: "+ pop[i].fitnessValue);

        }
        
        return pop;
    }





    //takes a level and returns the biggest drop/downward slope in the level
    private int HighestDropFinder(int[] heightArray)
    {
        int highestDrop = 0;
        int currentDrop = 0;

        int dropTop = 0;
        int dropBottom = 0;

        //loop through heightArray
        for (int i=0; i<heightArray.Length-1; i++) 
        {
            //if a point is higher than the following point start searching for where slope ends
            if (heightArray[i] > heightArray[i + 1])
            {
                dropTop = heightArray[i];
                dropBottom = heightArray[i + 1];

                //loop to find out where the decrease in height ends, i.e where the bottom of the slope is. note that even ground also breaks slope
                for (int j=i+1; j<heightArray.Length-2; j++) 
                {
                    
                    //if the points following [i] keep decreasing set bottom of drop
                    if (heightArray[j] > heightArray[j+1])
                    {
                        dropBottom = heightArray[j+1];
                    }
                    else { break; }                                 
                }
            }
            currentDrop = dropTop - dropBottom;
     //       Debug.Log("iteration: " + i + "\nCurrent drop = " + currentDrop + "=" + dropTop + "-" +  dropBottom);
            if (currentDrop > highestDrop)
            {
                highestDrop = currentDrop;
            }
        }
        return highestDrop;
    }




    //returns total 'amount' of terrain in level that is flat.
    private int NbrOfFlatTerrain(int[] heightArray)
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

        Debug.Log("b4 sort: " + pop[0].fitnessValue + ", last slot: " + pop[pop.Count-1].fitnessValue);
        pop.Sort((x, y) => y.fitnessValue.CompareTo(x.fitnessValue));
        Debug.Log("after sort: " + pop[0].fitnessValue + ", last slot: " + pop[pop.Count - 1].fitnessValue);
        int mostFit = pop[0].fitnessValue;
        int leastFit = pop[populationSize-1].fitnessValue;
        int avgFitness = 0;

        //code section that calcs avg fitness
        for(int i=0; i<populationSize; i++)
        {
            avgFitness += pop[i].fitnessValue;
        }
        avgFitness /= populationSize;

        return (pop, mostFit, leastFit, avgFitness);
    } 

    //replaces bottom half of population with top half
    public List<Level> NaturalSelection(List<Level> pop)
    {
        for(int i=0; i<(populationSize/2); i++)
        {
            pop[populationSize / 2 + i] = pop[i];
        }

        return pop;
    }

    //mutates 2nd half of population, i.e the copies of fittest half
    public List<Level> MutatePopulation(List<Level> pop)
    {
        //    System.Array.ForEach(pop[pop.Count - 1].heightArray, System.Console.WriteLine);
    //    string str = "{";
    //    foreach(int e in pop[pop.Count - 1].heightArray)
    //    {
   //         str += e + ",";
    //    }
   //     str += "}";
   //     Debug.Log("before" + str);

        for (int i=(populationSize/2); i<populationSize; i++)
        {
            int mutationPoint = Random.Range(1, individSize - 1);
            int mutationsLeft = mutationIntervalSize;

            while(mutationsLeft>0 && mutationPoint < individSize)
            {

                //loop that randomises value of a point. it performs a check so that difference between a point and its previous point isnt more than max allowed. Prevents 'exaggerated' up/down slopes
                do
                {
                    pop[i].heightArray[mutationPoint] = Random.Range(0, maxHeight);
                } while (Mathf.Abs(pop[i].heightArray[mutationPoint] - pop[i].heightArray[mutationPoint-1]) > maxDifferenceBetweenPoints);

                mutationsLeft--;
                mutationPoint++;
            }
            
        }
     
   //      str = "{";
   //     foreach (int e in pop[pop.Count - 1].heightArray)
   //     {
  //          str += e + ",";
  //      }
   //     str += "}";
  //      Debug.Log("after" + str);

        return pop;
    }

    private void printArray(int[] array, string text)
    {
        for (int i = 0; i < array.Length; i++)
        {

        }
    }

    public void GenerateLevel(int[] levelPoints)
    {

        //make level somehow
    }


}
