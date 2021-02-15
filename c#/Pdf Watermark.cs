    public bool add(string sourcePath,string waterMark,string fontFamily,int fontSize,string fontColor)
        {
            bool result = true;
            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".pdf")).ToArray();
            string ResultPath = Path.Combine(sourcePath, "result");
           
            if (!Directory.Exists(ResultPath))
                Directory.CreateDirectory(ResultPath);

            string watermark = "";
            for (int i = waterMark.Length - 1; i >= 0; i--)
            {
                watermark += waterMark[i];
            }

            XPdfFontOptions op = new XPdfFontOptions(PdfFontEncoding.Unicode);
            XFont font = new XFont(fontFamily, fontSize, XFontStyle.Underline, op);
            XBrush brush = new XSolidBrush(XColor.FromName(fontColor));
            //XBrush brush = new XSolidBrush(XColor.FromArgb(128, 255, 0, 0));
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Near;


            foreach (string file in files)
            {

                try
                {
                   PdfDocument doc = PdfReader.Open(file);
                   string OutputFile = Path.Combine(ResultPath, Path.GetFileName(file).ToString());
                   foreach (PdfPage page in doc.Pages)
                    {
                        XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);
                        gfx.TranslateTransform(page.Width / 2, page.Height / 2);
                        gfx.RotateTransform(-Math.Atan(page.Height / page.Width) * 180 / Math.PI);
                        gfx.TranslateTransform(-page.Width / 2, -page.Height / 2);
                        XSize size = gfx.MeasureString(watermark, font);
                        page.Orientation = PdfSharp.PageOrientation.Portrait;
                        gfx.DrawString(watermark, font, brush, new XPoint((page.Width - size.Width) / 2, (page.Height - size.Height) / 2), format);
                    }
                    doc.Save(OutputFile);
                }
                catch
                {
                    return false;
                }
            }
            return result;
        }
