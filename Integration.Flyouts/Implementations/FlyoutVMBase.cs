﻿using Integration.Flyouts.Interfaces;
using MahApps.Metro.Controls;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Integration.Flyouts.Implementations
{
    public abstract class FlyoutVMBase : BindableBase, IFlyoutViewModel
    {
        #region Properties
        private string header;
        public string Header
        {
            get { return this.header; }
            set { SetProperty(ref this.header, value); }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return this.isOpen; }
            set { SetProperty(ref this.isOpen, value); }
        }

        private bool isModal;
        public bool IsModal
        {
            get { return this.isModal; }
            set { SetProperty(ref this.isModal, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set { SetProperty(ref this._isEnabled, value); }
        }

        private Position position;
        public Position Position
        {
            get { return this.position; }
            set { SetProperty(ref this.position, value); }
        }

        private Theme theme;
        public Theme Theme
        {
            get { return this.theme; }
            set { SetProperty(ref this.theme, value); }
        }

        private Visibility closeButtonVisibility;
        public Visibility CloseButtonVisibility
        {
            get { return this.closeButtonVisibility; }
            set { SetProperty(ref this.closeButtonVisibility, value); }
        }
        #endregion


        public FlyoutVMBase()
        {
            Position = Position.Right;
            Theme = Theme.Dark;
            CloseButtonVisibility = Visibility.Visible;
            IsEnabled = true;
        }
    }
}