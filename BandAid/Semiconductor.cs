using Band.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Band
{
	public enum DopingType
	{
		P,
		N
	}

	public class Semiconductor : Material
	{
		public ElectricPotential SurfacePotential { get; set; }
		public double DielectricConstant { get; set; }
		public Energy BandGap { get; set; }
		public Energy ElectronAffinity { get; set; }
		public DopingType DopingType { get; set; }
		public Concentration DopantConcentration { get; set; }
		public Concentration IntrinsicCarrierConcentration { get; set; }

		public ElectricPotential ThermalVoltage
		{
			get { return ParentStructure.Temperature.ToEnergy() / ElectricCharge.Elementary; }
		}

		public override Length Thickness
		{
			get
			{
				return Length.FromMicrometers(50);
			}
		}

		public override Energy EnergyFromVacuumToTopBand
		{
			get { return ElectronAffinity; }
		}

		public override Energy EnergyFromVacuumToBottomBand
		{
			get { return ElectronAffinity + BandGap; }
		}

		public override Energy EnergyFromVacuumToEfi
		{
			get { return ElectronAffinity + BandGap / 2; }
		}

		public Permittivity Permittivity
		{
			get { return DielectricConstant * Permittivity.OfFreeSpace; }
		}

		public override Energy WorkFunction
		{
            get
            {
                Energy phiFEv = PhiF;
                return EnergyFromVacuumToEfi + phiFEv; 
            }
		}

		public ElectricPotential PhiF
		{
			get
			{
				var phiF = ThermalVoltage * Math.Log(DopantConcentration / IntrinsicCarrierConcentration);

				return DopingType == DopingType.P ? phiF : -phiF;
			}
		}

        private double SemiconductorConstant
        {
            get
            {
                return Math.Sqrt(2 * ElectricCharge.Elementary.Coulombs
                    * Permittivity.OfFreeSpace.FaradsPerCentimeter * DielectricConstant
                    * DopantConcentration.PerCubicCentimeter);
            }
        }

		public sealed override void Prepare()
		{
			EvalPoints.Clear();
			EvalPoints.Add(new EvalPoint {
				Location = Length.Zero,
                ChargeDensity = ChargeDensity.Zero,
				ElectricField = ElectricField.Zero,
				Potential = ElectricPotential.Zero
			});

			EvalPoints.Add(new EvalPoint {
                Location = Length.FromNanometers(50),
                ChargeDensity = ChargeDensity.Zero,
				ElectricField = ElectricField.Zero,
				Potential = ElectricPotential.Zero
			});
		}
            
		public ChargeDensity GetChargeY(ElectricPotential potential)
		{
			if (DopingType == DopingType.P)
			{
				var cConc = ElectricCharge.Elementary * 
                    (FreeHoleConcentration * Math.Exp(-potential / ThermalVoltage) 
                    - FreeElectronConcentration * Math.Exp(potential / ThermalVoltage) 
                    - DopantConcentration);

                // Apparently this is ok, but I don't know why - RT
                return ChargeDensity.FromCoulombsPerSquareCentimeter(cConc.CoulombsPerCubicCentimeter);
			}
			else
			{
                var cConc = -ElectricCharge.Elementary * 
                    (FreeElectronConcentration * Math.Exp(potential / ThermalVoltage) 
                    - FreeHoleConcentration * Math.Exp(-potential / ThermalVoltage) 
                    - DopantConcentration);

                // Apparently this is ok, but I don't know why - RT
                return ChargeDensity.FromCoulombsPerSquareCentimeter(cConc.CoulombsPerCubicCentimeter);
			}
        }

		public Concentration FreeElectronConcentration
		{
			get
			{
				if (DopingType == DopingType.P)
				{
					return IntrinsicCarrierConcentration * (IntrinsicCarrierConcentration / DopantConcentration);
				}
				else
				{
					return DopantConcentration;
				}
			}
		}

		public Concentration FreeHoleConcentration
		{
			get
			{
				if (DopingType == DopingType.N)
				{
					return IntrinsicCarrierConcentration * (IntrinsicCarrierConcentration / DopantConcentration);
				}
				else
				{
					return DopantConcentration;
				}
			}
		}

        public Semiconductor()
        {
            Prepare();
        }

        public override Material DeepClone()
        {
            var semiconductor = new Semiconductor()
            {
                    SurfacePotential = SurfacePotential,
                    DielectricConstant = DielectricConstant,
                    BandGap = BandGap,
                    ElectronAffinity = ElectronAffinity,
                    DopingType = DopingType,
                    DopantConcentration = DopantConcentration,
                    IntrinsicCarrierConcentration = IntrinsicCarrierConcentration
            };

            InitClone(semiconductor);
            return semiconductor;
        }

        public ChargeDensity GetChargeDensity(ElectricPotential surfacePotential)
        {
            ElectricPotential innerTerm;
			if (DopingType == DopingType.P)
			{
                innerTerm = ThermalVoltage * Math.Exp(-surfacePotential / ThermalVoltage)
				                 + surfacePotential - ThermalVoltage + Math.Exp(-2 * PhiF / ThermalVoltage)
				                 * (ThermalVoltage * Math.Exp(surfacePotential / ThermalVoltage) - surfacePotential 
								 - ThermalVoltage);

				
			}
			else
			{
                innerTerm = ThermalVoltage * Math.Exp(surfacePotential / ThermalVoltage)
				                 - surfacePotential - ThermalVoltage + Math.Exp(2 * PhiF / ThermalVoltage)
				                 * (ThermalVoltage * Math.Exp(-surfacePotential / ThermalVoltage) - surfacePotential 
								 - ThermalVoltage);
			}

            var charge = ChargeDensity.FromCoulombsPerSquareCentimeter(SemiconductorConstant * Math.Sqrt(innerTerm.Volts));

            return surfacePotential >= ElectricPotential.Zero ? -charge : charge;
		}

        private bool ShouldKeepTryingSurfacePotential(ChargeDensity charge,
            ChargeDensity guessCharge, int iterationNumber)
        {
            var maxDelta = ChargeDensity.FromCoulombsPerSquareCentimeter(Math.Abs(charge.CoulombsPerSquareCentimeter * 1e-6));
            return ((charge - maxDelta > guessCharge) || (guessCharge > charge + maxDelta))
                    && iterationNumber < 1000;
        }

        public ElectricPotential GetSurfacePotential(ChargeDensity charge)
		{
			var highPotential = BandGap.ToPotential() * 3;
            var lowPotential = ElectricPotential.Zero - 2 * BandGap;
			var guessPotential = (highPotential + lowPotential) / 2;
			var guessCharge = GetChargeDensity(guessPotential);

            for (int i = 0; ShouldKeepTryingSurfacePotential(charge, guessCharge, i); i++)
            {
                if (guessCharge > charge)
                {
                    lowPotential = guessPotential;
                }
                else
                {
                    highPotential = guessPotential;
                }

                guessPotential = (highPotential + lowPotential) / 2;
                guessCharge = GetChargeDensity(guessPotential);
            }

            return guessPotential;
		}

        public ElectricField GetElectricField(ElectricPotential potential)
        {
            ElectricPotential typeDependantTerm;
			if (DopingType == DopingType.P)
            {
                typeDependantTerm = 
                    ThermalVoltage * Math.Exp(-potential / ThermalVoltage) 
                    + potential
                    - ThermalVoltage 
                    + Math.Exp(-2 * PhiF / ThermalVoltage) 
                        * (ThermalVoltage * Math.Exp(potential / ThermalVoltage) 
                        - potential 
                        - ThermalVoltage);
			}
			else 
            {
                typeDependantTerm = 
                    ThermalVoltage * Math.Exp(potential / ThermalVoltage) 
                    - potential 
                    - ThermalVoltage 
                    + Math.Exp(2 * PhiF / ThermalVoltage) 
                        * (ThermalVoltage * Math.Exp(-potential / ThermalVoltage) 
                        + potential 
                        - ThermalVoltage);
			}

            var otherTerm = SemiconductorConstant / (Permittivity.OfFreeSpace.FaradsPerCentimeter
                * DielectricConstant);

            var value = ElectricField.FromVoltsPerCentimeter(otherTerm * Math.Sqrt(typeDependantTerm.Volts));

            return potential >= ElectricPotential.Zero ? value : -value;
		}

        public CapacitanceDensity CapacitanceDensity
        {
            get
            {
                // capacitance value taken from Tsividis
                // get the surface potential
                var phiS = SurfacePotential;

                if (DopingType == DopingType.P)
                {
                    var numerator = 1 - Math.Exp(-phiS / ThermalVoltage)
                        + Math.Exp(-2 * PhiF / ThermalVoltage)
                        * (Math.Exp(phiS / ThermalVoltage) - 1);

                    var denominatorInside = ThermalVoltage * Math.Exp(-phiS / ThermalVoltage)
                        + phiS - ThermalVoltage
                        + Math.Exp(-2 * PhiF / ThermalVoltage)
                        * (ThermalVoltage * Math.Exp(phiS / ThermalVoltage)
                        - phiS - ThermalVoltage);

                    var denominator = 2 * Math.Sqrt(denominatorInside.Volts);

                    var value = CapacitanceDensity.FromFaradsPerSquareCentimeter(
                        SemiconductorConstant * numerator / denominator);

                    return phiS > ElectricPotential.Zero ? value : -value;
                }
                else
                {
                    var numerator = 1 - Math.Exp(phiS / ThermalVoltage) 
                        + Math.Exp(2 * PhiF / ThermalVoltage) 
                        * (Math.Exp(-phiS / ThermalVoltage) - 1);

                    var denominatorInside = ThermalVoltage * Math.Exp(phiS / ThermalVoltage)
                        - phiS - ThermalVoltage
                        + Math.Exp(2 * PhiF / ThermalVoltage)
                        * (ThermalVoltage * Math.Exp(-phiS / ThermalVoltage)
                        + phiS - ThermalVoltage);

                    var denominator = 2 * Math.Sqrt(denominatorInside.Volts);

                    var value = CapacitanceDensity.FromFaradsPerSquareCentimeter(
                        SemiconductorConstant * numerator / denominator);

                    return phiS > ElectricPotential.Zero ? -value : value;
                }
            }
        }

        public override ElectricField GetElectricField(Length location)
        {
            var pointPastLocation = EvalPoints.FirstOrDefault(p => location < p.Location);

            return pointPastLocation != null ?
                pointPastLocation.ElectricField : ElectricField.Zero;
        }

        public override ElectricPotential GetPotential(Length location)
        {
            var pointPastLocation = EvalPoints.FirstOrDefault(p => location < p.Location);

            return pointPastLocation != null ?
                pointPastLocation.Potential : ElectricPotential.Zero;
        }

        public ChargeDensity GetDepletionCharge(ElectricPotential surfacePotential)
        {
            if (DopingType == DopingType.P)
            {
                if (surfacePotential <= ElectricPotential.Zero) return ChargeDensity.Zero;

                var realSurfacePotential = surfacePotential - ThermalVoltage;
                var value = -SemiconductorConstant * Math.Sqrt(realSurfacePotential.Volts);

                return ChargeDensity.FromCoulombsPerSquareCentimeter(value);
            }
            else
            {
                // TODO: Implement for N type!
                throw new NotImplementedException();
            }
        }

        public override PlotDataSet GetChargeDensityDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name
            };

            for (var i = 0; i < EvalPoints.Count; i++)
            {
                var location = EvalPoints[i].Location + offset;
                var charge = EvalPoints[i].ChargeDensity;

                if (i == 0)
                {
                    dataset.DataPoints.Add(new Tuple<double, double>(
                        location.Nanometers, 0.0));
                }

                dataset.DataPoints.Add(new Tuple<double, double>(
                    location.Nanometers, charge.CoulombsPerSquareCentimeter));
            }

            return dataset;
        }

        public override List<PlotDataSet> GetEnergyDatasets(Length offset)
        {
            var cbDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Conduction Band", Name)
            };

            var vbDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Valance Band", Name)
            };

            var efiDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Fermi Level", Name)
            };

            var wfDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Work Function", Name)
            };

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                var cbEnergy = -EnergyFromVacuumToTopBand - point.Potential;
                var vbEnergy = -EnergyFromVacuumToBottomBand - point.Potential;
                var efiEnergy = -EnergyFromVacuumToEfi - point.Potential;
                var wfEnergy = -WorkFunction - point.Potential;

                cbDataset.DataPoints.Add(new Tuple<double, double>(
                    location.Nanometers, cbEnergy.ElectronVolts));
                vbDataset.DataPoints.Add(new Tuple<double, double>(
                    location.Nanometers, vbEnergy.ElectronVolts));
                efiDataset.DataPoints.Add(new Tuple<double, double>(
                    location.Nanometers, efiEnergy.ElectronVolts));
                wfDataset.DataPoints.Add(new Tuple<double, double>(
                    location.Nanometers, wfEnergy.ElectronVolts));
            }

            return new List<PlotDataSet> { cbDataset, vbDataset, efiDataset, wfDataset };
        }
			
		// *** NEW FUNCTIONS ***
		/*
		public double getCapacitanceFPerCm() {
			return capacitance();
		}

		public static double tempBandGap(String expr, double value) { // returns band gap at temp = value    
			return Functions.evaluateExpression(expr, 'T', value);
		}

		//returns intrinsic carrier concentration at temp = value
		public static double tempNi(String expr, double value) {
			return Functions.evaluateExpression(expr, 'T', value);
		}



		public double dit(double energy) {
			if (ditType == Constant.NONE) {
				return 0.0;
			}
			else {
				if (ditType == Constant.CONSTANT) {
					if (energy < -electronAffinity - bandGap) {
						return 0.0;
					}
					if (energy > -electronAffinity) {
						return 0.0;
					}
					return nit / bandGap;
				}
				else {
					if (ditType == Constant.PARABOLIC) {
						if (energy < -electronAffinity - bandGap) {
							return 0.0;
						}
						if (energy > -electronAffinity) {
							return 0.0;
						}
						double offset = energy + electronAffinity + bandGap;
						return 12 * (-co * bandGap + nit) / Math.pow(bandGap,3) *
							Math.pow(offset - bandGap / 2,2) + co;
					}
					else {
						if (ditType == Constant.GAUSSIAN) {
							if (o1 == 0 || o2 == 0) {
								return 0.0;
							}

							return nit * per1 / (o1 * Constant.Sqrt2Pi) * Math.exp(-Math.pow(energy - u1, 2) /
								(2 * o1 * o1)) + nit * (1 - per1) / (o2 * Constant.Sqrt2Pi) *
								Math.exp(-Math.pow(energy - u2, 2) / (2 * o2 * o2));
						}
					}
				}
			}
			return 0.0;
		}

		public double citFPerCm(double phiS) {
			if (ditType == Constant.NONE) {
				return 0.0;
			}
			else {
				if (ditType == Constant.CONSTANT) {
					double offset;
					if (getPType() == true) {
						offset = bandGap / 2 - phiF() + phiS;
					}
					else {
						offset = bandGap / 2 - phiF() + phiS;
					}
					if (offset < 0) {
						return 0.0;
					}
					if (offset > bandGap) {
						return 0.0;
					}
					return Constant.ElectronCharge * nit / bandGap;
				}
				else {
					if (ditType == Constant.PARABOLIC) {
						double offset;
						if (getPType() == true) {
							offset = bandGap / 2 - phiF() + phiS;
						}
						else {
							offset = bandGap / 2 - phiF() + phiS;
						}
						if (offset < 0) {
							return 0.0;
						}
						if (offset > bandGap) {
							return 0.0;
						}
						return Math.abs(Constant.ElectronCharge * (12 * (-co * bandGap + nit) /
							Math.pow(bandGap, 3) *
							Math.pow(offset - bandGap / 2, 2) + co));
					}
					else {
						if (ditType == Constant.GAUSSIAN) {
							double offset;
							if (getPType() == true) {
								offset = -electronAffinity - bandGap / 2 - phiF() + phiS;
							}
							else {
								offset = -electronAffinity - bandGap / 2 - phiF() + phiS;
							}

							if (o1 == 0.0 || o2 == 0.0) {
								return 0.0;
							}

							return Constant.ElectronCharge*Math.abs(nit * per1 / (o1 * Constant.Sqrt2Pi) * Math.exp(-Math.pow(offset - u1, 2) /
								(2 * o1 * o1)) + nit * (1 - per1) / (o2 * Constant.Sqrt2Pi) *
								Math.exp(-Math.pow(offset - u2, 2) / (2 * o2 * o2)));
						}
					}
				}
			}
			return 0.0;
		}

		public double qit(double phiS) {
			if (ditType == Constant.NONE) {
				return 0.0;
			}
			else {
				if (ditType == Constant.CONSTANT) {
					double offset;
					if (getPType() == true) {
						offset = bandGap / 2 - phiF() + phiS;
					}
					else {
						offset = bandGap / 2 - phiF() + phiS;
					}
					if (offset < 0) {
						return 0.0;
					}
					if (offset > bandGap) {
						return -Constant.ElectronCharge * nit;
					}
					return -Constant.ElectronCharge* (offset) * nit / bandGap;
				}
				else {
					if (ditType == Constant.PARABOLIC) {
						double offset;
						if (getPType() == true) {
							offset = bandGap / 2 - phiF() + phiS;
						}
						else {
							offset = bandGap / 2 - phiF() + phiS;
						}
						if (offset < 0) {
							return 0.0;
						}
						if (offset > bandGap) {
							return -Constant.ElectronCharge * nit;
						}

						return -Constant.ElectronCharge * (offset / Math.pow(bandGap, 3) *
							(-2 * co * bandGap * (2 * Math.pow(offset, 2) - 3 * offset * bandGap +
								bandGap * bandGap) + (4 * Math.pow(offset, 2) - 6 * offset * bandGap +
									3 * bandGap * bandGap) * nit));
					}
					else {
						if (ditType == Constant.GAUSSIAN) {
							double offset = -electronAffinity - bandGap / 2 - phiF() + phiS;
							return -Constant.ElectronCharge * (nit * getPer1() / 2 * (1 + BandMath.erf((offset - u1) / (o1 *
								Constant.Sqrt2))) + nit * (1 - getPer1())) / 2 * (1 + BandMath.erf((offset - u2) / (o2 *
									Constant.Sqrt2)));
						}
					}
				}
			}
			return 0.0;
		}

		public double ditMaxX() {
			double buffer = 0.4;
			if (ditType == Constant.NONE) {
				return -electronAffinity + buffer;
			}
			else {
				if (ditType == Constant.CONSTANT) {
					return -electronAffinity + buffer;
				}
				else {
					if (ditType == Constant.PARABOLIC) {
						return -electronAffinity + buffer;
					}
					else {
						if (ditType == Constant.GAUSSIAN) {
							double max = u1 + 4 * o1;
							if (max < u2 + 4 * o2) {
								max = u2 + 4 * o2;
							}
							if (max < -electronAffinity + buffer) {
								max = -electronAffinity + buffer;
							}
							return max;
						}
						else {
							return -electronAffinity + buffer;
						}
					}
				}
			}
		}

		public double ditMinX() {
			double buffer = 0.4;
			if (ditType == Constant.NONE) {
				return -electronAffinity - bandGap - buffer;
			}
			else {
				if (ditType == Constant.CONSTANT) {
					return -electronAffinity - bandGap - buffer;
				}
				else {
					if (ditType == Constant.PARABOLIC) {
						return -electronAffinity - bandGap - buffer;
					}
					else {
						if (ditType == Constant.GAUSSIAN) {
							double min = u1 - 4 * o1;
							if (min > u2 - 4 * o2) {
								min = u2 - 4 * o2;
							}
							if (min > -electronAffinity - bandGap - buffer) {
								min = -electronAffinity - bandGap - buffer;
							}
							return min;
						}
						else {
							return -electronAffinity - bandGap - buffer;
						}
					}
				}
			}
		}

		public double maxDit() {
			if (ditType == Constant.NONE) {
				return 0;
			}
			else {
				if (ditType == Constant.CONSTANT) {
					return nit / bandGap;
				}
				else {
					if (ditType == Constant.PARABOLIC) {
						return 12 * (-co * bandGap + nit) / Math.pow(bandGap, 3) *
							Math.pow(bandGap / 2, 2) + co;
					}
					else {
						if (ditType == Constant.GAUSSIAN) {
							return nit * per1 / (o1 * Constant.Sqrt2Pi) + nit * (1 - per1) / (o2 * Constant.Sqrt2Pi);
						}
						else {
							return 0;
						}
					}
				}
			}
		}

		*/
	}
}