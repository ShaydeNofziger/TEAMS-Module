﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Greet.DataStructureV3.Interfaces;
using Greet.Model.Interfaces;
using TEAMSModule;

namespace TEAMS_Plugin
{
    public partial class TEAMS : Form
    {
        // Constants
        #region Constants

        private const double KWperHP = 0.745699872;
        private const double GALperBBL = 42.0;
        private const double BBLperTONNE = 7.45;
        private const double BTUperKWH = 3412.14163;

        // Joules per 1 mmBtu
        private const double JOULES_PER_MMBTU = 1055870000.0;
        // Joules per 1 Btu
        private const double JOULES_PER_BTU = 1055.05585;
        // Gallons per 1 m^3
        private const double GALLONS_PER_CUBIC_METER = 264.172;
        // Btu's per 1 mmBtu
        private const double BTUS_PER_MMBTU = 1000000.00;

        // Conventional Diesel Path ID - Used for estimation of fuel gallons for input, does not appear on the results sheet.
        private const int CD_PATH_ID = 40;

        #endregion

        // API Controller
        private APIcalls APIcontroller = new APIcalls();

        //Used to do the calculations for the model
        #region Input Variables
        #region Main Engine

        // Main Engine Variables
        public string VesselTypeID;
        public int NumberOfEngines;
        public int SingleEngineHP;
        public int TotalOnboardHP;

        // Trip Distance and Time
        public double TotalTripDistanceInMiles;
        public double TripTimeHours;
        public double TripTimeMinutes;
        public double TotalTripTimeHours;

        //Engine Charicterization Per Mode
        // POT  -   Percent Of Trip, Time in mode is measured in hours
        // HPLF -   Horse Power Load Factor (Single engine)
        // HPPE -   Horse Power Per Engine
        // EP   -   Energy Production in KWH for all engines
        public double POTIdle;
        public double POTManeuvering;
        public double POTPrecautionary;
        public double POTSlowCruise;
        public double POTFullCruise;
        public double TimeInIdle;
        public double TimeInManeuvering;
        public double TimeInPrecautionary;
        public double TimeInSlowCruise;
        public double TimeInFullCruise;
        public double HPLFIdle;
        public double HPLFManeuvering;
        public double HPLFPrecautionary;
        public double HPLFSlowCruise;
        public double HPLFFullCruise;
        public double HPPEIdle;
        public double HPPEManeuvering;
        public double HPPEPrecautionary;
        public double HPPESlowCruise;
        public double HPPEFullCruise;
        public double EPIdle;
        public double EPManeuvering;
        public double EPPrecautionary;
        public double EPSlowCruise;
        public double EPFullCruise;
        public double EPTotal;

        // Calculation of Fuel Use Using Conventional Diesel as Baseline Fuel
        public double EngineEfficiency;
        public double KWHOutperTrip;
        public double MMBTUoutperTrip;
        public double MMBTUinperTrip;
        public double GALLONperTrip;

        // Emissions Factor Variables -- grams per horsepower hour
        public double Res_NOX_gphphr_out;
        public double Res_CO_gphphr_out;
        public double Res_VOC_gphphr_out;
        public double Res_PM10_gphphr_out;
        public double Res_PM25_gphphr_out;
        public double Res_N2O_gphphr_out;
        public double Res_CH4_gphphr_out;

        public double Die_NOX_gphphr_out;
        public double Die_CO_gphphr_out;
        public double Die_VOC_gphphr_out;
        public double Die_PM10_gphphr_out;
        public double Die_PM25_gphphr_out;
        public double Die_N2O_gphphr_out;
        public double Die_CH4_gphphr_out;

        public double Nat_NOX_gphphr_out;
        public double Nat_CO_gphphr_out;
        public double Nat_VOC_gphphr_out;
        public double Nat_PM10_gphphr_out;
        public double Nat_PM25_gphphr_out;
        public double Nat_N2O_gphphr_out;
        public double Nat_CH4_gphphr_out;

        public double Fis_NOX_gphphr_out;
        public double Fis_CO_gphphr_out;
        public double Fis_VOC_gphphr_out;
        public double Fis_PM10_gphphr_out;
        public double Fis_PM25_gphphr_out;
        public double Fis_N2O_gphphr_out;
        public double Fis_CH4_gphphr_out;

        public double Bio_NOX_gphphr_out;
        public double Bio_CO_gphphr_out;
        public double Bio_VOC_gphphr_out;
        public double Bio_PM10_gphphr_out;
        public double Bio_PM25_gphphr_out;
        public double Bio_N2O_gphphr_out;
        public double Bio_CH4_gphphr_out;

        public double Ult_NOX_gphphr_out;
        public double Ult_CO_gphphr_out;
        public double Ult_VOC_gphphr_out;
        public double Ult_PM10_gphphr_out;
        public double Ult_PM25_gphphr_out;
        public double Ult_N2O_gphphr_out;
        public double Ult_CH4_gphphr_out;

        public double[] Residual_Oil;
        public double[] Diesel;
        public double[] Natural_Gas;
        public double[] Fischer;
        public double[] Biodiesel;
        public double[] Ult_Low_Sulf;

        // Global Warming Potential
        public double CO2_GWP;
        public double CH4_GWP;
        public double N2O_GWP;
        public double VOC_GWP;
        public double CO_GWP;
        public double NO2_GWP;
        #endregion

        #region Auxiliary Engine(s)

        // Auxiliary Engine Variables
        public int NumberOfOnBoardAuxiliaryEngines;
        public int NumberOfAuxiliaryEnginesInUse;
        public int AuxiliaryEnginesRatedHPperEngine;
        public int TotalOnboardAUxHP;

        // Auxiliary Engine Characterization (Conventional Diesel as Baseline Fuel)
        public double PercentOfTripAuxiliaryIsActive;
        public double TimeAuxActiveHours;
        public double HPLoadFactorSingleEngine;
        public double ActiveHPPerAuxEngine;
        public double TotalAuxEnergyProduction;


        // Calculation of Auxiliary Engine Fuel use Using Conventional Diesel as Baseline Fuel
        public double AuxiliaryEngineEfficiency;
        public double AuxEngineKWHoutperTrip;
        public double AuxEngineMMBTUoutperTrip;
        public double AuxEngineMMBTUinperTrip;
        public double AuxEngineGALLONperTrip;

        public double Aux_Res_NOX_gphphr_out;
        public double Aux_Res_CO_gphphr_out;
        public double Aux_Res_VOC_gphphr_out;
        public double Aux_Res_PM10_gphphr_out;
        public double Aux_Res_PM25_gphphr_out;
        public double Aux_Res_N2O_gphphr_out;
        public double Aux_Res_CH4_gphphr_out;

        public double Aux_Die_NOX_gphphr_out;
        public double Aux_Die_CO_gphphr_out;
        public double Aux_Die_VOC_gphphr_out;
        public double Aux_Die_PM10_gphphr_out;
        public double Aux_Die_PM25_gphphr_out;
        public double Aux_Die_N2O_gphphr_out;
        public double Aux_Die_CH4_gphphr_out;

        public double Aux_Nat_NOX_gphphr_out;
        public double Aux_Nat_CO_gphphr_out;
        public double Aux_Nat_VOC_gphphr_out;
        public double Aux_Nat_PM10_gphphr_out;
        public double Aux_Nat_PM25_gphphr_out;
        public double Aux_Nat_N2O_gphphr_out;
        public double Aux_Nat_CH4_gphphr_out;

        public double Aux_Fis_NOX_gphphr_out;
        public double Aux_Fis_CO_gphphr_out;
        public double Aux_Fis_VOC_gphphr_out;
        public double Aux_Fis_PM10_gphphr_out;
        public double Aux_Fis_PM25_gphphr_out;
        public double Aux_Fis_N2O_gphphr_out;
        public double Aux_Fis_CH4_gphphr_out;

        public double Aux_Bio_NOX_gphphr_out;
        public double Aux_Bio_CO_gphphr_out;
        public double Aux_Bio_VOC_gphphr_out;
        public double Aux_Bio_PM10_gphphr_out;
        public double Aux_Bio_PM25_gphphr_out;
        public double Aux_Bio_N2O_gphphr_out;
        public double Aux_Bio_CH4_gphphr_out;

        public double Aux_Ult_NOX_gphphr_out;
        public double Aux_Ult_CO_gphphr_out;
        public double Aux_Ult_VOC_gphphr_out;
        public double Aux_Ult_PM10_gphphr_out;
        public double Aux_Ult_PM25_gphphr_out;
        public double Aux_Ult_N2O_gphphr_out;
        public double Aux_Ult_CH4_gphphr_out;

        public double[] Aux_Residual_Oil;
        public double[] Aux_Diesel;
        public double[] Aux_Natural_Gas;
        public double[] Aux_Fischer;
        public double[] Aux_Biodiesel;
        public double[] Aux_Ult_Low_Sulf;

        #endregion
        #endregion


        // Results variables - These are used to store calculated values later on, as well as the actual results that come from another round of calculations
        #region Results Variables
        public double CD_WTP_TE;
        public double CD_WTP_FF;
        public double CD_WTP_CF;
        public double CD_WTP_NG;
        public double CD_WTP_PF;
        public double CD_WTP_VOC;
        public double CD_WTP_CO;
        public double CD_WTP_NOX;
        public double CD_WTP_PM10;
        public double CD_WTP_PM25;
        public double CD_WTP_SOX;
        public double CD_WTP_CH4;
        public double CD_WTP_CO2;
        public double CD_WTP_N2O;
        public double CD_WTP_PM10_TBW;
        public double CD_WTP_PM25_TBW;
        public double CD_WTP_PM25_CO2Biogenic;

        public double CD_VO_TE;
        public double CD_VO_FF;
        public double CD_VO_CF;
        public double CD_VO_NG;
        public double CD_VO_PF;
        public double CD_VO_VOC;
        public double CD_VO_CO;
        public double CD_VO_NOX;
        public double CD_VO_PM10;
        public double CD_VO_PM25;
        public double CD_VO_SOX;
        public double CD_VO_CH4;
        public double CD_VO_CO2;
        public double CD_VO_N2O;
        public double CD_VO_PM10_TBW;
        public double CD_VO_PM25_TBW;
        public double CD_VO_PM25_CO2Biogenic;

        public double CD_Total_TE;
        public double CD_Total_FF;
        public double CD_Total_CF;
        public double CD_Total_NG;
        public double CD_Total_PF;
        public double CD_Total_VOC;
        public double CD_Total_CO;
        public double CD_Total_NOX;
        public double CD_Total_PM10;
        public double CD_Total_PM25;
        public double CD_Total_SOX;
        public double CD_Total_CH4;
        public double CD_Total_CO2;
        public double CD_Total_N2O;
        public double CD_Total_PM10_TBW;
        public double CD_Total_PM25_TBW;
        public double CD_Total_PM25_CO2Biogenic;

