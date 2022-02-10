using System;
using System.Collections.Generic;
// For the console
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
// For the ArrayList

// For the timer
using System.Windows.Threading;

namespace Start_Game
{
    // A partial class allows the fuctionality of a single class into mutiple files
    // A tutroial on dispatch timer is https://youtu.be/TI9eXnXpxJ8
    // massive help on movement from https://www.bing.com/videos/search?q=wpf+player+movement&docid=608009494919404810&mid=943283314C6E7F1785F4943283314C6E7F1785F4&view=detail&FORM=VIRE
    // space game tutroial is life saver https://www.mooict.com/wpf-c-tutorial-create-a-space-battle-shooter-game-in-visual-studio/3/

    public partial class MainWindow : Window
    {
        // Player Speed
        double playerSpeed = 5;

        // Enemy Speed
        double enemySpeed = 0.02;

        // Create a list for items that will be removed
        // These are only for rectangles!!!
        List<Rectangle> itemstoremove = new List<Rectangle>();

        // Add Timer for the game -> FrameRate
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        // Total Map, with 100 different rooms
        // this is a total life saver https://stackoverflow.com/questions/13601151/dictionary-of-lists 
        Dictionary<int, List<int>> totalMap = new Dictionary<int, List<int>>(100);


        // Player Hitbox as a rectangle
        Rect PlayerHitbox;

        // make a new random class to generate random numbers from
        Random rand = new Random();

        // If this was created
        bool weaponCreated = false;

        // Constant Size
        const int objectSize = 40;


        // Dirrection Moving
        enum playerDirrection
        {
            up,
            down,
            left,
            right
        }

        playerDirrection dirrection;


        // Get the Rooms
        int startRoom;
        int exitRoom;
        int lootRoom;

        int playerIsInRoom;

        List<int> currentRoom = new List<int>()
        {
            1, 1, 1, 1, 0, 0, 1, 1, 1, 1,
            1, 0, 0, 0, 3, 3, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            0, 3, 0, 0, 0, 0, 0, 0, 3, 0,
            0, 3, 0, 0, 0, 0, 0, 0, 3, 0,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 3, 3, 0, 0, 0, 1,
            1, 1, 1, 1, 0, 0, 1, 1, 1, 1
        };

        // if moving into another room
        bool nextRoom = false;

        // Enemies
        int enemyCounter = 2;


        // ANIMATION!!!

        // Frames
        int frame = 0;

        // Sword Animation Time
        int swordAnimation = 150;

        // Initial Dirrection
        playerDirrection initialDirrection;


        public MainWindow() // This function will load first and therefore be 
        {
            InitializeComponent();

            for (int i = 0; i < 100; i++)
            {
                totalMap[i] = new List<int>() { 0, 1, 2 };
            }


            // ORDER OF INITIATION

            // STEP 1
            // Select the Map

            int map = selectMap(1); // supposed to be random number given, but for now, there's only going to be 1 map

            // STEP 2
            // Set up the Map

            makeMap(map);

            // Step 3
            // Get Current Room

            getRoom(map);


            // Step 5
            // Begin the Game

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1); // running the timer every 20 milliseconds
            dispatcherTimer.Tick += new EventHandler(GameTimerEvent); // linking the timer event
            dispatcherTimer.Start(); // starting the timer

            PlayerSpace.Focus(); // this is what will be mainly focused on for the program


            // make an image for the player using an image brush
            // an image brush is a type of tile brush that define it's content as an image by it's Image Source proprety
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imagebrush?view=windowsdesktop-6.0
            ImageBrush playerImage = new ImageBrush();

            // Load tha Player into it
            // this is a specialized bitmap source using xaml to loading images
            playerImage.ImageSource = new BitmapImage(new Uri("C:/Users/peter/source/repos/Start_Game/Start_Game/images/1.png"));

            // asign the player the rect
            // filling the timage with the source
            PlayerCharacter.Fill = playerImage;

        }

