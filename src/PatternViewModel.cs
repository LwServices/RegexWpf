using System;
using System.Collections;
using System.ComponentModel;

namespace RegexWpf;

public class PatternViewModel : NotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
{
    private string _pattern = string.Empty;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public string Pattern
    {
        get => _pattern;
        set => SetProperty(ref _pattern, value);
    }

    public string this[string columnName] => throw new NotImplementedException();

    public string Error => throw new NotImplementedException();


    public bool HasErrors => throw new NotImplementedException();

    public IEnumerable GetErrors(string? propertyName)
    {
        throw new NotImplementedException();
    }
}