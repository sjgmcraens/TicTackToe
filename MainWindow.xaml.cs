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
        TTTBoard mainBoard;

        // 0==X 1==O
        int playerToken;

        //
        bool playersTurn = false;

        //
        readonly Random globalRandom;

        public MainWindow()
        {
            InitializeComponent();
            mainBoard = new TTTBoard();
            
            globalRandom = new Random();

            NewPlayerToken(-1);

            SetPlayersTurn(true);
        }

        private void Button_Board_Click(object sender, RoutedEventArgs e)
        {
            // Check if it is players turn
            if (!playersTurn)
            {
                return;
            }

            int col = Grid.GetColumn(sender as Button);
            int row = Grid.GetRow(sender as Button);

            // Check if square is taken
            if (mainBoard.state[col,row] != -1)
            {
                return;
            }

            UpdateBoardState(col, row, playerToken);
            SetPlayersTurn(false);
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
            }
        }

        private void UpdateBoardState(int col, int row, int token)
        {
            mainBoard.state[col, row] = playerToken;
            Canvas visualToken = Get_CanvasTemplate(token);
            Grid.SetColumn(visualToken, col);
            Grid.SetRow(visualToken, row);
            Grid_Board.Children.Add(visualToken);
        }

        private Canvas Get_CanvasTemplate(int token)
        {
            Canvas C;
            switch (token)
            {
                case 0:
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
                case 1:
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
                    Canvas.SetLeft(E,-50);
                    Canvas.SetTop(E, -50);
                    C.Children.Add(E);
                    break;
                default:
                    C = new Canvas();
                    break;
            }
            return C;
        }

        private void NewPlayerToken(int lastToken)
        {
            switch (lastToken)
            {
                case 0:
                    playerToken = 1;
                    break;
                case 1:
                    playerToken = 0;
                    break;
                default:
                    playerToken = globalRandom.Next(0, 2);
                    break;
            }
            switch (playerToken)
            {
                case 0:
                    PlayerTokenVisual.Text = "X";
                    return;
                case 1:
                    PlayerTokenVisual.Text = "O";
                    return;
            }
            
        }

        class TTTBoard
        {
            // -1=empty, 0=X, 1=O
            public int[,] state;
            public TTTBoard()
            {
                state = new int[3, 3];
                Reset();
            }

            public void Reset()
            {
                for (int i=0; i< state.GetLength(0); i++)
                {
                    for (int j = 0; j < state.GetLength(1); j++)
                    {
                        state[i, j] = -1;
                    }
                }
            }
        }
    }
}