        public double RO_WTP_TE;
        public double RO_WTP_FF;
        public double RO_WTP_CF;
        public double RO_WTP_NG;
        public double RO_WTP_PF;
        public double RO_WTP_VOC;
        public double RO_WTP_CO;
        public double RO_WTP_NOX;
        public double RO_WTP_PM10;
        public double RO_WTP_PM25;
        public double RO_WTP_SOX;
        public double RO_WTP_CH4;
        public double RO_WTP_CO2;
        public double RO_WTP_N2O;
        public double RO_WTP_PM10_TBW;
        public double RO_WTP_PM25_TBW;
        public double RO_WTP_PM25_CO2Biogenic;

        public double RO_VO_TE;
        public double RO_VO_FF;
        public double RO_VO_CF;
        public double RO_VO_NG;
        public double RO_VO_PF;
        public double RO_VO_VOC;
        public double RO_VO_CO;
        public double RO_VO_NOX;
        public double RO_VO_PM10;
        public double RO_VO_PM25;
        public double RO_VO_SOX;
        public double RO_VO_CH4;
        public double RO_VO_CO2;
        public double RO_VO_N2O;
        public double RO_VO_PM10_TBW;
        public double RO_VO_PM25_TBW;
        public double RO_VO_PM25_CO2Biogenic;

        public double RO_Total_TE;
        public double RO_Total_FF;
        public double RO_Total_CF;
        public double RO_Total_NG;
        public double RO_Total_PF;
        public double RO_Total_VOC;
        public double RO_Total_CO;
        public double RO_Total_NOX;
        public double RO_Total_PM10;
        public double RO_Total_PM25;
        public double RO_Total_SOX;
        public double RO_Total_CH4;
        public double RO_Total_CO2;
        public double RO_Total_N2O;
        public double RO_Total_PM10_TBW;
        public double RO_Total_PM25_TBW;
        public double RO_Total_PM25_CO2Biogenic;

        public double LSD_WTP_TE;
        public double LSD_WTP_FF;
        public double LSD_WTP_CF;
        public double LSD_WTP_NG;
        public double LSD_WTP_PF;
        public double LSD_WTP_VOC;
        public double LSD_WTP_CO;
        public double LSD_WTP_NOX;
        public double LSD_WTP_PM10;
        public double LSD_WTP_PM25;
        public double LSD_WTP_SOX;
        public double LSD_WTP_CH4;
        public double LSD_WTP_CO2;
        public double LSD_WTP_N2O;
        public double LSD_WTP_PM10_TBW;
        public double LSD_WTP_PM25_TBW;
        public double LSD_WTP_PM25_CO2Biogenic;

        public double LSD_VO_TE;
        public double LSD_VO_FF;
        public double LSD_VO_CF;
        public double LSD_VO_NG;
        public double LSD_VO_PF;
        public double LSD_VO_VOC;
        public double LSD_VO_CO;
        public double LSD_VO_NOX;
        public double LSD_VO_PM10;
        public double LSD_VO_PM25;
        public double LSD_VO_SOX;
        public double LSD_VO_CH4;
        public double LSD_VO_CO2;
        public double LSD_VO_N2O;
        public double LSD_VO_PM10_TBW;
        public double LSD_VO_PM25_TBW;
        public double LSD_VO_PM25_CO2Biogenic;

        public double LSD_Total_TE;
        public double LSD_Total_FF;
        public double LSD_Total_CF;
        public double LSD_Total_NG;
        public double LSD_Total_PF;
        public double LSD_Total_VOC;
        public double LSD_Total_CO;
        public double LSD_Total_NOX;
        public double LSD_Total_PM10;
        public double LSD_Total_PM25;
        public double LSD_Total_SOX;
        public double LSD_Total_CH4;
        public double LSD_Total_CO2;
        public double LSD_Total_N2O;
        public double LSD_Total_PM10_TBW;
        public double LSD_Total_PM25_TBW;
        public double LSD_Total_PM25_CO2Biogenic;

        public double NG_WTP_TE;
        public double NG_WTP_FF;
        public double NG_WTP_CF;
        public double NG_WTP_NG;
        public double NG_WTP_PF;
        public double NG_WTP_VOC;
        public double NG_WTP_CO;
        public double NG_WTP_NOX;
        public double NG_WTP_PM10;
        public double NG_WTP_PM25;
        public double NG_WTP_SOX;
        public double NG_WTP_CH4;
        public double NG_WTP_CO2;
        public double NG_WTP_N2O;
        public double NG_WTP_PM10_TBW;
        public double NG_WTP_PM25_TBW;
        public double NG_WTP_PM25_CO2Biogenic;

        public double NG_VO_TE;
        public double NG_VO_FF;
        public double NG_VO_CF;
        public double NG_VO_NG;
        public double NG_VO_PF;
        public double NG_VO_VOC;
        public double NG_VO_CO;
        public double NG_VO_NOX;
        public double NG_VO_PM10;
        public double NG_VO_PM25;
        public double NG_VO_SOX;
        public double NG_VO_CH4;
        public double NG_VO_CO2;
        public double NG_VO_N2O;
        public double NG_VO_PM10_TBW;
        public double NG_VO_PM25_TBW;
        public double NG_VO_PM25_CO2Biogenic;

        public double NG_Total_TE;
        public double NG_Total_FF;
        public double NG_Total_CF;
        public double NG_Total_NG;
        public double NG_Total_PF;
        public double NG_Total_VOC;
        public double NG_Total_CO;
        public double NG_Total_NOX;
        public double NG_Total_PM10;
        public double NG_Total_PM25;
        public double NG_Total_SOX;
        public double NG_Total_CH4;
        public double NG_Total_CO2;
        public double NG_Total_N2O;
        public double NG_Total_PM10_TBW;
        public double NG_Total_PM25_TBW;
        public double NG_Total_PM25_CO2Biogenic;

        public double BD_WTP_TE;
        public double BD_WTP_FF;
        public double BD_WTP_CF;
        public double BD_WTP_NG;
        public double BD_WTP_PF;
        public double BD_WTP_VOC;
        public double BD_WTP_CO;
        public double BD_WTP_NOX;
        public double BD_WTP_PM10;
        public double BD_WTP_PM25;
        public double BD_WTP_SOX;
        public double BD_WTP_CH4;
        public double BD_WTP_CO2;
        public double BD_WTP_N2O;
        public double BD_WTP_PM10_TBW;
        public double BD_WTP_PM25_TBW;
        public double BD_WTP_PM25_CO2Biogenic;

        public double BD_VO_TE;
        public double BD_VO_FF;
        public double BD_VO_CF;
        public double BD_VO_NG;
        public double BD_VO_PF;
        public double BD_VO_VOC;
        public double BD_VO_CO;
        public double BD_VO_NOX;
        public double BD_VO_PM10;
        public double BD_VO_PM25;
        public double BD_VO_SOX;
        public double BD_VO_CH4;
        public double BD_VO_CO2;
        public double BD_VO_N2O;
        public double BD_VO_PM10_TBW;
        public double BD_VO_PM25_TBW;
        public double BD_VO_PM25_CO2Biogenic;

        public double BD_Total_TE;
        public double BD_Total_FF;
        public double BD_Total_CF;
        public double BD_Total_NG;
        public double BD_Total_PF;
        public double BD_Total_VOC;
        public double BD_Total_CO;
        public double BD_Total_NOX;
        public double BD_Total_PM10;
        public double BD_Total_PM25;
        public double BD_Total_SOX;
        public double BD_Total_CH4;
        public double BD_Total_CO2;
        public double BD_Total_N2O;
        public double BD_Total_PM10_TBW;
        public double BD_Total_PM25_TBW;
        public double BD_Total_PM25_CO2Biogenic;

        public double FTD_WTP_TE;
        public double FTD_WTP_FF;
        public double FTD_WTP_CF;
        public double FTD_WTP_NG;
        public double FTD_WTP_PF;
        public double FTD_WTP_VOC;
        public double FTD_WTP_CO;
        public double FTD_WTP_NOX;
        public double FTD_WTP_PM10;
        public double FTD_WTP_PM25;
        public double FTD_WTP_SOX;
        public double FTD_WTP_CH4;
        public double FTD_WTP_CO2;
        public double FTD_WTP_N2O;
        public double FTD_WTP_PM10_TBW;
        public double FTD_WTP_PM25_TBW;
        public double FTD_WTP_PM25_CO2Biogenic;

        public double FTD_VO_TE;
        public double FTD_VO_FF;
        public double FTD_VO_CF;
        public double FTD_VO_NG;
        public double FTD_VO_PF;
        public double FTD_VO_VOC;
        public double FTD_VO_CO;
        public double FTD_VO_NOX;
        public double FTD_VO_PM10;
        public double FTD_VO_PM25;
        public double FTD_VO_SOX;
        public double FTD_VO_CH4;
        public double FTD_VO_CO2;
        public double FTD_VO_N2O;
        public double FTD_VO_PM10_TBW;
        public double FTD_VO_PM25_TBW;
        public double FTD_VO_PM25_CO2Biogenic;

        public double FTD_Total_TE;
        public double FTD_Total_FF;
        public double FTD_Total_CF;
        public double FTD_Total_NG;
        public double FTD_Total_PF;
        public double FTD_Total_VOC;
        public double FTD_Total_CO;
        public double FTD_Total_NOX;
        public double FTD_Total_PM10;
        public double FTD_Total_PM25;
        public double FTD_Total_SOX;
        public double FTD_Total_CH4;
        public double FTD_Total_CO2;
        public double FTD_Total_N2O;
        public double FTD_Total_PM10_TBW;
        public double FTD_Total_PM25_TBW;
        public double FTD_Total_PM25_CO2Biogenic;

        public double conventionalDieselBTUperGal;

        #endregion

        // The results sheet
        private GREETFormattedResults results_sheet;

        //Array of the panels
        Panel[] allPanels = new Panel[11];
        Label[] allPanelLabels = new Label[11];
        public TEAMS()
        {
            InitializeComponent();
            setPanelAndLabelArrays();
            useDefaults();
            changeResults();
            //initializePanelsAndLabels();
        }
        public void initializePanelsAndLabels()
        {
            for (int i = 0; i < allPanelLabels.Length; i++)
            {
                //labelClicked(allPanelLabels.ElementAt(i), allPanels.ElementAt(i));
                labelClicked(allPanelLabels.ElementAt(i),allPanels.ElementAt(i));
            }
        }
        private void setPanelAndLabelArrays()
        {
            allPanels[0] = panel_Main_Engine_Vars;
            allPanels[1] = panel_Main_Trip_Distance_Time;
            allPanels[2] = panel_Main_Engine_Chars;
            allPanels[3] = panel_Main_Fuel_Calculations;
            allPanels[4] = panel_Main_GWPs;
            allPanels[5] = panel_main_engine_inputs;
            allPanels[6] = panel_Aux_Engine_Vars;
            allPanels[7] = panel_Aux_Engine_Chars;
            allPanels[8] = panel_Aux_Fuel_Calc;
            allPanels[9] = panel_Aux_Emissions_Input;
            allPanels[10] = Panel_FuelSelection;

            allPanelLabels[0] = label_Main_Engine_Vars;
            allPanelLabels[1] = label_Main_Total_Distance_And_Time;
            allPanelLabels[2] = label_Main_Engine_Char_Per_Mode;
            allPanelLabels[3] = label_Main_Fuel_Calculations;
            allPanelLabels[4] = label_Main_Global_Warming_Potentials;
            allPanelLabels[5] = label_Main_Engine_Inputs;
            allPanelLabels[6] = label_Aux_Engine_Vars;
            allPanelLabels[7] = label_Aux_Engine_Chars;
            allPanelLabels[8] = label_Aux_Fuel_Calc;
            allPanelLabels[9] = label_Aux_Emissions_Calc_Inputs;
            allPanelLabels[10] = Label_FuelSelection;
        }

