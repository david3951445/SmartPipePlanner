using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SmartPipePlanner.UI;

public class BindableQuaternion : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private float _x, _y, _z, _t;

    public float Ux { get => _x; set { _x = value; OnPropertyChanged(); } }
    public float Uy { get => _y; set { _y = value; OnPropertyChanged(); } }
    public float Uz { get => _z; set { _z = value; OnPropertyChanged(); } }
    public float Theta { get => _t; set { _t = value; OnPropertyChanged(); } }

    public void Set(Quaternion value)
    {
        // Ensure the quaternion is normalized
        value = Quaternion.Normalize(value);

        // Convert quaternion to axis-angle
        Theta = 2f * MathF.Acos(value.W) * 180f / MathF.PI;

        float sinHalfTheta = MathF.Sqrt(1f - value.W * value.W);
        if (sinHalfTheta < 1e-6f)
        {
            // If angle is near zero, direction is arbitrary
            Ux = 1f;
            Uy = 0f;
            Uz = 0f;
        }
        else
        {
            Ux = value.X / sinHalfTheta;
            Uy = value.Y / sinHalfTheta;
            Uz = value.Z / sinHalfTheta;
        }
    }

    public Quaternion ToQuaternion() => Quaternion.CreateFromAxisAngle(new(Ux, Uy, Uz), Theta / 180f * MathF.PI);

    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
