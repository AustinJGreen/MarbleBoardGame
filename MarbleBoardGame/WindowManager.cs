using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleBoardGame
{
    public class WindowManager
    {
        private List<WindowPane> windows;

        public void AddWindow(WindowPane window)
        {
            windows.Add(window);
        }



        public WindowManager()
        {
            windows = new List<WindowPane>();
        }
    }
}
