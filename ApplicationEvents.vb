Imports System.Threading
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    Partial Friend Class MyApplication
        Private Shared appMutex As Mutex
        Private Shared createdNew As Boolean

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                ' Intentar crear el mutex
                appMutex = New Mutex(True, "Global\VirtualesAppMutex", createdNew)

                If Not createdNew Then
                    ' Si ya está activo, cerrar la nueva instancia
                    MessageBox.Show("La aplicación ya está en ejecución.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    e.Cancel = True
                    Return
                End If
            Catch ex As Exception
                ' Manejo de errores en la inicialización
                MessageBox.Show("Error al inicializar la aplicación: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                e.Cancel = True
            End Try
        End Sub

        Private Sub MyApplication_Shutdown(sender As Object, e As EventArgs) Handles Me.Shutdown
            Try

                If appMutex IsNot Nothing Then
                    appMutex.ReleaseMutex()
                    appMutex.Dispose()
                    appMutex = Nothing
                End If
            Catch ex As Exception

                MessageBox.Show("Error al cerrar la aplicación: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
