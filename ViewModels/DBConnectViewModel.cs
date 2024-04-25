using Geo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Geo.Models.Db;
using Geo.Views;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls;
using GeoMeasure.Models.Db;


namespace Geo.ViewModels
{
    public class DBConnectViewModel : NotifyProperty
    {
        private string dbName;
        public string DbName
        {
            get { return dbName; }
            set
            {
                dbName = value;
                OnPropertyChanged(nameof(DbName));
            }
        }

        public ICommand ConnectCommand { get; set; }
        public ICommand EnterDefaultCommand { get; set; }

        public DBConnectViewModel()
        {
            ConnectCommand = new RelayCommand(ConnectToDatabase);
            EnterDefaultCommand = new RelayCommand(EnterDefault);
        }

        private void EnterDefault(object obj)
        {
            DbName = "Geo1223";
            OnPropertyChanged(nameof(DbName));
        }

        private void ConnectToDatabase(object obj)
        {
            if (!string.IsNullOrWhiteSpace(DbName))
            {
                using (var db = new ApplicationContext(DbName.ToString()))
                {
                    bool exists = db.Database.CanConnect();
                    if (exists)
                    {
                        MessageBox.Show($"База данных {DbName} существует, подключаемся.", "Подключение");
                    }
                    else
                    {
                        db.Database.EnsureCreated();
                        MessageBox.Show($"Создана новая база данных {DbName}, подключаемся.", "Подключение");
                    }
                    var win = new MainWindow()
                    {
                        DataContext = new MainViewModel()
                    };
                    win.Show();
                    Application.Current.MainWindow.Close();
                    Application.Current.MainWindow = win;
                    OnPropertyChanged(nameof(obj));
                }
            }
        }
    }
}
