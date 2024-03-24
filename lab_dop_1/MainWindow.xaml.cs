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

namespace lab_dop_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewData mainViewData;
        public MainWindow()
        {
            InitializeComponent();

            mainViewData = new ViewData();
            try
            {
                mainViewData.InitializeData();
                TextBoxString.Text = mainViewData.DataListObj?.Key;
                MainList.ItemsSource = mainViewData.DataListObj?.DataItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}