using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection.Emit;
using MegaKnightChess.Domain;
using yt_DesignUI;
using System.Drawing.Drawing2D;
using Label = System.Windows.Forms.Label;
using System.Diagnostics;
using System.IO;
using System.Drawing.Text;

namespace MegaKnightChess
{
    public partial class ChessForm : Form
    {
        public Image StartChessBack;

        public Image MainChessBack;

        public Image ClassicChessSprites;

        public Image MemesChessSprites;

        public Image FruitsChessSprites;

        public Image ChessSprites;

        static readonly int BoardSize = 8;

        public int ForKingSkinWhite = 0;

        public int ForKingSkinBlack = 0;

        public Game CurrentGame = new(BoardSize);

        public Button[,] CellButtons = new Button[BoardSize, BoardSize];

        public Button PreviousCellButton;

        public bool IsButtonsShowed = false;

        public Action GameMode { get; private set; }

        public yt_Button Start = new()
        {
            Text = "Start" + "\n" + "(choose mode)",
            BackColor = Color.FromArgb(200, 3, 20, 50),
            BackColorAdditional = Color.Black,
            RoundingEnable = true,
            Rounding = 20,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
        };

        public yt_Button StartDefaultGame = new()
        {
            Text = "Режим: обычный",
            //Font = new Font(fonts[0], 35),
            BackColor = Color.FromArgb(200, 3, 25, 50),
            BackColorAdditional = Color.Black,
            RoundingEnable = true,
            Rounding = 20,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
        };

        public yt_Button StartKnightMadnessGame = new()
        {
            Text = "Режим: megaknight madness",
            BackColor = Color.FromArgb(200, 3, 15, 50),
            BackColorAdditional = Color.Black,
            RoundingEnable = true,
            Rounding = 20,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
        };

        public yt_Button StartKnightInfectionGame = new()
        {
            Text = "Режим: knight infection",
            BackColor = Color.FromArgb(200, 13, 15, 50),
            BackColorAdditional = Color.Black,
            RoundingEnable = true,
            Rounding = 20,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
        };

        public yt_Button Restart = new()
        {
            Size = new Size(200, 50),
            Location = new Point((BoardSize + 2) * 75, 25),
            Text = "начать заново",
            BackColor = Color.DarkSlateGray,
            BackColorAdditional = Color.Black,
            RoundingEnable = true,
            Rounding = 20,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
        };

        public Label Turn = new()
        {
            Size = new Size(200, 40),
            Location = new Point(BoardSize * 75 + 25, 200),
            Text = "текущий ход: ",
            BackColor = Color.Transparent,
            ForeColor = Color.White
        };

        public Label WhiteCheck = new()
        {
            Size = new Size(200, 60),
            Location = new Point(BoardSize * 75 +25, 250),
            Text = "Черные ставят " + "\n" + "шах белым",
            BackColor = Color.Transparent,
            ForeColor = Color.White
        };

        public Label BlackCheck = new()
        {
            Size = new Size(200, 60),
            Location = new Point(BoardSize * 75 + 25, 250),
            Text = "Белые ставят " + "\n" + "шах черным",
            BackColor = Color.Transparent,
            ForeColor = Color.White
        };

        public Label WhiteMate = new()
        {
            Size = new Size(200, 60),
            Location = new Point(BoardSize * 75 + 25, 250),
            Text = "Мат. " + "\n" + "Черные победили",
            BackColor = Color.Transparent,
            ForeColor = Color.White
        };

        public Label BlackMate = new()
        {
            Size = new Size(200, 60),
            Location = new Point(BoardSize * 75 + 25, 250),
            Text = "Мат. " + "\n" + "Белые победили",
            BackColor = Color.Transparent,
            ForeColor = Color.White
        };

        public Label Draw = new()
        {
            Size = new Size(200, 40),
            Location = new Point(BoardSize * 75 + 25, 250),
            Text = "Пат. Ничья",
            BackColor = Color.Transparent,
            ForeColor = Color.White
        };

