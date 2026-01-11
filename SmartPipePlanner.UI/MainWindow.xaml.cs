using HelixToolkit.Geometry;
using HelixToolkit.Wpf;
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

        public MainWindow()
        {
            InitializeComponent();

            Point3D[] pathPoints =
            [
                new Point3D(-3, 0, 0),
                new Point3D(-1, 1, 0.5),
                new Point3D(0, 2, 1),
                new Point3D(1, 1, 1.5),
                new Point3D(3, 0, 2)
            ];
            DrawPath(pathPoints, Colors.Blue);
            AddSphere(pathPoints[0], 0.2, Colors.Green);
            AddSphere(pathPoints[^1], 0.2, Colors.Red);

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
            viewModel.LoadElements();
        }

        void AddElementToViewport(Element element)
        {
            ModelVisual3D visual = GetVisual3D(element);
            viewport.Children.Add(visual);
            _elementVisualMap[element] = visual;
        }

        private static ModelVisual3D GetVisual3D(Element element)
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

            //element.PropertyChanged -= Element_PropertyChanged;
        }

        void ClearViewportElements()
        {
            // 只刪除元素，不刪 Grid
            foreach (var v in _elementVisualMap.Values)
                viewport.Children.Remove(v);

            _elementVisualMap.Clear();
        }

        /// <summary>
        /// Draws a 3D polyline through the points
        /// </summary>
        void DrawPath(Point3D[] points, Color color)
        {
            var lines = new LinesVisual3D
            {
                Color = color,
                Thickness = 2
            };

            for (int i = 0; i < points.Length - 1; i++)
            {
                lines.Points.Add(points[i]);
                lines.Points.Add(points[i + 1]);
            }

            viewport.Children.Add(lines);
        }

        /// <summary>
        /// Adds a sphere marker at the specified position
        /// </summary>
        void AddSphere(Point3D center, double radius, Color color)
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(center.ToVector3(), (float)radius);
            var visual = AddMesh(meshBuilder, color);
            viewport.Children.Add(visual);
        }

        static ModelVisual3D AddMesh(MeshBuilder builder, Color color)
        {
            var mesh = builder.ToMesh();

            var model = new GeometryModel3D
            {
                Geometry = mesh.ToWndMeshGeometry3D(),
                Material = new DiffuseMaterial(new SolidColorBrush(color)),
                BackMaterial = new DiffuseMaterial(new SolidColorBrush(color))
            };

            return new ModelVisual3D
            {
                Content = model
            };
        }
    }
}
