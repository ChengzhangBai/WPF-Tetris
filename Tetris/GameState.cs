using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;
        public int Score { get; private set; }  
        public Block CurrentBlock 
        {
            get => currentBlock;
            private set 
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i=0;i<2;i++)
                {
                    currentBlock.Move(1, 0);
                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }

                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public bool CanHold { get; private set; }
        public Block HeldBlock { get; private set; }
        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }
        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }
            if (HeldBlock==null)
            {
                HeldBlock = CurrentBlock;
                
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }
            CanHold = false;
        }
        private bool BlockFits() //check if current block is in legal position
        {
            foreach(Position p in CurrentBlock.TilePositions())
            { 
            if (!GameGrid.IsEmpty(p.Row,p.Column)) //some cell has been occupied by other blocks
                {
                    return false;
                }
            }
            return true;
        }

        public void RotateBlockCW()
        {
            currentBlock.RotateCW();
            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }
        public void RotateBlockCCW()
        {
            currentBlock.RotateCCW();
            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0,1);
            }
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }
        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach(Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id; //from 0 to Id, 0->empty
            }
            Score += GameGrid.ClearFullRows();
            if (IsGameOver())
            {
                GameOver = true;
            }
            else 
            {
                CurrentBlock = BlockQueue.GetAndUpdate(); //clear full row, and get new block
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1,0);
            if (!BlockFits())  //if the position is not empty
            {
                CurrentBlock.Move(-1,0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position p)  //one block contains 4 tiles
        {
            int drop = 0;
            while (GameGrid.IsEmpty(p.Row + drop +1, p.Column))
            {
                drop++;
            }

            return drop;

        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;
            foreach (Position p in CurrentBlock.TilePositions()) 
            {
            drop = System.Math.Min(drop, TileDropDistance(p));
            }
            return drop;
        }

        public void DropBlock() 
        {
            CurrentBlock.Move(BlockDropDistance(),0);
            PlaceBlock();
        }
    }
}
