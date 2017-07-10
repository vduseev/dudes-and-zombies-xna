using Project.Game;

namespace Project
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            using (GameManager game = new GameManager())
            {
                game.Run();
            }
        }
    }
#endif
}

