using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopClient
{
    class AItree
    {
        public int[,] map;
        public List<AItree> tree;
        public int level = 0;
        public int score = 0;
        public Point position;

        public AItree(int[,] actualMap):this(actualMap,new Point(0,0),0)
        {
            
        }

        public AItree(int[,] actualMap, Point p, int level)
        {
            generateMe(actualMap, p, level);
        }

        private void generateMe(int[,] actualMap, Point p, int level)
        {
            this.level = level;
            this.score = 0;
            this.position = p;
            this.tree = new List<AItree>();
            this.map = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.map[i, j] = actualMap[i, j];
                }
            }
            if (GameEngine.GetGameState(this.map) == GameState.ONGOING)
            {

                for (int row = 0; row < 3; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        if (actualMap[row, column] == 0)
                        {

                            int[,] map = new int[3, 3];
                            for (int i = 0; i < 3; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    map[i, j] = this.map[i, j];
                                }
                            }

                            map[row, column] = 2 - (level % 2);
                            tree.Add(new AItree(map, new Point(row, column), this.level + 1));
                        }
                    }
                }
            }
            this.score = GetScore();
        }

        /// <summary>
        /// Vypocet skore sa odvija od finalneho stavu hry a urovne v ktorej je dosiahnuty
        /// </summary>
        /// <returns>Skore pre danu cast stromu</returns>
        private int GetScore()
        {
            int myScore = 0;
            //ak si posledny v zozname, vyrataj svoje skore, inak si zober najvyssie skore svojich listov
            if (tree.Count == 0)
            {
                myScore = GameEngine.GetGameState(this.map) == GameState.WIN_COMPUTER ? 10 : GameEngine.GetGameState(this.map) == GameState.WIN_PLAYER ? -10 : 0;
                myScore = myScore * 10 * (9 - level);
            }
            else
            {
                foreach (AItree leaf in tree)
                {
                    if (Math.Abs(leaf.score) > Math.Abs(myScore))
                        myScore = leaf.score;
                }

            }
            return myScore;
        }
    }
}
