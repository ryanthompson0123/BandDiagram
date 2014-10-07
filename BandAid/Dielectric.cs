using Band.Units;
using System;
using System.Linq;

namespace Band
{
	public class Dielectric : Material
	{

		public double DielectricConstant { get; set; }
		public string DielectricConstantExpression { get; set; }
		public Energy BandGap { get; set; }
		public Energy ElectronAffinity { get; set; }
		public Mass ElectronEffectiveMass { get; set; }
		public Mass HoleEffectiveMass { get; set; }

		public override Energy EnergyFromVacuumToBottomBand
		{
			get
			{
				return ElectronAffinity + BandGap;
			}
		}

		public override Energy EnergyFromVacuumToEfi
		{
			get
			{
				return ElectronAffinity + BandGap;
			}
		}

		public override Energy EnergyFromVacuumToTopBand
		{
			get
			{
				return ElectronAffinity;
			}
		}

        public override Energy WorkFunction
        {
            get
            {
                throw new NotImplementedException("Dielectrics don't have a work function?");
            }
        }

		public sealed override void Prepare()
		{
			// Check to see if there is a point at the beginning (0), if not, add one.
			if (EvalPoints.All(p => p.Location > Length.Zero))
			{
				EvalPoints.Add(new EvalPoint
				{
					Location = Length.Zero,
					ChargeDensity = ChargeDensity.Zero,
					ElectricField = ElectricField.Zero,
					Potential = ElectricPotential.Zero
				});
			}

			// Check to see if there is a point at the end (at the thickness), if not, add one.
			if (EvalPoints.All(p => p.Location < Thickness))
			{
				EvalPoints.Add(new EvalPoint {
					Location = Thickness,
                    ChargeDensity = ChargeDensity.Zero,
					ElectricField = ElectricField.Zero,
					Potential = ElectricPotential.Zero
				});
			}

			// Remove any zero charge points that aren't the beginning or end.
			EvalPoints.RemoveAll(p => 
				p.Location != Length.Zero && 
				p.Location != Thickness &&
                p.ChargeDensity == ChargeDensity.Zero);

			// Sort the points.
			EvalPoints.Sort();
		}

		/**
	    * Find the distance to the valance band in Cm when the energy is set to the
	    * provided level.
	    *
	    * @param energy - energy level used to evaluate the structure.
	    *
	    * @return thickness - distance to valance band in Cm.
	    */
		public Length DistanceToValenceBand(Energy energy) {
			// find between which points the energy of interest lies
			// if energy is above all the points return 0 distance
			if (EvalPoints.All(p => ValenceBandEnergy(p) >= energy))
			{
				return Length.Zero;
			}


			EvalPoint abovePoint = null;
			EvalPoint belowPoint = null;
            var aboveEnergy = Energy.FromElectronVolts(1E10);
			var belowEnergy = Energy.FromElectronVolts(-1E10);
			// a ridiculus start energy if we
			// find a point above the energy it will definately be lower than this value.
			foreach (var p in EvalPoints)
			{
				var vbEnergy = ValenceBandEnergy(p);
				if (vbEnergy > energy && vbEnergy < aboveEnergy)
				{
					aboveEnergy = vbEnergy;
					abovePoint = p;
				}

				if (vbEnergy < energy && vbEnergy > belowEnergy)
				{
					belowEnergy = vbEnergy;
					belowPoint = p;
				}
			}

			// if we didn't find a point in energy below the input energy then tunnel through whole dielectric
			if (abovePoint == null)
			{
				return Thickness;
			}

			// make sure that we have above and below points
			if (belowPoint == null || abovePoint == null) {
				return null; // negative thickness so we know something is wrong.
			}

			// interpolate cross points
			var slope = (aboveEnergy - belowEnergy).ElectronVolts 
				/ (abovePoint.Location - belowPoint.Location).Centimeters;
			var intercept = aboveEnergy.ElectronVolts - slope * abovePoint.Location.Centimeters;
			var distance = (energy.ElectronVolts - intercept) / slope;

			return Length.FromCentimeters(distance);
		}

