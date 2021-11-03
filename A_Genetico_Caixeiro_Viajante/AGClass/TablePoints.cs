using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Genetico_Caixeiro_Viajante.AGClass
{
    public static class TablePoints // classe estatica, nao depende de criaçao de objeto para funcionar
    {
        // essa classe vai guardar os pontos (cidades) que foram inseridos na tela
        // bem como as distancias entre esses pontos

        // se colocar um ponto na tela, o primeiro, a coordenada X ficara na posicao 0 do vetor X
        // e a coordenada Y ficara na posicao 0 do vetor Y, e assim por diante
        private static ArrayList X = new ArrayList(); // vetor de valores de X
        private static ArrayList Y = new ArrayList(); // vetor de valores de Y
        private static double[,] tableDist; // matriz que vai guardar as distancias entre os pontos
        public static int pointCount = 0; // quantidade de pontos (cidades) na tabela

        // implementaçao dos metodos

        // funcao para adicionar o ponto inserido na tela
        public static void addPoint(int x, int y)
        {
            X.Add(x); // adicionando os paramentros para dentro dos vetores/atributos da classe
            Y.Add(y);
            pointCount++; // incrementa a quantidade de pontos inseridos na classe
            generateTable(); // gera a tabela
        }

        // funcao para gerar a tabela com os valores de distancia entre 2 pontos
        public static void generateTable()
        {
            // a cada inserção de ponto/cidade, a tabela/matriz é recriada dinamicamente
            // com o novo tamanho
            tableDist = new double[pointCount, pointCount];

            for (int i= 0; i < pointCount; i++) // coordenada X
            {
                for(int j= 0; j < pointCount; j++) // coordenada Y
                {
                    // calculo de distancia entre 2 pontos: raiz quadrada de (x2 - x1)^2 + (y2 - y1)^2
                    // sendo que x1 e y1 sao as coordenadas do ponto A e x2 e y2, as coordenadas do ponto B

                    // int.Parse(X[j].ToString()) == convertendo o valor para inteiro

                    tableDist[i, j] = Math.Sqrt(Math.Pow(int.Parse(X[i].ToString())
                                                          - int.Parse(X[j].ToString()), 2)
                                                          + Math.Pow(int.Parse(Y[i].ToString())
                                                          - int.Parse(Y[j].ToString()), 2));
                }
            }
            // atualizar o tamanho do cromosso (em relaçao a quantidade de cidades)
            ConfigurationGA.sizeChromossome = pointCount;

        }

        // funcao para retornar a tabela de distancias (a matriz)
        public static double[,] getTableDist()
        {
            return tableDist;
        }

        // funcao para encontrar a distancia de um ponto ao outro
        public static double getDist(int pointOne, int pointTwo)
        {
            // as coordenadas que deseja verificar precisam estar na matriz
            // se usar essa funcao para buscar coordenadas inexistentes, havera erro no programa
            return tableDist[pointOne, pointTwo];
        }

        // funcao para contar quantos pontos tem na tabela
        public static int count()
        {
            return pointCount;
        }

        // funcao para printar a matriz de distancias
        public static string print()
        {
            string data = string.Empty; // colocar "data" como vazio

            // percorrer a matriz de distancia entre as cidades
            for (int i= 0; i < pointCount; i++)
            {
                for (int j = 0; j < pointCount; j++)
                {
                                                     // convertendo as coordenadas de string para double
                    data += string.Format("{0:0.#}", double.Parse(tableDist[i, j].ToString())) + "            ";
                }

                data += Environment.NewLine; // funcao para pular uma linha (\n)
            }

            data += Environment.NewLine + "--------------------------------------------------------------------" + Environment.NewLine;

            return data;
        }

        // funcao para retornar as coordenadas X e Y de um ponto (cidade)
        public static int[] getCoordinates(int point)
        {
            int[] arrayCoordinates = new int[2]; // vetor de tamanho 2 para guardar as coordenadas X e Y
            arrayCoordinates[0] = int.Parse(X[point].ToString()); // converte o valor em X que esta em string para int
            arrayCoordinates[1] = int.Parse(Y[point].ToString());

            return arrayCoordinates;
        }

        // funcao para limpar (zerar) os valores dessa classe
        public static void clear()
        {
            X.Clear(); // esvazia os arrays 
            Y.Clear();
            pointCount = 0; // zera o contador
            tableDist = null; // faz a variavel apontar para o vazio
        }

    }
}
