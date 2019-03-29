﻿using IntegrationSolution.Common.Converters;
using IntegrationSolution.Common.Enums;
using IntegrationSolution.Entities.Helpers;
using IntegrationSolution.Entities.Implementations;
using IntegrationSolution.Entities.Interfaces;
using IntegrationSolution.Entities.SelfEntities;
using IntegrationSolution.Excel.Common;
using IntegrationSolution.Excel.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationSolution.Excel.Implementations
{
    public class ExcelCarOperations : ExcelBase, ICarOperations
    {
        private Dictionary<string, ExcelCellAddress> _tripsAddress;


        #region Constructors
        public ExcelCarOperations(ExcelPackage excelPackage) : base(excelPackage)
        {
            _tripsAddress = new Dictionary<string, ExcelCellAddress>();
            TryInitializeAll();
        }
        #endregion


        #region Initialize headers
        private bool TryInitializeAll()
        {
            if (InitializeDriverHeaders() && InitializeFuelHeaders()
                && InitializeIndicatorsHeaders() && InitializeMoveDateTimeCheckHeaders())
                return true;
            return false;
        }


        private bool InitializeFuelHeaders()
        {
            var fuelAddress = StaticHelper.GetHeadersAddress(this, HeaderNames.DepartureBalanceGas, HeaderNames.DepartureBalanceDisel, HeaderNames.DepartureBalanceLPG);
            if (fuelAddress?.Count == 0)
                return false;

            fuelAddress = StaticHelper.GetHeadersAddress(this,
                HeaderNames.DepartureBalanceGas,
                HeaderNames.ReturnBalanceGas,
                HeaderNames.ConsumptionGasActual,
                HeaderNames.ConsumptionGasNormative,
                HeaderNames.ConsumptionGasSavingsOrOverruns,

                HeaderNames.DepartureBalanceDisel,
                HeaderNames.ReturnBalanceDisel,
                HeaderNames.ConsumptionDiselActual,
                HeaderNames.ConsumptionDiselNormative,
                HeaderNames.ConsumptionDiselSavingsOrOverruns,

                HeaderNames.DepartureBalanceLPG,
                HeaderNames.ReturnBalanceLPG,
                HeaderNames.ConsumptionLPGActual,
                HeaderNames.ConsumptionLPGNormative,
                HeaderNames.ConsumptionLPGSavingsOrOverruns);

            // 15 - fuel columns
            if (fuelAddress.Count == 15)
            {
                _tripsAddress = _tripsAddress.Concat(fuelAddress).ToDictionary(e => e.Key, e => e.Value);
                return true;
            }

            return false;
        }


        private bool InitializeDriverHeaders()
        {
            var driverAddress = StaticHelper.GetHeadersAddress(this, HeaderNames.FullNameOfDriver, HeaderNames.NumberOfDriver);
            if (driverAddress?.Count == 0)
                return false;

            _tripsAddress = _tripsAddress.Concat(driverAddress).ToDictionary(e => e.Key, e => e.Value);
            return true;
        }


        private bool InitializeIndicatorsHeaders()
        {
            var driverAddress = StaticHelper.GetHeadersAddress(this,
                HeaderNames.DepartureOdometerValue,
                HeaderNames.ReturnOdometerValue,
                HeaderNames.TotalMileage,
                HeaderNames.DepartureMotoHoursIndications,
                HeaderNames.ReturnMotoHoursIndications,
                HeaderNames.MotoHoursIndicationsAtAll);
            if (driverAddress?.Count == 0)
                return false;

            _tripsAddress = _tripsAddress.Concat(driverAddress).ToDictionary(e => e.Key, e => e.Value);
            return true;
        }


        private bool InitializeMoveDateTimeCheckHeaders()
        {
            var driverAddress = StaticHelper.GetHeadersAddress(this,
                HeaderNames.DepartureFromGarageDate,
                HeaderNames.DepartureFromGarageTime,
                HeaderNames.ReturnToGarageDate,
                HeaderNames.ReturnToGarageTime,
                HeaderNames.TimeOnDutyAtAll);
            if (driverAddress?.Count == 0)
                return false;

            _tripsAddress = _tripsAddress.Concat(driverAddress).ToDictionary(e => e.Key, e => e.Value);
            return true;
        }
        #endregion


        public IEnumerable<IVehicle> GetVehicles()
        {
            ICollection<IVehicle> cars = new List<IVehicle>();
            try
            {
                IDictionary<string, ExcelCellAddress> headers = StaticHelper.GetHeadersAddress(
                    this,
                    HeaderNames.TypeOfVehicle,
                    HeaderNames.ModelOfVehicle,
                    HeaderNames.StateNumber,
                    HeaderNames.Departments);

                headers.Add(StaticHelper.GetSameHeadersAddress(this, HeaderNames.PartOfStructureNameForResult).FirstOrDefault());

                for (int row = headers.First().Value.Row + 1; row < this.EndCell.Row; row++)
                {
                    IVehicle vehicle = new Car();
                    foreach (var item in headers)
                    {
                        try
                        {
                            switch (item.Key)
                            {
                                case nameof(HeaderNames.TypeOfVehicle):
                                    vehicle.Type = this.WorkSheet.Cells[row, item.Value.Column].Text;
                                    break;

                                case nameof(HeaderNames.ModelOfVehicle):
                                    vehicle.UnitModel = this.WorkSheet.Cells[row, item.Value.Column].Text;
                                    break;

                                case nameof(HeaderNames.StateNumber):
                                    vehicle.StateNumber = this.WorkSheet.Cells[row, item.Value.Column].Text;
                                    break;

                                case nameof(HeaderNames.Departments):
                                    vehicle.Department = this.WorkSheet.Cells[row, item.Value.Column].Text;
                                    break;

                                case nameof(HeaderNames.PartOfStructureNameForResult):
                                    vehicle.StructureName = this.WorkSheet.Cells[row, item.Value.Column].Text;
                                    break;

                                default:
                                    continue;
                            }
                        }
                        catch (Exception ex)
                        { throw ex; }
                    }
                    if (!string.IsNullOrWhiteSpace(vehicle.StateNumber))
                        cars.Add(vehicle);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return cars;
        }


        public bool SetFieldsOfVehicleByAvaliableData(ref IVehicle vehicle)
        {
            if (StaticHelper.GetHeadersAddress(this,
                HeaderNames.UnitNumber,
                HeaderNames.StateNumber,
                HeaderNames.PathListStatus).Count == 0
                || vehicle == null)
                return false;

            vehicle.Trips = GetTripsByStateNumber(vehicle.StateNumber)?.ToList();

            return true;
        }


        public IEnumerable<Trip> GetTripsByStateNumber(string StateNumber)
        {
            var rows = StaticHelper.GetRowsWithValue(this, StateNumber, HeaderNames.StateNumber);
            if (rows.Count() == 0)
                return null;

            List<Trip> result = new List<Trip>();
            foreach (var row in rows)
            {
                try
                {
                    Trip trip = new Trip
                    {
                        FuelDictionary = StaticHelper.GetFuelDataByRow(this, row.Row, _tripsAddress).ToDictionary(x => x.Key, y => y.Value),
                        Driver = StaticHelper.GetDriverFromRow(this, row.Row, _tripsAddress)
                    };

                    #region GetHeaders of indicators
                    // Indicators: odometr, mileage...
                    var headerDepartureOdometerValue = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.DepartureOdometerValue))).FirstOrDefault();
                    var headerReturnOdometerValue = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.ReturnOdometerValue))).FirstOrDefault();
                    var headerTotalMileage = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.TotalMileage))).FirstOrDefault();
                    var headerDepartureMotoHoursIndications = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.DepartureMotoHoursIndications))).FirstOrDefault();
                    var headerReturnMotoHoursIndications = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.ReturnMotoHoursIndications))).FirstOrDefault();
                    var headerMotoHoursIndicationsAtAll = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.MotoHoursIndicationsAtAll))).FirstOrDefault();

                    // Inidicators: date, time
                    var headerDepartureFromGarageDate = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.DepartureFromGarageDate))).FirstOrDefault();
                    var headerDepartureFromGarageTime = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.DepartureFromGarageTime))).FirstOrDefault();
                    var headerReturnToGarageDate = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.ReturnToGarageDate))).FirstOrDefault();
                    var headerReturnToGarageTime = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.ReturnToGarageTime))).FirstOrDefault();
                    var headerTimeOnDutyAtAll = _tripsAddress.Where(x => x.Key.Contains(nameof(HeaderNames.TimeOnDutyAtAll))).FirstOrDefault();
                    #endregion

                    #region SetValues of car
                    // Set values for odometr
                    if (headerDepartureOdometerValue.Value != null)
                        trip.DepartureOdometerValue = WorkSheet.Cells[row.Row, headerDepartureOdometerValue.Value.Column].Text.ToDouble();

                    if (headerReturnOdometerValue.Value != null)
                        trip.ReturnOdometerValue = WorkSheet.Cells[row.Row, headerReturnOdometerValue.Value.Column].Text.ToDouble();

                    if (headerTotalMileage.Value != null)
                        trip.TotalMileage = WorkSheet.Cells[row.Row, headerTotalMileage.Value.Column].Text.ToDouble();

                    if (headerDepartureMotoHoursIndications.Value != null)
                        trip.DepartureMotoHoursIndications = WorkSheet.Cells[row.Row, headerDepartureMotoHoursIndications.Value.Column].Text.ToDouble();

                    if (headerReturnMotoHoursIndications.Value != null)
                        trip.ReturnMotoHoursIndications = WorkSheet.Cells[row.Row, headerReturnMotoHoursIndications.Value.Column].Text.ToDouble();

                    if (headerMotoHoursIndicationsAtAll.Value != null)
                        trip.MotoHoursIndicationsAtAll = WorkSheet.Cells[row.Row, headerMotoHoursIndicationsAtAll.Value.Column].Text.ToDouble();

                    // Start set values for date and time
                    if (headerDepartureFromGarageDate.Value != null)
                        trip.DepartureFromGarageDate = WorkSheet.Cells[row.Row, headerDepartureFromGarageDate.Value.Column].Text;

                    if (headerDepartureFromGarageTime.Value != null)
                        trip.DepartureFromGarageTime = WorkSheet.Cells[row.Row, headerDepartureFromGarageTime.Value.Column].Text;

                    if (headerReturnToGarageDate.Value != null)
                        trip.ReturnToGarageDate = WorkSheet.Cells[row.Row, headerReturnToGarageDate.Value.Column].Text;

                    if (headerReturnToGarageTime.Value != null)
                        trip.ReturnToGarageTime = WorkSheet.Cells[row.Row, headerReturnToGarageTime.Value.Column].Text;

                    if (headerTimeOnDutyAtAll.Value != null)
                        trip.TimeOnDutyAtAll = WorkSheet.Cells[row.Row, headerTimeOnDutyAtAll.Value.Column].Text;
                    #endregion

                    result.Add(trip);
                }
                catch (Exception ex)
                { throw ex; }
            }
            return result;
        }


        public void WriteInHeadersAndDataForTotalResult(ICollection<IVehicle> vehicles)
        {
            StaticHelper.WriteVehicleDataAndHeaders(this, vehicles, 
                HeaderNames.TotalMileageResult, 
                HeaderNames.TotalJobDoneResult,
                HeaderNames.ConsumptionGasActualResult,
                HeaderNames.ConsumptionDieselActualResult, 
                HeaderNames.ConsumptionLPGActualResult);
        }


        public void WriteInTotalResultOfEachStructure(ICollection<IVehicle> vehicles)
        {
            var total = GetTotal(vehicles);

            if (total.Count == 0)
                return;

            StaticHelper.WriteSummaryFormula(this, total,
                HeaderNames.TotalMileageResult,
                HeaderNames.TotalJobDoneResult,
                HeaderNames.ConsumptionGasActualResult,
                HeaderNames.ConsumptionDieselActualResult,
                HeaderNames.ConsumptionLPGActualResult);

        }


        /// <summary>
        /// Get total values of each Structure (Структурные подразделения)
        /// </summary>
        /// <param name="vehicles"></param>
        /// <returns>Dictionary, where key is Structure, value is TotalIndicators</returns>
        private IDictionary<string, TotalIndicators> GetTotal(ICollection<IVehicle> vehicles)
        {
            var structures = vehicles.ToLookup(x => x.StructureName);
            Dictionary<string, TotalIndicators> summary = new Dictionary<string, TotalIndicators>();

            foreach (var structure in structures)
            {
                TotalIndicators total = new TotalIndicators();

                foreach (var auto in structure)
                {
                    if (auto.TripResulted == null)
                        continue;

                    total.Mileage += auto.TripResulted.TotalMileage;
                    total.MotoJob += auto.TripResulted.MotoHoursIndicationsAtAll;
                    total.Gas += auto.TripResulted.FuelDictionary[FuelEnum.Gas].ConsumptionActual;
                    total.LPG += auto.TripResulted.FuelDictionary[FuelEnum.LPG].ConsumptionActual;
                    total.Disel += auto.TripResulted.FuelDictionary[FuelEnum.Disel].ConsumptionActual;
                }
                summary.Add(structure.Key, total);
            }

            return summary;
        }
    }
}