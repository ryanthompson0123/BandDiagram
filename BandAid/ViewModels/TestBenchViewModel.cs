using System;
using Band.Units;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ComponentModel;
using System.IO;

namespace Band
{
    public class TestBenchViewModel : ObservableObject
    {
        private TestBench testBenchValue;
        public TestBench TestBench
        {
            get { return testBenchValue; }
            set
            {
                SetProperty(ref testBenchValue, value);

                TestBench.PropertyChanged += TestBench_PropertyChanged;
            }
        }

        private bool needsScreenshotValue;
        public bool NeedsScreenshot
        {
            get { return needsScreenshotValue; }
            private set { SetProperty(ref needsScreenshotValue, value); }
        }

        private string titleTextValue;
        public string TitleText
        {
            get { return titleTextValue; }
            set
            {
                var oldName = titleTextValue;
                SetProperty(ref titleTextValue, value);

                TestBench.Name = value;
                RemoveTestBench(oldName);
            }
        }

        private bool computingValue;
        public bool Computing
        {
            get { return computingValue; }
            private set { SetProperty(ref computingValue, value); }
        }

        private bool noSolutionValue;
        public bool NoSolution
        {
            get { return noSolutionValue; }
            private set { SetProperty(ref noSolutionValue, value); }
        }

        private PlotType plotTypeValue;
        public PlotType PlotType
        {
            get { return plotTypeValue; }
            private set { SetProperty(ref plotTypeValue, value); }
        }

        private PlotAnimationGrouping plotGroupValue;
        public PlotAnimationGrouping PlotGroup
        {
            get { return plotGroupValue; }
            private set { SetProperty(ref plotGroupValue, value); }
        }

        // Don't observe these because they get created once and passed to their respective
        // views. Those views will observe these objects individually.
        public StructureParameterListViewModel StructureParameterList { get; set; }
        //public StructureSceneViewModel Scene { get; set; }

        public TestBenchViewModel(TestBench testBench)
        {
            TestBench = testBench;

            StructureParameterList = new StructureParameterListViewModel(TestBench);
            //Scene = new StructureSceneViewModel(TestBench);

            //BiasSliderMaxValue = TestBench.MaxVoltage.Volts;
            //BiasSliderMinValue = testBench.MinVoltage.Volts;

            TitleText = TestBench.Name;
            Compute();
        }

        private async void Compute()
        {
            await TestBench.ComputeIfNeededAsync();
        }

