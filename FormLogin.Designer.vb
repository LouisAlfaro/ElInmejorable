<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormLogin
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
        txtClave = New TextBox()
        Label1 = New Label()
        btnAceptar = New Button()
        btnCancelar = New Button()
        SuspendLayout()
        ' 
        ' txtClave
        ' 
        txtClave.BackColor = Color.FromArgb(CByte(255), CByte(195), CByte(62))
        txtClave.Location = New Point(40, 138)
        txtClave.Name = "txtClave"
        txtClave.PasswordChar = "*"c
        txtClave.Size = New Size(289, 27)
        txtClave.TabIndex = 0
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.ForeColor = Color.FromArgb(CByte(255), CByte(195), CByte(62))
        Label1.Location = New Point(27, 89)
        Label1.Name = "Label1"
        Label1.Size = New Size(313, 20)
        Label1.TabIndex = 1
        Label1.Text = "Ingrese la contraseña para abrir configuración"
        ' 
        ' btnAceptar
        ' 
        btnAceptar.BackColor = Color.FromArgb(CByte(255), CByte(195), CByte(62))
        btnAceptar.Location = New Point(40, 190)
        btnAceptar.Name = "btnAceptar"
        btnAceptar.Size = New Size(94, 29)
        btnAceptar.TabIndex = 2
        btnAceptar.Text = "Aceptar"
        btnAceptar.UseVisualStyleBackColor = False
        ' 
        ' btnCancelar
        ' 
        btnCancelar.BackColor = Color.FromArgb(CByte(255), CByte(195), CByte(62))
        btnCancelar.Location = New Point(226, 190)
        btnCancelar.Name = "btnCancelar"
        btnCancelar.Size = New Size(94, 29)
        btnCancelar.TabIndex = 3
        btnCancelar.Text = "Cancelar"
        btnCancelar.UseVisualStyleBackColor = False
        ' 
        ' FormLogin
        ' 
        AutoScaleDimensions = New SizeF(8.0F, 20.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(27), CByte(27), CByte(27))
        ClientSize = New Size(382, 353)
        Controls.Add(btnCancelar)
        Controls.Add(btnAceptar)
        Controls.Add(Label1)
        Controls.Add(txtClave)
        Name = "FormLogin"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Login"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtClave As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnAceptar As Button
    Friend WithEvents btnCancelar As Button
End Class
