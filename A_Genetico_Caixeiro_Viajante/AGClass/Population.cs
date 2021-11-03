using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Genetico_Caixeiro_Viajante.AGClass
{
    public class Population // contem um array de individuos
    {
        public Individual[] population; // grupo de individuos

        // metodos

        // construtor
        public Population()
        {
            // instanciar o array population
            this.population = new Individual[ConfigurationGA.sizePopulation];

            // inserir os individuos na populacao
            for(int i= 0; i < ConfigurationGA.sizePopulation; i++)
            {
                this.population[i] = new Individual(); // individuo aleatorio
                this.population[i].indexOfVector = i; // indicando sua posicao no vetor
            }

            // avaliar os individuos
            CalculateFitness();
        }

        // funcao para calcular fitness
        public void CalculateFitness()
        {
            for(int i= 0; i < ConfigurationGA.sizePopulation; i++)
            {
                this.population[i].CalcFitness(); // calculo do fitness de cada individuo
            }
        }

        // funcao para avaliar a populacao
        public void Evaluate()
        {
            RefreshIndexOfIndividual(); // atualizar o index (posicao) dos individuos
            CalculateFitness(); // calcular o fitness
        }

        // funcao para atualizar o index (posicao no array) do individuo na populacao
        // um individuo novo sera posto no lugar de um velho
        public void RefreshIndexOfIndividual()
        {
            for(int i= 0; i < ConfigurationGA.sizePopulation; i++)
            {
                this.population[i].indexOfVector = i; // atribuir o valor de "i" ao indexOfVector do individuo que esta na populacao
            }
        }

        // funcao para retornar a populacao atual
        public Individual[] GetPopulation()
        {
            return this.population;
        }

        // funcao para inserir um individuo na populacao
        public void SetIndividuals(int posicao, Individual individual)
        {
            this.population[posicao] = individual;
        }

        // funcao para retornar a media da populacao
        public double GetAvaragePopulation()
        {
            // somatoria do fitness total dos individuos, dividido pela quantidade de individuos
            double sum = 0;

            foreach(Individual ind in this.population)
            {
                sum += ind.getFitness(); // somatorio dos fitness dos individuos
            }

            return (sum/ConfigurationGA.sizePopulation);
        }

        // funcao para ordenar a populacao. É importante para fazer o elitismo (ordenar por melhores individuos)
        public void OrderPopulation()
        {
            // dos melhores para os piores
            // nesse caso, quanto menor o somatorio das distancias (fitness) de um individuo, melhor ele é
            Individual aux;

            for(int i= 0; i < ConfigurationGA.sizePopulation; i++) // pega um individuo na posicao "i"
            {
                for (int j = 0; j < ConfigurationGA.sizePopulation; j++) // e compara ele com todos os individuos no vetor
                {
                    // se o fitness do individuo na posicao "i" for menor que o fitness do individuo na posicao "j"
                    if (population[i].getFitness() < population[j].getFitness())
                    {
                        aux = population[i];
                        population[i] = population[j];
                        population[j] = aux;
                    }
                }
            }
        }

        // funcao para retornar o melhor individuo
        public Individual GetBest()
        {
            OrderPopulation(); // ordena a populacao
            return population[0]; // e retorna o individuo na posicao "0", pois ele é o melhor
        }

        // funcao para retornar o pior individuo
        public Individual GetBad()
        {
            OrderPopulation(); // ordena a populacao
            return population[ConfigurationGA.sizePopulation - 1]; // e retorna o individuo na ultima posicao, pois ele é o pior
        }

        // funcao para printar a populacao no console
        // override permite sobrescrever um metodo existente na classe "pai" da classe "individuo"
        // no caso, alteraremos o metodo "ToString"
        public override string ToString()
        {
            base.ToString();
            
            // qualquer coisa que escrever a partir daqui, sera o novo metodo
            string result = string.Empty;

            for(int i= 0; i < ConfigurationGA.sizePopulation; i++)
            {
                result += population[i].ToString() + "\n";
            }

            return result;
        }
    }
}
