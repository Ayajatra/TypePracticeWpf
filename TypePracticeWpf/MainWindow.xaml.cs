using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.IO;

namespace TypePracticeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        List<char[]> generatedText = new List<char[]>();
        int currentPosition;
        TextBlock currentTextBlock;
        string currentWord;
        DateTime startTime;
        string[] paragraphs;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            paragraphs = File.ReadAllLines("./paragraphs.txt");
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

            var paragraph = paragraphs[random.Next(paragraphs.Length)];
            var words = Regex.Split(paragraph, @"(?<=\s)", RegexOptions.Compiled);
            foreach (var word in words)
            {
                var characters = word.ToCharArray();
                var textBlock = new TextBlock();

                foreach (var character in characters)
                {
                    var run = new Run(character.ToString());
                    textBlock.Inlines.Add(run);
                }
                    
                generatedText.Add(characters);
                wrapPanelPhrase.Children.Add(textBlock);
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
