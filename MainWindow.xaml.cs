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

namespace TicTackToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //
        char[] mainBoard;

        // 0==X 1==O
        char playerToken = 'x';
        char botToken = 'o';

        string currentBot = "Randy";

        //
        bool playersTurn = false;

        //
        string gameState;

        //
        readonly Random globalRandom;

        List<Canvas> visualTokens = new List<Canvas>();

        public MainWindow()
        {
            InitializeComponent();
            mainBoard = new char[9];
            mainBoard = mainBoard.Select(x => '0').ToArray();

            gameState = "start";

            globalRandom = new Random();

            SetPlayersTurn(true);
            //SetRandomTurn();
        }

        private void ResetBoard()
        {
            mainBoard = mainBoard.Select(x => '0').ToArray();
            // Remove visualTokens
            for (int i=0; i< visualTokens.Count; i++)
            {
                Grid_Board.Children.Remove(visualTokens[i]);
            }
            visualTokens = new List<Canvas>();

        }

        private void Button_Board_Click(object sender, RoutedEventArgs e)
        {
            // Check if it is players turn
            if (playersTurn)
            {
                // Get position
                int col = Grid.GetColumn(sender as Button);
                int row = Grid.GetRow(sender as Button);

                // Check if square is taken
                char bState = GetBoardState(col, row);
                if (bState != '0')
                {
                    return;
                }
                else // If not taken
                {
                    // Update board
                    DoMove(col, row, playerToken);
                }
            }
        }

        private void DoMove(int col, int row, char token)
        {
            UpdateBoardState(col, row, token);
            if (gameState == "play")
            {
                if (token == playerToken)
                {
                    SetPlayersTurn(false);
                }
                else
                {
                    SetPlayersTurn(true);
                }
            }
        }

        private char GetBoardState(int col, int row)
        {
            int i = row * 3 + col;
            return mainBoard[i];
        }

        private void SetPlayersTurn(bool pTurn)
        {
            if (pTurn)
            {
                playersTurn = true;
                TurnNotifier.Text = "It's your turn";
            }
            else
            {
                playersTurn = false;
                TurnNotifier.Text = "It's your opponents turn";
                DoBotMove();
            }
        }

        private void DoBotMove()
        {
            int move;
            switch (currentBot)
            {
                case "Randy":   move = RandyNextMove(); break;
                case "ToeFish": move = ToeFishNextMove(); break;
                default:        move = RandyNextMove(); break;
            }
            DoMove(move%3, move/3, botToken);
        }

        private void UpdateBoardState(int col, int row, char token)
        {
            mainBoard[row * 3 + col] = token;

            Canvas visualToken = Get_CanvasTemplate(token);
            Grid.SetColumn(visualToken, col);
            Grid.SetRow(visualToken, row);
            visualTokens.Add(visualToken);
            Grid_Board.Children.Add(visualToken);

            // If Endstate
            switch (GetEndState(mainBoard))
            {
                case '0':
                    break;
                case 'd':
                    InvokeEndState("draw");
                    break;
                case 'x':
                    InvokeEndState("playerWin");
                    break;
                case 'o':
                    InvokeEndState("botWin");
                    break;
            }
        }

        private void InvokeEndState(string state)
        {
            gameState = "end";

            switch (state)
            {
                case "draw":      TextBlock_EndGame_Title.Text = "It's a draw!"; TextBlock_EndGame_SubTitle.Text = "This tends to happen a lot..."; break;
                case "playerWin": TextBlock_EndGame_Title.Text = "You won!";     TextBlock_EndGame_SubTitle.Text = "Huh... This wasn't supposed to happen."; break;
                case "botWin":    TextBlock_EndGame_Title.Text = "You lost!";    TextBlock_EndGame_SubTitle.Text = "Who would have guessed."; break;
            }
            Border_EndGame.Visibility = Visibility.Visible;
        }

        private Canvas Get_CanvasTemplate(int token)
        {
            Canvas C;
            switch (token)
            {
                case 'x':
                    C = new Canvas()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    C.Children.Add(new Line()
                    {
                        X1 = -50,
                        Y1 = -50,
                        X2 = 50,
                        Y2 = 50,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    });
                    C.Children.Add(new Line()
                    {
                        X1 = -50,
                        Y1 = 50,
                        X2 = 50,
                        Y2 = -50,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    });
                    break;
                case 'o':
                    C = new Canvas()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    Ellipse E = new Ellipse()
                    {
                        Width = 100,
                        Height = 100,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5
                    };
                    Canvas.SetLeft(E, -50);
                    Canvas.SetTop(E, -50);
                    C.Children.Add(E);
                    break;
                default:
                    C = new Canvas();
                    break;
            }
            return C;
        }

        private void SetRandomTurn()
        {
            switch (globalRandom.Next(0, 2))
            {
                case 0:
                    SetPlayersTurn(true);
                    return;
                case 1:
                    SetPlayersTurn(false);
                    return;
            }
        }

        private int ToeFishNextMove()
        {
            /// Based on the current board, determine the best move.
            /// Bot token == 'x'


            // We keep a dictionairy of moves
            Dictionary<int, float> moveScores = new Dictionary<int, float>();

            // Calc move scores
            foreach (int move in Enumerable.Range(0, mainBoard.Length).Where(x => mainBoard[x] == '0'))
            {
                moveScores.Add(move, GetMoveScore(GetCopy(mainBoard), move));
            }

            // Do random best move
            return RandomChoice(moveScores.Where(p => p.Value == moveScores.Values.Max()).Select(x => x.Key).ToArray());
        }

        private int RandyNextMove()
        {
            // Do random legal move
            return RandomChoice(Enumerable.Range(0, mainBoard.Length).Where(x => mainBoard[x] == '0').ToArray());
        }



        private float GetMoveScore(char[] board, int move)
        {
            // Do move
            board[move] = botToken;

            // Return score based on possible end state.
            switch (GetEndState(board))
            {
                case 'o':
                    return 1F;
                case 'd':
                    return 0F;
            }

            // Determine score based on players next move
            for (int i = 0; i < board.Length; i++)
            {
                // If legal move
                if (board[i] == '0')
                {
                    var b = GetCopy(board);
                    b[i] = playerToken;
                    switch (GetEndState(b))
                    {
                        case 'x': // PlayerWin
                            return -1F;
                        case 'd':
                            return 0F;
                    }
                }
            }

            // Determine score based on deeper movesets
            float score = 0;

            // No end state ==> go deeper
            foreach (int m in Enumerable.Range(0, board.Length).Where(x => board[x] == '0'))
            {
                // Calc score ==> depth means /10
                score += GetMoveScore(GetCopy(board), m) / 10;
            }
            return score;
        }

        public static char GetEndState(char[] b)
        {
            // Returns 'x' for playerWin
            // Returns 'd' for draw
            // Returns 'o' for botWin
            // Returns '0' for no endstate

            if (b[4] != '0')
            {
                if (b[4] == b[0] && b[4] == b[8] ||
                    b[4] == b[2] && b[4] == b[6] ||
                    b[4] == b[1] && b[4] == b[7] ||
                    b[4] == b[3] && b[4] == b[5])
                {
                    return b[4];
                }
            }

            if (b[0] != '0')
            {
                if (b[0] == b[1] && b[0] == b[2] ||
                    b[0] == b[3] && b[0] == b[6])
                {
                    return b[0];
                }
            }

            if (b[8] != '0')
            {
                if (b[8] == b[2] && b[8] == b[5] ||
                    b[8] == b[6] && b[8] == b[7])
                {
                    return b[8];
                }
            }

            // Check for draw
            if (b.All(x => x != '0'))
            {
                return 'd';
            }

            // No end state detected
            return '0';
        }

        private void Button_EndGame_Return_Click(object sender, RoutedEventArgs e)
        {
            Border_EndGame.Visibility = Visibility.Hidden;
            ResetGame();
        }

        private void ResetGame()
        {
            gameState = "start";
            Border_StartGame.Visibility = Visibility.Visible;
            ResetBoard();
        }

        private void Button_StartGame_Click(object sender, RoutedEventArgs e)
        {
            gameState = "play";
            Border_StartGame.Visibility = Visibility.Hidden;

            // If bot's turn
            SetRandomTurn();
            if (!playersTurn)
            {
                DoBotMove();
            }
        }

        private void Button_MenuReset_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        private char[] GetCopy(char[] arr)
        {
            char[] arrC = new char[arr.Length];
            Array.Copy(arr, arrC, arr.Length);
            return arrC;
        }


        private int RandomChoice(int[] E)
        {
            return E[globalRandom.Next(E.Length)];
        }

        private void Button_ChooseBot_Click(object sender, RoutedEventArgs e)
        {
            switch (currentBot)
            {
                case "Randy": SetCurrentBot("ToeFish"); break;
                case "ToeFish": SetCurrentBot("Randy"); break;
            }
        }

        private void SetCurrentBot(string botName)
        {
            switch (botName)
            {
                case "Randy": 
                    currentBot = "Randy";
                    break;
                case "ToeFish": 
                    currentBot = "ToeFish"; 
                    break;
            }
            TextBlock_CurrentBot.Text = currentBot;
        }
    }
}
