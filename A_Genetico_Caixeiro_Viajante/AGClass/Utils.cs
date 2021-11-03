using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Genetico_Caixeiro_Viajante.AGClass
{
    public static class Utils // classe com "utilidades" para auxiliar outras classes
    {

        // metodos

        // funcao que retorna uma lista de valores numericos aleatorios e que nao repetem
        // esses numeros sao as cidades
        public static List<int> randomNumbers(int start, int end) 
        {
            List<int> numbers = new List<int>();

            for(int i= start; i < end; i++)
            {
                numbers.Add(i);
            }

            // embaralhar a lista de numeros
            for(int i= 0; i < numbers.Count; i++)
            {
                // variacao dos numeros de 0 ate a quantidade de numeros que tem na lista
                int a = ConfigurationGA.random.Next(numbers.Count);
                int temp = numbers[i];
                numbers[i] = numbers[a];
                numbers[a] = temp; // troca de posicao
            }

            return numbers.GetRange(0, end);
        }



    }
}
