using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using lab_code_3_3;
using Microsoft.Win32;

namespace lab_1_wpf
{

    public partial class MainWindow : Window
    {
        ViewData mainViewData;
        public MainWindow()
        {
            InitializeComponent();

            mainViewData = new ViewData();
            BindConnections(mainViewData);
        }
        public void BindConnections(ViewData VD)
        {
            // Ввод границ сетки
            ParseBoundaries SD_Converter = new ParseBoundaries();
            Binding GridBoundariesBoxBind = new Binding();
            GridBoundariesBoxBind.Source = VD;
            GridBoundariesBoxBind.Path = new PropertyPath("GridBoundaries");
            GridBoundariesBoxBind.Converter = SD_Converter;
            GridBoundariesBox.SetBinding(TextBox.TextProperty, GridBoundariesBoxBind);
            // Ввод числа узлов сетки
            Binding NumberOfNodesBoxBind = new Binding(); 
            NumberOfNodesBoxBind.Source = VD;
            NumberOfNodesBoxBind.Path = new PropertyPath("NodesNum");
            NumberOfNodesBox.SetBinding(TextBox.TextProperty, NumberOfNodesBoxBind);
            //Выбор функции для вычисления значений компонент поля
            Binding FuncBoxBind = new Binding(); 
            FuncBoxBind.Source = VD;
            FuncBoxBind.Path = new PropertyPath("FuncId");
            FuncBox.SetBinding(ComboBox.SelectedIndexProperty, FuncBoxBind);
            //Выбор типа поля
            Binding UniformBoxBind = new Binding();
            UniformBoxBind.Source = VD;
            UniformBoxBind.Path = new PropertyPath("IsGridUniform");
            UniformBox.SetBinding(ComboBox.SelectedIndexProperty, UniformBoxBind);
            // Ввод числа узлов сглаживающего сплайна
            Binding SmoothingSplineBoxBind = new Binding();
            SmoothingSplineBoxBind.Source = VD;
            SmoothingSplineBoxBind.Path = new PropertyPath("SmoothingSplineNum");
            SmoothingSplineBox.SetBinding(TextBox.TextProperty, SmoothingSplineBoxBind);
            // Ввод числа узлов равномерной сетки
            Binding UniformNumBoxBind = new Binding();
            UniformNumBoxBind.Source = VD;
            UniformNumBoxBind.Path = new PropertyPath("UniformNum");
            UniformNumBox.SetBinding(TextBox.TextProperty, UniformNumBoxBind);
            // Ввод коэффициента расхождения
            Binding DiscrepancyRateBoxBind = new Binding();
            DiscrepancyRateBoxBind.Source = VD;
            DiscrepancyRateBoxBind.Path = new PropertyPath("DiscrepancyRate");
            DiscrepancyRateBox.SetBinding(TextBox.TextProperty, DiscrepancyRateBoxBind);
            // Ввод максимального числа итерваций
            Binding MaxIterationsBoxBind = new Binding();
            MaxIterationsBoxBind.Source = VD;
            MaxIterationsBoxBind.Path = new PropertyPath("MaxIterations");
            MaxIterationsBox.SetBinding(TextBox.TextProperty, MaxIterationsBoxBind);
        }
        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string FilePath = saveFileDialog?.ShowDialog() == true ? saveFileDialog.FileName : "";
                if (!string.IsNullOrEmpty(FilePath))
                {
                    mainViewData.InitializeThroughControl();
                    mainViewData.Save(FilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DataFromControlsItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mainViewData.InitializeThroughControl();
                mainViewData.InitializeData();
                mainViewData.Calculation();
                MainList.ItemsSource = mainViewData.SplineDataObj?.splineResults;
                SecondList.ItemsSource = mainViewData.SplineDataObj?.ResultOnAddonGrid;
                IterCntBox.AppendText(mainViewData.SplineDataObj?.ActualNumberOfIterations.ToString() + "\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataFromFileItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string FilePath = openFileDialog?.ShowDialog() == true ? openFileDialog.FileName : "";
                if (!string.IsNullOrEmpty(FilePath))
                {
                    mainViewData.Load(FilePath);
                    mainViewData.InitializeData();
                    mainViewData.Calculation();
                    mainViewData.UpData();
                    GridBoundariesBox.Text = $"{mainViewData.GridBoundaries[0]} {mainViewData.GridBoundaries[1]}";
                    NumberOfNodesBox.Text = $"{mainViewData.NodesNum}";
                    MainList.ItemsSource = mainViewData.SplineDataObj?.splineResults;
                    SecondList.ItemsSource = mainViewData.SplineDataObj?.ResultOnAddonGrid;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) & e.Text[0] != '.')
            {
                e.Handled = true;
                MessageBox.Show("Введите только числа");
            }
        }

    }
}