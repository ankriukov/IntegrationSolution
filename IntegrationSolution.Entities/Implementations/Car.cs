﻿using IntegrationSolution.Common.Converters;
using IntegrationSolution.Entities.Interfaces;
using IntegrationSolution.Entities.SelfEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IntegrationSolution.Entities.Implementations
{
    public class Car : IVehicleSAP
    {
        public string UnitModel { get; set; }
        public string UnitNumber { get; set; }
        public string Type { get; set; }
        public string Department { get; set; }
        public string StructureName { get; set; }

        private string _stateNumber;
        public string StateNumber { get => _stateNumber; set => _stateNumber = StateNumberConverter.ToStateNumber(value); }

        public TripSAP TripResulted { get => GetTripResulted(); }
        public ICollection<TripSAP> Trips { get; set; }

        public int? CountTrips => Trips != null ? Trips.Count : (int?)null;

        public Car()
        {
            Trips = new Collection<TripSAP>();
        }


        private TripSAP GetTripResulted()
        {
            if (Trips == null || Trips.Count == 0)
                return null;

            TripSAP trip = new TripSAP(new CompareIndicator<double>());

            foreach (var item in Trips)
            {
                trip.TotalMileage += item.TotalMileage;
                trip.MotoHoursIndicationsAtAll += 
                    (item.MotoHoursIndicationsAtAll == 0 && 
                    item.DepartureMotoHoursIndications != item.ReturnMotoHoursIndications &&
                    item.DepartureMotoHoursIndications != 0) ?
                    item.ReturnMotoHoursIndications - item.DepartureMotoHoursIndications : item.MotoHoursIndicationsAtAll;

                foreach (var fuel in item.FuelDictionary)
                {
                    if (!trip.FuelDictionary.ContainsKey(fuel.Key))
                        trip.FuelDictionary.Add(fuel.Key, new FuelBase(fuel.Key.ToString()));

                    trip.FuelDictionary[fuel.Key].ConsumptionActual += fuel.Value.ConsumptionActual;
                }

            }

            return trip;
        }

        public override string ToString()
        {
            return this.StateNumber;
        }

        public int CompareTo(object obj)
        {
            IVehicleSAP vehicle = obj as IVehicleSAP;
            if(vehicle == null)
                throw new Exception("Невозможно сравнить два объекта");

            if (vehicle.TripResulted == null)
                return -1;
            else if (this.TripResulted == null)
                return 1;

            return this.TripResulted.TotalMileage.CompareTo(vehicle.TripResulted.TotalMileage);
        }
    }
}
