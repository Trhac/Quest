using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isGame;                //bezi aktualne hra
        PlayerSymbol playerSymbol;  //hracom zvoleny symbol X/O
        bool isPlayerTurn;          //je hrac na tahu
        int[,] gameMap;             //herna mapa, 0 = volne pole, 1 = hracovo pole, 2 = protivnikovo pole

        public MainWindow()
        {
            InitializeComponent();
            isGame = false;
            playerSymbol = PlayerSymbol.X;
            isPlayerTurn = true;
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            //vymazat hraci grid
            foreach (Button child in GameGrid.Children)
            {
                child.Content = "";
                child.Background = Brushes.White;
            }

            gameMap = new int[3, 3];

            //ziskat nove nastavenia
            playerSymbol = (rbSymbolX.IsChecked == true) ? PlayerSymbol.X : PlayerSymbol.O;
            isPlayerTurn = rbSelectPlayer.IsChecked == true;

            //ak PC, nechat ho zahrat
            if (!isPlayerTurn)
                ComputerMove();

            //ak hrac, zapisat jeho tah
            isGame = true;
        }

        /// <summary>
        /// Tah hraca
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //overit validitu vstupu
            if (!isPlayerTurn)
                return;

            if (!isGame)
            {
                NewGameButton_Click(null, null);
                return;
            }

            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);
            
            if (GridCellEmpty(row,column))
            {
                //zaznac tah
                button.Content = playerSymbol;
                button.Foreground = (playerSymbol == PlayerSymbol.X) ? Brushes.LightPink : Brushes.LightBlue;
                button.Background = Brushes.DarkGoldenrod;
                gameMap[row, column] = 1;
            }

            isPlayerTurn = !isPlayerTurn;
            CheckEndGame();

            if (!isGame)
                return;

            //zavolaj hrat protivnika
            ComputerMove();
        }

        /// <summary>
        /// Tah protivnika
        /// </summary>
        private async void ComputerMove()
        {
            await Task.Delay(200);

            //zahrat tah s najlepsim skore            
            ComputerMove allData = GameEngine.GetComputerMove(gameMap);
            Point move = allData.finalPosition;

            ShowGameData(allData.allMoves);

            int row = (int)move.X;
            int column = (int)move.Y;

            Button opponentButton = (Button)GetGridElement(GameGrid, row, column);
            opponentButton.Content = (playerSymbol == PlayerSymbol.O) ? PlayerSymbol.X : PlayerSymbol.O;
            opponentButton.Foreground = (playerSymbol == PlayerSymbol.X) ? Brushes.LightBlue : Brushes.LightPink;
            opponentButton.Background = Brushes.DarkGreen;

            gameMap[row, column] = 2;

            isPlayerTurn = true;
            CheckEndGame();

        }

        /// <summary>
        /// Vykresli ohodnotenie jednotlivych poli algoritmom
        /// </summary>
        /// <param name="allMoves"></param>
        private void ShowGameData(List<MoveData> allMoves)
        {
            //vymaz minule data
            foreach(TextBlock child in gameData.Children)
            {
                child.Text = "";
            }

            foreach(MoveData move in allMoves)
            {
                TextBlock text = (TextBlock)GetGridElement(gameData, (int)move.coords.X, (int)move.coords.Y);
                text.Text = move.score.ToString();
            }
        }


        private void CheckEndGame()
        {
            //pri detekcii vyhry vyhodi messagebox
            if (GameEngine.GetGameState(gameMap) != GameState.ONGOING)
            {
                string message;
                switch (GameEngine.GetGameState(gameMap))
                {
                    case GameState.DRAW:
                        message = "Je remiza.";
                        break;
                    case GameState.WIN_PLAYER:
                        message = "Vyhral uzivatel";
                        break;
                    case GameState.WIN_COMPUTER:
                        message = "Vyhral computer";
                        break;
                    default:                    
                        message = "Neznama chyba";
                        break;
                };                
                
                MessageBox.Show(message,"Tic-Tac-Toe-quest");
                isGame = false;
            }
        }
        

        private bool GridCellEmpty(int row, int column)
        {
            Button cell = (Button)GetGridElement(GameGrid, row, column);
            return cell.Content == "";
        }

        UIElement GetGridElement(Grid g, int r, int c)
        {
            for (int i = 0; i < g.Children.Count; i++)
            {
                UIElement e = g.Children[i];
                if (Grid.GetRow(e) == r && Grid.GetColumn(e) == c)
                    return e;
            }
            return null;
        }
    }
}
