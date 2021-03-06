﻿using IntegrationSolution.Entities.Interfaces;
using System;
using System.Collections.Generic;

namespace IntegrationSolution.Excel.Interfaces
{
    public interface ICarOperations : IDisposable
    {
        /// <summary>
        /// This function gets all IVehicleSAP-objects from excel file which is storage of cars.
        /// Fill next properties of object: Тип, Гос.знак единицы оборудования, Марка
        /// </summary>
        /// <exception cref=""></exception>
        /// <returns>IEnumerable of IVehicleSAP</returns>
        IEnumerable<IVehicleSAP> GetVehicles();


        /// <summary>
        /// Fill by Trips and Fuel
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        bool SetFieldsOfVehicleByAvaliableData(ref IVehicleSAP vehicle);


        /// <summary>
        /// Add Headers and write data of each car
        /// </summary>
        /// <param name="vehicles"></param>
        void WriteInHeadersAndDataForTotalResult(ICollection<IVehicleSAP> vehicles);


        void WriteInTotalResultOfEachStructure(ICollection<IVehicleSAP> vehicles);
    }
}
