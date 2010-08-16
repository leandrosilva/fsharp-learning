Imports BackTest
Imports System.ComponentModel
Imports System.Threading


Public Class Form1

    Dim requestList As New List(Of TickerRequest)

    Sub Form1_Load() Handles MyBase.Load

        requestList.Add(New TickerRequest With {.Symbol = "MSFT", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "GOOG", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "YHOO", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "AAPL", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "IBM", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "KO", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "BAC", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "C", .StartDate = #1/1/1900#, .EndDate = Today})
        requestList.Add(New TickerRequest With {.Symbol = "MMM", .StartDate = #1/1/1900#, .EndDate = Today})

        Me.BindingSource1.DataSource = requestList
        Me.BindingNavigator1.BindingSource = Me.BindingSource1
        Me.DataRepeater1.DataSource = Me.BindingSource1
    End Sub


    Sub Button1_Click() Handles btnLoadAllTickers.Click

        'Dim request As TickerRequest = requestList(0)
        'Dim x = Funcs.DownloadTickerData(request.Symbol, request.StartDate, request.EndDate)


        For i = 1 To requestList.Count
            Dim request = requestList(i - 1)

            Dim bwTicker As New BackgroundWorker With {.WorkerReportsProgress = True}
            AddHandler bwTicker.DoWork, AddressOf LoadDataInBackground
            AddHandler bwTicker.ProgressChanged, AddressOf UpdateUI
            bwTicker.RunWorkerAsync(request)
        Next

    End Sub

    'Private Function GetGridOnTabPage(ByVal symbol As String, ByVal type As String, Optional ByVal LoadingText As String = "") As DataGridView

    '    If Me.tabTickers.TabPages.ContainsKey(symbol) Then
    '        Return Me.tabTickers.TabPages(symbol).Controls(type)
    '    Else

    '        Me.tabTickers.TabPages.Add(symbol, If(LoadingText <> "", LoadingText, UCase(symbol)))

    '        CreateGrid("Prices", symbol, 100, 100)
    '        CreateGrid("Dividends", symbol, 400, 100)
    '        CreateGrid("Splits", symbol, 400, 100)

    '    End If
    'End Function

    Private Sub CreateGrid(ByVal name As String, _
                           ByVal parent As Control, _
                           ByVal dockStyle As DockStyle)

        Dim dgv As New DataGridView With {.Name = name, _
                                          .Dock = dockStyle}

        parent.Controls.Add(dgv)
        dgv.MinimumSize = New Size(parent.Width / 2, 0)

    End Sub

    Private Sub CreateTabPage(ByVal symbol As String)
        Me.tabTickers.TabPages.Add(symbol, "Loading " & symbol & "...")

        Dim tabPage = Me.tabTickers.TabPages(symbol)
        Dim pnlDivSplit As New Panel With {.Name = "pnlDivSplit", _
                                           .Dock = DockStyle.Bottom}
        pnlDivSplit.Padding = New Padding(0, 10, 0, 0)
        pnlDivSplit.MinimumSize = New Size(tabPage.Width, tabPage.Height / 2.1)

        tabPage.Controls.Add(pnlDivSplit)

        CreateGrid("Prices", tabPage, DockStyle.Fill)
        CreateGrid("Dividends", pnlDivSplit, DockStyle.Left)
        CreateGrid("Splits", pnlDivSplit, DockStyle.Right)

        pnlDivSplit.SendToBack()
    End Sub


    Private Sub LoadDataIntoTabPage(ByVal symbol As String, ByVal priceData As IList(Of MyPrice), ByVal dividendData As IList(Of MyDividend), ByVal splitData As IList(Of MySplit))
        Dim tabPage = Me.tabTickers.TabPages(symbol)

        tabPage.Text = symbol


        CType(tabPage.Controls("Prices"), DataGridView).DataSource = priceData
        CType(tabPage.Controls("pnlDivSplit").Controls("Dividends"), DataGridView).DataSource = dividendData
        CType(tabPage.Controls("pnlDivSplit").Controls("Splits"), DataGridView).DataSource = splitData

        ReorderColumns(tabPage)

    End Sub

    Sub ReorderColumns(ByVal tabpage As TabPage)


        For Each grid In tabpage.Controls.OfType(Of DataGridView)()
            Dim count = grid.Columns.Count

            grid.AutoGenerateColumns = False

            For i = 0 To count - 1
                grid.Columns(i).DisplayIndex = count - 1 - i
            Next

        Next


    End Sub

    Private Sub UpdateUI(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)

        If e.ProgressPercentage = 10 Then
            CreateTabPage(e.UserState.Symbol)
        Else

            LoadDataIntoTabPage(e.UserState.Symbol, e.UserState.StockData.Prices, _
                                e.UserState.StockData.Dividends, e.UserState.StockData.Splits)

        End If

    End Sub

    Private Sub LoadDataInBackground(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim request = TryCast(e.Argument, TickerRequest)

        Dim stockData As TickerData

        Dim userState = New With {.Symbol = UCase(request.Symbol), _
                                  .StockData = stockData, _
                                  .ThreadStuff = Thread.CurrentThread.ManagedThreadId}

        CType(sender, BackgroundWorker).ReportProgress(10, userState)

        stockData = Funcs.DownloadTickerData(request.Symbol, request.StartDate, request.EndDate)

        userState.StockData = stockData


        CType(sender, BackgroundWorker).ReportProgress(70, userState)

    End Sub


    Private Sub CopyToolStripMenuItem_Click() Handles CopyToolStripMenuItem.Click

        Dim targetCtl = CType(ToolStripContainer1.ActiveControl, ContainerControl).ActiveControl

        If TypeOf targetCtl Is DataGridView Then
            My.Computer.Clipboard.SetDataObject(CType(targetCtl, DataGridView).GetClipboardContent())
        ElseIf TypeOf targetCtl Is PowerPacks.DataRepeater Then
            CType(CType(targetCtl, ContainerControl).ActiveControl, TextBox).Copy()
        End If

    End Sub
End Class
