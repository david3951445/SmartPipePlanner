using CommunityToolkit.Mvvm.Input;
using SmartPipePlanner.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartPipePlanner.UI.ViewModels;

class ElementCreateViewModel : INotifyPropertyChanged
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

    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
