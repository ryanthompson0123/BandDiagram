﻿using Band.Units;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Band
{
    [JsonObject(MemberSerialization.OptIn)]
	public class Dielectric : Material
	{
        #region Configuration

        [JsonProperty]
        private double dielectricConstant;
		public double DielectricConstant
        {
            get { return dielectricConstant; }
            set { SetProperty(ref dielectricConstant, value); }
        }

        [JsonProperty]
        private Energy bandGap;
		public Energy BandGap
        {
            get { return bandGap; }
            set { SetProperty(ref bandGap, value); }
        }

        [JsonProperty]
        private Energy electronAffinity;
		public Energy ElectronAffinity
        {
            get { return electronAffinity; }
            set { SetProperty(ref electronAffinity, value); }
        }

        [JsonProperty]
        private Mass electronEffectiveMass;
		public Mass ElectronEffectiveMass
        {
            get { return electronEffectiveMass; }
            set { SetProperty(ref electronEffectiveMass, value); }
        }

        [JsonProperty]
        private Mass holeEffectiveMass;
        public Mass HoleEffectiveMass
        {
            get { return holeEffectiveMass; }
            set { SetProperty(ref holeEffectiveMass, value); }
        }

        #endregion

        #region Property Accessors

        private Energy lazyEnergyFromVacuumToBottomBand;
		public override Energy EnergyFromVacuumToBottomBand
		{
            get
            {
                if (lazyEnergyFromVacuumToBottomBand == null)
                {
                    lazyEnergyFromVacuumToBottomBand = ElectronAffinity + BandGap;
                }

                return lazyEnergyFromVacuumToBottomBand;
            }
		}
            
		public override Energy EnergyFromVacuumToEfi
		{
            get
            {
                return EnergyFromVacuumToBottomBand;
            }
		}
            
		public override Energy EnergyFromVacuumToTopBand
		{
            get { return ElectronAffinity; }
		}

        private Permittivity lazyPermittivity;
        public Permittivity Permittivity
        {
            get
            {
                if (lazyPermittivity == null)
                {
                    lazyPermittivity = DielectricConstant * Permittivity.OfFreeSpace;
                }

                return lazyPermittivity;
            }
        }

        private CapacitanceDensity lazyOxideCapacitance;
        public CapacitanceDensity OxideCapacitance
        {
            get
            {
                if (lazyOxideCapacitance == null)
                {
                    lazyOxideCapacitance = Permittivity / Thickness;
                }

                return lazyOxideCapacitance;
            }
        }

        public ElectricPotential VoltageDrop
        {
            get
            {
                return EvalPoints.First().Potential - EvalPoints.Last().Potential;
            }
        }

        #endregion

        [JsonConstructor]
        internal Dielectric()
        {
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);
        }

        public override Material WithThickness(Length thickness)
        {
            var dielectric = new Dielectric
            {
                DielectricConstant = DielectricConstant,
                BandGap = BandGap,
                ElectronAffinity = ElectronAffinity,
                HoleEffectiveMass = HoleEffectiveMass,
                ElectronEffectiveMass = ElectronEffectiveMass
            };

            InitClone(dielectric, thickness);

            dielectric.Prepare();

            return dielectric;
        }

		public sealed override void Prepare()
		{
            if (Thickness == null) return;
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

        public override PlotDataSet GetElectricFieldDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                LineThickness = 2,
                PlotColor = FillColor
            };

            var previousMaterial = ParentStructure.Layers[ParentStructure.Layers.IndexOf(this) - 1];
            var lastEField = previousMaterial.GetLastElectricField();

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;

                if (point != EvalPoints.Last())
                {
                    dataset.DataPoints.Add(new PlotDataPoint
                    {
                        X = location.Nanometers, 
                        Y = lastEField.MegavoltsPerCentimeter
                    });
                }
                    
                lastEField = point.ElectricField;

                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = lastEField.MegavoltsPerCentimeter
                });
            }

            return dataset;
        }

        public override List<PlotDataSet> GetEnergyDatasets(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                LineThickness = 2,
                PlotColor = FillColor
            };

            var location1 = EvalPoints.First().Location + offset;
            var energy1 = -EnergyFromVacuumToTopBand - EvalPoints.First().Potential;

            dataset.DataPoints.Add(new PlotDataPoint
            {
                X = location1.Nanometers, 
                Y = energy1.ElectronVolts
            });

            var location2 = EvalPoints.First().Location + offset;
            var energy2 = -EnergyFromVacuumToBottomBand - EvalPoints.First().Potential;

            dataset.DataPoints.Add(new PlotDataPoint
            {
                X = location2.Nanometers, 
                Y = energy2.ElectronVolts
            });

            // for in between points
            foreach (var point in EvalPoints)
            {
                if (point == EvalPoints.First()) continue;

                var locationA = point.Location + offset;
                var energyA = -EnergyFromVacuumToBottomBand - point.Potential;

                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = locationA.Nanometers, 
                    Y = energyA.ElectronVolts
                });
            }

            // for the last point
            var location3 = EvalPoints.Last().Location + offset;
            var energy3 = -EnergyFromVacuumToTopBand - EvalPoints.Last().Potential;

            dataset.DataPoints.Add(new PlotDataPoint
            {
                X = location3.Nanometers, 
                Y = energy3.ElectronVolts
            });

            // for in between points
            foreach (var point in EvalPoints.Reverse<EvalPoint>())
            {
                var locationB = point.Location + offset;
                var energyB = -EnergyFromVacuumToTopBand - point.Potential;
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = locationB.Nanometers, 
                    Y = energyB.ElectronVolts
                });
            }

            var location4 = EvalPoints.First().Location + offset;
            var energy4 = -EnergyFromVacuumToTopBand - EvalPoints.First().Potential;
            dataset.DataPoints.Add(new PlotDataPoint
            {
                X = location4.Nanometers, 
                Y = energy4.ElectronVolts
            });

            return new List<PlotDataSet> { dataset };
        }
	}
}