        /// <summary>
        /// Calculated values based on the current values of variables in the model
        /// </summary>
        #region Do Calculations
        private void doCalculations()
        {
            // Total Horsepower
            TotalOnboardHP = SingleEngineHP * NumberOfEngines;
            // Total Trip Time
            TotalTripTimeHours = TripTimeHours + (TripTimeMinutes / 60);

            // Time in Stage of Transit
            TimeInIdle = (POTIdle / 100) * TotalTripTimeHours;
            TimeInManeuvering = (POTManeuvering / 100) * TotalTripTimeHours;
            TimeInPrecautionary = (POTPrecautionary / 100) * TotalTripTimeHours;
            TimeInSlowCruise = (POTSlowCruise / 100) * TotalTripTimeHours;
            TimeInFullCruise = (POTFullCruise / 100) * TotalTripTimeHours;

            // Horsepower Per Engine in a given mode
            HPPEIdle = (HPLFIdle / 100) * SingleEngineHP;
            HPPEManeuvering = (HPLFManeuvering / 100) * SingleEngineHP;
            HPPEPrecautionary = (HPLFPrecautionary / 100) * SingleEngineHP;
            HPPESlowCruise = (HPLFSlowCruise / 100) * SingleEngineHP;
            HPPEFullCruise = (HPLFFullCruise / 100) * SingleEngineHP;

            // Energy Production (in KWH for all engines) in a given mode
            EPIdle = NumberOfEngines * HPPEIdle * TimeInIdle * KWperHP;
            EPManeuvering = NumberOfEngines * HPPEManeuvering * TimeInManeuvering * KWperHP;
            EPPrecautionary = NumberOfEngines * HPPEPrecautionary * TimeInPrecautionary * KWperHP;
            EPSlowCruise = NumberOfEngines * HPPESlowCruise * TimeInSlowCruise * KWperHP;
            EPFullCruise = NumberOfEngines * HPPEFullCruise * TimeInFullCruise * KWperHP;

            // Energy Production Total
            EPTotal = EPIdle + EPManeuvering + EPPrecautionary + EPSlowCruise + EPFullCruise;

            // Kilowat Hours out Per Trip
            KWHOutperTrip = EPTotal;

            // Million BTUs of Energy needed to power the trip
            MMBTUoutperTrip = (KWHOutperTrip * BTUperKWH) / BTUS_PER_MMBTU;

            // Million BTUs of Energy to put into the system to get the MMBTU's required as output
            MMBTUinperTrip = MMBTUoutperTrip * (100 / EngineEfficiency);

            // Gallons of fuel per trip if it were powered by conventional diesel
            GALLONperTrip = (1 / conventionalDieselBTUperGal) * BTUS_PER_MMBTU * MMBTUinperTrip;

            // Total Horsepower of Auxiliary engines
            TotalOnboardAUxHP = NumberOfAuxiliaryEnginesInUse * AuxiliaryEnginesRatedHPperEngine;

            // Time auxiliary engines are active in hours
            TimeAuxActiveHours = (PercentOfTripAuxiliaryIsActive / 100) * TotalTripTimeHours;

            // Active Horsepower per Auxiliary engine
            ActiveHPPerAuxEngine = (HPLoadFactorSingleEngine / 100) * AuxiliaryEnginesRatedHPperEngine;

            // Total auxiliary engine production
            TotalAuxEnergyProduction = NumberOfAuxiliaryEnginesInUse * ActiveHPPerAuxEngine * TimeAuxActiveHours * KWperHP;

            // Aux Engine Kilowat Hours out per trip
            AuxEngineKWHoutperTrip = TotalAuxEnergyProduction;

            // Auxiliary Engine million BTUs of energy to power the trip
            AuxEngineMMBTUoutperTrip = (AuxEngineKWHoutperTrip * BTUperKWH) / BTUS_PER_MMBTU;

            // Auxiliary Engine million BTUs of energy to put in to the engine in order to get the needed energy out
            AuxEngineMMBTUinperTrip = AuxEngineMMBTUoutperTrip * (100 / AuxiliaryEngineEfficiency);

            // Auxiliary Engine gallons of fuel per trip if it were using conventional diesel
            AuxEngineGALLONperTrip = (1 / conventionalDieselBTUperGal) * BTUS_PER_MMBTU * AuxEngineMMBTUinperTrip;

        }
        #endregion

        /// <summary>
        /// Changes labels based on updated values from calculation/manual changes
        /// </summary>
        #region Change Results
        private void changeResults()
        {
            // 5 
            textBox_Vessel_Type.Text = (string)VesselTypeID;

            Res_NOX.Value = (decimal)Res_NOX_gphphr_out;
            Res_CO.Value = (decimal)Res_CO_gphphr_out;
            Res_VOC.Value = (decimal)Res_VOC_gphphr_out;
            Res_PM10.Value = (decimal)Res_PM10_gphphr_out;
            Res_PM25.Value = (decimal)Res_PM25_gphphr_out;
            Res_N2O.Value = (decimal)Res_N2O_gphphr_out;
            Res_CH4.Value = (decimal)Res_CH4_gphphr_out;

            Die_NOX.Value = (decimal)Die_NOX_gphphr_out;
            Die_CO.Value = (decimal)Die_CO_gphphr_out;
            Die_VOC.Value = (decimal)Die_VOC_gphphr_out;
            Die_PM10.Value = (decimal)Die_PM10_gphphr_out;
            Die_PM25.Value = (decimal)Die_PM25_gphphr_out;
            Die_N2O.Value = (decimal)Die_N2O_gphphr_out;
            Die_CH4.Value = (decimal)Die_CH4_gphphr_out;

            Nat_NOX.Value = (decimal)Nat_NOX_gphphr_out;
            Nat_CO.Value = (decimal)Nat_CO_gphphr_out;
            Nat_VOC.Value = (decimal)Nat_VOC_gphphr_out;
            Nat_PM10.Value = (decimal)Nat_PM10_gphphr_out;
            Nat_PM25.Value = (decimal)Nat_PM25_gphphr_out;
            Nat_N2O.Value = (decimal)Nat_N2O_gphphr_out;
            Nat_CH4.Value = (decimal)Nat_CH4_gphphr_out;

            Fis_NOX.Value = (decimal)Fis_NOX_gphphr_out;
            Fis_CO.Value = (decimal)Fis_CO_gphphr_out;
            Fis_VOC.Value = (decimal)Fis_VOC_gphphr_out;
            Fis_PM10.Value = (decimal)Fis_PM10_gphphr_out;
            Fis_PM25.Value = (decimal)Fis_PM25_gphphr_out;
            Fis_N2O.Value = (decimal)Fis_N2O_gphphr_out;
            Fis_CH4.Value = (decimal)Fis_CH4_gphphr_out;

            Bio_NOX.Value = (decimal)Bio_NOX_gphphr_out;
            Bio_CO.Value = (decimal)Bio_CO_gphphr_out;
            Bio_VOC.Value = (decimal)Bio_VOC_gphphr_out;
            Bio_PM10.Value = (decimal)Bio_PM10_gphphr_out;
            Bio_PM25.Value = (decimal)Bio_PM25_gphphr_out;
            Bio_N2O.Value = (decimal)Bio_N2O_gphphr_out;
            Bio_CH4.Value = (decimal)Bio_CH4_gphphr_out;

            Ult_NOX.Value = (decimal)Ult_NOX_gphphr_out;
            Ult_CO.Value = (decimal)Ult_CO_gphphr_out;
            Ult_VOC.Value = (decimal)Ult_VOC_gphphr_out;
            Ult_PM10.Value = (decimal)Ult_PM10_gphphr_out;
            Ult_PM25.Value = (decimal)Ult_PM25_gphphr_out;
            Ult_N2O.Value = (decimal)Ult_N2O_gphphr_out;
            Ult_CH4.Value = (decimal)Ult_CH4_gphphr_out;

            updown_Number_Of_Engines.Value = (decimal)NumberOfEngines;
            updown_Single_Engine_HP.Value = (decimal)SingleEngineHP;
            updown_Total_Trip_Distance.Value = (decimal)TotalTripDistanceInMiles;
            updown_Trip_Time_Hours.Value = (decimal)TripTimeHours;
            updown_Trip_Time_Minutes.Value = (decimal)TripTimeMinutes;
            updown_Percent_Idle.Value = (decimal)POTIdle;
            updown_Percent_Maneuvering.Value = (decimal)POTManeuvering;
            updown_Percent_Precautionary.Value = (decimal)POTPrecautionary;
            updown_Percent_Slow_Cruise.Value = (decimal)POTSlowCruise;
            updown_Percent_Full_Cruise.Value = (decimal)POTFullCruise;
            updown_Horsepower_Idle.Value = (decimal)HPLFIdle;
            updown_Horsepower_Maneuvering.Value = (decimal)HPLFManeuvering;
            updown_Horsepower_Precautionary.Value = (decimal)HPLFPrecautionary;
            updown_Horsepower_Slow_Cruise.Value = (decimal)HPLFSlowCruise;
            updown_Horsepower_Full_Cruise.Value = (decimal)HPLFFullCruise;
            updown_Engine_Efficiency.Value = (decimal)EngineEfficiency;

            updown_Aux_Number_Engines.Value = (decimal)NumberOfOnBoardAuxiliaryEngines;
            updown_Aux_Engines_In_Use.Value = (decimal)NumberOfAuxiliaryEnginesInUse;
            updown_Aux_HP_Per_Engine.Value = (decimal)AuxiliaryEnginesRatedHPperEngine;
            updown_Aux_Percent_Trip_Active.Value = (decimal)PercentOfTripAuxiliaryIsActive;
            updown_Aux_HP_Load_Factor.Value = (decimal)HPLoadFactorSingleEngine;
            updown_Aux_Engine_Efficiency.Value = (decimal)AuxiliaryEngineEfficiency;

            doCalculations();
            // All Results That Needed Calculation
            updown_Tot_Onboard_HP.Value = (decimal)TotalOnboardHP;
            updown_Tot_Trip_Hours.Value = (decimal)TotalTripTimeHours;
            updown_Time_Idle.Value = (decimal)TimeInIdle;
            updown_Time_Maneuvering.Value = (decimal)TimeInManeuvering;
            updown_Time_Precautionary.Value = (decimal)TimeInPrecautionary;
            updown_Time_Slow_Cruise.Value = (decimal)TimeInSlowCruise;
            updown_Time_Full_Cruise.Value = (decimal)TimeInFullCruise;
            updown_HP_Idle.Value = (decimal)HPPEIdle;
            updown_HP_Maneuvering.Value = (decimal)HPPEManeuvering;
            updown_HP_Precautionary.Value = (decimal)HPPEPrecautionary;
            updown_HP_Slow_Cruise.Value = (decimal)HPPESlowCruise;
            updown_HP_Full_Cruise.Value = (decimal)HPPEFullCruise;
            updown_EP_Idle.Value = (decimal)EPIdle;
            updown_EP_Maneuvering.Value = (decimal)EPManeuvering;
            updown_EP_Precautionary.Value = (decimal)EPPrecautionary;
            updown_EP_Slow_Cruise.Value = (decimal)EPSlowCruise;
            updown_EP_Full_Cruise.Value = (decimal)EPFullCruise;
            updown_EP_Total.Value = (decimal)EPTotal;
            updown_KW_HR_Out.Value = (decimal)KWHOutperTrip;
            updown_MMBTU_Out.Value = (decimal)MMBTUoutperTrip;
            updown_MMBTU_In.Value = (decimal)MMBTUinperTrip;
            updown_GALLON.Value = (decimal)GALLONperTrip;

            updown_Tot_Onboard_AUX_HP.Value = (decimal)TotalOnboardAUxHP;
            updown_AUX_Time_Active.Value = (decimal)TimeAuxActiveHours;
            updown_AUX_Active_HP.Value = (decimal)ActiveHPPerAuxEngine;
            updown_Tot_AUX_Energy.Value = (decimal)TotalAuxEnergyProduction;
            updown_AUX_KW_HR_Out.Value = (decimal)AuxEngineKWHoutperTrip;
            updown_AUX_MMBTU_Out.Value = (decimal)AuxEngineMMBTUoutperTrip;
            updown_AUX_MMBTU_In.Value = (decimal)AuxEngineMMBTUinperTrip;
            updown_AUX_GALLON.Value = (decimal)AuxEngineGALLONperTrip;

            Aux_Res_NOX.Value = (decimal)Aux_Res_NOX_gphphr_out;
            Aux_Res_CO.Value = (decimal)Aux_Res_CO_gphphr_out;
            Aux_Res_VOC.Value = (decimal)Aux_Res_VOC_gphphr_out;
            Aux_Res_PM10.Value = (decimal)Aux_Res_PM10_gphphr_out;
            Aux_Res_PM25.Value = (decimal)Aux_Res_PM25_gphphr_out;
            Aux_Res_N2O.Value = (decimal)Aux_Res_N2O_gphphr_out;
            Aux_Res_CH4.Value = (decimal)Aux_Res_CH4_gphphr_out;

            Aux_Die_NOX.Value = (decimal)Aux_Die_NOX_gphphr_out;
            Aux_Die_CO.Value = (decimal)Aux_Die_CO_gphphr_out;
            Aux_Die_VOC.Value = (decimal)Aux_Die_VOC_gphphr_out;
            Aux_Die_PM10.Value = (decimal)Aux_Die_PM10_gphphr_out;
            Aux_Die_PM25.Value = (decimal)Aux_Die_PM25_gphphr_out;
            Aux_Die_N2O.Value = (decimal)Aux_Die_N2O_gphphr_out;
            Aux_Die_CH4.Value = (decimal)Aux_Die_CH4_gphphr_out;

            Aux_Nat_NOX.Value = (decimal)Aux_Nat_NOX_gphphr_out;
            Aux_Nat_CO.Value = (decimal)Aux_Nat_CO_gphphr_out;
            Aux_Nat_VOC.Value = (decimal)Aux_Nat_VOC_gphphr_out;
            Aux_Nat_PM10.Value = (decimal)Aux_Nat_PM10_gphphr_out;
            Aux_Nat_PM25.Value = (decimal)Aux_Nat_PM25_gphphr_out;
            Aux_Nat_N2O.Value = (decimal)Aux_Nat_N2O_gphphr_out;
            Aux_Nat_CH4.Value = (decimal)Aux_Nat_CH4_gphphr_out;

            Aux_Fis_NOX.Value = (decimal)Aux_Fis_NOX_gphphr_out;
            Aux_Fis_CO.Value = (decimal)Aux_Fis_CO_gphphr_out;
            Aux_Fis_VOC.Value = (decimal)Aux_Fis_VOC_gphphr_out;
            Aux_Fis_PM10.Value = (decimal)Aux_Fis_PM10_gphphr_out;
            Aux_Fis_PM25.Value = (decimal)Aux_Fis_PM25_gphphr_out;
            Aux_Fis_N2O.Value = (decimal)Aux_Fis_N2O_gphphr_out;
            Aux_Fis_CH4.Value = (decimal)Aux_Fis_CH4_gphphr_out;

            Aux_Bio_NOX.Value = (decimal)Aux_Bio_NOX_gphphr_out;
            Aux_Bio_CO.Value = (decimal)Aux_Bio_CO_gphphr_out;
            Aux_Bio_VOC.Value = (decimal)Aux_Bio_VOC_gphphr_out;
            Aux_Bio_PM10.Value = (decimal)Aux_Bio_PM10_gphphr_out;
            Aux_Bio_PM25.Value = (decimal)Aux_Bio_PM25_gphphr_out;
            Aux_Bio_N2O.Value = (decimal)Aux_Bio_N2O_gphphr_out;
            Aux_Bio_CH4.Value = (decimal)Aux_Bio_CH4_gphphr_out;

            Aux_Ult_NOX.Value = (decimal)Aux_Ult_NOX_gphphr_out;
            Aux_Ult_CO.Value = (decimal)Aux_Ult_CO_gphphr_out;
            Aux_Ult_VOC.Value = (decimal)Aux_Ult_VOC_gphphr_out;
            Aux_Ult_PM10.Value = (decimal)Aux_Ult_PM10_gphphr_out;
            Aux_Ult_PM25.Value = (decimal)Aux_Ult_PM25_gphphr_out;
            Aux_Ult_N2O.Value = (decimal)Aux_Ult_N2O_gphphr_out;
            Aux_Ult_CH4.Value = (decimal)Aux_Ult_CH4_gphphr_out;

            // Global Warming Potential
            updown_CO2_GWP.Value = (decimal)CO2_GWP;
            updown_CH4_GWP.Value = (decimal)CH4_GWP;
            updown_N2O_GWP.Value = (decimal)N2O_GWP;
            updown_VOC_GWP.Value = (decimal)VOC_GWP;
            updown_CO_GWP.Value = (decimal)CO_GWP;
            updown_NO2_GWP.Value = (decimal)NO2_GWP;
        }
        #endregion

