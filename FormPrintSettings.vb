Imports System.Drawing.Printing
Imports System.IO



Public Class FormPrintSettings
    Public Property SelectedLogo As String
    Public Property SelectedUser As String
    Public Property FinalMessage As String
    Public Property SelectedPrinter As String
    Public Property PaperWidth As Integer



    Private Sub FormPrintSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ShowInTaskbar = False

        cmbMode.Items.Add("Cajero")
        cmbMode.Items.Add("Terminal")

        cmbBacklot.Items.Add("Activado")
        cmbBacklot.Items.Add("Desactivado")

        'chkImprimirLogo.Checked = My.Settings.ImprimirLogo
        chkImprimirUsuario.Checked = My.Settings.ImprimirUsuario
        chkImprimirMensajeFinal.Checked = My.Settings.ImprimirMensajeFinal


        For Each printer In PrinterSettings.InstalledPrinters
            cmbPrinters.Items.Add(printer)
        Next


        If Not String.IsNullOrEmpty(My.Settings.SelectedPrinter) Then
            cmbPrinters.SelectedItem = My.Settings.SelectedPrinter
        Else
            Dim defaultPrinter As New PrinterSettings()
            If PrinterSettings.InstalledPrinters.Cast(Of String)().Contains(defaultPrinter.PrinterName) Then
                cmbPrinters.SelectedItem = defaultPrinter.PrinterName
            ElseIf cmbPrinters.Items.Count > 0 Then
                cmbPrinters.SelectedIndex = 0
            End If
        End If

        If My.Settings.IsTerminal Then
            cmbMode.SelectedItem = "Terminal"
        Else
            cmbMode.SelectedItem = "Cajero"
        End If

        If My.Settings.IsBacklot Then
            cmbBacklot.SelectedItem = "Activado"
        Else
            cmbBacklot.SelectedItem = "Desactivado"
        End If

        UpdateControlsBasedOnMode()

        If My.Settings.PaperWidth = 58 Then
            rb58mm.Checked = True
        ElseIf My.Settings.PaperWidth = 80 Then
            rb80mm.Checked = True
        Else

            rb80mm.Checked = True
        End If


        Dim defaultLogoPath As String = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png")


        If String.IsNullOrEmpty(My.Settings.SelectedLogo) OrElse Not File.Exists(My.Settings.SelectedLogo) Then
            If File.Exists(defaultLogoPath) Then
                My.Settings.SelectedLogo = defaultLogoPath
            End If
        End If


        If Not String.IsNullOrEmpty(My.Settings.SelectedLogo) AndAlso File.Exists(My.Settings.SelectedLogo) Then
            picLogo.Image = Image.FromFile(My.Settings.SelectedLogo)
        Else
            picLogo.Image = Nothing
        End If


        txtUsuario.Text = My.Settings.SelectedUser


        txtMensajeFinal.Text = My.Settings.FinalMessage
    End Sub



    Private Sub UpdateControlsBasedOnMode()
        If cmbMode.SelectedItem.ToString() = "Terminal" Then

            cmbPrinters.Enabled = False
            rb58mm.Enabled = False
            rb80mm.Enabled = False



            cmbPrinters.SelectedItem = Nothing
            rb58mm.Checked = False
            rb80mm.Checked = False
        ElseIf cmbMode.SelectedItem.ToString() = "Cajero" Then

            cmbPrinters.Enabled = True
            rb58mm.Enabled = True
            rb80mm.Enabled = True
        End If
    End Sub



    Private Sub LoadPrinters()
        cmbPrinters.Items.Clear()
        For Each printer In PrinterSettings.InstalledPrinters
            cmbPrinters.Items.Add(printer)
        Next


        If Not String.IsNullOrEmpty(My.Settings.SelectedPrinter) AndAlso
       cmbPrinters.Items.Contains(My.Settings.SelectedPrinter) Then
            cmbPrinters.SelectedItem = My.Settings.SelectedPrinter
        ElseIf cmbPrinters.Items.Count > 0 Then
            cmbPrinters.SelectedIndex = 0
        End If
    End Sub



    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Try
            '            chkImprimirLogo.Checked OrElse si mas adelante lo uso lo wa poner esto

            If Not (chkImprimirUsuario.Checked OrElse chkImprimirMensajeFinal.Checked) Then
                MessageBox.Show("Por favor, selecciona al menos un elemento para imprimir.", "Configuración Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If


            'My.Settings.ImprimirLogo = chkImprimirLogo.Checked
            My.Settings.ImprimirUsuario = chkImprimirUsuario.Checked
            My.Settings.ImprimirMensajeFinal = chkImprimirMensajeFinal.Checked


            If cmbMode.SelectedItem.ToString() = "Terminal" Then
                My.Settings.IsTerminal = True
                My.Settings.SelectedPrinter = String.Empty
                My.Settings.PaperWidth = 0
            ElseIf cmbMode.SelectedItem.ToString() = "Cajero" Then
                My.Settings.IsTerminal = False


                If cmbPrinters.SelectedItem Is Nothing Then
                    MessageBox.Show("Por favor, selecciona una impresora.", "Selección de Impresora", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If


                My.Settings.SelectedPrinter = cmbPrinters.SelectedItem.ToString()
                If Not rb58mm.Checked AndAlso Not rb80mm.Checked Then
                    MessageBox.Show("Por favor, selecciona un tipo de impresión válido.", "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                If rb58mm.Checked Then
                    My.Settings.PaperWidth = 58
                ElseIf rb80mm.Checked Then
                    My.Settings.PaperWidth = 80
                End If
            Else
                MessageBox.Show("Por favor, selecciona un modo válido (Terminal o Cajero).", "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If cmbBacklot.SelectedItem.ToString() = "Activado" Then
                My.Settings.IsBacklot = True

            ElseIf cmbBacklot.SelectedItem.ToString() = "Desactivado" Then
                My.Settings.IsBacklot = False

            End If
            Debug.WriteLine(My.Settings.IsBacklot)


            If String.IsNullOrWhiteSpace(txtUsuario.Text) Then
                MessageBox.Show("Por favor, ingresa un nombre de usuario.", "Usuario Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            My.Settings.SelectedUser = txtUsuario.Text.Trim()


            If String.IsNullOrWhiteSpace(txtMensajeFinal.Text) Then
                MessageBox.Show("Por favor, ingresa un mensaje final.", "Mensaje Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            My.Settings.FinalMessage = txtMensajeFinal.Text.Trim()

            Dim defaultLogoPath As String = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png")

            If picLogo.Image IsNot Nothing AndAlso Not String.IsNullOrEmpty(SelectedLogo) AndAlso File.Exists(SelectedLogo) Then
                My.Settings.SelectedLogo = SelectedLogo
            ElseIf File.Exists(defaultLogoPath) Then
                My.Settings.SelectedLogo = defaultLogoPath
            Else
                MessageBox.Show("Por favor, selecciona un logo.", "Logo No Seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            DirectCast(Application.OpenForms("Form1"), Form1).updateButton(My.Settings.IsBacklot)

            DirectCast(Application.OpenForms("Form1"), Form1).UpdatePanelVisibility(My.Settings.IsTerminal, My.Settings.IsBacklot)


            My.Settings.Save()

            MessageBox.Show("Configuración guardada correctamente.", "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Ocurrió un error al guardar la configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub



    Private Sub btnSeleccionarLogo_Click(sender As Object, e As EventArgs)
        Dim openFileDialog As New OpenFileDialog
        openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png"

        If openFileDialog.ShowDialog = DialogResult.OK Then

            SelectedLogo = openFileDialog.FileName

            picLogo.Image = Image.FromFile(SelectedLogo)
        End If
    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs)
        Dim openFileDialog As New OpenFileDialog
        openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif"

        If openFileDialog.ShowDialog = DialogResult.OK Then
            Dim selectedImagePath = openFileDialog.FileName



            'picBoxPreview.Image = Image.FromFile(selectedImagePath)


            My.Settings.TerminalImagePath = selectedImagePath
            My.Settings.Save()

            MessageBox.Show("Imagen guardada correctamente.", "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub



    Private Sub cmbMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMode.SelectedIndexChanged
        UpdateControlsBasedOnMode()
    End Sub
End Class
