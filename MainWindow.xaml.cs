using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FindPairsGame
{
    public partial class MainWindow : Window
    {
        private List<string> cardContentList;  
        private List<Button> cardButtonList;   
        private Button firstCard;              
        private Button secondCard;             
        private int pairsFound;                
        private bool isTimerRunning;           
        private Stopwatch stopwatch;           

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Начальные значения переменных
            pairsFound = 0;
            firstCard = null;
            secondCard = null;
            isTimerRunning = false;
            stopwatch = new Stopwatch();

            // создаём список с содержимым карточек
            cardContentList = new List<string>()
            {
                "😺","😺",
                "🐵","🐵",
                "🐶","🐶",
                "🐺","🐺",
                "🦁","🦁",
                "🐷","🐷",
                "🐮","🐮",
                "🐰","🐰"
            };

            // перемешиваем наш список
            Random random = new Random();
            int n = cardContentList.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                string value = cardContentList[k];
                cardContentList[k] = cardContentList[n];
                cardContentList[n] = value;
            }

            // создание и настройки кнопок
            cardButtonList = new List<Button>();
            for (int i = 0; i < 16; i++)
            {
                Button button = new Button()
                {
                    Content = "",
                    Tag = cardContentList[i],
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(10),
                    FontSize = 48,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                button.Click += CardButton_Click;
                cardButtonList.Add(button);
            }

            // добавление кнопок в сетку
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    grid.Children.Add(cardButtonList[row * 4 + column]);
                    Grid.SetRow(cardButtonList[row * 4 + column], row);
                    Grid.SetColumn(cardButtonList[row * 4 + column], column);
                }
            }
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            Button currentCard = (Button)sender;

            // если таймер не работает,запустить его
            if (!isTimerRunning)
            {
                StartTimer();
            }

            // Если выбранная карта уже открыта или принадлежит совпавшей паре, ничего не делать
            if (currentCard.Content.ToString() != "" || currentCard == firstCard || currentCard == secondCard)
            {
                return;
            }

            currentCard.Content = currentCard.Tag;   // Открыть нажатую карту

            if (firstCard == null)
            {
                firstCard = currentCard;
            }
            else if (secondCard == null)
            {
                secondCard = currentCard;

                // совпадает ли содержимое двух открытых карточек.
                if (firstCard.Tag.ToString() == secondCard.Tag.ToString())
                {
                    pairsFound++;
                    firstCard = null;
                    secondCard = null;

                    // Если все пары найдены,то останавливаем таймер и выводим сообщение
                    if (pairsFound == 8)
                    {
                        StopTimer();
                        MessageBox.Show($"Поздравляем,вы нашли все пары за {stopwatch.Elapsed.TotalSeconds} секунд.", "Победа!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    // Если карты не совпадают,делаем небольшую задержка,а потом закрываем их
                    DispatcherTimer delayTimer = new DispatcherTimer();
                    delayTimer.Interval = TimeSpan.FromSeconds(0.15);
                    delayTimer.Tick += DelayTimer_Tick;
                    delayTimer.Start();
                }
            }
        }

        private void StartTimer()
        {
            stopwatch.Start();
            isTimerRunning = true;
        }

        private void StopTimer()
        {
            stopwatch.Stop();
            isTimerRunning = false;
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer delayTimer = (DispatcherTimer)sender;
            delayTimer.Stop();

            firstCard.Content = "";
            secondCard.Content = "";
            firstCard = null;
            secondCard = null;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }
    }
}