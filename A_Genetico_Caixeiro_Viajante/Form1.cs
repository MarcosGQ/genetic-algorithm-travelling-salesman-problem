using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using A_Genetico_Caixeiro_Viajante.AGClass;
using ZedGraph;

namespace A_Genetico_Caixeiro_Viajante
{
    public partial class Form1 : Form
    {
        Graphics g; // desenhar elementos na tela
        int count = 0; // contador de pontos na tela
        int pointcount = 0; // sequenciador para identificar pontos na tela

        int evolucoes = 0;
        double bestAux; // o valor do melhor individuo, mas variavel auxiliar
        int i = 0;
        int iTemp = 0; // iteraçoes para quando clicar no botao executar/continuar, especialmente quando quer continuar

        // criar objeto de population
        Population pop;

        // criar os componentes do zedGraph, que vai mostrar a media da populacao a medida que ela evolui
        private GraphPane paneMedia;
        private PointPairList mediaPopulacao = new PointPairList();


        // construtor da interface
        public Form1()
        {
            InitializeComponent();

            paneMedia = zedMedia.GraphPane; // "paneMedia" recebe o grafico da interface
            paneMedia.Title.Text = "Média da população";
            paneMedia.XAxis.Title.Text = "Evolução";
            paneMedia.YAxis.Title.Text = "Média Fitness";
            zedMedia.Refresh(); // atualiza o grafico
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = CreateGraphics(); // gerar os graficos
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // gerar graficos 2D com filtro de imagem

        }

        protected override void OnMouseClick(MouseEventArgs e) // janela da interface
        {
            base.OnMouseClick(e);
            //Console.WriteLine("Point Insert");

            // criar um lapis para desenhar na tela
            Pen blackPen = new Pen(Color.Red, 3);
            int X= e.X, Y= e.Y; // pega quais pixels esta "pegando" na tela
            //Console.WriteLine("X: " + X + " Y: " + Y);

            // cada vez que clicar na tela para criar um ponto, ele sera atualizado na matriz
            TablePoints.addPoint(X, Y);
            
            // Criar retangulos (na verdade serao as bolinhas vermelhas)
            Rectangle rect = new Rectangle(X-5, Y-5, 10, 10);

            // criar os desenhos
            g.DrawEllipse(blackPen, rect);

            // impressao de informacoes: numero do ponto (numero da cidade)
            g.DrawString((pointcount + 1).ToString(), new Font("Arial black", 11), Brushes.Black, X + 3, Y);
            g.DrawString("X: " + X.ToString(), new Font("Arial Black", 8), Brushes.Black, X - 20, Y - 27); // quanto maior o numero em "y" mais afastado da bolinha
            g.DrawString("Y: " + Y.ToString(), new Font("Arial Black", 8), Brushes.Black, X - 20, Y - 17);

            pointcount++; // cada bolinha recebe um numero, em ordem crescente

            txtQtdCidades.Text = pointcount.ToString(); // incrementa a quantidade de cidades sempre que criar uma
            txtComplex.Text = Fatorial((ulong)pointcount).ToString();

            // implementando as funcoes dos botoes criar populacao e limpar

            if(++count >= 4) // se o numero de cidades for igual ou maior que 3
            {
                btnCriarPop.Enabled = true; // ativa o botão "criar população"
            }

            if(++count >= 1) // se tiver uma ou mais cidades, permitir limpar
            {
                btnLimpar.Enabled = true;
            }
            else
            {
                btnLimpar.Enabled = false;
            }

            // mostrar na tela a matriz preenchida
            Console.WriteLine(TablePoints.print());
        }

        // funcao fatorial recursiva

        public ulong Fatorial(ulong number) 
        {
            if(number <= 1)
            {
                return 1;
            }
            else
            {
                return number * Fatorial(number - 1);             
            }
        }

