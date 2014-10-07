using NUnit.Framework;
using System;
using Band;
using Band.Units;

namespace BandAidTests
{
    [TestFixture]
    public class MetalTests
    {
        [TestCase]
        public void TestWorkFunction()
        {
            var metal = new Metal(Length.FromNanometers(10));
            var workFunction = Energy.FromElectronVolts(5);

            metal.SetWorkFunction(workFunction);

            Assert.AreEqual(metal.WorkFunction, workFunction);
        }

        [TestCase]
        public void TestPotential()
        {
            var thickness = Length.FromNanometers(10);
            var metal = new Metal(thickness);
            metal.SetWorkFunction(Energy.FromElectronVolts(5));

            var expectedPotential = ElectricPotential.Zero;

            var actualPotential = metal.GetPotential(thickness);

            Assert.AreEqual(expectedPotential, actualPotential);
        }
    }
}

