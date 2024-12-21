using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DynamicData;
using People.Models;
using ReactiveUI;

namespace People.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly PersonService _personService;
    
    private readonly ObservableCollection<Person> _people = [];

    public HierarchicalTreeDataGridSource<Person> PeopleSource { get; }
    
    public ReactiveCommand<Unit, Unit> CommandLoadData { get; }

    public MainWindowViewModel()
    {
        var mongoDbConfig = MongoDbConfig.Load();
        _personService = new PersonService(mongoDbConfig);
        
        PeopleSource = new HierarchicalTreeDataGridSource<Person>(_people)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<Person>(
                    new TextColumn<Person, string>
                        ("Имя", x => x.FullName), x => x.Children),
                new TextColumn<Person, int>("Возраст", x => x.Age),
            },
        };
        
        CommandLoadData = ReactiveCommand.Create(InitPeople);
    }

    private void InitPeople()
    {
        var people = _personService.GetAll();
        
        _people.Clear();
        _people.AddRange(people);
    }
}