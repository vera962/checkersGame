using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using FinalProjectClient.Models;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Timers;

namespace FinalProjectClient
{
    public partial class Form1 : Form
    {
        private int gameid;

        // for replay
        private int replayGameId;
        private int replayCurStepNum;


        public string UserName { get; set; }
        public int UserId { get; set; }
        public DateTime dateStartGame { get; set; }
        public int GameLength { get; set; }
        private HttpClient client = new HttpClient();
        private GameBoard game;
        private List<Coordinate> currOpMoves = new List<Coordinate>();
        private Coordinate currSoldier=null;
        private GAMEDBDataContext db = new GAMEDBDataContext();

        private bool IsReplay;
        private EventHandler closeFormEvent;
        private List<TblPo> steps;
        private System.Timers.Timer replayTimer;
        private double replayDelayStepSec = 2;
        //only one thread can change the images at the same time
        private static object _imageLock = new object();

        public Form1(int replayGameId)
        {
            InitializeComponent();

            this.IsReplay = true;
            this.replayGameId = replayGameId;
            this.replayCurStepNum = 0;
        }

        public Form1()
        {
            InitializeComponent();

            this.IsReplay = false;
        }

        //initialization of the graphic board
        private void Form1_Load(object sender, EventArgs e)
        {

            game = new GameBoard();
            boardCreation();
            drawTable();
            // listeners that update the view
            game.soldierEatenEvent += soldirEatenListner;
            // this one optionally adds the move to db (if its not a replay) 
            game.soldierMovedEvent += soldirMovedListner;

            game.gameOverEvent += gameOverListner;


            showWhosPlaying();

            pbrobot.Image = Properties.Resources.retro_robot_pacing_success_300_wht;
            pbrobot.SizeMode = PictureBoxSizeMode.Zoom;

            if (!this.IsReplay)
            {
                // LoginForm form = new LoginForm();
                client.BaseAddress = new Uri("https://localhost:44332/");
                dateStartGame = DateTime.Now;

                addGameInfoToDb();

                
                game.changedTurnEvent += changedTurnListner;

            }
            //We are in a replay!
            else
            {
                this.btnSurrender.Enabled = false;
                // make game board (datagrid) unclickable !!!!! remove click listener
                // in the replay there is no interaction by the user allowed
                this.dgvBoard.CellContentClick -= this.dgvBoard_CellContentClick;

                // get steps from db 

                //steps = db.TblPos.Where(x => (x.GameId == replayGameId) && x.stepOrder == replayCurStepNum).ToList();
                steps = db.TblPos.Where(x => x.GameId == this.replayGameId).OrderBy(x=>x.stepOrder).ToList();
                if (!steps.Any())
                {

                    endReplay();
                    return;

                }

                // creating a timer, every tick we do a move from the replay
                replayTimer = new System.Timers.Timer();
                replayTimer.Interval = replayDelayStepSec * 1000;
                replayTimer.Elapsed += replayDoStep;            

                replayTimer.Start();

                
            }


        }

        private void endReplay()
        {
            replayTimer.Stop();
            replayTimer.Dispose();
            DialogResult = MessageBox.Show("no more recordeed steps were found", "Replay over");

            //closeFormEvent.Invoke("helo", new EventArgs());
            CloseWindow();
        }

        private void replayDoStep(object sender, ElapsedEventArgs e)
        {
            // to prevent tick overlaps
            replayTimer.Enabled = false;


            if (steps.Count <= replayCurStepNum)
            {
                endReplay();
                return;
            }
            TblPo step = steps[replayCurStepNum];

            var src = new Coordinate(step.srcPosX, step.srcPosY);
            var dest = new Coordinate(step.destPosX, step.destPosY);

            this.game.doTurn(src, dest);                     

            replayCurStepNum++;
            


            // to prevent tick overlaps
            replayTimer.Enabled = true;
        }


        async Task<Uri> CreateGameAsync(Games game)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/gameInfo", game);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }


        //adding all the images to the dataGridView
        private void boardCreation()
        {
            for (int i = 0; i < 8; i++)
            {
                DataGridViewImageColumn imageCol = new DataGridViewImageColumn();
                dgvBoard.Columns.Add(imageCol);
                dgvBoard.Rows.Add();

            }

            dgvBoard.ColumnHeadersVisible = false;
            dgvBoard.RowHeadersVisible = false;

            dgvBoard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            dgvBoard.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
        }

        //Draw initial table according to the matrix in game board
        private void drawTable()
        {
            for (int i = 0; i < 8; i++)
            {


                for (int j = 0; j < 8; j++)
                {

                    setDataGridCellImage(new Coordinate(i, j));
                }

            }
        }

        //Showing the optionals paths the current player can move to.

        private void clearDisplayMoves()
        {
            foreach (Coordinate x in currOpMoves)
            {
                Console.WriteLine($"Clear Move From Board {x}");
                //Reseting the old optional move
                //Updating cell images
                setDataGridCellImage(x);
            }
            currOpMoves.Clear();
        }
        private void displayOptionalMoves(Coordinate c)
        {
            clearDisplayMoves();
            //Adding the new options to move to on the board
            currOpMoves = game.soldierOptionsToMove(c);

            foreach (Coordinate x in currOpMoves)
            {
                dgvBoard.Rows[x.row].Cells[x.column].Value = Properties.Resources.op2chosen;

            }

        }

        //Suppose to ,ake the size of the gridview permanent without option to change
        void sizeDGV(DataGridView dgv)
        {
            DataGridViewElementStates states = DataGridViewElementStates.None;
            dgv.ScrollBars = ScrollBars.None;
            var totalHeight = dgv.Rows.GetRowsHeight(states); //+ dgv.ColumnHeadersHeight;
           // totalHeight += dgv.Rows.Count * 4;  // a correction I need
            var totalWidth = dgv.Columns.GetColumnsWidth(states); //+ dgv.RowHeadersWidth;
            dgv.ClientSize = new Size(totalWidth, totalHeight);
        }


        //This function is going to listen to an event
        //The event is is going to happen when a soldier is eaten
        //The listener get this information from the event and
        //and it really preforms the updates and changes
        //i.e we can see the soldier is gone (eaten)on the visual board
        private void soldirEatenListner(object sender, SoldierEatenEventArgs e)
        {

            Console.WriteLine($"Listner eating soldier {e.rowEaten} , {e.columnEaten}");
            setDataGridCellImage(new Coordinate(e.rowEaten, e.columnEaten));
            Console.WriteLine($"eating soldier {e.rowEaten} , {e.columnEaten}");


        }


