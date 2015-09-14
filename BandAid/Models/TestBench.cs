using Band.Units;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Band
{
    public class TestBench : ObservableObject
    {
        private bool needsComputeValue;
        [JsonIgnore]
        public bool NeedsCompute
        {
            get { return needsComputeValue; }
            set { SetProperty(ref needsComputeValue, value); }
        }

        public string Name { get; set; }

        private Structure structureValue;
        public Structure Structure
        {
            get { return structureValue; }
            set
            {
                structureValue = value;
                value.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "Layers")
                    {
                        SetNeedsCompute();
                    }
                };

                SetNeedsCompute();
            }
        }

        [JsonIgnore]
        public Dictionary<int, Structure> Steps { get; set; }

        private ElectricPotential currentVoltageValue;
        [JsonIgnore]
        public ElectricPotential CurrentVoltage
        {
            get { return currentVoltageValue; }
            set
            {
                if (currentVoltageValue == value) return;

                SetProperty(ref currentVoltageValue, value);
                CurrentStep = StepForPotential(value);
            }
        }

        private int currentStepValue;
        public int CurrentStep
        {
            get { return currentStepValue; }
            set
            {
                if (currentStepValue == value) return;

                SetProperty(ref currentStepValue, value);
                CurrentVoltage = PotentialForStep(value);
            }
        }

        [JsonIgnore]
        public Structure CurrentStructure
        {
            get { return Steps[CurrentStep]; }
        }

        private ElectricPotential minVoltageValue;
        public ElectricPotential MinVoltage
        {
            get { return minVoltageValue; }
            set
            {
                SetProperty(ref minVoltageValue, value);
                SetNeedsCompute();
            }
        }

        private ElectricPotential maxVoltageValue;
        public ElectricPotential MaxVoltage
        {
            get { return maxVoltageValue; }
            set
            {
                SetProperty(ref maxVoltageValue, value);
                SetNeedsCompute();
            }
        }

        private ElectricPotential stepSizeValue;
        public ElectricPotential StepSize
        {
            get { return stepSizeValue; }
            set
            {
                if (value.Volts == 0.0) return;
                SetProperty(ref stepSizeValue, value);
                SetNeedsCompute();
            }
        }

        [JsonIgnore]
        public int StepCount
        {
            get { return (int)((MaxVoltage - MinVoltage) / StepSize) + 1; }
        }

        public TestBench()
        {
            Steps = new Dictionary<int, Structure>();
            CurrentVoltage = new ElectricPotential(1.0);
            MinVoltage = new ElectricPotential(-2.0);
            MaxVoltage = new ElectricPotential(2.0);
            StepSize = new ElectricPotential(0.25);

            CurrentStep = StepForPotential(CurrentVoltage);
            Structure = Structure.Default;
        }

        public static async Task<TestBench> CreateDefaultAsync()
        {
            var nextName = await FigureOutNextNameAsync();
            var fileManager = DependencyService.Get<IFileManager>();

            var testBench = await fileManager.LoadDefaultTestBenchAsync();
            testBench.Name = nextName;

            return testBench;
        }

        public void SetNeedsCompute()
        {
            NeedsCompute = true;
        }

        public async Task ComputeIfNeededAsync()
        {
            if (NeedsCompute)
            {
                await Task.Run(() => Compute()); 
            }
        }

        private void Compute()
        {
            Debug.WriteLine(string.Format("Beginning calculation of {0} steps", StepCount));
            var stopwatch = Stopwatch.StartNew();

            var steps = Enumerable.Range(0, StepCount).Select(s => PotentialForStep(s).RoundMillivolts)
                .ToDictionary(p => p, p =>
                {
                    return Steps.ContainsKey(p) ? Steps[p] :
                            Structure.DeepClone(ElectricPotential.FromMillivolts(p), new Temperature(300.0));
                });

            Debug.WriteLine(string.Format("Finished all calculations in {0} ms", stopwatch.ElapsedMilliseconds));

            Steps = steps;

            NeedsCompute = false;
        }

        private ElectricPotential PotentialForStep(int step)
        {
            return MinVoltage + StepSize * step;
        }

        private int StepForPotential(ElectricPotential potential)
        {
            return (int)((potential - MinVoltage) / StepSize);
        }

        private static async Task<string> FigureOutNextNameAsync()
        {
            var fileManager = DependencyService.Get<IFileManager>();

            var nextName = "MyStructure";
            var nextNumber = 0;
            var tryAgain = await fileManager.CheckTestBenchExistsAsync(nextName);

            while (tryAgain)
            {
                nextNumber++;
                nextName = "MyStructure" + nextNumber;
                tryAgain = await fileManager.CheckTestBenchExistsAsync(nextName);
            }

            return nextName;
        }
    }
}