        /// <summary>
        /// Sets the variables to their initial default state
        /// </summary>
        #region Use Defaults
        private void useDefaults()
        {
            #region Main Engine Variables

            // Baseline conventional diesel call for estimates. Will ultimately be re-calculated in the results.
            conventionalDieselBTUperGal = APIcontroller.getApproxBTUperGAL();

            // Main Engine Variables
            VesselTypeID = "Cont. Ship 6000";
            NumberOfEngines = 1;
            SingleEngineHP = 75097;

            // Trip Distance and Time
            TotalTripDistanceInMiles = 10600.00;
            TripTimeHours = 480.00;
            TripTimeMinutes = 0.00;

            // Engine Charicterization Per Mode
            // POT  -   Percent Of Trip, Time in mode is measured in hours
            // HPLF -   Horse Power Load Factor (Single engine)
            // HPPE -   Horse Power Per Engine
            // EP   -   Energy Production in KWH for all engines
            POTIdle = 1.25;
            POTManeuvering = 1.75;
            POTPrecautionary = 5.00;
            POTSlowCruise = 7.00;
            POTFullCruise = 85.00;
            HPLFIdle = 2.00;
            HPLFManeuvering = 8.00;
            HPLFPrecautionary = 12.00;
            HPLFSlowCruise = 50.00;
            HPLFFullCruise = 95.00;

            // Calculation of all Fuels
            EngineEfficiency = 45.00;

            Res_NOX_gphphr_out = 14.015;
            Res_CO_gphphr_out = 2.429;
            Res_VOC_gphphr_out = 0.528;
            Res_PM10_gphphr_out = 0.415;
            Res_PM25_gphphr_out = 0.415;    // Left as same as PM10 for now, per Jamie's request
            Res_N2O_gphphr_out = 0.011;
            Res_CH4_gphphr_out = 0.025;

            Die_NOX_gphphr_out = 14.015;
            Die_CO_gphphr_out = 2.429;
            Die_VOC_gphphr_out = 0.528;
            Die_PM10_gphphr_out = 0.415;
            Die_PM25_gphphr_out = 0.415;
            Die_N2O_gphphr_out = 0.011;
            Die_CH4_gphphr_out = 0.025;

            Nat_NOX_gphphr_out = 14.015;
            Nat_CO_gphphr_out = 1.214;
            Nat_VOC_gphphr_out = 0.528;
            Nat_PM10_gphphr_out = 0.004;
            Nat_PM25_gphphr_out = 0.004;
            Nat_N2O_gphphr_out = 0.011;
            Nat_CH4_gphphr_out = 0.517;

            Fis_NOX_gphphr_out = 14.015;
            Fis_CO_gphphr_out = 2.429;
            Fis_VOC_gphphr_out = 0.528;
            Fis_PM10_gphphr_out = 0.415;
            Fis_PM25_gphphr_out = 0.415;
            Fis_N2O_gphphr_out = 0.011;
            Fis_CH4_gphphr_out = 0.025;

            Bio_NOX_gphphr_out = 14.015;
            Bio_CO_gphphr_out = 2.429;
            Bio_VOC_gphphr_out = 0.528;
            Bio_PM10_gphphr_out = 0.415;
            Bio_PM25_gphphr_out = 0.415;
            Bio_N2O_gphphr_out = 0.022;
            Bio_CH4_gphphr_out = 0.025;

            Ult_NOX_gphphr_out = 14.015;
            Ult_CO_gphphr_out = 2.429;
            Ult_VOC_gphphr_out = 0.528;
            Ult_PM10_gphphr_out = 0.415;
            Ult_PM25_gphphr_out = 0.415;
            Ult_N2O_gphphr_out = 0.011;
            Ult_CH4_gphphr_out = 0.025;

            // Global Warming Potential
            CO2_GWP = 1;
            CH4_GWP = 23;
            N2O_GWP = 296;
            VOC_GWP = 0;
            CO_GWP = 0;
            NO2_GWP = 0;
            #endregion

            #region Auxiliary Engine Variables

            // Auxiliary Engine Variables
            NumberOfOnBoardAuxiliaryEngines = 4;
            NumberOfAuxiliaryEnginesInUse = 2;
            AuxiliaryEnginesRatedHPperEngine = 1400;

            // Auxiliary Engine Characterization (Conventional Diesel as Baseline Fuel)
            PercentOfTripAuxiliaryIsActive = 50.00;
            HPLoadFactorSingleEngine = 80.00;

            // Calculation of Auxiliary Engine Fuel use Using Conventional Diesel as Baseline Fuel
            AuxiliaryEngineEfficiency = 40.00;

            Aux_Res_NOX_gphphr_out = 14.015;
            Aux_Res_CO_gphphr_out = 2.429;
            Aux_Res_VOC_gphphr_out = 0.528;
            Aux_Res_PM10_gphphr_out = 0.415;
            Aux_Res_PM25_gphphr_out = 0.415;
            Aux_Res_N2O_gphphr_out = 0.011;
            Aux_Res_CH4_gphphr_out = 0.025;

            Aux_Die_NOX_gphphr_out = 14.015;
            Aux_Die_CO_gphphr_out = 2.429;
            Aux_Die_VOC_gphphr_out = 0.528;
            Aux_Die_PM10_gphphr_out = 0.415;
            Aux_Die_PM25_gphphr_out = 0.415;
            Aux_Die_N2O_gphphr_out = 0.011;
            Aux_Die_CH4_gphphr_out = 0.025;

            Aux_Nat_NOX_gphphr_out = 14.015;
            Aux_Nat_CO_gphphr_out = 1.214;
            Aux_Nat_VOC_gphphr_out = 0.528;
            Aux_Nat_PM10_gphphr_out = 0.004;
            Aux_Nat_PM25_gphphr_out = 0.004;
            Aux_Nat_N2O_gphphr_out = 0.011;
            Aux_Nat_CH4_gphphr_out = 0.517;

            Aux_Fis_NOX_gphphr_out = 14.015;
            Aux_Fis_CO_gphphr_out = 2.429;
            Aux_Fis_VOC_gphphr_out = 0.528;
            Aux_Fis_PM10_gphphr_out = 0.415;
            Aux_Fis_PM25_gphphr_out = 0.415;
            Aux_Fis_N2O_gphphr_out = 0.011;
            Aux_Fis_CH4_gphphr_out = 0.025;

            Aux_Bio_NOX_gphphr_out = 14.015;
            Aux_Bio_CO_gphphr_out = 2.429;
            Aux_Bio_VOC_gphphr_out = 0.528;
            Aux_Bio_PM10_gphphr_out = 0.415;
            Aux_Bio_PM25_gphphr_out = 0.415;
            Aux_Bio_N2O_gphphr_out = 0.022;
            Aux_Bio_CH4_gphphr_out = 0.025;

            Aux_Ult_NOX_gphphr_out = 14.015;
            Aux_Ult_CO_gphphr_out = 2.429;
            Aux_Ult_VOC_gphphr_out = 0.528;
            Aux_Ult_PM10_gphphr_out = 0.415;
            Aux_Ult_PM25_gphphr_out = 0.415;
            Aux_Ult_N2O_gphphr_out = 0.011;
            Aux_Ult_CH4_gphphr_out = 0.025;

            #endregion

            #region Results functions
            tree_Main_Fuel_Pathways.Select();
            tree_Aux_Fuel_Pathways.Select();
            BuildingFuelPathways();
            #endregion
        }
        #endregion

