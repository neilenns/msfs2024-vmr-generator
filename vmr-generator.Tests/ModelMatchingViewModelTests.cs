using NUnit.Framework;
using vmr_generator.ViewModels.ModelMatching;
using vmr_generator.Models;
using vmr_generator.Helpers;
using Moq;
using vmr_generator.Interfaces;
using System.Collections.Generic;

namespace vmr_generator.Tests
{
    [TestFixture]
    public class ModelMatchingViewModelTests
    {
        private ModelMatchingViewModel _viewModel;
        private Mock<IMessageBoxService> _messageBoxServiceMock;
        private Mock<ISaveDialogService> _saveDialogServiceMock;

        [SetUp]
        public void Setup()
        {
            _messageBoxServiceMock = new Mock<IMessageBoxService>();
            _saveDialogServiceMock = new Mock<ISaveDialogService>();

            _viewModel = new ModelMatchingViewModel
            {
                MessageBoxService = _messageBoxServiceMock.Object,
                SaveDialogService = _saveDialogServiceMock.Object
            };
        }

        [Test]
        public void GetLiveries_ShouldAddLiveries()
        {
            // Arrange
            _viewModel.IsConnected = true;
            _viewModel.SimConnect = new Mock<SimConnect>().Object;

            // Act
            _viewModel.GetLiveries();

            // Assert
            Assert.IsNotEmpty(_viewModel.Liveries);
        }

        [Test]
        public void SaveLiveries_ShouldSaveLiveriesToFile()
        {
            // Arrange
            var fileName = "test.vmr";
            _saveDialogServiceMock.Setup(s => s.ShowDialog()).Returns(fileName);
            _viewModel.Liveries.Add(new Livery { CallsignPrefix = "AIB", TypeCode = "CL60", ModelName = "FSLTL_GA_C25C_ZZZ" });

            // Act
            _viewModel.SaveLiveries();

            // Assert
            _saveDialogServiceMock.Verify(s => s.ShowDialog(), Times.Once);
        }

        [Test]
        public void FlattenedList_ShouldReturnFlattenedLiveries()
        {
            // Arrange
            _viewModel.Liveries.Add(new Livery { CallsignPrefix = "AIB", TypeCode = "CL60", ModelName = "FSLTL_GA_C25C_ZZZ" });
            _viewModel.Liveries.Add(new Livery { CallsignPrefix = "AIB", TypeCode = "CL60", ModelName = "FSLTL_GA_C25C_M-MIKE" });

            // Act
            var flattenedList = _viewModel.FlattenedList;

            // Assert
            Assert.AreEqual(1, flattenedList.Count);
            Assert.AreEqual("FSLTL_GA_C25C_ZZZ//FSLTL_GA_C25C_M-MIKE", flattenedList[0].ModelName);
        }

        [Test]
        public void AddSampleData_ShouldAddSampleLiveries()
        {
            // Act
            _viewModel.AddSampleData();

            // Assert
            Assert.IsNotEmpty(_viewModel.Liveries);
        }
    }
}
