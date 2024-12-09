#nullable enable
using VmrGenerator.Interfaces;
using VmrGenerator.ViewModels.ModelMatching;
using Moq;
using VmrGenerator.Models;

namespace VmrGenerator.tests
{
    public class ModelMatchingViewModelTests
    {
        private ModelMatchingViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new ModelMatchingViewModel();
            _viewModel.Liveries.AddRange([
                // <ModelMatchRule CallsignPrefix="AIB" TypeCode="CL60" ModelName="FSLTL_GA_C25C_ZZZ//FSLTL_GA_C25C_M-MIKE//FSLTL_GA_C25C_PS-CSF" /> 
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_ZZZ",
                    TypeCode = "CL60",
                },
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_M-MIKE",
                    TypeCode = "CL60",
                },
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_PS-CSF",
                    TypeCode = "CL60",
                },
                // <ModelMatchRule CallsignPrefix="AIB" TypeCode="CRJX" ModelName="FSLTL_CRJ7_ZZZZ" /> 
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_CRJ7_ZZZZ",
                    TypeCode = "CRJX",
                },
                // <ModelMatchRule TypeCode="C172" ModelName="FSLTL_GA_C172_ZZZ" /> 
                new Livery() {
                    ModelName = "FSLTL_GA_C172_ZZZ",
                    TypeCode = "C172",
                },
                // <ModelMatchRule CallsignPrefix="DAL" TypeCode="B739" ModelName="FSLTL_FAIB_B739_DAL-Delta_SSW//FSLTL_FAIB_B739_DAL-Delta_WL" /> 
                new Livery() {
                    CallsignPrefix = "DAL",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_SSW",
                    TypeCode = "B739",
                },
                new Livery() {
                    CallsignPrefix = "DAL",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_WL",
                    TypeCode = "B739",
                },
                // Should be standalone
                // <ModelMatchRule CallsignPrefix="DAL" FlightNumberRange="4439-4858" TypeCode="B739" ModelName="FSLTL_FAIB_B739_DAL-Delta_WL" />
                new Livery() {
                    CallsignPrefix = "DAL",
                    FlightNumberRange = "4439-4858",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_WL",
                    TypeCode = "B739",
                }
             ]);
        }

        [Test]
        public void FlattenedList_ShouldCombineLiveriesWithSamePrefixAndType()
        {
            // Arrange & Act
            var flattenedList = _viewModel.FlattenedList;

            // Assert
            var aibCl60 = flattenedList.FirstOrDefault(l => l.CallsignPrefix == "AIB" && l.TypeCode == "CL60");
            Assert.That(aibCl60, Is.Not.Null);
            Assert.That(aibCl60.ModelName, Is.EqualTo("FSLTL_GA_C25C_ZZZ//FSLTL_GA_C25C_M-MIKE//FSLTL_GA_C25C_PS-CSF"));
        }

        [Test]
        public void FlattenedList_ShouldNotCombineLiveriesWithDifferentPrefixesOrTypes()
        {
            // Arrange & Act
            var flattenedList = _viewModel.FlattenedList;

            // Assert
            var aibCrjx = flattenedList.FirstOrDefault(l => l.CallsignPrefix == "AIB" && l.TypeCode == "CRJX");
            Assert.That(aibCrjx, Is.Not.Null);
            Assert.That(aibCrjx.ModelName, Is.EqualTo("FSLTL_CRJ7_ZZZZ"));
        }

        [Test]
        public void FlattenedList_ShouldHandleStandaloneEntries()
        {
            // Arrange & Act
            var flattenedList = _viewModel.FlattenedList;

            // Assert
            var standaloneDal = flattenedList.FirstOrDefault(l =>
                l.CallsignPrefix == "DAL" &&
                l.TypeCode == "B739" &&
                l.FlightNumberRange == "4439-4858");
            Assert.That(standaloneDal, Is.Not.Null);
            Assert.That(standaloneDal.ModelName, Is.EqualTo("FSLTL_FAIB_B739_DAL-Delta_WL"));
        }

        [Test]
        public void FlattenedList_ShouldContainCorrectNumberOfEntries()
        {
            // Arrange & Act
            var flattenedList = _viewModel.FlattenedList;

            // Assert
            Assert.That(flattenedList, Has.Count.EqualTo(5));
        }
    }
}