        // Runs the simulation, and opens up the new results windows
        private void run_menu_Click(object sender, EventArgs e)
        {
            Recalculate();
            results_sheet = new GREETFormattedResults(this);
            results_sheet.Show();
        }

        // This will make a new input sheet, so you can perform multiple simulations at one time
        private void file_new_menu_Click(object sender, EventArgs e)
        {
            Form f = new TEAMS();
            f.Show();
        }

        // Closes the sheets if you hit the exit button
        private void file_exit_menu_Click(object sender, EventArgs e)
        {
            results_sheet.Close();
            this.Close();
        }

        #region Recalculate Values

        private void Recalc_Button_Click(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void Recalculate()
        {
            if (checkValid() == false)
            {
                MessageBox.Show("Percent of Trip In Mode (time) on Main Engine tab must sum to 100%!!", "ERROR");
                return;
            }

            VesselTypeID = textBox_Vessel_Type.Text;
            NumberOfEngines = (int)updown_Number_Of_Engines.Value;
            SingleEngineHP = (int)updown_Single_Engine_HP.Value;
            TotalOnboardHP = SingleEngineHP * NumberOfEngines;
            updown_Tot_Onboard_HP.Value = (decimal)TotalOnboardHP;

            TotalTripDistanceInMiles = (double)updown_Total_Trip_Distance.Value;
            TripTimeHours = (double)updown_Trip_Time_Hours.Value;
            TripTimeMinutes = (double)updown_Trip_Time_Minutes.Value;
            TotalTripTimeHours = TripTimeHours + (TripTimeMinutes / 60);
            updown_Tot_Trip_Hours.Value = (decimal)TotalTripTimeHours;

            POTIdle = (double)updown_Percent_Idle.Value;
            POTManeuvering = (double)updown_Percent_Maneuvering.Value;
            POTPrecautionary = (double)updown_Percent_Precautionary.Value;
            POTSlowCruise = (double)updown_Percent_Slow_Cruise.Value;
            POTFullCruise = (double)updown_Percent_Full_Cruise.Value;
            TimeInIdle = (POTIdle / 100) * TotalTripTimeHours;
            updown_Time_Idle.Value = (decimal)TimeInIdle;
            TimeInManeuvering = (POTManeuvering / 100) * TotalTripTimeHours;
            updown_Time_Maneuvering.Value = (decimal)TimeInManeuvering;
            TimeInPrecautionary = (POTPrecautionary / 100) * TotalTripTimeHours;
            updown_Time_Precautionary.Value = (decimal)TimeInPrecautionary;
            TimeInSlowCruise = (POTSlowCruise / 100) * TotalTripTimeHours;
            updown_Time_Slow_Cruise.Value = (decimal)TimeInSlowCruise;
            TimeInFullCruise = (POTFullCruise / 100) * TotalTripTimeHours;
            updown_Time_Full_Cruise.Value = (decimal)TimeInFullCruise;
            HPLFIdle = (double)updown_Horsepower_Idle.Value;
            HPLFManeuvering = (double)updown_Horsepower_Maneuvering.Value;
            HPLFPrecautionary = (double)updown_Horsepower_Precautionary.Value;
            HPLFSlowCruise = (double)updown_Horsepower_Slow_Cruise.Value;
            HPLFFullCruise = (double)updown_Horsepower_Full_Cruise.Value;

            HPPEIdle = (HPLFIdle / 100) * SingleEngineHP;
            updown_HP_Idle.Value = (decimal)HPPEIdle;
            HPPEManeuvering = (HPLFManeuvering / 100) * SingleEngineHP;
            updown_HP_Maneuvering.Value = (decimal)HPPEManeuvering;
            HPPEPrecautionary = (HPLFPrecautionary / 100) * SingleEngineHP;
            updown_HP_Precautionary.Value = (decimal)HPPEPrecautionary;
            HPPESlowCruise = (HPLFSlowCruise / 100) * SingleEngineHP;
            updown_HP_Slow_Cruise.Value = (decimal)HPPESlowCruise;
            HPPEFullCruise = (HPLFFullCruise / 100) * SingleEngineHP;
            updown_HP_Full_Cruise.Value = (decimal)HPPEFullCruise;
            EPIdle = NumberOfEngines * HPPEIdle * TimeInIdle * KWperHP;
            updown_EP_Idle.Value = (decimal)EPIdle;
            EPManeuvering = NumberOfEngines * HPPEManeuvering * TimeInManeuvering * KWperHP;
            updown_EP_Maneuvering.Value = (decimal)EPManeuvering;
            EPPrecautionary = NumberOfEngines * HPPEPrecautionary * TimeInPrecautionary * KWperHP;
            updown_EP_Precautionary.Value = (decimal)EPPrecautionary;
            EPSlowCruise = NumberOfEngines * HPPESlowCruise * TimeInSlowCruise * KWperHP;
            updown_EP_Slow_Cruise.Value = (decimal)EPSlowCruise;
            EPFullCruise = NumberOfEngines * HPPEFullCruise * TimeInFullCruise * KWperHP;
            updown_EP_Full_Cruise.Value = (decimal)EPFullCruise;
            EPTotal = EPIdle + EPManeuvering + EPPrecautionary + EPSlowCruise + EPFullCruise;
            updown_EP_Total.Value = (decimal)EPTotal;

            EngineEfficiency = (double)updown_Engine_Efficiency.Value;
            KWHOutperTrip = EPTotal;
            updown_KW_HR_Out.Value = (decimal)KWHOutperTrip;
            MMBTUoutperTrip = (KWHOutperTrip * BTUperKWH) / BTUS_PER_MMBTU;
            updown_MMBTU_Out.Value = (decimal)MMBTUoutperTrip;
            MMBTUinperTrip = MMBTUoutperTrip * (100 / EngineEfficiency);
            updown_MMBTU_In.Value = (decimal)MMBTUinperTrip;
            GALLONperTrip = (1 / conventionalDieselBTUperGal) * BTUS_PER_MMBTU * MMBTUinperTrip;
            updown_GALLON.Value = (decimal)GALLONperTrip;

            Res_NOX_gphphr_out = (double)Res_NOX.Value;
            Res_CO_gphphr_out = (double)Res_CO.Value;
            Res_VOC_gphphr_out = (double)Res_VOC.Value;
            Res_PM10_gphphr_out = (double)Res_PM10.Value;
            Res_PM25_gphphr_out = (double)Res_PM25.Value;
            Res_N2O_gphphr_out = (double)Res_N2O.Value;
            Res_CH4_gphphr_out = (double)Res_CH4.Value;
            Residual_Oil = new double[7] 
                {Res_VOC_gphphr_out, Res_CO_gphphr_out, Res_NOX_gphphr_out, 
                Res_PM10_gphphr_out, Res_PM25_gphphr_out, Res_CH4_gphphr_out, 
                Res_N2O_gphphr_out};

            Die_NOX_gphphr_out = (double)Die_NOX.Value;
            Die_CO_gphphr_out = (double)Die_CO.Value;
            Die_VOC_gphphr_out = (double)Die_VOC.Value;
            Die_PM10_gphphr_out = (double)Die_PM10.Value;
            Die_PM25_gphphr_out = (double)Die_PM25.Value;
            Die_N2O_gphphr_out = (double)Die_N2O.Value;
            Die_CH4_gphphr_out = (double)Die_CH4.Value;
            Diesel = new double[7] 
                {Die_VOC_gphphr_out, Die_CO_gphphr_out, Die_NOX_gphphr_out, 
                Die_PM10_gphphr_out, Die_PM25_gphphr_out, Die_CH4_gphphr_out, 
                Die_N2O_gphphr_out};

            Nat_NOX_gphphr_out = (double)Nat_NOX.Value;
            Nat_CO_gphphr_out = (double)Nat_CO.Value;
            Nat_VOC_gphphr_out = (double)Nat_VOC.Value;
            Nat_PM10_gphphr_out = (double)Nat_PM10.Value;
            Nat_PM25_gphphr_out = (double)Nat_PM25.Value;
            Nat_N2O_gphphr_out = (double)Nat_N2O.Value;
            Nat_CH4_gphphr_out = (double)Nat_CH4.Value;
            Natural_Gas = new double[7] 
                {Nat_VOC_gphphr_out, Nat_CO_gphphr_out, Nat_NOX_gphphr_out, 
                Nat_PM10_gphphr_out, Nat_PM25_gphphr_out, Nat_CH4_gphphr_out, 
                Nat_N2O_gphphr_out};

            Fis_NOX_gphphr_out = (double)Fis_NOX.Value;
            Fis_CO_gphphr_out = (double)Fis_CO.Value;
            Fis_VOC_gphphr_out = (double)Fis_VOC.Value;
            Fis_PM10_gphphr_out = (double)Fis_PM10.Value;
            Fis_PM25_gphphr_out = (double)Fis_PM25.Value;
            Fis_N2O_gphphr_out = (double)Fis_N2O.Value;
            Fis_CH4_gphphr_out = (double)Fis_CH4.Value;
            Fischer = new double[7] 
                {Fis_VOC_gphphr_out, Fis_CO_gphphr_out, Fis_NOX_gphphr_out, 
                Fis_PM10_gphphr_out, Fis_PM25_gphphr_out, Fis_CH4_gphphr_out, 
                Fis_N2O_gphphr_out};

            Bio_NOX_gphphr_out = (double)Bio_NOX.Value;
            Bio_CO_gphphr_out = (double)Bio_CO.Value;
            Bio_VOC_gphphr_out = (double)Bio_VOC.Value;
            Bio_PM10_gphphr_out = (double)Bio_PM10.Value;
            Bio_PM25_gphphr_out = (double)Bio_PM25.Value;
            Bio_N2O_gphphr_out = (double)Bio_N2O.Value;
            Bio_CH4_gphphr_out = (double)Bio_CH4.Value;
            Biodiesel = new double[7] 
                {Bio_VOC_gphphr_out, Bio_CO_gphphr_out, Bio_NOX_gphphr_out, 
                Bio_PM10_gphphr_out, Bio_PM25_gphphr_out, Bio_CH4_gphphr_out, 
                Bio_N2O_gphphr_out};

            Ult_NOX_gphphr_out = (double)Ult_NOX.Value;
            Ult_CO_gphphr_out = (double)Ult_CO.Value;
            Ult_VOC_gphphr_out = (double)Ult_VOC.Value;
            Ult_PM10_gphphr_out = (double)Ult_PM10.Value;
            Ult_PM25_gphphr_out = (double)Ult_PM25.Value;
            Ult_N2O_gphphr_out = (double)Ult_N2O.Value;
            Ult_CH4_gphphr_out = (double)Ult_CH4.Value;
            Ult_Low_Sulf = new double[7] 
                {Ult_VOC_gphphr_out, Ult_CO_gphphr_out, Ult_NOX_gphphr_out, 
                Ult_PM10_gphphr_out, Ult_PM25_gphphr_out, Ult_CH4_gphphr_out, 
                Ult_N2O_gphphr_out};


            // Global Warming Potential
            CO2_GWP = (double)updown_CO2_GWP.Value;
            CH4_GWP = (double)updown_CH4_GWP.Value;
            N2O_GWP = (double)updown_N2O_GWP.Value;
            VOC_GWP = (double)updown_VOC_GWP.Value;
            CO_GWP = (double)updown_CO_GWP.Value;
            NO2_GWP = (double)updown_NO2_GWP.Value;

            NumberOfOnBoardAuxiliaryEngines = (int)updown_Aux_Number_Engines.Value;
            NumberOfAuxiliaryEnginesInUse = (int)updown_Aux_Engines_In_Use.Value;
            AuxiliaryEnginesRatedHPperEngine = (int)updown_Aux_HP_Per_Engine.Value;
            TotalOnboardAUxHP = NumberOfAuxiliaryEnginesInUse * AuxiliaryEnginesRatedHPperEngine;
            updown_Tot_Onboard_AUX_HP.Value = (decimal)TotalOnboardAUxHP;

            PercentOfTripAuxiliaryIsActive = (double)updown_Aux_Percent_Trip_Active.Value;
            TimeAuxActiveHours = (PercentOfTripAuxiliaryIsActive / 100) * TotalTripTimeHours;
            updown_AUX_Time_Active.Value = (decimal)TimeAuxActiveHours;
            HPLoadFactorSingleEngine = (double)updown_Aux_HP_Load_Factor.Value;
            ActiveHPPerAuxEngine = (HPLoadFactorSingleEngine / 100) * AuxiliaryEnginesRatedHPperEngine;
            updown_AUX_Active_HP.Value = (decimal)ActiveHPPerAuxEngine;
            TotalAuxEnergyProduction = NumberOfAuxiliaryEnginesInUse * ActiveHPPerAuxEngine * TimeAuxActiveHours * KWperHP;
            updown_Tot_AUX_Energy.Value = (decimal)TotalAuxEnergyProduction;

            AuxiliaryEngineEfficiency = (double)updown_Aux_Engine_Efficiency.Value;
            AuxEngineKWHoutperTrip = TotalAuxEnergyProduction;
            updown_AUX_KW_HR_Out.Value = (decimal)AuxEngineKWHoutperTrip;
            AuxEngineMMBTUoutperTrip = (AuxEngineKWHoutperTrip * BTUperKWH) / BTUS_PER_MMBTU;

            updown_AUX_MMBTU_Out.Value = (decimal)AuxEngineMMBTUoutperTrip;
            AuxEngineMMBTUinperTrip = AuxEngineMMBTUoutperTrip * (100 / AuxiliaryEngineEfficiency);
            updown_AUX_MMBTU_In.Value = (decimal)AuxEngineMMBTUinperTrip;
            AuxEngineGALLONperTrip = (1 / conventionalDieselBTUperGal) * BTUS_PER_MMBTU * AuxEngineMMBTUinperTrip;
            updown_AUX_GALLON.Value = (decimal)AuxEngineGALLONperTrip;

            Aux_Res_NOX_gphphr_out = (double)Aux_Res_NOX.Value;
            Aux_Res_CO_gphphr_out = (double)Aux_Res_CO.Value;
            Aux_Res_VOC_gphphr_out = (double)Aux_Res_VOC.Value;
            Aux_Res_PM10_gphphr_out = (double)Aux_Res_PM10.Value;
            Aux_Res_PM25_gphphr_out = (double)Aux_Res_PM25.Value;
            Aux_Res_N2O_gphphr_out = (double)Aux_Res_N2O.Value;
            Aux_Res_CH4_gphphr_out = (double)Aux_Res_CH4.Value;
            Aux_Residual_Oil = new double[7] 
                {Aux_Res_VOC_gphphr_out, Aux_Res_CO_gphphr_out, Aux_Res_NOX_gphphr_out, 
                Aux_Res_PM10_gphphr_out, Aux_Res_PM25_gphphr_out, Aux_Res_CH4_gphphr_out, 
                Aux_Res_N2O_gphphr_out};

            Aux_Die_NOX_gphphr_out = (double)Aux_Die_NOX.Value;
            Aux_Die_CO_gphphr_out = (double)Aux_Die_CO.Value;
            Aux_Die_VOC_gphphr_out = (double)Aux_Die_VOC.Value;
            Aux_Die_PM10_gphphr_out = (double)Aux_Die_PM10.Value;
            Aux_Die_PM25_gphphr_out = (double)Aux_Die_PM25.Value;
            Aux_Die_N2O_gphphr_out = (double)Aux_Die_N2O.Value;
            Aux_Die_CH4_gphphr_out = (double)Aux_Die_CH4.Value;
            Aux_Diesel = new double[7] 
                {Aux_Die_VOC_gphphr_out, Aux_Die_CO_gphphr_out, Aux_Die_NOX_gphphr_out, 
                Aux_Die_PM10_gphphr_out, Aux_Die_PM25_gphphr_out, Aux_Die_CH4_gphphr_out, 
                Aux_Die_N2O_gphphr_out};

            Aux_Nat_NOX_gphphr_out = (double)Aux_Nat_NOX.Value;
            Aux_Nat_CO_gphphr_out = (double)Aux_Nat_CO.Value;
            Aux_Nat_VOC_gphphr_out = (double)Aux_Nat_VOC.Value;
            Aux_Nat_PM10_gphphr_out = (double)Aux_Nat_PM10.Value;
            Aux_Nat_PM25_gphphr_out = (double)Aux_Nat_PM25.Value;
            Aux_Nat_N2O_gphphr_out = (double)Aux_Nat_N2O.Value;
            Aux_Nat_CH4_gphphr_out = (double)Aux_Nat_CH4.Value;
            Aux_Natural_Gas = new double[7] 
                {Aux_Nat_VOC_gphphr_out, Aux_Nat_CO_gphphr_out, Aux_Nat_NOX_gphphr_out, 
                Aux_Nat_PM10_gphphr_out, Aux_Nat_PM25_gphphr_out, Aux_Nat_CH4_gphphr_out, 
                Aux_Nat_N2O_gphphr_out};

            Aux_Fis_NOX_gphphr_out = (double)Aux_Fis_NOX.Value;
            Aux_Fis_CO_gphphr_out = (double)Aux_Fis_CO.Value;
            Aux_Fis_VOC_gphphr_out = (double)Aux_Fis_VOC.Value;
            Aux_Fis_PM10_gphphr_out = (double)Aux_Fis_PM10.Value;
            Aux_Fis_PM25_gphphr_out = (double)Aux_Fis_PM25.Value;
            Aux_Fis_N2O_gphphr_out = (double)Aux_Fis_N2O.Value;
            Aux_Fis_CH4_gphphr_out = (double)Aux_Fis_CH4.Value;
            Aux_Fischer = new double[7] 
                {Aux_Fis_VOC_gphphr_out, Aux_Fis_CO_gphphr_out, Aux_Fis_NOX_gphphr_out, 
                Aux_Fis_PM10_gphphr_out, Aux_Fis_PM25_gphphr_out, Aux_Fis_CH4_gphphr_out, 
                Aux_Fis_N2O_gphphr_out};

            Aux_Bio_NOX_gphphr_out = (double)Aux_Bio_NOX.Value;
            Aux_Bio_CO_gphphr_out = (double)Aux_Bio_CO.Value;
            Aux_Bio_VOC_gphphr_out = (double)Aux_Bio_VOC.Value;
            Aux_Bio_PM10_gphphr_out = (double)Aux_Bio_PM10.Value;
            Aux_Bio_PM25_gphphr_out = (double)Aux_Bio_PM25.Value;
            Aux_Bio_N2O_gphphr_out = (double)Aux_Bio_N2O.Value;
            Aux_Bio_CH4_gphphr_out = (double)Aux_Bio_CH4.Value;
            Aux_Biodiesel = new double[7] 
                {Aux_Bio_VOC_gphphr_out, Aux_Bio_CO_gphphr_out, Aux_Bio_NOX_gphphr_out, 
                Aux_Bio_PM10_gphphr_out, Aux_Bio_PM25_gphphr_out, Aux_Bio_CH4_gphphr_out, 
                Aux_Bio_N2O_gphphr_out};

            Aux_Ult_NOX_gphphr_out = (double)Aux_Ult_NOX.Value;
            Aux_Ult_CO_gphphr_out = (double)Aux_Ult_CO.Value;
            Aux_Ult_VOC_gphphr_out = (double)Aux_Ult_VOC.Value;
            Aux_Ult_PM10_gphphr_out = (double)Aux_Ult_PM10.Value;
            Aux_Ult_PM25_gphphr_out = (double)Aux_Ult_PM25.Value;
            Aux_Ult_N2O_gphphr_out = (double)Aux_Ult_N2O.Value;
            Aux_Ult_CH4_gphphr_out = (double)Aux_Ult_CH4.Value;
            Aux_Ult_Low_Sulf = new double[7] 
                {Aux_Ult_VOC_gphphr_out, Aux_Ult_CO_gphphr_out, Aux_Ult_NOX_gphphr_out, 
                Aux_Ult_PM10_gphphr_out, Aux_Ult_PM25_gphphr_out, Aux_Ult_CH4_gphphr_out, 
                Aux_Ult_N2O_gphphr_out};

            doCalculations();
            changeResults();
        }
        #endregion

        // Checks to see that the percentage of trip variables actually add up to 100
        private bool checkValid()
        {
            decimal sum = 0;
            sum = (updown_Percent_Idle.Value + updown_Percent_Maneuvering.Value + updown_Percent_Precautionary.Value +
                    updown_Percent_Slow_Cruise.Value + updown_Percent_Full_Cruise.Value);
            if (sum != 100) { return false; }
            return true;
        }

        // Resets the input sheet back to default inputs
        private void ResetButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you wish to reset the form back to default values?", "Reset Input", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Close();
                TEAMS newWindow = new TEAMS();
                newWindow.Show();
            }
        }

