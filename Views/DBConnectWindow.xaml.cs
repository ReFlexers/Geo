using Geo.ViewModels;
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

namespace Geo.Views
{
    /// <summary>
    /// Логика взаимодействия для DBConnectWindow.xaml
    /// </summary>
    public partial class DBConnectWindow : Window
    {
        public DBConnectWindow()
        {
            InitializeComponent();
            DataContext = new DBConnectViewModel();
        }
    }
}
