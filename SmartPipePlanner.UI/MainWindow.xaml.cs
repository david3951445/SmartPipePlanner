using Microsoft.EntityFrameworkCore;
using SmartPipePlanner.Core.Search;
using SmartPipePlanner.Data;
using System.Collections.Specialized;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SmartPipePlanner.UI
{
    public partial class MainWindow : Window
    {
        readonly Dictionary<Element, ModelVisual3D> _elementVisualMap = [];
        readonly Dictionary<Problem, (ModelVisual3D Start, ModelVisual3D End)> _problemVisualMap = [];

        public MainWindow()
        {
            InitializeComponent();
            loadMenuItem.Click += async (s, e) => await LoadAsync();
            saveMenuItem.Click += async (s, e) => await SaveAsync();
            viewModel.Elements.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Element el in e.NewItems!)
                        AddElementToViewport(el);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Element el in e.OldItems!)
                        RemoveElementFromViewport(el);
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    ClearViewportElements();
                }
            };
            _ = LoadAsync();
            planningViewModel.Problems.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Problem pb in e.NewItems!)
                        AddProblemToViewport(pb);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Problem pb in e.OldItems!)
                        RemoveProblemFromViewport(pb);
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    ClearViewportProblems();
                }
            };
        }

        async Task LoadAsync()
        {
            using var db = new PlanningDbContext();

            var elements = await db.Elements.ToArrayAsync();
            var problems = await db.Problems.ToArrayAsync();
            viewModel.ResetElements(elements);
            planningViewModel.ResetProblems([.. problems.Select(p => p.ToProblem())]);
        }

        public async Task SaveAsync()
        {
            await using var db = new PlanningDbContext();

            db.Elements.RemoveRange(db.Elements);
            db.Problems.RemoveRange(db.Problems);
            db.Elements.AddRange(viewModel.Elements);
            db.Problems.AddRange(planningViewModel.Problems.Select(ProblemDto.FromProblem));

            await db.SaveChangesAsync();
        }

        void AddElementToViewport(Element element)
        {
            ModelVisual3D visual = GetVisual3D(element);
            viewport.Children.Add(visual);
            _elementVisualMap[element] = visual;
        }

        static ModelVisual3D GetVisual3D(Element element)
        {
            Color color = GetColor(element);
            Vector3 center = element.Location;
            return element.Geometry.Type switch
            {
                GeometryType.Box => MeshFactory.AddBox(
                    center,
                    1, 1, 1,
                    color),
                GeometryType.Pipe1 => MeshFactory.AddPipe(
                    center,
                    element.Geometry.Orientation,
                    length: 1,
                    color),
                GeometryType.Pipe2 => MeshFactory.AddPipe(
                    center,
                    element.Geometry.Orientation,
                    length: 2,
                    color),
                GeometryType.LPipe1 => MeshFactory.AddLPipe(
                    center,
                    element.Geometry.Orientation,
                    length: 1,
                    color),
                _ => throw new NotImplementedException()
            };
        }

        static Color GetColor(Element element) => element.Category switch
        {
            ElementCategory.HotWaterPipe => element.Geometry.Type switch
            {
                GeometryType.Pipe1 => Colors.Orange,
                GeometryType.Pipe2 => Colors.Red,
                GeometryType.LPipe1 => Colors.Gold,
                _ => Colors.Gray
            },
            ElementCategory.ColdWaterPipe => element.Geometry.Type switch
            {
                GeometryType.Pipe1 => Colors.LightBlue,
                GeometryType.Pipe2 => Colors.DodgerBlue,
                GeometryType.LPipe1 => Colors.DeepSkyBlue,
                _ => Colors.Gray
            },
            _ => Colors.Gray
        };

        void RemoveElementFromViewport(Element element)
        {
            if (!_elementVisualMap.TryGetValue(element, out var visual))
                return;

            viewport.Children.Remove(visual);
            _elementVisualMap.Remove(element);
        }

        void ClearViewportElements()
        {
            // 只刪除元素，不刪 Grid
            foreach (var v in _elementVisualMap.Values)
                viewport.Children.Remove(v);

            _elementVisualMap.Clear();
        }

        void AddProblemToViewport(Problem item)
        {
            var start = MeshFactory.AddSphere(new Point3D(item.Start.X, item.Start.Y, item.Start.Z), 0.2, Colors.LightGreen);
            var end = MeshFactory.AddSphere(new Point3D(item.End.X, item.End.Y, item.End.Z), 0.2, Colors.DarkGreen);
            viewport.Children.Add(start);
            viewport.Children.Add(end);
            _problemVisualMap[item] = (start, end);
        }

        void RemoveProblemFromViewport(Problem item)
        {
            if (!_problemVisualMap.TryGetValue(item, out var x))
                return;

            viewport.Children.Remove(x.Start);
            viewport.Children.Remove(x.End);
            _problemVisualMap.Remove(item);
        }

        void ClearViewportProblems()
        {
            // 只刪除元素，不刪 Grid
            foreach (var (Start, End) in _problemVisualMap.Values)
            {
                viewport.Children.Remove(Start);
                viewport.Children.Remove(End);
            }

            _problemVisualMap.Clear();
        }
    }
}
