using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleBoardGame
{
    public class Variation
    {
        public Move Move { get; set; }

        public Vector4 Eval { get; set; }

        public Variation(Move move, Vector4 eval)
        {
            Move = move;
            Eval = eval;
        }
    }
}