        // implementar o metodo de plotar os pontos armazenados na tabela de pontos      
        private void plotPoints()
        {
            // 1 - verificar se ha pontos na tabela de pontos
            if(TablePoints.pointCount > 0)
            {
                // plotar os pontos na tela
                for(int i= 0; i < TablePoints.pointCount; i++)
                {
                    // criar um lapis para desenhar na tela
                    Pen blackPen = new Pen(Color.Red, 3);
                    
                    // criar um vetor de inteiros (vetor de coordenadas[x,y] == [0,1])
                    int[] coo = TablePoints.getCoordinates(i); // insere em cada posicao, as coordenadas X e Y de cada ponto 'i'
                    
                    // printar o retangulo
                    Rectangle rect = new Rectangle(coo[0] - 5, coo[1] - 5, 10, 10); // coo[0]= X e coo[1]= Y
                    g.DrawEllipse(blackPen, rect);                
                    g.DrawString((i + 1).ToString(), new Font("Arial black", 11), Brushes.Black, coo[0] + 3, coo[1]);
                    g.DrawString("X: " + coo[0].ToString(), new Font("Arial Black", 8), Brushes.Black, coo[0] - 20, coo[1] - 27);
                    g.DrawString("Y: " + coo[1].ToString(), new Font("Arial Black", 8), Brushes.Black, coo[0] - 20, coo[1] - 17);
                }
            }
        }       

