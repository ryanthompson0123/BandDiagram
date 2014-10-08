using System;
using NUnit.Framework;
using Band;
using Band.Units;

namespace BandAidTests
{
    [TestFixture]
    public class SemiconductorTests
    {
        private static Semiconductor CreateTestSemiconductor()
        {
            var semiconductor = new Semiconductor();
            semiconductor.BandGap = Energy.FromElectronVolts(1.1252);
            semiconductor.ElectronAffinity = Energy.FromElectronVolts(4.05);
            semiconductor.DielectricConstant = 11.7;
            semiconductor.IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(1.41E10);
            semiconductor.DopingType = DopingType.N;
            semiconductor.DopantConcentration = Concentration.FromPerCubicCentimeter(1E18);

            return semiconductor;
        }

        [TestCase]
        public void TestCalculatesCorrectFermiLevel()
        {
            var semiconductor = CreateTestSemiconductor();

            var roundedFermiLevel 
                = Math.Round(semiconductor.EnergyFromVacuumToEfi.ElectronVolts, 4);

            Assert.AreEqual(4.6126, roundedFermiLevel);
        }

        [TestCase]
        public void TestCalculatesCorrectWorkFunction()
        {
            var shellStructure = new Structure();
            shellStructure.Temperature = new Temperature(300);

            var semiconductor = CreateTestSemiconductor();
            shellStructure.AddLayer(semiconductor);

            var roundedWorkFunction = Math.Round(semiconductor.WorkFunction.ElectronVolts, 3);

            Assert.AreEqual(4.145, roundedWorkFunction);
        }

        [TestCase]
        public void TestCalculatesChargeYCorrectly()
        {
            var shellStructure = new Structure();
            shellStructure.Temperature = new Temperature(300);

            var semiconductor = CreateTestSemiconductor();
            shellStructure.AddLayer(semiconductor);

            var testPotential = new ElectricPotential(-.175);

            var chargeY = semiconductor.GetChargeY(testPotential);
            var roundedChargeY = Math.Round(chargeY.CoulombsPerSquareCentimeter, 5);

            Assert.AreEqual(.16003, roundedChargeY);
        }

        [TestCase]
        public void TestCalculatesChargeDensityCorrectly()
        {
            var shellStructure = new Structure();
            shellStructure.Temperature = new Temperature(300);

            var semiconductor = CreateTestSemiconductor();
            shellStructure.AddLayer(semiconductor);

            var testPotential = new ElectricPotential(-.75);

            var charge = semiconductor.GetChargeDensity(testPotential);
            var roundedCharge = Math.Round(charge.CoulombsPerSquareCentimeter, 10);

            Assert.AreEqual(4.903E-7, roundedCharge);
        }

        [TestCase]
        public void TestCalculatesSurfacePotentialCorrectly()
        {
            var shellStructure = new Structure();
            shellStructure.Temperature = new Temperature(300);

            var semiconductor = CreateTestSemiconductor();
            shellStructure.AddLayer(semiconductor);

            var testCharge = ChargeDensity.FromCoulombsPerSquareCentimeter(-.25);

            var potential = semiconductor.GetSurfacePotential(testCharge);
            var roundedPotential = Math.Round(potential.Volts, 3);

            Assert.AreEqual(.766, roundedPotential);
        }

        [TestCase]
        public void TestCalculatesElectricFieldCorrectly()
        {
            var shellStructure = new Structure();
            shellStructure.Temperature = new Temperature(300);

            var semiconductor = CreateTestSemiconductor();
            shellStructure.AddLayer(semiconductor);

            var testPotential = new ElectricPotential(-.25);

            var electricField = semiconductor.GetElectricField(testPotential);
            var roundedEField = Math.Round(electricField.VoltsPerCentimeter, 3);

            Assert.AreEqual(-263312.736, roundedEField);
        }

        [TestCase]
        public void TestCalculatesCapacitanceDensityCorrectly()
        {
            var shellStructure = new Structure();
            shellStructure.Temperature = new Temperature(300);

            var semiconductor = CreateTestSemiconductor();
            semiconductor.SurfacePotential = new ElectricPotential(-0.17558930053710936);
            shellStructure.AddLayer(semiconductor);

            var capacitance = semiconductor.CapacitanceDensity;
            var roundedCapacitance = Math.Round(capacitance.FaradsPerSquareCentimeter, 10);

            Assert.AreEqual(7.436E-7, roundedCapacitance);
        }
    }
}