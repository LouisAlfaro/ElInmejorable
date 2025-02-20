<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormPrintSettings
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        cmbPrinters = New ComboBox()
        Label1 = New Label()
        rb58mm = New RadioButton()
        rb80mm = New RadioButton()
        btnOK = New Button()
        btnCancel = New Button()
        Label2 = New Label()
        picLogo = New PictureBox()
        Label3 = New Label()
        txtUsuario = New TextBox()
        txtMensajeFinal = New TextBox()
        Label4 = New Label()
        cmbMode = New ComboBox()
        Label6 = New Label()
        chkImprimirUsuario = New CheckBox()
        chkImprimirMensajeFinal = New CheckBox()
        cmbBacklot = New ComboBox()
        Label5 = New Label()
        CType(picLogo, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' cmbPrinters
        ' 
        cmbPrinters.DropDownStyle = ComboBoxStyle.DropDownList
        cmbPrinters.FormattingEnabled = True
        cmbPrinters.Location = New Point(102, 34)
        cmbPrinters.Margin = New Padding(3, 2, 3, 2)
        cmbPrinters.Name = "cmbPrinters"
        cmbPrinters.Size = New Size(133, 23)
        cmbPrinters.TabIndex = 0
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(25, 37)
        Label1.Name = "Label1"
        Label1.Size = New Size(60, 15)
        Label1.TabIndex = 1
        Label1.Text = "Impresora"
        ' 
        ' rb58mm
        ' 
        rb58mm.AutoSize = True
        rb58mm.Location = New Point(25, 76)
        rb58mm.Margin = New Padding(3, 2, 3, 2)
        rb58mm.Name = "rb58mm"
        rb58mm.Size = New Size(115, 19)
        rb58mm.TabIndex = 2
        rb58mm.TabStop = True
        rb58mm.Text = "Impresion 58mm"
        rb58mm.UseVisualStyleBackColor = True
        ' 
        ' rb80mm
        ' 
        rb80mm.AutoSize = True
        rb80mm.Location = New Point(25, 110)
        rb80mm.Margin = New Padding(3, 2, 3, 2)
        rb80mm.Name = "rb80mm"
        rb80mm.Size = New Size(115, 19)
        rb80mm.TabIndex = 3
        rb80mm.TabStop = True
        rb80mm.Text = "Impresion 80mm"
        rb80mm.UseVisualStyleBackColor = True
        ' 
        ' btnOK
        ' 
        btnOK.Location = New Point(204, 307)
        btnOK.Margin = New Padding(3, 2, 3, 2)
        btnOK.Name = "btnOK"
        btnOK.Size = New Size(82, 22)
        btnOK.TabIndex = 4
        btnOK.Text = "Guardar"
        btnOK.UseVisualStyleBackColor = True
        ' 
        ' btnCancel
        ' 
        btnCancel.Location = New Point(361, 307)
        btnCancel.Margin = New Padding(3, 2, 3, 2)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New Size(82, 22)
        btnCancel.TabIndex = 5
        btnCancel.Text = "Cancelar"
        btnCancel.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(302, 44)
        Label2.Name = "Label2"
        Label2.Size = New Size(90, 15)
        Label2.TabIndex = 6
        Label2.Text = "Logo Impresión"
        ' 
        ' picLogo
        ' 
        picLogo.Location = New Point(414, 21)
        picLogo.Margin = New Padding(3, 2, 3, 2)
        picLogo.Name = "picLogo"
        picLogo.Size = New Size(130, 58)
        picLogo.SizeMode = PictureBoxSizeMode.StretchImage
        picLogo.TabIndex = 7
        picLogo.TabStop = False
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(314, 100)
        Label3.Name = "Label3"
        Label3.Size = New Size(47, 15)
        Label3.TabIndex = 9
        Label3.Text = "Usuario"
        ' 
        ' txtUsuario
        ' 
        txtUsuario.Location = New Point(414, 100)
        txtUsuario.Margin = New Padding(3, 2, 3, 2)
        txtUsuario.Name = "txtUsuario"
        txtUsuario.Size = New Size(130, 23)
        txtUsuario.TabIndex = 10
        ' 
        ' txtMensajeFinal
        ' 
        txtMensajeFinal.Location = New Point(414, 147)
        txtMensajeFinal.Margin = New Padding(3, 2, 3, 2)
        txtMensajeFinal.Name = "txtMensajeFinal"
        txtMensajeFinal.Size = New Size(130, 23)
        txtMensajeFinal.TabIndex = 11
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(314, 149)
        Label4.Name = "Label4"
        Label4.Size = New Size(79, 15)
        Label4.TabIndex = 12
        Label4.Text = "Mensaje Final"
        ' 
        ' cmbMode
        ' 
        cmbMode.DropDownStyle = ComboBoxStyle.DropDownList
        cmbMode.FormattingEnabled = True
        cmbMode.Location = New Point(557, 222)
        cmbMode.Margin = New Padding(3, 2, 3, 2)
        cmbMode.Name = "cmbMode"
        cmbMode.Size = New Size(133, 23)
        cmbMode.TabIndex = 17
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(486, 224)
        Label6.Name = "Label6"
        Label6.Size = New Size(47, 15)
        Label6.TabIndex = 18
        Label6.Text = "Usuario"
        ' 
        ' chkImprimirUsuario
        ' 
        chkImprimirUsuario.AutoSize = True
        chkImprimirUsuario.Location = New Point(276, 99)
        chkImprimirUsuario.Name = "chkImprimirUsuario"
        chkImprimirUsuario.Size = New Size(15, 14)
        chkImprimirUsuario.TabIndex = 21
        chkImprimirUsuario.UseVisualStyleBackColor = True
        ' 
        ' chkImprimirMensajeFinal
        ' 
        chkImprimirMensajeFinal.AutoSize = True
        chkImprimirMensajeFinal.Location = New Point(276, 149)
        chkImprimirMensajeFinal.Name = "chkImprimirMensajeFinal"
        chkImprimirMensajeFinal.Size = New Size(15, 14)
        chkImprimirMensajeFinal.TabIndex = 22
        chkImprimirMensajeFinal.UseVisualStyleBackColor = True
        ' 
        ' cmbBacklot
        ' 
        cmbBacklot.DropDownStyle = ComboBoxStyle.DropDownList
        cmbBacklot.FormattingEnabled = True
        cmbBacklot.Location = New Point(25, 287)
        cmbBacklot.Margin = New Padding(3, 2, 3, 2)
        cmbBacklot.Name = "cmbBacklot"
        cmbBacklot.Size = New Size(92, 23)
        cmbBacklot.TabIndex = 23
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(25, 270)
        Label5.Name = "Label5"
        Label5.Size = New Size(64, 15)
        Label5.TabIndex = 24
        Label5.Text = "Animalitos"
        ' 
        ' FormPrintSettings
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(700, 338)
        Controls.Add(Label5)
        Controls.Add(cmbBacklot)
        Controls.Add(chkImprimirMensajeFinal)
        Controls.Add(chkImprimirUsuario)
        Controls.Add(Label6)
        Controls.Add(cmbMode)
        Controls.Add(Label4)
        Controls.Add(txtMensajeFinal)
        Controls.Add(txtUsuario)
        Controls.Add(Label3)
        Controls.Add(picLogo)
        Controls.Add(Label2)
        Controls.Add(btnCancel)
        Controls.Add(btnOK)
        Controls.Add(rb80mm)
        Controls.Add(rb58mm)
        Controls.Add(Label1)
        Controls.Add(cmbPrinters)
        Margin = New Padding(3, 2, 3, 2)
        Name = "FormPrintSettings"
        Text = "FormPrintSettings"
        CType(picLogo, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cmbPrinters As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents rb58mm As RadioButton
    Friend WithEvents rb80mm As RadioButton
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents picLogo As PictureBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtUsuario As TextBox
    Friend WithEvents txtMensajeFinal As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents cmbMode As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents chkImprimirUsuario As CheckBox
    Friend WithEvents chkImprimirMensajeFinal As CheckBox
    Friend WithEvents cmbBacklot As ComboBox
    Friend WithEvents Label5 As Label
End Class
