using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public int populationSize; // size of population, should be an even number
    public int fitnessTarget; // fitness value to strive for
    public int generationsLimit; // max nbr of generations allowed
    public int individSize; // how many points each member of pop should have
    public int mutationIntervalSize; //size of the random interval of points that is to be mutated
    public int maxHeight;  //highest allowed point that terrain can be
    public int maxDifferenceBetweenPoints; //max allowed delta between height of 2 points

    //private List<Level> population;

    public Evolution(){

        Search();

    }

    private void Search()
    {
        List<Level> population = new List<Level>();
        int nbrOfGenerations = 0;

        population = InitializePopulation(population); //initialize a population with random values

        while (nbrOfGenerations < generationsLimit)
        {

            population = Fitness(population);   // assign them all a fitness value
            var (pop, mostFit, leastFit, avgFitness) = SortByFitness(population);               //sort based on fitness value

            //if target fitness has been reached search is stopped
            if (mostFit >= fitnessTarget)
            { break; }

            population = NaturalSelection(population);     //replace bottom half with copies of the top half
            population = MutatePopulation(population);     //mutate the copied members of population
            



            
            nbrOfGenerations++;
        }

        GenerateLevel(population[0].heightArray); //sends in top ranked candidate for level generation after search is finished

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

    

    public List<Level> Fitness(List<Level> pop)
    {
        int populationMaxFitness = 0;



        return pop;
    }





    //sorts population in list based on fitness
    public (List<Level> pop, int mostFit, int leastFit, int avgFitness) SortByFitness(List<Level> pop)
    {


        pop.Sort((x, y) => x.fitnessValue.CompareTo(y.fitnessValue));

        int mostFit = pop[0].fitnessValue;
        int leastFit = pop[populationSize-1].fitnessValue;
        int avgFitness = 0;

        //code section that calcs avg fitness
        for(int i=0; i<populationSize; i++)
        {
            avgFitness = pop[i].fitnessValue;
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
        
        for(int i=(populationSize/2); i<populationSize; i++)
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
        return pop;
    }


    public void GenerateLevel(int[] levelPoints)
    {

        //make level somehow
    }


}
