using Band.Units;
using System;

namespace Band
{
	public class Metal : Material
	{
		public override Energy WorkFunction { get; set; }
		public ElectricCharge ExtraCharge { get ; set; }

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

		public override void Prepare()
		{
			EvalPoints.Clear();
			EvalPoints.Add(new EvalPoint());
			EvalPoints.Add(new EvalPoint {
				Location = Thickness
			});
			EvalPoints.Sort();
		}
	}
}
