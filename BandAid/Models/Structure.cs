using System;
using System.Collections.Generic;
using System.Linq;
using Band.Units;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Band
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Structure : ObservableObject
	{
        [JsonProperty("layers")]
        private List<Material> layersValue;
        public List<Material> Layers
        {
            get { return layersValue; }
        }

        public Structure()
        {
            layersValue = new List<Material>();
        }

        public Structure(List<Material> layers)
        {
            layersValue = layers;

            foreach (var layer in Layers)
            {
                layer.ParentStructure = this;
            }

            Evaluate();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            foreach (var layer in Layers)
            {
                layer.ParentStructure = this;
                layer.PropertyChanged += Layer_PropertyChanged;
            }

            Evaluate();
        }

        void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Evaluate();

            OnPropertyChanged("Layers");
        }

        public Structure DeepClone(ElectricPotential bias)
        {
            var structure = new Structure();

            structure.Bias = bias;

            var layerClones = Layers.Select(l => l.DeepClone()).ToList();

            foreach (var layer in layerClones)
            {
                layer.ParentStructure = structure;
            }

            structure.layersValue.AddRange(layerClones);
            structure.Evaluate();

            return structure;
        }

        public Structure DeepClone()
        {
            return DeepClone(Bias);
        }

        public void InsertLayer(int index, Material layer)
        {
            layer.ParentStructure = this;
            layer.PropertyChanged += Layer_PropertyChanged;
            layersValue.Insert(index, layer);

            Evaluate();

            OnPropertyChanged("Layers");
        }

        public void AddLayer(Material layer)
        {
            if (BottomLayer.MaterialType != MaterialType.Dielectric)
            {
                // If we're inserting a layer, and there's a metal or semiconductor on the
                // bottom, then there's no point inserting it at the bottom
                // because that's not valid, so put it above the bottom layer
                InsertLayer(Layers.Count - 1, layer);
            }
            else
            {
                // Otherwise, insert it at the bottom
                InsertLayer(Layers.Count, layer);
            }
        }

        public void RemoveLayer(Material layer)
        {
            layer.PropertyChanged -= Layer_PropertyChanged;
            layersValue.Remove(layer);

            Evaluate();
            OnPropertyChanged("Layers");
        }

        public void ReplaceLayer(Material layer, int index)
        {
            if (index < 0 || layer == null) return;

            Layers[index].PropertyChanged -= Layer_PropertyChanged;
            Layers.RemoveAt(index);
            Layers.Insert(index, layer);

            Evaluate();
            OnPropertyChanged("Layers");
        }

        public void MoveLayer(Material layer, int index)
        {
            if (index < 0 || layer == null) return;

            Layers.Remove(layer);
            Layers.Insert(index, layer);

            Evaluate();
            OnPropertyChanged("Layers");
        }

        public void MoveLayerUp(int index)
        {
            if (index < 1) return;

            var m = Layers[index];
            Layers.RemoveAt(index);
            Layers.Insert(index - 1, m);

            Evaluate();
            OnPropertyChanged("Layers");
        }

        public void MoveLayerDown(int index)
        {
            if (index >= Layers.Count) return;

            var m = Layers[index];
            Layers.RemoveAt(index);
            Layers.Insert(index + 1, m);

            Evaluate();
            OnPropertyChanged("Layers");
        }

        public Material TopLayer
        {
            get { return Layers.First(); }
        }

        public Material BottomLayer
        {
            get { return Layers.Last(); }
        }

        public Material GetLayer(Length location)
        {
            if (location.Meters < 0)
                throw new ArgumentException("Location was negative");

            var evaluatedThickness = Length.Zero;

            foreach (var layer in Layers)
            {
                if (layer.Thickness == null)
                {
                    evaluatedThickness += Length.FromNanometers(1000);
                }
                else
                {
                    evaluatedThickness += layer.Thickness;
                }
                if (location < evaluatedThickness)
                    return layer;
            }

            throw new ArgumentException("Location was outside the structure");
        }

        public Material GetLayerAbove(Material material)
        {
            var index = Layers.IndexOf(material);
            if (index > 0)
                return Layers[index - 1];

            return null;
        }

        public Material GetLayerBelow(Material material)
        {
            var index = Layers.IndexOf(material);
            if (index < Layers.Count - 1)
                return Layers[index + 1];

            return null;
        }

        public bool IsTopLayerMetal
        {
            get { return TopLayer is Metal; }
        }

        public bool IsBottomLayerSemiconductor
        {
            get { return BottomLayer is Semiconductor; }
        }

        public bool IsBottomLayerMetal
        {
            get { return BottomLayer is Metal; }
        }

        public bool IsSemiconductorAboveBottomLayer
        {
            get
            {
                for (var i = 0; i < Layers.Count - 1; i++)
                {
                    if (Layers[i] is Semiconductor) return true;
                }

                return false;
            }
        }

        public bool HasTwoMetalsNextToEachOther
        {
            get
            {
                var lastLayerWasMetal = false;
                foreach (var layer in Layers)
                {
                    if (layer is Metal)
                    {
                        if (lastLayerWasMetal)
                        {
                            return false;
                        }
                        else
                        {
                            lastLayerWasMetal = true;
                        }
                    }
                    else
                    {
                        lastLayerWasMetal = false;
                    }
                }

                return false;
            }
        }

        public bool HasAtLeastOneDielectricLayer
        {
            get { return Layers.Any(l => l is Dielectric); }
        }

        private bool noSolutionValue;
        public bool NoSolution
        {
            get { return noSolutionValue; }
            private set
            {
                noSolutionValue = value;
                OnPropertyChanged("NoSolution");
            }
        }

        public Length Thickness
        {
            get
            {
                return new Length(Layers.Sum(l => l.Thickness.Meters));
            }
        }

        public Energy WorkFunctionDifference
        {
            get
            {
                return TopLayer.WorkFunction - BottomLayer.WorkFunction;
            }
        }

        [JsonProperty]
        public ElectricPotential Bias { get; private set; }

        public bool IsValid
        {
            get
            {
                // Make sure there is at least 1 material
                if (Layers.Count == 0) return false;

                // Make sure the top layer is a metal
                if (!IsTopLayerMetal) return false;

                // Make sure the bottom layer is not a dielectric
                if (!IsBottomLayerMetal && !IsBottomLayerSemiconductor) return false;

                // Make sure there's no semiconductor inside the structure
                if (IsSemiconductorAboveBottomLayer) return false;

                // Make sure we have at least one oxide
                if (!HasAtLeastOneDielectricLayer) return false;

                // Make sure we don't have two metals next to each other
                return !HasTwoMetalsNextToEachOther;
            }
        }

        private const int maximumIterations = 1000;

        private void Evaluate()
        {
            // Don't do anything if the structure isn't valid
            if (!IsValid) return;

            int iterationNumber;

            // Since we integrate left to right, we want to specify the voltage on the left
            var voltageBias = -Bias + WorkFunctionDifference;

            // Iterate until we find the desired voltage
            var chargeHigh = ChargeDensity.FromCoulombsPerSquareCentimeter(1.0);
            var chargeLow = ChargeDensity.FromCoulombsPerSquareCentimeter(-1.0);
            var chargeGuess = (chargeHigh + chargeLow) / 2;

            // !!!!!!!!!!!!!!!!!!
            EvaluateGivenCharge(chargeGuess);
            // !!!!!!!!!!!!!!!!!!

            var potentialCalc = BottomLayer.EvalPoints[1].Potential;
            var tinyPositiveBias = new ElectricPotential(1e-6);
            var tinyNegativeBias = new ElectricPotential(-1e-6);

            // Iterate
            for (iterationNumber = 0; 
                (potentialCalc > voltageBias + ElectricPotential.Abs(voltageBias * 1e-6) 
                    + tinyPositiveBias
                || potentialCalc < voltageBias - ElectricPotential.Abs(voltageBias * 1e-6) 
                    + tinyNegativeBias)
                && iterationNumber < maximumIterations;
                iterationNumber++)
            {
                if (potentialCalc > voltageBias)
                {
                    chargeLow = chargeGuess;
                }
                else
                {
                    chargeHigh = chargeGuess;
                }

                // Update the guessCharge
                chargeGuess = (chargeHigh + chargeLow) / 2;

                // !!!!!!!!!!!!!!!!!!
                EvaluateGivenCharge(chargeGuess);
                // !!!!!!!!!!!!!!!!!!

                potentialCalc = BottomLayer.EvalPoints[1].Potential;

                if (iterationNumber == maximumIterations - 1)
                {
                    if (!(potentialCalc > voltageBias + ElectricPotential.FromMillivolts(1)
                        || potentialCalc < voltageBias - ElectricPotential.FromMillivolts(1)))
                    {
                            // TODO: Inform that solution only found to accuracy of 1e-3
                    }
                    else
                    {
                        NoSolution = true;
                        return;
                    }
                }
            }

            // If the last material is a semiconductor, fill in the missing points
            if (IsBottomLayerSemiconductor)
            {
                ((Semiconductor)BottomLayer).Evaluate();

                // Now subtract the potential from all the points so the right is ref to 0
                var lastPoint = BottomLayer.EvalPoints.Last();
                var potential = lastPoint.Potential;

                foreach (var layer in Layers)
                {
                    foreach (var ep in layer.EvalPoints)
                    {
                        ep.Potential = ep.Potential - potential;
                    }
                }

                var trueLast = lastPoint.DeepClone();
                if (trueLast.Location < Length.FromNanometers(50))
                {
                    trueLast.Location = Length.FromNanometers(50);
                }
                BottomLayer.EvalPoints.Add(trueLast);
            }

            NoSolution = false;
            /*
            Debug.WriteLine(String.Format("Evaluation finished after {0} iterations in {1} ms", 
                iterationNumber, stopwatch.ElapsedMilliseconds));
            */
        }

        // Calculate based on top charge return running charge
        private ChargeDensity EvaluateGivenCharge(ChargeDensity topCharge)
        {
            // Set the top metal to have a charge at the bottom (location = thickness)
            TopLayer.Prepare();

            // Set the first point to all zeros
            TopLayer.EvalPoints[0] = new EvalPoint();

            // Add charge to the last point
            TopLayer.EvalPoints[1] = new EvalPoint
            {
                Location = TopLayer.Thickness,
                ChargeDensity = topCharge
            };

            var runningCharge = topCharge;
            var runningPotential = ElectricPotential.Zero;

            // Now integrate the charges to get the electric field in all the dielectrics
            foreach (var layer in Layers.Skip(1)) // Only inner layers
            {
                if (layer is Dielectric)
                {
                    var dielectric = (Dielectric)layer;
                    dielectric.Prepare();

                    for (var i = 0; i < dielectric.EvalPoints.Count; i++)
                    {
                        var point = dielectric.EvalPoints[i];

                        // Integrate the charge (sum really)
                        runningCharge += point.ChargeDensity;

                        // Calculate the Electric Field
                        point.ElectricField = runningCharge / dielectric.Permittivity;

                        // Calculate the potential
                        if (i == 0)
                        {
                            point.Potential = runningPotential;
                        }
                        else
                        {
                            var previousPoint = dielectric.EvalPoints[i - 1];
                            runningPotential -= previousPoint.ElectricField
                                * (point.Location - previousPoint.Location);

                            point.Potential = runningPotential;
                        }
                    }
                }
                else if (layer is Metal)
                {
                    var metal = (Metal)layer;
                    metal.Prepare();

                    // For the first point put the neg of the charge we have accumulated so far
                    metal.EvalPoints[0].ChargeDensity = -runningCharge;
                    metal.EvalPoints[0].ElectricField = ElectricField.Zero;
                    metal.EvalPoints[0].Potential = runningPotential;

                    // For the last point put the accumulated charge plus the free charge
                    runningCharge += metal.ExtraCharge; // Integrate the extra charge
                    metal.EvalPoints[1].ChargeDensity = runningCharge;
                    metal.EvalPoints[1].ElectricField = ElectricField.Zero;
                    metal.EvalPoints[1].Potential = runningPotential;
                }
                else // layer is Semiconductor
                {
                    // do nothing
                }
            }

            // Now add the stuff for the last point - here we assume that it is a metal
            if (IsBottomLayerMetal)
            {
                var metal = (Metal)BottomLayer;
                metal.Prepare();

                // For the first point put the neg of the charge we have accumulated so far
                metal.EvalPoints[0].ChargeDensity = -runningCharge;
                metal.EvalPoints[0].ElectricField = ElectricField.Zero;
                metal.EvalPoints[0].Potential = runningPotential;

                // For the last point put no charge
                metal.EvalPoints[0].ChargeDensity = ChargeDensity.Zero;
                metal.EvalPoints[0].ElectricField = ElectricField.Zero;
                metal.EvalPoints[0].Potential = runningPotential;
            }
            else if (IsBottomLayerSemiconductor)
            {
                var semiconductor = (Semiconductor)BottomLayer;

                // Calculate the surface potential and prepare
                semiconductor.ExtraCharge = -runningCharge;
                semiconductor.Prepare();

                // Evaulate the potential drop given the remaining charge
                semiconductor.EvalPoints[0].ChargeDensity = -runningCharge;
                semiconductor.EvalPoints[0].Potential = runningPotential;

                // Last Point
                semiconductor.EvalPoints[1].Potential 
                    = runningPotential - semiconductor.SurfacePotential;
            }

            return runningCharge;
        }

        public ElectricPotential ThresholdVoltage 
        {
            get
            {
                if (IsBottomLayerSemiconductor)
                {
                    // Make a deep copy of the structure so we don't ruin anything.
                    var structureClone = DeepClone();

                    var semiconductor = (Semiconductor)structureClone.BottomLayer;
                    var surfacePotential = 2 * semiconductor.PhiF;
                    var chargeAtVth = semiconductor.GetChargeDensity(surfacePotential);
                    var trappedCharge = ChargeDensity.Zero;

                    // Get all the charge in the metals and dielectrics.
                    foreach (var layer in structureClone.Layers)
                    {
                        if (layer is Metal)
                        {
                            trappedCharge += ((Metal)layer).ExtraCharge;
                        }

                        if (layer is Dielectric)
                        {
                            foreach (var point in layer.EvalPoints)
                            {
                                trappedCharge += point.ChargeDensity;
                            }
                        }
                    }

                    structureClone.EvaluateGivenCharge(-chargeAtVth - trappedCharge);

                    var vth = (ElectricPotential)structureClone.WorkFunctionDifference
                                           - structureClone.BottomLayer.EvalPoints[1].Potential;

                    return vth;
                }
                else
                {
                    return null;   // Can't have a threshold without a semiconductor
                }
            }
        }

        public ElectricPotential FlatbandVoltage
        {
            get
            {
                // Make a deep copy of the structure so we don't ruin anything
                var structureClone = DeepClone();
                var semiconductorCharge = ChargeDensity.Zero;

                if (structureClone.IsBottomLayerSemiconductor)
                {
                    semiconductorCharge = ((Semiconductor)structureClone.BottomLayer)
                        .GetChargeDensity(ElectricPotential.Zero);
                }

                var runningTrapCharge = ChargeDensity.Zero;

                foreach (var layer in structureClone.Layers)
                {
                    if (layer is Metal)
                    {
                        runningTrapCharge += ((Metal)layer).ExtraCharge;
                    }

                    if (layer is Dielectric)
                    {
                        foreach (var point in layer.EvalPoints)
                        {
                            runningTrapCharge += point.ChargeDensity;
                        }
                    }
                }

                structureClone.EvaluateGivenCharge(-runningTrapCharge - semiconductorCharge);
                var vfb = (ElectricPotential)structureClone.WorkFunctionDifference 
                    - structureClone.BottomLayer.EvalPoints[0].Potential;

                return vfb;
            }
        }

        public CapacitanceDensity OxideCapacitance
        {
            get
            {
                var oneOverCap = 0.0;

                foreach (var layer in Layers)
                {
                    if (layer is Dielectric)
                    {
                        oneOverCap += 1 / ((Dielectric)layer).OxideCapacitance.FaradsPerSquareMeter;
                    }
                }

                return new CapacitanceDensity(1 / oneOverCap);
            }
        }

        public CapacitanceDensity StackCapacitance
        {
            get
            {
                var oneOverCap = 0.0;

                foreach (var layer in Layers)
                {
                    if (layer is Dielectric)
                    {
                        oneOverCap += 1 / ((Dielectric)layer).OxideCapacitance
                            .FaradsPerSquareCentimeter;
                    }

                    if (layer is Semiconductor)
                    {
                        oneOverCap += 1 / ((Semiconductor)layer).CapacitanceDensity
                            .FaradsPerSquareCentimeter;
                    }
                }

                return CapacitanceDensity.FromFaradsPerSquareCentimeter(1 / oneOverCap);
            }
        }

        public Length EquivalentOxideThickness
        {
            get
            {
                return 3.9 * Permittivity.OfFreeSpace / OxideCapacitance;
            }
        }

        public ElectricField GetElectricField(Length location)
        {
            var thickness = Length.Zero;
            var lastThickness = Length.Zero;

            foreach (var layer in Layers)
            {
                lastThickness = thickness;
                thickness += layer.Thickness;
                if (location < thickness)
                {
                    return layer.GetElectricField(location - lastThickness);
                }
            }

            return ElectricField.Zero;
        }

        public ElectricPotential GetPotential(Length location)
        {
            var thickness = Length.Zero;
            var lastThickness = Length.Zero;

            foreach (var layer in Layers)
            {
                lastThickness = thickness;
                thickness += layer.Thickness;
                if (location < thickness)
                {
                    return layer.GetPotential(location - lastThickness);
                }
            }

            return ElectricPotential.Zero;
        }

