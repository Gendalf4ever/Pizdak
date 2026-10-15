using Raylib_cs;
public enum MenuState
{
    MainMenu,
    LevelSelect
}
class UI
{
    static void Main()
    {
        Raylib.InitWindow(1280, 720, "Game");
        Hud.Load();
        Raylib.SetTargetFPS(60);
        MenuState currentState = MenuState.MainMenu;
        int menuSelection = 0;
        int levelSelection = 0;
        int screenWidth = Raylib.GetScreenWidth();

        while (!Raylib.WindowShouldClose())
        {
            if (currentState == MenuState.MainMenu)
            {
                //navigation
                if (Raylib.IsKeyPressed(KeyboardKey.Down)) menuSelection = (menuSelection + 1) % 3;
                if (Raylib.IsKeyPressed(KeyboardKey.Up)) menuSelection = (menuSelection - 1 + 3) % 3;

                //selection
                if (Raylib.IsKeyDown(KeyboardKey.Enter))
                {
                    if (menuSelection == 0) Program.StartGame(1); //new game
                    else if (menuSelection == 1) currentState = MenuState.LevelSelect; //select a level
                    else if (menuSelection == 2) break; //exit game
                }
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color((byte)20, (byte)20, (byte)20, (byte)255));
                int titleW = Raylib.MeasureText("Pizdak", 60);
                Raylib.DrawText("Pizdak", (screenWidth - titleW)/ 2, 150, 60, Color.Red);
                string[] menuItems = {"NEW GAME", "SELECT LEVEL", "EXIT"};
                for (int i = 0; i < menuItems.Length; i++)
                {
                    Color color = (i== menuSelection) ? Color.Yellow: Color.White;
                    int itemW = Raylib.MeasureText(menuItems[i], 40);
                    Raylib.DrawText(menuItems[i], (screenWidth - itemW) / 2, 350 + i * 60, 40, color);
                }
                Raylib.EndDrawing();
            }
            else if (currentState == MenuState.LevelSelect)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Down)) levelSelection = (levelSelection + 1) %3;
                if (Raylib.IsKeyPressed(KeyboardKey.Up)) levelSelection = (levelSelection - 1 + 3) %3;
                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    if (levelSelection == 0) Program.StartGame(1);
                    else if (levelSelection == 1) Program.StartGame(2);
                    else if (levelSelection == 2) currentState = MenuState.MainMenu;
                }
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color((byte)20, (byte)20, (byte)20, (byte)255));
                int titleW = Raylib.MeasureText("SELECT LEVEL", 50);
                Raylib.DrawText("SELECT LEVEL", (screenWidth - titleW) / 2, 150, 50, Color.Red);
                string[] levelItems = {"LEVEL 1", "LEVEL 2", "BACK"};
                for (int i = 0; i < levelItems.Length; i++)
                {
                    Color color = (i == levelSelection) ? Color.Yellow : Color.White;
                    int itemW = Raylib.MeasureText(levelItems[i], 40);
                    Raylib.DrawText(levelItems[i], (screenWidth - itemW) / 2, 350 + i * 60, 40, color);
                }
                Raylib.EndDrawing();
            }
        }
        Hud.Unload();
        Raylib.CloseWindow();
    }
}