        //These are the label clicks, which handles the extension of the panels
        #region labelClicks
        private void labelClicked(Label labelClicked, Panel panelExtending)
        {
            if (panelExtending.Visible == true)
            {
                panelExtending.Visible = false;
            }
            else
            {
                panelExtending.Visible = true;
            }

            foreach (Panel p in allPanels)
            {
                if (p != panelExtending && p.Top > panelExtending.Top)
                {
                    if (panelExtending.Visible == true)
                    {
                        p.Top += panelExtending.Height;
                    }
                    else if (panelExtending.Visible == false)
                    {
                        p.Top -= panelExtending.Height;
                    }
                }
            }

            foreach (Label l in allPanelLabels)
            {
                if (l != labelClicked && l.Top > panelExtending.Top)
                {
                    if (panelExtending.Visible == true)
                    {
                        l.Top += panelExtending.Height;
                    }
                    else if (panelExtending.Visible == false)
                    {
                        l.Top -= panelExtending.Height;
                    }
                }
            }
        }

        private void label_Main_Engine_Vars_Click(object sender, EventArgs e)
        {
            labelClicked(label_Main_Engine_Vars, panel_Main_Engine_Vars);
        }

        private void label_Main_Total_Distance_And_Time_Click(object sender, EventArgs e)
        {
            labelClicked(label_Main_Total_Distance_And_Time, panel_Main_Trip_Distance_Time);
        }

