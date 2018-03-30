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
        private int framesPerSecond = 120;//no lagging in this house

        private double playerX;//for player position
        private double playerCenterX;//for player's center
        private double playerY;//for player's feet
        private double playerTopY;//for player's head
        private double playerSpeedX = 7;//sets speed for player's horizontal movement

        private int projectileWidth = 10;//for projectile's width
        private int projectileHeight = 10;//for projectile's height
        private double projectileCenterX;//for projectile x-axis center
        private double projectileY;//for projectile's bottom most point
        private int projectileSpeedY = 10;//rate at which projectile moves
        private const double GAP_BETWEEN_PROJECTILES = 2.5;//creates a gap between shots so it looks more natural
        private int shotLimit = 5;//limits the player to only having 5 shots on screen at any given time

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
            if (Keyboard.IsKeyDown(Key.V) && shots.Count < shotLimit)//is fired when V is pressed or held down
            {
                //running into an with the shots freezing in mid air if V is pressed multiple times or held down: SOLVED 03/29/2018
                DrawProjectiles(projectileWidth, projectileHeight, Colors.Firebrick, Colors.White, 2,
                    playerCenterX - (projectileWidth / 2)/*sets location of shot's center to the center of the player*/,
                    playerTopY/*sets the location of the shot's bottom point*/);
                shots.Add(projectile);//adds the instance of the shot to the list
            }
        }

        private void MoveAll()
        {
            playerCenterX = playerX + (player.Width / 2);//player's x-axis center is 200

            ////next line fires if the number of elements in the shots list is greater than 0
            if (shots.Count > 0)
            {                          
                /*Note to self: issue with me checking each individual shot's bottom and deleting them as each one goes off screen was 
                 * SOLVED 03/30/18 with the advice of an experienced software engineer from Microsoft
                 */
                for (int indexOfProjectile = 0; indexOfProjectile < shots.Count; indexOfProjectile++)
                {
                    /*in this for loop I'm looping through each index in the shots list
                     * I then check for the actual element rather than the index by calling ElementAt()
                     * passing it the index that I want to check
                     * the first if statement then checks if the index is not null
                     * if that holds true then it will update the x and y coords for each projectile
                     * the first if statement must be checked before I check and remove shots that are off screen.
                     * If I flipped it around then I would get exception that occurs by removing the object from list
                     * and then trying to check for an index that is no longer contained in the list
                     */
                    if (shots.ElementAt(indexOfProjectile) != null)//fires only if projectile isn't null
                    {
                        //next line gets the updated x-coord for the projectile
                        projectileCenterX = Canvas.GetLeft(shots.ElementAt(indexOfProjectile));
                        //next line gets the updated y-coord for the projectile's bottom which is always the same since the player doesn't move up or down
                        projectileY = Canvas.GetBottom(shots.ElementAt(indexOfProjectile));

                        projectileY += projectileSpeedY;//adds projectileSpeed to projectileY

                        Canvas.SetBottom(shots.ElementAt(indexOfProjectile), projectileY + GAP_BETWEEN_PROJECTILES);//sets projectile's bottom to new projectileY
                        projectile.Visibility = Visibility.Visible;//makes projectile visible after it's set.
                    }
                   
                    /* Okay so this part was the biggest hassle in the entire program.
                     * It's job is to check each individual shots and compare their bottom property which has been
                     * updated thanks to the previous statement. If the shots bottom value is greater
                     * than the height value of the canvas then it will first remove it from the canvas
                     * after that is done then it will be removed from the shots list.
                     * The issue I was running into earlier this week was that I was removing 
                     * the index from the list first and then trying to use that index, which no 
                     * longer existed as, as the index to remove from the canvas
                     */
                    if (Canvas.GetBottom(shots.ElementAt(indexOfProjectile)) > gameCanvas.Height)
                    {
                        gameCanvas.Children.Remove(shots.ElementAt(indexOfProjectile));
                        shots.Remove(shots.ElementAt(indexOfProjectile));
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

            //next line sets the player's position based on their input
            Canvas.SetLeft(player, playerX);
        }

        private void DrawProjectiles(int shotWidth, int shotHeight, Color shotOutline, Color shotColor, int shotThickness, double xAxisLocation, double yAxisLocation)
        {
            /*creates new instance of a rectangle named projectile
             * hides the shot then sets it width/height
             * sets outline color, thickness of outline, and inner color
             * sets shot's position based on the updated player's x and y coords
             * final line actually adds the rectangle to the canvas
             */
            /*Note to self: Issue is occuring somewhere here in conjunction with the 
             * event handler for the V key because the variable refers to only one object at any given time
             * needs to find a way have multiple shots individually based off one reference 
             * probably not possible and will most likely need to have a list of shots
             * something like this: List<Rectangle> shotsList = new List<Rectangle>();
             * That way I can added multiple objects to it and loop through the list and fire
             * the ones that in the list and then I should remove them from the list once they go
             * off screen: SOLVED 03/29/18
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
