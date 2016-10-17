using Band.Units;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Threading;

namespace Band
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TestBench : ObservableObject
    {
        private bool needsComputeValue;
        public bool NeedsCompute
        {
            get { return needsComputeValue; }
            set { SetProperty(ref needsComputeValue, value); }
        }
            
        private string nameValue;
        [JsonProperty]
        public string Name
        {
            get { return nameValue; }
            set { SetProperty(ref nameValue, value); }
        }
            
        private Structure structureValue;
        [JsonProperty]
        public Structure Structure
        {
            get { return structureValue; }
            set
            {
                structureValue = value;
                value.PropertyChanged += Structure_PropertyChanged;

                if (value.IsValid)
                {
                    SetNeedsCompute();
                }
            }
        }

        private List<Structure> stepsValue;
        public List<Structure> Steps
        {
            get { return stepsValue; }
            set { SetProperty(ref stepsValue, value); }
        }

        private int currentIndexValue;
        [JsonProperty]
        public int CurrentIndex
        {
            get { return currentIndexValue; }
            set { SetProperty(ref currentIndexValue, value); }
        }

        public ElectricPotential CurrentVoltage
        {
            get { return PotentialForStep(CurrentIndex); }
        }

        public Structure CurrentStructure
        {
            get { return Steps[CurrentIndex]; }
        }

        private ElectricPotential minVoltageValue;
        [JsonProperty]
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
        [JsonProperty]
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
        [JsonProperty]
        public ElectricPotential StepSize
        {
            get { return stepSizeValue; }
            set
            {
                if (value.RoundMilliVolts == 0) return;
                SetProperty(ref stepSizeValue, value);
                SetNeedsCompute();
            }
        }

        private bool noSolutionValue;
        public bool NoSolution
        {
            get { return noSolutionValue; }
            private set { SetProperty(ref noSolutionValue, value); }
        }
        
        public int StepCount
        {
            get { return (int)((MaxVoltage - MinVoltage) / StepSize) + 1; }
        }

        public TestBench()
        {
            Steps = new List<Structure>();
            MinVoltage = new ElectricPotential(-2.0);
            MaxVoltage = new ElectricPotential(2.0);
            StepSize = new ElectricPotential(0.25);
            CurrentIndex = StepCount / 2;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            NeedsCompute = true;
        }

        public static Task<TestBench> CreateAsync(string name)
        {
            var fileManager = DependencyService.Get<IFileManager>();

            return fileManager.LoadTestBenchAsync(name);
        }

        public static async Task<TestBench> CreateDefaultAsync()
        {
            var nextName = await FigureOutNextNameAsync();
            var fileManager = DependencyService.Get<IFileManager>();

            var testBench = await fileManager.LoadDefaultTestBenchAsync();
            testBench.Name = nextName;

            return testBench;
        }

        public Task SaveAsync()
        {
            var fileManager = DependencyService.Get<IFileManager>();

            return fileManager.SaveTestBenchAsync(this);
        }

        public void SetNeedsCompute()
        {
            NeedsCompute = true;
        }

        public async Task ComputeIfNeededAsync(CancellationToken cancellationToken)
        {
            if (NeedsCompute)
            {
                await Task.Run(() => Compute(cancellationToken));
            }
        }

        public void SetBias(ElectricPotential potential)
        {
            CurrentIndex = StepForPotential(potential);
        }

        public void SetRange(ElectricPotential min, ElectricPotential max, ElectricPotential step)
        {
            minVoltageValue = min;
            maxVoltageValue = max;
            stepSizeValue = step;

            SetNeedsCompute();
        }

        public Structure GetStep(ElectricPotential potential)
        {
            return Steps[StepForPotential(potential)];
        }

        private void Structure_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Layers":
                    if (Structure.IsValid && !Structure.NoSolution) SetNeedsCompute();
                    break;
                case "NoSolution":
                    NoSolution = Structure.NoSolution;
                    break;
            }
        }

        private void Compute(CancellationToken cancellationToken)
        {
            NeedsCompute = false;

            if (!Structure.IsValid) return;

            var referenceStructure = Structure.DeepClone();

            Debug.WriteLine(string.Format("Beginning calculation of {0} steps", StepCount));
            var stopwatch = Stopwatch.StartNew();

            var steps = new List<Structure>();

            foreach (var step in Enumerable.Range(0, StepCount))
            {
                var potential = PotentialForStep(step).RoundMilliVolts;
                var structure = referenceStructure.DeepClone(ElectricPotential.FromMillivolts(potential), cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.WriteLine("Cancellation requested - aborting.");
                    return;
                }

                steps.Add(structure);
            }

            Debug.WriteLine(string.Format("Finished all calculations in {0} ms", stopwatch.ElapsedMilliseconds));

            Steps = steps;
        }

        private ElectricPotential PotentialForStep(int step)
        {
            var delta = StepSize * step;
            var potential = MinVoltage + delta;

            return potential;
        }

        private int StepForPotential(ElectricPotential potential)
        {
            var delta = potential - MinVoltage;
            var step = delta / StepSize;

            return Convert.ToInt32(step);
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