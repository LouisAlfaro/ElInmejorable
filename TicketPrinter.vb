Imports System.Drawing
Imports System.Drawing.Printing
Imports ZXing
Imports ZXing.Common
Imports System.IO

Public Class TicketPrinter

    Private _ticket As Ticket
    Private _printerSettings As PrinterSettings
    Private _paperWidth As Integer
    Private _logoPath As String
    Private _usuario As String
    Private _mensajeFinal As String

    Public Sub New(ticket As Ticket, Optional paperWidth As Integer = 80, Optional printerName As String = "", Optional logoPath As String = "", Optional usuario As String = "", Optional mensajeFinal As String = "")
        _ticket = ticket
        _paperWidth = paperWidth
        _printerSettings = New PrinterSettings()
        _usuario = usuario
        _mensajeFinal = mensajeFinal

        Dim defaultLogoPath As String = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png")

        If Not String.IsNullOrEmpty(logoPath) AndAlso File.Exists(logoPath) Then
            _logoPath = logoPath
        ElseIf File.Exists(defaultLogoPath) Then
            _logoPath = defaultLogoPath
        Else
            _logoPath = Nothing
        End If

        If Not String.IsNullOrEmpty(printerName) AndAlso PrinterSettings.InstalledPrinters.Cast(Of String)().Contains(printerName) Then
            _printerSettings.PrinterName = printerName
        End If
    End Sub

    Public Sub PrintAutomatically()
        If My.Settings.IsTerminal Then
            Debug.WriteLine("Modo Terminal activo. No se imprimirá el ticket en PrintAutomatically.")
            Return
        End If

        Try
            If My.Settings.IsTerminal Then
                Throw New Exception("La impresión está deshabilitada en modo Terminal.")
            End If

            If PrinterSettings.InstalledPrinters.Count = 0 Then Throw New Exception("No hay impresoras instaladas.")
            If Not _printerSettings.IsValid Then Throw New Exception("La impresora seleccionada no es válida.")

            Dim pd As New PrintDocument()
            pd.PrinterSettings = _printerSettings
            Dim paperWidthHundredths As Integer = CInt(_paperWidth / 25.4 * 100)
            pd.DefaultPageSettings.PaperSize = New PaperSize("Custom", paperWidthHundredths, 3000)
            pd.DefaultPageSettings.Margins = New Margins(If(_paperWidth = 58, 5, 10), If(_paperWidth = 58, 5, 10), 5, 5)
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
            Dim marginLeft As Integer = If(_paperWidth = 58, 5, 10)
            Dim pageWidth As Integer = e.PageBounds.Width - marginLeft * 2
            Dim fontNormal As New Font("Consolas", If(_paperWidth = 58, 6, 8), FontStyle.Regular)
            Dim fontBold As New Font("Consolas", If(_paperWidth = 58, 6, 8), FontStyle.Bold)
            Dim logoWidth As Integer = If(_paperWidth = 58, 140, 220)
            Dim logoHeight As Integer = If(_paperWidth = 58, 70, 130)


            If My.Settings.ImprimirLogo AndAlso Not String.IsNullOrEmpty(_logoPath) AndAlso File.Exists(_logoPath) Then
                Dim logo As Image = Image.FromFile(_logoPath)
                Dim logoX As Single = (pageWidth - logoWidth) / 2 - 10
                g.DrawImage(logo, logoX, y, logoWidth, logoHeight)
                y += logoHeight + 10
            End If


            Dim separator As String = New String("-"c, _paperWidth)
            g.DrawString(separator, fontBold, Brushes.Black, marginLeft, y)
            y += 15


            If Not String.IsNullOrEmpty(_ticket.IDDeTicket) Then
                g.DrawString($"ID Ticket:", fontNormal, Brushes.Black, marginLeft, y)
                g.DrawString(SanitizeText(_ticket.IDDeTicket), fontNormal, Brushes.Black, pageWidth - 100, y)
                y += 15
            End If

            If My.Settings.ImprimirUsuario AndAlso Not String.IsNullOrEmpty(_usuario) Then
                g.DrawString($"Emitido por:", fontNormal, Brushes.Black, marginLeft, y)
                g.DrawString(SanitizeText(_usuario), fontNormal, Brushes.Black, pageWidth - 100, y)
                y += 15
            End If

            If Not String.IsNullOrEmpty(_ticket.Fecha) Then
                g.DrawString($"Impreso en:", fontNormal, Brushes.Black, marginLeft, y)
                g.DrawString(SanitizeText(_ticket.Fecha), fontNormal, Brushes.Black, pageWidth - 150, y)
                y += 15
            End If

            If Not String.IsNullOrEmpty(_ticket.TipoDeApuesta) Then
                g.DrawString($"Tipo de Apuesta:", fontNormal, Brushes.Black, marginLeft, y)
                g.DrawString(SanitizeText(_ticket.TipoDeApuesta), fontNormal, Brushes.Black, pageWidth - 100, y)
                y += 15
            End If

            g.DrawString(separator, fontBold, Brushes.Black, marginLeft, y)
            y += 15


            For Each apuesta In _ticket.Apuestas
                If Not String.IsNullOrEmpty(apuesta.Juego) Then
                    g.DrawString($"Juego: {SanitizeText(apuesta.Juego)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.IDDeLaRonda) Then
                    g.DrawString($"ID de la Ronda: {SanitizeText(apuesta.IDDeLaRonda)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Numero) Then
                    g.DrawString($"Apostado a: {SanitizeText(apuesta.Numero)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Ganancia) Then
                    g.DrawString($"Cuota: {SanitizeText(apuesta.Ganancia)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.MontoDeLaApuesta) Then
                    g.DrawString($"Monto de Apuesta: {SanitizeText(apuesta.MontoDeLaApuesta)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If

                If Not String.IsNullOrEmpty(apuesta.NumeroGanador) Then
                    g.DrawString($"Número Ganador: {SanitizeText(apuesta.NumeroGanador)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Par) Then
                    g.DrawString($"Par: {SanitizeText(apuesta.Par)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Impar) Then
                    g.DrawString($"Impar: {SanitizeText(apuesta.Impar)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.PronosticoDirecto) Then
                    g.DrawString($"Pronóstico Directo: {SanitizeText(apuesta.PronosticoDirecto)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.TrifectaDirecta) Then
                    g.DrawString($"Trifecta Directa: {SanitizeText(apuesta.TrifectaDirecta)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.PronosticoReverso) Then
                    g.DrawString($"Pronóstico Reverso: {SanitizeText(apuesta.PronosticoReverso)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Menos) Then
                    g.DrawString($"Menos: {SanitizeText(apuesta.Menos)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Mas) Then
                    g.DrawString($"Más: {SanitizeText(apuesta.Mas)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Segundo) Then
                    g.DrawString($"2nd: {SanitizeText(apuesta.Segundo)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Tercero) Then
                    g.DrawString($"3rd: {SanitizeText(apuesta.Tercero)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.PrimeroOTercero) Then
                    g.DrawString($"1st or 3rd: {SanitizeText(apuesta.PrimeroOTercero)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.SegundoOTercero) Then
                    g.DrawString($"2nd or 3rd: {SanitizeText(apuesta.SegundoOTercero)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If

                If Not String.IsNullOrEmpty(apuesta.Mostrar) Then
                    g.DrawString($"Mostrar: {SanitizeText(apuesta.Mostrar)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Lugar) Then
                    g.DrawString($"Lugar: {SanitizeText(apuesta.Lugar)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Amarillo) Then
                    g.DrawString($"Amarillo: {SanitizeText(apuesta.Amarillo)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Verde) Then
                    g.DrawString($"Verde: {SanitizeText(apuesta.Verde)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Azul) Then
                    g.DrawString($"Azul: {SanitizeText(apuesta.Azul)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.Rojo) Then
                    g.DrawString($"Rojo: {SanitizeText(apuesta.Rojo)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If
                If Not String.IsNullOrEmpty(apuesta.GananciaPosible) Then
                    g.DrawString($"Ganancia Posible: {SanitizeText(apuesta.GananciaPosible)}", fontNormal, Brushes.Black, marginLeft, y)
                    y += 15
                End If


            Next

            g.DrawString(separator, fontBold, Brushes.Black, marginLeft, y)
            y += 15

            ' Totales
            If Not String.IsNullOrEmpty(_ticket.ApuestaTotal) Then
                g.DrawString($"Apuesta Total: {SanitizeText(_ticket.ApuestaTotal)}", fontNormal, Brushes.Black, marginLeft, y)
                y += 15
            End If
            If Not String.IsNullOrEmpty(_ticket.GananciaTotalPosible) Then
                g.DrawString($"Ganancia Total Posible: {SanitizeText(_ticket.GananciaTotalPosible)}", fontNormal, Brushes.Black, marginLeft, y)
                y += 15
            End If

            g.DrawString(separator, fontBold, Brushes.Black, marginLeft, y)
            y += 15

            If My.Settings.ImprimirMensajeFinal AndAlso Not String.IsNullOrEmpty(_mensajeFinal) Then
                Dim mensajeCompleto As String = $"PREMIO MÁXIMO A PAGAR  {_mensajeFinal}"
                Dim mensajeSize As SizeF = g.MeasureString(mensajeCompleto, fontBold)
                Dim mensajeX As Single = (e.MarginBounds.Width - mensajeSize.Width) / 2
                g.DrawString(mensajeCompleto, fontBold, Brushes.Black, mensajeX - 5, y)
                y += mensajeSize.Height + 10
            End If



            If String.IsNullOrEmpty(_ticket.CodigoDeBarra) OrElse _ticket.CodigoDeBarra = "No disponible" Then
                Dim mensajeReimpresion As String = "Copia de ticket"
                Dim mensajeSize As SizeF = g.MeasureString(mensajeReimpresion, fontBold)
                Dim mensajeX As Single = (e.MarginBounds.Width - mensajeSize.Width) / 2
                g.DrawString(mensajeReimpresion, fontBold, Brushes.Black, mensajeX, y)
                y += 30
            Else

                Dim writer As New ZXing.OneD.Code128Writer()
                Dim matrix As ZXing.Common.BitMatrix = writer.encode(
        _ticket.CodigoDeBarra, ZXing.BarcodeFormat.CODE_128,
        If(_paperWidth = 58, 100, 150), If(_paperWidth = 58, 40, 70)
    )
                Dim barcodeBitmap As New Bitmap(matrix.Width, matrix.Height)

                For yPos As Integer = 0 To matrix.Height - 1
                    For x As Integer = 0 To matrix.Width - 1
                        barcodeBitmap.SetPixel(x, yPos, If(matrix(x, yPos), Color.Black, Color.White))
                    Next
                Next

                If _paperWidth = 58 Then

                    Dim serialFont As New Font("Consolas", 8, FontStyle.Bold)
                    Dim serialTextWidth As Single = g.MeasureString(_ticket.CodigoDeBarra, serialFont).Width
                    Dim serialPositionX As Single = (pageWidth - serialTextWidth) / 2
                    g.DrawString(_ticket.CodigoDeBarra, serialFont, Brushes.Black, serialPositionX , y)
                    y += 20
                End If


                g.DrawImage(barcodeBitmap, New Rectangle((pageWidth - matrix.Width) / 2, y, matrix.Width, matrix.Height))
                y += matrix.Height + 10

                If _paperWidth = 80 Then

                    Dim serialFont As New Font("Consolas", 8, FontStyle.Bold)
                    Dim serialTextWidth As Single = g.MeasureString(_ticket.CodigoDeBarra, serialFont).Width
                    Dim serialPositionX As Single = (pageWidth - serialTextWidth) / 2
                    g.DrawString(_ticket.CodigoDeBarra, serialFont, Brushes.Black, serialPositionX, y)
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

    Private Function CenterText(text As String, g As Graphics, font As Font, pageWidth As Integer) As Single
        Return (pageWidth - g.MeasureString(text, font).Width) / 2
    End Function
End Class