//        public static Structure Default
//        {
//            get
//            {
//                var topMetal = new Metal(Length.FromNanometers(4));
//                topMetal.WorkFunction = Energy.FromElectronVolts(4.45);
//                topMetal.FillColor = "#ff0000";
//                topMetal.Name = "TiN";
//
//                var oxide = new Dielectric(Length.FromNanometers(2));
//                oxide.DielectricConstant = 3.9;
//                oxide.BandGap = Energy.FromElectronVolts(8.9);
//                oxide.ElectronAffinity = Energy.FromElectronVolts(0.95);
//                oxide.FillColor = "#804040";
//                oxide.Name = "SiO2";
//
//                var semiconductor = new Semiconductor();
//                semiconductor.BandGap = Energy.FromElectronVolts(1.1252);
//                semiconductor.ElectronAffinity = Energy.FromElectronVolts(4.05);
//                semiconductor.DielectricConstant = 11.7;
//                semiconductor.IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(1.41E10);
//                semiconductor.DopingType = DopingType.N;
//                semiconductor.DopantConcentration = Concentration.FromPerCubicCentimeter(1E18);
//                semiconductor.FillColor = "#00ff00";
//                semiconductor.Name = "Si";
//
//                var structure = new Structure();
//                structure.Temperature = new Temperature(300);
//                structure.AddLayer(semiconductor);
//                structure.AddLayer(oxide);
//                structure.AddLayer(topMetal);
//
//                return structure;
//            }
//        }
    }
}