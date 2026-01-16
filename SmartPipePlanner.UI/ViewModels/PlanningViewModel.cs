using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SmartPipePlanner.Core;
using SmartPipePlanner.Core.Search;

namespace SmartPipePlanner.UI.ViewModels;

class PlanningViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    Direction _startDir;
    PipeCategory _category;
    Problem? _selectedProblem;

    public Array Directions { get; } = Enum.GetValues<Direction>();
    public Array PipeCategories { get; } = Enum.GetValues<PipeCategory>();
    public BindableVector3 Start { get; set; } = new();
    public Direction StartDir
    {
        get => _startDir;
        set
        {
            _startDir = value;
            OnPropertyChanged();
        }
    }
    public BindableVector3 End { get; set; } = new();
    public PipeCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged();
        }
    }
    public Problem? SelectedProblem
    {
        get => _selectedProblem;
        set
        {
            _selectedProblem = value;
            if (value != null)
            {
                Start.Set(new Vector3(value.Start.X, value.Start.Y, value.Start.Z));
                StartDir = value.StartDir;
                End.Set(new Vector3(value.End.X, value.End.Y, value.End.Z));
                Category = value.Category;
            }
            OnPropertyChanged();
        }
    }
    public ObservableCollection<Problem> Problems { get; } = [];
    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand ApplyCommand { get; }

    public PlanningViewModel()
    {
        AddCommand = new RelayCommand(AddProblem);
        RemoveCommand = new RelayCommand(RemoveSelectedProblem);
        ApplyCommand = new RelayCommand(ApplyToSelected);
    }

    void AddProblem()
    {
        var item = new Problem(
            new Coordinate((int)Start.X, (int)Start.Y, (int)Start.Z),
            StartDir,
            new Coordinate((int)End.X, (int)End.Y, (int)End.Z),
            Category);

        Problems.Add(item);
        SelectedProblem = item;
    }

    void RemoveSelectedProblem()
    {
        if (SelectedProblem != null)
        {
            int index = Problems.IndexOf(SelectedProblem);
            Problems.Remove(SelectedProblem);

            // 選下一個元素（如果有）
            if (Problems.Count > 0)
            {
                if (index >= Problems.Count)
                    index = Problems.Count - 1; // 如果刪最後一個，選最後一個
                SelectedProblem = Problems[index];
            }
            else
            {
                SelectedProblem = null; // 清空選擇
            }
        }
    }

    void ApplyToSelected()
    {
        if (SelectedProblem == null)
            return;

        Problems.Remove(SelectedProblem);
        AddProblem();
    }

    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void ResetProblems(Problem[] problems)
    {
        Problems.Clear();
        foreach (var e in problems)
            Problems.Add(e);

        SelectedProblem = Problems.FirstOrDefault();
    }
}
