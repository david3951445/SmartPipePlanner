using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SmartPipePlanner.UI;

public class BindableVector3 : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private float _x, _y, _z;

    public float X { get => _x; set { _x = value; OnPropertyChanged(); } }
    public float Y { get => _y; set { _y = value; OnPropertyChanged(); } }
    public float Z { get => _z; set { _z = value; OnPropertyChanged(); } }

    public void Set(Vector3 value)
    {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
    }

    public Vector3 ToVector3() => new(X, Y, Z);

    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
