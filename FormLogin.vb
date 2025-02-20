Public Class FormLogin

    Private Const ClaveCorrecta As String = "betbet123"

    Private Sub btnAceptar_Click(sender As Object, e As EventArgs) Handles btnAceptar.Click
        If txtClave.Text = ClaveCorrecta Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            MessageBox.Show("Clave incorrecta. Intenta nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtClave.Clear()
            txtClave.Focus()
        End If
    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Me.DialogResult = DialogResult.Cancel ' Cancelar ingreso
        Me.Close()
    End Sub

    Private Sub FormLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ShowInTaskbar = False
    End Sub
End Class
