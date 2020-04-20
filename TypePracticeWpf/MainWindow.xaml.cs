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
using System.IO;

namespace TypePracticeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int maxSentence = 4;
        const int minSentence = 1;
        const int maxWord = 15;
        const int minWord = 5;
        Punctuation[] punctuations = new[]
        {
            new Punctuation(".", true),
            new Punctuation("?", true),
            new Punctuation(",", false),
            new Punctuation(";", false),
            new Punctuation(":", false)
        };

        Random random = new Random();
        List<char[]> generatedText = new List<char[]>();
        int currentPosition;
        TextBlock currentTextBlock;
        string currentWord;
        DateTime startTime;
        string[] words;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            words = File.ReadAllLines("./words.txt");
            buttonGenerate_Click(this, e);
        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            wrapPanelPhrase.Children.Clear();
            generatedText.Clear();
            progressBar.Value = 0;
            textBoxInput.IsEnabled = true;
            textBoxInput.Text = "";
            startTime = default;
            currentPosition = 0;

            var sentenceLength = random.Next(minSentence, maxSentence + 1);
            bool capitalizeNextWord = true;
            for (int i = 0; i < sentenceLength; i++)
            {
                var punctuation = punctuations[random.Next(punctuations.Length)];
                var wordLength = random.Next(minWord, maxWord + 1);
                for (int j = 0; j < wordLength; j++)
                {
                    var word = (words[random.Next(words.Length)] + (i + 1 == sentenceLength && j + 1 == wordLength
                        ? punctuation.Value
                        : j + 1 == wordLength
                            ? $"{punctuation.Value} "
                            : " ")).ToCharArray();

                    if (capitalizeNextWord)
                    {
                        capitalizeNextWord = false;
                        word.Capitalize();
                    }

                    generatedText.Add(word);
                    var textBlock = new TextBlock();
                    foreach (var character in word)
                    {
                        var run = new Run(character.ToString());
                        textBlock.Inlines.Add(run);
                    }
                    
                    wrapPanelPhrase.Children.Add(textBlock);
                }

                capitalizeNextWord = punctuation.CapitalizeNextWord;
            }

            MoveTextBlock();
        }

        private void textBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (currentTextBlock == null) return;
            if (startTime == default) startTime = DateTime.Now;

            for (int i = 0; i < currentWord.Length; i++)
            {
                currentTextBlock.Inlines.ElementAt(i).Foreground = i >= textBoxInput.Text.Length
                    ? Brushes.White
                    : textBoxInput.Text[i] == currentWord[i]
                        ? Brushes.MediumPurple
                        : Brushes.Red;
            }

            if (currentWord == textBoxInput.Text)
            {
                ++currentPosition;
                MoveTextBlock();
                textBoxInput.Text = "";
            }
        }

        private void MoveTextBlock()
        {
            progressBar.Value = (currentPosition / (double)wrapPanelPhrase.Children.Count) * 100;
            var difference = DateTime.Now - startTime;
            int totalWordLength = 0;
            for (int i = 0; i < currentPosition; i++)
            {
                totalWordLength += generatedText[i].Length;
            }

            textBlockWpm.Text = $"{(totalWordLength / 5d) / difference.TotalMinutes:N0} WPM";

            if (currentPosition == wrapPanelPhrase.Children.Count)
            {
                currentTextBlock = null;
                currentWord = "";
                textBoxInput.IsEnabled = false;
                return;
            }

            currentTextBlock = (TextBlock)wrapPanelPhrase.Children[currentPosition];
            currentWord = currentTextBlock.Inlines
                .Aggregate(new StringBuilder(), (acc, current) => acc.Append(((Run)current).Text))
                .ToString();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMaximze_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
