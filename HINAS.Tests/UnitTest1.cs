using HINAS;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using HINAS.ViewModels;
using Xunit;

namespace HINAS.Tests
{

    public class PlanerViewModelTests
    {
        private readonly Mock<HinasContext> _mockContext;
        private readonly PlanerViewModel _viewModel;

        public PlanerViewModelTests()
        {
            _mockContext = new Mock<HinasContext>();
            _viewModel = new PlanerViewModel(_mockContext.Object);
        }

        [Fact]
        public void Constructor_InitializesWithEmptyPlanner()
        {
            Assert.NotNull(_viewModel.Planner);
            Assert.Equal(string.Empty, _viewModel.Title);
            Assert.Null(_viewModel.TripDate);
            Assert.Equal(string.Empty, _viewModel.Route);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PlanerViewModel(null));
        }

        [Fact]
        public void Title_PropertyChanged_UpdatesPlannerTitle()
        {
            const string testTitle = "Test Title";
            _viewModel.Title = testTitle;

            Assert.Equal(testTitle, _viewModel.Planner.Title);
        }

        [Fact]
        public void TripDate_PropertyChanged_UpdatesPlannerTripDate()
        {
            var testDate = new DateTime(2023, 10, 15);
            _viewModel.TripDate = testDate;

            Assert.Equal(DateOnly.FromDateTime(testDate), _viewModel.Planner.TripDate);
        }

        [Fact]
        public void LeftItemsText_UpdateCombinedItems_UpdatesItemsToPack()
        {
            _viewModel.LeftItemsText = "Left";
            _viewModel.RightItemsText = "Right";

            Assert.Equal("Left\n---\nRight", _viewModel.Planner.ItemsToPack);
        }

        [Fact]
        public void LoadItemsToPack_SplitsItemsCorrectly()
        {
            _viewModel.Planner.ItemsToPack = "Left\n---\nRight";
            _viewModel.LoadItemsToPack();

            Assert.Equal("Left", _viewModel.LeftItemsText);
            Assert.Equal("Right", _viewModel.RightItemsText);
        }

        [Fact]
        public void AddCheckListItem_AddsItemToCollection()
        {
            _viewModel.NewCheckListItem = "New Task";
            _viewModel.AddCheckListItem();

            Assert.Single(_viewModel.CheckListItems);
            Assert.Equal("New Task", _viewModel.CheckListItems[0].Task);
        }

        [Fact]
        public void AddCheckListItem_DoesNotAddEmptyItem()
        {
            _viewModel.NewCheckListItem = "   ";
            _viewModel.AddCheckListItem();

            Assert.Empty(_viewModel.CheckListItems);
        }

        [Fact]
        public async Task SaveAllDataAsync_ReturnsFalse_WhenTitleIsEmpty()
        {
            _viewModel.Title = "";
            var result = await _viewModel.SaveAllDataAsync();

            Assert.False(result);
        }

        [Fact]
        public async Task SaveCheckListItemsAsync_ReturnsFalse_WhenPlannerNotSaved()
        {
            _viewModel.CheckListItems.Add(new CheckListItemViewModel { Task = "Test" });
            var result = await _viewModel.SaveCheckListItemsAsync();

            Assert.False(result);
        }


        [Fact]
        public void CheckListItemViewModel_PropertyChanged_RaisesEvent()
        {
            var item = new CheckListItemViewModel();
            bool eventRaised = false;
            item.PropertyChanged += (sender, args) => eventRaised = true;

            item.Task = "New Task";

            Assert.True(eventRaised);
        }
    }
}