using Geo.Models.Db;
using Geo.Models;
using Geo.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls;

namespace Geo.ViewModels
{
    class AreaViewModel : NotifyProperty
    {
        ApplicationContext db = ApplicationContext.getInstance();
        DrawingImage image;

        Profile selectedProfile;
        AreaPoint selectedPoint;
        public Area Area { get; set; }

        public AreaViewModel() : this(null) { }
        public AreaViewModel(Area area)
        {
            Area = area;
            AddPointCommand = new(AddPoint);
            AddRandomPointCommand = new(AddRandomPoint);
            DeletePointCommand = new(DeletePoint, (o) => SelectedPoint != null);
            AddProfileCommand = new(AddProfile);
            DeleteProfileCommand = new(DeleteProfile, (o) => SelectedProfile != null);
            OpenProfileCommand = new(OpenProfile);
            SavePointCommand = new(SavePoint);
            Redraw();
        }
        public RelayCommand AddPointCommand { get; set; }
        public RelayCommand AddRandomPointCommand { get; set; }
        public RelayCommand DeletePointCommand { get; set; }
        public RelayCommand AddProfileCommand { get; set; }
        public RelayCommand DeleteProfileCommand { get; set; }
        public RelayCommand OpenProfileCommand { get; set; }
        public RelayCommand SavePointCommand { get; set; }

        private RelayCommand _minimizeWindow;
        public RelayCommand MinimizeWindow
        {
            get
            {
                if (_minimizeWindow == null)
                {
                    _minimizeWindow = new RelayCommand(obj =>
                    {
                        if (obj is Window window)
                        {
                            window.WindowState = WindowState.Minimized;
                        }
                    });
                }
                return _minimizeWindow;
            }
        }


        private RelayCommand _closeWindow;
        public RelayCommand CloseWindow
        {
            get
            {
                if (_closeWindow == null)
                {
                    _closeWindow = new RelayCommand(obj =>
                    {
                        if (obj is Window window)
                        {
                            window.Close();
                        }
                    });
                }
                return _closeWindow;
            }
        }

        void AddPoint(object obj)
        {
            var p = new AreaPoint() { X=0, Y=0, Area=Area };
            db.AreaPoints.Add(p);
            db.SaveChanges();
            SelectedPoint = p;
            OnPropertyChanged(nameof(Area));
            Area.CalcArea = 0;
            Redraw();
        }
        void AddRandomPoint(object obj)
        {
            AreaPoint pp = Area.Points?.Count > 0 ? Area.Points.Last() : new() { X = 0, Y = 0 }, p;
            int off = 25;
            Random r = new();
            while (true)
            {
                p = new AreaPoint() { X = pp.X + r.Next(-off, off), Y = pp.Y + r.Next(-off, off), Area = Area };
                db.AreaPoints.Add(p);
                if (Area.IsCorrect()) break;
                else db.AreaPoints.Remove(p);
            }
            db.SaveChanges();
            SelectedPoint = p;
            OnPropertyChanged(nameof(Area));
            Area.CalcArea = 0;
            Redraw();
        }
        void DeletePoint(object obj)
        {
            db.AreaPoints.Remove(SelectedPoint);
            db.SaveChanges();
            Area.CalcArea=0;
            Redraw();
        }
        void AddProfile(object obj)
        {
            var p = new Profile() { Area=Area };
            db.Profiles.Add(p);
            db.SaveChanges();
            SelectedProfile = p;
            OnPropertyChanged(nameof(Area));
            Redraw();
        }
        void DeleteProfile(object obj)
        {
            db.Profiles.Remove(SelectedProfile);
            db.SaveChanges();
            OnPropertyChanged(nameof(Area));
            Redraw();
        }
        void OpenProfile(object obj)
        {

            new ProfileWindow()
            {

                DataContext = new ProfileViewModel((Profile)obj)
            }.ShowDialog();
            OnPropertyChanged(nameof(obj));
            Redraw();
        }
        void SavePoint(object obj)
        {
            if (obj is AreaPoint)
            {
                db.Entry((AreaPoint)obj).State = EntityState.Modified;
                db.SaveChanges();
                Area.CalcArea = 0;
                Redraw();
            }
        }
      

        void Redraw()
        {
            var vd = new VisDraw();
            Area.Draw(vd, Area.IsCorrect()? Brushes.Green : Brushes.Red);
            foreach (var p in Area.Points ?? new())
                vd.DrawCircle(p.X, p.Y, 0.6, SelectedPoint == p ? Brushes.Yellow : Brushes.Green);
            foreach (var p in Area.Profiles ?? new())
                p.Draw(vd, p == SelectedProfile ? (p.IsCorrect() ? Brushes.Yellow : Brushes.Orange) 
                                                : (p.IsCorrect() ? Brushes.Green : Brushes.Red));
            Image = vd.Render();

        }
        public string? AreaName
        {
            get => Area.Name;
            set
            {
                Area.Name = value;
                db.Entry(Area).State = EntityState.Modified;
                db.SaveChanges();
            }
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
        public Profile SelectedProfile
        {
            get => selectedProfile;
            set
            {
                selectedProfile = value;
                OnPropertyChanged(nameof(SelectedProfile));
                Redraw();
            }
        }
        public AreaPoint SelectedPoint
        {
            get => selectedPoint;
            set
            {
                selectedPoint = value;
                OnPropertyChanged(nameof(SelectedPoint));
                Redraw();
            }
        }
        
    }
}