        // implementar as linhas/arestas de ligacao entre os pontos
        private void ploLines(Population pop, Color color)
        {
            Pen penBest = new Pen(color, 4);
            int genA, genB; // genes do individuo

            Individual best = pop.GetBest();

            // é preciso percorrer todo o cromosso do individuo
            for(int i= 0; i<ConfigurationGA.sizeChromossome; i++)
            {
                if(i < ConfigurationGA.sizeChromossome - 1) // '-1' para nao verificar apos o vetor
                {
                    genA = best.GetGene(i);
                    genB = best.GetGene(i + 1);
                }
                else
                {
                    genA = best.GetGene(i);
                    genB = best.GetGene(0); // retorna para o começo
                }

                // criar 2 vetores ára guardar as coordenadas X e Y
                int[] vetA = TablePoints.getCoordinates(genA);
                int[] vetB = TablePoints.getCoordinates(genB);

                // desenhar a linha
                g.DrawLine(penBest, vetA[0], vetA[1], vetB[0], vetB[1]);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnCriarPop_Click(object sender, EventArgs e)
        {
            ConfigurationGA.sizePopulation = int.Parse(txtTamPop.Text);
            pop = new Population();

            btnExecutar.Enabled = true;
            
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            // limpar a tabela de pontos, a parte grafica, quantidade de cidades e complexidade       
            TablePoints.clear();
            g.Clear(Color.White);
            ConfigurationGA.sizePopulation = 0;                      
            pop = null;
            txtQtdCidades.Text = "--";
            txtComplex.Text = "00";

            // limpar os atributos da classe interface
            count = 0;
            pointcount = 0;
            i = 0;
            iTemp = 0;
            evolucoes = 0;
            lbEvolucoes.Text = "00";

            // desabilita os botoes criar populacao, executar/continuar e limpar
            btnCriarPop.Enabled = false;
            btnExecutar.Enabled = false;
            btnLimpar.Enabled = false;

            // limpar o grafico
            mediaPopulacao.Clear();
            zedMedia.Refresh();
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            // desativar o botão criar nova população
            btnCriarPop.Enabled = false;

            // pegar os valores da interface
            float taxaCrossover = float.Parse(txtTaxaCrossOver.Text); // taxa de crossover
            float taxaMutacao = float.Parse(txtTaxaMutacao.Text); // taxa de mutacao
            evolucoes += int.Parse(txtEvolucao.Text); // qtd de evolucoes          
            int torneio = int.Parse(txtQtdTorneio.Text); // qtd de torneios

            bestAux = double.PositiveInfinity; // recebe um valor infinito e positivo

            // configurar o algoritmo genetico (a classe ConfigurationGA)
            ConfigurationGA.rateCrossover = taxaCrossover;
            ConfigurationGA.rateMutation = taxaMutacao;
            ConfigurationGA.numbOfCompetitors = torneio;

            // cria um tipo padrao de mutacao
            ConfigurationGA.Mutation mutacao = ConfigurationGA.Mutation.NewIndividual;

            // verifica o tipo de mutacao selecionado
            if(rbNovoIndividuo.Checked)
            {
                mutacao = ConfigurationGA.Mutation.NewIndividual;
            }
            else if (rbPopulacao.Checked)
            {
                mutacao = ConfigurationGA.Mutation.InPopulation;
            }

            // configurar o tipo de mutacao
            ConfigurationGA.mutationType = mutacao;

            // verificar se elitismo foi selecionado
            if(chElitismo.Checked)
            {
                ConfigurationGA.elitism = true;
                ConfigurationGA.sizeElitism = int.Parse(txtQtdElitismo.Text);
            }
            else
            {
                ConfigurationGA.elitism = false;
            }

            // printar as configuraçoes selecionadas na tela
            Console.WriteLine("--------------------------------------------------------------------\n");
            Console.WriteLine("População: " + ConfigurationGA.sizePopulation + "\n");
            Console.WriteLine("Tipo de Crossover: " + "PMX" + "\n");
            Console.WriteLine("Tipo de Mutação: " + ConfigurationGA.mutationType + "\n");
            Console.WriteLine("Tipo de Seleção: " + "Torneio" + "\n");
            Console.WriteLine("Elitismo: " + ConfigurationGA.elitism + ". Quantidade: " + ConfigurationGA.sizeElitism + "\n");
            Console.WriteLine("Taxa de Crossover: " + ConfigurationGA.rateCrossover + "\n");
            Console.WriteLine("Taxa de Mutação: " + ConfigurationGA.rateMutation + "\n");
            Console.WriteLine("Evoluções: " + evolucoes.ToString() + "\n");
            Console.WriteLine("--------------------------------------------------------------------\n");

            // instanciando um objeto de algoritmo genetico
            GeneticAlgorithm AG = new GeneticAlgorithm();

            // implementar o mecanismo de evolucao do algoritmo genetico
            for(i= iTemp; i < evolucoes; i++)
            {
                iTemp++; // toda vez que executar o GA, "iTemp" é incrementado
                lbEvolucoes.Text = i.ToString();
                lbEvolucoes.Refresh(); // mostrar a mudança das evoluções na interface

                // fazer a evolucao do AG
                pop = AG.ExecuteGA(pop); // executa o AG em cima dessa população e retorna o resultado nela mesmo, porem, evoluido

                // mostra a evolução da população na interface
                zedMedia.GraphPane.CurveList.Clear(); // primeiro, limpa o grafico
                zedMedia.GraphPane.GraphObjList.Clear();

                double mediaPop = pop.GetAvaragePopulation(); // pegando a media da populacao
                mediaPopulacao.Add(i, mediaPop);

                double bestFitness = pop.GetBest().getFitness(); // pegando o melhor fitness da populaçao
                lbMenorDistancia.Text = bestFitness.ToString();
                lbMenorDistancia.Refresh(); // mostrando o melhor fitness na interface

                // plotando o novo grafico
                LineItem media = paneMedia.AddCurve("Média", mediaPopulacao, Color.Red, SymbolType.None);

                // plotar as arestas a cada 6 evoluções e se o novo fitness for menor que o atual
                if(i%6 == 0 && bestFitness < bestAux)
                {
                    bestAux = bestFitness;
                    g.Clear(Color.White);
                    ploLines(pop, Color.Blue);
                    plotPoints();
                }

                zedMedia.AxisChange();
                zedMedia.Invalidate(); // nao permitir que usuario mexa no grafico enquanto estiver atualizando
                zedMedia.Refresh(); // atualiza o grafico em tempo real
            }

            g.Clear(Color.White);
            ploLines(pop, Color.Blue);
            plotPoints();
        }
    }
}
