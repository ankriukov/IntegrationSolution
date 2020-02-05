﻿using MahApps.Metro.Controls;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrationSolution.ShellGUI.ControlRegionAdapter
{
    public class FlyoutsControlRegionAdapter : RegionAdapterBase<FlyoutsControl>
    {
        public FlyoutsControlRegionAdapter(IRegionBehaviorFactory factory) : base(factory)
        { }


        protected override void Adapt(IRegion region, FlyoutsControl regionTarget)
        {
            region.ActiveViews.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (FrameworkElement element in e.NewItems)
                    {
                        Flyout flyout = new Flyout();
                        flyout.Content = element;
                        flyout.DataContext = element.DataContext;
                        regionTarget.Items.Add(flyout);
                    }
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}
