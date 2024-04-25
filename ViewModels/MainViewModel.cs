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
    public class MainViewModel : NotifyProperty
    {
        ApplicationContext db = ApplicationContext.getInstance();
        DrawingImage image;

        Customer selectedCustomer;
        Project selectedProject;
        Area selectedArea;
        public ObservableCollection<Customer> Customers { get=>db.Customers.Local.ToObservableCollection(); }
        public MainViewModel()
        {
            AddCustomerCommand = new(AddCustomer);
            DeleteCustomerCommand = new(DeleteCustomer, (o)=>SelectedCustomer!=null);
            AddProjectCommand = new(AddProject, (o) => SelectedCustomer != null);
            DeleteProjectCommand = new(DeleteProject, (o) => SelectedProject != null);
            AddAreaCommand = new(AddArea, (o) => SelectedProject != null);
            DeleteAreaCommand = new(DeleteArea, (o) => SelectedArea != null);
            OpenAreaCommand = new(OpenArea);
    
        }
        public RelayCommand AddCustomerCommand { get; set; }
        public RelayCommand DeleteCustomerCommand { get; set; }
        public RelayCommand AddProjectCommand { get; set; }
        public RelayCommand DeleteProjectCommand { get; set; }
        public RelayCommand AddAreaCommand { get; set; }
        public RelayCommand DeleteAreaCommand { get; set; }
        public RelayCommand OpenAreaCommand { get; set; }
     

        private RelayCommand _minimizeWindow;
        public RelayCommand MinimizeWindow
        {
            get
            {
                _minimizeWindow = new RelayCommand(obj =>
                {
                    if (obj is Window window)
                    {
                        window.WindowState = WindowState.Minimized;
                    }
                });
                return _minimizeWindow;
            }
        }


        private RelayCommand _closeApplication;
        public RelayCommand CloseApplication
        {
            get
            {
                _closeApplication = new RelayCommand(obj =>
                {
                    Application.Current.MainWindow.Close();
                });
                return _closeApplication;
            }
        }

        void AddCustomer(object obj)
        {
            var c = new Customer() { Name="", Phone="" };
            if (new CustomerWindow(c).ShowDialog() == false) return;
            db.Customers.Add(c); 
            db.SaveChanges();
            SelectedCustomer = c;
        }
        void DeleteCustomer(object obj)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить заказчика?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Customers.Remove(SelectedCustomer);
            db.SaveChanges();
        }
        void AddProject(object obj)
        {
            var p = new Project() { Name = "", Address = "" , Customer = SelectedCustomer };
            if (new ProjectWindow(p).ShowDialog() == false) return;
            db.Projects.Add(p);
            db.SaveChanges();
            SelectedProject = p;
        }
        void DeleteProject(object obj)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить проект?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Projects.Remove(SelectedProject);
            db.SaveChanges();
        }
        void AddArea(object obj)
        {
            var a = new Area() { Project=SelectedProject };
            db.Areas.Add(a);
            db.SaveChanges();
            a.Name = $"Площадь {a.Id}";
            db.SaveChanges();
            OnPropertyChanged(nameof(SelectedProject));
            SelectedArea = a;
        }
        void DeleteArea(object obj)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить площадь?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Areas.Remove(SelectedArea);
            db.SaveChanges();
            OnPropertyChanged(nameof(SelectedProject));
            db.Areas.Load();
        }
        void OpenArea(object obj)
        {
            new AreaWindow() { 
                    DataContext=new AreaViewModel((Area)obj) 
                }.ShowDialog();
            OnPropertyChanged(nameof(SelectedProject.Areas));
            OnPropertyChanged(nameof(obj));
            Redraw();
        }
  
        void Redraw()
        {
            var vis = new VisDraw();
            foreach (var area in SelectedProject?.Areas ?? new())
            {
                area.Draw(vis, area == SelectedArea ? Brushes.Yellow : (area.IsCorrect() ? Brushes.Green : Brushes.Red));
                foreach (var profile in area.Profiles ?? new())
                    profile.Draw(vis, area == SelectedArea ? Brushes.Yellow : (profile.IsCorrect() ? Brushes.Green : Brushes.Red));
            }
            Image = vis.Render();
        }
        public DrawingImage Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }
        public Project SelectedProject
        {
            get => selectedProject;
            set
            {
                selectedProject = value;
                OnPropertyChanged(nameof(SelectedProject));
                Redraw();
            }
        }
        public Area SelectedArea
        {
            get => selectedArea;
            set
            {
                selectedArea = value;
                OnPropertyChanged(nameof(SelectedArea));
                Redraw();
            }
        }
        
    }
}
