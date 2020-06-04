using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace Snake
{
    struct Position
    {
        public int Row;
        public int Col;
        public Position(int x, int y)
        {
            this.Row = x;
            this.Col = y;
        }
    }
    class Program
    {
        static void DrawSnake (Queue<Position> toDraw,int direct)
        {
            foreach (Position element in toDraw)
            {
                Console.SetCursorPosition(element.Col, element.Row);
                Console.ForegroundColor = bodyColour;
                Console.Write(snakeBody);
            }
            Position head = toDraw.Last();
            Console.SetCursorPosition(head.Col, head.Row);
            if (direct == 0 && useSpecHead) snakeHead = '>';
            if (direct == 1 && useSpecHead) snakeHead = '<';
            if (direct == 2 && useSpecHead) snakeHead = 'v';
            if (direct == 3 && useSpecHead) snakeHead = '^';
            Console.ForegroundColor = headColour;
            Console.Write(snakeHead);
        }
        static void ClearSnake(Queue<Position> toClear)
        {
            //foreach (Position element in toClear)
            //{
            //    Console.SetCursorPosition(element.Col, element.Row);
            //    Console.Write(" ");
            //}
            Console.SetCursorPosition(toClear.First().Col, toClear.First().Row); // clear only tail
            Console.Write(" ");
        }
        static int NewDirection (ConsoleKeyInfo key, int oldDirection)
        {
            int newDirection;
            if (key.Key == ConsoleKey.RightArrow && oldDirection != 1)
            {
                newDirection = 0;
            }
            else if (key.Key == ConsoleKey.LeftArrow && oldDirection != 0)
            {
                newDirection = 1;
            }
            else if (key.Key == ConsoleKey.DownArrow && oldDirection != 3)
            {
                newDirection = 2;
            }
            else if (key.Key == ConsoleKey.UpArrow && oldDirection != 2)
            {
                newDirection = 3;
            }
            else newDirection = oldDirection;
            return newDirection;
        }
        static Position FoodGenerator ()
        {
            Random rand = new Random();
            Position newfood = new Position(rand.Next(0, Console.WindowHeight - 1), rand.Next(0, Console.WindowWidth));
            return newfood;
        }
        static void DrawFood(Position foodToDraw)
        {
            Console.SetCursorPosition(foodToDraw.Col, foodToDraw.Row);
            Console.ForegroundColor = foodColour;
            Console.Write(food);
        }
        static bool CheckDead(Position toCheck,Queue<Position> curSnake)
        {
            bool dead = false;
            if(curSnake.Contains(toCheck))
            {
                dead = true;
            }
            if(toCheck.Row < 0 || toCheck.Row > Console.WindowHeight-1 
               || toCheck.Col < 0 || toCheck.Col > Console.WindowWidth-1)
            {
                dead = true;
            }
            return dead;
        }
        static bool CheckExpiration (int start, int expTime, bool exp)
        {
            if (exp==false)
            {
                return false;
            }
            bool expired = false;
            if(Environment.TickCount > start+expTime)
            {
                expired = true;
            }
            return expired;
        }

        public static ConsoleColor bodyColour = ConsoleColor.Green;
        public static ConsoleColor headColour = ConsoleColor.Red;
        public static ConsoleColor foodColour = ConsoleColor.Yellow;
        public static ConsoleColor defaultColour = ConsoleColor.White;
        public static char snakeBody = '#';
        public static char snakeHead = '@';
        public static bool useSpecHead = true;
        public static char food = '*';
        public static int snakeLenght = 4;
        public static int pointsPerEat = 10;
        public static int penaltyPoints = 10;
        public static int points = 0;
        public static bool foodExpires = false;
        public static int expirationTime = 10000;
        public static int delay = 100;
        public static int accelerate = -10; //delay decrease
        public static int maxSpeed = 100; //min delay
        static void Main(string[] args)
        {
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
            // Initual snake
            Queue<Position> snake = new Queue<Position>();       
            for (int i = 0; i <= snakeLenght; i++)
            {
                snake.Enqueue(new Position(0, i));
            }

            // User input
            Position[] directions = new Position[]
            {
                new Position(0,1), //Right
                new Position(0,-1), //Left
                new Position(1,0), //Down
                new Position(-1,0) //Up
            };
            int direction = 0;
            DrawSnake(snake,direction);
            Position newFood = FoodGenerator();
            int curTick = Environment.TickCount;
            while (snake.Contains(newFood))
            {
                newFood = FoodGenerator();
                curTick = Environment.TickCount;
            }
            DrawFood(newFood);
            while (true)
            {
                //Read user
                if (Console.KeyAvailable)
                { 
                    direction = NewDirection(Console.ReadKey(),direction);
                }
                ClearSnake(snake);                
                Position nextDidection = (directions[direction]);
                Position head = snake.Last();
                Position newHead = new Position(head.Row + nextDidection.Row, head.Col + nextDidection.Col);
                if(CheckDead(newHead,snake))
                {
                    break;
                }
                snake.Enqueue(newHead);
                
                if (newHead.Row == newFood.Row && newHead.Col == newFood.Col||CheckExpiration(curTick,expirationTime,foodExpires))
                {
                    if (newHead.Row == newFood.Row && newHead.Col == newFood.Col)
                    {
                        points += pointsPerEat;
                    }
                    if (CheckExpiration(curTick, expirationTime, foodExpires))
                    {
                        snake.Dequeue();
                        points -= penaltyPoints;
                    }
                    Console.SetCursorPosition(newFood.Col, newFood.Row);
                    Console.Write(" ");
                    while (true)
                    {
                        newFood = FoodGenerator();
                        curTick = Environment.TickCount;
                        if (snake.Contains(newFood) || (newHead.Row == newFood.Row && newHead.Col == newFood.Col))
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (delay>maxSpeed)
                    {
                        delay = delay + accelerate;
                    }
                }
                else
                {
                    snake.Dequeue();
                }
                DrawFood(newFood);
                DrawSnake(snake,direction);
                Thread.Sleep(delay);
                

            }
            Console.Clear();
            Console.ForegroundColor = defaultColour;
            Console.SetCursorPosition(Console.WindowWidth/2 - 4, Console.WindowHeight/2);
            Console.WriteLine("Game Over");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 4, Console.WindowHeight / 2 + 1);
            if (points < 0) points = 0;
            Console.WriteLine("Points: {0}",points);
            Console.ReadLine();
        }
    }
}
