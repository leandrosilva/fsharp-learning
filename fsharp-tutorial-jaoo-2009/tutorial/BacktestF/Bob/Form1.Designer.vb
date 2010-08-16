<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
Me.components = New System.ComponentModel.Container
Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer
Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
Me.BindingNavigator1 = New System.Windows.Forms.BindingNavigator(Me.components)
Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton
Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel
Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton
Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton
Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton
Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator
Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox
Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator
Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton
Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton
Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator
Me.DataRepeater1 = New Microsoft.VisualBasic.PowerPacks.DataRepeater
Me.EndDateDateTimePicker = New System.Windows.Forms.DateTimePicker
Me.StartDateDateTimePicker = New System.Windows.Forms.DateTimePicker
Me.SymbolTextBox = New System.Windows.Forms.TextBox
Me.btnLoadAllTickers = New System.Windows.Forms.Button
Me.Label1 = New System.Windows.Forms.Label
Me.tabTickers = New System.Windows.Forms.TabControl
Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
Me.PrintToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.PrintPreviewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.UndoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.RedoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator
Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.CustomizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.ContentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.IndexToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.SearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator
Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
Me.NewToolStripButton = New System.Windows.Forms.ToolStripButton
Me.OpenToolStripButton = New System.Windows.Forms.ToolStripButton
Me.SaveToolStripButton = New System.Windows.Forms.ToolStripButton
Me.PrintToolStripButton = New System.Windows.Forms.ToolStripButton
Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator
Me.CutToolStripButton = New System.Windows.Forms.ToolStripButton
Me.CopyToolStripButton = New System.Windows.Forms.ToolStripButton
Me.PasteToolStripButton = New System.Windows.Forms.ToolStripButton
Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
Me.HelpToolStripButton = New System.Windows.Forms.ToolStripButton
Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
Me.CutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.PasteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.ToolStripContainer1.ContentPanel.SuspendLayout()
Me.ToolStripContainer1.TopToolStripPanel.SuspendLayout()
Me.ToolStripContainer1.SuspendLayout()
Me.SplitContainer1.Panel1.SuspendLayout()
Me.SplitContainer1.Panel2.SuspendLayout()
Me.SplitContainer1.SuspendLayout()
CType(Me.BindingNavigator1, System.ComponentModel.ISupportInitialize).BeginInit()
Me.BindingNavigator1.SuspendLayout()
Me.DataRepeater1.ItemTemplate.SuspendLayout()
Me.DataRepeater1.SuspendLayout()
Me.MenuStrip1.SuspendLayout()
Me.ToolStrip1.SuspendLayout()
CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
Me.SuspendLayout()
'
'ToolStripContainer1
'
'
'ToolStripContainer1.ContentPanel
'
Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.SplitContainer1)
Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(1020, 672)
Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
Me.ToolStripContainer1.Location = New System.Drawing.Point(0, 0)
Me.ToolStripContainer1.Name = "ToolStripContainer1"
Me.ToolStripContainer1.Size = New System.Drawing.Size(1020, 721)
Me.ToolStripContainer1.TabIndex = 0
Me.ToolStripContainer1.Text = "ToolStripContainer1"
'
'ToolStripContainer1.TopToolStripPanel
'
Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.MenuStrip1)
Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.ToolStrip1)
'
'SplitContainer1
'
Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
Me.SplitContainer1.Name = "SplitContainer1"
'
'SplitContainer1.Panel1
'
Me.SplitContainer1.Panel1.AutoScroll = True
Me.SplitContainer1.Panel1.Controls.Add(Me.BindingNavigator1)
Me.SplitContainer1.Panel1.Controls.Add(Me.DataRepeater1)
Me.SplitContainer1.Panel1.Controls.Add(Me.btnLoadAllTickers)
Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
'
'SplitContainer1.Panel2
'
Me.SplitContainer1.Panel2.Controls.Add(Me.tabTickers)
Me.SplitContainer1.Size = New System.Drawing.Size(1020, 672)
Me.SplitContainer1.SplitterDistance = 407
Me.SplitContainer1.TabIndex = 0
'
'BindingNavigator1
'
Me.BindingNavigator1.AddNewItem = Me.BindingNavigatorAddNewItem
Me.BindingNavigator1.CountItem = Me.BindingNavigatorCountItem
Me.BindingNavigator1.DeleteItem = Me.BindingNavigatorDeleteItem
Me.BindingNavigator1.Dock = System.Windows.Forms.DockStyle.Bottom
Me.BindingNavigator1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem})
Me.BindingNavigator1.Location = New System.Drawing.Point(0, 43)
Me.BindingNavigator1.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
Me.BindingNavigator1.MoveLastItem = Me.BindingNavigatorMoveLastItem
Me.BindingNavigator1.MoveNextItem = Me.BindingNavigatorMoveNextItem
Me.BindingNavigator1.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
Me.BindingNavigator1.Name = "BindingNavigator1"
Me.BindingNavigator1.PositionItem = Me.BindingNavigatorPositionItem
Me.BindingNavigator1.Size = New System.Drawing.Size(407, 25)
Me.BindingNavigator1.TabIndex = 4
Me.BindingNavigator1.Text = "BindingNavigator1"
'
'BindingNavigatorAddNewItem
'
Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.BindingNavigatorAddNewItem.Image = CType(resources.GetObject("BindingNavigatorAddNewItem.Image"), System.Drawing.Image)
Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
Me.BindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True
Me.BindingNavigatorAddNewItem.Size = New System.Drawing.Size(23, 22)
Me.BindingNavigatorAddNewItem.Text = "Add new"
'
'BindingNavigatorCountItem
'
Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(35, 22)
Me.BindingNavigatorCountItem.Text = "of {0}"
Me.BindingNavigatorCountItem.ToolTipText = "Total number of items"
'
'BindingNavigatorDeleteItem
'
Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"), System.Drawing.Image)
Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(23, 22)
Me.BindingNavigatorDeleteItem.Text = "Delete"
'
'BindingNavigatorMoveFirstItem
'
Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"), System.Drawing.Image)
Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(23, 22)
Me.BindingNavigatorMoveFirstItem.Text = "Move first"
'
'BindingNavigatorMovePreviousItem
'
Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"), System.Drawing.Image)
Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(23, 22)
Me.BindingNavigatorMovePreviousItem.Text = "Move previous"
'
'BindingNavigatorSeparator
'
Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 25)
'
'BindingNavigatorPositionItem
'
Me.BindingNavigatorPositionItem.AccessibleName = "Position"
Me.BindingNavigatorPositionItem.AutoSize = False
Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(50, 23)
Me.BindingNavigatorPositionItem.Text = "0"
Me.BindingNavigatorPositionItem.ToolTipText = "Current position"
'
'BindingNavigatorSeparator1
'
Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 25)
'
'BindingNavigatorMoveNextItem
'
Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"), System.Drawing.Image)
Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(23, 22)
Me.BindingNavigatorMoveNextItem.Text = "Move next"
'
'BindingNavigatorMoveLastItem
'
Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"), System.Drawing.Image)
Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(23, 22)
Me.BindingNavigatorMoveLastItem.Text = "Move last"
'
'BindingNavigatorSeparator2
'
Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 25)
'
'DataRepeater1
'
Me.DataRepeater1.Dock = System.Windows.Forms.DockStyle.Bottom
'
'DataRepeater1.ItemTemplate
'
Me.DataRepeater1.ItemTemplate.Controls.Add(Me.EndDateDateTimePicker)
Me.DataRepeater1.ItemTemplate.Controls.Add(Me.StartDateDateTimePicker)
Me.DataRepeater1.ItemTemplate.Controls.Add(Me.SymbolTextBox)
Me.DataRepeater1.ItemTemplate.Size = New System.Drawing.Size(399, 29)
Me.DataRepeater1.Location = New System.Drawing.Point(0, 68)
Me.DataRepeater1.Name = "DataRepeater1"
Me.DataRepeater1.Size = New System.Drawing.Size(407, 554)
Me.DataRepeater1.TabIndex = 3
Me.DataRepeater1.Text = "DataRepeater1"
'
'EndDateDateTimePicker
'
Me.EndDateDateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.BindingSource1, "EndDate", True))
Me.EndDateDateTimePicker.Location = New System.Drawing.Point(254, 3)
Me.EndDateDateTimePicker.Name = "EndDateDateTimePicker"
Me.EndDateDateTimePicker.Size = New System.Drawing.Size(200, 20)
Me.EndDateDateTimePicker.TabIndex = 1
'
'StartDateDateTimePicker
'
Me.StartDateDateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.BindingSource1, "StartDate", True))
Me.StartDateDateTimePicker.Location = New System.Drawing.Point(48, 3)
Me.StartDateDateTimePicker.Name = "StartDateDateTimePicker"
Me.StartDateDateTimePicker.Size = New System.Drawing.Size(200, 20)
Me.StartDateDateTimePicker.TabIndex = 3
'
'SymbolTextBox
'
Me.SymbolTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BindingSource1, "Symbol", True))
Me.SymbolTextBox.Location = New System.Drawing.Point(3, 3)
Me.SymbolTextBox.Name = "SymbolTextBox"
Me.SymbolTextBox.Size = New System.Drawing.Size(39, 20)
Me.SymbolTextBox.TabIndex = 5
'
'btnLoadAllTickers
'
Me.btnLoadAllTickers.Dock = System.Windows.Forms.DockStyle.Bottom
Me.btnLoadAllTickers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
Me.btnLoadAllTickers.Location = New System.Drawing.Point(0, 622)
Me.btnLoadAllTickers.Name = "btnLoadAllTickers"
Me.btnLoadAllTickers.Size = New System.Drawing.Size(407, 50)
Me.btnLoadAllTickers.TabIndex = 2
Me.btnLoadAllTickers.Text = "&Load All Tickers"
Me.btnLoadAllTickers.UseVisualStyleBackColor = True
'
'Label1
'
Me.Label1.AutoSize = True
Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
Me.Label1.Location = New System.Drawing.Point(12, 15)
Me.Label1.Name = "Label1"
Me.Label1.Size = New System.Drawing.Size(199, 20)
Me.Label1.TabIndex = 0
Me.Label1.Text = "&Enter Ticker and Dates:"
'
'tabTickers
'
Me.tabTickers.Dock = System.Windows.Forms.DockStyle.Fill
Me.tabTickers.Location = New System.Drawing.Point(0, 0)
Me.tabTickers.Name = "tabTickers"
Me.tabTickers.SelectedIndex = 0
Me.tabTickers.Size = New System.Drawing.Size(609, 672)
Me.tabTickers.TabIndex = 0
'
'MenuStrip1
'
Me.MenuStrip1.Dock = System.Windows.Forms.DockStyle.None
Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.HelpToolStripMenuItem})
Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
Me.MenuStrip1.Name = "MenuStrip1"
Me.MenuStrip1.Size = New System.Drawing.Size(1020, 24)
Me.MenuStrip1.TabIndex = 1
Me.MenuStrip1.Text = "MenuStrip1"
'
'FileToolStripMenuItem
'
Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.OpenToolStripMenuItem, Me.toolStripSeparator2, Me.SaveToolStripMenuItem, Me.SaveAsToolStripMenuItem, Me.toolStripSeparator3, Me.PrintToolStripMenuItem, Me.PrintPreviewToolStripMenuItem, Me.toolStripSeparator4, Me.ExitToolStripMenuItem})
Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
Me.FileToolStripMenuItem.Text = "&File"
'
'NewToolStripMenuItem
'
Me.NewToolStripMenuItem.Image = CType(resources.GetObject("NewToolStripMenuItem.Image"), System.Drawing.Image)
Me.NewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
Me.NewToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
Me.NewToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
Me.NewToolStripMenuItem.Text = "&New"
'
'OpenToolStripMenuItem
'
Me.OpenToolStripMenuItem.Image = CType(resources.GetObject("OpenToolStripMenuItem.Image"), System.Drawing.Image)
Me.OpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
Me.OpenToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
Me.OpenToolStripMenuItem.Text = "&Open"
'
'toolStripSeparator2
'
Me.toolStripSeparator2.Name = "toolStripSeparator2"
Me.toolStripSeparator2.Size = New System.Drawing.Size(143, 6)
'
'SaveToolStripMenuItem
'
Me.SaveToolStripMenuItem.Image = CType(resources.GetObject("SaveToolStripMenuItem.Image"), System.Drawing.Image)
Me.SaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
Me.SaveToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
Me.SaveToolStripMenuItem.Text = "&Save"
'
'SaveAsToolStripMenuItem
'
Me.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem"
Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
Me.SaveAsToolStripMenuItem.Text = "Save &As"
'
'toolStripSeparator3
'
Me.toolStripSeparator3.Name = "toolStripSeparator3"
Me.toolStripSeparator3.Size = New System.Drawing.Size(143, 6)
'
'PrintToolStripMenuItem
'
Me.PrintToolStripMenuItem.Image = CType(resources.GetObject("PrintToolStripMenuItem.Image"), System.Drawing.Image)
Me.PrintToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.PrintToolStripMenuItem.Name = "PrintToolStripMenuItem"
Me.PrintToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.P), System.Windows.Forms.Keys)
Me.PrintToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
Me.PrintToolStripMenuItem.Text = "&Print"
'
'PrintPreviewToolStripMenuItem
'
Me.PrintPreviewToolStripMenuItem.Image = CType(resources.GetObject("PrintPreviewToolStripMenuItem.Image"), System.Drawing.Image)
Me.PrintPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.PrintPreviewToolStripMenuItem.Name = "PrintPreviewToolStripMenuItem"
Me.PrintPreviewToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
Me.PrintPreviewToolStripMenuItem.Text = "Print Pre&view"
'
'toolStripSeparator4
'
Me.toolStripSeparator4.Name = "toolStripSeparator4"
Me.toolStripSeparator4.Size = New System.Drawing.Size(143, 6)
'
'ExitToolStripMenuItem
'
Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
Me.ExitToolStripMenuItem.Text = "E&xit"
'
'EditToolStripMenuItem
'
Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UndoToolStripMenuItem, Me.RedoToolStripMenuItem, Me.toolStripSeparator5, Me.CutToolStripMenuItem, Me.CopyToolStripMenuItem, Me.PasteToolStripMenuItem, Me.toolStripSeparator6, Me.SelectAllToolStripMenuItem})
Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
Me.EditToolStripMenuItem.Text = "&Edit"
'
'UndoToolStripMenuItem
'
Me.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem"
Me.UndoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
Me.UndoToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.UndoToolStripMenuItem.Text = "&Undo"
'
'RedoToolStripMenuItem
'
Me.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem"
Me.RedoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
Me.RedoToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.RedoToolStripMenuItem.Text = "&Redo"
'
'toolStripSeparator5
'
Me.toolStripSeparator5.Name = "toolStripSeparator5"
Me.toolStripSeparator5.Size = New System.Drawing.Size(149, 6)
'
'CopyToolStripMenuItem
'
Me.CopyToolStripMenuItem.Image = CType(resources.GetObject("CopyToolStripMenuItem.Image"), System.Drawing.Image)
Me.CopyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
Me.CopyToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.CopyToolStripMenuItem.Text = "&Copy"
'
'toolStripSeparator6
'
Me.toolStripSeparator6.Name = "toolStripSeparator6"
Me.toolStripSeparator6.Size = New System.Drawing.Size(149, 6)
'
'SelectAllToolStripMenuItem
'
Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.SelectAllToolStripMenuItem.Text = "Select &All"
'
'ToolsToolStripMenuItem
'
Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CustomizeToolStripMenuItem, Me.OptionsToolStripMenuItem})
Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
Me.ToolsToolStripMenuItem.Text = "&Tools"
'
'CustomizeToolStripMenuItem
'
Me.CustomizeToolStripMenuItem.Name = "CustomizeToolStripMenuItem"
Me.CustomizeToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.CustomizeToolStripMenuItem.Text = "&Customize"
'
'OptionsToolStripMenuItem
'
Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.OptionsToolStripMenuItem.Text = "&Options"
'
'HelpToolStripMenuItem
'
Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContentsToolStripMenuItem, Me.IndexToolStripMenuItem, Me.SearchToolStripMenuItem, Me.toolStripSeparator7, Me.AboutToolStripMenuItem})
Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
Me.HelpToolStripMenuItem.Text = "&Help"
'
'ContentsToolStripMenuItem
'
Me.ContentsToolStripMenuItem.Name = "ContentsToolStripMenuItem"
Me.ContentsToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
Me.ContentsToolStripMenuItem.Text = "&Contents"
'
'IndexToolStripMenuItem
'
Me.IndexToolStripMenuItem.Name = "IndexToolStripMenuItem"
Me.IndexToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
Me.IndexToolStripMenuItem.Text = "&Index"
'
'SearchToolStripMenuItem
'
Me.SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
Me.SearchToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
Me.SearchToolStripMenuItem.Text = "&Search"
'
'toolStripSeparator7
'
Me.toolStripSeparator7.Name = "toolStripSeparator7"
Me.toolStripSeparator7.Size = New System.Drawing.Size(119, 6)
'
'AboutToolStripMenuItem
'
Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
Me.AboutToolStripMenuItem.Text = "&About..."
'
'ToolStrip1
'
Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripButton, Me.OpenToolStripButton, Me.SaveToolStripButton, Me.PrintToolStripButton, Me.toolStripSeparator, Me.CutToolStripButton, Me.CopyToolStripButton, Me.PasteToolStripButton, Me.toolStripSeparator1, Me.HelpToolStripButton})
Me.ToolStrip1.Location = New System.Drawing.Point(3, 24)
Me.ToolStrip1.Name = "ToolStrip1"
Me.ToolStrip1.Size = New System.Drawing.Size(208, 25)
Me.ToolStrip1.TabIndex = 0
'
'NewToolStripButton
'
Me.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.NewToolStripButton.Image = CType(resources.GetObject("NewToolStripButton.Image"), System.Drawing.Image)
Me.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.NewToolStripButton.Name = "NewToolStripButton"
Me.NewToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.NewToolStripButton.Text = "&New"
'
'OpenToolStripButton
'
Me.OpenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.OpenToolStripButton.Image = CType(resources.GetObject("OpenToolStripButton.Image"), System.Drawing.Image)
Me.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.OpenToolStripButton.Name = "OpenToolStripButton"
Me.OpenToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.OpenToolStripButton.Text = "&Open"
'
'SaveToolStripButton
'
Me.SaveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.SaveToolStripButton.Image = CType(resources.GetObject("SaveToolStripButton.Image"), System.Drawing.Image)
Me.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.SaveToolStripButton.Name = "SaveToolStripButton"
Me.SaveToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.SaveToolStripButton.Text = "&Save"
'
'PrintToolStripButton
'
Me.PrintToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.PrintToolStripButton.Image = CType(resources.GetObject("PrintToolStripButton.Image"), System.Drawing.Image)
Me.PrintToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.PrintToolStripButton.Name = "PrintToolStripButton"
Me.PrintToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.PrintToolStripButton.Text = "&Print"
'
'toolStripSeparator
'
Me.toolStripSeparator.Name = "toolStripSeparator"
Me.toolStripSeparator.Size = New System.Drawing.Size(6, 25)
'
'CutToolStripButton
'
Me.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.CutToolStripButton.Image = CType(resources.GetObject("CutToolStripButton.Image"), System.Drawing.Image)
Me.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.CutToolStripButton.Name = "CutToolStripButton"
Me.CutToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.CutToolStripButton.Text = "C&ut"
'
'CopyToolStripButton
'
Me.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.CopyToolStripButton.Image = CType(resources.GetObject("CopyToolStripButton.Image"), System.Drawing.Image)
Me.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.CopyToolStripButton.Name = "CopyToolStripButton"
Me.CopyToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.CopyToolStripButton.Text = "&Copy"
'
'PasteToolStripButton
'
Me.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.PasteToolStripButton.Image = CType(resources.GetObject("PasteToolStripButton.Image"), System.Drawing.Image)
Me.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.PasteToolStripButton.Name = "PasteToolStripButton"
Me.PasteToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.PasteToolStripButton.Text = "&Paste"
'
'toolStripSeparator1
'
Me.toolStripSeparator1.Name = "toolStripSeparator1"
Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 25)
'
'HelpToolStripButton
'
Me.HelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
Me.HelpToolStripButton.Image = CType(resources.GetObject("HelpToolStripButton.Image"), System.Drawing.Image)
Me.HelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
Me.HelpToolStripButton.Name = "HelpToolStripButton"
Me.HelpToolStripButton.Size = New System.Drawing.Size(23, 22)
Me.HelpToolStripButton.Text = "He&lp"
'
'BindingSource1
'
Me.BindingSource1.DataSource = GetType(Bob.TickerRequest)
'
'CutToolStripMenuItem
'
Me.CutToolStripMenuItem.Image = CType(resources.GetObject("CutToolStripMenuItem.Image"), System.Drawing.Image)
Me.CutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.CutToolStripMenuItem.Name = "CutToolStripMenuItem"
Me.CutToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
Me.CutToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.CutToolStripMenuItem.Text = "Cu&t"
'
'PasteToolStripMenuItem
'
Me.PasteToolStripMenuItem.Image = CType(resources.GetObject("PasteToolStripMenuItem.Image"), System.Drawing.Image)
Me.PasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
Me.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem"
Me.PasteToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
Me.PasteToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
Me.PasteToolStripMenuItem.Text = "&Paste"
'
'Form1
'
Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
Me.ClientSize = New System.Drawing.Size(1020, 721)
Me.Controls.Add(Me.ToolStripContainer1)
Me.Name = "Form1"
Me.Text = "Form1"
Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
Me.ToolStripContainer1.TopToolStripPanel.ResumeLayout(False)
Me.ToolStripContainer1.TopToolStripPanel.PerformLayout()
Me.ToolStripContainer1.ResumeLayout(False)
Me.ToolStripContainer1.PerformLayout()
Me.SplitContainer1.Panel1.ResumeLayout(False)
Me.SplitContainer1.Panel1.PerformLayout()
Me.SplitContainer1.Panel2.ResumeLayout(False)
Me.SplitContainer1.ResumeLayout(False)
CType(Me.BindingNavigator1, System.ComponentModel.ISupportInitialize).EndInit()
Me.BindingNavigator1.ResumeLayout(False)
Me.BindingNavigator1.PerformLayout()
Me.DataRepeater1.ItemTemplate.ResumeLayout(False)
Me.DataRepeater1.ItemTemplate.PerformLayout()
Me.DataRepeater1.ResumeLayout(False)
Me.MenuStrip1.ResumeLayout(False)
Me.MenuStrip1.PerformLayout()
Me.ToolStrip1.ResumeLayout(False)
Me.ToolStrip1.PerformLayout()
CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
Me.ResumeLayout(False)

End Sub
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents PrintToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PrintPreviewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UndoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RedoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SelectAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CustomizeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContentsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IndexToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SearchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents NewToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents OpenToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SaveToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents PrintToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents toolStripSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CutToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents CopyToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents PasteToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents HelpToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents btnLoadAllTickers As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DataRepeater1 As Microsoft.VisualBasic.PowerPacks.DataRepeater
    Friend WithEvents BindingNavigator1 As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingSource1 As System.Windows.Forms.BindingSource
    Friend WithEvents EndDateDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents StartDateDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents SymbolTextBox As System.Windows.Forms.TextBox
    Friend WithEvents tabTickers As System.Windows.Forms.TabControl
    Friend WithEvents CutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PasteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