        public void CloseWindow()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(new Action(() => { this.Close(); })));
                }
                else
                {
                    this.Close();
                }
            } catch
            {

            }

        }

        private async void changedTurnListner(object sender, WhoIsPlaying e)
        {
            //in seconds
            Random rnd = new Random();
            //random number from 2-10 sec
            int serverThinkTime = 1;
            this.Invoke(new Action(() =>
            {
                showWhosPlaying();
            }));
            //if this is the server's turn we generate the random move
            if (e == WhoIsPlaying.SERVER)
            {
                this.Invoke(new Action(() =>
                {
                     pbrobot.Visible = true;
                }));
                
                //    System.Threading.Thread.Sleep(2000);
                await Task.Delay(TimeSpan.FromSeconds(serverThinkTime))
                    .ContinueWith(async task =>
                    {
               //         game.getRandomMove(solList);
                         game.PostRandomMoveAsync();
                        this.Invoke(new Action(() =>
                        {
                            pbrobot.Visible = false;
                        }));
                    });

            }

        }

        //This function is going to listen to an event
        //The event is is going to happen when a soldier is moved
        //The listener get this information from the event and
        //and it really preforms the updates and changes
        //i.e we can see the soldier is moved on the visual board
        private void soldirMovedListner(object sender, SoldierMovedEventArgs e)
        {
            setDataGridCellImage(e.Fromcell);
            setDataGridCellImage(e.ToCell);

            // save step to db IF THIS IS NOT A REPLAY
            if (!this.IsReplay)
            {
                
                var steps = db.TblPos.Where(x => x.GameId == gameid);
                // starting step num, if there are no steps
                int stepInc = 0;
                // if there are steps associated to this *gameid*
                // add this step with the last steps num +1
                if (steps.Any())
                {
                    stepInc = steps
                        .OrderByDescending(x => x.stepOrder)
                        .First().stepOrder + 1;
                }

                TblPo s = new TblPo
                {
                    GameId = gameid,
                    stepOrder = stepInc,
                    srcPosX = e.Fromcell.row,
                    srcPosY = e.Fromcell.column,
                    destPosX = e.ToCell.row,
                    destPosY = e.ToCell.column
                };

                db.TblPos.InsertOnSubmit(s);
                db.SubmitChanges();
            }

        }

        private void addGameInfoToDb()
        {
            //descending order is the biggest number that comes first 
            //1,2,3,4,5
            //descending order 5,4,3,2,1
            //the lambda tells us according to which field we order the lines
            //First represent the whole line and gameId the field we choose
            //we can do it by one (the game id)
            var listgames = db.TblGames.OrderByDescending(x => x.GameId);
            if (listgames.Any()) 
            {
                gameid = listgames.First().GameId + 1;
            }else { gameid = 0; }
            TblGame g = new TblGame { GameId = gameid, UserId = UserId, StartTime = dateStartGame, Name = UserName };
            //TblGame g = new TblGame { UserId = UserId, StartTime = dateStartGame, Name = UserName };

            db.TblGames.InsertOnSubmit(g);
            db.SubmitChanges();
        }


        private async void gameOverListner(object sender, GAMESTATE e)
        {
           // setDataGridCellImage(new Coordinate(e.rowEaten, e.columnEaten));
           // Console.WriteLine($"eating soldier {e.rowEaten} , {e.columnEaten}");
           if(e == GAMESTATE.DRAW)
            {
                MessageBox.Show("DRAW", "GAME OVER", MessageBoxButtons.OK);
                
            }
           else if (e == GAMESTATE.BLACKWON)
            {
                MessageBox.Show("BLACK WON", "GAME OVER", MessageBoxButtons.OK);
            }
           else if (e == GAMESTATE.WHITEWON)
            {
                MessageBox.Show("WHITE WON", "GAME OVER", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("ERROR!!!",$"WTF IS THAT???????????{e}", MessageBoxButtons.OK);
            }

            if (!IsReplay)
            {
                GameLength = (int)((DateTime.Now- dateStartGame).TotalSeconds);
                Games game = new Games
                {
                    Id = gameid,
                    UserId = UserId,
                    Player = UserName,
                    StartDate = dateStartGame,
                    GameLength = GameLength
                };
                await CreateGameAsync(game);

            }
        }


        //This function is called when a cell is clicked
        private void dgvBoard_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
            
            //The Content
            SOLDIER cell = game.getCell(e.RowIndex, e.ColumnIndex);
            //if we entered the click function second time, we want to check
            //if the cell we pressed is an empty cell to move on
            if (cell == SOLDIER.NOTHING)
            {
                if (currSoldier !=null)
                {
                    Coordinate dest= new Coordinate(e.RowIndex, e.ColumnIndex);
                    //if we succeeded to move the soldier we will draw him, otherwise, not. 
                    if (game.doTurn(currSoldier, dest))
                    {
                        clearDisplayMoves();
                        Console.WriteLine($"srcpoint: {currSoldier}, destpoint: {e.RowIndex},{e.ColumnIndex}");
                        currSoldier = null;
                        
                    }
                }
            }
            else if (cell == SOLDIER.BLACK)
            {

                displayOptionalMoves(new Coordinate(e.RowIndex, e.ColumnIndex));
                currSoldier = new Coordinate(e.RowIndex, e.ColumnIndex);
            }
            else if (cell == SOLDIER.BLACK_KING)
            {

                displayOptionalMoves(new Coordinate(e.RowIndex, e.ColumnIndex));
                currSoldier = new Coordinate(e.RowIndex, e.ColumnIndex);
            }
            else if (cell == SOLDIER.WHITE)
            {
                displayOptionalMoves(new Coordinate(e.RowIndex, e.ColumnIndex));
                currSoldier = new Coordinate(e.RowIndex, e.ColumnIndex);
            }
            else if (cell == SOLDIER.WHITE_KING)
            {
                displayOptionalMoves(new Coordinate(e.RowIndex, e.ColumnIndex));
                currSoldier = new Coordinate(e.RowIndex, e.ColumnIndex);
            }
            showWhosPlaying();

        }

        private void setDataGridCellImage(Coordinate cell)
        {

            // hi datagrodview, do i need premission to access you?
            if (dgvBoard.InvokeRequired)
            {
                Console.WriteLine("THiS SOLVE IT!!!!!!XXXXXXX");
                // i dont have the athority
                this.Invoke(new Action(() =>
                {
                    // gui thread, do this for me. youre the only one who can do it.
                    // the same function im in right now, with the same arguments i got
                    setDataGridCellImage(cell);
                }));
            }
            else
            {
                lock (_imageLock)

                {
                    // i do have, continue as usual
                    //The Content
                    SOLDIER cellContent = game.getCell(cell.row, cell.column);
                    //if we entered the click function second time, we want to check
                    //if the ce
                    // ll we pressed is an empty cell to move on
                    Bitmap image = null;
                    if (cellContent == SOLDIER.NOTHING)
                    {
                        Console.WriteLine($"Updating the image of {cell} to NOTHING");
                        //   dgvBoard.Rows[cell.row].Cells[cell.column].Value = Properties.Resources.op2blackbox;

                        //we draw empty cells black or white depending on their position
                        if (cell.row % 2 == 0)
                        {
                            if (cell.column % 2 == 0)
                            {
                                image = Properties.Resources.op2blackbox;
                            }
                            else
                            {
                                image = Properties.Resources.op2whitebox;
                            }
                        }
                        else
                        {
                            if (cell.column % 2 != 0)
                            {
                                image = Properties.Resources.op2blackbox;
                            }
                            else
                            {
                                image = Properties.Resources.op2whitebox;
                            }
                        }
                    }


                    else if (cellContent == SOLDIER.BLACK)
                    {
                        //    Console.WriteLine($"Updating the image of {cell} to BLACK");

                        image = Properties.Resources.op2black;

                    }
                    else if (cellContent == SOLDIER.BLACK_KING)
                    {
                        //      Console.WriteLine($"Updating the image of {cell} to BLACK_KING");
                        image = Properties.Resources.BlackKing;
                    }
                    else if (cellContent == SOLDIER.WHITE)
                    {
                        //        Console.WriteLine($"Updating the image of {cell} to WHITE");
                        image = Properties.Resources.op2white;

                    }
                    else if (cellContent == SOLDIER.WHITE_KING)
                    {
                        //         Console.WriteLine($"Updating the image of {cell} to WHITE_KING");
                        image = Properties.Resources.WhiteKing;

                    }
                    dgvBoard.Rows[cell.row].Cells[cell.column].Value = image;
                }
            }

        }

       
        private void showWhosPlaying()
        {
            //check who's turn is
            //marks the relavant label according the first case
            if (game.isSoldiersTurn(SOLDIER.WHITE))
            {
                lblServer.BackColor = Color.DarkGray;
                lblPlayer.BackColor = Color.Yellow;
            }else if (game.isSoldiersTurn(SOLDIER.BLACK))
                {
                lblPlayer.BackColor = Color.DarkGray;
                lblServer.BackColor = Color.Yellow;

            }
        }

        private void btnSurrender_Click(object sender, EventArgs e)
        {
            DialogResult yn = MessageBox.Show("Are you sure?", "wanna quit?", MessageBoxButtons.YesNoCancel);

            if(yn == DialogResult.Yes)
            {
                pbQuiter.Visible = true;
                Task.Delay(TimeSpan.FromSeconds(3))
                    .ContinueWith(async task =>
                    {
                        CloseWindow();
                    });
                
            }
        }
    }
}
