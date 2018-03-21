using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    public class BoardLayouts
    {
        public static string[][][] LAYOUTS = new string[4][][]
        {
            new string[13][]
		    {
			    new string[] { null, null, null, null, null, "r11", "r12", "g1", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r10", "hg1", "g2", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r9", "hg2", "g3", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r8", "hg3", "g4", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r7", "hg4", "g5", null, null, null, null, null },
			    new string[] { "r1", "r2", "r3", "r4", "r5", "r6", "hg5", "g6", "g7", "g8", "g9", "g10", "g11" },
			    new string[] { "y12", "hr1", "hr2", "hr3", "hr4", "hr5", null, "hb5", "hb4", "hb3", "hb2", "hb1", "g12" },
			    new string[] { "y11", "y10", "y9", "y8", "y7", "y6", "hy5", "b6", "b5", "b4", "b3", "b2", "b1" },
			    new string[] { null, null, null, null, null, "y5", "hy4", "b7", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y4", "hy3", "b8", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y3", "hy2", "b9", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y2", "hy1", "b10", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y1", "b12", "b11", null, null, null, null, null }
		    },
            new string[13][]
		    {
			    new string[] { null, null, null, null, null, "g11", "g12", "b1", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "g10", "hb1", "b2", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "g9", "hb2", "b3", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "g8", "hb3", "b4", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "g7", "hb4", "b5", null, null, null, null, null },
			    new string[] { "g1", "g2", "g3", "g4", "g5", "g6", "hb5", "b6", "b7", "b8", "b9", "b10", "b11" },
			    new string[] { "r12", "hg1", "hg2", "hg3", "hg4", "hg5", null, "hy5", "hy4", "hy3", "hy2", "hy1", "b12" },
			    new string[] { "r11", "r10", "r9", "r8", "r7", "r6", "hr5", "y6", "y5", "y4", "y3", "y2", "y1" },
			    new string[] { null, null, null, null, null, "r5", "hr4", "y7", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r4", "hr3", "y8", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r3", "hr2", "y9", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r2", "hr1", "y10", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "r1", "y12", "y11", null, null, null, null, null }
		    },
            new string[13][]
            {
                new string[] { null, null, null, null, null, "b11", "b12", "y1",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "b10", "hy1", "y2",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "b9", "hy2", "y3",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "b8", "hy3", "y4",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "b7", "hy4", "y5",  null, null, null, null, null },
                new string[] { "b1", "b2", "b3", "b4", "b5", "b6", "hy5", "y6",  "y7", "y8", "y9", "y10", "y11" },
                new string[] { "g12", "hb1", "hb2", "hb3", "hb4", "hb5", null, "hr5", "hr4", "hr3", "hr2", "hr1", "y12" },
                new string[] { "g11", "g10", "g9", "g8", "g7", "g6", "hg5", "r6", "r5", "r4", "r3", "r2", "r1" },
                new string[] { null, null, null, null, null, "g5", "hg4", "r7",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "g4", "hg3", "r8",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "g3", "hg2", "r9",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "g2", "hg1", "r10",  null, null, null, null, null },
                new string[] { null, null, null, null, null, "g1", "r12", "r11",  null, null, null, null, null }
            },
            new string[13][]
		    {
			    new string[] { null, null, null, null, null, "y11", "y12", "r1", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y10", "hr1", "r2", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y9", "hr2", "r3", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y8", "hr3", "r4", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "y7", "hr4", "r5", null, null, null, null, null },
			    new string[] { "y1", "y2", "y3", "y4", "y5", "y6", "hr5", "r6", "r7", "r8", "r9", "r10", "r11" },
			    new string[] { "b12", "hy1", "hy2", "hy3", "hy4", "hy5", null, "hg5", "hg4", "hg3", "hg2", "hg1", "r12" },
			    new string[] { "b11", "b10", "b9", "b8", "b7", "b6", "hb5", "g6", "g5", "g4", "g3", "g2", "g1" },
			    new string[] { null, null, null, null, null, "b5", "hb4", "g7", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "b4", "hb3", "g8", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "b3", "hb2", "g9", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "b2", "hb1", "g10", null, null, null, null, null },
			    new string[] { null, null, null, null, null, "b1", "g12", "g11", null, null, null, null, null }
		    }
        };
    }
}
