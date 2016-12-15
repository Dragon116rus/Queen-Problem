using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace bdd_interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int tableSize = 4;
        Color whiteColor = Color.FromRgb(248, 230, 173);
        Color blackColor = Color.FromRgb(183, 134, 94);
        public MainWindow()
        {
            InitializeComponent();

            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = "cudd.exe";
            procInfo.Arguments = "11";            
            procInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            Process proc=Process.Start(procInfo);
            proc.Kill();

            grid.Background = new SolidColorBrush(Colors.Gray);
            Grid newTable = createTable(tableSize);
            newTable.SetValue(Grid.RowProperty, 1);
            newTable.SetValue(Grid.ColumnProperty, 1);
            setCell(newTable, 1, 1);
            grid.Children.Add(newTable);
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
    }
}