        private void label_Main_Engine_Char_Per_Mode_Click(object sender, EventArgs e)
        {
            labelClicked(label_Main_Engine_Char_Per_Mode, panel_Main_Engine_Chars);
        }

        private void label_Main_Fuel_Calculations_Click(object sender, EventArgs e)
        {

            labelClicked(label_Main_Fuel_Calculations, panel_Main_Fuel_Calculations);
        }

        private void label_Main_Global_Warming_Potentials_Click(object sender, EventArgs e)
        {

            labelClicked(label_Main_Global_Warming_Potentials, panel_Main_GWPs);
        }

        private void label_Main_Engine_Inputs_Click(object sender, EventArgs e)
        {
            labelClicked(label_Main_Engine_Inputs, panel_main_engine_inputs);
        }


        private void label_Aux_Engine_Vars_Click(object sender, EventArgs e)
        {
            labelClicked(label_Aux_Engine_Vars, panel_Aux_Engine_Vars);
        }

        private void label_Aux_Engine_Chars_Click(object sender, EventArgs e)
        {
            labelClicked(label_Aux_Engine_Chars, panel_Aux_Engine_Chars);
        }

        private void label_Aux_Fuel_Calc_Click(object sender, EventArgs e)
        {
            labelClicked(label_Aux_Fuel_Calc, panel_Aux_Fuel_Calc);
        }

        private void label_Aux_Emissions_Calc_Inputs_Click(object sender, EventArgs e)
        {
            labelClicked(label_Aux_Emissions_Calc_Inputs, panel_Aux_Emissions_Input);
        }

        private void Label_FuelSelection_Click(object sender, EventArgs e)
        {
            labelClicked(Label_FuelSelection, Panel_FuelSelection);

        }

        #endregion

        #region Results Constants

        // Grams per 1 Kilogram
        private const double GRAMS_PER_KILOGRAM = 1000.0;
        // Kilowatt-hours per 1 Horsepower-hour
        private const double KWHRS_PER_HPHR = 0.745699871;
        // Grams Sulfur Oxide to Grams Sulfur Ratio
        private const double GRAMS_SOX_PER_GRAMS_S = 64 / 32;

        // Resource ID Numbers
        private const int CONVENTIONAL_DIESEL_ID = 27;
        private const int RESIDUAL_OIL_ID = 33;
        private const int LOW_SULFUR_DIESEL_ID = 30;
        private const int LIQUID_NATURAL_GAS_ID = 41;
        private const int BIODIESEL_ID = 44;
        private const int FISCHER_TROPSCH_ID = 45;

        // Collection of resource id numbers for use in the tree building selections
        private int[] resource_ids = new int[6] { CONVENTIONAL_DIESEL_ID, RESIDUAL_OIL_ID, LOW_SULFUR_DIESEL_ID, 
                                                      LIQUID_NATURAL_GAS_ID, BIODIESEL_ID, FISCHER_TROPSCH_ID };
        #endregion
        #region Fuel Pathways Setup
        private void BuildingFuelPathways()
        {
            tree_Main_Fuel_Pathways.Nodes.Clear();
            tree_Aux_Fuel_Pathways.Nodes.Clear();
            TreeView[] FuelTrees = new TreeView[2] { tree_Main_Fuel_Pathways, tree_Aux_Fuel_Pathways };

            // Adds pathways and mixes to the list so the user can select one
            foreach (TreeView tree in FuelTrees)
            {
                foreach (int id in resource_ids)
                {
                    foreach (IResource resource in APIcontroller.getSpecificResources(id))
                    {
                        TreeNode resourceTreeNode = new TreeNode(resource.Name);
                        resourceTreeNode.Tag = resource;

                        foreach (IPathway pathway in APIcontroller.getSpecificPathways(id))
                        {
                            TreeNode pathwayNode = new TreeNode(pathway.Name);
                            pathwayNode.Tag = pathway;
                            resourceTreeNode.Nodes.Add(pathwayNode);
                        }
                        if (resourceTreeNode.Nodes.Count > 0)
                            tree.Nodes.Add(resourceTreeNode);
                    }
                }
            }
        }
        #endregion

        private void Panel_FuelSelection_Validated(object sender, EventArgs e)
        {
            //initializePanelsAndLabels();
        }

        private void TEAMS_Shown(object sender, EventArgs e)
        {
            initializePanelsAndLabels();
        }

