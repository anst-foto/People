using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DynamicData;
using DynamicData.Binding;
using People.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace People.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly PersonService _personService;
    
    
    private readonly ObservableCollection<Person> _people = [];
    public HierarchicalTreeDataGridSource<Person> PeopleSource { get; }
    
    [Reactive] public Person? SelectedPerson { get; set; }
    [Reactive] private bool IsSelectedPerson { get; set; }
    [Reactive] public string? LastName { get; set; }
    [Reactive] public string? FirstName { get; set; }
    [Reactive] public string? BirthDate { get; set; }
    
    [Reactive] public string LabelLastName { get; set; } = "Фамилия";
    [Reactive] public string LabelFirstName { get; set; } = "Имя";
    [Reactive] public string LabelBirthDate { get; set; } = "Дата рождения";
    
    public ReactiveCommand<Unit, Unit> CommandLoadData { get; }
    public ReactiveCommand<Unit, Unit> CommandSave { get; }
    public ReactiveCommand<Unit, Unit> CommandDelete { get; }
    public ReactiveCommand<Unit, Unit> CommandClear { get; }

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
        var selected = PeopleSource.RowSelection;
        selected.WhenAnyValue(p => p.SelectedItem)
            .Subscribe(p =>
            {
                SelectedPerson = p;
            });

        this.WhenValueChanged(vm => vm.SelectedPerson)
            .Subscribe(p =>
            {
                IsSelectedPerson = p is not null;
                
                LastName = p?.LastName;
                FirstName = p?.FirstName;
                BirthDate = p?.BirthDate.ToShortDateString();
            });
        
        CommandLoadData = ReactiveCommand.Create(InitPeople);

        var canExecuteCommandSave = this.WhenAnyValue(
            vm => vm.LastName,
            vm => vm.FirstName,
            vm => vm.BirthDate,
            (p1, p2, p3) => !string.IsNullOrWhiteSpace(p1) 
                            && !string.IsNullOrWhiteSpace(p2) 
                            && !string.IsNullOrWhiteSpace(p3));
        CommandSave = ReactiveCommand.Create(
            () => {},
            canExecuteCommandSave);

        var canExecuteCommandDelete = this
            .WhenAnyValue(vm => vm.IsSelectedPerson);
        CommandDelete = ReactiveCommand.Create(
            execute: () =>
            {
                var id = SelectedPerson!.Id;
                if (id.Timestamp == 0) return;
                
                _personService.Delete(id);
                InitPeople();
            },
            canExecuteCommandDelete);
        
        var canExecuteCommandClear = this.WhenAnyValue(
            vm => vm.LastName,
            vm => vm.FirstName,
            vm => vm.BirthDate,
            (p1, p2, p3) => !string.IsNullOrWhiteSpace(p1) 
                            || !string.IsNullOrWhiteSpace(p2) 
                            || !string.IsNullOrWhiteSpace(p3));
        CommandClear = ReactiveCommand.Create(
            execute: () =>
            {
                SelectedPerson = null;
                
                LastName = null;
                FirstName = null;
                BirthDate = null;
            },
            canExecuteCommandClear);
    }

    private void InitPeople()
    {
        var people = _personService.GetAll();
        
        _people.Clear();
        _people.AddRange(people);
    }
}