Public Class TickerRequest

    Private _Symbol As String

    Public Property Symbol() As String
        Get
            Return _Symbol
        End Get
        Set(ByVal value As String)
            _Symbol = value
        End Set
    End Property

    Private _StartDate As Date = Date.Now

    Public Property StartDate() As Date
        Get
            Return _StartDate
        End Get
        Set(ByVal value As Date)
            _StartDate = value
        End Set
    End Property

    Private _EndDate As Date = Date.Now

    Public Property EndDate() As Date
        Get
            Return _EndDate
        End Get
        Set(ByVal value As Date)
            _EndDate = value
        End Set
    End Property

End Class
