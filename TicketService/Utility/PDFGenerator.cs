using BookingService.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Utility
{
    public static class PDFGenerator
    {
        public static string GenerateTicket(BookingModel model)
        {
            //Create document  
            Document doc = new Document();
            //Create PDF Table  
            PdfPTable tableLayout = new PdfPTable(5);
            //Create a PDF file in specific path  
            var filePathName = Path.Combine(Directory.GetCurrentDirectory(), "Downloads", model.PNR + "_" + DateTime.Now.Hour + "_" + "_Ticket.pdf");
            PdfWriter.GetInstance(doc, new FileStream(filePathName, FileMode.Create));
            //Open the PDF document  
            doc.Open();
            //Add Content to PDF  
            doc.Add(Add_Content_To_PDF(tableLayout, model));
            // Closing the document  
            doc.Close();
            return filePathName;
        }

        private static PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, BookingModel model)
        {
            float[] headers = { 20, 20, 30, 30, 20 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 80; //Set the PDF File witdh percentage  
                                              //Add Title to the PDF file at the top  
            tableLayout.AddCell(new PdfPCell(new Phrase("Flight Booking Form", new Font(Font.HELVETICA, 16, 1, new BaseColor(153, 51, 0))))
            {
                Colspan = 5,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase("Email Address", new Font(Font.HELVETICA, 12, 1, BaseColor.Black)))
            {
                Colspan = 2,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_LEFT
            });
            tableLayout.AddCell(new PdfPCell(new Phrase(model.Email, new Font(Font.HELVETICA, 12, 1, BaseColor.Black)))
            {
                Colspan = 3,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase("Journey Date", new Font(Font.HELVETICA, 12, 1, BaseColor.Black)))
            {
                Colspan = 2,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_LEFT
            });
            tableLayout.AddCell(new PdfPCell(new Phrase(model.BookingDate.ToString(), new Font(Font.HELVETICA, 12, 1, BaseColor.Black)))
            {
                Colspan = 3,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase("PNR Number", new Font(Font.HELVETICA, 12, 1, BaseColor.Black)))
            {
                Colspan = 2,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_LEFT
            });
            tableLayout.AddCell(new PdfPCell(new Phrase(model.PNR.ToString(), new Font(Font.HELVETICA, 12, 1, BaseColor.Black)))
            {
                Colspan = 3,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase("Passenger Details", new Font(Font.HELVETICA, 14, 1, BaseColor.DarkGray)))
            {
                Colspan = 5,
                Border = 0,
                PaddingBottom = 20,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            //Add header  
            AddCellToHeader(tableLayout, "Passenger Name");
            AddCellToHeader(tableLayout, "Age");
            AddCellToHeader(tableLayout, "Gender");
            AddCellToHeader(tableLayout, "Meal Type");
            AddCellToHeader(tableLayout, "Seat No");
            //Add body  
            foreach (var item in model.BookingDetails)
            {
                AddCellToBody(tableLayout, item.Name);
                AddCellToBody(tableLayout, item.Age.ToString());
                AddCellToBody(tableLayout, item.Gender);
                AddCellToBody(tableLayout, item.MealType);
                AddCellToBody(tableLayout, item.SeatNo.ToString());
            }

            return tableLayout;
        }
        // Method to add single cell to the header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.HELVETICA, 8, 1, BaseColor.White)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5,
                BackgroundColor = new BaseColor(0, 51, 102)
            });
        }
        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.HELVETICA, 8, 1, BaseColor.Black)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5,
                BackgroundColor = BaseColor.White
            });
        }

        //public static string GetPDFTicketHTML(BookingModel booking)
        //{
        //    //var employees = DataStorage.GetAllEmployess();
        //    var sb = new StringBuilder();
        //    sb.Append(@"
        //                <html>
        //                    <head>
        //                    </head>
        //                    <body>
        //                        <div class='header'><h1>Flight Ticket</h1></div>
        //                        <table align='center'>
        //                            <tr>
        //                                <th>Name</th>
        //                                <th>Age</th>
        //                                <th>Gender</th>
        //                                <th>Meal Type</th>
        //                                <th>Seat No</th>
        //                            </tr>");
        //    foreach (var item in booking.BookingDetails)
        //    {
        //        sb.AppendFormat(@"<tr>
        //                            <td>{0}</td>
        //                            <td>{1}</td>
        //                            <td>{2}</td>
        //                            <td>{3}</td>
        //                            <td>{4}</td>
        //                          </tr>", item.Name, item.Age, item.Gender, item.MealType, item.SeatNo);
        //    }
        //    sb.Append(@"
        //                        </table>
        //                    </body>
        //                </html>");
        //    return sb.ToString();
        //}

    }
}
