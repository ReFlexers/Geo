using Geo.Models.Db;
using Geo.Models;
using Geo.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using GeoMeasure.Models.Db;

namespace Geo.ViewModels
{
    class ProfileViewModel : NotifyProperty
    {
        ApplicationContext db = ApplicationContext.getInstance();
        DrawingImage image;
        DrawingImage graphImage;

        Picket selectedPicket;
        ProfilePoint selectedPoint;
        public Profile Profile { get; set; }
        public ObservableCollection<Operator> Operators { get => db.Operators.Local.ToObservableCollection(); }
        public ProfileViewModel() : this(null) { }
        public ProfileViewModel(Profile prof)
        {
            Profile = prof;
            AddOperatorCommand = new(AddOperator);
            DeleteOperatorCommand = new(DeleteOperator, (o) => SelectedOperator != null);
            AddPointCommand = new(AddPoint);
            AddRandomPointCommand = new(AddRandomPoint);
            DeletePointCommand = new(DeletePoint, (o) => SelectedPoint != null);
            AddPicketCommand = new(AddPicket);
            AddRandomPicketCommand = new(AddRandomPicket);
            DeletePicketCommand = new(DeletePicket, (o) => SelectedPicket != null);
            SavePicketCommand = new(SavePicket);
            SavePointCommand = new(SavePoint);
            Redraw();
        }
        public RelayCommand AddOperatorCommand { get; set; }
        public RelayCommand DeleteOperatorCommand { get; set; }
        public RelayCommand AddPointCommand { get; set; }
        public RelayCommand AddRandomPointCommand { get; set; }
        public RelayCommand DeletePointCommand { get; set; }
        public RelayCommand AddPicketCommand { get; set; }
        public RelayCommand AddRandomPicketCommand { get; set; }
        public RelayCommand DeletePicketCommand { get; set; }
        public RelayCommand SavePicketCommand { get; set; }
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
        void AddOperator(object obj)
        {
            var c = new Operator() { Name = "", Surname = "" };
            if (new OperatorWindow(c).ShowDialog() == false) return;
            db.Operators.Add(c);
            OnPropertyChanged(nameof(Operators));
            db.SaveChanges();
            SelectedOperator = c;
        }
        
        void DeleteOperator(object obj)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить оператора?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Operators.Remove(SelectedOperator!);
            SelectedOperator = null!;
            db.SaveChanges();
        }
        void AddPoint(object obj)
        {
            var p = new ProfilePoint() { X = 0, Y = 0, Profile = Profile };
            db.ProfilePoints.Add(p);
            db.SaveChanges();
            SelectedPoint = p;
            OnPropertyChanged(nameof(Profile));
            Redraw();
        }
        void AddRandomPoint(object obj)
        {
            ProfilePoint p, pp = Profile.Points?.Count > 0 ? Profile.Points.Last() : new() { X = Profile.Area.Points[0].X, Y=Profile.Area.Points[0].Y };
            int off = 15;
            Random r = new();
            while (true)
            {
                p = new ProfilePoint() { X = pp.X + r.Next(-off, off), Y = pp.Y + r.Next(-off, off), Profile = Profile };
                db.ProfilePoints.Add(p);
                if (Profile.IsCorrect()) break;
                else db.ProfilePoints.Remove(p);
            }
            db.SaveChanges();
            SelectedPoint = p;
            OnPropertyChanged(nameof(Profile));
            Redraw();
        }
        void DeletePoint(object obj)
        {
            db.ProfilePoints.Remove(SelectedPoint);
            db.SaveChanges();
            Redraw();
        }
        void AddPicket(object obj)
        {
            var p = new Picket() { Profile = Profile };
            db.Pickets.Add(p);
            db.SaveChanges();
            SelectedPicket = p;
            OnPropertyChanged(nameof(Profile));
            Redraw();
        }
        void AddRandomPicket(object obj)
        {
            Random r = new();

            ProfilePoint pp = Profile.Points?.Count > 0 ? Profile.Points[r.Next(Profile.Points.Count)] : new() { X = 0, Y = 0 };
            int off = 10;

            // Получаем максимальные значения X и Th из базы данных
            int previousX = (int)(db.Pickets.Any() ? db.Pickets.Max(p => p.X) : 0);
            int previousTh = (int)(db.Pickets.Any() ? db.Pickets.Max(p => p.H) : 0);

            // Генерируем новые значения X и Th, которые будут больше или равны предыдущим
            int newX = previousX + r.Next(off);
            int newTh = previousTh + r.Next(off);

            // Генерация положительных значений для Ra
            double newRa = r.Next(1, off);

            var p = new Picket() { Profile = Profile, X = newX, Y = pp.Y + r.Next(-off, off), H = newTh, A = newRa };

            db.Pickets.Add(p);
            db.SaveChanges();
            SelectedPicket = p;
            OnPropertyChanged(nameof(Profile));
            Redraw();
        }


        void DeletePicket(object obj)
        {
            db.Pickets.Remove(SelectedPicket);
            db.SaveChanges();
            OnPropertyChanged(nameof(Profile));
            Redraw();
        }
        void SavePicket(object obj)
        {
            if (obj is Picket)
            {
                db.Entry((Picket)obj).State = EntityState.Modified;
                db.SaveChanges();
                Redraw();
            }
        }
        void SavePoint(object obj)
        {
            if (obj is ProfilePoint)
            {
                db.Entry((ProfilePoint)obj).State = EntityState.Modified;
                db.SaveChanges();
                Redraw();
            }
        }
 
        void Redraw()
        {
            var pickets = Profile.OrderPickets();

            var vd = new VisDraw();
            foreach (var p in pickets)
                vd.DrawLine(p.proj.X, p.proj.Y, p.pic.X, p.pic.Y, Brushes.Orange, 0.2);
            Profile.Draw(vd, (Profile.IsCorrect() ? Brushes.Green : Brushes.Red));
            foreach (var p in Profile.Points ?? new())
                vd.DrawCircle(p.X, p.Y, 0.5, SelectedPoint == p ? Brushes.Yellow : Brushes.Green);
            foreach (var p in Profile.Pickets ?? new())
                vd.DrawCircle(p.X, p.Y, 0.5, p == SelectedPicket ? Brushes.Yellow : Brushes.Orange);
            Image = vd.Render();


            var graph = new VisDraw();
            graph.DrawPoly(pickets.Select((v, i) => new Point(v.pic.H, v.pic.A)).ToList(), Brushes.Orange, 0.3, false);

            if (SelectedPicket != null)
                for (int i = 0; i < pickets.Count; i++)
                    if (pickets[i].pic == SelectedPicket)
                    {
                        graph.DrawCircle(pickets[i].pic.H, pickets[i].pic.A, 0.5, Brushes.Yellow);

                    }
            GraphImage = graph.Render(drawAxies: true);
        }


        public Operator? SelectedOperator
        {
            get=> Profile.Operator;
            set
            {
                Profile.Operator = value;
                db.Entry(Profile).State = EntityState.Modified;
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
        public DrawingImage GraphImage
        {
            get { return graphImage; }
            set
            {
                graphImage = value;
                OnPropertyChanged(nameof(GraphImage));
            }
        }
        public Picket SelectedPicket
        {
            get => selectedPicket;
            set
            {
                selectedPicket = value;
                OnPropertyChanged(nameof(SelectedPicket));
                Redraw();
            }
        }
        public ProfilePoint SelectedPoint
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
