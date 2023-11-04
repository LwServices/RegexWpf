using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using FluentValidation;

namespace RegexWpf;

public class MainWindowViewModelValidator : AbstractValidator<MainWindowViewModel>
{
    public MainWindowViewModelValidator()
    {
        RuleFor(x => x.Pattern).Must(x => x.Length > 0).WithMessage("Pattern is 0");
    }
}

public class MainWindowViewModel : NotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
{
    // private readonly ObservableCollection<CheckboxItem> CheckboxItems;
    // public IEnumerable<CheckboxItem> CheckboxItems => _mCheckboxItems;

    public ObservableCollection<CheckboxItem> FunctionItems { get; set; } = new ObservableCollection<CheckboxItem>();
    public ObservableCollection<CheckboxItem> OptionsItems { get; set; } = new ObservableCollection<CheckboxItem>();
    public ObservableCollection<Match> Matches { get; set; } = new ObservableCollection<Match>();
    public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();

    private IValidator<MainWindowViewModel> Validator { get; set; }
    // public EventHandler<Match> RegexMatchEventHandler { get; set; } = delegate { };
    //
    // public EventHandler<MatchCollection> RegexMatchCollectionEventHandler { get; set; } = delegate { };

    public MainWindowViewModel()
    {
        var r = new RichTextBox();
        Validator = new MainWindowViewModelValidator();
        var d = r.Document;

        PropertyChanged += OnPropertyChangedEventHandler;

        FunctionItems.CollectionChanged += CheckboxItemsCollectionChanged;
        OptionsItems.CollectionChanged += CheckboxItemsCollectionChanged;

        var regexFuncNames = new string[] {"IsMatch", "Match", "Matches", "Replace"};
        foreach (var funcName in regexFuncNames)
        {
            FunctionItems.Add(new CheckboxItem(funcName));
        }

        foreach (var regexOptions in Enum.GetValues<RegexOptions>())
        {
            OptionsItems.Add(new CheckboxItem((int) regexOptions, regexOptions.ToString()));
        }
    }

    private void OnPropertyChangedEventHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(Pattern) && args.PropertyName != nameof(InputText) &&
            args.PropertyName != nameof(Replacement) && args.PropertyName != "IsChecked")
        {
            return;
        }

        try
        {
            OutputText = string.Empty;
            IsValidPattern = true;

            Options = OptionsItems.Where(x => x.IsChecked).Select(x => x.Value).Sum();
            var regex = new Regex(Pattern, (RegexOptions) Options);

            if (FunctionItems.Any(x => x.IsChecked && x.Name == "IsMatch"))
            {
                IsMatch = regex.IsMatch(InputText);
            }

            if (FunctionItems.Any(x => x.IsChecked && x.Name == "Match"))
            {
                Match = regex.Match(InputText);

                Groups.Clear();
                foreach (Group matchGroup in Match.Groups)
                {
                    Groups.Add(matchGroup);
                }

                CaptureCollection.Clear();
                foreach (Capture capture in Match.Captures)
                {
                    CaptureCollection.Add(capture);
                }
            }

            if (FunctionItems.Any(x => x.IsChecked && x.Name == "Matches"))
            {
                var matches = regex.Matches(InputText);

                Matches.Clear();
                foreach (Match match in matches)
                {
                    Matches.Add(match);
                }
            }

            if (FunctionItems.Any(x => x.IsChecked && x.Name == "Replace"))
            {
                Replaced = regex.Replace(InputText, Replacement);
            }
        }
        catch (RegexParseException re)
        {
            IsValidPattern = false;
            OutputText = re.Message;
        }
        catch (Exception e)
        {
            OutputText = e.ToString();
        }
    }

    #region Properties

    #region InputArea

    private bool _isValidPattern;

    public bool IsValidPattern
    {
        get => _isValidPattern;
        set => SetProperty(ref _isValidPattern, value);
    }

    private string _replacement = string.Empty;

    public string Replacement
    {
        get => _replacement;
        set => SetProperty(ref _replacement, value);
    }

    private string _pattern = string.Empty;

    public string Pattern
    {
        get => _pattern;
        set => SetProperty(ref _pattern, value);
    }

    private int _options;

    public int Options
    {
        get => _options;
        set => SetProperty(ref _options, value);
    }

    private string _inputText = string.Empty;

    public string InputText
    {
        get => _inputText;
        set => SetProperty(ref _inputText, value);
    }

    private FlowDocument? _document;

    public FlowDocument? Document
    {
        get => _document;
        set => SetProperty(ref _document, value);
    }

    #endregion InputArea

    #region IsMatch

    private bool _isMatch;

    public bool IsMatch
    {
        get => _isMatch;
        set => SetProperty(ref _isMatch, value);
    }

    #endregion IsMatch

    private Match? _match;

    public Match? Match
    {
        get => _match;
        set => SetProperty(ref _match, value);
    }

    private string _outputText = string.Empty;

    public string OutputText
    {
        get => _outputText;
        set => SetProperty(ref _outputText, value);
    }

    private ObservableCollection<Capture> _captureCollection = new();

    public ObservableCollection<Capture> CaptureCollection
    {
        get => _captureCollection;
        set => SetProperty(ref _captureCollection, value);
    }

    private string _replaced = string.Empty;


    public string Replaced
    {
        get => _replaced;
        set => SetProperty(ref _replaced, value);
    }


    #region INotifyDataErrorInfo

    public bool HasErrors => Validator.Validate(this).IsValid;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    #endregion

    #region IDataErrorInfo

    public string Error
    {
        get { return "throw new NotImplementedException();"; }
    }


    public string this[string columnName]
    {
        get
        {
            if (columnName == nameof(Pattern))
            {
                if (Pattern.Length < 5)
                    return "FirstName can't be more than 20 characters.";
            }

            return string.Empty;
        }
    }

    #endregion

    #endregion Properties

    private void CheckboxItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (CheckboxItem item in e.OldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (CheckboxItem item in e.NewItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
        //OnPropertyChangedEventHandler(this, e);
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsChecked")
        {
            OnPropertyChangedEventHandler(this, e);
        }
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        var v = Validator.Validate(this);
        if (v.IsValid)
        {
            return Enumerable.Empty<string>();
        }
        else
        {
            return v.Errors;
        }
    }
}