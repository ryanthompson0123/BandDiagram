using NUnit.Framework;
using System;
using Band;
using Band.Units;

namespace BandAidTests
{
    [TestFixture]
    public class StructureTests
    {
        private static Structure CreateSiO2TestStructure()
        {
            var topMetal = new Metal(Length.FromNanometers(4));
            topMetal.SetWorkFunction(Energy.FromElectronVolts(4.45));

            var oxide = new Dielectric(Length.FromNanometers(2));
            oxide.DielectricConstant = 3.9;
            oxide.BandGap = Energy.FromElectronVolts(8.9);
            oxide.ElectronAffinity = Energy.FromElectronVolts(0.95);

            var semiconductor = new Semiconductor();
            semiconductor.BandGap = Energy.FromElectronVolts(1.1252);
            semiconductor.ElectronAffinity = Energy.FromElectronVolts(4.05);
            semiconductor.DielectricConstant = 11.7;
            semiconductor.IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(1.41E10);
            semiconductor.DopingType = DopingType.N;
            semiconductor.DopantConcentration = Concentration.FromPerCubicCentimeter(1E18);

            var structure = new Structure();
            structure.Temperature = new Temperature(300);
            structure.AddLayer(semiconductor);
            structure.AddLayer(oxide);
            structure.AddLayer(topMetal);

            return structure;
        }

        private static Structure CreateMIMTestStructure()
        {
            var topMetal = new Metal(Length.FromNanometers(4));
            topMetal.SetWorkFunction(Energy.FromElectronVolts(4.45));

            var oxide = new Dielectric(Length.FromNanometers(2));
            oxide.DielectricConstant = 3.9;
            oxide.BandGap = Energy.FromElectronVolts(8.9);
            oxide.ElectronAffinity = Energy.FromElectronVolts(0.95);

            var bottomMetal = new Metal(Length.FromNanometers(4));
            bottomMetal.SetWorkFunction(Energy.FromElectronVolts(4.45));

            var structure = new Structure();
            structure.Temperature = new Temperature(300);
            structure.AddLayer(bottomMetal);
            structure.AddLayer(oxide);
            structure.AddLayer(topMetal);

            return structure;
        }

        [TestCase]
        public void TestValidStructureIsValid()
        {
            var structure = CreateSiO2TestStructure();

            Assert.That(structure.IsValid);
        }

        [TestCase]
        public void TestStructureHasCorrectThresholdVoltage()
        {
            var structure = CreateSiO2TestStructure();

            var roundedVth = Math.Round(structure.ThresholdVoltage.Volts, 3);

            Assert.AreEqual(-.953, roundedVth);
        }

        [TestCase]
        public void TestStructureHasCorrectEOT()
        {
            var structure = CreateSiO2TestStructure();

            var eot = structure.EquivalentOxideThickness;

            Assert.AreEqual(2.0, eot.Nanometers);
        }

        [TestCase]
        public void TestStructureHasCorrectStackCapacitanceAtZeroVolts()
        {
            var structure = CreateSiO2TestStructure();

            var cStack = Math.Round(structure.StackCapacitance.FaradsPerSquareCentimeter, 10);

            Assert.AreEqual(5.197E-7, cStack);
        }

        [TestCase]
        public void TestStructureHasCorrectFlatBandVoltage()
        {
            var structure = CreateSiO2TestStructure();

            var roundedVfb = Math.Round(structure.FlatbandVoltage.Volts, 3);

            Assert.AreEqual(.305, roundedVfb);
        }

        [TestCase]
        public void TestMIMStructureHasCorrectStackCapacitance()
        {
            var structure = CreateMIMTestStructure();

            var roundedCStack = Math.Round(structure.StackCapacitance.FaradsPerSquareCentimeter, 9);

            Assert.AreEqual(1.727E-6, roundedCStack);
        }

        [TestCase]
        public void TestStructureWithOnlySemiconductorIsInvalid()
        {
            var semiconductor = new Semiconductor();
            semiconductor.BandGap = Energy.FromElectronVolts(1.125);
            semiconductor.ElectronAffinity = Energy.FromElectronVolts(4.05);
            semiconductor.DielectricConstant = 11.7;
            semiconductor.IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(1.41E10);
            semiconductor.DopingType = DopingType.N;
            semiconductor.DopantConcentration = Concentration.FromPerCubicCentimeter(1E18);

            var structure = new Structure();
            structure.AddLayer(semiconductor);

            Assert.That(!structure.IsValid);
        }

        [TestCase]
        public void TestStructureWithoutDielectricIsInvalid()
        {
            var topMetal = new Metal(Length.FromNanometers(10));
            topMetal.SetWorkFunction(Energy.FromElectronVolts(4.9));

            var semiconductor = new Semiconductor();
            semiconductor.BandGap = Energy.FromElectronVolts(1.125);
            semiconductor.ElectronAffinity = Energy.FromElectronVolts(4.05);
            semiconductor.DielectricConstant = 11.7;
            semiconductor.IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(1.41E10);
            semiconductor.DopingType = DopingType.N;
            semiconductor.DopantConcentration = Concentration.FromPerCubicCentimeter(1E18);

            var structure = new Structure();
            structure.AddLayer(semiconductor);
            structure.AddLayer(topMetal);

            Assert.That(!structure.IsValid);
        }
    }
}

