Imports Microsoft.Web.WebView2.Core
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports Newtonsoft.Json
Imports System.Web
Imports System.Drawing
Imports System.Drawing.Printing
Imports ZXing
Imports ZXing.Common
Imports System.Windows.Forms
Imports System.Runtime.InteropServices


Public Class backlot
    Private requestData As Dictionary(Of String, String) = New Dictionary(Of String, String)()
    Private WithEvents inactivityTimer As New Timer()
    Private Const inactivityThreshold As Integer = 2 * 60 * 1000
    Private lastMousePosition As Point
    Private isActivityDetected As Boolean = False
    Private Const WH_MOUSE_LL As Integer = 14
    Private Const WM_LBUTTONDOWN As Integer = &H201

    Private Delegate Function LowLevelMouseProc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    Private mouseProc As LowLevelMouseProc = AddressOf MouseHookCallback
    Private hookId As IntPtr = IntPtr.Zero

    <StructLayout(LayoutKind.Sequential)>
    Private Structure MSLLHOOKSTRUCT
        Public pt As Point
        Public mouseData As UInteger
        Public flags As UInteger
        Public time As UInteger
        Public dwExtraInfo As IntPtr
    End Structure

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowsHookEx(idHook As Integer, lpfn As LowLevelMouseProc, hMod As IntPtr, dwThreadId As UInteger) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function UnhookWindowsHookEx(hhk As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function CallNextHookEx(hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function GetModuleHandle(lpModuleName As String) As IntPtr
    End Function

    Private Function MouseHookCallback(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        If nCode >= 0 AndAlso wParam = CType(WM_LBUTTONDOWN, IntPtr) Then
            Dim mouseInfo As MSLLHOOKSTRUCT = Marshal.PtrToStructure(Of MSLLHOOKSTRUCT)(lParam)
            Dim cursorPosition As Point = mouseInfo.pt

            If Not Me.Bounds.Contains(cursorPosition) Then
                Me.Close()
            End If
        End If
        Return CallNextHookEx(hookId, nCode, wParam, lParam)
    End Function

    Private Sub AjustarFormulario()
        Dim screenWidth As Integer = Screen.PrimaryScreen.Bounds.Width
        Dim screenHeight As Integer = Screen.PrimaryScreen.Bounds.Height

        Dim baseWidth As Double = 1585
        Dim baseHeight As Double = 790
        Debug.WriteLine("screenWidth: " & screenWidth & " | screenHeight: " & screenHeight)

        Dim scaleFactor As Double = Math.Min(screenWidth / baseWidth,
                                         screenHeight / baseHeight)

        Dim newWidth As Integer = CInt(baseWidth * scaleFactor)
        Dim newHeight As Integer = CInt(baseHeight * scaleFactor)

        If newWidth > screenWidth Then newWidth = screenWidth
        If newHeight > screenHeight Then newHeight = screenHeight

        Dim offsetFromBottom As Integer = 0
        Dim extraHeight As Integer = 0



        If screenWidth <= 800 Then
            offsetFromBottom = 70
            extraHeight = 60
        ElseIf screenWidth <= 1024 Then
            offsetFromBottom = 50
            extraHeight = 130
        ElseIf screenWidth <= 1280 Then
            If screenHeight <= 720 Then
                offsetFromBottom = 5
                extraHeight = 10
            ElseIf screenHeight <= 800 Then
                offsetFromBottom = 20
                extraHeight = 70
            ElseIf screenHeight <= 1024 Then
                offsetFromBottom = 10
                extraHeight = 300
            End If
        ElseIf screenWidth <= 1366 Then
            offsetFromBottom = 10
            extraHeight = 5
        ElseIf screenWidth <= 1440 Then
            offsetFromBottom = 40
            extraHeight = 60
        ElseIf screenWidth <= 1600 Then
            offsetFromBottom = 30
            extraHeight = 5
        ElseIf screenWidth <= 1680 Then
            offsetFromBottom = 40
            extraHeight = 100
        ElseIf screenWidth <= 1920 Then
            offsetFromBottom = 0
            extraHeight = 40
        Else
            offsetFromBottom = 0
            extraHeight = 0
        End If

        newHeight += extraHeight

        If newHeight > screenHeight Then
            newHeight = screenHeight
        End If

        Me.Width = newWidth
        Me.Height = newHeight


        Me.Left = (screenWidth - Me.Width) \ 2


        Me.Top = screenHeight - Me.Height - offsetFromBottom


        Debug.WriteLine("offsetFromBottom: " & offsetFromBottom & " | extraHeight: " & extraHeight)
    End Sub


    Private Sub backlot_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        AjustarFormulario()
    End Sub

    Private Async Sub backlot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ShowInTaskbar = False
        Me.FormBorderStyle = FormBorderStyle.None
        Me.StartPosition = FormStartPosition.Manual

        If Not String.IsNullOrEmpty(My.Settings.SelectedPrinter) Then
            Debug.WriteLine($"Impresora seleccionada al iniciar: {My.Settings.SelectedPrinter}")
        Else
            Debug.WriteLine("No se seleccionó una impresora al iniciar.")
        End If

        inactivityTimer.Interval = inactivityThreshold
        inactivityTimer.Start()

        lastMousePosition = Cursor.Position
        AjustarFormulario()

        Try

            Await WebView21.EnsureCoreWebView2Async()
            Debug.WriteLine("WebView2 inicializado correctamente.")
            ConfigureWebView2PopupBlocking()

            WebView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All)

            AddHandler WebView21.CoreWebView2.WebResourceRequested, AddressOf WebView21_WebResourceRequested
            AddHandler WebView21.CoreWebView2.WebResourceResponseReceived, AddressOf WebView21_WebResourceResponseReceived

            AddHandler Me.MouseMove, AddressOf OnMouseMove
            AddHandler Me.KeyPress, AddressOf ResetInactivityTimer

            Await InjectActivityDetectionScriptAsync()
            Await InjectPrintInterceptionScriptAsync()
            Await InjectAutofillScriptAsync()
            SetupWebViewActivityListener()

            WebView21.CoreWebView2.Navigate("https://www.caja.backlot.bet/")
        Catch ex As Exception
            MessageBox.Show("Error inicializando WebView2: " & ex.Message)
        End Try
    End Sub


    Private Async Function InjectPrintInterceptionScriptAsync() As Task
        Dim script As String = "
        (function() {
            window.print = function() {
                try {
                    const ticketElement = document.querySelector('.ticket-content'); // Selecciona el contenido del ticket si existe
                    if (ticketElement) {
                        const message = {
                            type: 'print',
                            content: ticketElement.outerHTML
                        };
                        window.chrome.webview.postMessage(message); // Envía el contenido al host
                        console.log('Contenido del ticket enviado al host:', message);
                    }
                } catch (error) {
                    console.error('Error en print interceptado:', error);
                }
            };
        })();
    "
        Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script)
    End Function
    Private Async Function InjectActivityDetectionScriptAsync() As Task
        Dim script As String =
        "
        (function() {
            function notifyHost() {
                try {
                    window.chrome.webview.postMessage({
                        type: 'userActivity',
                        message: 'User is active inside WebView'
                    });
                } catch (e) {
                    console.error('Error posting activity message:', e);
                }
            }
            // Detectar mousemove, keydown y click
            document.addEventListener('mousemove', notifyHost);
            document.addEventListener('keydown', notifyHost);
            document.addEventListener('click', notifyHost);
        })();
        "
        Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script)
    End Function

    Private Function GetConfigFilePath() As String
        Dim appDataFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BacklotApp")
        If Not Directory.Exists(appDataFolder) Then
            Directory.CreateDirectory(appDataFolder)
        End If
        Return Path.Combine(appDataFolder, "config.json")
    End Function
    Private Sub SaveConfiguration(config As AppConfig)
        Try
            Dim configFilePath As String = GetConfigFilePath()
            Dim json As String = JsonConvert.SerializeObject(config, Formatting.Indented)
            File.WriteAllText(configFilePath, json)
            Debug.WriteLine($"Configuración guardada en: {configFilePath}")
        Catch ex As Exception
            Debug.WriteLine($"Error guardando configuración: {ex.Message}")
        End Try
    End Sub

    Private Function LoadConfiguration() As AppConfig
        Try
            Dim configFilePath As String = GetConfigFilePath()
            If File.Exists(configFilePath) Then
                Dim json As String = File.ReadAllText(configFilePath)
                Dim config As AppConfig = JsonConvert.DeserializeObject(Of AppConfig)(json)
                Debug.WriteLine("Configuración cargada correctamente.")
                Return config
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error cargando configuración: {ex.Message}")
        End Try
        Return Nothing
    End Function

    Private Async Function InjectAutofillScriptAsync() As Task
        Try
            Dim config As AppConfig = LoadConfiguration()
            If config Is Nothing OrElse String.IsNullOrEmpty(config.UsuarioBacklot) OrElse String.IsNullOrEmpty(config.ContraseñaBacklot) Then
                Debug.WriteLine("No hay credenciales guardadas para autocompletar.")
                Return
            End If

            Dim script As String = $"
                (function() {{
                    function simulateTyping(element, textValue) {{
                        if (!element) return;
                        element.focus();
                        element.value = textValue;
                        element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                        element.dispatchEvent(new Event('change', {{ bubbles: true }}));
                    }}

                    function autofill() {{
                        const userField = document.querySelector('#usuario');
                        const passField = document.querySelector('#clave');

                        if (!userField || !passField) {{
                            console.log('Campos de usuario o contraseña no encontrados.');
                            return;
                        }}

                        simulateTyping(userField, '{config.UsuarioBacklot}');
                        simulateTyping(passField, '{config.ContraseñaBacklot}');
                        console.log('Autocompletado realizado.');
                    }}

                    document.addEventListener('DOMContentLoaded', autofill);
                }})();
            "
            Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script)
            Debug.WriteLine("Script de autofill inyectado correctamente.")
        Catch ex As Exception
            Debug.WriteLine($"Error al inyectar el script de autofill: {ex.Message}")
        End Try
    End Function
    Private Sub SetupWebViewActivityListener()
        AddHandler WebView21.CoreWebView2.WebMessageReceived, AddressOf OnWebMessageReceived
    End Sub
    Private Sub OnWebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)
        Try
            Dim message As String = e.WebMessageAsJson
            Dim jsonData = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(message)


            If jsonData.ContainsKey("type") AndAlso jsonData("type") = "userActivity" Then
                ResetInactivityTimer(Nothing, Nothing) ' Reinicia el temporizador
                Debug.WriteLine("Actividad detectada dentro de WebView2.")
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error procesando WebMessageReceived: {ex.Message}")
        End Try
    End Sub

    Private Sub ConfigureWebView2PopupBlocking()
        AddHandler WebView21.CoreWebView2.NewWindowRequested, AddressOf HandleNewWindowRequested
    End Sub
    Private Sub inactivityTimer_Tick(sender As Object, e As EventArgs) Handles inactivityTimer.Tick
        If Not isActivityDetected Then
            inactivityTimer.Stop()
            Debug.WriteLine("Inactividad detectada. Cerrando formulario Backlot.")
            Me.Close()
        Else
            isActivityDetected = False
            Debug.WriteLine("Actividad detectada en el último intervalo. Continuando.")
        End If
    End Sub

    Private Sub ResetInactivityTimer(sender As Object, e As EventArgs)
        isActivityDetected = True
        inactivityTimer.Stop()
        inactivityTimer.Start()
        Debug.WriteLine("Actividad detectada. Temporizador reiniciado.")
    End Sub

    Private Sub HandleNewWindowRequested(sender As Object, e As CoreWebView2NewWindowRequestedEventArgs)
        Try
            e.Handled = True
            Debug.WriteLine($"Ventana emergente bloqueada: {e.Uri}")
        Catch ex As Exception
            Debug.WriteLine($"Error manejando ventana emergente: {ex.Message}")
        End Try
    End Sub

    Private Sub WebView21_WebResourceRequested(sender As Object, e As CoreWebView2WebResourceRequestedEventArgs)
        Try
            Dim request = e.Request
            If request.Uri.Contains("https://www.caja.backlot.bet/controllers/login") Then
                Debug.WriteLine("Interceptada una solicitud al endpoint /controllers/login")

                If request.Method = "POST" AndAlso request.Content IsNot Nothing Then
                    Dim content As String
                    Using memoryStream As New MemoryStream()
                        request.Content.CopyTo(memoryStream)
                        memoryStream.Position = 0
                        content = New StreamReader(memoryStream).ReadToEnd()
                    End Using

                    Debug.WriteLine($"Contenido de la solicitud de login: {content}")

                    Dim parsed = HttpUtility.ParseQueryString(content)

                    Dim user As String = parsed("usuario")
                    Dim pass As String = parsed("contraseña")

                    Debug.WriteLine($"Usuario capturado: {user}")
                    Debug.WriteLine($"Contraseña capturada: {pass}")


                    GuardarCredencialesBacklot(user, pass)
                Else
                    Debug.WriteLine("La solicitud no tiene contenido o no es POST (login).")
                End If
            End If


            If request.Uri.Contains("https://www.caja.backlot.bet/controllers/guardarTicket") Then
                Debug.WriteLine("Interceptada una solicitud al endpoint guardarTicket")


                If request.Method = "POST" AndAlso request.Content IsNot Nothing Then
                    Dim content As String
                    Using memoryStream As New MemoryStream()
                        request.Content.CopyTo(memoryStream)
                        memoryStream.Position = 0
                        content = New StreamReader(memoryStream).ReadToEnd()
                    End Using

                    Debug.WriteLine($"Carga útil de la solicitud: {content}")


                    Dim keyValuePairs = HttpUtility.ParseQueryString(content)


                    requestData("apuesta") = String.Join(",", keyValuePairs.GetValues("apuesta[]"))
                    requestData("evento") = String.Join(",", keyValuePairs.GetValues("evento[]"))
                    requestData("opcion") = String.Join(",", keyValuePairs.GetValues("opcion[]"))
                    requestData("horaSorteo") = keyValuePairs.GetValues("horaSorteo[]")?.FirstOrDefault()
                    requestData("juego") = keyValuePairs.GetValues("juego[]")?.FirstOrDefault()
                    requestData("hora") = keyValuePairs("hora")
                    requestData("total") = keyValuePairs("total")

                    Debug.WriteLine($"Total capturado: {requestData("total")}")

                    Debug.WriteLine($"Apuesta: {requestData("apuesta")}")
                    Debug.WriteLine($"Evento: {requestData("evento")}")
                    Debug.WriteLine($"Opciones: {requestData("opcion")}")
                    Debug.WriteLine($"Hora Sorteo: {requestData("horaSorteo")}")
                    Debug.WriteLine($"Juego: {requestData("juego")}")
                    Debug.WriteLine($"Hora: {requestData("hora")}")
                    Debug.WriteLine($"Total: {requestData("total")}")

                Else
                    Debug.WriteLine("La solicitud no tiene contenido o no es POST.")
                End If
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error procesando la solicitud interceptada: {ex.Message}")
        End Try
    End Sub
    Private Sub GuardarCredencialesBacklot(usuario As String, contraseña As String)
        Try
            Dim config As AppConfig = LoadConfiguration()
            If config Is Nothing Then
                config = New AppConfig()
            End If


            config.UsuarioBacklot = usuario
            config.ContraseñaBacklot = contraseña
            config.IsBacklot = True

            SaveConfiguration(config)

            Debug.WriteLine("Credenciales de Backlot guardadas correctamente.")
        Catch ex As Exception
            Debug.WriteLine($"Error guardando credenciales Backlot: {ex.Message}")
        End Try
    End Sub

    Private Sub backlot_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        If hookId <> IntPtr.Zero Then
            UnhookWindowsHookEx(hookId)
        End If
        RemoveHandler Me.MouseMove, AddressOf OnMouseMove
        RemoveHandler Me.KeyPress, AddressOf ResetInactivityTimer
        If WebView21 IsNot Nothing AndAlso WebView21.CoreWebView2 IsNot Nothing Then
            RemoveHandler WebView21.CoreWebView2.WebMessageReceived, AddressOf OnWebMessageReceived
        End If
        Debug.WriteLine("Formulario Backlot cerrado y eventos limpiados.")
    End Sub

    Private Async Sub WebView21_WebResourceResponseReceived(sender As Object, e As CoreWebView2WebResourceResponseReceivedEventArgs)
        Try
            If e.Request.Uri.Contains("https://www.caja.backlot.bet/controllers/guardarTicket") Then
                Debug.WriteLine("Interceptada la respuesta del endpoint guardarTicket")

                Dim response = e.Response
                Using responseStream = Await response.GetContentAsync()
                    Dim responseBody As String
                    Using reader As New StreamReader(responseStream)
                        responseBody = Await reader.ReadToEndAsync()
                    End Using

                    Debug.WriteLine($"Respuesta de la solicitud: {responseBody}")


                    Dim jsonResponse = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(responseBody)


                    Dim ticket As New TicketBack With {
                    .Numero = If(jsonResponse.ContainsKey("ticket"), CInt(jsonResponse("ticket")), 0),
                    .Fecha = If(jsonResponse.ContainsKey("fecha"), jsonResponse("fecha").ToString(), ""),
                    .Seguridad = If(jsonResponse.ContainsKey("seguridad"), jsonResponse("seguridad").ToString(), ""),
                    .Apuesta = requestData("apuesta").Split(",").ToList(),
                    .Evento = requestData("evento").Split(",").ToList(),
                    .Opciones = requestData("opcion").Split(",").ToList(),
                    .HoraSorteo = requestData("horaSorteo"),
                    .Juego = requestData("juego"),
                   .Hora = requestData("hora"),
                   .TotalMonto = requestData("total")
                }
                    Debug.WriteLine($"Total asignado al TicketBack: {requestData("total")}")


                    Dim printer As New TicketPrinterBack(
                            ticket,
                            My.Settings.PaperWidth,
                            My.Settings.SelectedPrinter,
                            My.Settings.SelectedLogo
                        )
                    printer.PrintAutomatically()


                    requestData.Clear()
                End Using
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error procesando la respuesta interceptada: {ex.Message}")
        End Try
    End Sub

    Private Function ExtractRequestData(content As Stream, key As String) As String
        Try
            If content Is Nothing Then Return ""
            Dim urlEncodedString As String = New StreamReader(content).ReadToEnd()
            Dim keyValuePairs = HttpUtility.ParseQueryString(urlEncodedString)
            Return If(keyValuePairs.AllKeys.Contains(key), keyValuePairs(key), "")
        Catch ex As Exception
            Debug.WriteLine($"Error extrayendo dato {key}: {ex.Message}")
            Return ""
        End Try
    End Function

    Private Sub OnMouseMove(sender As Object, e As EventArgs)
        Dim currentMousePosition As Point = Cursor.Position
        If Not currentMousePosition.Equals(lastMousePosition) Then
            lastMousePosition = currentMousePosition
            ResetInactivityTimer(sender, e)
            Debug.WriteLine($"MouseMove detectado en posición: {currentMousePosition}. (Formulario)")
        End If
    End Sub

    Private Function ExtractRequestDataList(content As Stream, key As String) As List(Of String)
        Try
            If content Is Nothing Then Return New List(Of String)()
            Dim urlEncodedString As String = New StreamReader(content).ReadToEnd()
            Dim keyValuePairs = HttpUtility.ParseQueryString(urlEncodedString)
            Return If(keyValuePairs.AllKeys.Contains(key & "[]"),
                      keyValuePairs.GetValues(key & "[]").ToList(),
                      New List(Of String)())
        Catch ex As Exception
            Debug.WriteLine($"Error extrayendo lista {key}: {ex.Message}")
            Return New List(Of String)()
        End Try
    End Function

    Private Sub backlot_Deactivate(sender As Object, e As EventArgs) Handles MyBase.Deactivate
        Me.Close()
    End Sub


End Class

