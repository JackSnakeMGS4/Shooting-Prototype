using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shooting_Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer update = new DispatcherTimer();
        private int framesPerSecond = 10;

        private double playerX;//for player position
        private double playerCenterX;//for player's center
        private double playerY;//for player's feet
        private double playerTopY;//for player's head
        private double playerSpeedX = 7;

        private int projectileWidth = 10;//for projectile's width
        private int projectileHeight = 10;//for projectile's height
        private double projectileCenterX;//for projectile x-axis center
        private double projectileY;//for projectil bottom most point
        private int projectileSpeedY = 10;//rate at which projectile moves
        private const double GAP_BETWEEN_PROJECTILES = 2.5;//creates a gap between shots so it looks more natural
        private int shotLimit = 5;//limits the player to only shooting 5 shots

        private Rectangle projectile;//let's me reference the projectile outside of the drawing function
        private List<Rectangle> shots = new List<Rectangle>();//list that contains each individual instance of the rectangle class

        public MainWindow()
        {
            InitializeComponent();

            playerX = Canvas.GetLeft(player);//playerX is 170
           
            playerY = Canvas.GetBottom(player);//player is 0 
            playerTopY = playerY + player.Height;//set playerTop to 40

            //next three lines are the game loop
            update.Tick += Update_Tick;
            update.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond);
            update.Start();
        }

        private void Update_Tick(object sender, EventArgs e)
        {       
            MoveAll();//calling MoveAll() first so projectile are actually fired from the player's current position
            if (Keyboard.IsKeyDown(Key.V) && shots.Count != shotLimit)//is fired when V is pressed or held down
            {
                //running into an with the shots freezing in mid air if V is pressed multiple times or held down
                DrawProjectiles(projectileWidth, projectileHeight, Colors.Firebrick, Colors.White, 2,
                    playerCenterX - (projectileWidth / 2)/*sets location of shot's center to the center of the player*/,
                    playerTopY/*sets the location of the shot's bottom point*/);
                shots.Add(projectile);//adds the instance of the shot to the list
            }
        }

        private void MoveAll()
        {
            playerCenterX = playerX + (player.Width / 2);//player's x-axis center is 200

            ////next line checks if projectile bottom is past the play area height
            //if (projectileY > gameCanvas.Height)
            //{
            //    projectile = null;//makes projectile null if it goes off the top of the play area
            //}
            if (shots.Count > 0)
            {
                foreach (Rectangle projectile in shots)//loops through the list of rectangles and applies the if statement to each invdividual instance
                {
                    if (projectile != null)//fires only if projectile isn't null
                    {
                        projectileCenterX = Canvas.GetLeft(projectile);//get the updated x-coord for the projectile
                        projectileY = Canvas.GetBottom(projectile);//get the updated y-coord for the projectile which is always the same since the player doesn't move up or down

                        projectileY += projectileSpeedY;//adds projectileSpeed to projectileY

                        Canvas.SetBottom(projectile, projectileY + GAP_BETWEEN_PROJECTILES);//sets projectile's bottom to new projectileY
                        projectile.Visibility = Visibility.Visible;//makes projectile visible after it's set
                    }
                }
                foreach (Rectangle projectile in shots)
                {
                    //managed to limit the player to five shots but now I want remove each individual shot from my list and the canvas as they pass the canvas's boundaries
                    if (Canvas.GetBottom(shots.Last(projectile)) /*Canvas.GetBottom(projectile)*/ > gameCanvas.Height)
                    {
                        shots.Remove(projectile);
                        gameCanvas.Children.Remove(projectile);
                    }
                }
              
            }           

            //next two if statements are for the player movement on the x-axis and will fire if the player is within the game boundaries
            if (Keyboard.IsKeyDown(Key.A) && playerX > 0)
            {
                playerX -= playerSpeedX;
            }
            if (Keyboard.IsKeyDown(Key.D) && playerX < gameCanvas.Width - player.Width)
            {
                playerX += playerSpeedX;
            }

            Canvas.SetLeft(player, playerX);
        }

        private void DrawProjectiles(int shotWidth, int shotHeight, Color shotOutline, Color shotColor, int shotThickness, double xAxisLocation, double yAxisLocation)
        {
            /*creates new instance of a rectangle named projectile
             * hides the shot then sets it width/height
             * sets outline color, thickness of outline, and inner color
             * sets shot's position based on the updated player's x and y coords
             */
            /*Note to self: Issue is occuring somewhere here in conjunction with the 
             * event handler for the V key because the variable refers to only one object at any given time
             * needs to find a way have multiple shots individually based off one reference 
             * probably not possible and will most likely need to have a list of shots
             * something like this: List<Rectangle> shotsList = new List<Rectangle>();
             * That way I can added multiple objects to it and loop through the list and fire
             * the ones that in the list and then I should remove them from the list once they go
             * off screen
             */
            projectile = new Rectangle();
            projectile.Visibility = Visibility.Hidden;
            projectile.Width = projectileWidth;
            projectile.Height = projectileHeight;
           
            projectile.Stroke = new SolidColorBrush(shotOutline);
            projectile.StrokeThickness = shotThickness;
            projectile.Fill = new SolidColorBrush(shotColor);

            Canvas.SetLeft(projectile, xAxisLocation);
            Canvas.SetBottom(projectile, yAxisLocation);

            gameCanvas.Children.Add(projectile);
        }
    }
}
