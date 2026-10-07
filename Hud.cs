using Raylib_cs;
using System.Numerics;
static class Hud
{
 public const int Height = 100; //hud height   

//player values
public static int Health = 100;
public static int Armor = 0;
public static int Ammo = 50;
public static int MaxAmmo = 200;
public static List<Weapon> Weapons = new List<Weapon>();
public static int CurrentWeaponIndex = 0;
public static Weapon CurrentWeapon => Weapons[CurrentWeaponIndex];
//pizdak textures
static Texture2D _faceNormal;
static Texture2D _faceHurt;
static Texture2D _faceDead;

public static void Load()
    {
        _faceNormal = Raylib.LoadTexture("png/pizdak/pizdak-face.png");
        _faceHurt = Raylib.LoadTexture("png/pizdak/pizdak-hurt.png");
        _faceDead = Raylib.LoadTexture("png/pizdak/pizdak-dead.png");

        Raylib.SetTextureFilter(_faceNormal, TextureFilter.Point);
        Raylib.SetTextureFilter(_faceHurt, TextureFilter.Point);
        Raylib.SetTextureFilter(_faceDead, TextureFilter.Point);

        LoadWepons();
    }

    public static void Unload()
    {
        Raylib.UnloadTexture(_faceNormal);
        Raylib.UnloadTexture(_faceHurt);
        Raylib.UnloadTexture(_faceDead);
    }
public static void Draw()
{
    int ScreenW = Raylib.GetScreenWidth();
    int ScreenH = Raylib.GetScreenHeight();
    int panelY = ScreenH - Height;

    // hud background
    Raylib.DrawRectangle(0, panelY, ScreenW, Height, new Color((byte)70, (byte)70, (byte)70, (byte)255));
    Raylib.DrawRectangle(0, panelY, ScreenW, 3, new Color((byte)40, (byte)40, (byte)40, (byte)255));
    Raylib.DrawRectangle(0, panelY + Height - 3, ScreenW, 3, new Color((byte)40, (byte)40, (byte)40, (byte)255));

    int cellSize = 80;
    int spacing = 20;
    int faceSize = 70;
    int centerY = panelY + Height / 2;

    int startX = 20;
    DrawStatBlock(startX, centerY, "Health", Health, Color.Red);
    DrawStatBlock(startX + cellSize + spacing, centerY, "Armor", Armor, Color.Green);

    int ammoX = startX + (cellSize + spacing) * 2;
    if (Ammo > 0)
        DrawStatBlock(ammoX, centerY, "Ammo", Ammo, Color.Yellow);
    else
        DrawNoAmmoBlock(ammoX, centerY);

    //Ebalo
    int faceX = (ScreenW - faceSize) / 2;
    DrawFace(faceX, centerY);

    //weapon slots
    DrawWeaponSlots(ScreenW, panelY, Height);
}
   
    //pizdak face drawing
    static void DrawFace(int x, int centerY)
    {
        int size = 70;
        int y = centerY - size / 2;

        Raylib.DrawRectangle(x, y, size, size, new Color((byte)30, (byte)30,(byte)30,(byte)255));
        Raylib.DrawRectangleLines(x, y, size, size, new Color((byte)150, (byte)150,(byte)150,(byte)255));

        //select texture depending on health
        Texture2D texture;
        if (Health <= 0) texture = _faceDead;
        else if (Health <= 50) texture = _faceHurt;
        else texture = _faceNormal;

        Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
        Rectangle dest = new Rectangle(x, y, size, size);
        Vector2 origin = new Vector2(0, 0);

        Raylib.DrawTexturePro(texture, source, dest, origin, 0f, Color.White);
    }
    static void DrawStatBlock(int x, int centerY, string label, int value, Color valueColor)
    {
        int w = 80;
        int h = 70;
        int y = centerY - h / 2;

        Raylib.DrawRectangle(x, y, w, h, new Color((byte)30, (byte)30,(byte)30,(byte)255));
        Raylib.DrawRectangleLines(x, y, w, h, new Color((byte)150, (byte)150,(byte)150,(byte)255));

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
        int totalSlots = Weapons.Count;
        int totalWidth = totalSlots * slotSize + (totalSlots - 1) * slotGap;
        int startX = screenW - totalWidth - 20;
        int y = panelY + (panelHeight - slotSize) / 2;

        for(int i = 1; i <= totalSlots; i++)
        {
            int x = startX + (i - 1) * (slotSize + slotGap);
            
            //slot background
            Color BackGround = ((i - 1) == CurrentWeaponIndex)
            ? new Color((byte)180, (byte)180, (byte)60, (byte)255)
            : new Color((byte)40, (byte)40, (byte)40, (byte)255);

            Raylib.DrawRectangle(x, y, slotSize, slotSize, BackGround);
            Raylib.DrawRectangleLines(x, y, slotSize, slotSize, new Color((byte)150, (byte)150,(byte)150,(byte)255));
            //number of weapon
            string num = i.ToString();
            int numW = Raylib.MeasureText(num, 20);
            Raylib.DrawText(num, x + (slotSize - numW) / 2, y + slotSize / 2 - 12, 20,
            ((i - 1) == CurrentWeaponIndex) ? Color.Black : Color.LightGray);
        }
    }
    //load wepons
    public static void LoadWepons()
    {
        Weapons.Clear();
        //pistol
        Weapons.Add(new Weapon(
            name: "Pistol",
            damage: 1,
            magazineSize: 10,
            reloadTime: 1.5f,
            ammoPerShot: 1,
            fireRate:1
        ));
        Weapons[0].Owned = true;
        //shotgun
        Weapons.Add(new Weapon(
            name: "Shotgun",
            damage: 10,
            magazineSize: 5,
            reloadTime: 2.0f,
            ammoPerShot: 3,
            fireRate: 0.8f
        ));
         Weapons[1].Owned = true;
    }
    //damage to player function
    public static void DamagePlayer(int amount)
    {
        if (amount <= 0) return;

        if (Armor > 0)
        {
            int absorbed = Math.Min(Armor, amount);
            Armor -= absorbed;
            amount -= absorbed;
        }
        if(amount > 0)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
        }
    }
    
} 