        public yt_Button BackTurn = new()
        {
            Size = new Size(100, 40),
            Location = new Point(BoardSize * 75 + 25, 25),
            Text = "Ход назад",
            BackColor = Color.DarkViolet,
            BackColorAdditional = Color.Black,
            RoundingEnable = true,
            Rounding = 50,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
        };

        public yt_Button NextTurn = new()
        {
            Size = new Size(100, 40),
            Location = new Point(BoardSize * 75 + 25, 75),
            Text = "Ход вперед",
            BackColor = Color.DarkTurquoise,
            BackColorAdditional = Color.Black,
            RoundingEnable = true,
            Rounding = 50,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
        };

        public yt_Button Skins = new()
        {
            Size = new Size(200, 50),
            Location = new Point((BoardSize + 2) * 75, 100),
            Text = "Скины на фигуры",
            BackColor = Color.DarkSlateBlue,
            BackColorAdditional = Color.Black,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
            RoundingEnable = true,
            Rounding = 20,
        };

        public Button SkinClassicBut = new()
        {
            Size = new Size(74, 74),
            Location = new Point((BoardSize + 2) * 75 + 100, 150),
        };

        public Button SkinMemesBut = new()
        {
            Size = new Size(74, 74),
            Location = new Point((BoardSize + 2) * 75 + 100, 225),
        };

        public Button SkinFruitsBut = new()
        {
            Size = new Size(74, 74),
            Location = new Point((BoardSize + 2) * 75 + 100, 300),
        };

        public yt_Button DocumentationBut = new()
        {
            Size = new Size(50, 50),
            Location = new Point(BoardSize * 75 + 375 - 50, BoardSize * 75 - 55),
            Text = "?",
            BackColor = Color.FromArgb(100, 20, 150, 250),
            BackColorAdditional = Color.Black,
            BackColorGradientEnabled = true,
            BackColorGradientMode = LinearGradientMode.ForwardDiagonal,
            RoundingEnable = true,
            Rounding = 30,
        };

        public bool IsFigureChoosen { get; set; }

        public ChessForm()
        {
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            AddAndSetFonts();
            var screenWidth = Screen.PrimaryScreen.Bounds.Size.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Size.Height;
            StartChessBack = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Sprites\\StartDisplay.jpg"));
            MainChessBack = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Sprites\\MainDisplay.jpg"));
            Image background = new Bitmap(screenWidth, screenHeight);
            var backgroundGraphics = Graphics.FromImage(background);
            backgroundGraphics.DrawImage(StartChessBack, new Rectangle(0, 0, screenWidth, screenHeight),
                0, 0, 1920, 1080, GraphicsUnit.Pixel);
            BackgroundImage = background;
            ClassicChessSprites = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Sprites\\mychessBig.png"));
            MemesChessSprites = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Sprites\\mychessmemesBig.png"));
            FruitsChessSprites = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Sprites\\mychessfruitsBig.png"));
            ChessSprites = ClassicChessSprites;
            Start.Size = new Size(screenWidth / 11 * 5, screenHeight / 3);
            Start.Location = new Point(screenWidth / 11 * 3, screenHeight / 2);
            Controls.Add(Start);
            Start.Click += new EventHandler(ChooseMode);
        }

        public void AddAndSetFonts()
        {
            var fonts = new PrivateFontCollection();
            fonts.AddFontFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Fonts\\American Captain.ttf"));
            fonts.AddFontFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\Fonts\\Azonix.otf"));
            Start.Font = new Font(fonts.Families[1], 50);
            StartDefaultGame.Font = new Font(fonts.Families[0], 35);
            StartKnightMadnessGame.Font = new Font(fonts.Families[0], 35);
            StartKnightInfectionGame.Font = new Font(fonts.Families[0], 35);
            Restart.Font = new Font(fonts.Families[0], 15);
            Turn.Font = new Font(fonts.Families[0], 16);
            BlackCheck.Font = new Font(fonts.Families[0], 16);
            WhiteCheck.Font = new Font(fonts.Families[0], 16);
            BlackMate.Font = new Font(fonts.Families[0], 16);
            WhiteMate.Font = new Font(fonts.Families[0], 16);
            Draw.Font = new Font(fonts.Families[0], 16);
            BackTurn.Font = new Font(fonts.Families[0], 11);
            NextTurn.Font = new Font(fonts.Families[0], 11);
            Skins.Font = new Font(fonts.Families[0], 14);
            DocumentationBut.Font = new Font(fonts.Families[0], 23);
        }

