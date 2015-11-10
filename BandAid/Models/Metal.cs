using Band.Units;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Band
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Metal : Material
	{
        [JsonProperty]
        public ChargeDensity ExtraCharge { get ; private set; }

		public override Energy EnergyFromVacuumToBottomBand
		{
            get { return WorkFunction; }
		}

		public override Energy EnergyFromVacuumToEfi
		{
            get { return WorkFunction; }
		}

		public override Energy EnergyFromVacuumToTopBand
		{
            get { return WorkFunction; }
		}

        [JsonConstructor]
        internal Metal()
        {
            ExtraCharge = ChargeDensity.Zero;
        }

        [OnDeserialized]
        new public void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);
        }

        public override Material WithThickness(Length thickness)
        {
            var metal = new Metal();
            InitClone(metal, thickness);

            metal.ExtraCharge = ExtraCharge;

            metal.Prepare();

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
            if (Thickness == null) return;

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
                Name = Name,
                LineThickness = 2,
                PlotColor = FillColor
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
