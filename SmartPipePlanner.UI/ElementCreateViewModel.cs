using CommunityToolkit.Mvvm.Input;
using HelixToolkit.Maths;
using Microsoft.EntityFrameworkCore;
using SmartPipePlanner.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Automation.Provider;
using System.Windows.Input;

namespace SmartPipePlanner.UI;

class ElementCreateViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    ElementCategory _selectedCategory;
    GeometryType _selectedGeometry;
    double _price;
    private Element? _selectedElement;

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
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    public ElementCreateViewModel()
    {
        AddElementCommand = new RelayCommand(AddElement);
        RemoveElementCommand = new RelayCommand(RemoveSelectedElement);
        ApplyToSelectedCommand = new RelayCommand(ApplyToSelected);
        SaveCommand = new RelayCommand(SaveElements);
        LoadCommand = new RelayCommand(LoadElements);
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

    async void SaveElements()
    {
        using var db = new PlanningDbContext();

        // 先取得 DB 所有元素
        var dbElements = db.Elements.ToList();

        // 1️ 刪除已被移除的元素
        var toRemove = dbElements.Where(dbEl => Elements.All(el => el.Id != dbEl.Id)).ToList();
        if (toRemove.Any())
            db.Elements.RemoveRange(toRemove);

        // 2️ 新增或更新
        foreach (var element in Elements)
        {
            var existing = db.Elements.Find(element.Id);
            if (existing != null)
            {
                // 更新屬性
                existing.Category = element.Category;
                existing.Geometry = element.Geometry;
                existing.Location = element.Location;
                existing.Price = element.Price;
            }
            else
            {
                db.Elements.Add(element);
            }
        }

        await db.SaveChangesAsync();
    }

    public async void LoadElements()
    {
        using var db = new PlanningDbContext();
        db.Database.EnsureCreated();
        var elements = await db.Elements.ToListAsync();

        Elements.Clear();
        foreach (var e in elements)
            Elements.Add(e);

        SelectedElement = Elements.FirstOrDefault();
    }

    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