        public void StartInit()
        {
            CreateBoard();
            RemakeBoardView();
            AddButtonsPictures();
            Controls.Add(Skins);
            Skins.Click += new EventHandler(InitSkinsControls);
        }

        public void SetDefaultMode(object sender, EventArgs args)
        {
            StartDefaultGame = sender as yt_Button;
            GameMode = CurrentGame.StartDefaultGame;
            StartGame();
        }

        public void SetKnightMadnessMode(object sender, EventArgs args)
        {
            StartKnightMadnessGame = sender as yt_Button;
            GameMode = CurrentGame.StartKnightMadnessGame;
            StartGame();
        }

        public void SetKnightIndectionMode(object sender, EventArgs args)
        {
            StartKnightMadnessGame = sender as yt_Button;
            GameMode = CurrentGame.StartKnightInfectionGame;
            StartGame();
        }

        public void CreateBoard()
        {
            GameMode();
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    var cellOnBoard = CurrentGame.MyBoard.Cells[i, j];
                    CellButtons[i, j] = new Button();

                    Button cell = new();
                    cell.Size = new Size(75, 75);
                    cell.Location = new Point(i * 75, j * 75);
                    ReviewCell(i, j);
                    cell.BackColor = Color.White;
                    cell.Click += new EventHandler(ClickCellButton);
                    Controls.Add(cell);
                    CellButtons[i, j] = cell;
                }
            }
        }

        public void RemakeBoardView()
        {
            for(int x = 0; x < BoardSize; x++)
            {
                for(int y = 0; y < BoardSize; y++)
                {
                    ReviewCell(x, y);
                }
            }
            if (PreviousCellButton == null) return;
            if (CurrentGame.MyBoard.Cells[PreviousCellButton.Location.X / 75, 
                                          PreviousCellButton.Location.Y / 75].HasFigure())
            {
                if (CurrentGame.MyBoard.Cells[PreviousCellButton.Location.X / 75, 
                                              PreviousCellButton.Location.Y / 75]
                        .CheckFigureColour() == CurrentGame.ColourOfCurrentPlayer)
                {
                    PreviousCellButton.BackColor = Color.Red;
                    ShowLegalMoves();
                    IsFigureChoosen = true;
                }
                else
                {
                    CloseSteps();
                    IsFigureChoosen = false;
                }
            }
        }

        public void ReviewCell(int x, int y)
        {
            var cellOnBoard = CurrentGame.MyBoard.Cells[x, y];
            switch (cellOnBoard.Figure)
            {
                case null:
                    CellButtons[x, y].BackgroundImage = null;
                    break;
                case Figure:
                    switch (cellOnBoard.Figure.Colour)
                    {
                        case Colour.White:
                            switch (cellOnBoard.Figure.Type)
                            {
                                case FigureType.Pawn:
                                    Image whitePawn = new Bitmap(75, 75);
                                    Graphics graphics1 = Graphics.FromImage(whitePawn);
                                    graphics1.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 5, 0, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x,y].BackgroundImage = whitePawn;
                                    break;

                                case FigureType.Knight:
                                    Image whiteKnight = new Bitmap(75, 75);
                                    Graphics graphics2 = Graphics.FromImage(whiteKnight);
                                    graphics2.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 3, 0, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = whiteKnight;
                                    break;

                                case FigureType.EliteKnight:
                                    Image whiteEliteKnight = new Bitmap(75, 75);
                                    Graphics graphics5 = Graphics.FromImage(whiteEliteKnight);
                                    graphics5.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 4, 0, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = whiteEliteKnight;
                                    break;

                                case FigureType.Megaknight:
                                    Image whiteMegaknight = new Bitmap(75, 75);
                                    Graphics graphics3 = Graphics.FromImage(whiteMegaknight);
                                    graphics3.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 1, 0, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = whiteMegaknight;
                                    break;

                                case FigureType.King:
                                    Image whiteKing = new Bitmap(75, 75);
                                    Graphics graphics4 = Graphics.FromImage(whiteKing);
                                    graphics4.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * ForKingSkinWhite, 0, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = whiteKing;
                                    break;
                            }
                            break;
                        case Colour.Black:
                            switch (cellOnBoard.Figure.Type)
                            {
                                case FigureType.Pawn:
                                    Image blackPawn = new Bitmap(75, 75);
                                    Graphics graphics1 = Graphics.FromImage(blackPawn);
                                    graphics1.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 5, 225, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = blackPawn;
                                    break;

                                case FigureType.Knight:
                                    Image blackKnight = new Bitmap(75, 75);
                                    Graphics graphics2 = Graphics.FromImage(blackKnight);
                                    graphics2.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 3, 225, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = blackKnight;
                                    break;

                                case FigureType.EliteKnight:
                                    Image BlackEliteKnight = new Bitmap(75, 75);
                                    Graphics graphics5 = Graphics.FromImage(BlackEliteKnight);
                                    graphics5.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 4, 225, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = BlackEliteKnight;
                                    break;

                                case FigureType.Megaknight:
                                    Image blackMegaknight = new Bitmap(75, 75);
                                    Graphics graphics3 = Graphics.FromImage(blackMegaknight);
                                    graphics3.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * 1, 225, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = blackMegaknight;
                                    break;

                                case FigureType.King:
                                    Image blackKing = new Bitmap(75, 75);
                                    Graphics graphics4 = Graphics.FromImage(blackKing);
                                    graphics4.DrawImage(ChessSprites, new Rectangle(0, 0, 75, 75),
                                        0 + 225 * ForKingSkinBlack, 225, 225, 225, GraphicsUnit.Pixel);
                                    CellButtons[x, y].BackgroundImage = blackKing;
                                    break;
                            }
                            break;
                    }
                    break;
            }
            if (!CurrentGame.MyBoard.Cells[x,y].LegalNextMove)
            {
                if ((x + y) % 2 == 0)
                    CellButtons[x, y].BackColor = Color.White;
                else CellButtons[x, y].BackColor = Color.Gray;
            }
        }

        public void ClickCellButton(object sender, EventArgs args)
        {
            Button pressedButton = sender as Button;
            CurrentGame.ClickCell(pressedButton, SetupPreviousCellButton, InitUpdate);
        }
        
        public void SetupPreviousCellButton(Button pressedButton)
        {
            PreviousCellButton = pressedButton;
        }

        public void InitUpdate()
        {
            UpdateGameStatus();
            RemakeBoardView();
            UpdateTurnText();
        }

        public void InitSkinsControls(object sender, EventArgs args)
        {
            if (!IsButtonsShowed)
            {
                Controls.Add(SkinClassicBut);
                Controls.Add(SkinMemesBut);
                Controls.Add(SkinFruitsBut);
                SkinClassicBut.Click += new EventHandler(PeakClassicSkin);
                SkinMemesBut.Click += new EventHandler(PeakMemesSkin);
                SkinFruitsBut.Click += new EventHandler(PeakFruitsSkin);
                IsButtonsShowed = true;
            }
            else
            {
                Controls.Remove(SkinFruitsBut);
                Thread.Sleep(50);
                Controls.Remove(SkinMemesBut);
                Thread.Sleep(50);
                Controls.Remove(SkinClassicBut);
                IsButtonsShowed = false;
            }
        }

        public void AddButtonsPictures()
        {
            Image skinClassicBack = new Bitmap(74, 74);
            var classicGraphics = Graphics.FromImage(skinClassicBack);
            classicGraphics.DrawImage(ClassicChessSprites, new Rectangle(0, 0, 74, 74),
                0, 0, 225, 225, GraphicsUnit.Pixel);
            SkinClassicBut.BackgroundImage = skinClassicBack;
            Image skinMemesBack = new Bitmap(74, 74);
            var memesGraphics = Graphics.FromImage(skinMemesBack);
            memesGraphics.DrawImage(MemesChessSprites, new Rectangle(0, 0, 74, 74),
                0, 0, 225, 225, GraphicsUnit.Pixel);
            SkinMemesBut.BackgroundImage = skinMemesBack;
            Image skinFruitsBack = new Bitmap(74, 74);
            var fruitsGraphics = Graphics.FromImage(skinFruitsBack);
            fruitsGraphics.DrawImage(FruitsChessSprites, new Rectangle(0, 0, 74, 74),
                0, 0, 225, 225, GraphicsUnit.Pixel);
            SkinFruitsBut.BackgroundImage = skinFruitsBack;
        }

        public void PeakClassicSkin(object sender, EventArgs args)
        {
            SkinClassicBut = sender as Button;
            ChessSprites = ClassicChessSprites;
            InitUpdate();
        }

        public void PeakMemesSkin(object sender, EventArgs args)
        {
            SkinMemesBut = sender as Button;
            ChessSprites = MemesChessSprites;
            InitUpdate();
        }

        public void PeakFruitsSkin(object sender, EventArgs args)
        {
            SkinFruitsBut = sender as Button;
            ChessSprites = FruitsChessSprites;
            InitUpdate();
        }

        public void MakeBackBoardState(object sender, EventArgs args)
        {
            PreviousCellButton = null;
            CurrentGame.BackTurn();
            InitUpdate();
        }

        public void MakeNextBoardState(object sender, EventArgs args)
        {
            PreviousCellButton = null;
            CurrentGame.NextTurn();
            InitUpdate();
        }

        public void UpdateTurnText()
        {
            Turn.Text = "текущий ход: " + (CurrentGame.CurrentTurn + 1).ToString();
        }

        public void UpdateGameStatus()
        {
            Controls.Remove(Draw);
            Controls.Remove(WhiteCheck);
            Controls.Remove(WhiteMate);
            Controls.Remove(BlackCheck);
            Controls.Remove(BlackMate);
            ForKingSkinWhite = 0;
            ForKingSkinBlack = 0;
            if (CurrentGame.BlackStatus == StatusType.Mate)
            {
                Controls.Add(BlackMate);
                if (ChessSprites != ClassicChessSprites)
                    ForKingSkinBlack = 2;
                return;
            }
            if (CurrentGame.WhiteStatus == StatusType.Mate)
            {
                Controls.Add(WhiteMate);
                if (ChessSprites != ClassicChessSprites)
                    ForKingSkinWhite = 2;
                return;
            }
            if (CurrentGame.WhiteStatus == StatusType.Check)
            {
                Controls.Add(WhiteCheck);
                if (ChessSprites != ClassicChessSprites)
                    ForKingSkinWhite = 2;
                return;
            }
            if (CurrentGame.BlackStatus == StatusType.Check)
            {
                Controls.Add(BlackCheck);
                if (ChessSprites != ClassicChessSprites)
                    ForKingSkinBlack = 2;
                return;
            }
            if ((CurrentGame.WhiteStatus == StatusType.Stalemate && CurrentGame.CurrentTurn % 2 == 0) || 
                (CurrentGame.BlackStatus == StatusType.Stalemate && CurrentGame.CurrentTurn % 2 == 1))
            {
                Controls.Add(Draw);
                return;
            }
        }

        public void RestartGame(object sender, EventArgs args)
        {
            Restart = sender as yt_Button;
            var result = MessageBox.Show("Вы хотите начать новую игру?", 
                                        "Предупреждение", 
                                        MessageBoxButtons.OKCancel,
                                        MessageBoxIcon.Question);
            if (result == DialogResult.Cancel) return;
            Controls.Clear();
            CurrentGame.ClearGame();
            StartInit();
            InitUpdate();
            Controls.Add(Restart);
            UpdateTurnText();
            Controls.Add(Turn);
            Controls.Add(BackTurn);
            Controls.Add(NextTurn);
            Controls.Add(DocumentationBut);
            DocumentationBut.Click += new EventHandler(OpenDocumentation);
            Restart.Click += new EventHandler(RestartGame);
            BackTurn.Click += new EventHandler(MakeBackBoardState);
            NextTurn.Click += new EventHandler(MakeNextBoardState);
        }
        
        public void ChooseMode(object sender, EventArgs args)
        {
            Start = sender as yt_Button;
            StartDefaultGame.Location = Start.Location;
            StartDefaultGame.Size = new Size(Start.Size.Width, Start.Size.Height / 3);
            StartKnightMadnessGame.Location = new Point(Start.Location.X, 
                                                        Start.Location.Y + Start.Size.Height / 3);
            StartKnightMadnessGame.Size = new Size(Start.Size.Width, Start.Size.Height / 3);
            StartKnightInfectionGame.Location = new Point(Start.Location.X,
                                                        Start.Location.Y + Start.Size.Height / 3 * 2);
            StartKnightInfectionGame.Size = new Size(Start.Size.Width, Start.Size.Height / 3);
            Controls.Add(StartDefaultGame);
            Controls.Add(StartKnightMadnessGame);
            Controls.Add(StartKnightInfectionGame);
            Controls.Remove(Start);
            StartDefaultGame.Click += new EventHandler(SetDefaultMode);
            StartKnightMadnessGame.Click += new EventHandler(SetKnightMadnessMode);
            StartKnightInfectionGame.Click += new EventHandler(SetKnightIndectionMode);
        }

        public void StartGame()
        {
            WindowState = FormWindowState.Normal;
            Controls.Remove(StartDefaultGame);
            Controls.Remove(StartKnightMadnessGame);
            Width = 75 * BoardSize + 400;
            Height = 75 * BoardSize + 50;
            Location = new Point((Screen.PrimaryScreen.Bounds.Size.Width - Width) / 2, 
                                 (Screen.PrimaryScreen.Bounds.Size.Height - Height) / 2);
            MaximumSize = Size;
            Image background = new Bitmap(Width, Height);
            var backgroundGraphics = Graphics.FromImage(background);
            backgroundGraphics.DrawImage(MainChessBack, new Rectangle(0, 0, Width, Height),
                0, 0, 1332, 850, GraphicsUnit.Pixel);
            BackgroundImage = background;
            StartInit();
            Controls.Add(Restart);
            Turn.Text += (CurrentGame.CurrentTurn + 1).ToString();
            Controls.Add(Turn);
            Controls.Add(BackTurn);
            Controls.Add(NextTurn);
            Controls.Add(DocumentationBut);
            DocumentationBut.Click += new EventHandler(OpenDocumentation);
            Restart.Click += new EventHandler(RestartGame);
            BackTurn.Click += new EventHandler(MakeBackBoardState);
            NextTurn.Click += new EventHandler(MakeNextBoardState);
        }

        public void OpenDocumentation(object sender, EventArgs args)
        {
            Process.Start(new ProcessStartInfo("https://docs.google.com/document/d/1Y_I9dUurop5cnG7aJSnnt6jyhnhK1um9wzkz___aEQ0/edit") { UseShellExecute = true });
        }

        public void CloseSteps()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if ((i + j) % 2 == 0)
                        CellButtons[i, j].BackColor = Color.White;
                    else CellButtons[i, j].BackColor = Color.Gray;
                }
            }
        }

        public void ShowLegalMoves()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (CurrentGame.MyBoard.Cells[i, j] == CurrentGame.CurrentChoosenCell) continue;
                    if (CurrentGame.MyBoard.Cells[i,j].LegalNextMove) CellButtons[i, j].BackColor = Color.Green;
                }
            }
        }
    }
}