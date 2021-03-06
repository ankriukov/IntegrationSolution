﻿using Integration.ModuleGUI.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Integration.ModuleGUI.ViewModels
{
    public class FinalViewModel : VMLocalBase
    {
        public FinalViewModel(IUnityContainer container, IEventAggregator ea) : base(container, ea)
        {
            CanGoBack = true;
            CanGoNext = false;
            this.Title = "Конец работы";
        }

        public override bool MoveBack()
        {
            return true;
        }

        public override async Task<bool> MoveNext()
        {
            return true;
        }
    }
}
