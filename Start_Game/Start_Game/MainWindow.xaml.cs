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



// MAIN GOAL:
// Polish Menu
// Set bars to health and mp
// take damage and invisble frames




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

        List<double> playerStats = new List<double>()
        {
            // image, Health, MP, Phys, Magic, Gun, Phys Def, Mag Def, Speed, mp regen, Size
            0, 100, 100, 10, 10, 10, 10, 10, 5, 10, 25
        };

        int currentStat = 0;

        // Player list
        Dictionary<double, List<double>> totalPlayers = new Dictionary<double, List<double>>(4);

        int playerPosition = 0;


        List<double> enemyStats = new List<double>()
        {
            // Enemy Speed
            0.5
        };

        // Enemy Speed
        double enemySpeed = 0.02;

        // Create a list for items that will be removed
        // These are only for rectangles!!!
        List<Rectangle> itemstoremove = new List<Rectangle>();

        // Add Timer for the game -> FrameRate
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        // Map Size
        const int mapSize = 100;

        // Total Map, with 100 different rooms
        // this is a total life saver https://stackoverflow.com/questions/13601151/dictionary-of-lists 
        Dictionary<int, List<int>> totalMap = new Dictionary<int, List<int>>(mapSize);


        // Make Stats
        showStats DrawTheStats = new showStats();


        // Player Hitbox as a rectangle
        Rect PlayerHitbox;

        // make a new random class to generate random numbers from
        Random rand = new Random();

        // If this was created
        bool weaponCreated = false;

        // Constant Size
        const int objectSize = 40;

        // Dirrection Moving

        List<string> playerDirrection = new List<string>()
        {
            "up",
            "down",
            "left",
            "right"
        };


        // Get the Rooms
        int startRoom;

        int playerIsInRoom;

        List<int> currentRoom = new List<int>()
        {
            1, 1, 1, 1, 0, 0, 1, 1, 1, 1,
            1, 0, 0, 0, 1, 1, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            0, 1, 0, 0, 0, 0, 0, 0, 1, 0,
            0, 1, 0, 0, 0, 0, 0, 0, 1, 0,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
            1, 0, 0, 0, 1, 1, 0, 0, 0, 1,
            1, 1, 1, 1, 0, 0, 1, 1, 1, 1
        };

        // if moving into another room
        bool nextRoom = false;

        // Enemies
        // int enemyCounter = 2;


        // ANIMATION!!!

        // Frames
        int frame = 0;

        // Sword Animation Time
        int swordAnimation = 5;

        // Initial Dirrection
        string currentDirrection = "up";
        string oldDirrection = "up";


        // TOTAL MAP
        int map;


        public MainWindow() // This function will load first
        {
            InitializeComponent();

            DrawTheStats.displayStats(HpStat, MpStat, PhysStat, MagStat, GunStat, PhysDefStat, MagDefStat, SpeedStat, MpRegenStat, SizeStat, playerStats);
        }

        private void StartTheGame()
        {
            // ORDER OF INITIATION
            drawSetting DrawTheSetting = new drawSetting();
            basicMapMake MakingTheMap = new basicMapMake();
            makeObjects MakingTheObjects = new makeObjects();
            setUpRooms SettingUpTheRooms = new setUpRooms();
            checkForInteraction CheckingForInterations = new checkForInteraction();

            // Step 0
            // Add lists to dictionary
            MakingTheMap.addLists(mapSize, totalMap);

            // STEP 1
            // Select the Map
            map = MakingTheMap.selectMap(0); // supposed to be random number given, but for now, there's only going to be 1 map

            // STEP 2
            // Set up the Map
            // Object cannot be used yet since the map hasn't been fully implemented
            playerIsInRoom = MakingTheMap.makeMap(map, startRoom, currentRoom, totalMap, SettingUpTheRooms);

            // Step 3
            // Get Current Room
            currentRoom = totalMap[playerIsInRoom];

            // Step 4
            // Draw Current Room
            DrawTheSetting.makeSurrondings(currentRoom, map, playerIsInRoom, MakingTheObjects, PlayerSpace, objectSize, logBox, scroll, totalMap);

            // Step 5
            // Begin the Game
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1); // running the timer every 20 milliseconds
            dispatcherTimer.Tick += new EventHandler(GameTimerEvent); // linking the timer event
            dispatcherTimer.Start(); // starting the timer

            PlayerSpace.Focus(); // this is what will be mainly focused on for the program


            // Step 6
            // Fix Player Character
            MakingTheObjects.makePlayer(PlayerCharacter, "C:/Users/peter/source/repos/Start_Game/Start_Game/images/1.png");
        }

        // Character Movement
        private void GameTimerEvent(object? sender, EventArgs e) // This is what's getting affected by the ticks
        {

            // Link the hitbox to the drawn rectangle
            // This new rect is created by (get the object, the loction, width, height)
            PlayerHitbox = new Rect(Canvas.GetLeft(PlayerCharacter), Canvas.GetTop(PlayerCharacter), PlayerCharacter.Width, PlayerCharacter.Height);

            // Not going to next room
            nextRoom = false;



            // SET OBJECTS UP HERE



            // Player Movement!
            playerMovement MoveThePlayer = new playerMovement();

            // Map Stuff
            drawSetting DrawTheSetting = new drawSetting();
            makeObjects MakingTheObjects = new makeObjects();

            // Attacking stuff
            makeAttack MakingTheAttack = new makeAttack();

            // Saving the map
            saveClass SavingTheMap = new saveClass();

            // Interactions
            checkForInteraction CheckingForInterations = new checkForInteraction();
            typeInteractionChecker IfInteract = new typeInteractionChecker();
            animationMaker MakingAnimation = new animationMaker();



            // change the character imaged based on who is in control

            MakingTheObjects.getImageCharacter(totalPlayers, playerPosition, PlayerCharacter);

            MakingTheObjects.highlightCurrentPlayer(totalPlayers, playerPosition, Player1Name, Player2Name, Player3Name, Player4Name);


            // RUN THINGS HERE



            // Making Things


            // Check Which Character is Active
            playerPosition = MoveThePlayer.switchPlayers(playerPosition);

            // Run the Player Movement!
            currentDirrection = MoveThePlayer.movementChecker(totalPlayers, PlayerCharacter, objectSize, playerDirrection, playerIsInRoom, nextRoom, currentDirrection, playerPosition);

            // Get the Next Room
            nextRoom = MoveThePlayer.goNextRoom(PlayerCharacter, objectSize, nextRoom);

            // If go to next room
            playerIsInRoom += MoveThePlayer.movePlayerOnMap(PlayerCharacter, objectSize);

            // Old dirrection
            oldDirrection = MakingTheAttack.facing(currentDirrection, oldDirrection, currentDirrection, weaponCreated);

            // Weapon Create
            weaponCreated = MakingTheAttack.swordAttack(weaponCreated, currentDirrection, playerDirrection, PlayerSpace);


            // if swrd animaion is activating
            frame = MakingAnimation.swordAnimationLength(weaponCreated, frame);




            // Actually Running The Interactions

            CheckingForInterations.checkingWall(PlayerSpace, IfInteract, PlayerHitbox, currentDirrection, playerDirrection, PlayerCharacter, totalPlayers, playerPosition);
            CheckingForInterations.checkingItem(PlayerSpace, IfInteract, PlayerHitbox, SavingTheMap, itemstoremove, totalMap, playerIsInRoom, objectSize);

            weaponCreated = CheckingForInterations.checkingWeapon(PlayerSpace, IfInteract, weaponCreated, currentDirrection, playerDirrection, PlayerCharacter, MakingAnimation, frame, swordAnimation, itemstoremove, oldDirrection);

            CheckingForInterations.checkingEnemy(PlayerSpace, IfInteract, PlayerCharacter, enemyStats, PlayerHitbox, itemstoremove, dispatcherTimer, logBox);




            // removing the rectangles

            // check how many rectangles are inside of the item to remove list
            foreach (Rectangle x in itemstoremove)
            {
                // remove them permanently from the canvas
                PlayerSpace.Children.Remove(x);

            }


            // Remove every space on the canvas


            // if going into next room is true, then redo the graphics based on the level
            if (nextRoom)
            {
                foreach (Rectangle x in PlayerSpace.Children.OfType<Rectangle>())
                {
                    if (x is Rectangle && ((string)x.Tag != "player" && (string)x.Tag != "background"))
                    {
                        itemstoremove.Add(x);
                    }

                }

                DrawTheSetting.makeSurrondings(currentRoom, map, playerIsInRoom, MakingTheObjects, PlayerSpace, objectSize, logBox, scroll, totalMap);
            }

        }


        // Add Box
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            currentStat = DrawTheStats.addPlayer(playerStats, classSelect, DrawTheStats, totalPlayers, currentStat, PlayersBlock, HpStat, MpStat, PhysStat, MagStat, GunStat, PhysDefStat, MagDefStat, SpeedStat, MpRegenStat, SizeStat, Player1Name, Player2Name, Player3Name, Player4Name, GetName);


        }

        // Drop Box
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawTheStats.classDescription(classSelect, ClassDisplay);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TitleSpace.Children.Clear();

            StartTheGame();
        }

        /// <summary>
        ///  Start The Game
        /// </summary>
    }


    // Title
    class showStats
    {
        public void displayStats(TextBlock HpStat, TextBlock MpStat, TextBlock PhysStat, TextBlock MagStat, TextBlock GunStat, TextBlock PhysDefStat, TextBlock MagDefStat, TextBlock SpeedStat, TextBlock MpRegenStat, TextBlock SizeStat, List<double> playerStats)
        {
            HpStat.Text = playerStats[1].ToString();
            MpStat.Text = playerStats[2].ToString();
            PhysStat.Text = playerStats[3].ToString();
            MagStat.Text = playerStats[4].ToString();
            GunStat.Text = playerStats[5].ToString();
            PhysDefStat.Text = playerStats[6].ToString();
            MagDefStat.Text = playerStats[7].ToString();
            SpeedStat.Text = playerStats[8].ToString();
            MpRegenStat.Text = playerStats[9].ToString();
            SizeStat.Text = playerStats[10].ToString();
        }

        public void increaseStats(List<double> playerStats, int imageUsed, int IncreaseHealth, int IncreasedMP, int IncreasePhysical, int IncreaseMagic, int IncreaseGun, int IncreasePhysicalDefence, int IncreaseMagicalDefenceStat, int IncreaseSpeedStat, int IncreaseMpRegenStat, int IncreaseSizeStat)
        {
            // make list of stats
            List<int> currentStats = new List<int>()
            {
                imageUsed, IncreaseHealth, IncreasedMP, IncreasePhysical, IncreaseMagic, IncreaseGun, IncreasePhysicalDefence, IncreaseMagicalDefenceStat, IncreaseSpeedStat, IncreaseMpRegenStat, IncreaseSizeStat
            };

            for (int i = 0; i < currentStats.Count; i++)
            {
                playerStats[i] += currentStats[i];
            }


        }


        public int addPlayer(List<double> playerStats, ComboBox classSelect, showStats DrawTheStats, Dictionary<double, List<double>> totalPlayers, int currentStat, TextBlock PlayersBlock, TextBlock HpStat, TextBlock MpStat, TextBlock PhysStat, TextBlock MagStat, TextBlock GunStat, TextBlock PhysDefStat, TextBlock MagDefStat, TextBlock SpeedStat, TextBlock MpRegenStat, TextBlock SizeStat, TextBlock Player1Name, TextBlock Player2Name, TextBlock Player3Name, TextBlock Player4Name, TextBox GetName)
        {
            playerStats = new List<double> { 0, 100, 100, 10, 10, 10, 10, 10, 5, 10, 25 };

            string className = classSelect.SelectedItem.ToString();

            string currentName = "";

            // 0
            if (className == "System.Windows.Controls.ComboBoxItem: Knight")
            {
                //0, 1, 2, 3, 4, 5, 6, 7
                DrawTheStats.increaseStats(playerStats, 0, 0, 0, 5, 0, 0, 3, 0, -3, 0, 0);

                totalPlayers[currentStat] = playerStats;

                currentName = $"Knight";

            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Squire")
            {
                //0, 1, 2, 3, 4, 5, 6, 7
                DrawTheStats.increaseStats(playerStats, 1, 0, 0, -1, -1, -1, -1, -1, 3, 0, 0);

                totalPlayers[currentStat] = playerStats;

                currentName = $"Squire";
            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Paladin")
            {
                //0, 1, 2, 3, 4, 5, 6, 7
                DrawTheStats.increaseStats(playerStats, 2, 10, 0, 2, -5, -5, 10, 5, -2, 0, 5);

                totalPlayers[currentStat] = playerStats;

                currentName = $"Paladin";

            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Red Mage")
            {
                //0, 1, 2, 3, 4, 5, 6, 7
                DrawTheStats.increaseStats(playerStats, 3, 0, 20, -3, 6, -3, 0, 0, 0, 3, 0);

                totalPlayers[currentStat] = playerStats;

                currentName = $"Red_Mage";

            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Dark Mage")
            {
                //0, 1, 2, 3, 4, 5, 6, 7
                DrawTheStats.increaseStats(playerStats, 4, 0, 20, 3, 3, -3, -3, 0, 0, 3, 0);

                totalPlayers[currentStat] = playerStats;

                currentName = $"Dark_Mage";

            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Gunslinger")
            {
                //0, 1, 2, 3, 4, 5, 6, 7
                DrawTheStats.increaseStats(playerStats, 5, 0, -100, 5, -10, 10, 10, -10, 0, -10, 0);

                totalPlayers[currentStat] = playerStats;

                currentName = $"Gunslinger";

            }


            // Implement Names onto the other canvas bellow the title
            DrawTheStats.implementName(currentStat, Player1Name, Player2Name, Player3Name, Player4Name, currentName, GetName);

            // Draw The Stats
            DrawTheStats.displayStats(HpStat, MpStat, PhysStat, MagStat, GunStat, PhysDefStat, MagDefStat, SpeedStat, MpRegenStat, SizeStat, playerStats);




            currentStat += 1;

            return currentStat;
        }

        public void implementName(int currentStat, TextBlock Player1Name, TextBlock Player2Name, TextBlock Player3Name, TextBlock Player4Name, string currentName, TextBox GetName)
        {
            if (currentStat == 0)
            {
                Player1Name.Text = GetName.Text + " the " + currentName;
            }
            else if (currentStat == 1)
            {
                Player2Name.Text = GetName.Text + " the " + currentName;
            }
            else if (currentStat == 2)
            {
                Player3Name.Text = GetName.Text + " the " + currentName;
            }
            else if (currentStat == 3)
            {
                Player4Name.Text = GetName.Text + " the " + currentName;

            }
        }

        public void classDescription(ComboBox classSelect, TextBlock ClassDisplay)
        {
            string className = classSelect.SelectedItem.ToString();

            if (className == "System.Windows.Controls.ComboBoxItem: Knight") // Emblem: Sword
            {
                ClassDisplay.Text = "This being is one made of metalic fury. Very basic class.";
            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Squire") // Emblem: Dagger
            {
                ClassDisplay.Text = "One built from weakness and scared yet ambitious. High Speed, No Defence";
            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Paladin") // Emblem: Shield
            {
                ClassDisplay.Text = "One built from defence, yet low speed.";
            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Red Mage") // Emblem: Fire
            {
                ClassDisplay.Text = "One built from fury and destruction. High magic, low physical.";
            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Dark Mage") // Emblem: Swirl
            {
                ClassDisplay.Text = "They lurk in the dark yet are destroyed by it. Decent Magic and Decent phys, low defence.";
            }
            else if (className == "System.Windows.Controls.ComboBoxItem: Gunslinger") // Emblem: bullet
            {
                ClassDisplay.Text = "The ones who stop the moving on. High gun, High phys, low magic, low magic defence";
            }
        }
    }




    // The Main Engine, you could say
    class checkForInteraction
    {
        // Check for Walls
        public void checkingWall(Canvas PlayerSpace, typeInteractionChecker IfInteract, Rect PlayerHitbox, string currentDirrection, List<string> playerDirrection, Rectangle PlayerCharacter, Dictionary<double, List<double>> totalPlayers, int playerPosition)
        {

            // Checking for Walls
            foreach (var x in PlayerSpace.Children.OfType<Rectangle>())
            {

                if (x is Rectangle && (string)x.Tag == "wall")
                {
                    Rect wall = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    IfInteract.wallInteract(PlayerHitbox, wall, currentDirrection, playerDirrection, PlayerCharacter, totalPlayers, playerPosition);

                }

            }


        }

        public void checkingItem(Canvas PlayerSpace, typeInteractionChecker IfInteract, Rect PlayerHitbox, saveClass SavingTheMap, List<Rectangle> itemstoremove, Dictionary<int, List<int>> totalMap, int playerIsInRoom, int objectSize)
        {

            // Checking for items
            foreach (var c in PlayerSpace.Children.OfType<Rectangle>())
            {

                if (c is Rectangle && (string)c.Tag == "coin")
                {
                    Rect coin = new Rect(Canvas.GetLeft(c), Canvas.GetTop(c), c.Width, c.Height);
                    IfInteract.coinInteract(c, PlayerHitbox, coin, SavingTheMap, itemstoremove, totalMap, playerIsInRoom, objectSize);
                }

            }

        }

        public bool checkingWeapon(Canvas PlayerSpace, typeInteractionChecker IfInteract, bool weaponCreated, string currentDirrection, List<string> playerDirrection, Rectangle PlayerCharacter, animationMaker MakingAnimation, int frame, int swordAnimation, List<Rectangle> itemstoremove, string oldDirrection)
        {
            // Checking for Weapon
            foreach (var z in PlayerSpace.Children.OfType<Rectangle>())
            {

                if (z is Rectangle && (string)z.Tag == "weapon-sword-phys" && weaponCreated)
                {
                    Rect sword = new Rect(Canvas.GetLeft(z), Canvas.GetTop(z), z.Width, z.Height);

                    return weaponCreated = IfInteract.swordInteract(z, weaponCreated, currentDirrection, playerDirrection, PlayerCharacter, MakingAnimation, frame, swordAnimation, itemstoremove, oldDirrection);

                }



            }

            return weaponCreated;

        }

        public void checkingEnemy(Canvas PlayerSpace, typeInteractionChecker IfInteract, Rectangle PlayerCharacter, List<double> enemyStats, Rect PlayerHitbox, List<Rectangle> itemstoremove, DispatcherTimer dispatcherTimer, TextBlock logBox)
        {
            // Checking for enemy
            foreach (var y in PlayerSpace.Children.OfType<Rectangle>())
            {
                if (y is Rectangle && (string)y.Tag == "enemy")
                {
                    Rect enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);


                    IfInteract.enemyInteract(y, PlayerCharacter, enemyStats, PlayerHitbox, enemy, itemstoremove, dispatcherTimer, logBox, PlayerSpace);
                }

            }
        }
    }




    class typeInteractionChecker
    {
        public void wallInteract(Rect PlayerHitbox, Rect wall, string currentDirrection, List<string> playerDirrection, Rectangle PlayerCharacter, Dictionary<double, List<double>> totalPlayers, int playerPosition)
        {
            // if the player hit box and the enemy is colliding 
            if (PlayerHitbox.IntersectsWith(wall))
            {

                // HITBOXES AND STOP FROM GOING INTO WALLS!!!


                if (currentDirrection == playerDirrection[0]) // if going up, push down at the same speed
                {
                    Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) + totalPlayers[playerPosition][8]);
                }
                else if (currentDirrection == playerDirrection[1]) // if going down, push up, push up at the same speed
                {
                    Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) - totalPlayers[playerPosition][8]);
                }
                else if (currentDirrection == playerDirrection[2]) // if going left, push right, push right at the same speed
                {
                    Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) + totalPlayers[playerPosition][8]);
                }
                else if (currentDirrection == playerDirrection[3]) // if going right, push left, push left at the same speed
                {
                    Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) - totalPlayers[playerPosition][8]);
                }





            }
        }

        public void coinInteract(Rectangle c, Rect PlayerHitbox, Rect coin, saveClass SavingTheMap, List<Rectangle> itemstoremove, Dictionary<int, List<int>> totalMap, int playerIsInRoom, int objectSize)
        {

            if (PlayerHitbox.IntersectsWith(coin))
            {
                SavingTheMap.saveRoom(c, totalMap, playerIsInRoom, objectSize);
                itemstoremove.Add(c);
            }

        }


        public bool swordInteract(Rectangle z, bool weaponCreated, string currentDirrection, List<string> playerDirrection, Rectangle PlayerCharacter, animationMaker MakingAnimation, int frame, int swordAnimation, List<Rectangle> itemstoremove, string oldDirrection)
        {

            if (oldDirrection == "nothing")
            {
                oldDirrection = currentDirrection;
            }

            // Follow the Player Perfectly

            var rt = new RotateTransform();

            if (oldDirrection == playerDirrection[0]) //up
            {

                rt.Angle = 0;

                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) - z.Height));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width / 2) - z.Width / 2);

                return MakingAnimation.swordAnimation(weaponCreated, z, frame, swordAnimation, itemstoremove);
            }
            else if (oldDirrection == playerDirrection[1]) //down
            {

                rt.Angle = 180;

                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Height));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width / 2) - z.Width / 2);

                return MakingAnimation.swordAnimation(weaponCreated, z, frame, swordAnimation, itemstoremove);


            }

            if (oldDirrection == playerDirrection[2]) // left
            {

                rt.Angle = 0;

                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Height / 4));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) - z.Width));

                return MakingAnimation.swordAnimation(weaponCreated, z, frame, swordAnimation, itemstoremove);

            }
            else if (oldDirrection == playerDirrection[3]) // left
            {
                rt.Angle = 180;


                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Width / 4));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width));

                return MakingAnimation.swordAnimation(weaponCreated, z, frame, swordAnimation, itemstoremove);
            }

            return MakingAnimation.swordAnimation(weaponCreated, z, frame, swordAnimation, itemstoremove);



        }


        public void enemyInteract(Rectangle y, Rectangle PlayerCharacter, List<double> enemyStats, Rect PlayerHitbox, Rect enemy, List<Rectangle> itemstoremove, DispatcherTimer dispatcherTimer, TextBlock logBox, Canvas PlayerSpace)
        {
            // See if the player is interacting with the enemy



            if (Canvas.GetTop(PlayerCharacter) < Canvas.GetTop(y)) // if player is above, follow
            {
                Canvas.SetTop(y, Canvas.GetTop(y) - enemyStats[0]);

            }
            else if (Canvas.GetTop(PlayerCharacter) > Canvas.GetTop(y))
            {
                Canvas.SetTop(y, Canvas.GetTop(y) + enemyStats[0]);
            }

            if (Canvas.GetLeft(PlayerCharacter) < Canvas.GetLeft(y))
            {
                Canvas.SetLeft(y, Canvas.GetLeft(y) - enemyStats[0]);
            }
            else if (Canvas.GetLeft(PlayerCharacter) > Canvas.GetTop(y))
            {
                Canvas.SetLeft(y, Canvas.GetLeft(y) + enemyStats[0]);
            }





            // hit box
            if (PlayerHitbox.IntersectsWith(enemy))
            {
                logBox.Text += "You are dead! \n";
                dispatcherTimer.Stop();
            }


            foreach (var z in PlayerSpace.Children.OfType<Rectangle>())
            {
                if (z is Rectangle && (string)z.Tag == "weapon-sword-phys")
                {
                    Rect sword = new Rect(Canvas.GetLeft(z), Canvas.GetTop(z), z.Width, z.Height);

                    if (enemy.IntersectsWith(sword))
                    {
                        itemstoremove.Add(y);
                    }
                }

            }


        }

        public void removeAll(List<Rectangle> itemstoremove, Rectangle x, Rectangle y, Rectangle c)
        {
            itemstoremove.Add(x);
            itemstoremove.Add(c);
            itemstoremove.Add(y);
        }

        public void interactionsWithOtherItems(Rect enemy, Rect sword, Rectangle y, Rect PlayerHitbox, DispatcherTimer dispatcherTimer, List<Rectangle> itemstoremove)
        {

            if (enemy.IntersectsWith(sword))
            {
                itemstoremove.Add(y);

            }
        }


    }



    class animationMaker
    {
        public bool swordAnimation(bool weaponCreated, Rectangle z, int frame, int swordAnimation, List<Rectangle> itemstoremove)
        {
            // ANIMATION!!!!!

            // Start to count the ticks, + 1 frame every 1 tick

            // If the frames go as long as how long the sword is supposed to last
            if (frame >= swordAnimation)
            {
                // Remove Sword
                itemstoremove.Add(z);

                // Weapon is no longer created now
                return false;
            }

            return weaponCreated;
        }


        public int swordAnimationLength(bool weaponCreated, int frame)
        {
            if (weaponCreated)
            {
                frame++;

                return frame;
            }

            return frame = 0;
        }
    }



    // Move Player. This class works!

    class playerMovement
    {
        public string movementChecker(Dictionary<double, List<double>> totalPlayers, Rectangle PlayerCharacter, int objectSize, List<string> playerDirrection, int playerIsInRoom, bool nextRoom, string currentDirrection, int playerPosition)
        {
            // Player Movement!

            // GOING LEFT
            if (Keyboard.IsKeyDown(Key.Left) && Canvas.GetLeft(PlayerCharacter) > objectSize) // If left key is presssed, and if the key is 5 away from the border
            {
                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) - totalPlayers[playerPosition][8]); // get the object we want to move then the X value (Set Left). Since we are going left we are going to subteract that X value by how fast the player is moving

                return playerDirrection[2];

            }


            // GOING UP
            if (Keyboard.IsKeyDown(Key.Up) && Canvas.GetTop(PlayerCharacter) > 0) // if up key is pressed and is 5 away from the top
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) - totalPlayers[playerPosition][8]);

                return playerDirrection[0];

            }



            // GOING DOWN
            if (Keyboard.IsKeyDown(Key.Down) && Canvas.GetTop(PlayerCharacter) + (350) < Application.Current.MainWindow.Height) // if down key is pressed and if the location of the player character + it's height is away from the bottom
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) + totalPlayers[playerPosition][8]);

                return playerDirrection[1];

            }



            // GOING RIGHT

            if (Keyboard.IsKeyDown(Key.Right) && Canvas.GetLeft(PlayerCharacter) + 300 < Application.Current.MainWindow.Width) // basically get playter character and it's current width, comparing it to the border which uses the current mainwindow width
            {
                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) + totalPlayers[playerPosition][8]);

                return playerDirrection[3];

            }


            return "nothing";
        }

        public bool goNextRoom(Rectangle PlayerCharacter, int objectSize, bool nextRoom)
        {
            if ((Canvas.GetLeft(PlayerCharacter) == objectSize) || (Canvas.GetTop(PlayerCharacter) == 0) || (Canvas.GetTop(PlayerCharacter) + (350) == Application.Current.MainWindow.Height) || (Canvas.GetLeft(PlayerCharacter) + 300 == Application.Current.MainWindow.Width)) // If touch the border
            {

                return nextRoom = true;


            }

            return nextRoom = false;
        }


        public int movePlayerOnMap(Rectangle PlayerCharacter, int objectSize)
        {
            if (Canvas.GetLeft(PlayerCharacter) == objectSize) // If touch the border, left
            {

                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) + 300);

                return -1;


            }
            else if (Canvas.GetTop(PlayerCharacter) == 0) // If touch the border, up
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) + 350);

                return -10;
            }
            else if (Canvas.GetTop(PlayerCharacter) + (350) == Application.Current.MainWindow.Height) // If touch the border, down
            {
                Canvas.SetTop(PlayerCharacter, Canvas.GetTop(PlayerCharacter) - 350);


                return +10;
            }
            else if (Canvas.GetLeft(PlayerCharacter) + 300 == Application.Current.MainWindow.Width) // If touch the border, right
            {
                Canvas.SetLeft(PlayerCharacter, Canvas.GetLeft(PlayerCharacter) - 350);

                return +1;
            }

            return 0;
        }


        public int switchPlayers(int playerPosition)
        {
            if (Keyboard.IsKeyDown(Key.Z))
            {
                return playerPosition = 0;
            }
            else if (Keyboard.IsKeyDown(Key.X))
            {
                return playerPosition = 1;
            }
            else if (Keyboard.IsKeyDown(Key.C))
            {
                return playerPosition = 2;
            }
            else if (Keyboard.IsKeyDown(Key.V))
            {
                return playerPosition = 3;
            }
            else
            {
                return playerPosition;
            }
        }


    }

    // Make The map
    // the addList, selectMap works!!! Otherwise, work in progress
    class basicMapMake
    {

        // Step 0
        // Add Lists to Array
        public void addLists(int mapSize, Dictionary<int, List<int>> totalMap)
        {
            // Maybe turn this into a function
            for (int i = 0; i < mapSize; i++)
            {
                totalMap[i] = new List<int>() { 0 };
            }

        }


        // STEP 1
        // Select which map is going to be made
        public int selectMap(int mapSelect)
        {
            return 0; // be random later, but now just the tutorial will be used
        }

        // STEP 2
        // Make the Map
        // THIS WILL BE LARRRRRGEEE!!!
        // Maybe I should get this from a text file...
        public int makeMap(int map, int startRoom, List<int> currentRoom, Dictionary<int, List<int>> totalMap, setUpRooms SettingUpTheRooms)
        {
            if (map == 0)
            {
                startRoom = 56;
            }


            // INITIAIZE ALL THE CODE INTO THE DICTIONARY


            for (int i = 0; i < 100; i++)
            {
                SettingUpTheRooms.loadRooms(totalMap, i);
            }


            return startRoom;
        }

    }

    class drawSetting
    {
        public void makeSurrondings(List<int> currentRoom, int map, int playerIsInRoom, makeObjects MakingTheObjects, Canvas PlayerSpace, int objectSize, TextBlock DisplayDateTextBlock2, ScrollViewer scroll, Dictionary<int, List<int>> totalMap)
        {

            // Create map. This will be used as the basis for the place where the player moves
            List<int> room = new List<int>();

            room = totalMap[playerIsInRoom];

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

                        MakingTheObjects.makeWall(row, col, "block", objectSize, PlayerSpace);


                    }
                    else if (room[generateRoom] == 2) // generate a door, unused for now
                    {
                        row++;

                        // Get Function
                        // none
                    }
                    else if (room[generateRoom] == 3) // generate an enemy
                    {
                        row++;

                        // Get Function
                        MakingTheObjects.makeEnemy(row, col, "zombie", objectSize, PlayerSpace);
                    }
                    else if (room[generateRoom] == 4) // generate an enemy
                    {
                        row++;

                        // Get Function
                        MakingTheObjects.makeItem(row, col, "coin", objectSize, PlayerSpace);
                    }
                    else if (room[generateRoom] == 5) // generate a textbox
                    {
                        row++;

                        string text = "";

                        if (playerIsInRoom == 56)
                        {
                            text = "Welcome to the tutorial! Use the arrow keys to move! Use the spacebar to attack!";
                        }
                        else if (playerIsInRoom == 55)
                        {
                            text = "Enemies endlessly respawn from set points! If you have a [holy relic] you can stop the spawning but only in that room";
                        }
                        else if (playerIsInRoom == 46)
                        {
                            text = "These are items. Touching them will give you stuff but removes them permanently. A coin can be used to open crates. A heart for health. A bullet for a gun.";
                        }


                        // Get Function
                        MakingTheObjects.makeText(text, DisplayDateTextBlock2, scroll);
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


    }



    class makeObjects
    {

        public void makePlayer(Rectangle PlayerCharacter, String ImageUsed)
        {
            // make an image for the player using an image brush
            // an image brush is a type of tile brush that define it's content as an image by it's Image Source proprety
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imagebrush?view=windowsdesktop-6.0
            ImageBrush playerImage = new ImageBrush();

            // Load tha Player into it
            // this is a specialized bitmap source using xaml to loading images
            playerImage.ImageSource = new BitmapImage(new Uri(ImageUsed));

            // asign the player the rect
            // filling the timage with the source
            PlayerCharacter.Fill = playerImage;
        }

        public void highlightCurrentPlayer(Dictionary<double, List<double>> totalPlayers, int playerPosition, TextBlock Player1Name, TextBlock Player2Name, TextBlock Player3Name, TextBlock Player4Name)
        {

            Player1Name.Foreground = Brushes.Black;
            Player2Name.Foreground = Brushes.Black;
            Player3Name.Foreground = Brushes.Black;
            Player4Name.Foreground = Brushes.Black;

            if (playerPosition == 0)
            {
                Player1Name.Foreground = Brushes.Blue;
            }
            else if (playerPosition == 1)
            {
                Player2Name.Foreground = Brushes.Blue;
            }
            else if (playerPosition == 2)
            {
                Player3Name.Foreground = Brushes.Blue;
            }
            else if (playerPosition == 3)
            {
                Player4Name.Foreground = Brushes.Blue;
            }


        }

        public void getImageCharacter(Dictionary<double, List<double>> totalPlayers, int playerPosition, Rectangle PlayerCharacter)
        {
            if (totalPlayers[playerPosition][0] == 0)
            {
                makePlayer(PlayerCharacter, "C:/Users/peter/source/repos/Start_Game/Start_Game/images/knight.png");

            }
            else if (totalPlayers[playerPosition][0] == 1)
            {
                makePlayer(PlayerCharacter, "C:/Users/peter/source/repos/Start_Game/Start_Game/images/squire.png");
            }
            else if (totalPlayers[playerPosition][0] == 2)
            {
                makePlayer(PlayerCharacter, "C:/Users/peter/source/repos/Start_Game/Start_Game/images/paladin.png");
            }
            else if (totalPlayers[playerPosition][0] == 3)
            {
                makePlayer(PlayerCharacter, "C:/Users/peter/source/repos/Start_Game/Start_Game/images/red_mage.png");
            }
            else if (totalPlayers[playerPosition][0] == 4)
            {
                makePlayer(PlayerCharacter, "C:/Users/peter/source/repos/Start_Game/Start_Game/images/dark_mage.png");
            }
            else if (totalPlayers[playerPosition][0] == 5)
            {
                makePlayer(PlayerCharacter, "C:/Users/peter/source/repos/Start_Game/Start_Game/images/gunslinger.png");
            }
        }

        public void makeWall(int row, int col, string blockType, int objectSize, Canvas PlayerSpace)
        {
            // Basic Zombie
            drawItem(row, col, blockType, objectSize, objectSize, objectSize, PlayerSpace, "wall", "C:/Users/peter/source/repos/Start_Game/Start_Game/images/2.png");
        }


        public void makeEnemy(int row, int col, string blockType, int objectSize, Canvas PlayerSpace)
        {

            // Basic Zombie
            drawItem(row, col, blockType, objectSize, 20, 20, PlayerSpace, "enemy", "C:/Users/peter/source/repos/Start_Game/Start_Game/images/3.png");

        }

        public void makeItem(int row, int col, string itemType, int objectSize, Canvas PlayerSpace)
        {
            // Coin
            drawItem(row, col, itemType, objectSize, 20, 20, PlayerSpace, "coin", "C:/Users/peter/source/repos/Start_Game/Start_Game/images/5.png");
        }

        public void makeText(string innerText, TextBlock DisplayDateTextBlock2, ScrollViewer scroll)
        {
            DisplayDateTextBlock2.Text += $"{innerText} \n";
            scroll.ScrollToEnd();
        }

        public void drawItem(int row, int col, string type, int objectSize, int height, int width, Canvas PlayerSpace, string tagName, string imageUrl)
        {

            // Image Brush
            ImageBrush Image = new ImageBrush();

            Image.ImageSource = new BitmapImage(new Uri(imageUrl));

            Rectangle newImage = new Rectangle
            {
                Tag = tagName,
                Height = height,
                Width = width,
                Fill = Image
            };

            Canvas.SetLeft(newImage, (row * objectSize));
            Canvas.SetTop(newImage, (col * objectSize));


            PlayerSpace.Children.Add(newImage);


            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game
        }


    }




    // WEIRD BUG WITH ATTACK WHEN STANDING STILL
    class makeAttack
    {
        public bool swordAttack(bool weaponCreated, string currentDirrection, List<string> playerDirrection, Canvas PlayerSpace)
        {
            // Create Object when pressed
            if (Keyboard.IsKeyDown(Key.Space) && weaponCreated == false)
            {
                int weaponHeight = 0;
                int weaponWidth = 0;
                double weaponPosition = 0;

                ImageBrush weaponImage = new ImageBrush();


                if (currentDirrection == playerDirrection[0] || currentDirrection == playerDirrection[1]) // If current Dirrection up or down
                {
                    weaponHeight = 40;
                    weaponWidth = 15;
                    weaponPosition = 4;

                    makeWeapon(weaponHeight, weaponWidth, weaponPosition, weaponImage, PlayerSpace);

                    return weaponCreated = true;

                }
                if (currentDirrection == playerDirrection[2] || currentDirrection == playerDirrection[3]) // if dirrection left or right
                {
                    weaponHeight = 15;
                    weaponWidth = 40;
                    weaponPosition = 4.5;

                    makeWeapon(weaponHeight, weaponWidth, weaponPosition, weaponImage, PlayerSpace);

                    return weaponCreated = true;
                }


            }

            return weaponCreated;

        }

        public void makeWeapon(int weaponHeight, int weaponWidth, double weaponPosition, ImageBrush weaponImage, Canvas PlayerSpace)
        {
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
        }

        public string facing(string currentDirrecton, string oldDirrection, string currentDirrection, bool weaponCreated)
        {
            if (weaponCreated == false)
            {
                return oldDirrection = currentDirrection;
            }

            return oldDirrection;
        }


    }



    class saveClass
    {
        public void saveRoom(Rectangle c, Dictionary<int, List<int>> totalMap, int playerIsInRoom, int objectSize)
        {

            // Create map. This will be used as the basis for the place where the player moves
            List<int> room = new List<int>();

            room = totalMap[playerIsInRoom];

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
                    room[row + (Convert.ToInt32(Canvas.GetTop(c)) / 4) - 1] = 0;

                    // should get currentRoom[27]

                }




                generateRoom++;






            }

        }
    }



    class setUpRooms
    {
        public void loadRooms(Dictionary<int, List<int>> totalMap, int i)
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

            }
            else if (i == 55) // left room
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
            }
            else if (i == 57) // right room
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
            else if (i == 66) // down room
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
            else if (i == 46) // up room
            {
                List<int> setUpMap = new List<int>()
                {
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 5, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 4, 0, 0, 4, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 1, 1, 1, 0, 0, 1, 1, 1, 1
                };

                totalMap[i] = setUpMap;
            }
            else
            {
                List<int> setUpMap = new List<int>()
                {
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
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
