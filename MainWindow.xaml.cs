using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
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

namespace PAIN_Lab5_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum Continent { Europe, Asia, Africa, SouthAmerica, NorthAmerica, All, Oceania };
        enum Mode { Start, ChooseCategory, Play };
        private int image_id;
        private int image_number;
        private List<string> flags;
        private int hintSize;
        private int score;
        private int bestScore;
        private int maxScore;
        private static readonly Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            resetGame();
        }

        private void resetGame()
        {
            image_id = 0;
            hintSize = 0;
            score = 0;
            bestScore = 0;
            maxScore = 0;
        }
        
        private void setScore(bool correct)
        {
            int answerLength = flags[image_id].Length;
            if (correct)
            {
                score += (answerLength - hintSize) * 10;
                maxScore += answerLength *10;
            }
            else
                score -= answerLength * 5;
        }

        private void answerPressed()
        {
            if (AnswerBox.Text.ToLower() == flags[image_id].ToLower())
            {
                setScore(true);
                flags.RemoveAt(image_id);
                image_number = flags.Count;
                nextPicture();
            }
            else
                setScore(false);
            AnswerBox.Text = "";
            scoreLabel.Content = score+" / "+maxScore;
        }

        private void answerButton_Click(object sender, RoutedEventArgs e)
        {
            answerPressed();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            changeModes(Mode.Start, Mode.ChooseCategory);
        }
        
        private void gameOver()
        {
            MessageBox.Show("Twój wynik to "+score+" / "+maxScore+".");
            changeModes(Mode.Play, Mode.Start);
            resetGame();
        }
        
        private void nextPicture()
        {
            if (image_number==0)
                gameOver();
            else
            {
                hintSize = 0;
                image_id = random.Next(0, image_number);

                try
                {
                    Uri uri = new Uri(@"Flags/" + flags[image_id] + ".png", UriKind.Relative);
                    this.image.Source = new BitmapImage(uri);
                }
                catch (Exception)
                {
                    Uri uri = new Uri(@"Flags/error.png", UriKind.Relative);
                    image.Source = new BitmapImage(uri);
                    return;
                }
                if (!hintButton.IsEnabled)
                    hintButton.IsEnabled = true;
                if (!noIdeaButton.IsEnabled)
                    noIdeaButton.IsEnabled = true;
                if ((string)noIdeaButton.Content != "Nie wiem")
                    noIdeaButton.Content = "Nie wiem";
            }
        }
        
        private void hintButton_Click(object sender, RoutedEventArgs e)
        {
            AnswerBox.Text = flags[image_id].Substring(0, ++hintSize);
            if (hintSize == flags[image_id].Length)
            {
                hintButton.IsEnabled = false;
                noIdeaButton.IsEnabled = false;
            }
        }

        private void noIdeaButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnswerBox.Text != flags[image_id])
            {
                AnswerBox.Text = flags[image_id];
                noIdeaButton.Content = "Następny";
                maxScore += flags[image_id].Length * 10;
                hintButton.IsEnabled = false;
                answerButton.IsEnabled = false;
            }
            else // nastepna flaga
            {
                setScore(false);
                AnswerBox.Text = "";
                scoreLabel.Content = score + " / " + maxScore; ;
                answerButton.IsEnabled = true;
                flags.RemoveAt(image_id);
                image_number = flags.Count;
                nextPicture();
            }
        }
        
        private void AnswerBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                answerPressed();
            }
        }

        private void changeModes(Mode from, Mode to)
        {
            switch(from)
            {
                case Mode.Start:
                    playButton.Visibility = Visibility.Hidden;
                    image.Visibility = Visibility.Hidden;
                    break;
                case Mode.ChooseCategory:
                    categoriesPanel.Visibility = Visibility.Hidden;
                    break;
                case Mode.Play:
                    backToMenuButton.Visibility = Visibility.Hidden;
                    image.Visibility = Visibility.Hidden;
                    answerButton.Visibility = Visibility.Hidden;
                    AnswerBox.Visibility = Visibility.Hidden;
                    noIdeaButton.Visibility = Visibility.Hidden;
                    hintButton.Visibility = Visibility.Hidden;
                    yourScoreLabel.Visibility = Visibility.Hidden;
                    scoreLabel.Visibility = Visibility.Hidden;
                    break;
            }   
            
            switch(to)
            {
                case Mode.Start:
                    image.Source = new BitmapImage(new Uri(@"Flags/0.png", UriKind.Relative));
                    playButton.Visibility = Visibility.Visible;
                    image.Visibility = Visibility.Visible;
                    break;
                case Mode.ChooseCategory:
                    categoriesPanel.Visibility = Visibility.Visible;
                    break;
                case Mode.Play:
                    image_number = flags.Count;
                    scoreLabel.Content = "0";
                    backToMenuButton.Visibility = Visibility.Visible;
                    image.Visibility = Visibility.Visible;
                    answerButton.Visibility = Visibility.Visible;
                    AnswerBox.Visibility = Visibility.Visible;
                    noIdeaButton.Visibility = Visibility.Visible;
                    hintButton.Visibility = Visibility.Visible;
                    yourScoreLabel.Visibility = Visibility.Visible;
                    scoreLabel.Visibility = Visibility.Visible;
                    break;
            }         
        }

        private void allButton_Click(object sender, RoutedEventArgs e)
        {
            string test = Properties.Resources.Countries;
            string[] flagsArr = test.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //string[] flagsArr = System.IO.File.ReadAllLines(@"C:/Users/lenak/OneDrive/Documents/Visual Studio 2015/Projects/PAIN_Lab5_WPF/Flags/Countries.txt");
            flags = new List<string>(flagsArr);
            changeModes(Mode.ChooseCategory, Mode.Play);
            nextPicture();
        }

        private void europeButton_Click(object sender, RoutedEventArgs e)
        {
            string test = Properties.Resources.Europe;
            string[] flagsArr = test.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //string[] flagsArr = System.IO.File.ReadAllLines(@"C:/Users/lenak/OneDrive/Documents/Visual Studio 2015/Projects/PAIN_Lab5_WPF/Flags/Europe.txt");
            flags = new List<string>(flagsArr);
            changeModes(Mode.ChooseCategory, Mode.Play);
            nextPicture();
        }

        private void northAmericaButton_Click(object sender, RoutedEventArgs e)
        {
            string test = Properties.Resources.NorthAmerica;
            string[] flagsArr = test.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //string[] flagsArr = System.IO.File.ReadAllLines(@"C:/Users/lenak/OneDrive/Documents/Visual Studio 2015/Projects/PAIN_Lab5_WPF/Flags/NorthAmerica.txt");
            flags = new List<string>(flagsArr);
            changeModes(Mode.ChooseCategory, Mode.Play);
            nextPicture();
        }

        private void southAmericaButton_Click(object sender, RoutedEventArgs e)
        {
            string test = Properties.Resources.SouthAmerica;
            string[] flagsArr = test.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //string[] flagsArr = System.IO.File.ReadAllLines(@"C:/Users/lenak/OneDrive/Documents/Visual Studio 2015/Projects/PAIN_Lab5_WPF/Flags/SouthAmerica.txt");
            flags = new List<string>(flagsArr);
            changeModes(Mode.ChooseCategory, Mode.Play);
            nextPicture();
        }

        private void africaButton_Click(object sender, RoutedEventArgs e)
        {
            string test = Properties.Resources.Africa;
            string[] flagsArr = test.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //string[] flagsArr = System.IO.File.ReadAllLines(@"C:/Users/lenak/OneDrive/Documents/Visual Studio 2015/Projects/PAIN_Lab5_WPF/Flags/Africa.txt");
            flags = new List<string>(flagsArr);
            changeModes(Mode.ChooseCategory, Mode.Play);
            nextPicture();
        }

        private void asiaButton_Click(object sender, RoutedEventArgs e)
        {
            string test = Properties.Resources.Asia;
            string[] flagsArr = test.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //string[] flagsArr = System.IO.File.ReadAllLines(@"C:/Users/lenak/OneDrive/Documents/Visual Studio 2015/Projects/PAIN_Lab5_WPF/Flags/Asia.txt");
            flags = new List<string>(flagsArr);
            changeModes(Mode.ChooseCategory, Mode.Play);
            nextPicture();
        }

        private void oceaniaButton_Click(object sender, RoutedEventArgs e)
        {
            string test = Properties.Resources.Australia1;
            string[] flagsArr = test.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //string[] flagsArr = System.IO.File.ReadAllLines(@"C:/Users/lenak/OneDrive/Documents/Visual Studio 2015/Projects/PAIN_Lab5_WPF/Flags/Australia.txt");
            flags = new List<string>(flagsArr);
            changeModes(Mode.ChooseCategory, Mode.Play);
            nextPicture();
        }

        private void backToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            changeModes(Mode.Play, Mode.ChooseCategory);
        }
    }
}
