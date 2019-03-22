﻿using IntegrationSolution.Excel.Common;
using IntegrationSolution.Excel.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationSolution.Excel.Implementations
{
    public abstract class ExcelBase : IExcel, IExcelBorders, IDisposable
    {
        #region Properties
        public ExcelPackage Excel { get; protected set; }
        public ExcelWorkbook Workbook { get; private set; }
        public ExcelWorksheet WorkSheet { get; private set; }
        public ExcelCellAddress StartCell { get; private set; }
        public ExcelCellAddress EndCell { get; private set; }
        #endregion

        #region Variables
        
        #endregion


        public ExcelBase(ExcelPackage excelPackage)
        {
            Excel = excelPackage;
            TryClearFromPathList();
        }


        public void TryClearFromPathList()
        {
            if (StaticHelper.GetHeadersAddress(this, HeaderNames.PathListStatus).Count == 0)
                return;

            var rows = StaticHelper.GetRowsWithValue(this,
                PathListData.PathListStatusDictionary[IntegrationSolution.Common.Enums.PathListStatusEnum.Miv],
                HeaderNames.PathListStatus);

            foreach (var item in rows)
            {
                try
                {
                    WorkSheet.DeleteRow(item.Row);
                }
                catch (Exception)
                { }
            }
        }


        /// <summary>
        /// This function is trying to open Excel and set the next values: workbook, worksheet, startCell, endCell
        /// </summary>
        public void TryOpen()
        {
            try
            {
                Workbook = Excel?.Workbook;
                WorkSheet = Workbook?.Worksheets.First();

                SetBorders();
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// This function sets borders: startCell, endCell
        /// </summary>
        public void SetBorders()
        {
            if (WorkSheet == null)
                TryOpen();

            StartCell = WorkSheet.Dimension.Start;
            EndCell = WorkSheet.Dimension.End;
        }


        public void Dispose()
        {
            Excel?.Dispose();
        }
    }
}
