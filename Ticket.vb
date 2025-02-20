Public Class Ticket
    Public Property Fecha As String
    Public Property IDDeTicket As String
    Public Property CodigoDeBarra As String
    Public Property TipoDeApuesta As String
    Public Property Apuestas As List(Of Apuesta)
    Public Property ApuestaTotal As String
    Public Property GananciaTotalPosible As String

    Public Class Apuesta
        Public Property Juego As String
        Public Property IDDeLaRonda As String
        Public Property Ganancia As String
        Public Property MontoDeLaApuesta As String
        Public Property GananciaPosible As String
        Public Property Numero As String
        Public Property Par As String
        Public Property Impar As String
        Public Property NumeroGanador As String
        Public Property PronosticoDirecto As String
        Public Property TrifectaDirecta As String
        Public Property PronosticoReverso As String
        Public Property Menos As String
        Public Property Mas As String

        Public Property Segundo As String

        Public Property Tercero As String
        Public Property PrimeroOTercero As String
        Public Property SegundoOTercero As String
        Public Property Lugar As String
        Public Property Mostrar As String
        Public Property Amarillo As String
        Public Property Rojo As String
        Public Property Verde As String
        Public Property Azul As String

        Public Property Ganador As String
        Public Property TotalProbabilidades As String

        Public Property Place As String



    End Class
End Class
