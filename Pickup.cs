using Raylib_cs;

enum PickupType
{
    Ammo,
    Health,
    Armor
}
class Pickup
{
    public PickupType Type;
    public float X;
    public float Y;

    public int Amount;
    public bool Taken = false;

    public Color BodyColor;

    public Pickup(PickupType type, float x, float y, int amount)
    {
        Type = type;
        X = x;
        Y = y;
        Amount = amount;

        BodyColor = type switch
        {
            PickupType.Ammo => new Color((byte)230, (byte)220, (byte)60, (byte)255),
            PickupType.Health => new Color((byte)220, (byte)60, (byte)60, (byte)255),
            PickupType.Armor => new Color((byte)60, (byte)180, (byte)80, (byte)255),
            _  => Color.White
        };
    }
    //apply pickup to player
    public bool Apply()
    {
        if (Taken) return false;

        switch (Type)
        {
            case PickupType.Ammo:
                if (Hud.Ammo >= Hud.MaxAmmo) return false;
                Hud.Ammo += Amount;
                if (Hud.Ammo > Hud.MaxAmmo) Hud.Ammo = Hud.MaxAmmo;
                break;
            case PickupType.Health:
                if (Hud.Health >= 100) return false;
                Hud.Health += Amount;
                if (Hud.Health > 100) Hud.Health = 100;
                break;
            case PickupType.Armor:
                if (Hud.Armor >= 100) return false;
                Hud.Armor += Amount;
                if (Hud.Armor > 100) Hud.Armor = 100;
                break;
        }
        Taken = true;
        return true;
    }
}