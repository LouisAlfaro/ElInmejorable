Imports System.Drawing
Imports System.Drawing.Printing
Imports System.IO
Imports ZXing
Imports ZXing.Common

Public Class TicketPrinterBack

    Private _ticket As TicketBack
    Private _printerSettings As PrinterSettings
    Private _paperWidth As Integer
    Private _logoPath As String

    Public Sub New(ticket As TicketBack, Optional paperWidth As Integer = 80, Optional printerName As String = "", Optional logoPath As String = "")
        _ticket = ticket
        _paperWidth = paperWidth
        _printerSettings = New PrinterSettings()


        Dim defaultLogoPath As String = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png")
        'Logos
        If Not String.IsNullOrEmpty(logoPath) AndAlso File.Exists(logoPath) Then
            _logoPath = logoPath
        ElseIf File.Exists(defaultLogoPath) Then
            _logoPath = defaultLogoPath
        Else
            _logoPath = Nothing
        End If

        If Not String.IsNullOrEmpty(My.Settings.SelectedPrinter) AndAlso
    PrinterSettings.InstalledPrinters.Cast(Of String)().Contains(My.Settings.SelectedPrinter) Then
            _printerSettings.PrinterName = My.Settings.SelectedPrinter
        Else
            _printerSettings.PrinterName = New PrinterSettings().PrinterName ' Usar predeterminada
            Debug.WriteLine("Usando impresora predeterminada del sistema.")
        End If

    End Sub

    Public Sub PrintAutomatically()
        Try
            If PrinterSettings.InstalledPrinters.Count = 0 Then Throw New Exception("No hay impresoras instaladas.")
            If Not _printerSettings.IsValid Then Throw New Exception("La impresora seleccionada no es válida.")

            Dim pd As New PrintDocument()
            pd.PrinterSettings = _printerSettings
            Dim paperWidthHundredths As Integer = CInt(_paperWidth / 25.4 * 100)
            pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", paperWidthHundredths, 3000)
            pd.DefaultPageSettings.Margins = New Margins(10, 10, 5, 5)
            AddHandler pd.PrintPage, AddressOf PrintPageHandler
            pd.Print()
        Catch ex As Exception
            Throw New Exception($"Error al imprimir el ticket: {ex.Message}")
        End Try
    End Sub

    Private Sub PrintPageHandler(sender As Object, e As PrintPageEventArgs)
        Try
            Dim g As Graphics = e.Graphics
            Dim y As Integer = 10
            Dim marginLeft As Integer = 10
            Dim pageWidth As Integer = e.PageBounds.Width - marginLeft * 2
            Dim fontNormal As New Font("Consolas", If(_paperWidth = 58, 6, 8), FontStyle.Regular)
            Dim fontBold As New Font("Consolas", If(_paperWidth = 58, 6, 8), FontStyle.Bold)
            Dim logoWidth As Integer = If(_paperWidth = 58, 140, 220)
            Dim logoHeight As Integer = If(_paperWidth = 58, 55, 60)

            If My.Settings.ImprimirLogo AndAlso Not String.IsNullOrEmpty(_logoPath) AndAlso File.Exists(_logoPath) Then
                Dim logo As Image = Image.FromFile(_logoPath)
                Dim logoX As Single = (pageWidth - logoWidth) / 2
                g.DrawImage(logo, logoX, y, logoWidth, logoHeight)
                y += logoHeight + 10
            End If

            ' Encabezado
            g.DrawString($"Ticket:", fontNormal, Brushes.Black, marginLeft, y)
            g.DrawString((SanitizeText(_ticket.Numero)), fontNormal, Brushes.Black, pageWidth - 150, y)
            y += 15

            g.DrawString($"Fecha:", fontNormal, Brushes.Black, marginLeft, y)
            g.DrawString((SanitizeText(_ticket.Fecha)), fontNormal, Brushes.Black, pageWidth - 150, y)
            y += 15

            g.DrawString($"Hora:", fontNormal, Brushes.Black, marginLeft, y)
            g.DrawString((SanitizeText(_ticket.Hora)), fontNormal, Brushes.Black, pageWidth - 150, y)
            y += 15

            g.DrawString($"HoraSorteo:", fontNormal, Brushes.Black, marginLeft, y)
            g.DrawString((SanitizeText(_ticket.HoraSorteo)), fontNormal, Brushes.Black, pageWidth - 150, y)
            y += 15


            g.DrawString(New String("-"c, 40), fontBold, Brushes.Black, marginLeft, y)
            y += 15


            g.DrawString("Evento", fontBold, Brushes.Black, marginLeft, y)
            g.DrawString("Apuesta", fontBold, Brushes.Black, marginLeft + pageWidth \ 3, y)
            g.DrawString("Monto", fontBold, Brushes.Black, marginLeft + (pageWidth * 2) \ 3, y)
            y += 15


            Dim maxRows As Integer = Math.Max(Math.Max(_ticket.Evento.Count, _ticket.Apuesta.Count), _ticket.Opciones.Count)
            For i As Integer = 0 To maxRows - 1
                Dim evento As String = If(i < _ticket.Evento.Count, _ticket.Evento(i), "")
                Dim apuesta As String = If(i < _ticket.Apuesta.Count, _ticket.Apuesta(i), "")
                Dim monto As String = If(i < _ticket.Opciones.Count, _ticket.Opciones(i), "")

                g.DrawString(evento, fontNormal, Brushes.Black, marginLeft, y)
                g.DrawString(monto, fontNormal, Brushes.Black, marginLeft + pageWidth \ 3, y)
                g.DrawString(apuesta, fontNormal, Brushes.Black, marginLeft + (pageWidth * 2) \ 3, y)
                y += 15
            Next


            g.DrawString(New String("-"c, 40), fontBold, Brushes.Black, marginLeft, y)
            y += 15


            Dim totalText As String = $"Total: {_ticket.TotalMonto} BS.D"
            Dim totalTextSize As SizeF = g.MeasureString(totalText, fontBold)
            Dim totalTextX As Single = marginLeft + 40 + (pageWidth - totalTextSize.Width) / 2
            g.DrawString(totalText, fontBold, Brushes.Black, totalTextX, y)
            y += 15


            If Not String.IsNullOrEmpty(_ticket.Seguridad) Then
                Dim writer As New ZXing.OneD.Code128Writer()
                Dim matrix As ZXing.Common.BitMatrix = writer.encode(
        _ticket.Seguridad, ZXing.BarcodeFormat.CODE_128,
        If(_paperWidth = 58, 150, 200), If(_paperWidth = 58, 40, 50)
    )
                Dim barcodeBitmap As New Bitmap(matrix.Width, matrix.Height)


                For yPos As Integer = 0 To matrix.Height - 1
                    For x As Integer = 0 To matrix.Width - 1
                        barcodeBitmap.SetPixel(x, yPos, If(matrix(x, yPos), Color.Black, Color.White))
                    Next
                Next

                If _paperWidth = 58 Then
                    Dim securityFont As New Font("Consolas", 8, FontStyle.Bold)
                    Dim securityTextX As Single = (pageWidth - g.MeasureString(_ticket.Seguridad, securityFont).Width) / 2
                    g.DrawString(_ticket.Seguridad, securityFont, Brushes.Black, securityTextX, y)
                    y += 20
                End If

                ' Dibujar el código de barras
                Dim barcodeX As Single = (pageWidth - matrix.Width) / 2
                g.DrawImage(barcodeBitmap, barcodeX - 20, y, matrix.Width, matrix.Height)
                y += matrix.Height + 10

                If _paperWidth = 80 Then
                    ' En 80mm, el texto de seguridad se imprime debajo del código de barras
                    Dim securityFont As New Font("Consolas", 8, FontStyle.Bold)
                    Dim securityTextX As Single = (pageWidth - g.MeasureString(_ticket.Seguridad, securityFont).Width) / 2
                    g.DrawString(_ticket.Seguridad, securityFont, Brushes.Black, securityTextX, y)
                    y += 20
                End If
            End If


            e.HasMorePages = False
        Catch ex As Exception
            Throw New Exception($"Error en PrintPageHandler: {ex.Message}")
        End Try
    End Sub

    Private Function SanitizeText(input As String) As String
        If input Is Nothing Then Return String.Empty
        Return input.Replace(Chr(10), " ").Replace(Chr(13), " ").Trim()
    End Function
End Class
