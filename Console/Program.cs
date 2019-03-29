﻿using IntegrationSolution.Excel.Implementations;
using IntegrationSolution.Excel.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace Console
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {            
            var fileMain = @"..\..\Main2.xlsx";
            var file = @"..\..\export.xlsx";

            ICarOperations excel = new ExcelCarOperations(new OfficeOpenXml.ExcelPackage(new FileInfo(fileMain)));
            var data = excel.GetVehicles();

            ICarOperations ex = new ExcelCarOperations(new OfficeOpenXml.ExcelPackage(new FileInfo(file)));
            
            for (int i = 0; i < data.Count(); i++)
            {
                var v = data.ElementAtOrDefault(i);
                if (v != null)
                {
                    ex.SetFieldsOfVehicleByAvaliableData(ref v);
                }
            }           

            excel.WriteInHeadersAndDataForTotalResult(data.ToList());
            excel.WriteInTotalResultOfEachStructure(data.ToList());
            (excel as IExcel)?.Save();

            for (int i = 0; i < data.Count(); i++)
            {
                var obj = data.ElementAt(i);
                System.Console.WriteLine(obj.StateNumber + "\t" + obj.UnitModel);
                System.Console.WriteLine($"Total mileage: {obj.TripResulted?.TotalMileage}");

                var dangerTrips = obj.TripsWithMileageDeviation();
                if (dangerTrips?.Count > 0)
                {
                    foreach (var item in dangerTrips)
                    {
                        System.Console.WriteLine("\t\tDeviation km:\t" + item.TotalMileage + "km");
                    }
                }

                var dangerTripsMoto = obj.TripsWithMotoHoursDeviation();
                if (dangerTripsMoto?.Count > 0)
                {
                    foreach (var item in dangerTripsMoto)
                    {
                        System.Console.WriteLine("\t\tDeviation moto:\t" + item.MotoHoursIndicationsAtAll + "hours");
                    }
                }
            }

            System.Console.WriteLine("Count:\t" + data.Count());
        }
    }
}
