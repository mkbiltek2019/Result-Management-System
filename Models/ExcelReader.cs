using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class ExcelReader
    {
        public ExcelReader()
        {

        }

        public List<StudentInfo> readStuInfoFromExcel(HttpPostedFileBase file)
        {
            List<StudentInfo> stuInfo = new List<StudentInfo>();
            //byte[] fileBytes = new byte[file.ContentLength];

            //var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
            using (var package = new ExcelPackage(file.InputStream))
            {
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;

                try
                {
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var info = new StudentInfo();
                        info.Name = workSheet.Cells[rowIterator, 1].Value.ToString();
                        info.StudentId = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value);
                        info.Reg = Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value);
                        info.Faculty = workSheet.Cells[rowIterator, 4].Value.ToString();
                        info.Session = workSheet.Cells[rowIterator, 5].Value.ToString();
                        info.Regularity = workSheet.Cells[rowIterator, 6].Value.ToString();
                        info.Hall = workSheet.Cells[rowIterator, 7].Value.ToString();
                        info.Blood = workSheet.Cells[rowIterator, 8].Value.ToString();
                        info.Sex = workSheet.Cells[rowIterator, 9].Value.ToString();
                        info.Fathers_name = workSheet.Cells[rowIterator, 10].Value.ToString();
                        info.Mothers_name = workSheet.Cells[rowIterator, 11].Value.ToString();
                        info.Phone = workSheet.Cells[rowIterator, 12].Value.ToString();
                        info.Email = workSheet.Cells[rowIterator, 13].Value.ToString();
                        info.Nationality = workSheet.Cells[rowIterator, 14].Value.ToString();
                        info.Religion = workSheet.Cells[rowIterator, 15].Value.ToString();

                        stuInfo.Add(info);
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }

            }

            return stuInfo;
        }

        public List<Marks> readMarksFromExcel(HttpPostedFileBase file)
        {
            List<Marks> marks = new List<Marks>();
            //byte[] fileBytes = new byte[file.ContentLength];

            //var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
            using (var package = new ExcelPackage(file.InputStream))
            {
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;

                try
                {
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var mark = new Marks();
                        mark.StudentId = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value);
                        mark.RegNo = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value);
                        try
                        {
                            mark.Mid = Convert.ToSingle(workSheet.Cells[rowIterator, 3].Value);
                        }
                        catch (Exception ex)
                        {
                            mark.Mid = -1;
                        }
                        try
                        {
                            mark.Attendence = Convert.ToSingle(workSheet.Cells[rowIterator, 4].Value);
                        }
                        catch (Exception ex)
                        {
                            mark.Attendence = -1;
                        }
                        try
                        {
                            mark.Assignment = Convert.ToSingle(workSheet.Cells[rowIterator, 5].Value);
                        }
                        catch (Exception ex)
                        {
                            mark.Assignment = -1;
                        }
                        try
                        {
                            mark.Final = Convert.ToSingle(workSheet.Cells[rowIterator, 6].Value);
                        }
                        catch (Exception ex)
                        {
                            mark.Final = -1;
                        }

                        marks.Add(mark);
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }

            }

            return marks;
        }
    }
}