        // Character Movement
        private void GameTimerEvent(object? sender, EventArgs e) // This is what's getting affected by the ticks
        {

            // Link the hitbox to the drawn rectangle
            // This new rect is created by (get the object, the loction, width, height)
            PlayerHitbox = new Rect(Canvas.GetLeft(PlayerCharacter), Canvas.GetTop(PlayerCharacter), PlayerCharacter.Width, PlayerCharacter.Height);

            nextRoom = false;

            // Player Movement!


            // GOING LEFT
            if (Keyboard.IsKeyDown(Key.Left) && Canvas.GetLeft(PlayerCharacter) > objectSize) // If left key is presssed, and if the key is 5 away from the border
            {
                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) - playerSpeed); // get the object we want to move then the X value (Set Left). Since we are going left we are going to subteract that X value by how fast the player is moving

                dirrection = playerDirrection.left;


            }
            else if (Canvas.GetLeft(PlayerCharacter) == objectSize) // If touch the boder / exit to another tom
            {

                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) + 300);

                playerIsInRoom--; // going to the left



                nextRoom = true;


            }

            // GOING UP
            if (Keyboard.IsKeyDown(Key.Up) && Canvas.GetTop(PlayerCharacter) > 0) // if up key is pressed and is 5 away from the top
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) - playerSpeed);

                dirrection = playerDirrection.up;

            }
            else if (Canvas.GetTop(PlayerCharacter) == 0)
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) + 350);

                playerIsInRoom += 10; // Going up by one room

                nextRoom = true;
            }


            // GOING DOWN
            if (Keyboard.IsKeyDown(Key.Down) && Canvas.GetTop(PlayerCharacter) + (350) < Application.Current.MainWindow.Height) // if down key is pressed and if the location of the player character + it's height is away from the bottom
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) + playerSpeed);

                dirrection = playerDirrection.down;

            }
            else if (Canvas.GetTop(PlayerCharacter) + (350) == Application.Current.MainWindow.Height)
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) - 350);

                playerIsInRoom -= 10; // Going down by one room

                nextRoom = true;
            }


            // GOING RIGHT

            if (Keyboard.IsKeyDown(Key.Right) && Canvas.GetLeft(PlayerCharacter) + 300 < Application.Current.MainWindow.Width) // basically get playter character and it's current width, comparing it to the border which uses the current mainwindow width
            {
                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) + playerSpeed);

                dirrection = playerDirrection.right;

            }
            else if (Canvas.GetLeft(PlayerCharacter) + 300 == Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) - 350);

                playerIsInRoom++; // going o the right

                nextRoom = true;
            }

            // Create Object when pressed
            if (Keyboard.IsKeyDown(Key.Space) && weaponCreated == false)
            {

                initialDirrection = dirrection;

                int weaponHeight = 0;
                int weaponWidth = 0;
                double weaponPosition = 0;

                ImageBrush weaponImage = new ImageBrush();



                if (initialDirrection == playerDirrection.up || initialDirrection == playerDirrection.down)
                {
                    weaponHeight = 40;
                    weaponWidth = 15;
                    weaponPosition = 4;

                }
                else if (initialDirrection == playerDirrection.left || initialDirrection == playerDirrection.right)
                {
                    weaponHeight = 15;
                    weaponWidth = 40;
                    weaponPosition = 4.5;
                }

                weaponImage.ImageSource = new BitmapImage(new Uri($"C:/Users/peter/source/repos/Start_Game/Start_Game/images/{weaponPosition}.png"));

                Rectangle newSword = new Rectangle
                {
                    Tag = "weapon-sword-phys",
                    Height = weaponHeight,
                    Width = weaponWidth,
                    Fill = weaponImage,
                    //Stroke = Brushes.Black
                };

                PlayerSpace.Children.Add(newSword);


                // Add the sword to the screen 

                weaponCreated = true;

            }




            // Search for players and walls

            // foreach searches for all the children objects that are attached to the playerspace (canvas)
            foreach (var x in PlayerSpace.Children.OfType<Rectangle>())
            {

                // if an rectangle has the tag 'wall' on it
                // x is Rectangle -> this is like this since it's a class, it can't be = like a string or num
                // (string)x -> this is like this since it's searching for a tag, which is a string and the var must be declared as such
                if (x is Rectangle && (string)x.Tag == "wall")
                {

                    // make a new wall rect for enemy hit box
                    Rect wall = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    // if the player hit box and the enemy is colliding 
                    if (PlayerHitbox.IntersectsWith(wall))
                    {
                        DisplayDateTextBlock.Text = "HIT!!!";


                        // HITBOXES AND STOP FROM GOING INTO WALLS!!!



                        if (dirrection == playerDirrection.up) // if going up, push down at the same speed
                        {
                            Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) + playerSpeed);
                        }
                        else if (dirrection == playerDirrection.down) // if going down, push up, push up at the same speed
                        {
                            Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) - playerSpeed);
                        }
                        else if (dirrection == playerDirrection.left) // if going left, push right, push right at the same speed
                        {
                            Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) + playerSpeed);
                        }
                        else if (dirrection == playerDirrection.right) // if going right, push left, push left at the same speed
                        {
                            Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) - playerSpeed);
                        }




                        // itemstoremove.Add(x); // remove item



                    }

                    // If interact with coin

                    foreach (var c in PlayerSpace.Children.OfType<Rectangle>())
                    {
                        Rect coin = new Rect(Canvas.GetLeft(c), Canvas.GetTop(c), c.Width, c.Height);

                        if (c is Rectangle && (string)c.Tag == "coin")
                        {
                            if (PlayerHitbox.IntersectsWith(coin))
                            {
                                saveRoom(c);

                                itemstoremove.Add(c);
                            }

                            if (nextRoom)
                            {
                                itemstoremove.Add(c);
                            }
                        }
                    }

                    // If weapon is in existanse
                    foreach (var z in PlayerSpace.Children.OfType<Rectangle>())
                    {


                        // If weapon exists and is a rectangle and is this spefic weapon
                        if (z is Rectangle && (string)z.Tag == "weapon-sword-phys" && weaponCreated)
                        {

                            // Follow the Player Perfectly

                            var rt = new RotateTransform();

                            if (initialDirrection == playerDirrection.up)
                            {
                                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) - z.Height));

                                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width / 2) - z.Width / 2);
                            }
                            else if (initialDirrection == playerDirrection.down)
                            {
                                rt.Angle = 180;


                                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Height));

                                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width / 2) - z.Width / 2);


                            }
                            else if (initialDirrection == playerDirrection.left)
                            {

                                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Height / 4));

                                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) - z.Width));

                            }
                            else if (initialDirrection == playerDirrection.right)
                            {
                                rt.Angle = 180;



                                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Width / 4));

                                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width));
                            }

                            z.RenderTransform = rt;
                            z.RenderTransformOrigin = new Point(0.5, 0.5);




                            // ANIMATION!!!!!

                            // Start to count the ticks, + 1 frame every 1 tick
                            frame++;

                            // If the frames go as long as how long the sword is supposed to last
                            if (frame == swordAnimation)
                            {

                                // Remove Sword
                                itemstoremove.Add(z);

                                // Return frames to 0
                                frame = 0;

                                // Weapon is no longer created now
                                weaponCreated = false;
                            }
                        }
                    }

                    foreach (var y in PlayerSpace.Children.OfType<Rectangle>())
                    {
                        // See if the player is interacting with the enemy
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {


                            if (Canvas.GetTop(PlayerCharacter) < Canvas.GetTop(y)) // if player is above, follow
                            {
                                Canvas.SetTop(y, Canvas.GetTop(y) - enemySpeed);

                            }
                            else if (Canvas.GetTop(PlayerCharacter) > Canvas.GetTop(y))
                            {
                                Canvas.SetTop(y, Canvas.GetTop(y) + enemySpeed);
                            }

                            if (Canvas.GetLeft(PlayerCharacter) < Canvas.GetLeft(y))
                            {
                                Canvas.SetLeft(y, Canvas.GetLeft(y) - enemySpeed);
                            }
                            else if (Canvas.GetLeft(PlayerCharacter) > Canvas.GetTop(y))
                            {
                                Canvas.SetLeft(y, Canvas.GetLeft(y) + enemySpeed);
                            }




                            Rect enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (PlayerHitbox.IntersectsWith(enemy))
                            {
                                dispatcherTimer.Stop();
                                DisplayDateTextBlock.Text = "ZOMBIE ATTACK!!! YOU ARE DEAD!!!";
                            }

                            foreach (var z in PlayerSpace.Children.OfType<Rectangle>())
                            {

                                Rect sword = new Rect(Canvas.GetLeft(z), Canvas.GetTop(z), z.Width, z.Height);

                                if (z is Rectangle && (string)z.Tag == "weapon-sword-phys")
                                {


                                    if (enemy.IntersectsWith(sword))
                                    {
                                        itemstoremove.Add(y);




                                    }


                                }





                            }





                            if (nextRoom)
                            {
                                itemstoremove.Add(y);
                            }

                        }



                    }









                    // Remove everything if going to the next room
                    if (nextRoom)
                    {
                        itemstoremove.Add(x);

                    }



                }
                



            }

            // removing the rectangles

            // check how many rectangles are inside of the item to remove list
            foreach (Rectangle x in itemstoremove)
            {
                // remove them permanently from the canvas
                PlayerSpace.Children.Remove(x);

            }


            // if going into next room is true, then redo the graphics based on the level
            // However, as seen bellow, the getRoom() has a 0 in it, meaning it will just be the tutorial for now
            if (nextRoom)
            {
                getRoom(0);
            }



        }



        private void makeWall(int row, int col, string blockType)
        {

            // Image Brush
            ImageBrush wallImage = new ImageBrush();

            wallImage.ImageSource = new BitmapImage(new Uri("C:/Users/peter/source/repos/Start_Game/Start_Game/images/2.png"));

            Rectangle newWall = new Rectangle
            {
                Tag = "wall",
                Height = objectSize,
                Width = objectSize,
                Fill = wallImage
            };

            Canvas.SetLeft(newWall, (row * objectSize));
            Canvas.SetTop(newWall, (col * objectSize));


            PlayerSpace.Children.Add(newWall);


            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game
        }

        private void makeEnemy(int row, int col, string enemyType)
        {
            // Image Brush
            ImageBrush enemyImage = new ImageBrush();

            enemyImage.ImageSource = new BitmapImage(new Uri("C:/Users/peter/source/repos/Start_Game/Start_Game/images/3.png"));

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 20,
                Width = 20,
                Fill = enemyImage
            };

            Canvas.SetLeft(newEnemy, row * objectSize);
            Canvas.SetTop(newEnemy, col * objectSize);


            PlayerSpace.Children.Add(newEnemy);

            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game
        }

        private void makeItem(int row, int col, string itemType)
        {
            // Image Brush
            ImageBrush itemImage = new ImageBrush();

            itemImage.ImageSource = new BitmapImage(new Uri("C:/Users/peter/source/repos/Start_Game/Start_Game/images/5.png"));

            Rectangle newItem = new Rectangle
            {
                Tag = itemType,
                Height = 20,
                Width = 20,
                Fill = itemImage
            };


            Canvas.SetLeft(newItem, row * objectSize);
            Canvas.SetTop(newItem, col * objectSize);


            PlayerSpace.Children.Add(newItem);

            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game

        }

        private void makeText(string innerText)
        {

            DisplayDateTextBlock2.Text += $"{innerText} \n";
            scroll.ScrollToEnd();
        }


        // STEP 1
        // Select which map is going to be made
        private int selectMap(int mapSelect)
        {

            return 0; // be random later, but now just the tutorial will be used
        }

        // STEP 2
        // Make the Map
        // THIS WILL BE LARRRRRGEEE!!!
        // Maybe I should get this from a text file...
        private void makeMap(int getMap)
        {
            if (getMap == 0)
            {
                startRoom = 56;
                exitRoom = 76;
                lootRoom = 64;
            }

            playerIsInRoom = startRoom;


            // INITIAIZE ALL THE CODE INTO THE DICTIONARY

            for (int i = 0; i < 100; i++)
            {

                if (i == 56) // Starting Room
                {
                    List<int> setUpMap = new List<int>()
                    {
                        1, 1, 1, 1, 0, 0, 1, 1, 1, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 5, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 1, 1, 1, 0, 0, 1, 1, 1, 1
                    };

                    totalMap[i] = setUpMap;

                } else if (i == 55) // left room
                {
                    List<int> setUpMap = new List<int>()
                    {
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 3, 0, 5, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 3, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                    };

                    totalMap[i] = setUpMap;
                } else if (i == 57) // right room
                {
                    List<int> setUpMap = new List<int>()
                    {
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                    };

                    totalMap[i] = setUpMap;
                }
                else if (i == 66) // up room
                {
                    List<int> setUpMap = new List<int>()
                    {
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 4, 4, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 1, 1, 1, 0, 0, 1, 1, 1, 1
                    };

                    totalMap[i] = setUpMap;
                }
                else if (i == 46) // down room
                {
                    List<int> setUpMap = new List<int>()
                    {
                        1, 1, 1, 1, 0, 0, 1, 1, 1, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                    };

                    totalMap[i] = setUpMap;
                }





                currentRoom = totalMap[56];
            }


        }

        //STEP 3
        // GET THE ROOM BASED ON POSITION IN TOTAL MAP
        private void getRoom(int getMap)
        {

            currentRoom = totalMap[playerIsInRoom];

            makeSurrondings();

        }


        private void makeSurrondings()
        {

            // Create map. This will be used as the basis for the place where the player moves
            List<int> room = new List<int>();

            room = currentRoom;

            // extra split

            int col = 0;
            int row = 0;
            int section = 10;
            int generateRoom = 0;

            // Creating a map based on an array
            while (generateRoom < 100)
            {

                if (generateRoom < section) // If this part of the array is bellow the length of the map (10)
                {
                    if (room[generateRoom] == 1) // If the given part of the map has a 1
                    {
                        // Increase the row
                        row++;

                        // Get Function
                        makeWall(row, col, "block");


                    }
                    else if (room[generateRoom] == 2) // generate a door, unused for now
                    {
                        row++;

                        // Get Function
                        makeWall(row, col, "door");
                    }
                    else if (room[generateRoom] == 3) // generate an enemy
                    {
                        row++;

                        // Get Function
                        makeEnemy(row, col, "zombie");
                    }
                    else if (room[generateRoom] == 4) // generate an enemy
                    {
                        row++;

                        // Get Function
                        makeItem(row, col, "coin");
                    }
                    else if (room[generateRoom] == 5) // generate a textbox
                    {
                        row++;

                        string text = "";
                        
                        if (playerIsInRoom == 56)
                        {
                            text = "Welcome to the tutorial! Use the arrow keys to move! Use the spacebar to attack!";
                        } else if (playerIsInRoom == 55)
                        {
                            text = "Enemies endlessly respawn from set points! If you have a [holy relic] you can stop the spawning but only in that room";
                        }
  
                        // Get Function
                        makeText(text);
                    }
                    else
                    {
                        row++; // If this part of the array is 0
                    }

                    generateRoom++; // Get to the next part of the array

                }
                else
                {
                    // When reading the array goes farther than the length of the map (10) switch to a new row
                    row = 0;
                    col++;
                    section += 10;
                }

            }




        }

        private void saveRoom(Rectangle c)
        {
            // Create map. This will be used as the basis for the place where the player moves
            List<int> room = new List<int>();

            room = currentRoom;

            // extra split

            int col = 0;
            int row = 0;
            int section = 2;
            int generateRoom = 0;

            int checkForItemX = 0;

            // Creating a map based on an array
            while (generateRoom < 10)
            {



                row++;



                checkForItemX = objectSize * row;

                if (Canvas.GetLeft(c) == checkForItemX)
                {
                    currentRoom[row + (Convert.ToInt32(Canvas.GetTop(c)) / 4) - 1] = 0;

                    // should get currentRoom[27]

                }




                generateRoom++;






            }


        }


    }
}


// Create Grid



/*

Rectangle wall = new Rectangle()
{
    Tag = "wall",
    Width=50,
    Height=50,
    Stroke = System.Windows.Media.Brushes.Black
};


Canvas.SetTop(wall, 100);
Canvas.SetLeft(wall, 100);

PlayerSpace.Children.Add(wall);

// extra split
for (int row = 0; row < 10; row++)
{
    for (int col = 0; col < 10; col++)
    {
        Rectangle rectangle = new Rectangle()
        {
            Name = $"cell{row}-{col}",
            Height = x,
            Width = y,
            Stroke = System.Windows.Media.Brushes.Black
        };
        Canvas.SetLeft(rectangle, (row * x));
        Canvas.SetTop(rectangle, (col * y));

        PlayerSpace.Children.Add(rectangle);
    }
}

}

*/
