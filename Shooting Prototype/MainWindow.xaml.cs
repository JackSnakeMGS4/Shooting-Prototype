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
        private int framesPerSecond = 120;

        private double playerX;//for player position
        private double playerCenterX;//for player's center
        private double playerY;//for player's feet
        private double playerTopY;//for player's head
        private double playerSpeedX = 7;

        private int projectileWidth = 10;//for projectile's width
        private int projectileHeight = 20;//for projectile's height
        private double projectileCenterX;//for projectile x-axis center
        private double projectileY;//for projectil bottom most point
        private int projectileSpeed = 10;//rate at which projectile moves
        private Rectangle projectile;//let's me reference the projectile outside of the drawing function

        public MainWindow()
        {
            InitializeComponent();

            playerX = Canvas.GetLeft(player);//playerX is 170
           
            playerY = Canvas.GetBottom(player);//player is 0 
            playerTopY = playerY + player.Height;//set playerTop to 40

            update.Tick += Update_Tick;
            update.Interval = TimeSpan.FromMilliseconds(1000 / framesPerSecond);
            update.Start();
        }

        private void Update_Tick(object sender, EventArgs e)
        {       
            MoveAll();
            if (Keyboard.IsKeyDown(Key.V))
            {
                DrawProjectiles(projectileWidth, projectileHeight, Colors.Firebrick, Colors.White, 2,
                    playerCenterX - (projectileWidth / 2)/*sets location of shot's center to the center of the player*/,
                    playerTopY/*sets the location of the shot's bottom point*/);
            }
        }

        private void MoveAll()
        {
            playerCenterX = playerX + (player.Width / 2);//player's x-axis center is 200
            if (projectile != null)
            {               
                projectileCenterX = Canvas.GetLeft(projectile);
                projectileY = Canvas.GetBottom(projectile);

                projectileY += projectileSpeed;
               
                Canvas.SetBottom(projectile, projectileY);
                projectile.Visibility = Visibility.Visible;
            }
           
            if (projectileY > gameCanvas.Height)
            {
                projectile = null;
            }

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
