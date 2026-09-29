using Raylib_cs;
static class Hud
{
 public const int Height = 100; //hud height   

//player values
public static int Health = 100;
public static int Armor = 0;
public static int Ammo = 50;
public static int MaxAmmo = 200;
public static int CurrentWeapon = 1; //1 - pistol 2 - shotgun 3 - assault rifle

public static void Draw()
    {
        int ScreenW = Raylib.GetScreenWidth();
        int ScreenH = Raylib.GetScreenHeight();
        int panelY = ScreenH - Height;

        //hud background
        Raylib.DrawRectangle(0, panelY, ScreenW, Height, new Color((byte)70, (byte)70,(byte)70,(byte)255));
        Raylib.DrawRectangle(0, panelY, ScreenW, 3, new Color((byte)40, (byte)40,(byte)40,(byte)255));
        Raylib.DrawRectangle(0, panelY + Height -3, ScreenW, 3, new Color((byte)40, (byte)40,(byte)40,(byte)255));

        //health block
        int cellSize = 80;
        int spacing = 20;
        int startX = 20;
        int centerY = panelY + Height / 2;

        DrawStatBlock(startX, centerY, "Health", Health, Color.Red);

        //armor block
        DrawStatBlock(startX + cellSize + spacing, centerY, "Armor", Armor, Color.Green);
        //ammo block
        int ammoX = startX + (cellSize + spacing) * 2;
        if (Ammo > 0)
            DrawStatBlock(ammoX, centerY, "Ammo", Ammo, Color.Yellow);
        else
            DrawNoAmmoBlock(ammoX, centerY);
        DrawWeaponSlots(ScreenW, panelY, Height);
    }

    static void DrawStatBlock(int x, int centerY, string label, int value, Color valueColor)
    {
        int w = 80;
        int h = 70;
        int y = centerY - h / 2;

        Raylib.DrawRectangle(x, y, w, h, new Color((byte)30, (byte)30,(byte)30,(byte)255));
        Raylib.DrawRectangle(x, y, w, h, new Color((byte)150, (byte)150,(byte)150,(byte)255));

        int labelW = Raylib.MeasureText(label, 10);
        Raylib.DrawText(label, x + (w - labelW) / 2, y + 4, 10, Color.LightGray);

        //number in center
        string text = value.ToString();
        int fontSize = 30;
        int textW = Raylib.MeasureText(text, fontSize);
        Raylib.DrawText(text, x + (w - textW) / 2, y + 22, fontSize, valueColor);
    }
    static void DrawNoAmmoBlock(int x, int centerY)
    {
        int w = 80;
        int h = 70;
        int y = centerY - h / 2;
        Raylib.DrawRectangle(x, y, w, h, new Color((byte)30, (byte)30, (byte)30, (byte)255));
        Raylib.DrawRectangleLines(x, y, w, h, new Color((byte)150, (byte)150, (byte)150, (byte)255));

        int labelW = Raylib.MeasureText("Ammo", 10);
        Raylib.DrawText("Ammo", x + (w - labelW) / 2, y + 4, 10, Color.LightGray);

        string line1 = "NO";
        string line2 = "AMMO";
        int fontSize = 16;
        int w1 = Raylib.MeasureText(line1, fontSize);
        int w2 = Raylib.MeasureText(line2, fontSize);
        Raylib.DrawText(line1, x + (w-w1) / 2, y + 22, fontSize, Color.Red);
        Raylib.DrawText(line2, x + (w-w2) / 2, y + 42, fontSize, Color.Red);
    }
    //weapon slots
    static void DrawWeaponSlots(int screenW, int panelY, int panelHeight)
    {
        int slotSize = 40;
        int slotGap = 5;
        int totalSlots = 3;
        int totalWidth = totalSlots * slotSize + (totalSlots - 1) * slotGap;
        int startX = screenW - totalWidth - 20;
        int y = panelY + (panelHeight - slotSize) / 2;

        for(int i = 1; i <= totalSlots; i++)
        {
            int x = startX + (i - 1) * (slotSize + slotGap);
            
            //slot background
            Color BackGround = (i == CurrentWeapon)
            ? new Color((byte)180, (byte)180, (byte)60, (byte)255)
            : new Color((byte)40, (byte)40, (byte)40, (byte)255);

            Raylib.DrawRectangle(x, y, slotSize, slotSize, BackGround);
            Raylib.DrawRectangleLines(x, y, slotSize, slotSize, new Color((byte)150, (byte)150,(byte)150,(byte)255));
            //number of weapon
            string num = i.ToString();
            int numW = Raylib.MeasureText(num, 20);
            Raylib.DrawText(num, x + (slotSize - numW) / 2, y + slotSize / 2 - 12, 20,
            (i == CurrentWeapon) ? Color.Black : Color.LightGray);
        }
    }
} 