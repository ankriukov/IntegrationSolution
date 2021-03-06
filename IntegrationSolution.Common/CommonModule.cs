﻿using IntegrationSolution.Common.Entities;
using IntegrationSolution.Common.Implementations;
using IntegrationSolution.Common.Interfaces;
using IntegrationSolution.Common.Models;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationSolution.Common
{
    public class CommonModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<FuelPrice>();
            containerRegistry.RegisterSingleton<AppConfiguration>();
            containerRegistry.RegisterSingleton<SerializeConfigDTO>();
        }
    }
}
