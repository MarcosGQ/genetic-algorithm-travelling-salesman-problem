using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Genetico_Caixeiro_Viajante.AGClass
{
    public class GeneticAlgorithm // classe que vai implementar o algoritmo genetico
    {
        private double rateCrossover; // taxa de cruzamento
        private double rateMutation; // taxa de mutacao

        // metodos

        // funcao para auxiliar na escolha do tipo de crossover
        public delegate Individual[] Crossover(Individual father1, Individual father2); // insere 2 pais e retorna um vetor de individuos
        // instanciar um objeto Crossover
        public Crossover crossover;

        // funcao para auxiliar na escolha do tipo de selecao
        public delegate Individual Selection(Population pop);
        // instanciar um objeto selection
        public Selection selection;

        // construtor
        public GeneticAlgorithm()
        {
            // parametros iniciais
            this.crossover = CrossoverPMX;
            this.selection = Tournament;

            // definir a taxa de crossover e mutacao
            this.rateCrossover = ConfigurationGA.rateCrossover;
            this.rateMutation = ConfigurationGA.rateMutation;
        }

        // execucao do algoritmo genetico
        public Population ExecuteGA(Population pop)
        {
            // passo a passo:
            // 1º - criar populacao, que ja vira por parametro
            // inicio do algoritmo genetico

            Population newPopulation = new Population(); // cria uma nova população. Ela sera retornada por esse metodo
            List<Individual> popTemp = new List<Individual>(); // cria uma lista de populacao temporaria (facilita implementacao)

            // atribuir individuos na populacao temporaria
            for(int i= 0; i < ConfigurationGA.sizePopulation; i++)
            {
                popTemp.Add(pop.GetPopulation()[i]); // passa a populacao do parametro para a temporaria
            }

            // 2º - verificar se vai fazer elitismo
            // definir a quantidade de individuos que vao participar do elitismo
            Individual[] indElit = new Individual[ConfigurationGA.sizeElitism];

            // testar condicao para saber se fará elitismo
            if(ConfigurationGA.elitism)
            {
                // 1º - ordenar os individuos
                pop.OrderPopulation();

                // 2º - pegar os individuos selecionados
                for(int i= 0; i < ConfigurationGA.sizeElitism; i++)
                {
                    indElit[i] = pop.GetPopulation()[i];
                }
            }

            // 3º, 4º e 5º - selecao dos pais (torneio), cruzamento e mutação
            // cada execução desse "for" gera 2 filhos, entao so vai rodar ate a metade da populacao total
            // para que assim, seja gerado uma quantidade de filhos igual ao total da populacao
            for (int i= 0; i < (ConfigurationGA.sizePopulation/2); i++) 
            {
                // seleçao dos pais
                Individual father1 = selection(pop); // chama a funcao "Tournament"
                Individual father2 = selection(pop); // e retorna o vencedor (aquele com menor fitness)

                // cruzamento dos pais
                // verificar se o individuo esta na taxa de cruzamento
                double sortCrossNum = ConfigurationGA.random.NextDouble(); // gera um numero aleatorio

                if (sortCrossNum <= rateCrossover) // "rateCrossover" foi definido no construtor, linha 34
                {
                    Console.WriteLine("Crossover em: " + i);
                    Individual[] children = crossover(father1, father2); // chama a funcao "CrossoverPMX" e retorna 2 novos individuos

                    // mutação
                    //se a mutaçao escolhida (na interface) for para novos individuos
                    if(ConfigurationGA.mutationType == ConfigurationGA.Mutation.NewIndividual)
                    {
                        children[0] = Mutation(children[0]);
                        children[1] = Mutation(children[1]);
                    }
                    // rearanjar os filhos no vetor
                    children[0].indexOfVector = father1.indexOfVector; 
                    children[1].indexOfVector = father2.indexOfVector;

                    popTemp[father1.indexOfVector] = children[0]; // esse filho substitui seu pai, ocupando sua posicao no vetor
                    popTemp[father2.indexOfVector] = children[1];                   
                }
                else // se nao for realizado o cruzamento
                {
                    popTemp[father1.indexOfVector] = father1; // mantem os pais
                    popTemp[father2.indexOfVector] = father2;
                }
            }

            // eliminar os individuos velhos e inserir os novos

            for(int i= 0; i < ConfigurationGA.sizePopulation; i++)
            {
                newPopulation.SetIndividuals(i, popTemp[i]);
            }
            popTemp = null; // nao precisamos mais desse vetor

            //se a mutaçao escolhida (na interface) for para população geral
            if (ConfigurationGA.mutationType == ConfigurationGA.Mutation.InPopulation)
            {
                newPopulation = MutationThePopulation(newPopulation); // aplica nova mutacao na nova populacao
            }

            // 6º - inserir nova populacao
            // inserir os individuos velhos "salvos" pelo elitismo
            if (ConfigurationGA.elitism) // se foi selecionado na interface
            {
                // avalia a populacao
                newPopulation.Evaluate();
                // ordenar a populaçao: colocar os individuos "salvos" no lugar dos piores
                newPopulation.OrderPopulation();
                // valor de inicio da insercao dos individuos "salvos"
                int initPoint = ConfigurationGA.sizePopulation - ConfigurationGA.sizeElitism;
                int count = 0;

                for(int i= initPoint; i < ConfigurationGA.sizePopulation; i++)
                {
                    newPopulation.SetIndividuals(i, indElit[count]);
                    count++;
                }

            }

            // 7º - avaliacao novamente
            newPopulation.Evaluate();
          
            return newPopulation;
        }

        // funcao de cruzamento
        public Individual[] CrossoverPMX(Individual father1, Individual father2)
        {
            // criar a variavel de retorno (os 2 individuos que serao gerados nesse metodo)
            // passa 2 individuos por parametro e retorna outros 2
            Individual[] newInd = new Individual[2];

            // criar variaveis para fazer substituicao e cruzamento de individuos
            int[] parent1 = new int[ConfigurationGA.sizeChromossome]; // vetor "pai" de tamanho igual ao vetor de cromossomos
            int[] parent2 = new int[ConfigurationGA.sizeChromossome];

            int[] offspring1Vector = new int[ConfigurationGA.sizeChromossome];
            int[] offspring2Vector = new int[ConfigurationGA.sizeChromossome];

            // criar um vetor para recolocar no lugar
            int[] replacement1 = new int[ConfigurationGA.sizeChromossome];
            int[] replacement2 = new int[ConfigurationGA.sizeChromossome];

            // fazer a selecao dos pontos para cruzamento
            // vai gerar um numero aleatorio de 0 ao tamanho maximo do vetor de cromossomos - 1
            int firstPoint = ConfigurationGA.random.Next(0, ConfigurationGA.sizeChromossome - 1);
            int secondPoint = ConfigurationGA.random.Next(0, ConfigurationGA.sizeChromossome - 1);

            if(firstPoint == secondPoint) // para garantir que firstPoint e secondPoint serao diferentes entre si
            {
                secondPoint = ConfigurationGA.random.Next(firstPoint, ConfigurationGA.sizeChromossome - 1);
            }

            if(secondPoint < firstPoint) // por definicao, o primeiro ponto precisa ser maior que o segundo
            {
                int temp = secondPoint;
                secondPoint = firstPoint;
                firstPoint = temp;
            }

            // teste do metodo
            //Console.WriteLine("P1: " + firstPoint + " P2: " + secondPoint);

            // instanciar os novos individuos
            newInd[0] = new Individual();
            newInd[1] = new Individual();

            // processo de transferencia dos genes para um "parent"
            for(int i= 0; i < ConfigurationGA.sizeChromossome; i++)
            {
                // pega o gene do "father" que esta na posicao "i" e passa para a mesma posicao em "offspring" e "parent"
                parent1[i] = offspring1Vector[i] = father1.GetGene(i);
                parent2[i] = offspring2Vector[i] = father2.GetGene(i);
            }

            // popular o replacement em valores abaixo de 0
            for (int i = 0; i < ConfigurationGA.sizeChromossome; i++)
            {
                replacement1[i] = replacement2[i] = -1;
            }

            // fazer inversao dos genes (passo de cruzamento) dentro do "offspringvector"
            for (int i = firstPoint; i <= secondPoint; i++)
            {
                // "offspring1Vector" representa o "parent1" e vai receber partes dos genes do "parent2"
                offspring1Vector[i] = parent2[i];
                // "offspring2Vector" representa o "parent2" e vai receber partes dos genes do "parent1"
                offspring2Vector[i] = parent1[i];

                // "replacement1" recebe o gene de "parent1" na posicao do gene de "parent2"
                replacement1[parent2[i]] = parent1[i];
                // "replacement2" recebe o gene de "parent2" na posicao do gene de "parent1"
                replacement2[parent1[i]] = parent2[i];
            }

            // troca dos genes repetidos (caixeiro viajante não pode visitar a mesma cidade mais de uma vez)
            for(int i= 0; i < ConfigurationGA.sizeChromossome; i++)
            {
                // se "i" for maior ou igual ao "firstPoint" e menor ou igual ao "secondPoint", continue
                if ((i >= firstPoint) && (i <= secondPoint))
                    continue;

                // definir aonde vai trocar os genes
                int n1 = parent1[i];
                int m1 = replacement1[n1];

                int n2 = parent2[i];
                int m2 = replacement2[n2];

                while(m1 != -1)
                {
                    n1 = m1;
                    m1 = replacement1[m1];
                }
                while (m2 != -1)
                {
                    n2 = m2;
                    m2 = replacement2[m2];
                }

                offspring1Vector[i] = n1;
                offspring2Vector[i] = n2;
            }

            // colocar os genes nos individuos
            for (int i = 0; i < ConfigurationGA.sizeChromossome; i++)
            {
                newInd[0].SetGene(i, offspring1Vector[i]);
                newInd[1].SetGene(i, offspring2Vector[i]);
            }

            // calcular o fitness desses 2 novos individuos
            newInd[0].CalcFitness();
            newInd[1].CalcFitness();

            return newInd;
        }

        // funcao de mutacao de individuo
        public Individual Mutation(Individual ind)
        {
            // verificar se o individuo precisa passar pela mutacao
            // vai gerar um numero aleatorio de 0 a 1, e se for menor ou igual a taxa de mutacao
            if(ConfigurationGA.random.NextDouble() <= rateMutation)
            {
                Console.WriteLine("Mutação");
                // definir os 2 pontos de troca de genes dos cromossomos do individuo
                // essa definicao é aleatoria, como tudo no GA é aleatorio
                int genePosition1 = ConfigurationGA.random.Next(0, ConfigurationGA.sizeChromossome - 1);
                int genePosition2 = ConfigurationGA.random.Next(0, ConfigurationGA.sizeChromossome - 1);

                //Console.WriteLine("P1: " + genePosition1 + " P2: " + genePosition2);

                // verificacao: esses pontos de troca nao podem ser iguais e o primeiro nao pode ser maior que o segundo
                if (genePosition1 == genePosition2)
                {
                    if(genePosition2 < ConfigurationGA.sizeChromossome - 2)
                    {
                        genePosition2++;
                    }
                }

                // faz a mutacao 
                ind.mutate(genePosition1, genePosition2);
                // retorna o individuo mutado
                return ind;
            }

            // se nao cair na margem de mutacao, apenas retorna o individuo
            return ind;
        }

        // funcao de mutacao da populacao
        public Population MutationThePopulation(Population pop)
        {
            // praticamente igual ao metodo mutation acima, mas ao inves de ser com 1 individuo, é com toda a populacao
            for (int i= 0; i < ConfigurationGA.sizePopulation; i++)
            {
                // verificar se os individuos precisam passar pela mutacao
                // vai gerar um numero aleatorio de 0 a 1, e se for menor ou igual a taxa de mutacao
                if (ConfigurationGA.random.NextDouble() <= rateMutation)
                {
                    // definir os 2 pontos de troca de genes dos cromossomos do individuo
                    // essa definicao é aleatoria, como tudo no GA é aleatorio
                    int genePosition1 = ConfigurationGA.random.Next(0, ConfigurationGA.sizeChromossome - 1);
                    int genePosition2 = ConfigurationGA.random.Next(0, ConfigurationGA.sizeChromossome - 1);

                    // verificacao: esses pontos de troca nao podem ser iguais e o primeiro nao pode ser maior que o segundo
                    if (genePosition1 == genePosition2)
                    {
                        if (genePosition2 < ConfigurationGA.sizeChromossome - 2)
                        {
                            genePosition2++;
                        }
                        else
                        {
                            genePosition2 -= 2; // menos 2
                        }
                    }

                    // faz a mutacao 
                    pop.GetPopulation()[i].mutate(genePosition1, genePosition2); // retorna um vetor de individuos                    
                }
            }
            // retorna a populacao com individuos mutados ou nao
            return pop;
        }

        // funcao de mutacao dos genes da populacao
        public Population MutationGenesOfPopulation(Population pop)
        {
            return null;
        }

        // funcao de selecao por torneio
        public Individual Tournament(Population pop)
        {
            // seleciona aleatoriamente "n" individuos da população e eles vao disputar entre si
            // quem tiver menor fitness vencerá esse torneio. No minimo, 2 individuos disputam o torneio

            // lista de competidores de tamanho igual ao numero estabelecido em "numbOfCompetitors", da classe ConfigurationGA
            Individual[] competitors = new Individual[ConfigurationGA.numbOfCompetitors];

            // criar um individuo auxiliar para ajudar na verificação do melhor individuo do torneio
            Individual aux = new Individual();
            // coloca fitness infinito e positivo, para que ele tenha o maior fitness (o pior)
            aux.SetFitness(float.PositiveInfinity);

            // seleçao dos competidores
            for(int i= 0; i < ConfigurationGA.numbOfCompetitors; i++)
            {
                competitors[i] = new Individual();
                // retorna um vetor de individuos, escolhidos aleatoriamente
                competitors[i] = pop.GetPopulation()[ConfigurationGA.random.Next(0, ConfigurationGA.sizePopulation - 1)];

                //Console.WriteLine("competidores: " + competitors[i] + "\n");
            }

            // escolher o vencedor
            for (int i = 0; i < ConfigurationGA.numbOfCompetitors; i++)
            {
                if(competitors[i].getFitness() < aux.getFitness())
                {
                    aux = competitors[i];
                    aux.CalcFitness();
                }
            }

            // retorna o individuo vencedor
            return aux;
        }
    }
}