        //Currently commented out because it will not work without a bunch of things from the results sheet
        #region Final results calculation
        /*
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Object tag = tree_Main_Fuel_Pathways.SelectedNode.Tag;
            if (tag is IPathway)
            {
                IData data = APIcontroller.getData();

                IPathway path = tag as IPathway;
                IResource resourceUsed = APIcontroller.getResourceUsed(path);

                // Grabs the fuel used in the main engine to display to the user.
                MainfuelUsed = APIcontroller.getFuelUsed(path);

                IResults pathwayResults = APIcontroller.getPathwayResults(data, path);

                teams_sheet.GALLONperTrip = APIcontroller.getGallonsPerMMBTU(resourceUsed) * teams_sheet.MMBTUinperTrip;

                //These numbers will be used in calculations below, and are based on whether or not the user has edited GREET resource variables
                double resourceDensity = APIcontroller.getResourceDensity(resourceUsed);
                double resourceSulfurRatio = APIcontroller.getResourceSulfurRatio(resourceUsed);
                double resourceLowerHeatingValue = APIcontroller.getResourceLowerHeatingValue(resourceUsed);
                double resourceCarbonRatio = APIcontroller.getResourceCarbonRatio(resourceUsed);


                double[] main_fuel_type = new double[7];

                if (MainfuelUsed.Equals("Conventional Diesel"))
                {
                    Array.Copy(teams_sheet.Diesel, main_fuel_type, teams_sheet.Diesel.Length);
                }
                else if (MainfuelUsed.Equals("Residual Oil"))
                {
                    Array.Copy(teams_sheet.Residual_Oil, main_fuel_type, teams_sheet.Residual_Oil.Length);
                }
                else if (MainfuelUsed.Equals("Low-Sulfur Diesel"))
                {
                    Array.Copy(teams_sheet.Ult_Low_Sulf, main_fuel_type, teams_sheet.Ult_Low_Sulf.Length);
                }
                else if (MainfuelUsed.Equals("Liquefied Natural Gas"))
                {
                    Array.Copy(teams_sheet.Natural_Gas, main_fuel_type, teams_sheet.Natural_Gas.Length);
                }
                else if (MainfuelUsed.Equals("Biodiesel"))
                {
                    Array.Copy(teams_sheet.Biodiesel, main_fuel_type, teams_sheet.Biodiesel.Length);
                }
                else
                {
                    Array.Copy(teams_sheet.Fischer, main_fuel_type, teams_sheet.Fischer.Length);
                }

                //These should be relatively accurate no matter what, since it's a total energy and not the different engines
                //Total Energy Well To Pump = mmbtu of fuel put into the engine * all sections of energy for what it took to create 1 mmbtu of fuel - the 1 mmbtu of fuel

                TE_WTP = teams_sheet.MMBTUinperTrip * APIcontroller.getSumAllLifeCycleResources(pathwayResults) - 1 - teams_sheet.MMBTUinperTrip;
                //TE_WTP = teams_sheet.MMBTUinperTrip * APIcontroller.getSumAllLifeCycleResources(pathwayResults);

                //Total Energy Vessel Operation = mmbtu needed to put into the ship
                TE_VO = teams_sheet.MMBTUinperTrip;
                //Total Energy = Vessel Operation + Well to pump + aux vessel operation + aux well to pump
                TE_Total = TE_WTP + TE_VO + AUX_TE_WTP + AUX_TE_VO;

                // TODO: Implement Fossil Fuels and Petroleum Fuels
                // Fossil Fuels in WTP =  mmbtuin * a greet energy WTP value
                //FF_WTP = teams_sheet.MMBTUinperTrip * pathwayResults.LifeCycleResourcesGroups(data).ElementAt(0).Value.Value;
                //FF_Total = FF_WTP + FF_VO + AUX_FF_WTP + AUX_FF_VO;
                FF_WTP = 0;
                FF_Total = 0;

                // Petroleum Fuel in WTP =  mmbtuin * a greet energy WTP value
                //PF_WTP = teams_sheet.MMBTUinperTrip * pathwayResults.LifeCycleResourcesGroups(data).ElementAt(2).Value.Value;
                //PF_Total = PF_WTP + PF_VO + AUX_PF_WTP + AUX_PF_VO;
                PF_WTP = 0;
                PF_Total = 0;

                // Volatile Organic Compounds
                VOC_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 0) * teams_sheet.MMBTUinperTrip;
                VOC_VO = main_fuel_type[0] * (1 / KWHRS_PER_HPHR) * teams_sheet.KWHOutperTrip;
                VOC_Total = VOC_WTP + VOC_VO + AUX_VOC_WTP + AUX_VOC_VO;

                // Carbon Monoxide
                CO_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 1) * teams_sheet.MMBTUinperTrip;
                CO_VO = (main_fuel_type[1] * (1 / KWHRS_PER_HPHR) * teams_sheet.KWHOutperTrip);
                CO_Total = CO_WTP + CO_VO + AUX_CO_WTP + AUX_CO_VO;

                // Nitrogen Dioxide
                NOx_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 2) * teams_sheet.MMBTUinperTrip;
                NOx_VO = main_fuel_type[2] * (1 / KWHRS_PER_HPHR) * teams_sheet.KWHOutperTrip;
                NOx_Total = NOx_WTP + NOx_VO + AUX_NOx_WTP + AUX_NOx_VO;

                // Particulate Matter 10
                PM10_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 3) * teams_sheet.MMBTUinperTrip;
                PM10_VO = main_fuel_type[3] * (1 / KWHRS_PER_HPHR) * teams_sheet.KWHOutperTrip;
                PM10_Total = PM10_WTP + PM10_VO + AUX_PM10_WTP + AUX_PM10_VO;

                // Particulate Matter 25
                PM25_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 4) * teams_sheet.MMBTUinperTrip;
                PM25_VO = main_fuel_type[4] * (1 / KWHRS_PER_HPHR) * teams_sheet.KWHOutperTrip;
                PM25_Total = PM25_WTP + PM25_VO + AUX_PM25_WTP + AUX_PM25_VO;

                // Sulfur Oxides
                SOx_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 5) * teams_sheet.MMBTUinperTrip;
                SOx_VO = resourceDensity * resourceSulfurRatio * GRAMS_PER_KILOGRAM * (1 / GALLONS_PER_CUBIC_METER) * GRAMS_SOX_PER_GRAMS_S * teams_sheet.GALLONperTrip;
                SOx_Total = SOx_WTP + SOx_VO + AUX_SOx_WTP + AUX_SOx_VO;

                // Methane
                CH4_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 6) * teams_sheet.MMBTUinperTrip;
                CH4_VO = main_fuel_type[5] * (1 / KWHRS_PER_HPHR) * teams_sheet.KWHOutperTrip;
                CH4_Total = CH4_WTP + CH4_VO + AUX_CH4_WTP + AUX_CH4_VO;

                // Carbon Dioxide
                double gramsOfFuel = (1 / resourceLowerHeatingValue) * resourceDensity * JOULES_PER_MMBTU * GRAMS_PER_KILOGRAM * teams_sheet.MMBTUinperTrip;
                CO2_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 8) * teams_sheet.MMBTUinperTrip;
                CO2_VO = gramsOfFuel * resourceCarbonRatio * (44 / 12);
                CO2_Total = CO2_WTP + CO2_VO + AUX_CO2_WTP + AUX_CO2_VO;

                //Nitrous Oxide
                N2O_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 7) * teams_sheet.MMBTUinperTrip;
                N2O_VO = main_fuel_type[6] * (1 / KWHRS_PER_HPHR) * teams_sheet.KWHOutperTrip;
                N2O_Total = N2O_WTP + N2O_VO + AUX_N2O_WTP + AUX_N2O_VO;
            }
            setLabels();
        }
        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Object tag = tree_Aux_Fuel_Pathways.SelectedNode.Tag;
            if (tag is IPathway)
            {
                IData data = APIcontroller.getData();
                IPathway path = tag as IPathway;
                IResource resourceUsed = APIcontroller.getResourceUsed(path);

                // Gets the fuel used in the auxiliary engine to display to the user.
                auxFuelUsed = APIcontroller.getFuelUsed(path);

                IResults pathwayResults = APIcontroller.getPathwayResults(data, path);

                // These numbers will be used in calculations below, and are based on whether or not the user has tried to edit GREET resource variables
                double resourceDensity = APIcontroller.getResourceDensity(resourceUsed);
                double resourceSulfurRatio = APIcontroller.getResourceSulfurRatio(resourceUsed);
                double resourceLowerHeatingValue = APIcontroller.getResourceLowerHeatingValue(resourceUsed);
                double resourceCarbonRatio = APIcontroller.getResourceCarbonRatio(resourceUsed);

                double[] aux_fuel_type = new double[7];

                if (auxFuelUsed.Equals("Conventional Diesel"))
                {
                    Array.Copy(teams_sheet.Aux_Diesel, aux_fuel_type, teams_sheet.Aux_Diesel.Length);
                }
                else if (auxFuelUsed.Equals("Residual Oil"))
                {
                    Array.Copy(teams_sheet.Aux_Residual_Oil, aux_fuel_type, teams_sheet.Aux_Residual_Oil.Length);
                }
                else if (auxFuelUsed.Equals("Low-Sulfur Diesel"))
                {
                    Array.Copy(teams_sheet.Aux_Ult_Low_Sulf, aux_fuel_type, teams_sheet.Aux_Ult_Low_Sulf.Length);
                }
                else if (auxFuelUsed.Equals("Liquefied Natural Gas"))
                {
                    Array.Copy(teams_sheet.Aux_Natural_Gas, aux_fuel_type, teams_sheet.Aux_Natural_Gas.Length);
                }
                else if (auxFuelUsed.Equals("Biodiesel"))
                {
                    Array.Copy(teams_sheet.Aux_Biodiesel, aux_fuel_type, teams_sheet.Aux_Biodiesel.Length);
                }
                else
                {
                    Array.Copy(teams_sheet.Aux_Fischer, aux_fuel_type, teams_sheet.Aux_Fischer.Length);
                }

                teams_sheet.AuxEngineGALLONperTrip = (1 / resourceLowerHeatingValue) * GALLONS_PER_CUBIC_METER * JOULES_PER_MMBTU * teams_sheet.AuxEngineMMBTUinperTrip;

                AUX_TE_WTP = teams_sheet.AuxEngineMMBTUinperTrip * APIcontroller.getSumAllLifeCycleResources(pathwayResults) - 1 - teams_sheet.AuxEngineMMBTUinperTrip;

                AUX_TE_VO = teams_sheet.AuxEngineMMBTUinperTrip;
                TE_Total = TE_WTP + TE_VO + AUX_TE_WTP + AUX_TE_VO;

                // TODO: Implement Fossil Fuels and Petroleum Fuels
                // Fossil Fuels in WTP =  mmbtuin * a greet energy WTP value
                //AUX_FF_WTP = teams_sheet.AuxEngineMMBTUinperTrip * pathwayResults.LifeCycleResourcesGroups(data).ElementAt(0).Value.Value;
                //FF_Total = FF_WTP + FF_VO + AUX_FF_WTP + AUX_FF_VO;
                AUX_FF_WTP = 0;
                FF_Total = 0;

                // Petroleum Fuel in WTP =  mmbtuin * a greet energy WTP value
                //AUX_PF_WTP = teams_sheet.AuxEngineMMBTUinperTrip * pathwayResults.LifeCycleResourcesGroups(data).ElementAt(2).Value.Value;
                //PF_Total = PF_WTP + PF_VO + AUX_PF_WTP + AUX_PF_VO;              
                AUX_PF_WTP = 0;
                PF_Total = 0;

                AUX_VOC_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 0) * teams_sheet.MMBTUinperTrip;
                AUX_VOC_VO = aux_fuel_type[0] * (1 / KWHRS_PER_HPHR) * teams_sheet.AuxEngineKWHoutperTrip;
                VOC_Total = VOC_WTP + VOC_VO + AUX_VOC_WTP + AUX_VOC_VO;

                AUX_CO_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 1) * teams_sheet.MMBTUinperTrip;
                AUX_CO_VO = aux_fuel_type[1] * (1 / KWHRS_PER_HPHR) * teams_sheet.AuxEngineKWHoutperTrip;
                CO_Total = CO_WTP + CO_VO + AUX_CO_WTP + AUX_CO_VO;

                AUX_NOx_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 2) * teams_sheet.MMBTUinperTrip;
                AUX_NOx_VO = aux_fuel_type[2] * (1 / KWHRS_PER_HPHR) * teams_sheet.AuxEngineKWHoutperTrip;
                NOx_Total = NOx_WTP + NOx_VO + AUX_NOx_WTP + AUX_NOx_VO;

                AUX_PM10_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 3) * teams_sheet.MMBTUinperTrip;
                AUX_PM10_VO = aux_fuel_type[3] * (1 / KWHRS_PER_HPHR) * teams_sheet.AuxEngineKWHoutperTrip;
                PM10_Total = PM10_WTP + PM10_VO + AUX_PM10_WTP + AUX_PM10_VO;

                AUX_PM25_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 4) * teams_sheet.MMBTUinperTrip;
                AUX_PM25_VO = aux_fuel_type[4] * (1 / KWHRS_PER_HPHR) * teams_sheet.AuxEngineKWHoutperTrip;
                PM25_Total = PM25_WTP + PM25_VO + AUX_PM25_WTP + AUX_PM25_VO;

                AUX_SOx_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 5) * teams_sheet.MMBTUinperTrip;
                AUX_SOx_VO = resourceDensity * resourceSulfurRatio * GRAMS_PER_KILOGRAM * (1 / GALLONS_PER_CUBIC_METER) * GRAMS_SOX_PER_GRAMS_S * teams_sheet.AuxEngineGALLONperTrip;
                SOx_Total = SOx_WTP + SOx_VO + AUX_SOx_WTP + AUX_SOx_VO;

                AUX_CH4_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 6) * teams_sheet.MMBTUinperTrip;
                AUX_CH4_VO = aux_fuel_type[5] * (1 / KWHRS_PER_HPHR) * teams_sheet.AuxEngineKWHoutperTrip;
                CH4_Total = CH4_WTP + CH4_VO + AUX_CH4_WTP + AUX_CH4_VO;

                double gramsOfFuel = (1 / resourceLowerHeatingValue) * resourceDensity * JOULES_PER_MMBTU * GRAMS_PER_KILOGRAM * teams_sheet.AuxEngineMMBTUinperTrip;
                AUX_CO2_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 8) * teams_sheet.MMBTUinperTrip;
                AUX_CO2_VO = gramsOfFuel * resourceCarbonRatio * (44 / 12);
                CO2_Total = CO2_WTP + CO2_VO + AUX_CO2_WTP + AUX_CO2_VO;

                AUX_N2O_WTP = APIcontroller.getResourceWTPEmissions(pathwayResults, 7) * teams_sheet.MMBTUinperTrip;
                AUX_N2O_VO = aux_fuel_type[6] * (1 / KWHRS_PER_HPHR) * teams_sheet.AuxEngineKWHoutperTrip;
                N2O_Total = N2O_WTP + N2O_VO + AUX_N2O_WTP + AUX_N2O_VO;
            }
            setLabels();
        }

        /// <summary>
        /// Parses a double to a string with 2 significant figures after the decimal
        /// </summary>
        /// <param name="resource">The value to be parsed</param>
        /// <returns>The parsed string</returns>
        private String parseResourceToString(double resource)
        {
            if (resource != 0) { return (resource).ToString("#.##"); }
            else { return "0.00"; }
        }
         */
#endregion

    }
}