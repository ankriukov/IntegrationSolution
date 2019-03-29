﻿using Integration.ModuleGUI.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Integration.ModuleGUI.ViewModels
{
    public class LoadingFilesViewModel : VMLocalBase
    {
        public LoadingFilesViewModel(IUnityContainer container, IEventAggregator ea) : base(container, ea)
        {
            this.Title = "Загрузка файлов";
            LoadFileCommand = new DelegateCommand<string>(Load);            
        }


        public override async Task<bool> MoveNext()
        {
            var wnd = (MetroWindow)Application.Current.MainWindow;
            var progress = await wnd.ShowMessageAsync("Please wait...", "Progress message");
            //System.Threading.Thread.Sleep(5000);
            var exception = ModuleData.TryCreateObject();

            if (exception == null)
            {
                this.IsFinished = true;
                this.Error = new IntegrationSolution.Common.Entities.Error()
                {
                    IsError = false,
                    ErrorDescription = "Отлично!"
                };
            }
            else
            {
                this.Error = new IntegrationSolution.Common.Entities.Error()
                {
                    IsError = true,
                    ErrorDescription = exception.Message
                };
            }

            base.NotifyOnUpdateEvents();

            //if (progress.IsOpen)
            //    await progress.CloseAsync();

            return (exception == null) ? true : false;
        }


        public override bool MoveBack() => this.CanGoBack;


        private bool CheckAbilityForVisibilityMoveNextBtn()
        {
            if (File.Exists(ModuleData.PathToMainFile) && File.Exists(ModuleData.PathToPathListFile))
                return true;

            return false;
        }


        #region Commands
        public DelegateCommand<string> LoadFileCommand { get; private set; }
        protected async void Load(string NameControl)
        {
            var wnd = (MetroWindow)Application.Current.MainWindow;
            var progress = await wnd.ShowMessageAsync("Please wait...", "Progress message");
            if (string.IsNullOrWhiteSpace(NameControl))
                return;

            bool IsWithoutExceptions = true;
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckPathExists = true,
                DefaultExt = ".xlsx | .xls",
                Filter = "Excel document (.xlsx)|*.xlsx|Excel document (.xls)|*.xls|All files (*.*)|*.*"
            };

            if (fileDialog.ShowDialog() != true)
                return;

            if (!File.Exists(fileDialog.FileName))
            {
                this.Error = new IntegrationSolution.Common.Entities.Error()
                {
                    IsError = true,
                    ErrorDescription = $"Выберите существующий файл."
                };
                IsWithoutExceptions = false;
            }

            var ext = Path.GetExtension(fileDialog.FileName).ToLowerInvariant();
            if (ext != ".xls" && ext != ".xlsx")
            {
                this.Error = new IntegrationSolution.Common.Entities.Error()
                {
                    IsError = true,
                    ErrorDescription = $"Выберите файл с расширением \".xls\" или \".xlsx\"."
                };
                IsWithoutExceptions = false;
            }
            else
                this.Error = null;

            switch (NameControl)
            {
                case "loadMainFile":
                    this.ModuleData.PathToMainFile = fileDialog.FileName;
                    break;

                case "loadPathListFile":
                    this.ModuleData.PathToPathListFile = fileDialog.FileName;
                    break;

                default:
                    break;
            }

            if (IsWithoutExceptions && CheckAbilityForVisibilityMoveNextBtn())
                CanGoNext = true;
            else
                CanGoNext = false;

            base.NotifyOnUpdateEvents();
        }
        #endregion
    }
}
