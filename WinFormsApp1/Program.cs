using System;
using System.Windows.Forms;

namespace Snake
{ 
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Snake snake = new Snake();
            Application.Run(snake.GetForm);
            //Application.Run(new Form1());
        }
    }
}
