using HINAS.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HINAS
{
    public partial class Planer : Page
    {
        private readonly PlanerViewModel _viewModel;

        public Planer()
        {
            InitializeComponent();

            try
            {
                _viewModel = PlanerViewModel.CurrentInstance ?? new PlanerViewModel(new HinasContext());
                DataContext = _viewModel;

                if (!_viewModel.IsInitialized)
                {
                    _ = LoadPlannerDataAsync();
                    _viewModel.IsInitialized = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}");
            }
        }

        private async Task LoadPlannerDataAsync()
        {
            try
            {
                var planner = await _viewModel.Context.Planners
                    .Include(p => p.CheckLists)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (planner != null)
                {
                    _viewModel.Planner = planner;
                    await _viewModel.LoadCheckListItems();
                    _viewModel.LoadItemsToPack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await SavePlannerAsync();
        }

        public async Task SavePlannerAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.Title))
                {
                    MessageBox.Show("Пожалуйста, укажите название!");
                    return;
                }

                var saveResult = await _viewModel.SaveAllDataAsync();
                if (saveResult)
                {
                    MessageBox.Show("Все данные успешно сохранены!");
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка сохранения в базу данных: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
            }
        }

        private async void GoToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            await SavePlannerAsync();
            PlanerViewModel.CurrentInstance = _viewModel;

            if (NavigationService != null)
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Window.GetWindow(this)?.Close();
                }
            }
            else
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Window.GetWindow(this)?.Close();
            }
        }
    }
}