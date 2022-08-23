using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FinalProjectClient
{
    public enum SOLDIER
    {
      BLACK=0,BLACK_KING=1,WHITE=2, WHITE_KING=3, NOTHING=4
    }
    enum WhoIsPlaying
    {
        PLAYER, SERVER
    }

    enum GAMESTATE
    {
       BLACKWON, WHITEWON,ONGOING,DRAW
    }
    public class SoldierEatenEventArgs : EventArgs
    {
        public int rowEaten;
        public int columnEaten;

        public SoldierEatenEventArgs(int rowEaten, int columnEaten)
        {
            this.rowEaten = rowEaten;
            this.columnEaten = columnEaten;
        }

    }


    public class SoldierMovedEventArgs : EventArgs
    {
        public Coordinate Fromcell;
        public Coordinate ToCell;

        public SoldierMovedEventArgs(Coordinate src, Coordinate dest)
        {
            this.Fromcell = src;
            this.ToCell = dest;
        }

    }
    class GameBoard
    {
        public SOLDIER[,] matrix;
        public EventHandler<SoldierEatenEventArgs> soldierEatenEvent;
        public EventHandler<SoldierMovedEventArgs> soldierMovedEvent;
        public EventHandler<GAMESTATE> gameOverEvent;
        public EventHandler<WhoIsPlaying> changedTurnEvent;
        private HttpClient client = new HttpClient();



        private WhoIsPlaying turn;

        public GameBoard()
        {
            matrix = new SOLDIER[8, 8];
            initStartingBoard();
            //the player always starts first
            turn = WhoIsPlaying.PLAYER;
            client.BaseAddress = new Uri("https://localhost:44332/");

        }
        //initializing where each group stands on the board
        private void initStartingBoard()
        {
            SOLDIER soldier = SOLDIER.NOTHING;
            for (int i = 0; i < 8; i++)
            {
                if (i <= 2)
                {
                    soldier = SOLDIER.BLACK;
                }
                else if (i >= 5)
                {
                    soldier = SOLDIER.WHITE;

                }
                else
                {
                    soldier = SOLDIER.NOTHING;
                }
                for (int j = 0; j < 8; j++)
                {
                    //if player matrix[i][j] has player we draw the player
                    matrix[i, j] = SOLDIER.NOTHING;
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            matrix[i, j] = soldier;
                        }
                    }
                    else
                    {
                        if (j % 2 != 0)
                        {
                            matrix[i, j] = soldier;
                        }

                    }

                }

            }
        }

        //gets a soldier and a soldier that in it's way, and answers the question "can I eat this?"
        public bool canSoldierBeEaten(SOLDIER eater, SOLDIER food)
        {
            if (eater == SOLDIER.BLACK_KING || eater == SOLDIER.BLACK)
            {
               if(food ==SOLDIER.WHITE || food ==SOLDIER.WHITE_KING)
                {
                    return true;
                }
            }
            else if (eater == SOLDIER.WHITE_KING || eater == SOLDIER.WHITE)
            {
                if(food == SOLDIER.BLACK || food == SOLDIER.BLACK_KING)
                {
                    return true;
                }
            }
            return false;
        }


        public List<Coordinate> soldierOptionsToMove(Coordinate point)
        {
            Console.WriteLine($"Calculating optional moves, {point}");
            List<Coordinate> list = new List<Coordinate>();
            
            if (getCell(point) == SOLDIER.NOTHING)
            {
                return list;
            }
            //which soldiers can move downwards
            if (getCell(point) == SOLDIER.BLACK || getCell(point) == SOLDIER.BLACK_KING || getCell(point) == SOLDIER.WHITE_KING)
            {
                //assuming i move the black soldier
                //first case:
                Coordinate leftDiagonal = new Coordinate(point.row + 1, point.column - 1);

                if (!isOutOfMatrix(leftDiagonal) && getCell(leftDiagonal) == SOLDIER.NOTHING)
                {
                    list.Add(leftDiagonal);
                }
                Coordinate rightDiagonal = new Coordinate(point.row + 1, point.column + 1);

                if (!isOutOfMatrix(rightDiagonal) && getCell(rightDiagonal) == SOLDIER.NOTHING)
                {
                    list.Add(rightDiagonal);
                }
                //second case:
                Coordinate leftDiagonalx2 = new Coordinate(point.row + 2, point.column - 2);
                if (!isOutOfMatrix(leftDiagonalx2) && !isOutOfMatrix(leftDiagonal) &&
                    getCell(leftDiagonalx2) == SOLDIER.NOTHING &&  canSoldierBeEaten(getCell(point),getCell(leftDiagonal)))
                {
                    list.Add(leftDiagonalx2);
                }
                //third case:
                Coordinate rightDiagonalx2 = new Coordinate(point.row + 2, point.column + 2);
                if (!isOutOfMatrix(rightDiagonalx2) && !isOutOfMatrix(rightDiagonal) && 
                    getCell(rightDiagonalx2) == SOLDIER.NOTHING && canSoldierBeEaten(getCell(point), getCell(rightDiagonal)))
                {
                    list.Add(rightDiagonalx2);
                }
            }
            //which soldiers can move upward
            if(getCell(point) == SOLDIER.WHITE || getCell(point) == SOLDIER.WHITE_KING || getCell(point) == SOLDIER.BLACK_KING)
            {
                //first case:
                Coordinate leftDiagonal = new Coordinate(point.row - 1, point.column - 1);

                if (!isOutOfMatrix(leftDiagonal) && getCell(leftDiagonal) == SOLDIER.NOTHING)
                {
                    list.Add(leftDiagonal);
                }
                Coordinate rightDiagonal = new Coordinate(point.row - 1, point.column + 1);

                if (!isOutOfMatrix(rightDiagonal) && getCell(rightDiagonal) == SOLDIER.NOTHING)
                {
                    list.Add(rightDiagonal);
                }
                //second case:
                Coordinate leftDiagonalx2 = new Coordinate(point.row - 2, point.column - 2);
                if (!isOutOfMatrix(leftDiagonalx2) && !isOutOfMatrix(leftDiagonal) &&
                    getCell(leftDiagonalx2) == SOLDIER.NOTHING && canSoldierBeEaten(getCell(point), getCell(leftDiagonal)))
                {
                    list.Add(leftDiagonalx2);
                }
                //third case:
                Coordinate rightDiagonalx2 = new Coordinate(point.row - 2, point.column + 2);
                if (!isOutOfMatrix(rightDiagonalx2) && !isOutOfMatrix(rightDiagonal) &&
                    getCell(rightDiagonalx2) == SOLDIER.NOTHING && canSoldierBeEaten(getCell(point), getCell(rightDiagonal)))
                {
                    list.Add(rightDiagonalx2);
                }

            }
            Console.WriteLine($"Optional moves: {string.Join(", ", list)}");
            //fourth case:
            //included in cases 3 and 4
            return list;

        }
        //here e check that we are in the right baundaries

        private bool isOutOfMatrix(Coordinate cell)
        {
            try
            {
                getCell(cell);

            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }

            return false;
        }

        //this function will check 2 cases : 
        //1. Who's turn is
        //2.move soldier
        public bool doTurn(Coordinate src, Coordinate dest)
        {
            Console.WriteLine($" ============ Starting doTurn {turn}: {src}, ->{dest}");
            if (isSoldiersTurn(getCell(src)))
            {
                
                //it is the current player's turn and the move is valid 
                if (moveSoldier(src, dest))
                {
                    //if the turn successful, ie :the palyer moves successfully
                    //the turn passes to the other player
                    swap();
                    if (gameOverEvent != null)
                    {
                        GAMESTATE state = getGameState();
                        Console.WriteLine($"The GAMESTATE after the turn is:{state} ");
                        if (!(state == GAMESTATE.ONGOING))
                        {
                            gameOverEvent?.Invoke(this, state);
                        }
                    }

                    //the move is valid
                    Console.WriteLine($"Ending doTurn soldier with true: {src}, ->{dest}");
                    return true;

                }
                else
                {
                    Console.WriteLine($"invalid move");
                }

            }
            else
            {
                Console.WriteLine($"NOT your turn, its {this.turn}");
            }
            
            Console.WriteLine($"Ending doTurn soldier with FALSE");
            return false;

        }

        private void swap()

        {
           
             Console.WriteLine($"Starting swapping soldier");

            if (turn == WhoIsPlaying.PLAYER)
            {
                turn = WhoIsPlaying.SERVER;
            }
            else
            {
                turn = WhoIsPlaying.PLAYER;

            }
            changedTurnEvent?.Invoke(this, turn);
            Console.WriteLine($"Ending swapping soldier");

        }
        


        //this function determines whether the soldier belongs to player or the server  
        //and IF it is their turn
        public bool isSoldiersTurn(SOLDIER color)
        {

            if (turn == WhoIsPlaying.SERVER)
            {
                if ((color == SOLDIER.BLACK) || (color == SOLDIER.BLACK_KING))
                {
                    return true;
                }

            }
            else if (turn == WhoIsPlaying.PLAYER)
            {

                if ((color == SOLDIER.WHITE) || (color == SOLDIER.WHITE_KING))
                {
                    return true;
                }
           
            }

            return false;
        }

        //as long as the move of a soldier is valid ,this function will update the news locations of the
        // destination in the matrix and the source
        private bool moveSoldier(Coordinate src, Coordinate dest)
        {
            Console.WriteLine($"Starting moving soldier: {src}, ->{dest}");


            if (!isMoveValid(src, dest))
            {
                return false;
            }

            //if the dest is row 8 and dest.col
            //are we moving to row 7? if we do and we are in black color then we become king
            if (dest.row == 7 && matrix[src.row, src.column] == SOLDIER.BLACK)
            {
                    matrix[dest.row, dest.column] = SOLDIER.BLACK_KING;
  
            }
            else if (dest.row == 0 && matrix[src.row, src.column] == SOLDIER.WHITE)
            {

                matrix[dest.row, dest.column] = SOLDIER.WHITE_KING;

            }

            else
            {
                matrix[dest.row, dest.column] = matrix[src.row, src.column];
            }
            matrix[src.row, src.column] = SOLDIER.NOTHING;

            //eat the soldier that's in our path if there is one
            eatSoldier(src, dest);
            this.soldierMovedEvent.Invoke(this, new SoldierMovedEventArgs(src, dest));

            Console.WriteLine($"End moving soldier: {src}, ->{dest}");

            return true;
        }
        //eat the soldier that's in our path if there is one
        //this function will eat (delete) the eaten soldier and update the relevant coordinates
        public void eatSoldier(Coordinate src, Coordinate dest)
        {
            int rowEaten = (src.row + dest.row) / 2;
            int columnEaten = (src.column + dest.column) / 2;

            if (Math.Abs(src.row - dest.row) == 2)
            {
             //   Console.WriteLine("=================================================");

          //      Console.WriteLine($"Calculated eaten cell{rowEaten},{columnEaten}");
        //        Console.WriteLine("=================================================");

                matrix[rowEaten, columnEaten] = SOLDIER.NOTHING;
                this.soldierEatenEvent.Invoke(this, new SoldierEatenEventArgs(rowEaten, columnEaten));
                Console.WriteLine($"End eating soldier: {src}, ->{dest}");

            }

        }

        //this function returns a coordinate of a soldier that will be eaten
        //if enemy soldier took the move from src to dest
        //this function doesn't move anything or delete anything
        //it is only a speculation
        public Coordinate CheckeatSoldier(Coordinate src, Coordinate dest)
        {
            int rowEaten = (src.row + dest.row) / 2;
            int columnEaten = (src.column + dest.column) / 2;

            if (Math.Abs(src.row - dest.row) == 2)
            {

                return new Coordinate(rowEaten, columnEaten);

            }
            return null;
        }

        //was created coordinate that represent the middle move of a soldier that didn't eat and only moved
       public class MoveCords
            {
            public Coordinate src;
            public Coordinate dst;

            public MoveCords(Coordinate src, Coordinate dst)
            {
                this.src = src;
                this.dst = dst;
            }
        }

        public  async void PostRandomMoveAsync()
        {
            //int rand =0;
            //  var annonymous = new { name = color };
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var oneDimArray = getMatrixAsOneDArray();
            List<int> solInt = oneDimArray.Select(a => ((int)a)).ToList();
            var json = JsonConvert.SerializeObject(solInt, jsonSerializerSettings);
            HttpResponseMessage response = await client.PostAsJsonAsync("api/gameInfo/randomMove", json);

            if (response.IsSuccessStatusCode)
            {
                //       rand = await response.Content.ReadAsAsync<string>();
                var content = await response.Content.ReadAsStringAsync();
                var serverMove = JsonConvert.DeserializeObject<Dictionary<string, Tuple<int, int>>>(content);
                if (serverMove.ContainsKey("src"))
                {
                Coordinate src = new Coordinate(serverMove["src"].Item1, serverMove["src"].Item2);
                Coordinate dst = new Coordinate(serverMove["dst"].Item1, serverMove["dst"].Item2);
                doTurn(src, dst);
                }//server send an empty move!!
              

            }
          
        }




        //This function will allow the server move randomly
        public bool getRandomMove(List<SOLDIER> color)
        {
    //        Console.WriteLine($"Starting Random move of soldier:{color}");

            
            // WARNING: if changed from 8x8 need to change this code
            List<int> listOfIndexRows = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            List<int> listOfIndexColumn = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            //randomize the list of rows
            var rnd = new Random();
            var randomizedRows = listOfIndexRows.OrderBy(item => rnd.Next());
            //randomize the list of columns
            rnd = new Random();
            var randomizedCols = listOfIndexColumn.OrderBy(item => rnd.Next());

            List<MoveCords> OptionalOfMoves = new List<MoveCords>();

            foreach (int i in randomizedRows)
            {
                foreach(int j in randomizedCols)
                {
                    //we got a random cell in the matrix
                    if(color.Contains(matrix[i,j]))
                    {
                        var src = new Coordinate(i, j);
                        var optionLst = soldierOptionsToMove(src);
                        
                        //for every optional destination, 
                        foreach (Coordinate dst in optionLst)
                        {
                            //we check , does this move eat a soldier
                            //if eaten is no equal to null
                            //then a soldier was eaten
                            Coordinate eaten = CheckeatSoldier(src, dst);

                            // we will exit this function if we found 1 move that eats a soldier
                            if (eaten != null)
                            {
                                // this move will eat a soldier
                                doTurn(src, dst);
                                Console.WriteLine($"Ending Random move of soldier when true{color}");
                                return true;
                            }
                            // this move didnt eat anything, but we keep it for later, incase we didnt find ANY moves that do eat
                            // we still need to do a move
                            OptionalOfMoves.Add(new MoveCords(src, dst));
                        }
                    }
                }
            }

            // we didnt find a move that resulted in eating an enemy
            // so we take a optional move that doesnt eat, but is still a legal move
            if (OptionalOfMoves.Count != 0)
            {
                // there is an optional move
                // we take a random move from the list
                int randomIndex = rnd.Next(OptionalOfMoves.Count);
                var randomMove = OptionalOfMoves[randomIndex];
                doTurn(randomMove.src, randomMove.dst);
       //        Console.WriteLine($"Ending Random move of soldier when true{color}");
                return true;
            }

            //went over every cell of the matrix none of them had any moves. game over
     //       Console.WriteLine($"Ending Random move of soldier when return false no moves:{color}");

            return false;
        }



        public GAMESTATE getGameState()

            //Draw
        {
            if(!canSoldierMove(SOLDIER.BLACK) && !canSoldierMove(SOLDIER.WHITE)&&
                !canSoldierMove(SOLDIER.BLACK_KING) && !canSoldierMove(SOLDIER.WHITE_KING))
            {
                return GAMESTATE.DRAW;
            }
            //Black wins, the black soldier can move and the white can't
            if (!canSoldierMove(SOLDIER.WHITE) && !canSoldierMove(SOLDIER.WHITE_KING)&&
                (canSoldierMove(SOLDIER.BLACK) || canSoldierMove(SOLDIER.BLACK_KING)))
            {
                
                    return GAMESTATE.BLACKWON;
                
            }
            //the white soldier can move and the black can't, the white wins
            else if (!canSoldierMove(SOLDIER.BLACK) && !canSoldierMove(SOLDIER.BLACK_KING) &&
                (canSoldierMove(SOLDIER.WHITE) || canSoldierMove(SOLDIER.WHITE_KING)))
            {
                return GAMESTATE.WHITEWON;
            }
            //In the above code i checked all the options for the game state 
            //except for this one so it must be the case
            return GAMESTATE.ONGOING;
        }

        public bool canSoldierMove(SOLDIER color)
        {
            for (int i=0; i <matrix.GetLength(0); i++ )
            {
                for (int j=0;j<matrix.GetLength(1) ; j++)
                {
                    //We go over each soldier with *color*

                    if (matrix[i, j] == color)
                    {
                        //if there is a move(at least) then
                        //the the game is not over yet.
                        var src = new Coordinate(i, j);
                        var optionLst = soldierOptionsToMove(src);
                        if (optionLst.Count != 0)
                        {
                            return true;
                        }
                        //if there are no more options to move for this current soldier
                        //then we continue to next soldier and check for him

                    }
                }
            }

            //Game is over because none of the soldiers has any option to move
            return false;
        }
        public List<SOLDIER> getMatrixAsOneDArray()
        {
            List<SOLDIER> result = new List<SOLDIER>();
            for(int i =0; i<matrix.GetLength(0); i++) 
            {
                for (int j=0; j<matrix.GetLength(1); j++)
                {
                    result.Add(matrix[i, j]);
                
                }
            
            }
            return result;
        }
        
        public SOLDIER[,] getMatrixFrom1DArray(List<SOLDIER> oneDimArray, int rowNumber,int colNumber)
        {
            SOLDIER[,] resultMatrix = new SOLDIER[rowNumber,colNumber];
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < colNumber; j++)
                {
                    resultMatrix[i, j] = oneDimArray[i * colNumber + j];

                }

            }
            return resultMatrix;
        }
        private bool isMoveValid(Coordinate src, Coordinate dest)
        {

            List<Coordinate> lst = soldierOptionsToMove(src);


            return lst.Exists(c => (c.row == dest.row && c.column == dest.column));
        }

       public SOLDIER getCell(int row, int col)
        {
            return matrix[row, col];
        }

        public SOLDIER getCell(Coordinate c)
        {
            return matrix[c.row, c.column];
        }

    }
}
