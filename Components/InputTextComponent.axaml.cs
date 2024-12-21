using Avalonia;
using Avalonia.Controls;

namespace People.Components;

public partial class InputTextComponent : UserControl
{
    public static readonly StyledProperty<string> LabelTextProperty = 
        AvaloniaProperty.Register<InputTextComponent, string>(nameof(LabelText));
    public static readonly StyledProperty<string> InputTextProperty =
        AvaloniaProperty.Register<InputTextComponent, string>(nameof(InputText));
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<InputTextComponent, bool>(nameof(IsReadOnly));
    
    public string LabelText { get; set; }
    public string InputText { get; set; }
    public bool IsReadOnly { get; set; } = false;
    
    public InputTextComponent()
    {
        InitializeComponent();
    }
}