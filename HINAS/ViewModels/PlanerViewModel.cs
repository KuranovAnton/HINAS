using HINAS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HINAS.ViewModels
{
    public class PlanerViewModel : INotifyPropertyChanged
    {
        private static PlanerViewModel _currentInstance;
        private  HinasContext _context;
        private Planner _planner = new Planner();
        private string _leftItemsText = string.Empty;
        private string _rightItemsText = string.Empty;
        private ObservableCollection<CheckListItemViewModel> _checkListItems = new ObservableCollection<CheckListItemViewModel>();
        private string _newCheckListItem = string.Empty;
        private bool _isInitialized;

        public HinasContext Context
        {
            get => _context;
            set
            {
                if (_context != value)
                {
                    _context = value;
                    OnPropertyChanged();
                }
            }
        }

        public static PlanerViewModel CurrentInstance
        {
            get => _currentInstance ??= new PlanerViewModel(new HinasContext());
            set => _currentInstance = value;
        }

        public PlanerViewModel(HinasContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            set
            {
                _isInitialized = value;
                OnPropertyChanged();
            }
        }

        public Planner Planner
        {
            get => _planner;
            set
            {
                if (_planner != value)
                {
                    _planner = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(TripDate));
                    OnPropertyChanged(nameof(Route));
                    OnPropertyChanged(nameof(Events));
                    OnPropertyChanged(nameof(Reminders));
                    OnPropertyChanged(nameof(Maintenance));
                    OnPropertyChanged(nameof(Supplies));
                    _ = LoadCheckListItems();
                    LoadItemsToPack();
                }
            }
        }

        public string Title
        {
            get => _planner.Title ?? string.Empty;
            set
            {
                if (_planner.Title != value)
                {
                    _planner.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? TripDate
        {
            get => _planner.TripDate?.ToDateTime(TimeOnly.MinValue);
            set
            {
                _planner.TripDate = value.HasValue
                    ? DateOnly.FromDateTime(value.Value)
                    : null;
                OnPropertyChanged();
            }
        }

        public string Route
        {
            get => _planner.Route ?? string.Empty;
            set
            {
                if (_planner.Route != value)
                {
                    _planner.Route = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LeftItemsText
        {
            get => _leftItemsText;
            set
            {
                _leftItemsText = value;
                OnPropertyChanged();
                UpdateCombinedItems();
            }
        }

        public string RightItemsText
        {
            get => _rightItemsText;
            set
            {
                _rightItemsText = value;
                OnPropertyChanged();
                UpdateCombinedItems();
            }
        }

        public string Events
        {
            get => _planner.Events ?? string.Empty;
            set
            {
                if (_planner.Events != value)
                {
                    _planner.Events = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Reminders
        {
            get => _planner.Reminders ?? string.Empty;
            set
            {
                if (_planner.Reminders != value)
                {
                    _planner.Reminders = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Maintenance
        {
            get => _planner.Maintenance ?? string.Empty;
            set
            {
                if (_planner.Maintenance != value)
                {
                    _planner.Maintenance = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Supplies
        {
            get => _planner.Supplies ?? string.Empty;
            set
            {
                if (_planner.Supplies != value)
                {
                    _planner.Supplies = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<CheckListItemViewModel> CheckListItems
        {
            get => _checkListItems;
            set
            {
                _checkListItems = value;
                OnPropertyChanged();
            }
        }

        public string NewCheckListItem
        {
            get => _newCheckListItem;
            set
            {
                _newCheckListItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveAllCommand => new RelayCommand(async () => await SaveAllDataAsync());
        public ICommand AddCheckListItemCommand => new RelayCommand(AddCheckListItem);
        public ICommand GoBackCommand => new RelayCommand(async () => await SaveAndGoBack());

        private async Task SaveAndGoBack()
        {
            await SaveAllDataAsync();
            CurrentInstance = this;
        }

        private void UpdateCombinedItems()
        {
            _planner.ItemsToPack = $"{LeftItemsText}\n---\n{RightItemsText}";
        }

        public void LoadItemsToPack()
        {
            if (!string.IsNullOrEmpty(_planner.ItemsToPack))
            {
                var parts = _planner.ItemsToPack.Split(new[] { "\n---\n" }, StringSplitOptions.None);
                LeftItemsText = parts.Length > 0 ? parts[0] : string.Empty;
                RightItemsText = parts.Length > 1 ? parts[1] : string.Empty;
            }
        }

        public async Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                await LoadPlannerDataAsync();
                IsInitialized = true;
            }
        }

        public async Task LoadPlannerDataAsync()
        {
            try
            {
                var planner = await _context.Planners
                    .Include(p => p.CheckLists)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (planner != null)
                {
                    Planner = planner;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        public async Task<bool> SaveAllDataAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    MessageBox.Show("Пожалуйста, укажите название!");
                    return false;
                }

                if (_planner.Id == 0)
                {
                    _context.Planners.Add(_planner);
                }
                else
                {
                    _context.Planners.Update(_planner);
                }

                await _context.SaveChangesAsync();
                return await SaveCheckListItemsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                return false;
            }
        }

        public void AddCheckListItem()
        {
            if (!string.IsNullOrWhiteSpace(NewCheckListItem))
            {
                var newItem = new CheckListItemViewModel
                {
                    Task = NewCheckListItem,
                    Done = false
                };

                CheckListItems.Add(newItem);
                NewCheckListItem = string.Empty;
                _ = SaveCheckListItemsAsync();
            }
        }

        public async Task<bool> SaveCheckListItemsAsync()
        {
            try
            {
                if (_planner?.Id == 0)
                {
                    MessageBox.Show("Сначала сохраните основной план!");
                    return false;
                }

                var existingItems = await _context.CheckLists
                    .Where(x => x.PlannerId == _planner.Id)
                    .ToListAsync();

                var itemsToRemove = existingItems
                    .Where(e => !CheckListItems.Any(c => c.Id == e.Id))
                    .ToList();

                if (itemsToRemove.Any())
                {
                    _context.CheckLists.RemoveRange(itemsToRemove);
                }

                foreach (var item in CheckListItems)
                {
                    if (item.Id == 0)
                    {
                        _context.CheckLists.Add(new CheckList
                        {
                            PlannerId = _planner.Id,
                            Task = item.Task,
                            Done = item.Done
                        });
                    }
                    else
                    {
                        var existing = existingItems.FirstOrDefault(x => x.Id == item.Id);
                        if (existing != null)
                        {
                            existing.Task = item.Task;
                            existing.Done = item.Done;
                            _context.CheckLists.Update(existing);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения списка: {ex.Message}");
                return false;
            }
        }

        public async Task LoadCheckListItems()
        {
            try
            {
                if (_planner?.Id == null || _planner.Id == 0) return;

                var items = await _context.CheckLists
                    .Where(x => x.PlannerId == _planner.Id)
                    .OrderBy(x => x.Id)
                    .Select(x => new CheckListItemViewModel
                    {
                        Id = x.Id,
                        Task = x.Task ?? string.Empty,
                        Done = x.Done ?? false
                    })
                    .ToListAsync();

                CheckListItems = new ObservableCollection<CheckListItemViewModel>(items);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand DeleteCheckListItemCommand => new RelayCommand<CheckListItemViewModel>(async (item) =>
        {
            if (item == null) return;

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить задачу: \"{item.Task}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteCheckListItem(item);
            }
        });

        public async Task DeleteCheckListItem(CheckListItemViewModel item)
        {
            try
            {
                if (item.Id > 0)
                {
                    var dbItem = await _context.CheckLists.FindAsync(item.Id);
                    if (dbItem != null)
                    {
                        _context.CheckLists.Remove(dbItem);
                        await _context.SaveChangesAsync();
                    }
                }

                CheckListItems.Remove(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении задачи: {ex.Message}");
            }
        }
    }

    public class CheckListItemViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _task = string.Empty;
        private bool _done;

        public string Task
        {
            get => _task;
            set
            {
                if (_task != value)
                {
                    _task = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Done
        {
            get => _done;
            set
            {
                if (_done != value)
                {
                    _done = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}