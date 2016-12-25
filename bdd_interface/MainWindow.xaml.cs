using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace bdd_interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Process proc = null;
        Color whiteColor = Colors.WhiteSmoke;
        Color blackColor = Colors.Gray;


        int currentSolve = 0;
        string[] solves;
        int countOfSolves;
        int tableSize;
        string pathToSolves;
        string pathToCountOfSolves;
        int currentTactic = 0;
        string[] tacticNames = { "BDD-опт.", "Полный перебор", "Урезаный перебор", "Игра" };
        Thread currentThread;
        Bruteforce bruteForceSolver;
        Thread timerThread;
        bool isFirstStart = true;
        int countCellsToSet = 3;
        Grid table;
        bool[,] varTables;
        bool[,] varTablesMain;
        public MainWindow()
        {
            InitializeComponent();
            initLoandingAnimation();
            nameOfTactic.Content = tacticNames[0];
            importButton.Visibility = Visibility.Hidden;
            socketToLoandingAnimation.Visibility = Visibility.Hidden;
            setBackground();
            //grid.Background = new SolidColorBrush(Colors.Gray);
            //Grid newTable = createTable(tableSize);
            //newTable.SetValue(Grid.RowProperty, 1);
            //newTable.SetValue(Grid.ColumnProperty, 1);
            //setCell(newTable, 1, 1);
            //grid.Children.Add(newTable);
        }
        public void setBackground()
        {
            Image image = new Image();
            image.SetValue(Grid.RowProperty, 1);
            image.SetValue(Grid.ColumnProperty, 1);
            Uri source = new Uri(@"/bdd_interface;component/img/background.jpg", UriKind.Relative);
            image.Source = new BitmapImage(source);
            grid.Children.Add(image);
        }
        public Grid createTable(int tableSize)
        {
            Grid newTable = new Grid();
            for (int i = 0; i < tableSize; i++)
            {
                newTable.RowDefinitions.Add(new RowDefinition());
                newTable.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    Canvas canvas = new Canvas();
                    if ((i + j) % 2 == 0)
                    {
                        canvas.Background = new SolidColorBrush(whiteColor);
                    }
                    else
                    {
                        canvas.Background = new SolidColorBrush(blackColor);
                    }
                    canvas.SetValue(Grid.RowProperty, i);
                    canvas.SetValue(Grid.ColumnProperty, j);
                    newTable.Children.Add(canvas);
                }
            }
            return newTable;
        }
        public void setCell(Grid table, int i, int j)
        {
            Image image = new Image();
            image.SetValue(Grid.RowProperty, i);
            image.SetValue(Grid.ColumnProperty, j);


            Uri source;
            if ((i + j) % 2 == 0)
            {
                source = new Uri(@"/bdd_interface;component/img/white.png", UriKind.Relative);
            }
            else
            {
                source = new Uri(@"/bdd_interface;component/img/black.png", UriKind.Relative);
            }

            image.Source = new BitmapImage(source);
            table.Children.Add(image);
        }
        public void setClickableCell(Grid table, int i, int j)
        {
            Button button = new Button();
            button.SetValue(Grid.RowProperty, i);
            button.SetValue(Grid.ColumnProperty, j);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.BorderThickness = new Thickness(0);
            button.DataContext = (object)(new Tuple<int, int>(i, j));
            button.Click += checkQueen;
            table.Children.Add(button);
        }

        private void checkQueen(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Tuple<int, int> data = (Tuple<int, int>)button.DataContext;
            if (!isQueenCell(data.Item1, data.Item2))
            {
                if (isValidPlace(data.Item1, data.Item2))
                {
                    varTables[data.Item1, data.Item2] = true;
                    clearTable();
                    setCells();
                    checkIsEndOfGame();
                }
            }
            else
            {
                varTables[data.Item1, data.Item2] = false;
                clearTable();
                setCells();
            }
        }

        private void checkIsEndOfGame()
        {
            int countOfQueen = 0;
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    if (varTables[i, j] == true)
                        countOfQueen++;
                }
            }
            if (countOfQueen==tableSize)
            {
                MessageBox.Show("Поздравляем");
            }
        }

        private void setCells()
        {
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    if (varTables[i, j] == true)
                    {
                        setCell(table, i, j);                       
                    }
                    if (varTablesMain[i, j] == false)
                    {
                        if (varTables[i, j] == true)
                        {
                            setClickableCellQueen(table, i, j);
                        }
                        else
                        {
                            setClickableCell(table, i, j);
                        }
                    }
                }
            }
        }

        private void setClickableCellQueen(Grid table, int i, int j)
        {
            Button button = new Button();
            Uri source = new Uri(@"/bdd_interface;component/img/black.gif", UriKind.Relative);
            button.Background =new ImageBrush(new BitmapImage(source));
            button.SetValue(Grid.RowProperty, i);
            button.SetValue(Grid.ColumnProperty, j);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.BorderThickness = new Thickness(0);
            button.DataContext = (object)(new Tuple<int, int>(i, j));
            button.Click += checkQueen;
            table.Children.Add(button);
        }

        private void clearTable()
        {
            table.Children.Clear();
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    Canvas canvas = new Canvas();
                    if ((i + j) % 2 == 0)
                    {
                        canvas.Background = new SolidColorBrush(whiteColor);
                    }
                    else
                    {
                        canvas.Background = new SolidColorBrush(blackColor);
                    }
                    canvas.SetValue(Grid.RowProperty, i);
                    canvas.SetValue(Grid.ColumnProperty, j);
                    table.Children.Add(canvas);
                }
            }
        }

        private bool isValidPlace(int item1, int item2)
        {
            for (int i = 0; i < tableSize; i++)
            {
                if (varTables[item1, i] || varTables[i, item2])
                    return false;
            }
            for (int i = 0; i < tableSize; i++)
            {
                if (item2 - item1 + i < tableSize && item2 - item1 + i >= 0)
                    if (varTables[i, item2 - item1 + i])
                        return false;
            }
            for (int i = -tableSize; i < tableSize; i++)
            {
                if (item1 + i < tableSize && item2 - i >= 0 && item1 + i >= 0 && item2 - i < tableSize)
                    if (varTables[item1 + i, item2 - i])
                        return false;
            }
            return true;
        }

        private bool isQueenCell(int item1, int item2)
        {
            if (varTables[item1, item2] == true)
                return true;
            return false;
        }

        public delegate void WaitToEndOfProccess(int numberOfSolve, int tableSize, int init);
        private void Button_Click_Go(object sender, RoutedEventArgs e)
        {

            if (proc != null)
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }
            if (currentThread != null)
            {
                currentThread.Abort();
            }
            if (timerThread != null)
            {
                timerThread.Abort();
            }
            if (isFirstStart)
            {
                isFirstStart = false;
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(2, GridUnitType.Star);
                gridWithTimer.ColumnDefinitions.Add(columnDefinition);
                gridWithTimer.ColumnDefinitions.Add(new ColumnDefinition());
            }
            timerThread = new Thread(new ThreadStart(startTimer));
            timerThread.Start();

            showLoandingAnimation();

            int tableSize;
            if (int.TryParse(viewTableSize.Text, out tableSize))
            {
                if (tableSize == 2 && currentTactic < 3)
                {
                    solves = new string[4];
                    solves[0] = "0001";
                    solves[1] = "0010";
                    solves[2] = "0100";
                    solves[3] = "1000";
                    this.tableSize = tableSize;
                    this.countOfSolves = solves.Length;
                    showSolveBrutforce(0, 2);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        deleteLoandingAnimation();
                        stateToView.Content = string.Format("{0}/{1}", 1, solves.Length);
                    }));
                    return;
                }
                if (tableSize == 3 && currentTactic < 3)
                {
                    solves = new string[8];
                    solves[0] = "100000010";
                    solves[1] = "100001000";
                    solves[2] = "010000100";
                    solves[3] = "010000001";
                    solves[4] = "001100000";
                    solves[5] = "001000010";
                    solves[6] = "000100001";
                    solves[7] = "000001100";
                    this.tableSize = tableSize;
                    this.countOfSolves = solves.Length;
                    showSolveBrutforce(0, 3);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        deleteLoandingAnimation();
                        stateToView.Content = string.Format("{0}/{1}", 1, solves.Length);
                    }));
                    return;
                }
                if (currentTactic == 0)
                {
                    BDDSolver solver = new BDDSolver();
                    proc = solver.proccess(tableSize);
                    currentThread = new Thread(new ParameterizedThreadStart(checkProccessIsEndCUDD));
                    //  waitToEndOfProccess.BeginInvoke(tableSize, new AsyncCallback(showSolveCUDD), new Tuple<int, int>(firstSolve, tableSize));
                    var data = new Tuple<int, WaitToEndOfProccess>(tableSize, showSolveCUDD);
                    currentThread.Start(data);
                }
                if (currentTactic == 1)
                {
                    bruteForceSolver = new Bruteforce();
                    currentThread = new Thread(new ParameterizedThreadStart(bruteForceSolver.initSolvesFullBrutforce));

                    var data = new Tuple<int, WaitToEndOfProccess>(tableSize, showSolveBrutforce);
                    currentThread.Start(data);
                }
                if (currentTactic == 2)
                {
                    bruteForceSolver = new Bruteforce();
                    currentThread = new Thread(new ParameterizedThreadStart(bruteForceSolver.initSolvesOptimizeBrutforce));

                    var data = new Tuple<int, WaitToEndOfProccess>(tableSize, showSolveBrutforce);
                    currentThread.Start(data);
                }
                if (currentTactic == 3)
                {
                    if (timerThread != null)
                    {
                        timerThread.Abort();
                    }
                    string toViewTime = string.Format("{0} : {1} : {2}", 0, 0, 0);
                    Dispatcher.Invoke(new Action(() => timer.Content = toViewTime));

                    BDDSolver solver = new BDDSolver();
                    proc = solver.proccess(tableSize);
                    currentThread = new Thread(new ParameterizedThreadStart(checkProccessIsEndCUDD));
                    //  waitToEndOfProccess.BeginInvoke(tableSize, new AsyncCallback(showSolveCUDD), new Tuple<int, int>(firstSolve, tableSize));
                    var data = new Tuple<int, WaitToEndOfProccess>(-tableSize, showSolveCUDD);
                    currentThread.Start(data);
                }
            }
            else
            {
                MessageBox.Show("Введен неверный размер доски");
            }

        }
        private void showSolveBrutforce(int numberOfSolveToView, int tableSize, int init = 0)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (init == 1)
                {
                    currentSolve = 0;
                    this.tableSize = tableSize;
                    importSolvesFullBruteforce(tableSize, bruteForceSolver);
                    deleteLoandingAnimation();
                    stopTimer();
                }
                grid.Children.Clear();
                grid.Background = new SolidColorBrush(Colors.DarkGray);

                Grid newTable = createTable(tableSize);
                newTable.SetValue(Grid.RowProperty, 1);
                newTable.SetValue(Grid.ColumnProperty, 1);
                if (solves.Length > 0)
                {
                    for (int i = 0; i < tableSize; i++)
                    {
                        for (int j = 0; j < tableSize; j++)
                        {
                            if (solves[numberOfSolveToView][i * tableSize + j] == '1')
                            {
                                setCell(newTable, i, j);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Решений не найдено");
                }
                grid.Children.Add(newTable);
            }));
        }

        private void importSolvesFullBruteforce(int tableSize, Bruteforce solver)
        {
            solves = solver.solves;
            this.tableSize = tableSize;
            this.countOfSolves = solves.Length;
            stateToView.Content = string.Format("{0}/{1}", 1, solves.Length);
        }


        private void showLoandingAnimation()
        {
            importButton.Visibility = Visibility.Hidden;
            socketToLoandingAnimation.Visibility = Visibility.Visible;
        }
        private void initLoandingAnimation()
        {
            Image image = new Image();
            Uri source = new Uri(@"/bdd_interface;component/img/loanding.gif", UriKind.Relative);
            image.Source = new BitmapImage(source);


            var rotation = new RotateTransform(0);
            var rotationAnimation = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(5));
            rotationAnimation.RepeatBehavior = RepeatBehavior.Forever;


            rotation.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);
            image.LayoutTransform = rotation;
            socketToLoandingAnimation.Child = image;
            socketToLoandingAnimation.Visibility = Visibility.Hidden;
        }
        private void deleteLoandingAnimation()
        {
            socketToLoandingAnimation.Visibility = Visibility.Hidden;
            importButton.Visibility = Visibility.Visible;
        }




        private void exportSolvesCUDD(string fileName, string[] solves, int tableSize)
        {
            try
            {
                int countOfSolves = 0;
                using (StreamReader sr = new StreamReader(pathToCountOfSolves))
                {
                    string count = sr.ReadLine();
                    countOfSolves = int.Parse(count);
                }
                using (StreamReader sr = new StreamReader(pathToSolves))
                {
                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        for (int i = 0; i < countOfSolves; i++)
                        {
                            string solve = sr.ReadLine();
                            for (int k = 0; k < tableSize; k++)
                            {
                                for (int j = 0; j < tableSize; j++)
                                {
                                    sw.Write(solve[k * tableSize + j]);
                                }
                                sw.WriteLine();
                            }
                            sw.WriteLine();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка экспорта данных");
            }
        }
        public void showSolveCUDD(int numberOfSolve, int tableSize, int init = 0)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (init == 1)
                {
                    currentSolve = 0;
                    this.tableSize = tableSize;
                    importSolvesCUDD(tableSize);
                    deleteLoandingAnimation();
                    stopTimer();
                }
                if (init == -1)
                {
                    currentSolve = 0;
                    this.tableSize = tableSize;
                    importSolvesCUDD(tableSize);
                    deleteLoandingAnimation();
                }
                grid.Children.Clear();
                grid.Background = new SolidColorBrush(Colors.DarkGray);
                Grid newTable = createTable(tableSize);
                table = newTable;
                newTable.SetValue(Grid.RowProperty, 1);
                newTable.SetValue(Grid.ColumnProperty, 1);
                if (init != -1)
                {
                    for (int i = 0; i < tableSize; i++)
                    {
                        for (int j = 0; j < tableSize; j++)
                        {
                            if (solves[numberOfSolve][i * tableSize + j] == '1')
                            {
                                setCell(newTable, i, j);
                            }
                        }
                    }
                }
                if (init == -1)
                {
                    setCellByRandom(newTable, tableSize);
                }
                grid.Children.Add(newTable);
            }));
        }

        private void setCellByRandom(Grid newTable, int tableSize)
        {
            varTables = new bool[tableSize, tableSize];
            varTablesMain = new bool[tableSize, tableSize];
            Random random = new Random(DateTime.Now.Millisecond);
            int numberOfSolve = random.Next(solves.Length - 1);
            int[] toView = setRandomNumbers(random, countCellsToSet, tableSize);
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    if (toView.Contains(i) && solves[numberOfSolve][i * tableSize + j] == '1')
                    {
                        setCell(newTable, i, j);
                        varTables[i, j] = true;
                        varTablesMain[i, j] = true;
                    }
                    else
                    {
                        setClickableCell(newTable, i, j);
                    }
                }
            }
        }

        private int[] setRandomNumbers(Random random, int countCellsToSet, int tableSize)
        {
            int[] result = new int[countCellsToSet];
            for (int i = 0; i < countCellsToSet; i++)
            {
                result[i] = -1;
            }
            for (int i = 0; i < countCellsToSet; i++)
            {
                while (true)
                {
                    int randomNumber = random.Next(tableSize);
                    if (!result.Contains(randomNumber))
                    {
                        result[i] = randomNumber;
                        break;
                    }
                }
            }
            return result;
        }

        public void importSolvesCUDD(int n)
        {

            pathToSolves = "solvers/cudd_solves.txt";
            pathToCountOfSolves = "solvers/cudd_countOfSolves.txt";

            try
            {
                using (StreamReader sr = new StreamReader(pathToCountOfSolves))
                {
                    if (int.TryParse(sr.ReadLine(), out countOfSolves))
                    {
                        stateToView.Content = string.Format("{0}/{1}", 1, countOfSolves);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка связаная с содержимым файла с количеством решений");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка связаная с чтением файла с количеством решений");
                return;
            }
            try
            {
                using (StreamReader sr = new StreamReader(pathToSolves))
                {
                    solves = new string[Math.Min(600, countOfSolves)];
                    for (int i = 0; i < Math.Min(600, countOfSolves); i++)
                    {
                        solves[i] = sr.ReadLine().Substring(0, n * n);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка связаная с чтением файла с решениями");
            }

        }
        bool isItTrueFileCUDD(string pathIsFinish, int n)
        {
            using (StreamReader sr = new StreamReader(pathIsFinish))
            {
                int newN;
                var s = sr.ReadLine();
                if (int.TryParse(s, out newN))
                {
                    if (newN == n)
                        return true;
                    else
                        return false;
                }

            }
            return false;
        }
        public void checkProccessIsEndCUDD(object objectData)
        {
            Tuple<int, WaitToEndOfProccess> data = (Tuple<int, MainWindow.WaitToEndOfProccess>)objectData;
            int tableSize = data.Item1;
            int init = 1;
            if (tableSize < 0)
            {
                init = -1;
                tableSize = -tableSize;
            }
            string pathToSolves = "solvers/cudd_solves.txt";
            string pathToCountOfSolves = "solvers/cudd_countOfSolves.txt";
            string pathIsFinish = "solvers/cudd_finished.txt";
            int countOfSolve = -1;
            bool messageShowed = false;
            while (true)
            {
                if (File.Exists(pathIsFinish))
                {
                    if (isItTrueFileCUDD(pathIsFinish, tableSize))
                        using (StreamReader sr = new StreamReader(pathToCountOfSolves))
                        {
                            var s = sr.ReadLine();
                            if (int.TryParse(s, out countOfSolve))
                            {
                                File.Delete(pathIsFinish);
                                data.Item2(0, tableSize, init);
                                return;
                            }
                            //else   баги
                            //{
                            //    MessageBox.Show("Ошибка с файлом с количеством решений","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                            //    return;
                            //}
                        }
                }
                else
                {
                    if (proc.HasExited && !messageShowed)
                    {
                        MessageBox.Show("Произошла ошибка, скорее всего у вас не достаточно оперативной памяти для решения данной задачи", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        messageShowed = true;
                        Dispatcher.Invoke(new Action(() => deleteLoandingAnimation()));
                        return;
                    }
                }
                Thread.Sleep(100);
            }
        }

        public void startTimer()
        {
            DateTime startTime = DateTime.Now;
            while (true)
            {
                TimeSpan delta = DateTime.Now - startTime;
                string toViewTime = string.Format("{0} : {1} : {2}",
                    (int)delta.TotalMinutes,
                    delta.Seconds,
                    delta.Milliseconds);
                Dispatcher.Invoke(new Action(() => timer.Content = toViewTime));
                Thread.Sleep(77);
            }
        }
        public void stopTimer()
        {
            if (timerThread != null)
            {
                timerThread.Abort();
            }
        }

        private void Button_Click_Export_Solves(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog();
            exportSolvesCUDD(sfd.FileName, solves, tableSize);
        }
        private void Button_Show_Next_Solve(object sender, RoutedEventArgs e)
        {
            if (solves == null)
            {
                return;
            }
            if (currentTactic == 0)
            {
                if (currentSolve < Math.Min(600, countOfSolves) - 1)
                {
                    currentSolve++;
                    stateToView.Content = string.Format("{0}/{1}", currentSolve + 1, countOfSolves);
                    showSolveCUDD(currentSolve, tableSize);
                }
                else
                {
                    if (currentSolve == 598)
                    {
                        MessageBox.Show("Остальные решения можно только выгрузить");
                    }
                }
            }
            if (currentTactic == 1 || currentTactic == 2)
            {
                if (currentSolve < countOfSolves - 1)
                {
                    currentSolve++;
                    stateToView.Content = string.Format("{0}/{1}", currentSolve + 1, countOfSolves);
                    showSolveBrutforce(currentSolve, tableSize);
                }
            }
        }
        private void Button_Show_Prev_Solve(object sender, RoutedEventArgs e)
        {
            if (solves == null)
            {
                return;
            }
            if (currentTactic == 0)
            {
                if (currentSolve > 0)
                {
                    currentSolve--;
                    stateToView.Content = string.Format("{0}/{1}", currentSolve + 1, countOfSolves);
                    showSolveCUDD(currentSolve, tableSize);
                }
            }
            if (currentTactic == 1 || currentTactic == 2)
            {
                if (currentSolve > 0)
                {
                    currentSolve--;
                    stateToView.Content = string.Format("{0}/{1}", currentSolve + 1, countOfSolves);
                    showSolveBrutforce(currentSolve, tableSize);
                }
            }
        }
        private void Button_Click_Next_Tactic(object sender, RoutedEventArgs e)
        {
            currentTactic = (currentTactic + 1) % tacticNames.Length;
            nameOfTactic.Content = tacticNames[currentTactic];
        }
        private void Button_Click_Prev_Tactic(object sender, RoutedEventArgs e)
        {
            currentTactic = (currentTactic - 1 + tacticNames.Length) % tacticNames.Length;
            nameOfTactic.Content = tacticNames[currentTactic];
        }
        protected override void OnClosed(EventArgs e)
        {
            if (proc != null)
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }

            }
            if (currentThread != null)
            {
                currentThread.Abort();
            }
            if (timerThread != null)
            {
                timerThread.Abort();
            }
            base.OnClosed(e);
        }
    }
}
