using Band.Units;
using System;
using System.Collections.Generic;

namespace Band
{
	public abstract class Material
	{
		public string Name { get; set; }
		public string Notes { get; set; }
		public virtual Length Thickness { get; set; }
		public string FillColor { get; set; }
		public List<EvalPoint> EvalPoints { get; set; }

		public abstract Energy EnergyFromVacuumToTopBand { get; }
		public abstract Energy EnergyFromVacuumToBottomBand { get; }
		public abstract Energy EnergyFromVacuumToEfi { get; }
        public abstract Energy WorkFunction { get; }

        public Structure ParentStructure { get; set; }

		protected Material()
		{
			EvalPoints = new List<EvalPoint>();
		}

		public abstract ElectricField GetElectricField(Length location);
		public abstract ElectricPotential GetPotential(Length location);
		public abstract void Prepare();
	}
}