using Band.Units;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private ElectricPotential currentVoltageValue;
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

        public void SetNeedsCompute()
        {
            NeedsCompute = true;
        }

        public async Task ComputeIfNeededAsync()
        {
            if (NeedsCompute)
            {
                await Task.Run(() => Compute(),); 
            }
        }

        private void Compute()
        {
            Debug.WriteLine(string.Format("Beginning calculation of {0} steps", StepCount));
            var stopwatch = Stopwatch.StartNew();

            var steps = Enumerable.Range(0, StepCount).Select(s => PotentialForStep(s).RoundMillivolts)
                .ToDictionary(p => p, p =>
                {
                    return StructureSteps.ContainsKey(p) ? StructureSteps[p] :
                            ReferenceStructure.DeepClone(ElectricPotential.FromMillivolts((double)p), new Temperature(300.0));
                });

            Debug.WriteLine(String.Format("Finished all calculations in {0} ms", stopwatch.ElapsedMilliseconds));

            Device.BeginInvokeOnMainThread(() =>
            {
                StructureSteps = steps;
            });



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
    }
}
