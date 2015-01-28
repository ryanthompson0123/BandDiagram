using Band.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Band
{
    public enum MaterialType { Metal, Dielectric, Semiconductor }

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

        public PlotDataSet GetPotentialDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                PlotColor = FillColor,
                LineThickness = 2
            };

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                var potential = point.Potential;
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y =potential.Volts
                });
            }

            return dataset;
        }

        public abstract List<PlotDataSet> GetEnergyDatasets(Length offset);

        public virtual PlotDataSet GetElectricFieldDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                PlotColor = FillColor,
                LineThickness = 2
            };

            if (ParentStructure.Layers.IndexOf(this) > 0)
            {
                var previousMaterial = ParentStructure.Layers[ParentStructure.Layers.IndexOf(this) - 1];
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = offset.Nanometers,
                    Y = previousMaterial.GetElectricFieldDataset(Length.Zero).DataPoints.Last().Y
                });
            }

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = point.ElectricField.MegavoltsPerCentimeter
                });
            }

            return dataset;
        }

        public virtual PlotDataSet GetChargeDensityDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                PlotColor = FillColor,
                LineThickness = 2
            };

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                var charge = point.ChargeDensity;

                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = 0.0
                });
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = charge.MicroCoulombsPerSquareCentimeter
                });
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = 0.0
                });
            }

            return dataset;
        }
	}
}