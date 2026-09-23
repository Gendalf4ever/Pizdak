using Raylib_cs;
using System.Numerics;

class Program
{
    const int MapWidth = 24;
    const int MapHeight = 24;
    //const int TileSize = 20;

     // 1 — стена, 0 — пусто
    static int[,] map = new int[MapWidth, MapHeight]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    };
    //далее добавлю массив с другими картами
    //enemy

    class Enemy
    {
        public float X;
        public float Y;
        public bool Alive = true;        
    }
    static void Main()
    {
        Raylib.InitWindow(1280,720, "Game");
        Raylib.SetTargetFPS(60);
        Raylib.DisableCursor();

        float posX = 12.0f;
        float posY = 12.0f;

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
        int screenHeight = Raylib.GetScreenHeight();
        int halfHeight = screenHeight / 2;
        //z-buffer

        float[] zBuffer = new float[screenWidth];

        List<Enemy> enemies = new List<Enemy>();
        enemies.Add(new Enemy { X = 7.5f,  Y = 7.5f  });
        enemies.Add(new Enemy { X = 12.5f, Y = 4.5f  });
        enemies.Add(new Enemy { X = 18.5f, Y = 15.5f });
        
        while(!Raylib.WindowShouldClose())
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

            if (map[(int)newX, (int)posY] == 0) posX = newX;
            if (map[(int)posX, (int)newY] == 0) posY = newY;
            //shooting
            if(Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                TryShoot(posX, posY, dirX, dirY, planeX, planeY, zBuffer[screenWidth / 2], enemies);
                shotTimer = shotDuration;
            }
            if (shotTimer > 0f)
            {
                shotTimer -= Raylib.GetFrameTime();
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
                    if (map[mapX, mapY] ==1) break;
                }

                float perpWallDist;
                if (side == 0) perpWallDist = sideDistX - deltaDistX;
                else perpWallDist = sideDistY - deltaDistY;
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
            //Minimap
            DrawMinimap(posX, posY, dirX, dirY);

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

            Raylib.EndDrawing();
        }
            Raylib.CloseWindow();
         //draw enemies function
           
    }
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

                    Raylib.DrawRectangle(x, drawStartY, 1, drawEndY - drawStartY, Color.Red);
                }
            }
        }
        //try shoot function
            static void TryShoot(float posX, float posY, float dirX, float dirY, float planeX, float planeY, float wallDist, List<Enemy> enemies)
        {
            float invDet = 1.0f / (planeX * dirY - dirX * planeY);
            Enemy closest = null;
            float closestDist = float.MaxValue;

            foreach(var e in enemies)
            {
                if (!e.Alive) continue;
                float dx = e.X - posX;
                float dy = e.Y - posY;

                float transformX = invDet * (dirY * dx - dirX * dy);
                float transformY = invDet * (-planeY * dx + planeX * dy);

                if (transformY <=0 ) continue; //behind the player
                if (transformY >= wallDist) continue; //behind the wall
                if (Math.Abs(transformX) > 0.4f) continue; //not in crosshair

                if (transformY < closestDist)
                {
                    closestDist = transformY;
                    closest = e;
                }
            }

            if (closest != null)
            {
                closest.Alive = false;
                Console.WriteLine("Hit detected");
            }

        }
         //minimap function
         static void DrawMinimap(float posX, float posY, float dirX, float dirY)
        {
            int scale = 4;
            int offsetX = 10;
            int offsetY = 10;

            for (int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    Color c = map[x,y] == 1 ? Color.Gray : Color.DarkGray;
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
}