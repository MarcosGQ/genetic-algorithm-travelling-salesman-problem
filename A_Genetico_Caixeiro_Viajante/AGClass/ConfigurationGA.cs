using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Genetico_Caixeiro_Viajante.AGClass
{
    public static class ConfigurationGA
    {
        public static int sizeChromossome = 0; // tamanho do cromossomo, aumenta conforme adiciona cidades
        public static int sizePopulation = 100; // tamanho da população
        public static Random random = new Random((int)DateTime.Now.Ticks);
        public static bool elitism = false; // definir se usara elitismo
        public static int sizeElitism = 3; // quantidade de individuos que vai participar do elitismo
        public static double rateCrossover = 0.6; // 60%
        public static double rateMutation = 0.01; // 1%
        public static int numbOfCompetitors = 3; // numero de competidores no torneio

        public static Mutation mutationType = Mutation.NewIndividual; // por padrao, sera o newindividual

        // metodos

        // "sub classe" para enumerar as possibilidades/tipos de mutacao
        public enum Mutation
        {
            // mutacao de novos individuos, na populacao e nos genes da populacao
            NewIndividual,
            InPopulation,
            InGenesPopulacao
        }

    }
}
