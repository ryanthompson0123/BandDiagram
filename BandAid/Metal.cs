using Band.Units;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Band
{
    public class Metal : Material
	{
        private Energy workFunctionValue;
		public override Energy WorkFunction
        {
            get { return workFunctionValue; }
        }

        public void SetWorkFunction(Energy energy)
        {
            workFunctionValue = energy;
        }

        public ChargeDensity ExtraCharge { get ; set; }

		public override Energy EnergyFromVacuumToBottomBand
		{
			get
			{
				return WorkFunction;
			}
		}

		public override Energy EnergyFromVacuumToEfi
		{
			get
			{
				return WorkFunction;
			}
		}

		public override Energy EnergyFromVacuumToTopBand
		{
			get
			{
				return WorkFunction;
			}
		}

        public Metal(Length thickness)
        {
            ExtraCharge = ChargeDensity.Zero;
            Thickness = thickness;
            Prepare();
        }

        public override Material DeepClone()
        {
            var metal = new Metal(Thickness);
            InitClone(metal);

            metal.SetWorkFunction(WorkFunction);
            metal.ExtraCharge = ExtraCharge;

            return metal;
        }

		// Since EField is uniform in metal, we can do this
		public override ElectricField GetElectricField(Length location)
		{
			return EvalPoints[0].ElectricField;
		}

		// Since potential is uniform in metal, we can do this
		public override ElectricPotential GetPotential(Length location)
		{
			return EvalPoints[0].Potential;
		}

		public sealed override void Prepare()
		{
			EvalPoints.Clear();
			EvalPoints.Add(new EvalPoint());
			EvalPoints.Add(new EvalPoint {
				Location = Thickness
			});
			EvalPoints.Sort();
		}

        public override List<PlotDataSet> GetEnergyDatasets(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name
            };

            var startLocation = EvalPoints.First().Location + offset;
            var endLocation = EvalPoints.Last().Location + offset;

            var startEnergy = -EnergyFromVacuumToTopBand - EvalPoints.First().Potential;
            var endEnergy = -EnergyFromVacuumToTopBand - EvalPoints.Last().Potential;

            dataset.DataPoints.Add(new PlotDataPoint
            {
                X = startLocation.Nanometers, 
                Y = startEnergy.ElectronVolts
            });
            dataset.DataPoints.Add(new PlotDataPoint
            {
                X = endLocation.Nanometers, 
                Y = endEnergy.ElectronVolts
            });

            return new List<PlotDataSet> { dataset };
        }
	}
}
