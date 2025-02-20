<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        WebView21 = New Microsoft.Web.WebView2.WinForms.WebView2()
        panelOverlay = New Panel()
        btnATerminal = New Button()
        picBoxTerminal = New PictureBox()
        btnOTerminal = New Button()
        NotifyIcon1 = New NotifyIcon(components)
        ContextMenuStrip1 = New ContextMenuStrip(components)
        SalirToolStripMenuItem = New ToolStripMenuItem()
        pgCajero = New PictureBox()
        CType(WebView21, ComponentModel.ISupportInitialize).BeginInit()
        panelOverlay.SuspendLayout()
        CType(picBoxTerminal, ComponentModel.ISupportInitialize).BeginInit()
        ContextMenuStrip1.SuspendLayout()
        CType(pgCajero, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' WebView21
        ' 
        WebView21.AllowExternalDrop = True
        WebView21.CreationProperties = Nothing
        WebView21.DefaultBackgroundColor = Color.White
        WebView21.Dock = DockStyle.Fill
        WebView21.Location = New Point(0, 0)
        WebView21.Margin = New Padding(3, 2, 3, 2)
        WebView21.Name = "WebView21"
        WebView21.Size = New Size(700, 338)
        WebView21.TabIndex = 0
        WebView21.ZoomFactor = 1R
        ' 
        ' panelOverlay
        ' 
        panelOverlay.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        panelOverlay.BackColor = Color.FromArgb(CByte(27), CByte(27), CByte(27))
        panelOverlay.Controls.Add(btnATerminal)
        panelOverlay.Controls.Add(picBoxTerminal)
        panelOverlay.Location = New Point(0, 0)
        panelOverlay.Margin = New Padding(3, 2, 3, 2)
        panelOverlay.Name = "panelOverlay"
        panelOverlay.Size = New Size(700, 67)
        panelOverlay.TabIndex = 1
        panelOverlay.Visible = False
        ' 
        ' btnATerminal
        ' 
        btnATerminal.BackColor = Color.FromArgb(CByte(73), CByte(73), CByte(73))
        btnATerminal.ForeColor = Color.FromArgb(CByte(255), CByte(195), CByte(62))
        btnATerminal.Location = New Point(47, 9)
        btnATerminal.Margin = New Padding(3, 2, 3, 2)
        btnATerminal.Name = "btnATerminal"
        btnATerminal.Size = New Size(85, 40)
        btnATerminal.TabIndex = 1
        btnATerminal.Text = "Animalitos"
        btnATerminal.UseVisualStyleBackColor = False
        btnATerminal.Visible = False
        ' 
        ' picBoxTerminal
        ' 
        picBoxTerminal.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        picBoxTerminal.BackgroundImageLayout = ImageLayout.Stretch
        picBoxTerminal.Location = New Point(156, 2)
        picBoxTerminal.Margin = New Padding(0)
        picBoxTerminal.Name = "picBoxTerminal"
        picBoxTerminal.Padding = New Padding(7, 6, 7, 6)
        picBoxTerminal.Size = New Size(396, 67)
        picBoxTerminal.SizeMode = PictureBoxSizeMode.CenterImage
        picBoxTerminal.TabIndex = 0
        picBoxTerminal.TabStop = False
        picBoxTerminal.Visible = False
        ' 
        ' btnOTerminal
        ' 
        btnOTerminal.BackColor = Color.FromArgb(CByte(73), CByte(73), CByte(73))
        btnOTerminal.BackgroundImageLayout = ImageLayout.Zoom
        btnOTerminal.ForeColor = Color.FromArgb(CByte(255), CByte(195), CByte(62))
        btnOTerminal.Location = New Point(485, 9)
        btnOTerminal.Margin = New Padding(3, 2, 3, 2)
        btnOTerminal.Name = "btnOTerminal"
        btnOTerminal.Size = New Size(102, 34)
        btnOTerminal.TabIndex = 2
        btnOTerminal.Text = "Animalitos"
        btnOTerminal.UseVisualStyleBackColor = False
        btnOTerminal.Visible = False
        ' 
        ' NotifyIcon1
        ' 
        NotifyIcon1.ContextMenuStrip = ContextMenuStrip1
        NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), Icon)
        NotifyIcon1.Text = "El Inmejorable"
        NotifyIcon1.Visible = True
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.ImageScalingSize = New Size(20, 20)
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {SalirToolStripMenuItem})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(97, 26)
        ' 
        ' SalirToolStripMenuItem
        ' 
        SalirToolStripMenuItem.Name = "SalirToolStripMenuItem"
        SalirToolStripMenuItem.Size = New Size(96, 22)
        SalirToolStripMenuItem.Text = "Salir"
        ' 
        ' pgCajero
        ' 
        pgCajero.Anchor = AnchorStyles.Top
        pgCajero.BackColor = Color.FromArgb(CByte(27), CByte(27), CByte(27))
        pgCajero.BackgroundImageLayout = ImageLayout.Stretch
        pgCajero.Location = New Point(156, 2)
        pgCajero.Margin = New Padding(0)
        pgCajero.Name = "pgCajero"
        pgCajero.Padding = New Padding(7, 6, 7, 6)
        pgCajero.Size = New Size(396, 55)
        pgCajero.SizeMode = PictureBoxSizeMode.CenterImage
        pgCajero.TabIndex = 3
        pgCajero.TabStop = False
        pgCajero.Visible = False
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(700, 338)
        Controls.Add(panelOverlay)
        Controls.Add(pgCajero)
        Controls.Add(btnOTerminal)
        Controls.Add(WebView21)
        KeyPreview = True
        Margin = New Padding(3, 2, 3, 2)
        Name = "Form1"
        Text = "El inmejorable v1.4"
        CType(WebView21, ComponentModel.ISupportInitialize).EndInit()
        panelOverlay.ResumeLayout(False)
        CType(picBoxTerminal, ComponentModel.ISupportInitialize).EndInit()
        ContextMenuStrip1.ResumeLayout(False)
        CType(pgCajero, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents WebView21 As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents panelOverlay As Panel
    Friend WithEvents picBoxTerminal As PictureBox
    Friend WithEvents NotifyIcon1 As NotifyIcon
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents SalirToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnOTerminal As Button
    Friend WithEvents btnATerminal As Button
    Friend WithEvents pgCajero As PictureBox

End Class
