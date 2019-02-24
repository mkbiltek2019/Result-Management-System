using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class ExcelWriter
    {
        public static ExcelPackage CreateResultExcel(List<Result> Results)
        {
            if (Results == null) return null;

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            //workSheet.DefaultRowHeight = 12;
            //Header of table 
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "Sl. No.";
            workSheet.Cells[1, 2].Value = "Student Id";
            workSheet.Cells[1, 3].Value = "Reg. No.";
            workSheet.Cells[1, 4].Value = "Name";
            workSheet.Cells[1, 5].Value = "GPA";
            workSheet.Cells[1, 6].Value = "CGPA";
            workSheet.Cells[1, 7].Value = "CCH";
            workSheet.Cells[1, 8].Value = "REMARKS";

            int recordIndex = 2, sl = 1;

            foreach (Result Result in Results)
            {
                int HightRatioNm = 1, HightRatioRm = 1, Hight = 20;

                workSheet.Cells[recordIndex, 1].Value = sl;
                workSheet.Cells[recordIndex, 2].Value = Result.StudentId;
                workSheet.Cells[recordIndex, 3].Value = Result.RegNo;
                string Name = Result.Name;
                int NameLen = Name.Length;
                if (NameLen > 14) HightRatioNm = NameLen / 15;
                workSheet.Cells[recordIndex, 4].Value = Name;
                workSheet.Cells[recordIndex, 5].Value = Result.GPA;
                workSheet.Cells[recordIndex, 6].Value = Result.CGPA;
                workSheet.Cells[recordIndex, 7].Value = Result.CCH;
                string Remakrs = Result.Remarks;
                if (Remakrs == null || Remakrs == "") Remakrs = "Passsed";
                else
                {
                    Remakrs = "F in " + Remakrs;
                    int RemarksLen = Remakrs.Length;

                    if (RemarksLen > 24) HightRatioRm = RemarksLen / 24;
                }
                workSheet.Cells[recordIndex, 8].Value = Remakrs;

                Hight = (HightRatioNm > HightRatioRm) ? Hight * HightRatioNm : Hight * HightRatioRm;

                workSheet.Row(recordIndex).Height = Hight;
                workSheet.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                recordIndex++;
                sl++;
            }

            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();
            workSheet.Column(6).AutoFit();
            workSheet.Column(7).AutoFit();
            workSheet.Column(8).AutoFit();

            return excel;
        }
    }
}