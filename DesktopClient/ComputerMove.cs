using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopClient
{
    /// <summary>
    /// Obsahuje zvoleny tah a zoznam vsetkych moznych tahov s ich ohodnotenim
    /// </summary>
    class ComputerMove
    {
        public Point finalPosition;
        public List<MoveData> allMoves = new List<MoveData>();
    }
}
