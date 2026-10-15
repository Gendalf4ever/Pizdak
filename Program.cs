using Raylib_cs;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    const int MapWidth = 24;
    const int MapHeight = 24;

    static int[,] LoadMapFromFile(string fileName)
    {
        int[,] map = new int[MapWidth, MapHeight];
        
        if (!File.Exists(fileName)) 
        {
            Console.WriteLine($"ОШИБКА: Файл {fileName} не найден! Создана пустая комната.");
            for (int i = 0; i < MapWidth; i++)
            {
                map[i, 0] = 1;
                map[i, MapHeight - 1] = 1;
                map[0, i] = 1;
                map[MapWidth - 1, i] = 1;
            }
            return map; 
        }
 
        string text = File.ReadAllText(fileName);
        int x = 0, y = 0;
        
        foreach (char c in text)
        {
            if (char.IsDigit(c))
            {
                if (x < MapWidth && y < MapHeight)
                {
                    map[x, y] = c - '0'; 
                }
                
                x++;
                if (x >= MapWidth)
                {
                    x = 0;
                    y++;
                }
            }
        }
        
        for (int i = 0; i < MapWidth; i++)
        {
            if (map[i, 0] == 0) map[i, 0] = 1;
            if (map[i, MapHeight - 1] == 0) map[i, MapHeight - 1] = 1;
            if (map[0, i] == 0) map[0, i] = 1;
            if (map[MapWidth - 1, i] == 0) map[MapWidth - 1, i] = 1;
        }

        return map;
    }

    static LevelData LoadLevel(int levelIndex)
    {
        string fileName = Path.Combine("maps", $"level{levelIndex}.txt");
        int[,] mapLayout = LoadMapFromFile(fileName);

        LevelData level = new LevelData(mapLayout, ExitCondition.ReachExit, 12.0f, 12.0f);

        //default enemy
        level.Enemies.Add(new Enemy
        {
            X = 7.5f, Y = 7.5f,
            Health = 30, MaxHealth = 30,
            Speed = 0.02f,
            Damage = 5,
            AttackRange = 0.8f,
            AttackCooldown = 1.0f   
        });
        //tank enemy
        level.Enemies.Add(new Enemy
        {
            X = 12.5f, Y = 4.5f,
            Health = 100, MaxHealth = 100,
            Speed = 0.008f,
            Damage = 20,
            AttackRange = 0.8f,
            AttackCooldown = 1.5f,
            bodyColor = new Color((byte)120, (byte)30, (byte)30, (byte)255)   
        });
        //speed enemy
        level.Enemies.Add(new Enemy
        {
            X = 18.5f, Y = 15.5f,
            Health = 15, MaxHealth = 15,
            Speed = 0.04f,
            Damage = 3,
            AttackRange = 0.7f,
            AttackCooldown = 0.5f   
        });

        //pickup method
        //ammo
        level.Pickups.Add(new Pickup(PickupType.Ammo, 7.5f, 12.5f, 25));
        level.Pickups.Add(new Pickup(PickupType.Ammo, 20.5f, 3.5f, 25));
        level.Pickups.Add(new Pickup(PickupType.Ammo, 15.5f, 18.5f, 50));
        //hp
        level.Pickups.Add(new Pickup(PickupType.Health, 3.5f, 3.5f, 20));
        level.Pickups.Add(new Pickup(PickupType.Health, 22.5f, 20.5f, 25));
        level.Pickups.Add(new Pickup(PickupType.Health, 19.5f, 8.5f, 15));
        //armor
        level.Pickups.Add(new Pickup(PickupType.Armor, 5.5f, 20.5f, 15));
        level.Pickups.Add(new Pickup(PickupType.Armor, 19.5f, 5.5f, 30));
        
        return level;
    }

    static void Main()
    {
        Raylib.InitWindow(1280,720, "Game");
        Hud.Load();
        Raylib.SetTargetFPS(60);
        Raylib.DisableCursor();

        int currentLevelIndex = 1;

        LevelData currentLevel = LoadLevel(currentLevelIndex);
        int[,] map = currentLevel.Map;
        float posX = currentLevel.StartX;
        float posY = currentLevel.StartY;
        List<Enemy> enemies = currentLevel.Enemies;
        List<Pickup> pickups = currentLevel.Pickups;

        //sight direction
        float dirX = 1.0f;
        float dirY = 0.0f;

        //fov
        float planeX = 0.0f;
        float planeY = 0.66f;

        float moveSpeed = 0.05f;
        float rotSpeed = 0.03f;
        float mouseSens = 0.002f;
        float shotTimer = 0f;
        float shotDuration = 0.08f;

        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight() - Hud.Height;
        int halfHeight = screenHeight / 2;
        //z-buffer
        float[] zBuffer = new float[screenWidth];

        bool isDead = false;
        bool isLevelComplete = false;

        while(!Raylib.WindowShouldClose())
        {
            //check if player is alive
            if (Hud.Health <= 0) isDead = true;

            if(isDead && Raylib.IsKeyPressed(KeyboardKey.R))
            {
                Hud.Health = 100;
                Hud.Ammo = 50;
                Hud.Armor = 0;
                
                currentLevel = LoadLevel(currentLevelIndex);
                map = currentLevel.Map;
                posX = currentLevel.StartX;
                posY = currentLevel.StartY;
                enemies = currentLevel.Enemies;
                pickups = currentLevel.Pickups;

                dirX = 1.0f;
                dirY = 0.0f;
                planeX = 0.0f;
                planeY = 0.66f;
                isDead = false;
                isLevelComplete = false;
            }

            if (!isDead && !isLevelComplete)
            {
                CheckExitConditions(ref isLevelComplete, currentLevel.Condition, posX, posY, map, enemies);
            }

            if (isLevelComplete)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);
                int textW = Raylib.MeasureText("LEVEL COMPLETE!", 50);
                Raylib.DrawText("LEVEL COMPLETE!", (screenWidth - textW) / 2, screenHeight / 2 - 30, 50, Color.Green);
                
                int hintW = Raylib.MeasureText("Press ENTER to continue", 20);
                Raylib.DrawText("Press ENTER to continue", (screenWidth - hintW) / 2, screenHeight / 2 + 30, 20, Color.White);
                Raylib.EndDrawing();

                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    currentLevelIndex++;
                    
                    if (!File.Exists(Path.Combine("maps", $"level{currentLevelIndex}.txt")))
                    {
                        break; 
                    }

                    Hud.Health = 100;
                    currentLevel = LoadLevel(currentLevelIndex); 
                    map = currentLevel.Map;
                    posX = currentLevel.StartX;
                    posY = currentLevel.StartY;
                    enemies = currentLevel.Enemies;
                    pickups = currentLevel.Pickups;
                    isLevelComplete = false;
                }
                continue; 
            }

            if (!isDead)
            {
                //Turn angle
                float angle = 0;
                if (Raylib.IsKeyDown(KeyboardKey.Left)) angle -= rotSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.Right)) angle += rotSpeed;

                Vector2 mouseDelta = Raylib.GetMouseDelta();
                angle += mouseDelta.X * mouseSens;

                if (angle != 0)
                {
                    float cos = MathF.Cos(angle);
                    float sin = MathF.Sin(angle);

                    float oldDirX = dirX;
                    dirX = dirX * cos - dirY * sin;
                    dirY = oldDirX * sin + dirY * cos;

                    float oldPlaneX = planeX;
                    planeX = planeX * cos - planeY * sin;
                    planeY = oldPlaneX * sin + planeY * cos;
                }

                //movement
                float newX = posX;
                float newY = posY;

                if(Raylib.IsKeyDown(KeyboardKey.W))
                {
                    newX += dirX * moveSpeed;
                    newY += dirY * moveSpeed;
                }
                if(Raylib.IsKeyDown(KeyboardKey.S))
                {
                    newX -= dirX * moveSpeed;
                    newY -= dirY * moveSpeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.A))
                {
                    newX -= planeX * moveSpeed;
                    newY -= planeY * moveSpeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.D))
                {
                    newX += planeX * moveSpeed;
                    newY += planeY * moveSpeed;
                }

                if (newX >= 0 && newX < MapWidth && newY >= 0 && newY < MapHeight)
                {
                    int nextTileX = map[(int)newX, (int)posY];
                    int nextTileY = map[(int)posX, (int)newY];

                    if (nextTileX == 0 || nextTileX == 2) posX = newX;
                    if (nextTileY == 0 || nextTileY == 2) posY = newY;
                }

                //enemy update
                float dt = Raylib.GetFrameTime(); 
                foreach (var e in enemies)
                {
                    bool attacking = e.Update(posX, posY, map, dt);
                    if (attacking)
                    {
                        Hud.DamagePlayer(e.Damage);
                    }
                }

                //pickups
                foreach (var p in pickups)
                {
                    if (p.Taken) continue;
                    float dx = p.X - posX;
                    float dy = p.Y - posY;
                    float dist = MathF.Sqrt(dx * dx + dy* dy);
                    if (dist < 0.5f)
                    {
                        if (p.Apply())
                        {
                            Console.WriteLine($"Подобрано: {p.Type} +{p.Amount}");
                        }
                    }
                }

                //shooting
                if(Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Weapon weapon = Hud.CurrentWeapon;
                    if (weapon.TryFire(out int damage))
                    {
                        Enemy? hit = FindEnemyInCrosshair(posX, posY, dirX, dirY, planeX, planeY, zBuffer[screenWidth / 2], enemies);
                        if (hit != null)
                        {
                            bool died = hit.TakeDamage(damage);
                        }
                        shotTimer = shotDuration; 
                    }
                }
                if (Raylib.IsKeyPressed(KeyboardKey.One)) TrySwitchWeapon(0);
                if (Raylib.IsKeyPressed(KeyboardKey.Two)) TrySwitchWeapon(1);
                if (Raylib.IsKeyPressed(KeyboardKey.Three)) TrySwitchWeapon(2);

                //reload
                if (Raylib.IsKeyPressed(KeyboardKey.R)) Hud.CurrentWeapon.StartReload();

                Hud.CurrentWeapon.Update(Raylib.GetFrameTime());
                if (shotTimer > 0f)
                {
                    shotTimer -= Raylib.GetFrameTime();
                }
            }

            //drawing
            Raylib.BeginDrawing();

            //space and floor
            Raylib.DrawRectangle(0, 0, screenWidth, halfHeight, new Color(50, 50, 70, 255));
            Raylib.DrawRectangle(0, halfHeight, screenWidth, halfHeight, new Color(70, 55, 40, 255));
            
            //Walls raycasting
            for (int x = 0; x < screenWidth; x++)
            {
                float cameraX = 2.0f * x / screenWidth - 1.0f;

                float rayDirX = dirX + planeX * cameraX;
                float rayDirY = dirY + planeY * cameraX;

                int mapX = (int)posX;
                int mapY = (int)posY;

                float deltaDistX = MathF.Abs(1.0f / rayDirX);
                float deltaDistY = Math.Abs(1.0f / rayDirY);

                int stepX, stepY;
                float sideDistX, sideDistY;

                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (posX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0f - posX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (posY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0f - posY) * deltaDistY;
                }

                int side = 0;
                while (true)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    if (mapX < 0 || mapX >= MapWidth || mapY < 0 || mapY >= MapHeight)
                        break;

                    if (map[mapX, mapY] == 1) break; 
                }

                float perpWallDist = 100.0f; // <-- Фикс: если луч улетел за карту, рисуем его очень далеко, чтобы не перекрывал экран
                if (mapX >= 0 && mapX < MapWidth && mapY >= 0 && mapY < MapHeight)
                {
                    if (side == 0) perpWallDist = sideDistX - deltaDistX;
                    else perpWallDist = sideDistY - deltaDistY;
                }
                
                zBuffer[x] = perpWallDist;

                int lineHeight = (int)(screenHeight / perpWallDist);
                int drawStart = -lineHeight / 2 + halfHeight;
                int drawEnd = lineHeight / 2 + halfHeight;
                if (drawStart < 0) drawStart = 0;
                if (drawEnd >= screenHeight) drawEnd = screenHeight - 1;

                Color c = (side == 0)
                ? new Color(190, 130, 70, 255)
                : new Color(140, 90, 50, 255);

                Raylib.DrawRectangle(x, drawStart, 1, drawEnd - drawStart, c);
            }

            //enemies
            DrawEnemies(posX, posY, dirX, dirY, planeX, planeY, zBuffer, enemies);
            //pickups
            DrawPickups(posX, posY, dirX, dirY, planeX, planeY, zBuffer, pickups);
            //Minimap
            DrawMinimap(posX, posY, dirX, dirY, map);

            //Crosshair
            Raylib.DrawLine(screenWidth / 2 - 8, halfHeight, screenWidth / 2 + 8, halfHeight, Color.White);
            Raylib.DrawLine(screenWidth / 2, halfHeight - 8, screenWidth / 2, halfHeight + 8, Color.White);

            if (shotTimer > 0f)
            {
                float alpha = shotTimer / shotDuration;
                byte a = (byte)(alpha * 255);
                Raylib.DrawCircle(screenWidth / 2, halfHeight, 40, new Color((byte)255, (byte)220, (byte)100, a));
                Raylib.DrawLine(screenWidth / 2 - 60, halfHeight, screenWidth / 2 + 60, halfHeight, new Color((byte)255, (byte)255, (byte)200, a));
                Raylib.DrawLine(screenWidth / 2, halfHeight - 60, screenWidth / 2, halfHeight + 60, new Color((byte)255, (byte)255, (byte)200, a));
            }
            
            //Hud
            Hud.Draw();
            //death screen
            if (isDead) DrawDeathScreen();
            Raylib.EndDrawing();
        }
        Hud.Unload();
        Raylib.CloseWindow();
    }

    static void CheckExitConditions(ref bool isComplete, ExitCondition condition, float posX, float posY, int[,] map, List<Enemy> enemies)
    {
        switch (condition)
        {
            case ExitCondition.KillAllEnemies:
                if (enemies.TrueForAll(e => !e.Alive))
                {
                    isComplete = true;
                }
                break;
            case ExitCondition.ReachExit:
                if (posX >= 0 && posX < MapWidth && posY >= 0 && posY < MapHeight)
                {
                    if (map[(int)posX, (int)posY] == 2)
                    {
                        isComplete = true;
                    }
                }
                break;
            case ExitCondition.FindKeyAndExit:
                break;
        }
    }

    //draw enemies function
    static void DrawEnemies(float posX, float posY, float dirX, float dirY, float planeX, float planeY, float[] zBuffer, List<Enemy> enemies)
    {
        int screenW = Raylib.GetScreenWidth();
        int screenH = Raylib.GetScreenHeight();
        int halfH = screenH / 2;

        float invDet = 1.0f / (planeX * dirY - dirX * planeY);
        foreach (var e in enemies)
        {
            if (!e.Alive) continue;
            float dx = e.X - posX;
            float dy = e.Y - posY;

            //enemy  coordinates relative camera
            float transformX = invDet * (dirY * dx - dirX * dy);
            float transformY = invDet * (-planeY * dx + planeX * dy);

            if(transformY <= 0.01f) continue;

            int spriteScreenX = (int)((screenW /2) * (1 + transformX / transformY));
            int spriteHeight = Math.Abs((int)(screenH / transformY));
            int spriteWidth = spriteHeight;

            int drawStartY = -spriteHeight / 2 + halfH;
            int drawEndY = spriteHeight / 2 + halfH;
            int drawStartX = -spriteWidth / 2 + spriteScreenX;
            int drawEndX = spriteWidth / 2 + spriteScreenX;

            if (drawStartY < 0) drawStartY = 0;
            if (drawEndY >= screenH) drawEndY = screenH - 1;

            for (int x = drawStartX; x < drawEndX; x++)
            {
                if (x < 0 || x >= screenW) continue;
                if (transformY >= zBuffer[x]) continue;

                Raylib.DrawRectangle(x, drawStartY, 1, drawEndY - drawStartY, e.bodyColor);
            }
        }   
    }

    //draw pickups function
    static void DrawPickups(float posX, float posY, float dirX, float dirY, float planeX, float planeY, float[] zBuffer, List<Pickup> pickups)
    {
        int screenW = Raylib.GetScreenWidth();
        int screenH = Raylib.GetScreenHeight() - Hud.Height;
        int halfH = screenH / 2;
        float invDet = 1.0f / (planeX * dirY - dirX * planeY);
        foreach (var p in pickups)
        {
            if (p.Taken) continue;
            float dx = p.X - posX;
            float dy = p.Y - posY;

            float transformX = invDet * (dirY * dx - dirX * dy);
            float transformY = invDet * (-planeY * dx + planeX * dy);

            if (transformY <= 0.01f) continue;

            int spriteScreenX = (int)((screenW / 2) * (1 + transformX / transformY));

            //pickups height and width
            int spriteHeight = Math.Abs((int)(screenH / transformY)) / 2;
            int spriteWidth = spriteHeight;
            //pickups on the floor
            int floorOffset = (int)(screenH / transformY) / 4;
            
            int drawStartY = -spriteHeight / 2 + halfH + floorOffset;
            int drawEndY = spriteHeight / 2 + halfH + floorOffset;
            int drawStartX = -spriteWidth / 2 + spriteScreenX;
            int drawEndX = spriteWidth / 2 + spriteScreenX;

            if (drawStartY < 0) drawStartY = 0;
            if (drawEndY >= screenH) drawEndY = screenH - 1;

            for (int x = drawStartX; x < drawEndX; x++)
            {
                if (x < 0 || x>= screenW) continue;
                if (transformY >= zBuffer[x]) continue;

                Raylib.DrawRectangle(x, drawStartY, 1, drawEndY - drawStartY, p.BodyColor);
            }
        }
    }

    //find enemy in crosshair function
    static Enemy? FindEnemyInCrosshair(float posX, float posY, float dirX, float dirY, float planeX, float planeY, float wallDist, List<Enemy> enemies)
    {
        float invDet = 1.0f / (planeX * dirY - dirX * planeY);
        Enemy? closest = null;
        float closestDist = float.MaxValue;
        foreach (var e in enemies)
        {
            if (!e.Alive) continue;
            float dx = e.X - posX;
            float dy = e.Y - posY;

            float transformX = invDet * (dirY * dx - dirX * dy);
            float transformY = invDet * (-planeY * dx + planeX * dy);

            if (transformY <= 0) continue;
            if (transformY >= wallDist) continue;
            if (Math.Abs(transformX) > 0.4f) continue;

            if (transformY < closestDist)
            {
                closestDist = transformY;
                closest = e;
            }
        }
        return closest;
    }

    //minimap function
    static void DrawMinimap(float posX, float posY, float dirX, float dirY, int[,] map)
    {
        int scale = 4;
        int offsetX = 10;
        int offsetY = 10;

        for (int y = 0; y < MapHeight; y++)
        {
            for(int x = 0; x < MapWidth; x++)
            {
                Color c = map[x,y] == 1 ? Color.Gray : Color.DarkGray;
                if (map[x,y] == 2) c = Color.Green; 
                Raylib.DrawRectangle(offsetX + x * scale, offsetY + y * scale, scale, scale, c);
            }
        }
        int px = offsetX + (int)(posX * scale);
        int py = offsetY + (int)(posY * scale);
        Raylib.DrawCircle(px, py, 3, Color.Red);
        Raylib.DrawLine(px, py,
        px + (int)(dirX * scale * 3),
        py + (int)(dirY * scale * 3),
        Color.Yellow);      
    }

    //Try Switch Weapon
    static void TrySwitchWeapon(int index)
    {
        if (index < 0 || index >= Hud.Weapons.Count) return;
        if (!Hud.Weapons[index].Owned) return;
        Hud.CurrentWeaponIndex = index;
    }

    //death screen
    static void DrawDeathScreen()
    {
        int sw = Raylib.GetScreenWidth();
        int sh = Raylib.GetScreenHeight() - Hud.Height;

        Raylib.DrawRectangle(0, 0, sw, sh, new Color((byte)120, (byte)0, (byte)0, (byte)140));
        //dark souls reference xd
        string deathText = "YOU DIED";
        int fontSize = 60;
        int textW = Raylib.MeasureText(deathText, fontSize);
        Raylib.DrawText(deathText, (sw-textW) / 2, sh / 2 - 80, fontSize, Color.Black);

        string hint = "Press R to restart";
        int hintSize = 24;
        int hintW = Raylib.MeasureText(hint, hintSize);
        Raylib.DrawText(hint, (sw - hintW) / 2, sh / 2 + 20, hintSize, Color.White);
    }
}