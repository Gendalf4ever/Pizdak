using Raylib_cs;
using System.Numerics;

class Enemy
{
    public float X;
    public float Y;
    public bool Alive = true;

    public int Health;
    public int MaxHealth;
    public float Speed;
    public int Damage;
    public float AttackRange;
    public float AttackCooldown;
    public float LastAttackTime;

    public Color bodyColor = Color.Red;

    public bool Update(float playerX, float playerY, int[,] map, float deltaTime)
    {
        if (!Alive) return false;

        float dx = playerX - X;
        float dy = playerY - Y;
        float distance = MathF.Sqrt(dx * dx + dy * dy);

        if (distance <= AttackRange)
        {
            LastAttackTime -= deltaTime;
            if (LastAttackTime <= 0f)
            {
                LastAttackTime = AttackCooldown;
                return true;
            }
            return false;
        }
        //movement to player
        if (distance > 0.01f)
        {
            float moveX = dx / distance * Speed;
            float moveY = dy / distance * Speed;

            float newX = X + moveX;
            float newY = Y + moveY;
            if (map[(int)newX, (int)Y] == 0) X = newX;
            if (map[(int)X, (int)newY] == 0) Y = newY;
        }

        return false;
    }

    public bool TakeDamage(int amount)
    {
        if (!Alive) return false;

        Health -= amount;
        if (Health <= 0)
        {
            Health = 0;
            Alive = false;
            return true;
        }
        return false;
    }
}