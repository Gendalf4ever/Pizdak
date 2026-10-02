using Raylib_cs;

class Weapon
{
    public string Name;
    public int Damage;
    public int MagazineSize;
    public float ReloadTime;
    public int AmmoPerShot;

    //state changes in game
    public int AmmoInMagazine; //how much ammo before reload
    public float ReloadTimer;
    public float FireCooldown;
    public float FireRate;
    public bool Owned = false; //if player has weapon
    //weapon construct
    public Weapon(string name, int damage, int magazineSize, float reloadTime, int ammoPerShot, float fireRate)
    {
        Name = name;
        Damage = damage;
        MagazineSize = magazineSize;
        ReloadTime = reloadTime;
        AmmoPerShot = ammoPerShot;
        FireRate = fireRate;

        AmmoInMagazine = magazineSize;
        Owned = false;
    }

    public bool CanShoot()
    {
        if (ReloadTimer > 0f) return false;
        if (FireCooldown > 0f) return false;
        if (AmmoInMagazine <= 0) return false;
        if (Hud.Ammo < AmmoPerShot) return false;
        return true;
    }

    public void Update(float deltaTime)
    {
        if(ReloadTimer > 0f)
        {
            ReloadTimer -= deltaTime;
            if (ReloadTimer <= 0f)
            {
                int need = MagazineSize - AmmoInMagazine;
                int take = Math.Min(need, Hud.Ammo);
                AmmoInMagazine += take;
                Hud.Ammo -= take;
            }
        }
        if (FireCooldown > 0f) FireCooldown -= deltaTime;
    }
    public bool TryFire(out int damage)
    {
        damage = 0;
        if (!CanShoot()) return false;
        AmmoInMagazine--;
        Hud.Ammo -= AmmoPerShot;
        FireCooldown = FireRate;
        damage = Damage;
        if (AmmoInMagazine <= 0)
        {
            ReloadTimer = ReloadTime;
        }
        return true;
    }
    public void StartReload()
    {
        if (ReloadTimer > 0f) return;
        if (AmmoInMagazine >= MagazineSize) return;
        if (Hud.Ammo <= 0) return;
        ReloadTimer = ReloadTime;
    }
}