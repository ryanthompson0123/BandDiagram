using Band.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Band
{
	public abstract class Material
	{
		public string Name { get; set; }
		public string Notes { get; set; }
        public virtual Length Thickness { get; protected set; }

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

        protected void InitClone(Material material)
        {
            material.Name = Name;
            material.Notes = Notes;
            material.FillColor = FillColor;
            material.EvalPoints = EvalPoints.Select(ep => ep.DeepClone()).ToList();
        }

        public abstract Material DeepClone();

		public abstract ElectricField GetElectricField(Length location);
		public abstract ElectricPotential GetPotential(Length location);
		public abstract void Prepare();
	}
}