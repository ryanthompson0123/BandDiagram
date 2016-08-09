using Band.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Band
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum DopingType
	{
		P,
		N
	}

    [JsonObject(MemberSerialization.OptIn)]
	public class Semiconductor : Material
	{
        #region Configuration

        [JsonProperty]
		public double DielectricConstant { get; private set; }

        [JsonProperty]
		public Energy BandGap { get; private set; }

        [JsonProperty]
		public Energy ElectronAffinity { get; private set; }

        [JsonProperty]
		public DopingType DopingType { get; private set; }

        [JsonProperty]
		public Concentration DopantConcentration { get; private set; }

        [JsonProperty]
		public Concentration IntrinsicCarrierConcentration { get; private set; }

        #endregion

        #region Derived Property Accessors

        private ChargeDensity extraChargeValue;
        public ChargeDensity ExtraCharge
        {
            get { return extraChargeValue; }
            set
            {
                extraChargeValue = value;

                // If the charge changes, these will need to be recalculated.
                lazyCapacitanceDensity = null;
                lazySurfacePotential = null;
            }
        }

        public ElectricPotential ThermalVoltage
        {
            get { return ParentStructure.ThermalVoltage; }
        }

		public override Energy EnergyFromVacuumToTopBand
		{
            get { return ElectronAffinity; }
		}

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

        private ElectricPotential lazyEnergyFromVacuumToEfi;
		public override Energy EnergyFromVacuumToEfi
		{
            get
            {
                if (lazyEnergyFromVacuumToEfi == null)
                {
                    lazyEnergyFromVacuumToEfi = ElectronAffinity + BandGap / 2;
                }

                return lazyEnergyFromVacuumToEfi;
            }
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

        private Energy lazyWorkFunction;
		public override Energy WorkFunction
		{
            get
            {
                if (lazyWorkFunction == null)
                {
                    lazyWorkFunction = EnergyFromVacuumToEfi + (Energy)PhiF;
                }

                return lazyWorkFunction;
            }
		}
                
        private ElectricPotential lazyPhiF;
		public ElectricPotential PhiF
		{
            get
            {
                if (lazyPhiF == null)
                {
                    var phiF = ThermalVoltage * Math.Log(
                        DopantConcentration / IntrinsicCarrierConcentration);
                    lazyPhiF = DopingType == DopingType.P ? phiF : -phiF;
                }

                return lazyPhiF;
            }
		}

        private double? lazySemiconductorConstant;
        public double SemiconductorConstant
        {
            get
            {
                if (lazySemiconductorConstant == null)
                {
                    lazySemiconductorConstant = Math.Sqrt(2 * ElectricCharge.Elementary.Coulombs
                        * Permittivity.OfFreeSpace.FaradsPerCentimeter * DielectricConstant
                        * DopantConcentration.PerCubicCentimeter);
                }

                return lazySemiconductorConstant.Value;
            }
        }

        private Concentration lazyFreeElectronConcentration;
        public Concentration FreeElectronConcentration
        {
            get
            {
                if (lazyFreeElectronConcentration == null)
                {
                    if (DopingType == DopingType.P)
                    {
                        lazyFreeElectronConcentration = IntrinsicCarrierConcentration
                            * (IntrinsicCarrierConcentration / DopantConcentration);
                    }
                    else
                    {
                        lazyFreeElectronConcentration = DopantConcentration;
                    }
                }

                return lazyFreeElectronConcentration;
            }
        }

        private Concentration lazyFreeHoleConcentration;
        public Concentration FreeHoleConcentration
        {
            get
            {
                if (lazyFreeHoleConcentration == null)
                {
                    if (DopingType == DopingType.N)
                    {
                        lazyFreeHoleConcentration = IntrinsicCarrierConcentration
                            * (IntrinsicCarrierConcentration / DopantConcentration);
                    }
                    else
                    {
                        lazyFreeHoleConcentration = DopantConcentration;
                    }
                }

                return lazyFreeHoleConcentration;
            }
        }

        private ElectricPotential lazySurfacePotential;
        public ElectricPotential SurfacePotential
        {
            get
            {
                if (lazySurfacePotential == null)
                {
                    var highPotential = BandGap.ToPotential() * 3;
                    var lowPotential = ElectricPotential.Zero - 2 * BandGap;
                    var guessPotential = (highPotential + lowPotential) / 2;
                    var guessCharge = GetChargeDensity(guessPotential);
                    var charge = ExtraCharge;

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

                    lazySurfacePotential = guessPotential;
                }

                return lazySurfacePotential;
            }
        }

        private CapacitanceDensity lazyCapacitanceDensity;
        public CapacitanceDensity CapacitanceDensity
        {
            get
            {
                if (lazyCapacitanceDensity == null)
                {
                    // capacitance value taken from Tsividis
                    // get the surface potential
                    var vT = ThermalVoltage;
                    var phiS = SurfacePotential;

                    if (DopingType == DopingType.P)
                    {
                        var numerator = 1 - Math.Exp(-phiS / vT)
                            + Math.Exp(-2 * PhiF / vT)
                            * (Math.Exp(phiS / vT) - 1);

                        var denominatorInside = vT * Math.Exp(-phiS / vT)
                            + phiS - vT
                            + Math.Exp(-2 * PhiF / vT)
                            * (vT * Math.Exp(phiS / vT) - phiS - vT);

                        var denominator = 2 * Math.Sqrt(denominatorInside.Volts);

                        var value = CapacitanceDensity.FromFaradsPerSquareCentimeter(
                            SemiconductorConstant * numerator / denominator);

                        lazyCapacitanceDensity = phiS > ElectricPotential.Zero ? value : -value;
                    }
                    else
                    {
                        var numerator = 1 - Math.Exp(phiS / vT) 
                            + Math.Exp(2 * PhiF / vT) 
                            * (Math.Exp(-phiS / vT) - 1);

                        var denominatorInside = vT * Math.Exp(phiS / vT)
                            - phiS - vT
                            + Math.Exp(2 * PhiF / vT)
                            * (ThermalVoltage * Math.Exp(-phiS / vT)
                                + phiS - ThermalVoltage);

                        var denominator = 2 * Math.Sqrt(denominatorInside.Volts);

                        var value = CapacitanceDensity.FromFaradsPerSquareCentimeter(
                            SemiconductorConstant * numerator / denominator);

                        lazyCapacitanceDensity =  phiS > ElectricPotential.Zero ? -value : value;
                    }
                }

                return lazyCapacitanceDensity;
            }
        }

        #endregion

        [JsonConstructor]
        internal Semiconductor()
        {
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);
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

        public override Material WithThickness(Length thickness)
        {
            return DeepClone(DopingType);
        }

        public Material DeepClone(DopingType dopingType)
        {
            var semiconductor = new Semiconductor
            {
                DielectricConstant = DielectricConstant,
                BandGap = BandGap,
                ElectronAffinity = ElectronAffinity,
                DopingType = dopingType,
                DopantConcentration = DopantConcentration,
                IntrinsicCarrierConcentration = IntrinsicCarrierConcentration
            };

            InitClone(semiconductor, Length.FromMicrometers(50));
            semiconductor.Prepare();

            return semiconductor;
        }

        private bool ShouldKeepTryingSurfacePotential(ChargeDensity charge,
            ChargeDensity guessCharge, int iterationNumber)
        {
            var maxDelta = ChargeDensity.FromCoulombsPerSquareCentimeter(Math.Abs(charge.CoulombsPerSquareCentimeter * 1e-6));
            return ((charge - maxDelta > guessCharge) || (guessCharge > charge + maxDelta))
                && iterationNumber < 1000;
        }
            
		public ChargeDensity GetChargeY(ElectricPotential potential)
        {
            var vT = ThermalVoltage;

            if (DopingType == DopingType.P)
            {
                var cConc = ElectricCharge.Elementary *
                    (FreeHoleConcentration * Math.Exp(-potential / vT)
                        - FreeElectronConcentration * Math.Exp(potential / vT)
                    - DopantConcentration);

                // Apparently this is ok, but I don't know why - RT
                return ChargeDensity.FromCoulombsPerSquareCentimeter(cConc.CoulombsPerCubicCentimeter);
            }
            else
            {
                var cConc = -ElectricCharge.Elementary *
                    (FreeElectronConcentration * Math.Exp(potential / vT)
                        - FreeHoleConcentration * Math.Exp(-potential / vT)
                    - DopantConcentration);

                // Apparently this is ok, but I don't know why - RT
                return ChargeDensity.FromCoulombsPerSquareCentimeter(cConc.CoulombsPerCubicCentimeter);
            }
        }

        public ChargeDensity GetChargeDensity(ElectricPotential surfacePotential)
        {
            var vT = ThermalVoltage;
            ElectricPotential innerTerm;
			if (DopingType == DopingType.P)
			{
                innerTerm = vT * Math.Exp(-surfacePotential / vT)
                    + surfacePotential - vT + Math.Exp(-2 * PhiF / vT)
                    * (vT * Math.Exp(surfacePotential / vT) - surfacePotential - vT);
			}
			else
			{
                innerTerm = vT * Math.Exp(surfacePotential / vT)
                    - surfacePotential - vT + Math.Exp(2 * PhiF / vT)
                    * (vT * Math.Exp(-surfacePotential / vT) - surfacePotential - vT);
			}

            var charge = ChargeDensity.FromCoulombsPerSquareCentimeter(SemiconductorConstant * Math.Sqrt(innerTerm.Volts));

            return surfacePotential >= ElectricPotential.Zero ? -charge : charge;
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

        public void Evaluate()
        {
            var phiS = SurfacePotential;
            var storePotential = EvalPoints[0].Potential;

            // First remove all the points
            EvalPoints.Clear();

            // Find the thickness through integration
            // Integrate from 0 to the surface potential
            // Integrate 2000 times so change stepSize depending on the surface potential
            var stepSize = phiS / 20;

            stepSize = phiS > ElectricPotential.Zero ?
                ElectricPotential.Abs(stepSize) : -ElectricPotential.Abs(stepSize);

            var previousValue = 1 / GetElectricField(phiS).VoltsPerMeter;
            var runningThickness = Length.Zero;
            var previousThickness = Length.Zero;

            var point = new EvalPoint
            {
                Location = runningThickness,
                ChargeDensity = GetChargeY(phiS),
                ElectricField = new ElectricField(1 / previousValue),
                Potential = phiS
            };

            EvalPoints.Add(point);

            for (var i = 1; ElectricPotential.Abs(phiS - stepSize * i)
                > ElectricPotential.FromMillivolts(1) && i < 10000; i++)
            {
                var potentialValue = phiS - stepSize * i;
                var value = 1 / GetElectricField(potentialValue).VoltsPerMeter;
                previousThickness = runningThickness;
                runningThickness += new Length(((previousValue + value) / 2) * stepSize.Volts);

                if (double.IsNaN(runningThickness.Meters))
                {
                    Debug.WriteLine("Por que Nan??");
                }

                point = new EvalPoint
                {
                    Location = runningThickness,
                    ChargeDensity = GetChargeY(potentialValue) * 1E-8,
                    ElectricField = new ElectricField(1 / value),
                    Potential = potentialValue
                };

                EvalPoints.Add(point);
                previousValue = value;
            }

            // Now add the offset in potential
            var checkCharge = ChargeDensity.Zero; // check and see if the charges add up
            foreach (var checkPoint in EvalPoints)
            {
                checkPoint.Potential = checkPoint.Potential + storePotential;
                checkPoint.Potential = checkPoint.Potential - phiS;
                checkCharge += checkPoint.ChargeDensity;
            }
        }

        public override PlotDataSet GetChargeDensityDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                PlotColor = FillColor,
                LineThickness = 2
            };

            for (var i = 1; i < EvalPoints.Count; i++)
            {
                var location = EvalPoints[i].Location + offset;
                var charge = EvalPoints[i].ChargeDensity;

                if (i == 0)
                {
                    dataset.DataPoints.Add(new PlotDataPoint
                    {
                        X = location.Nanometers, 
                        Y = 0.0
                    });
                }

                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = charge.MicroCoulombsPerSquareCentimeter
                });
            }

            return dataset;
        }

        public override List<PlotDataSet> GetEnergyDatasets(Length offset)
        {
            var cbDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Conduction Band", Name),
                LineThickness = 2,
                PlotColor = FillColor
            };

            var vbDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Valance Band", Name),
                LineThickness = 2,
                PlotColor = FillColor
            };

            var efiDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Fermi Level", Name),
                LineThickness = 1,
                PlotColor = FillColor
            };

            var wfDataset = new PlotDataSet
            {
                Name = String.Format("{0} - Work Function", Name),
                LineThickness = 1,
                PlotColor = Color.Black
            };

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                var cbEnergy = -EnergyFromVacuumToTopBand - point.Potential;
                var vbEnergy = -EnergyFromVacuumToBottomBand - point.Potential;
                var efiEnergy = -EnergyFromVacuumToEfi - point.Potential;
                var wfEnergy = -WorkFunction;

                if (double.IsNaN(location.Meters))
                {
                    Debug.WriteLine("Why is this NaN");
                }

                cbDataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = cbEnergy.ElectronVolts
                });
                vbDataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = vbEnergy.ElectronVolts
                });
                efiDataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = efiEnergy.ElectronVolts
                });
                wfDataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, Y = wfEnergy.ElectronVolts
                });
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