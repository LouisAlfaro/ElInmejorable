Imports Microsoft.Web.WebView2.Core
Imports Newtonsoft.Json.Linq
Imports HtmlAgilityPack
Imports System.IO
Imports Newtonsoft.Json
Imports System.Drawing.Printing
Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Imports System.Numerics
Imports Virtuales.Ticket



Public Class Form1



    <DllImport("user32.dll")>
    Private Shared Function GetAsyncKeyState(ByVal vKey As Integer) As Short
    End Function



    Private Const VK_F2 As Integer = &H71
    Private reiniciarAlCerrar As Boolean = True
    Private _logoPath As String
    Private _usuario As String
    Private WithEvents TimerKeyListener As New Timer()



    Private Sub AbrirFormPrintSettings()
        Try

            Dim loginForm As New FormLogin()
            Dim dialogResult As DialogResult = loginForm.ShowDialog()


            If dialogResult <> DialogResult.OK Then

                MessageBox.Show("Clave incorrecta. No se puede abrir el formulario de configuración.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If


            Dim formAlreadyOpen As Boolean = False
            For Each frm As Form In Application.OpenForms
                If TypeOf frm Is FormPrintSettings Then

                    frm.Activate()
                    formAlreadyOpen = True
                    Exit For
                End If
            Next


            If formAlreadyOpen Then
                Return
            End If


            Dim settingsForm As New FormPrintSettings()


            settingsForm.SelectedPrinter = My.Settings.SelectedPrinter
            settingsForm.PaperWidth = My.Settings.PaperWidth
            settingsForm.SelectedLogo = My.Settings.SelectedLogo
            settingsForm.SelectedUser = My.Settings.SelectedUser
            settingsForm.FinalMessage = My.Settings.FinalMessage


            If settingsForm.ShowDialog() = DialogResult.OK Then

                My.Settings.SelectedPrinter = settingsForm.SelectedPrinter
                My.Settings.PaperWidth = settingsForm.PaperWidth
                My.Settings.SelectedLogo = settingsForm.SelectedLogo
                My.Settings.SelectedUser = settingsForm.SelectedUser
                My.Settings.FinalMessage = settingsForm.FinalMessage


                My.Settings.Save()
                SaveConfiguration(New AppConfig With {
                .SelectedPrinter = My.Settings.SelectedPrinter,
                .PaperWidth = My.Settings.PaperWidth,
                .SelectedLogo = My.Settings.SelectedLogo,
                .SelectedUser = My.Settings.SelectedUser,
                .FinalMessage = My.Settings.FinalMessage,
                .IsTerminal = My.Settings.IsTerminal,
                .IsBacklot = My.Settings.IsBacklot
            })
                Debug.WriteLine("Configuración guardada correctamente.")
            End If

        Catch ex As Exception
            ' Manejo de errores inesperados
            MessageBox.Show($"Ocurrió un error al abrir el formulario de configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Debug.WriteLine($"Error en AbrirFormPrintSettings: {ex.Message}")
        End Try
    End Sub



    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Icon = New Icon("Logos/icono.ico")

        Me.WindowState = FormWindowState.Maximized
        Me.TopMost = False



        NotifyIcon1.Text = "El Inmejorable"
        NotifyIcon1.Visible = True


        Dim contextMenu As New ContextMenuStrip()
        contextMenu.Items.Add("Salir", Nothing, AddressOf ExitApplication)
        NotifyIcon1.ContextMenuStrip = contextMenu

        SetApplicationToRunAtStartup()

        Dim currentProcess As Process = Process.GetCurrentProcess()
        Dim runningProcesses = Process.GetProcessesByName(currentProcess.ProcessName)

        If runningProcesses.Length > 1 Then
            MessageBox.Show("La aplicación ya está en ejecución.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        Dim config As AppConfig = LoadConfiguration()
        If config IsNot Nothing Then
            My.Settings.SelectedPrinter = config.SelectedPrinter
            My.Settings.PaperWidth = config.PaperWidth
            My.Settings.SelectedLogo = config.SelectedLogo
            My.Settings.SelectedUser = config.SelectedUser
            My.Settings.FinalMessage = config.FinalMessage
            My.Settings.IsTerminal = config.IsTerminal
            My.Settings.IsBacklot = config.IsBacklot
        End If

        Dim logoFolder As String = Path.Combine(Application.StartupPath, "Logos")

        TimerKeyListener.Interval = 100
        TimerKeyListener.Start()

        btnOTerminal.Enabled = False
        btnATerminal.Enabled = False
        Try


            Await WebView21.EnsureCoreWebView2Async()
            Debug.WriteLine("WebView2 inicializado correctamente.")
            ConfigureWebView2PopupBlocking()
            WebView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All)
            AddHandler WebView21.CoreWebView2.WebResourceRequested, AddressOf WebView21_WebResourceRequested
            AddHandler WebView21.NavigationCompleted, AddressOf WebView21_NavigationCompleted



            Await InjectPrintInterceptionScriptAsync()
            Await InjectTokenInterceptionScriptAsync()
            Await InjectAutofillScriptAsync()




            If Not String.IsNullOrEmpty(My.Settings.TerminalImagePath) AndAlso File.Exists(My.Settings.TerminalImagePath) Then
                picBoxTerminal.Image = Image.FromFile(My.Settings.TerminalImagePath)
            Else
                picBoxTerminal.Image = Nothing
            End If

            If Not String.IsNullOrEmpty(My.Settings.TerminalImagePath) AndAlso File.Exists(My.Settings.TerminalImagePath) Then
                pgCajero.Image = Image.FromFile(My.Settings.TerminalImagePath)
            Else
                pgCajero.Image = Nothing
            End If


            Debug.WriteLine($"Valor de My.Settings.IsTerminal en Load: {My.Settings.IsTerminal}")
            UpdatePanelVisibility(My.Settings.IsTerminal, My.Settings.IsBacklot)
            updateButton(My.Settings.IsBacklot)

            AddHandler btnOTerminal.Click, AddressOf btnOTerminal_Click
            AddHandler btnATerminal.Click, AddressOf btnATerminal_Click

            AddHandler WebView21.CoreWebView2.WebMessageReceived, AddressOf WebView21_WebMessageReceived

            WebView21.CoreWebView2.Navigate("https://virtuales.pro/")
        Catch ex As Exception
            MessageBox.Show("Error inicializando WebView2: " & ex.Message)
        End Try
    End Sub



    Private Sub TimerKeyListener_Tick(sender As Object, e As EventArgs) Handles TimerKeyListener.Tick
        If GetAsyncKeyState(VK_F2) <> 0 Then
            TimerKeyListener.Stop()
            Try
                If WebView21 IsNot Nothing AndAlso WebView21.CoreWebView2 IsNot Nothing Then
                    Dim currentUrl As String = WebView21.Source.ToString()
                    Debug.WriteLine($"Tecla F2 detectada. URL actual: {currentUrl}")


                    If currentUrl = "https://virtuales.pro/login" Then
                        MessageBox.Show("No puedes abrir el formulario de configuración mientras estás en la página de inicio de sesión. Por favor, completa el inicio de sesión", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Debug.WriteLine("Intento de abrir el formulario bloqueado mientras el usuario está en /login.")
                    Else
                        AbrirFormPrintSettings()
                    End If
                Else
                    MessageBox.Show("El navegador WebView2 no está inicializado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Debug.WriteLine("El WebView2 no está inicializado.")
                End If
            Catch ex As Exception
                Debug.WriteLine($"Error al verificar la URL: {ex.Message}")
            Finally
                TimerKeyListener.Start()
            End Try
        End If

        Try
            If WebView21 IsNot Nothing AndAlso WebView21.CoreWebView2 IsNot Nothing Then
                Dim currentUrl As String = WebView21.Source.ToString()
                If currentUrl = "https://virtuales.pro/login" Then
                    btnATerminal.Enabled = False
                    btnOTerminal.Enabled = False


                ElseIf currentUrl = "https://virtuales.pro/" Then

                    btnATerminal.Enabled = False
                    btnOTerminal.Enabled = False
                Else
                    btnATerminal.Enabled = True
                    btnOTerminal.Enabled = True
                End If
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error al controlar el estado de los botones: {ex.Message}")
        End Try
    End Sub

    Private Sub SetApplicationToRunAtStartup()
        Try
            Dim appName As String = "Virtuales"
            Dim appPath As String = Application.ExecutablePath

            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
                If key IsNot Nothing Then
                    key.SetValue(appName, appPath)
                    Debug.WriteLine("La aplicación ha sido configurada para ejecutarse al inicio.")
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine($"Error al configurar la aplicación para ejecutarse al inicio: {ex.Message}")
        End Try
    End Sub

    Private Sub RestartApplication()
        Try
            If Not reiniciarAlCerrar Then Return

            Dim restartProcess As New ProcessStartInfo()
            restartProcess.FileName = Application.ExecutablePath
            restartProcess.UseShellExecute = True
            Process.Start(restartProcess)
            Debug.WriteLine("Reiniciando la aplicación...")
        Catch ex As Exception
            Debug.WriteLine($"Error al reiniciar la aplicación: {ex.Message}")
        End Try
    End Sub

    Private Function GetConfigFilePath() As String
        Dim appDataFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VirtualesApp")
        If Not Directory.Exists(appDataFolder) Then
            Directory.CreateDirectory(appDataFolder)
        End If
        Return Path.Combine(appDataFolder, "config.json")
    End Function

    Public Sub SaveConfiguration(config As AppConfig)
        Try
            Dim configFilePath As String = GetConfigFilePath()
            Dim json As String = JsonConvert.SerializeObject(config, Formatting.Indented)


            File.WriteAllText(configFilePath, json)
            Debug.WriteLine($"Contenido guardado: {json}")
        Catch ex As Exception
            Debug.WriteLine($"Error al guardar configuración: {ex.Message}")
        End Try
    End Sub


    Public Function LoadConfiguration() As AppConfig
        Try
            Dim configFilePath As String = GetConfigFilePath()
            If File.Exists(configFilePath) Then
                Dim json As String = File.ReadAllText(configFilePath)
                Dim config As AppConfig = JsonConvert.DeserializeObject(Of AppConfig)(json)
                Debug.WriteLine($"Configuración cargada desde: {configFilePath}")
                Return config
            Else
                Debug.WriteLine("Archivo de configuración no encontrado. Usando valores predeterminados de My.Settings.")

                Return New AppConfig With {
                    .SelectedPrinter = My.Settings.SelectedPrinter,
                    .PaperWidth = My.Settings.PaperWidth,
                    .SelectedLogo = My.Settings.SelectedLogo,
                    .SelectedUser = My.Settings.SelectedUser,
                    .FinalMessage = My.Settings.FinalMessage,
                    .IsTerminal = My.Settings.IsTerminal,
                    .IsBacklot = My.Settings.IsBacklot
                }
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error al cargar configuración: {ex.Message}")
            Return Nothing
        End Try
    End Function
    Private Sub ExitApplication(sender As Object, e As EventArgs)

        Try
            Dim appName As String = "ElInmejorable"
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
                If key IsNot Nothing AndAlso key.GetValue(appName) IsNot Nothing Then
                    key.DeleteValue(appName)
                    Debug.WriteLine("La aplicación ha sido eliminada de la ejecución automática al inicio.")
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine($"Error al eliminar la aplicación del inicio automático: {ex.Message}")
        End Try


        reiniciarAlCerrar = False


        NotifyIcon1.Visible = False


        Application.Exit()
    End Sub

    Public Sub updateButton(isVisible As Boolean)
        Try
            If isVisible Then
                btnOTerminal.Visible = True
                btnATerminal.Visible = True
            Else
                btnOTerminal.Visible = False
                btnATerminal.Visible = False
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error al actualizar el botón: {ex.Message}")

        End Try
    End Sub

    Public Sub UpdatePanelVisibility(isTerminal As Boolean, isVisible As Boolean)
        Try
            If isTerminal Then
                panelOverlay.Visible = True
                picBoxTerminal.Visible = True
                pgCajero.Visible = False
                If isVisible Then
                    btnOTerminal.Visible = False
                End If
                Debug.WriteLine("Panel configurado como Visible (Terminal).")


                Dim defaultLogoPath As String = Path.Combine(Application.StartupPath, "Logos", "LogoInmejorable.png")

                If Not String.IsNullOrEmpty(My.Settings.TerminalImagePath) AndAlso File.Exists(My.Settings.TerminalImagePath) Then
                    picBoxTerminal.Image = Image.FromFile(My.Settings.TerminalImagePath)
                ElseIf File.Exists(defaultLogoPath) Then
                    picBoxTerminal.Image = Image.FromFile(defaultLogoPath)
                Else
                    picBoxTerminal.Image = Nothing
                    Debug.WriteLine("No se encontró ninguna imagen para el PictureBox.")
                End If
            Else
                panelOverlay.Visible = False
                picBoxTerminal.Visible = False
                pgCajero.Visible = True

                Debug.WriteLine("Panel configurado como Oculto (Cajero).")
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error al actualizar la visibilidad del panel: {ex.Message}")
        End Try
    End Sub


    Private currentTicket As Ticket


    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F2 Then
            OpenPrintSettings()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function



    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        MyBase.OnFormClosing(e)
        If reiniciarAlCerrar Then
            RestartApplication()
        End If
    End Sub


    Private Async Sub WebView21_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs)
        Try

            Dim currentUrl As String = WebView21.Source.ToString()
            Debug.WriteLine($"Navegación completada. URL actual: {currentUrl}")


            If currentUrl.Contains("/login") Then
                Debug.WriteLine("Navegación detectada a la página de inicio de sesión.")


                btnOTerminal.Enabled = False
                btnATerminal.Enabled = False


                Await InjectAutofillScriptAsync()
            Else
                Debug.WriteLine("Navegación fuera de la página de inicio de sesión.")


                Dim config As AppConfig = LoadConfiguration()
                If config IsNot Nothing Then
                    btnOTerminal.Enabled = True
                    btnATerminal.Enabled = config.IsBacklot
                Else
                    btnOTerminal.Enabled = True
                    btnATerminal.Enabled = True
                End If
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error al manejar la navegación completada: {ex.Message}")
        End Try
    End Sub



    Private Sub ConfigureWebView2PopupBlocking()
        AddHandler WebView21.CoreWebView2.NewWindowRequested, AddressOf HandleNewWindowRequested
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
            If request.Method = "POST" AndAlso request.Uri.Contains("/api/v1/token") Then

                Dim content As String = New StreamReader(request.Content).ReadToEnd()
                Debug.WriteLine($"Interceptado contenido POST: {content}")


                Dim headers = request.Headers
                Dim username As String = ""
                Dim password As String = ""


                Dim authHeader As String = Nothing
                For Each header As KeyValuePair(Of String, String) In headers
                    If header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) Then
                        authHeader = header.Value
                        Exit For
                    End If
                Next


                If Not String.IsNullOrEmpty(authHeader) AndAlso authHeader.StartsWith("Basic ") Then
                    Dim base64Credentials = authHeader.Substring(6)
                    Dim credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials))
                    Dim parts = credentials.Split(":"c)
                    If parts.Length = 2 Then
                        username = parts(0)
                        password = parts(1)
                    End If
                End If



                If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
                    Dim payload = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(content)
                    If payload IsNot Nothing Then
                        username = If(payload.ContainsKey("username"), payload("username"), "")
                        password = If(payload.ContainsKey("password"), payload("password"), "")
                    End If
                End If



                If Not String.IsNullOrEmpty(username) AndAlso Not String.IsNullOrEmpty(password) Then
                    Dim config As AppConfig = LoadConfiguration()
                    If config Is Nothing Then config = New AppConfig()


                    config.Usuario = username
                    config.Contraseña = password


                    SaveConfiguration(config)

                    Debug.WriteLine($"Credenciales guardadas: Usuario={username}, Contraseña={password}")
                Else
                    Debug.WriteLine("No se encontraron credenciales en el contenido o los encabezados.")
                End If

            End If
        Catch ex As Exception
            Debug.WriteLine($"Error interceptando solicitud: {ex.Message}")
        End Try
    End Sub



    Private Async Function InjectPrintInterceptionScriptAsync() As Task
        Dim script As String = "
        (function() {
            window.print = function() {
                try {
                    const ticketElement = document.querySelector('.ticket-content');
                    if (ticketElement) {
                        const message = {
                            type: 'print',
                            content: ticketElement.outerHTML
                        };
                        window.chrome.webview.postMessage(message); 
                        console.log('Mensaje enviado:', message);
                        
                    }
                } catch (error) {
                    console.error('Error en print:', error);a
                }
            };
        })();
        "
        Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script)
    End Function

    Private Async Function InjectTokenInterceptionScriptAsync() As Task
        Dim script As String = "
        (function() {
            const originalFetch = window.fetch;
            window.fetch = async function(input, init) {
                try {
                    if (typeof input === 'string' && input.includes('/api/v1/token')) {
                        const body = init && init.body ? init.body : null;
                        if (body) {
                            const params = JSON.parse(body);
                            const message = {
                                type: 'token',
                                username: params.username || '',
                                password: params.password || ''
                            };
                            window.chrome.webview.postMessage(message);
                            console.log('mensaje',message)
                        }
                    }
                } catch (err) {
                    console.error('Error interceptando fetch:', err);
                }
                return originalFetch(input, init);
            };
        })();
        "
        Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script)
    End Function

    Private Async Function InjectAutofillScriptAsync() As Task
        Try
            Dim config As AppConfig = LoadConfiguration()
            If config Is Nothing OrElse String.IsNullOrEmpty(config.Usuario) OrElse String.IsNullOrEmpty(config.Contraseña) Then
                Debug.WriteLine("No hay credenciales para autocompletar.")
                Return
            End If

            Dim script As String = $"
        (function() {{
            function simulateTyping(element, textValue) {{

               if (!element) return;

    // Hacer focus en el elemento
    element.focus();

    // Si ya coincide el valor, no hacer nada
    if (element.value === textValue) return;

    // Utilizar el setter nativo para establecer el valor
    const nativeInputValueSetter = Object.getOwnPropertyDescriptor(
        window.HTMLInputElement.prototype,
        'value'
    ).set;
    nativeInputValueSetter.call(element, textValue);

    // Disparar eventos para que los frameworks detecten el cambio
    element.dispatchEvent(new Event('input', {{ bubbles: true }}));
    element.dispatchEvent(new Event('change', {{bubbles: true }}));

    // Sacar el focus (opcional)
    element.blur();
            }}
    
            function enableAndSimulateClick(button) {{
                if (!button) return;
                
                // Quitar atributo disabled o clase disabled
                button.removeAttribute('disabled');
                button.classList.remove('disabled');

                // Simular hover y click (puedes comentar si no deseas clic automático)
                const mouseOver = new MouseEvent('mouseover', {{ bubbles: true }});
                button.dispatchEvent(mouseOver);

                const mouseDown = new MouseEvent('mousedown', {{ bubbles: true }});
                button.dispatchEvent(mouseDown);

                const mouseUp = new MouseEvent('mouseup', {{ bubbles: true }});
                button.dispatchEvent(mouseUp);

                
            }}

            function autofill() {{
                try {{
                    const userField = document.querySelector('input[placeholder=""Usuario""]');
                    const passField = document.querySelector('input[placeholder=""Contraseña""]');
                    const loginButton = document.querySelector('button.common-button.login-page-button');

                    if (!userField || !passField || !loginButton) {{
                        console.log('No se encontraron los campos o el botón.');
                        return;
                    }}

                    // Simular tipeo en ambos campos
                    simulateTyping(userField, '{config.Usuario}');
                    simulateTyping(passField, '{config.Contraseña}');


                    console.log('user',userField);
                    console.log('pass',passField);
                    console.log('boton',loginButton);

                    // Habilitar y (opcional) hacer clic
                    enableAndSimulateClick(loginButton);

                    console.log('Autocompletado y botón habilitado. Esperando interacción o clic automático...');
                }} catch (err) {{
                    console.error('Error en autofill:', err);
                }}
            }}

            document.addEventListener('DOMContentLoaded', autofill);

            // Si la página tarda en renderizar, repetimos un par de veces
            let attempt = 0;
            const intervalId = setInterval(() => {{
                const userField = document.querySelector('input[placeholder=""Usuario""]');
                const passField = document.querySelector('input[placeholder=""Contraseña""]');
                if (userField && passField) {{
                    autofill();
                    clearInterval(intervalId);
                }}
                attempt++;
                if (attempt > 5) {{
                    clearInterval(intervalId);
                }}
            }}, 1000);
        }})();
        "

            Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script)
            Debug.WriteLine("Script de autocompletado con focus y mouse events inyectado correctamente.")
        Catch ex As Exception
            Debug.WriteLine($"Error inyectando el script de autocompletar: {ex.Message}")
        End Try
    End Function



    Private Sub WebView21_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)
        Try
            Dim rawMessage As String = e.WebMessageAsJson
            Debug.WriteLine($"Mensaje recibido: {rawMessage}")


            Dim messageObject = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(rawMessage)
            Debug.WriteLine($"mensaje recibido:{messageObject}")

            If messageObject IsNot Nothing AndAlso messageObject.ContainsKey("type") Then
                Dim messageType As String = messageObject("type")

                Select Case messageType
                    Case "print"
                        HandlePrintMessage(messageObject, rawMessage)
                    Case "token"
                        HandleTokenMessage(messageObject)
                    Case Else
                        Debug.WriteLine($"Tipo de mensaje desconocido: {messageType}")
                End Select
            Else
                Debug.WriteLine("El mensaje recibido no contiene un campo 'type'.")
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error procesando el mensaje del WebView: {ex.Message}")
            MessageBox.Show($"Error procesando el mensaje: {ex.Message}")
        End Try
    End Sub

    Private Function ParseTicketHtml(htmlContent As String) As Ticket
        Try
            Dim knownKeys As New HashSet(Of String) From {
            "fecha", "id de ticket", "código de barra", "tipos de apuesta",
            "juego", "id de ronda", "ganancia", "monto de apuesta",
            "posible ganancia", "apuesta total", "ganancia total posible",
            "número ganador", "par", "impar", "pronóstico directo",
            "trifecta directa", "pronóstico reverso", "menos", "más",
            "2nd", "3rd", "1st or 3rd", "2nd or 3rd",
            "lugar", "mostrar", "amarillo", "rojo", "azul", "verde",
            "ganador", "total de probabilidades", "place"
        }

            Dim unknownKeys As New HashSet(Of String)()

            Dim doc As New HtmlAgilityPack.HtmlDocument()
            doc.LoadHtml(htmlContent)

            Dim ticket As New Ticket()
            ticket.Apuestas = New List(Of Ticket.Apuesta)()

            Dim keyValuePairs = doc.DocumentNode.SelectNodes("//div[contains(@class, 'ticket-key-value')]")
            Dim currentApuesta As New Ticket.Apuesta()

            If keyValuePairs IsNot Nothing Then
                For Each pair In keyValuePairs
                    Dim keyNode = pair.SelectSingleNode(".//div[contains(@class, 'ticket-key')]")
                    Dim valueNode = pair.SelectSingleNode(".//div[contains(@class, 'ticket-value')]")

                    If keyNode IsNot Nothing AndAlso valueNode IsNot Nothing Then
                        Dim key As String = keyNode.InnerText.Trim().ToLower()
                        Dim value As String = valueNode.InnerText.Trim()

                        If knownKeys.Contains(key) Then
                            Select Case key
                                Case "fecha"
                                    ticket.Fecha = value
                                Case "id de ticket"
                                    ticket.IDDeTicket = value
                                Case "código de barra"
                                    ticket.CodigoDeBarra = value
                                Case "tipos de apuesta", "tipo de apuesta"
                                    ticket.TipoDeApuesta = value
                                Case "juego"

                                    If Not String.IsNullOrEmpty(currentApuesta.Juego) OrElse
                                   Not String.IsNullOrEmpty(currentApuesta.Numero) Then
                                        ticket.Apuestas.Add(currentApuesta)
                                    End If

                                    currentApuesta = New Ticket.Apuesta()
                                    currentApuesta.Juego = value
                                Case "id de ronda"
                                    currentApuesta.IDDeLaRonda = value
                                Case "ganancia"
                                    currentApuesta.Ganancia = value
                                Case "monto de apuesta"
                                    currentApuesta.MontoDeLaApuesta = value
                                Case "posible ganancia"
                                    currentApuesta.GananciaPosible = value
                                    ticket.Apuestas.Add(currentApuesta)
                                    currentApuesta = New Ticket.Apuesta()
                                Case "apuesta total"
                                    ticket.ApuestaTotal = value
                                Case "ganancia total posible"
                                    ticket.GananciaTotalPosible = value
                                Case "número ganador"
                                    currentApuesta.NumeroGanador = value
                                Case "par"
                                    currentApuesta.Par = "Par"
                                Case "impar"
                                    currentApuesta.Impar = "Impar"
                                Case "pronóstico directo"
                                    currentApuesta.PronosticoDirecto = value
                                Case "trifecta directa"
                                    currentApuesta.TrifectaDirecta = value
                                Case "pronóstico reverso"
                                    currentApuesta.PronosticoReverso = value
                                Case "menos"
                                    currentApuesta.Menos = "Menos"
                                Case "más"
                                    currentApuesta.Mas = "Más"
                                Case "2nd"
                                    currentApuesta.Segundo = value
                                Case "3rd"
                                    currentApuesta.Tercero = value
                                Case "1st or 3rd"
                                    currentApuesta.PrimeroOTercero = value
                                Case "2nd or 3rd"
                                    currentApuesta.SegundoOTercero = value
                                Case "lugar"
                                    currentApuesta.Lugar = value
                                Case "mostrar"
                                    currentApuesta.Mostrar = value
                                Case "amarillo"
                                    currentApuesta.Amarillo = "Amarillo"
                                Case "verde"
                                    currentApuesta.Verde = "Verde"
                                Case "rojo"
                                    currentApuesta.Rojo = "Rojo"
                                Case "azul"
                                    currentApuesta.Azul = "Azul"
                                Case "ganador"
                                    currentApuesta.Ganador = value
                                Case "total de probabilidades"
                                    currentApuesta.TotalProbabilidades = value
                                Case "place"
                                    currentApuesta.Place = value
                            End Select
                        Else
                            If System.Text.RegularExpressions.Regex.IsMatch(key, "^\d+(\s\d+)*(\*\s?\d+)?$") Then
                                If Not String.IsNullOrEmpty(currentApuesta.Juego) OrElse
                               Not String.IsNullOrEmpty(currentApuesta.Numero) Then
                                    ticket.Apuestas.Add(currentApuesta)
                                End If
                                currentApuesta = New Ticket.Apuesta()
                                currentApuesta.Numero = key
                            Else
                                If Not unknownKeys.Contains(key) Then
                                    unknownKeys.Add(key)
                                    Debug.WriteLine($"Clave desconocida detectada: {key}")
                                End If
                            End If
                        End If
                    Else
                        Debug.WriteLine($"Nodo faltante: Clave={If(keyNode?.InnerText, "N/A")}, Valor={If(valueNode?.InnerText, "N/A")}")
                    End If
                Next

                If Not String.IsNullOrEmpty(currentApuesta.Juego) OrElse
               Not String.IsNullOrEmpty(currentApuesta.Numero) OrElse
               Not String.IsNullOrEmpty(currentApuesta.Ganancia) Then
                    ticket.Apuestas.Add(currentApuesta)
                End If

                If unknownKeys.Count > 0 Then
                    Dim filePath As String = "unknown_keys.txt"
                    If Not File.Exists(filePath) Then
                        File.Create(filePath).Dispose()
                    End If
                    Dim existingKeys = File.ReadAllLines(filePath).ToHashSet()
                    Dim newKeys = unknownKeys.Except(existingKeys).ToList()
                    If newKeys.Any() Then
                        File.AppendAllLines(filePath, newKeys)
                        Debug.WriteLine($"Claves desconocidas guardadas en {filePath}: {String.Join(", ", newKeys)}")
                    End If
                End If
            End If

            Return ticket
        Catch ex As Exception
            Debug.WriteLine($"Error parseando HTML: {ex.Message}")
            Return Nothing
        End Try
    End Function



    Private Sub HandlePrintMessage(messageObject As Dictionary(Of String, String), rawMessage As String)
        Try
            If messageObject.ContainsKey("content") Then
                Dim htmlContent = messageObject("content")
                Dim doc As New HtmlAgilityPack.HtmlDocument()

                doc.LoadHtml(htmlContent)
                Debug.WriteLine(htmlContent)

                Debug.WriteLine("Contenido del JSON recibido:")
                Debug.WriteLine(rawMessage)

                Dim ticketDivs = doc.DocumentNode.SelectNodes("//div[contains(@class, 'ticket-key-value')]")
                If ticketDivs Is Nothing OrElse ticketDivs.Count = 0 Then
                    Debug.WriteLine("No se encontraron datos en el ticket.")
                    Return
                End If


                Dim apuestasCapturadas As New List(Of List(Of KeyValuePair(Of String, String)))()
                Dim currentApuesta As New List(Of KeyValuePair(Of String, String))()

                Dim isFootballOrPenalty As Boolean = False
                Dim isDirectPrintGame As Boolean = False


                Dim ticketId As String = "No disponible"
                Dim ticketFecha As String = "No disponible"
                Dim ticketTipoApuesta As String = "No disponible"
                Dim ticketCodigoDeBarra As String = "No disponible"
                Dim ApuestaTotal As String
                Dim importeTotalGanado As String
                Dim GananciaTotalPosible As String



                For Each divNode In ticketDivs
                    Dim keyNode = divNode.SelectSingleNode(".//div[contains(@class, 'ticket-key')]")
                    Dim valueNode = divNode.SelectSingleNode(".//div[contains(@class, 'ticket-value')]")
                    If keyNode IsNot Nothing AndAlso valueNode IsNot Nothing Then
                        Dim key = keyNode.InnerText.Trim()
                        Dim value = valueNode.InnerText.Trim()

                        Debug.WriteLine($"Clave: {key}, Valor: {value}")

                        If key.ToLower() = "juego" Then

                            If currentApuesta.Count > 0 Then
                                apuestasCapturadas.Add(New List(Of KeyValuePair(Of String, String))(currentApuesta))
                                currentApuesta.Clear()
                            End If
                        End If


                        currentApuesta.Add(New KeyValuePair(Of String, String)(key, value))


                        If key.ToLower().Contains("posible ganancia") Then
                            apuestasCapturadas.Add(New List(Of KeyValuePair(Of String, String))(currentApuesta))
                            currentApuesta.Clear()
                        End If


                        Select Case key.ToLower()
                            Case "id de ticket"
                                ticketId = value
                            Case "fecha"
                                ticketFecha = value
                            Case "tipos de apuesta"
                                ticketTipoApuesta = value
                            Case "código de barra"
                                ticketCodigoDeBarra = value
                            Case "apuesta total"
                                ApuestaTotal = value
                            Case "importa total ganado"
                                importeTotalGanado = value
                            Case "ganancia total posible"
                                GananciaTotalPosible = value
                        End Select




                        If key.ToLower() = "juego" AndAlso
                   (value.ToLower().Contains("football league") OrElse value.ToLower().Contains("penalty")) Then
                            isFootballOrPenalty = True
                        End If

                        If key.ToLower() = "juego" Then

                            Dim sanitizedValue = value.Trim().ToLower()




                            If sanitizedValue.Contains("oddball") OrElse
                                   sanitizedValue.Contains("roulette") OrElse
                                   sanitizedValue.Contains("keno") OrElse
                                   sanitizedValue.Contains("doublewheel") OrElse
                                   sanitizedValue.Contains("autoroulette") OrElse
                                   sanitizedValue.Contains("spintowin") OrElse
                                   sanitizedValue.Contains("drag racing") OrElse
                                   sanitizedValue.Contains("spintowindeluxe") Then
                                isDirectPrintGame = True
                                Debug.WriteLine($"Juego detectado para impresión directa: {sanitizedValue}")
                            End If
                        End If

                    End If
                Next

                Dim isMultipleOrExpreso As Boolean = False
                If Not String.IsNullOrEmpty(ticketTipoApuesta) Then
                    Dim tipoApuestaLwr = ticketTipoApuesta.ToLower()
                    If tipoApuestaLwr.Contains("multiple") OrElse tipoApuestaLwr.Contains("expreso") Then
                        isMultipleOrExpreso = True
                    End If
                End If

                If currentApuesta.Count > 0 Then
                    apuestasCapturadas.Add(New List(Of KeyValuePair(Of String, String))(currentApuesta))
                End If

                If isMultipleOrExpreso Then
                    Debug.WriteLine("Imprimiendo apuesta Multiple/Expreso con diseño FootballPenalty.")
                    PrintFootballPenaltyApuestas(apuestasCapturadas, ticketId, ticketFecha, ticketTipoApuesta, ticketCodigoDeBarra, My.Settings.PaperWidth, ApuestaTotal, importeTotalGanado, GananciaTotalPosible)
                    Debug.WriteLine(apuestasCapturadas)
                ElseIf isFootballOrPenalty Then
                    Debug.WriteLine("Imprimiendo directamente Football League/Penalty con diseño básico.")

                    PrintFootballPenaltyApuestas(apuestasCapturadas, ticketId, ticketFecha, ticketTipoApuesta, ticketCodigoDeBarra, My.Settings.PaperWidth, ApuestaTotal, importeTotalGanado, GananciaTotalPosible)

                    Debug.WriteLine(apuestasCapturadas)

                ElseIf isDirectPrintGame Then
                    Debug.WriteLine("Imprimiendo directamente juegos de interés.")

                    PrintFootballPenaltyApuestas(apuestasCapturadas, ticketId, ticketFecha, ticketTipoApuesta, ticketCodigoDeBarra, My.Settings.PaperWidth, ApuestaTotal, importeTotalGanado, GananciaTotalPosible)

                    Debug.WriteLine(apuestasCapturadas)

                Else
                    Debug.WriteLine("Procesando con TicketPrinter para otros tipos de apuestas.")
                    Dim ticket As Ticket = ParseTicketHtml(htmlContent)

                    Dim printer As New TicketPrinter(
                    ticket,
                    My.Settings.PaperWidth,
                    My.Settings.SelectedPrinter,
                    My.Settings.SelectedLogo,
                    My.Settings.SelectedUser,
                    My.Settings.FinalMessage
                )

                    printer.PrintAutomatically()

                    Debug.WriteLine(apuestasCapturadas)


                End If
            Else
                Debug.WriteLine("El mensaje de tipo 'print' no contiene la clave 'content'.")
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error manejando mensaje de tipo 'print': {ex.Message}")
        End Try
    End Sub


    Private Sub PrintFootballPenaltyApuestas(
    apuestasList As List(Of List(Of KeyValuePair(Of String, String))),
    ticketId As String,
    ticketFecha As String,
    ticketTipoApuesta As String,
    ticketCodigoDeBarra As String,
    paperWidth As Integer,
    ApuestaTotal As String,
    importeTotalGanado As String,
    GananciaTotalPosible As String
)
        If My.Settings.IsTerminal Then
            Debug.WriteLine("Modo Terminal activo. No se imprimirá el ticket en PrintFootballPenaltyApuestas.")
            Return
        End If

        Dim printDoc As New PrintDocument()


        If Not String.IsNullOrEmpty(My.Settings.SelectedPrinter) Then
            printDoc.PrinterSettings.PrinterName = My.Settings.SelectedPrinter
        End If

        AddHandler printDoc.PrintPage,
    Sub(sender, e)
        Dim g = e.Graphics
        Dim fontTitle As New Font("Consolas", If(paperWidth = 58, 9, 11), FontStyle.Bold)
        Dim fontBody As New Font("Consolas", If(paperWidth = 58, 6, 8))
        Dim yPos As Single = 50
        Dim leftMargin As Single = If(paperWidth = 58, 10, 30)
        Dim pageWidth As Single = e.PageBounds.Width


        If Not String.IsNullOrEmpty(My.Settings.SelectedLogo) AndAlso IO.File.Exists(My.Settings.SelectedLogo) Then
            Try
                Dim logoImage As Image = Image.FromFile(My.Settings.SelectedLogo)
                Dim logoWidth As Integer = If(paperWidth = 58, 140, 220)
                Dim logoHeight As Integer = If(paperWidth = 58, 70, 130)
                g.DrawImage(logoImage, (pageWidth - logoWidth) / 2, yPos, logoWidth, logoHeight)
                yPos += logoHeight + 20
            Catch ex As Exception
                Debug.WriteLine($"Error al cargar/imprimir el logo: {ex.Message}")
            End Try
        End If


        g.DrawString($"ID Ticket:", fontBody, Brushes.Black, leftMargin, yPos)
        g.DrawString(SanitizeText(ticketId), fontBody, Brushes.Black, leftMargin + 150, yPos)
        yPos += 20

        g.DrawString($"Emitido por:", fontBody, Brushes.Black, leftMargin, yPos)
        g.DrawString(SanitizeText(My.Settings.SelectedUser), fontBody, Brushes.Black, leftMargin + 150, yPos)
        yPos += 20


        g.DrawString($"Fecha: {ticketFecha}", fontBody, Brushes.Black, leftMargin, yPos)
        yPos += 20


        g.DrawString(New String("-"c, If(paperWidth = 58, 30, 40)), fontTitle, Brushes.Black, leftMargin - 15, yPos)
        yPos += 20


        Dim printedApuestas As New HashSet(Of String)

        For Each apuesta In apuestasList
            Dim hasRelevantData As Boolean = False
            Dim apuestaString As String = String.Join("|", apuesta.Select(Function(kvp) $"{kvp.Key}:{kvp.Value}"))


            If printedApuestas.Contains(apuestaString) Then
                Continue For
            End If

            For Each kvp In apuesta

                If kvp.Key.ToLower().Contains("apuesta total") OrElse
                   kvp.Key.ToLower().Contains("importa total ganado") OrElse
                   kvp.Key.ToLower().Contains("ganancia total posible") OrElse
                   kvp.Key.ToLower().Contains("fecha") OrElse
                   kvp.Key.ToLower().Contains("código de barra") Then
                    Continue For
                End If


                g.DrawString($"{kvp.Key}: {kvp.Value}", fontBody, Brushes.Black, leftMargin - 12, yPos)
                yPos += 20
                hasRelevantData = True


                If kvp.Key.ToLower().Contains("posible ganancia") OrElse kvp.Key.ToLower().Contains("monto de ganancia") Then
                    yPos += 10
                    Exit For
                End If
            Next


            If hasRelevantData Then
                g.DrawString(New String("-"c, If(paperWidth = 58, 30, 40)), fontTitle, Brushes.Black, leftMargin - 15, yPos)
                yPos += 20
            End If


            printedApuestas.Add(apuestaString)
        Next


        Dim totalesImpresos As Boolean = False


        If Not totalesImpresos Then
            If Not String.IsNullOrEmpty(ApuestaTotal) Then
                g.DrawString($"Apuesta Total: {ApuestaTotal}", fontBody, Brushes.Black, leftMargin - 12, yPos)
                yPos += 20
            End If
            If Not String.IsNullOrEmpty(importeTotalGanado) Then
                g.DrawString($"Importe Total Ganancia: {importeTotalGanado}", fontBody, Brushes.Black, leftMargin - 12, yPos)
                yPos += 20
            End If

            If Not String.IsNullOrEmpty(GananciaTotalPosible) Then
                g.DrawString($"Ganancia Total Posible: {GananciaTotalPosible}", fontBody, Brushes.Black, leftMargin - 12, yPos)
                yPos += 20
            End If
            g.DrawString(New String("-"c, If(paperWidth = 58, 30, 40)), fontTitle, Brushes.Black, leftMargin - 15, yPos)
            yPos += 20
            totalesImpresos = True
        End If

        If Not String.IsNullOrEmpty(My.Settings.FinalMessage) Then
            Dim finalMessage As String = $"PREMIO MÁXIMO A PAGAR {My.Settings.FinalMessage}"
            Dim textSize As SizeF = g.MeasureString(finalMessage, fontBody)
            Dim centerX As Single = (e.PageBounds.Width - textSize.Width) / 2
            g.DrawString(finalMessage, fontBody, Brushes.Black, centerX, yPos)
            yPos += textSize.Height
        End If



        If String.IsNullOrEmpty(ticketCodigoDeBarra) OrElse ticketCodigoDeBarra = "No disponible" Then

            Dim mensajeReimpresion As String = "Copia de ticket"
            Dim fontBold As New Font("Consolas", 10, FontStyle.Bold)


            Dim mensajeWidth As Single = g.MeasureString(mensajeReimpresion, fontBold).Width
            Dim mensajeX As Single = (pageWidth - mensajeWidth) / 2


            g.DrawString(mensajeReimpresion, fontBold, Brushes.Black, mensajeX, yPos)
            yPos += 30
        Else

            Try
                Dim writer As New ZXing.OneD.Code128Writer()
                Dim matrix As ZXing.Common.BitMatrix = writer.encode(
        ticketCodigoDeBarra, ZXing.BarcodeFormat.CODE_128,
        If(paperWidth = 58, 100, 150), If(paperWidth = 58, 60, 90)
    )
                Dim barcodeBitmap As New Bitmap(matrix.Width, matrix.Height)


                For yMatrix As Integer = 0 To matrix.Height - 1
                    For xMatrix As Integer = 0 To matrix.Width - 1
                        barcodeBitmap.SetPixel(xMatrix, yMatrix, If(matrix(xMatrix, yMatrix), Color.Black, Color.White))
                    Next
                Next


                g.DrawImage(barcodeBitmap, New Rectangle((pageWidth - matrix.Width) / 2, yPos, matrix.Width, matrix.Height))
                yPos += matrix.Height + 10


                Dim serialFont As New Font("Consolas", 8, FontStyle.Bold)
                Dim serialTextWidth As Single = g.MeasureString(ticketCodigoDeBarra, serialFont).Width
                Dim serialPositionX As Single = (pageWidth - serialTextWidth) / 2
                g.DrawString(ticketCodigoDeBarra, serialFont, Brushes.Black, serialPositionX, yPos)
                yPos += 20
            Catch ex As Exception
                Debug.WriteLine($"Error generando/imprimiendo código de barras: {ex.Message}")
            End Try

        End If
    End Sub
        Try

            printDoc.Print()

            Debug.WriteLine(apuestasList)

        Catch ex As Exception
            Debug.WriteLine($"Error imprimiendo Football/Penalty: {ex.Message}")
        End Try


    End Sub

    Private Function SanitizeText(input As String) As String
        If input Is Nothing Then Return String.Empty
        Return input.Replace(Chr(10), " ").Replace(Chr(13), " ").Trim()
    End Function

    Private Sub HandleTokenMessage(messageObject As Dictionary(Of String, String))
        Try

            Dim username As String = If(messageObject.ContainsKey("username"), messageObject("username"), "")
            Dim password As String = If(messageObject.ContainsKey("password"), messageObject("password"), "")


            If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
                Debug.WriteLine("No se pudieron obtener el usuario o la contraseña del mensaje.")
                Exit Sub
            End If


            Dim config As AppConfig = LoadConfiguration()
            If config Is Nothing Then config = New AppConfig()


            If Not String.IsNullOrEmpty(username) Then
                config.Usuario = username
            Else
                Debug.WriteLine("El nuevo valor para Usuario es vacío. No se sobrescribirá el valor actual.")
            End If

            If Not String.IsNullOrEmpty(password) Then
                config.Contraseña = password
            Else
                Debug.WriteLine("El nuevo valor para Contraseña es vacío. No se sobrescribirá el valor actual.")
            End If


            SaveConfiguration(config)

            Debug.WriteLine($"Datos de inicio de sesión guardados: Usuario={config.Usuario}, Contraseña={config.Contraseña}")
        Catch ex As Exception
            Debug.WriteLine($"Error manejando mensaje de tipo 'token': {ex.Message}")
        End Try
    End Sub

    Private Sub ProcessTicketData(htmlContent As String)
        Try

            Dim ticket As Ticket = ParseTicketHtml(htmlContent)

            If ticket Is Nothing Then
                Debug.WriteLine("No se pudo procesar el ticket: datos nulos.")
                Return
            End If

            Debug.WriteLine("Procesando datos del ticket:")
            Debug.WriteLine($"Fecha: {ticket.Fecha}")
            Debug.WriteLine($"ID de Ticket: {ticket.IDDeTicket}")
            Debug.WriteLine($"Código de barra: {ticket.CodigoDeBarra}")
            Debug.WriteLine($"Tipo de apuesta: {ticket.TipoDeApuesta}")

            For Each apuesta In ticket.Apuestas
                Debug.WriteLine("Detalle de Apuesta:")
                Debug.WriteLine($"  Juego: {apuesta.Juego}")
                Debug.WriteLine($"  ID de la Ronda: {apuesta.IDDeLaRonda}")
                Debug.WriteLine($"  Ganancia: {apuesta.Ganancia}")
                Debug.WriteLine($"  Monto de la Apuesta: {apuesta.MontoDeLaApuesta}")
                Debug.WriteLine($"  Ganancia Posible: {apuesta.GananciaPosible}")
                Debug.WriteLine($"  Número: {apuesta.Numero}")
                Debug.WriteLine($"  Apuesta Total: {ticket.ApuestaTotal}")
                Debug.WriteLine($"  Ganancia Total Posible: {ticket.GananciaTotalPosible}")


                Debug.WriteLine($"  Segundo: {apuesta.Segundo}")
                Debug.WriteLine($"  Tercero: {apuesta.Tercero}")
                Debug.WriteLine($"  Primero o Tercero: {apuesta.PrimeroOTercero}")
                Debug.WriteLine($"  Segundo o Tercero: {apuesta.SegundoOTercero}")
                Debug.WriteLine($"  Lugar: {apuesta.Lugar}")
                Debug.WriteLine($"  Mostrar: {apuesta.Mostrar}")
                Debug.WriteLine($"  Amarillo: {apuesta.Amarillo}")
                Debug.WriteLine($"  Verde: {apuesta.Verde}")
                Debug.WriteLine($"  Rojo: {apuesta.Rojo}")
                Debug.WriteLine($"  Azul: {apuesta.Azul}")
                Debug.WriteLine($"  Azul: {apuesta.Ganador}")
                Debug.WriteLine($"  Azul: {apuesta.TotalProbabilidades}")
            Next

            currentTicket = ticket

            ' Configuración de impresión
            Dim printer As New TicketPrinter(
            ticket,
            My.Settings.PaperWidth,
            My.Settings.SelectedPrinter,
            My.Settings.SelectedLogo,
            My.Settings.SelectedUser,
            My.Settings.FinalMessage
        )
            printer.PrintAutomatically()

        Catch ex As Exception
            Debug.WriteLine($"Error procesando datos del ticket: {ex.Message}")
        End Try
    End Sub


    Private Sub OpenPrintSettings()
        Using settingsForm As New FormPrintSettings()

            If Not String.IsNullOrEmpty(My.Settings.SelectedPrinter) Then
                settingsForm.SelectedPrinter = My.Settings.SelectedPrinter
            End If


            settingsForm.PaperWidth = My.Settings.PaperWidth


            If settingsForm.ShowDialog() = DialogResult.OK Then

                Debug.WriteLine("Configuración de impresión actualizada.")
            End If
        End Using
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Dim config As AppConfig = LoadConfiguration()
        If config Is Nothing Then config = New AppConfig()

        config.SelectedPrinter = My.Settings.SelectedPrinter
        config.PaperWidth = My.Settings.PaperWidth
        config.SelectedLogo = My.Settings.SelectedLogo
        config.SelectedUser = My.Settings.SelectedUser
        config.FinalMessage = My.Settings.FinalMessage
        config.IsTerminal = My.Settings.IsTerminal
        config.IsBacklot = My.Settings.IsBacklot

        SaveConfiguration(config)

    End Sub

    Private Sub OpenBacklotForm()
        Try

            For Each frm As Form In Application.OpenForms
                If TypeOf frm Is backlot Then
                    frm.Activate()
                    Debug.WriteLine("Formulario backlot ya está abierto. Activándolo.")
                    Return
                End If
            Next


            Dim backlotForm As New backlot()


            backlotForm.Show()

            Debug.WriteLine("Formulario backlot abierto.")
        Catch ex As Exception
            MessageBox.Show($"Error al abrir el formulario backlot: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Debug.WriteLine($"Error al abrir el formulario backlot: {ex.Message}")
        End Try
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            Debug.WriteLine("Tecla F2 detectada.")
            OpenPrintSettings()
            e.Handled = True
        End If
    End Sub

    Private Sub btnATerminal_Click(sender As Object, e As EventArgs) Handles btnATerminal.Click
        OpenBacklotForm()
    End Sub

    Private Sub btnOTerminal_Click(sender As Object, e As EventArgs) Handles btnOTerminal.Click
        OpenBacklotForm()
    End Sub


End Class