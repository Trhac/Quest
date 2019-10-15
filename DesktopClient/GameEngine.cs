using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopClient
{
    static class GameEngine
    {
        /// <summary>
        /// Metoda na detekciu konca hry
        /// </summary>
        /// <param name="gameMap">Aktualna herna mapa</param>
        /// <returns>0 = nie je koniec hry, 1 = vyhra hraca, 2 = vyhra protivnika, -1 = hracie pole je plne obsadene</returns>
        public static GameState GetGameState(int[,] gameMap)
        {
            GameState state = GameState.ONGOING;

            //kontrola horizontalne
            for (int i = 0; i < 3; i++)
            {
                if (gameMap[i, 0] > 0 && gameMap[i, 0] == gameMap[i, 1] && gameMap[i, 0] == gameMap[i, 2])
                    state = (gameMap[i, 0] == 1) ? GameState.WIN_PLAYER : GameState.WIN_COMPUTER;
            }

            //kontrola vertikalne
            for (int i = 0; i < 3; i++)
            {
                if (gameMap[0, i] > 0 && gameMap[0, i] == gameMap[1, i] && gameMap[0, i] == gameMap[2, i])
                    state = (gameMap[0, i] == 1) ? GameState.WIN_PLAYER : GameState.WIN_COMPUTER;
            }

            //kontrola diagonalne

            if (gameMap[0, 0] > 0 && gameMap[0, 0] == gameMap[1, 1] && gameMap[0, 0] == gameMap[2, 2])
                state = (gameMap[0, 0] == 1) ? GameState.WIN_PLAYER : GameState.WIN_COMPUTER;
            if (gameMap[2, 0] > 0 && gameMap[2, 0] == gameMap[1, 1] && gameMap[2, 0] == gameMap[0, 2])
                state = (gameMap[2, 0] == 1) ? GameState.WIN_PLAYER : GameState.WIN_COMPUTER;

            if (state == GameState.ONGOING) { 
                //su obsadene vsetky polia
                bool isFull = true;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (gameMap[i, j] == 0)
                            isFull = false;
                    }
                }
                if (isFull)
                    state = GameState.DRAW;
            }
            return state;
        }

        //toto by malo vracat aktualny tah + diagnosticke data(list poloziek point,score)
        public static ComputerMove GetComputerMove(int [,] gameMap)
        {
            //MINIMAX
            //v prvom kole prejst vsetky mozne tahy a ohodnotit ich
            //ak je niekde vyherny tah, pouzit ho
            //ak su niekde neutralne tahy, rozvinut ich dalej
            //vytvorit strom protivnikovych tahov
            //kazdu polozku stromu dalej ohodnotit


            //vytvorenie noveho stromu so vsetkymi tahmi podla aktualneho stavu
            AItree root = new AItree(gameMap);
            int maxScore = -1000;
            int index = 0;
            //prejst vsetkych hlavnych potomkov korena stromu
            for (int i = 0; i < root.tree.Count; i++)
            {
                //vybrat potomka s najvyssim skore, ak je ale skore protivnika vyssie, vybrat to
                if (root.tree[i].score > maxScore || Math.Abs(root.tree[i].score) > maxScore)
                {
                    maxScore = root.tree[i].score;
                    index = i;
                }
            }

            //protivnik ma vyssie skore, treba branit
            maxScore = -1000;
            if (maxScore < 0)
            {
                index = 0;
                for (int i = 0; i < root.tree.Count; i++)
                {
                    if (root.tree[i].score > maxScore)
                    {
                        maxScore = root.tree[i].score;
                        index = i;
                    }
                }
            }

            //zahrat tah s najlepsim skore     
            ComputerMove move = new ComputerMove
            {
                finalPosition = root.tree[index].position
            };
            foreach (AItree leaf in root.tree)
            {
                MoveData data = new MoveData() { coords = leaf.position, score = leaf.score };
                move.allMoves.Add(data);
            }

            return move;
        }
    }
}
