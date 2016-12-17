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
        BDDSolver solver;
        int currentSolve = 0;
        string[] solves;
        int countOfSolves;
        int tableSize;
        public MainWindow()
        {
            InitializeComponent();


            //grid.Background = new SolidColorBrush(Colors.Gray);
            //Grid newTable = createTable(tableSize);
            //newTable.SetValue(Grid.RowProperty, 1);
            //newTable.SetValue(Grid.ColumnProperty, 1);
            //setCell(newTable, 1, 1);
            //grid.Children.Add(newTable);
        }

        public Grid createTable(int sizeOfTable)
        {
            Grid newTable = new Grid();
            for (int i = 0; i < sizeOfTable; i++)
            {
                newTable.RowDefinitions.Add(new RowDefinition());
                newTable.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < sizeOfTable; i++)
            {
                for (int j = 0; j < sizeOfTable; j++)
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
        public delegate void WaitToEndOfProccess();
        private void Button_Click_Go(object sender, RoutedEventArgs e)
        {

            if (proc != null)
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }                
            }
            showLoandingAnimation();
            solver = new BDDSolver();
            int n;
            if (int.TryParse(viewTableSize.Text, out n))
            {
                proc = solver.proccess(n);
                WaitToEndOfProccess waitToEndOfProccess = checkProccessIsEnd;
                waitToEndOfProccess.BeginInvoke(new AsyncCallback(showSolve), new Tuple<int, int>(0, n));
            }
            else
            {
                MessageBox.Show("Введен неверный размер доски");
            }

        }

        private void showLoandingAnimation()
        {
            throw new NotImplementedException();
        }
        private void deleteLoandingAnimation()
        {
            throw new NotImplementedException();
        }
        private void Button_Show_Next_Solve(object sender, RoutedEventArgs e)
        {
            if (solves == null)
            {
                return;
            }

            if (currentSolve < Math.Min(600, countOfSolves) - 1)
            {
                currentSolve++;
                stateToView.Content = string.Format("{0}/{1}", currentSolve + 1, countOfSolves);
                showSolve(currentSolve, tableSize);
            }
            else
            {
                if (currentSolve == 598)
                {
                    MessageBox.Show("Остальные решения можно только выгрузить");
                }
            }
        }
        private void Button_Show_Prev_Solve(object sender, RoutedEventArgs e)
        {
            if (solves == null)
            {
                return;
            }
            if (currentSolve > 0)
            {
                currentSolve--;
                stateToView.Content = string.Format("{0}/{1}", currentSolve + 1, countOfSolves);
                showSolve(currentSolve, tableSize);
            }
        }
        public void showSolve(IAsyncResult resObj)
        {
            Dispatcher.Invoke(() =>
            {
                grid.Children.Clear();
                grid.Background = new SolidColorBrush(Colors.DarkGray);
                currentSolve = 0;

                int numberOfSolve = ((Tuple<int, int>)resObj.AsyncState).Item1;
                int n = ((Tuple<int, int>)resObj.AsyncState).Item2;

                tableSize = n;
                importSolves();
                Grid newTable = newTable = createTable(n);
                newTable.SetValue(Grid.RowProperty, 1);
                newTable.SetValue(Grid.ColumnProperty, 1);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (solves[numberOfSolve][i * n + j] == '1')
                        {
                            setCell(newTable, i, j);
                        }
                    }
                }
                grid.Children.Add(newTable);
                deleteLoandingAnimation();
            });
        }

        

        public void showSolve(int numberOfSolve, int n)
        {
            Dispatcher.Invoke(() =>
            {
                grid.Children.Clear();
                grid.Background = new SolidColorBrush(Colors.DarkGray);


                Grid newTable = newTable = createTable(n);
                newTable.SetValue(Grid.RowProperty, 1);
                newTable.SetValue(Grid.ColumnProperty, 1);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (solves[numberOfSolve][i * n + j] == '1')
                        {
                            setCell(newTable, i, j);
                        }
                    }
                }
                grid.Children.Add(newTable);
            });
        }
        public void importSolves()
        {

            string pathToSolves = "solvers/cudd_solves.txt";
            string pathToCountOfSolves = "solvers/cudd_countOfSolves.txt";

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
                    solves = new string[Math.Min(100, countOfSolves)];
                    for (int i = 0; i < Math.Min(100, countOfSolves); i++)
                    {
                        solves[i] = sr.ReadLine();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка связаная с чтением файла с решениями");
            }

        }
        public void checkProccessIsEnd()
        {
            string pathToSolves = "solvers/cudd_solves.txt";
            string pathToCountOfSolves = "solvers/cudd_countOfSolves.txt";
            string pathIsFinish = "solvers/cudd_finished.txt";
            int countOfSolve = -1;
            while (true)
            {
                if (File.Exists(pathIsFinish))
                {
                    using (StreamReader sr = new StreamReader(pathToCountOfSolves))
                    {
                        var s = sr.ReadLine();
                        if (int.TryParse(s, out countOfSolve))
                        {
                            File.Delete(pathIsFinish);
                            return;
                        }
                    }
                }
                Thread.Sleep(500);
            }
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
            base.OnClosed(e);
        }

    }
}