		/**
	    * Find the distance to the conduction band in Cm when the energy is set to the
	    * provided level.
	    *
	    * @param energy - energy level used to evaluate the structure.
	    *
	    * @return thickness - distance to conduction band in Cm.
	    */
		public Length DistanceToConductionBand(Energy energy)
		{
			// find between which points the energy of interest lies
			// if energy is above all the points return 0 distance
			if (EvalPoints.All(p => ConductionBandEnergy(p) <= energy))
			{
				return Length.Zero;
			}

			EvalPoint abovePoint = null;
			EvalPoint belowPoint = null;
            var aboveEnergy = Energy.FromElectronVolts(1E10);
            var belowEnergy = Energy.FromElectronVolts(-1E10);
            // a ridiculus start energy if we
			// find a point above the energy it will definately be lower than this value.
			foreach (var p in EvalPoints)
			{
				var cbEnergy = ConductionBandEnergy(p);
				if (cbEnergy > energy && cbEnergy < aboveEnergy)
				{
					aboveEnergy = cbEnergy;
					abovePoint = p;
				}

				if (cbEnergy < energy && cbEnergy > belowEnergy)
				{
					belowEnergy = cbEnergy;
					belowPoint = p;
				}
			}

			// if we didn't find a point in energy below the input energy then tunnel through whole dielectric
			if (belowPoint == null) {
				return Thickness;
			}

			// make sure that we have above and below points
			if (belowPoint == null || abovePoint == null) {
				// we did something wrong
				return null; // negative thickness so we know something is wrong
			}

			// interpolate cross points
            var slope = (aboveEnergy - belowEnergy).ToPotential()
                / (abovePoint.Location - belowPoint.Location);
			var intercept = aboveEnergy.ToPotential() - slope * abovePoint.Location;
			var distance = (energy.ToPotential() - intercept) / slope;

            return distance;
		}

		/**
	    * The energy value for the conduction band at the given point's location.
	    *
	    * @param index - index value for the point to evaluate.
	    *
	    * @return energy - energy value at the specified point.
	    */
		public Energy ConductionBandEnergy(EvalPoint p)
		{
            return -p.Potential - ElectronAffinity;
		}

		/**
	    * The energy value for the valance band at the given point's location.
	    *
	    * @param index - index value for the point to evaluate.
	    *
	    * @return energy - energy value at the specified point.
	    */
		public Energy ValenceBandEnergy(EvalPoint p)
		{
			return -p.Potential - ElectronAffinity - BandGap;
		}

		public Permittivity Permittivity
		{
			get
			{
                // TODO: Support e-field dependent permittivity
				return DielectricConstant * Permittivity.OfFreeSpace;
			}
		}

        public Dielectric(Length thickness)
        {
            Thickness = thickness;
            Prepare();
        }

        public override Material DeepClone()
        {
            var dielectric = new Dielectric(new Length(Thickness.Meters))
            {
                    DielectricConstant = DielectricConstant,
                    DielectricConstantExpression = DielectricConstantExpression,
                    BandGap = BandGap,
                    ElectronAffinity = ElectronAffinity,
                    HoleEffectiveMass = HoleEffectiveMass,
                    ElectronEffectiveMass = ElectronEffectiveMass
            };

            InitClone(dielectric);

            return dielectric;
        }

        public CapacitanceDensity OxideCapacitance
        {
            get
            {
                return DielectricConstant * Permittivity.OfFreeSpace / Thickness;
            }
        }

		public ElectricPotential VoltageDrop()
		{
			return EvalPoints.First().Potential - EvalPoints.Last().Potential;
		}

		public override ElectricPotential GetPotential(Length location)
		{
			var startPotential = EvalPoints.First().Potential;
			var endPotential = EvalPoints.Last().Potential;
			var slope = (endPotential - startPotential) / Thickness;
			return slope * location + startPotential;
		}

		// Since eField is uniform in a dielectric, we can just grab the first one.
		public override ElectricField GetElectricField(Length location)
		{
			return EvalPoints.First().ElectricField;
		}
	}
}