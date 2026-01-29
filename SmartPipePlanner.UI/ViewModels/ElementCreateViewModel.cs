using CommunityToolkit.Mvvm.Input;
using SmartPipePlanner.Core;
using SmartPipePlanner.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartPipePlanner.UI.ViewModels;

class ElementCreateViewModel : INotifyPropertyChanged, IGridManager
{
    public event PropertyChangedEventHandler? PropertyChanged;
    ElementCategory _selectedCategory;
    GeometryType _selectedGeometry;
    double _price;
    Element? _selectedElement;

    public Array ElementCategories { get; } = Enum.GetValues<ElementCategory>();
    public Array GeometryTypes { get; } = Enum.GetValues<GeometryType>();
    public ElementCategory SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged();
        }
    }
    public GeometryType SelectedGeometry
    {
        get => _selectedGeometry;
        set
        {
            _selectedGeometry = value;
            OnPropertyChanged();
        }
    }
    public BindableVector3 Orientation { get; set; } = new();
    public BindableVector3 Location { get; set; } = new();
    public double Price
    {
        get => _price;
        set
        {
            _price = value;
            OnPropertyChanged();
        }
    }
    public Element? SelectedElement
    {
        get => _selectedElement;
        set
        {
            _selectedElement = value;
            if (value != null)
            {
                Location.Set(value.Location);
                Orientation.Set(value.Geometry.Orientation);
                SelectedCategory = value.Category;
                SelectedGeometry = value.Geometry.Type;
                Price = value.Price;
            }
            OnPropertyChanged();
        }
    }
    public ObservableCollection<Element> Elements { get; } = [];
    public ICommand AddElementCommand { get; }
    public ICommand RemoveElementCommand { get; }
    public ICommand ApplyToSelectedCommand { get; }

    public ElementCreateViewModel()
    {
        AddElementCommand = new RelayCommand(AddElement);
        RemoveElementCommand = new RelayCommand(RemoveSelectedElement);
        ApplyToSelectedCommand = new RelayCommand(ApplyToSelected);
    }

    void AddElement()
    {
        var element = new Element
        {
            Category = SelectedCategory,
            Geometry = new Geometry
            {
                Type = SelectedGeometry,
                Orientation = Orientation.ToVector3()
            },
            Location = Location.ToVector3(),
            Price = Price
        };

        Elements.Add(element);
        SelectedElement = element;
    }

    void RemoveSelectedElement()
    {
        if (SelectedElement != null)
        {
            int index = Elements.IndexOf(SelectedElement);
            Elements.Remove(SelectedElement);

            // 選下一個元素（如果有）
            if (Elements.Count > 0)
            {
                if (index >= Elements.Count)
                    index = Elements.Count - 1; // 如果刪最後一個，選最後一個
                SelectedElement = Elements[index];
            }
            else
            {
                SelectedElement = null; // 清空選擇
            }
        }
    }

    void ApplyToSelected()
    {
        if (SelectedElement == null)
            return;

        Elements.Remove(SelectedElement);
        AddElement();
    }

    public void ResetElements(Element[] elements)
    {
        Elements.Clear();
        foreach (var e in elements)
            Elements.Add(e);

        SelectedElement = Elements.FirstOrDefault();
    }

    public Grid GetGrid()
    {
        var obstacles = Elements
            .Where(e => e.Category == ElementCategory.Obstacle)
            .Select(e => e.Location)
            .ToArray();
        var grid = new Grid(10, 10, 10);
        foreach (var obstacle in obstacles)
        {
            grid.Cells[(int)obstacle.X, (int)obstacle.Y, (int)obstacle.Z] = CellType.Obstacle;
        }
        return grid;
    }

    public void SetPipes(Pipe[] pipes)
    {
        // Convert pipe to element
        var pipeElements = pipes.Select(pipe => new Element
        {
            Category = pipe.Category switch
            {
                PipeCategory.HotWaterPipe => ElementCategory.HotWaterPipe,
                PipeCategory.ColdWaterPipe => ElementCategory.ColdWaterPipe,
                _ => throw new InvalidOperationException(),
            },
            Geometry = new Geometry
            {
                Type = pipe.Geometry switch
                {
                    PipeGeometry.Pipe1 => GeometryType.Pipe1,
                    PipeGeometry.Pipe2 => GeometryType.Pipe2,
                    PipeGeometry.LPipe1 => GeometryType.LPipe1,
                    _ => throw new InvalidOperationException(),
                },
                Orientation = OrientationFromDirection(pipe.Direction, pipe.LPipeDirection)
            },
            Location = new Vector3(pipe.Location.X, pipe.Location.Y, pipe.Location.Z),
            Price = 0 // Price can be set accordingly
        });

        foreach (var e in pipeElements)
            Elements.Add(e);

        static Vector3 OrientationFromDirection(Direction d, Direction? ld)
        {
            var x1 = Vector3.UnitX;
            var y1 = Vector3.UnitY;
            var z1 = Vector3.UnitZ;

            var c1 = Coordinate.FromDirection(d);
            var x2 = new Vector3(c1.X, c1.Y, c1.Z);

            Vector3 y2;
            if (ld == null)
            {
                y2 = Vector3.Cross(z1, x2); // 任意選一個垂直方向
            }
            else
            {
                var c2 = Coordinate.FromDirection(ld.Value);
                y2 = new Vector3(c2.X, c2.Y, c2.Z);
            }

            var z2 = Vector3.Cross(x2, y2);

            var q = Extensions.QuaternionBetweenFrames(
                x1, y1, z1,
                x2, y2, z2);
            return q.ToEulerAngles();
        }
    }

    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
