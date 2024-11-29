<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmOpening
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOpening))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.QuickHelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpOnlineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CommunitySupportForumToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsWebsite = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsDonate = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsCopyrightAndLicense = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsCheckForUpdates = New System.Windows.Forms.ToolStripMenuItem()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripTip = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MultipleFileWatcherEventsTimer = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(4, 2, 0, 2)
        Me.MenuStrip1.Size = New System.Drawing.Size(494, 24)
        Me.MenuStrip1.TabIndex = 4
        Me.MenuStrip1.Text = "menuStrip1"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.AboutToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsHelp, Me.tsWebsite, Me.tsDonate, Me.tsCopyrightAndLicense, Me.ToolStripSeparator1, Me.tsCheckForUpdates})
        Me.AboutToolStripMenuItem.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AboutToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(93, 20)
        Me.AboutToolStripMenuItem.Text = "&About / Help"
        '
        'tsHelp
        '
        Me.tsHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.QuickHelpToolStripMenuItem, Me.HelpOnlineToolStripMenuItem, Me.CommunitySupportForumToolStripMenuItem})
        Me.tsHelp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsHelp.Name = "tsHelp"
        Me.tsHelp.Size = New System.Drawing.Size(263, 22)
        Me.tsHelp.Text = "&Help ..."
        '
        'QuickHelpToolStripMenuItem
        '
        Me.QuickHelpToolStripMenuItem.Name = "QuickHelpToolStripMenuItem"
        Me.QuickHelpToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.QuickHelpToolStripMenuItem.Text = "&Quick Help"
        '
        'HelpOnlineToolStripMenuItem
        '
        Me.HelpOnlineToolStripMenuItem.Name = "HelpOnlineToolStripMenuItem"
        Me.HelpOnlineToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.HelpOnlineToolStripMenuItem.Text = "&Online Help"
        '
        'CommunitySupportForumToolStripMenuItem
        '
        Me.CommunitySupportForumToolStripMenuItem.Name = "CommunitySupportForumToolStripMenuItem"
        Me.CommunitySupportForumToolStripMenuItem.Size = New System.Drawing.Size(231, 22)
        Me.CommunitySupportForumToolStripMenuItem.Text = "&Community Support Forum"
        '
        'tsWebsite
        '
        Me.tsWebsite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsWebsite.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsWebsite.Name = "tsWebsite"
        Me.tsWebsite.Size = New System.Drawing.Size(263, 22)
        Me.tsWebsite.Text = "&Website"
        '
        'tsDonate
        '
        Me.tsDonate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsDonate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsDonate.Name = "tsDonate"
        Me.tsDonate.Size = New System.Drawing.Size(263, 22)
        Me.tsDonate.Text = "&Donate"
        '
        'tsCopyrightAndLicense
        '
        Me.tsCopyrightAndLicense.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsCopyrightAndLicense.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsCopyrightAndLicense.Name = "tsCopyrightAndLicense"
        Me.tsCopyrightAndLicense.Size = New System.Drawing.Size(263, 22)
        Me.tsCopyrightAndLicense.Text = "&Copyright, License, Source and Credits"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(260, 6)
        '
        'tsCheckForUpdates
        '
        Me.tsCheckForUpdates.Checked = True
        Me.tsCheckForUpdates.CheckState = System.Windows.Forms.CheckState.Checked
        Me.tsCheckForUpdates.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsCheckForUpdates.Name = "tsCheckForUpdates"
        Me.tsCheckForUpdates.Size = New System.Drawing.Size(263, 22)
        Me.tsCheckForUpdates.Text = "&Automatically check for updates"
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), System.Drawing.Image)
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.PictureBox1.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.PictureBox1.InitialImage = Nothing
        Me.PictureBox1.Location = New System.Drawing.Point(1, 22)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(494, 174)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripTip})
        Me.StatusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 180)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 10, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(494, 5)
        Me.StatusStrip1.TabIndex = 6
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripTip
        '
        Me.ToolStripTip.BackColor = System.Drawing.SystemColors.Control
        Me.ToolStripTip.Name = "ToolStripTip"
        Me.ToolStripTip.Size = New System.Drawing.Size(0, 0)
        '
        'MultipleFileWatcherEventsTimer
        '
        '
        'frmOpening
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(494, 185)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MaximizeBox = False
        Me.Name = "frmOpening"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "A Viewer for Windows"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsWebsite As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsCopyrightAndLicense As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents tsHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsDonate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripTip As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsCheckForUpdates As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents QuickHelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpOnlineToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CommunitySupportForumToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MultipleFileWatcherEventsTimer As System.Windows.Forms.Timer
End Class
