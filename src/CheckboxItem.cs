namespace RegexWpf;

public class CheckboxItem : NotifyPropertyChanged
{
    public string Name { get; }
    public int Value { get; }

    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }

    private bool _isChecked;

    public CheckboxItem(string name)
    {
        Name = name;
    }

    public CheckboxItem(int value, string name)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return Name;
    }
}