        private async void TestBench_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    TitleText = TestBench.Name;
                    SaveTestBench();
                    break;
                case "NeedsCompute":
                    if (TestBench.NeedsCompute)
                    {
                        SaveTestBench();
                        Computing = true;
                        await TestBench.ComputeIfNeededAsync();
                        Computing = false;
                    }
                    break;
                case "NoSolution":
                    NoSolution = TestBench.NoSolution;
                    break;
                case "Steps":
                    UpdatePlot();
                    break;
            }
        }

        public async void SaveTestBench()
        {
            NeedsScreenshot = false;
            await TestBench.SaveAsync();
            NeedsScreenshot = true;
        }

        public async Task SaveScreenshotAsync(Stream imageStream)
        {
            var fm = DependencyService.Get<IFileManager>();

            await fm.SaveTestBenchScreenshotAsync(TestBench.Name, imageStream);

            NeedsScreenshot = false;
        }

        private void UpdatePlot()
        {
            if (TestBench.Steps.Count == 0) return;

            var animationAxis = new PlotAxis
            {
                Max = TestBench.MaxVoltage.Volts,
                Min = TestBench.MinVoltage.Volts,
                MajorSpan = TestBench.StepSize.Volts,
                Title = "Bias (V)"
            };

            PlotGroup = new PlotAnimationGrouping(animationAxis, 
                d => CreatePlot(TestBench.GetStep(new ElectricPotential(d))));
        }

        private Plot CreatePlot(Structure structure)
        {
            var dataSets = new List<PlotDataSet>();

            Length thickness = Length.Zero;
            foreach (var layer in structure.Layers)
            {
                switch (PlotType)
                {
                    case PlotType.Energy:
                        foreach (var dataset in layer.GetEnergyDatasets(thickness))
                        {
                            dataSets.Add(dataset);
                        }
                        break;
                    case PlotType.ElectricField:
                        dataSets.Add(layer.GetElectricFieldDataset(thickness));
                        break;
                    case PlotType.ChargeDensity:
                        dataSets.Add(layer.GetChargeDensityDataset(thickness));
                        break;
                    case PlotType.Potential:
                        dataSets.Add(layer.GetPotentialDataset(thickness));
                        break;
                }

                thickness += layer.Thickness;
            }

            var plot = CreatePlot(PlotType, dataSets);
            plot.AutoScale();
            return plot;
        }

        private static Plot CreatePlot(PlotType plotType, List<PlotDataSet> dataSets)
        {
            switch (plotType)
            {
                case PlotType.Energy:
                    return new Plot(dataSets)
                    {
                        Title = "Energy",
                        YAxis = new PlotAxis
                        {
                            Title = "Energy (eV)"
                        },
                        XAxis = new PlotAxis
                        {
                            Title = "Distance (nm)",
                            MajorSpan = 5.0
                        }
                    };
                case PlotType.ChargeDensity:
                    return new Plot(dataSets)
                    {
                        Title = "Charge Density",
                        YAxis = new PlotAxis
                        {
                            Title = "Charge Density (μC/cm\xB2)"
                        },
                        XAxis = new PlotAxis
                        {
                            Title = "Distance (nm)",
                            MajorSpan = 5.0
                        }
                    };
                case PlotType.ElectricField:
                    return new Plot(dataSets)
                    {
                        Title = "Electric Field",
                        YAxis = new PlotAxis
                        {
                            Title = "Electric Field (MV/cm)"
                        },
                        XAxis = new PlotAxis
                        {
                            Title = "Distance (nm)",
                            MajorSpan = 5.0
                        }
                    };
                case PlotType.Potential:
                    return new Plot(dataSets)
                    {
                        Title = "Potential",
                        YAxis = new PlotAxis
                        {
                            Title = "Potential (V)"
                        },
                        XAxis = new PlotAxis
                        {
                            Title = "Distance (nm)",
                            MajorSpan = 5.0
                        }
                    };
                default:
                    return null;
            }
        }

        public void SetSelectedBias(double bias)
        {
            // Set the current voltage to the snapped voltage.
            TestBench.SetBias(new ElectricPotential(bias));
        }

        public void SetSelectedPlotType(PlotType plotType)
        {
            PlotType = plotType;
            UpdatePlot();
        }

        public void UpdateSettings(string minVoltageText, string maxVoltageText, string stepSizeText)
        {
            double minVoltage;
            double maxVoltage;
            double stepSize;

            if (double.TryParse(minVoltageText, out minVoltage) &&
                double.TryParse(maxVoltageText, out maxVoltage) &&
                double.TryParse(stepSizeText, out stepSize))
            {
                TestBench.SetRange(new ElectricPotential(minVoltage), new ElectricPotential(maxVoltage),
                    new ElectricPotential(stepSize));
            }
        }

        public SettingsViewModel GetSettingsViewModel()
        {
            return new SettingsViewModel
            {
                MaxVoltageText = TestBench.MaxVoltage.Volts.ToString(),
                MinVoltageText = TestBench.MinVoltage.Volts.ToString(),
                StepSizeText = TestBench.StepSize.Volts.ToString()
            };
        }

        private async void RemoveTestBench(string oldName)
        {
            // If we are setting for first time, or the name didn't change,
            // we don't need to do anything.
            if (string.IsNullOrEmpty(oldName) || oldName == TitleText) return;

            var fileManager = DependencyService.Get<IFileManager>();
            await fileManager.DeleteTestBenchAsync(oldName);
        }
    }
}