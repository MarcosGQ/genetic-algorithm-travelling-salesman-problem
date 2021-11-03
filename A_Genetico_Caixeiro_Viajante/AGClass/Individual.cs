using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Genetico_Caixeiro_Viajante.AGClass
{
    public class Individual
    {
        private int[] chromossome; // vetor de inteiros. Cada gene (numero em cada posicao do vetor) é uma cidade
        private double fitness; // valor de aptidao do individuo
        public int indexOfVector = 0; // sera usado para inserir os novos individuos na posicao dos velhos

        // metodos

        // construtor
        public Individual()
        {
            // a classe ConfigurationGA vai determinar o tamanho do cromosso, que pode variar de tamanho
            this.chromossome = new int[ConfigurationGA.sizeChromossome];

            // cria uma lista de genes que receberá numeros aleatorios, a começar por 0 ate o numero de cidades
            List<int> genes = Utils.randomNumbers(0, ConfigurationGA.sizeChromossome);
            
            // pegar esses genes e atribui los ao cromossomo
            for(int i= 0; i < ConfigurationGA.sizeChromossome; i++)
            {
                this.chromossome[i] = genes[i];         
            }

            // definir o calculo do fitness: vai calcular a soma das distancias entre as cidades no cromossomo
            // esse valor sera o fitness/valor de aptidao desse individuo
            CalcFitness();
        }

        // funcao para avaliar o individuo
        public void Evaluate()
        {
            CalcFitness();
        }

        // funcao para retornar o cromossomo do individuo
        public int[] GetChromossome()
        {
            return this.chromossome;
        }

        // funcao para calcular o fitness
        public void CalcFitness()
        {
            // varredura do vetor de cromossomos, calculando a distancia entre cada 2 pontos
            double totalDist = 0.0; // variavel para guardar o somatorio

            for(int i= 0; i < ConfigurationGA.sizeChromossome; i++)
            {
                if(i < (ConfigurationGA.sizeChromossome - 1)) // evita que "i + 1" tente calcular fora do vetor
                {
                    // getDist retorna a distancia entre 2 pontos especificos, passados por parametro
                    totalDist += TablePoints.getDist(GetGene(i), GetGene(i + 1));
                }
                else
                {
                    // o ultimo calculo de distancia é entre a ultima cidade visitada e a primeira, pois no caixeiro viajante
                    // deve se voltar para cidade de partida
                    totalDist += TablePoints.getDist(GetGene(i), GetGene(0));
                }
            }

            // atribuir o valor de fitness
            SetFitness(totalDist);
        }

        // funcao para inserir os genes no cromossomo
        public void SetGene(int posicao, int gene)
        {
            this.chromossome[posicao] = gene;
        }

        // funcao para retornar o gene
        public int GetGene(int posicao)
        {
            return this.chromossome[posicao];
        }

        // funcao para atribuir o valor Fitness a um individuo
        public void SetFitness(double fitness)
        {
            this.fitness = fitness;
        }

        // funcao para retornar o valor fitness
        public double getFitness()
        {
            return this.fitness;
        }

        // funcao para fazer a mutacao do individuo (swap de genes)
        public void mutate(int pointOne, int pointTwo)
        {
            // 1º - verificar se o pontos 1 e 2 estao dentro das "regras"

            // pontos 1 e 2 precisam ser menor que o tamanho do cromossomo, e precisam ser diferentes entre si
            if ((pointOne < ConfigurationGA.sizeChromossome) 
                 && (pointTwo < ConfigurationGA.sizeChromossome)
                 && (pointOne != pointTwo))
            {
                // swap, trocar os genes 1 e 2 de posição
                int temp = chromossome[pointOne];
                chromossome[pointOne] = chromossome[pointTwo];
                chromossome[pointTwo] = temp;
            }
        }

        // override permite sobrescrever um metodo existente na classe "pai" da classe "individuo"
        // no caso, alteraremos o metodo "ToString"
        public override string ToString()
        {
            base.ToString();

            // qualquer coisa que escrever a partir daqui, sera o novo metodo
            string result = string.Empty;
            result += "Rota:   ";
            for(int i= 0; i < ConfigurationGA.sizeChromossome; i++)
            {
                result += (GetGene(i) + 1).ToString() + " -> ";
            }
            result += "Distancia: " + getFitness();

            return result;
        }


    }
}
