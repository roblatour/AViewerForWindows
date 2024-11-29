<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmViewer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewer))
        Me.PictureBoxPrimary = New System.Windows.Forms.PictureBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusMirrored = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep00 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusAngle = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep01 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusZoom = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep02 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusInverted = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep03 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBrightness = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep04 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusContrast = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep05 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusGamma = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep06 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSaturation = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep07 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusGrayscale = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSep08 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusEffects = New System.Windows.Forms.ToolStripStatusLabel()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ToolStripView = New System.Windows.Forms.ToolStrip()
        Me.tsMirror = New System.Windows.Forms.ToolStripButton()
        Me.tsLevel = New System.Windows.Forms.ToolStripButton()
        Me.tsRotate = New System.Windows.Forms.ToolStripButton()
        Me.tsFit = New System.Windows.Forms.ToolStripButton()
        Me.tsZoom = New System.Windows.Forms.ToolStripButton()
        Me.tsCrop = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsInvert = New System.Windows.Forms.ToolStripButton()
        Me.tsBrightness = New System.Windows.Forms.ToolStripButton()
        Me.tsContrast = New System.Windows.Forms.ToolStripButton()
        Me.tsGamma = New System.Windows.Forms.ToolStripButton()
        Me.tsSaturation = New System.Windows.Forms.ToolStripButton()
        Me.tsGrayscale = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsEffects = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsReset = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsViewer = New System.Windows.Forms.ToolStripButton()
        Me.tsPinnedToTop = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsAR4W = New System.Windows.Forms.ToolStripButton()
        Me.tsMagnify = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsCopy = New System.Windows.Forms.ToolStripButton()
        Me.tsSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsHelp = New System.Windows.Forms.ToolStripButton()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PictureBoxOriginal = New System.Windows.Forms.PictureBox()
        Me.PictureBoxBottom = New System.Windows.Forms.PictureBox()
        Me.PictureBoxRight = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.PictureBoxPrimary, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ToolStripView.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBoxOriginal, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxBottom, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxRight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBoxPrimary
        '
        Me.PictureBoxPrimary.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxPrimary.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.PictureBoxPrimary.Location = New System.Drawing.Point(3, 3)
        Me.PictureBoxPrimary.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.PictureBoxPrimary.Name = "PictureBoxPrimary"
        Me.PictureBoxPrimary.Size = New System.Drawing.Size(544, 412)
        Me.PictureBoxPrimary.TabIndex = 0
        Me.PictureBoxPrimary.TabStop = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusMirrored, Me.ToolStripStatusSep00, Me.ToolStripStatusAngle, Me.ToolStripStatusSep01, Me.ToolStripStatusZoom, Me.ToolStripStatusSep02, Me.ToolStripStatusInverted, Me.ToolStripStatusSep03, Me.ToolStripStatusBrightness, Me.ToolStripStatusSep04, Me.ToolStripStatusContrast, Me.ToolStripStatusSep05, Me.ToolStripStatusGamma, Me.ToolStripStatusSep06, Me.ToolStripStatusSaturation, Me.ToolStripStatusSep07, Me.ToolStripStatusGrayscale, Me.ToolStripStatusSep08, Me.ToolStripStatusEffects})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 548)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 11, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(824, 22)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusMirrored
        '
        Me.ToolStripStatusMirrored.Name = "ToolStripStatusMirrored"
        Me.ToolStripStatusMirrored.Size = New System.Drawing.Size(53, 17)
        Me.ToolStripStatusMirrored.Text = "Mirrored"
        '
        'ToolStripStatusSep00
        '
        Me.ToolStripStatusSep00.Name = "ToolStripStatusSep00"
        Me.ToolStripStatusSep00.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep00.Text = "|"
        '
        'ToolStripStatusAngle
        '
        Me.ToolStripStatusAngle.Name = "ToolStripStatusAngle"
        Me.ToolStripStatusAngle.Size = New System.Drawing.Size(38, 17)
        Me.ToolStripStatusAngle.Text = "Angle"
        '
        'ToolStripStatusSep01
        '
        Me.ToolStripStatusSep01.Name = "ToolStripStatusSep01"
        Me.ToolStripStatusSep01.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep01.Text = "|"
        '
        'ToolStripStatusZoom
        '
        Me.ToolStripStatusZoom.Name = "ToolStripStatusZoom"
        Me.ToolStripStatusZoom.Size = New System.Drawing.Size(39, 17)
        Me.ToolStripStatusZoom.Text = "Zoom"
        '
        'ToolStripStatusSep02
        '
        Me.ToolStripStatusSep02.Name = "ToolStripStatusSep02"
        Me.ToolStripStatusSep02.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep02.Text = "|"
        '
        'ToolStripStatusInverted
        '
        Me.ToolStripStatusInverted.Name = "ToolStripStatusInverted"
        Me.ToolStripStatusInverted.Size = New System.Drawing.Size(50, 17)
        Me.ToolStripStatusInverted.Text = "Inverted"
        '
        'ToolStripStatusSep03
        '
        Me.ToolStripStatusSep03.Name = "ToolStripStatusSep03"
        Me.ToolStripStatusSep03.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep03.Text = "|"
        '
        'ToolStripStatusBrightness
        '
        Me.ToolStripStatusBrightness.Name = "ToolStripStatusBrightness"
        Me.ToolStripStatusBrightness.Size = New System.Drawing.Size(62, 17)
        Me.ToolStripStatusBrightness.Text = "Brightness"
        '
        'ToolStripStatusSep04
        '
        Me.ToolStripStatusSep04.Name = "ToolStripStatusSep04"
        Me.ToolStripStatusSep04.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep04.Text = "|"
        '
        'ToolStripStatusContrast
        '
        Me.ToolStripStatusContrast.Name = "ToolStripStatusContrast"
        Me.ToolStripStatusContrast.Size = New System.Drawing.Size(52, 17)
        Me.ToolStripStatusContrast.Text = "Contrast"
        '
        'ToolStripStatusSep05
        '
        Me.ToolStripStatusSep05.Name = "ToolStripStatusSep05"
        Me.ToolStripStatusSep05.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep05.Text = "|"
        '
        'ToolStripStatusGamma
        '
        Me.ToolStripStatusGamma.Name = "ToolStripStatusGamma"
        Me.ToolStripStatusGamma.Size = New System.Drawing.Size(49, 17)
        Me.ToolStripStatusGamma.Text = "Gamma"
        '
        'ToolStripStatusSep06
        '
        Me.ToolStripStatusSep06.Name = "ToolStripStatusSep06"
        Me.ToolStripStatusSep06.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep06.Text = "|"
        '
        'ToolStripStatusSaturation
        '
        Me.ToolStripStatusSaturation.Name = "ToolStripStatusSaturation"
        Me.ToolStripStatusSaturation.Size = New System.Drawing.Size(61, 17)
        Me.ToolStripStatusSaturation.Text = "Saturation"
        '
        'ToolStripStatusSep07
        '
        Me.ToolStripStatusSep07.Name = "ToolStripStatusSep07"
        Me.ToolStripStatusSep07.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep07.Text = "|"
        '
        'ToolStripStatusGrayscale
        '
        Me.ToolStripStatusGrayscale.Name = "ToolStripStatusGrayscale"
        Me.ToolStripStatusGrayscale.Size = New System.Drawing.Size(57, 17)
        Me.ToolStripStatusGrayscale.Text = "Grayscale"
        '
        'ToolStripStatusSep08
        '
        Me.ToolStripStatusSep08.Name = "ToolStripStatusSep08"
        Me.ToolStripStatusSep08.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusSep08.Text = "|"
        Me.ToolStripStatusSep08.TextAlign = System.Drawing.ContentAlignment.TopLeft
        '
        'ToolStripStatusEffects
        '
        Me.ToolStripStatusEffects.Name = "ToolStripStatusEffects"
        Me.ToolStripStatusEffects.Size = New System.Drawing.Size(42, 17)
        Me.ToolStripStatusEffects.Text = "Effects"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(0, -1)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.ToolStripView)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.AutoScroll = True
        Me.SplitContainer1.Panel2.AutoScrollMargin = New System.Drawing.Size(10, 10)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Panel1)
        Me.SplitContainer1.Size = New System.Drawing.Size(824, 551)
        Me.SplitContainer1.SplitterDistance = 56
        Me.SplitContainer1.SplitterWidth = 3
        Me.SplitContainer1.TabIndex = 4
        '
        'ToolStripView
        '
        Me.ToolStripView.Dock = System.Windows.Forms.DockStyle.Left
        Me.ToolStripView.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStripView.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsMirror, Me.tsLevel, Me.tsRotate, Me.tsFit, Me.tsZoom, Me.tsCrop, Me.ToolStripSeparator1, Me.tsInvert, Me.tsBrightness, Me.tsContrast, Me.tsGamma, Me.tsSaturation, Me.tsGrayscale, Me.ToolStripSeparator2, Me.tsEffects, Me.ToolStripSeparator3, Me.tsReset, Me.ToolStripSeparator4, Me.tsViewer, Me.tsPinnedToTop, Me.ToolStripSeparator5, Me.tsAR4W, Me.tsMagnify, Me.ToolStripSeparator6, Me.tsCopy, Me.tsSave, Me.ToolStripSeparator7, Me.tsHelp})
        Me.ToolStripView.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripView.Name = "ToolStripView"
        Me.ToolStripView.Size = New System.Drawing.Size(88, 551)
        Me.ToolStripView.TabIndex = 4
        Me.ToolStripView.Text = "View"
        '
        'tsMirror
        '
        Me.tsMirror.Image = CType(resources.GetObject("tsMirror.Image"), System.Drawing.Image)
        Me.tsMirror.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsMirror.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsMirror.Name = "tsMirror"
        Me.tsMirror.Size = New System.Drawing.Size(85, 20)
        Me.tsMirror.Text = "Mirror"
        Me.tsMirror.ToolTipText = "Mirror" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(left)    horizontally" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(right)  vertically"
        '
        'tsLevel
        '
        Me.tsLevel.Image = Global.Resources.AssociationRelationship
        Me.tsLevel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsLevel.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsLevel.Name = "tsLevel"
        Me.tsLevel.Size = New System.Drawing.Size(85, 20)
        Me.tsLevel.Text = "Level"
        '
        'tsRotate
        '
        Me.tsRotate.Image = CType(resources.GetObject("tsRotate.Image"), System.Drawing.Image)
        Me.tsRotate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsRotate.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsRotate.Name = "tsRotate"
        Me.tsRotate.Size = New System.Drawing.Size(85, 20)
        Me.tsRotate.Text = "Rotate"
        Me.tsRotate.ToolTipText = "Rotate" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(left)    counter-clockwise" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(right)  clockwise" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "             [Shift] x2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "             [Ctrl]   x5" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "             [Shift][Ctrl] x10 " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "[Alt] rotate to next" &
    "/previous 45 degree stop"
        '
        'tsFit
        '
        Me.tsFit.Image = CType(resources.GetObject("tsFit.Image"), System.Drawing.Image)
        Me.tsFit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsFit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsFit.Name = "tsFit"
        Me.tsFit.Size = New System.Drawing.Size(85, 20)
        Me.tsFit.Text = "Fit     "
        Me.tsFit.ToolTipText = "Fit" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(left)          horizontally and vertically " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(middle)   horizontally" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(righ" &
    "t)        vertically" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'tsZoom
        '
        Me.tsZoom.Image = CType(resources.GetObject("tsZoom.Image"), System.Drawing.Image)
        Me.tsZoom.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsZoom.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsZoom.Name = "tsZoom"
        Me.tsZoom.Size = New System.Drawing.Size(85, 20)
        Me.tsZoom.Text = "Zoom"
        Me.tsZoom.ToolTipText = "Zoom" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(left)    out " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(right)  in" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "             [Shift] x2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "             [Ctrl]  " &
    "x5" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "             [Shift][Ctrl] x10" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'tsCrop
        '
        Me.tsCrop.Image = CType(resources.GetObject("tsCrop.Image"), System.Drawing.Image)
        Me.tsCrop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsCrop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsCrop.Name = "tsCrop"
        Me.tsCrop.Size = New System.Drawing.Size(85, 20)
        Me.tsCrop.Text = "Crop    "
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(85, 6)
        '
        'tsInvert
        '
        Me.tsInvert.Image = Global.Resources.BooleanData
        Me.tsInvert.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsInvert.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsInvert.Name = "tsInvert"
        Me.tsInvert.Size = New System.Drawing.Size(85, 20)
        Me.tsInvert.Text = "Invert"
        '
        'tsBrightness
        '
        Me.tsBrightness.Image = CType(resources.GetObject("tsBrightness.Image"), System.Drawing.Image)
        Me.tsBrightness.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsBrightness.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBrightness.Name = "tsBrightness"
        Me.tsBrightness.Size = New System.Drawing.Size(85, 20)
        Me.tsBrightness.Text = "Brightness"
        Me.tsBrightness.ToolTipText = "Brightness" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift] x2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Ctrl] x5" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift][Ctrl] x10"
        '
        'tsContrast
        '
        Me.tsContrast.Image = CType(resources.GetObject("tsContrast.Image"), System.Drawing.Image)
        Me.tsContrast.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsContrast.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsContrast.Name = "tsContrast"
        Me.tsContrast.Size = New System.Drawing.Size(85, 20)
        Me.tsContrast.Text = "Contrast"
        Me.tsContrast.ToolTipText = "Contrast" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift] x2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Ctrl] x5" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift][Ctrl] x10"
        '
        'tsGamma
        '
        Me.tsGamma.Image = CType(resources.GetObject("tsGamma.Image"), System.Drawing.Image)
        Me.tsGamma.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsGamma.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsGamma.Name = "tsGamma"
        Me.tsGamma.Size = New System.Drawing.Size(85, 20)
        Me.tsGamma.Text = "Gamma"
        Me.tsGamma.ToolTipText = "Gamma" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift] x2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Ctrl] x5" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift][Ctrl] x10" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'tsSaturation
        '
        Me.tsSaturation.Image = CType(resources.GetObject("tsSaturation.Image"), System.Drawing.Image)
        Me.tsSaturation.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsSaturation.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsSaturation.Name = "tsSaturation"
        Me.tsSaturation.Size = New System.Drawing.Size(85, 20)
        Me.tsSaturation.Text = "  Saturation"
        Me.tsSaturation.ToolTipText = "Saturation" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift] x2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Ctrl] x5" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "     [Shift][Ctrl] x10" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'tsGrayscale
        '
        Me.tsGrayscale.Image = CType(resources.GetObject("tsGrayscale.Image"), System.Drawing.Image)
        Me.tsGrayscale.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsGrayscale.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsGrayscale.Name = "tsGrayscale"
        Me.tsGrayscale.Size = New System.Drawing.Size(85, 20)
        Me.tsGrayscale.Text = " Grayscale"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(85, 6)
        '
        'tsEffects
        '
        Me.tsEffects.Image = CType(resources.GetObject("tsEffects.Image"), System.Drawing.Image)
        Me.tsEffects.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsEffects.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsEffects.Name = "tsEffects"
        Me.tsEffects.Size = New System.Drawing.Size(85, 19)
        Me.tsEffects.Text = "Effects ..."
        Me.tsEffects.ToolTipText = "Effects ..." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(left)   open Designer" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(right) set Effects"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(85, 6)
        '
        'tsReset
        '
        Me.tsReset.Image = CType(resources.GetObject("tsReset.Image"), System.Drawing.Image)
        Me.tsReset.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsReset.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsReset.Name = "tsReset"
        Me.tsReset.Size = New System.Drawing.Size(85, 20)
        Me.tsReset.Text = "Reset    "
        Me.tsReset.ToolTipText = "Hold a mouse button down on the Reset button for one second to reset the image"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(85, 6)
        '
        'tsViewer
        '
        Me.tsViewer.Image = CType(resources.GetObject("tsViewer.Image"), System.Drawing.Image)
        Me.tsViewer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsViewer.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsViewer.Name = "tsViewer"
        Me.tsViewer.Size = New System.Drawing.Size(85, 20)
        Me.tsViewer.Text = "Viewer   "
        Me.tsViewer.ToolTipText = "Viewer" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(left) original on bottom" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(right) original on right"
        '
        'tsPinnedToTop
        '
        Me.tsPinnedToTop.Image = CType(resources.GetObject("tsPinnedToTop.Image"), System.Drawing.Image)
        Me.tsPinnedToTop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsPinnedToTop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsPinnedToTop.Name = "tsPinnedToTop"
        Me.tsPinnedToTop.Size = New System.Drawing.Size(85, 20)
        Me.tsPinnedToTop.Text = " Pin to top"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(85, 6)
        '
        'tsAR4W
        '
        Me.tsAR4W.Image = CType(resources.GetObject("tsAR4W.Image"), System.Drawing.Image)
        Me.tsAR4W.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsAR4W.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsAR4W.Name = "tsAR4W"
        Me.tsAR4W.Size = New System.Drawing.Size(85, 20)
        Me.tsAR4W.Text = "AR4W"
        Me.tsAR4W.ToolTipText = "A Ruler for Windows" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(left) Reading guide" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(middle) close" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(right) Ruler"
        '
        'tsMagnify
        '
        Me.tsMagnify.Image = CType(resources.GetObject("tsMagnify.Image"), System.Drawing.Image)
        Me.tsMagnify.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsMagnify.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsMagnify.Name = "tsMagnify"
        Me.tsMagnify.Size = New System.Drawing.Size(85, 20)
        Me.tsMagnify.Text = "Magnify"
        Me.tsMagnify.ToolTipText = "Microsoft's Magnify tool"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(85, 6)
        '
        'tsCopy
        '
        Me.tsCopy.Image = CType(resources.GetObject("tsCopy.Image"), System.Drawing.Image)
        Me.tsCopy.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsCopy.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsCopy.Name = "tsCopy"
        Me.tsCopy.Size = New System.Drawing.Size(85, 20)
        Me.tsCopy.Text = "Copy"
        '
        'tsSave
        '
        Me.tsSave.Image = CType(resources.GetObject("tsSave.Image"), System.Drawing.Image)
        Me.tsSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsSave.Name = "tsSave"
        Me.tsSave.Size = New System.Drawing.Size(85, 20)
        Me.tsSave.Text = "Save"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(85, 6)
        '
        'tsHelp
        '
        Me.tsHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsHelp.Image = CType(resources.GetObject("tsHelp.Image"), System.Drawing.Image)
        Me.tsHelp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.tsHelp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsHelp.Name = "tsHelp"
        Me.tsHelp.Size = New System.Drawing.Size(85, 19)
        Me.tsHelp.Text = "Quick Help"
        Me.tsHelp.ToolTipText = "Quick Help (or 'Alt' + Quick Help to view online)"
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.AutoScroll = True
        Me.Panel1.AutoScrollMargin = New System.Drawing.Size(10, 10)
        Me.Panel1.AutoSize = True
        Me.Panel1.Controls.Add(Me.PictureBoxOriginal)
        Me.Panel1.Controls.Add(Me.PictureBoxBottom)
        Me.Panel1.Controls.Add(Me.PictureBoxRight)
        Me.Panel1.Controls.Add(Me.PictureBoxPrimary)
        Me.Panel1.Location = New System.Drawing.Point(2, 3)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(874, 630)
        Me.Panel1.TabIndex = 1
        '
        'PictureBoxOriginal
        '
        Me.PictureBoxOriginal.Location = New System.Drawing.Point(229, 184)
        Me.PictureBoxOriginal.Name = "PictureBoxOriginal"
        Me.PictureBoxOriginal.Size = New System.Drawing.Size(144, 174)
        Me.PictureBoxOriginal.TabIndex = 3
        Me.PictureBoxOriginal.TabStop = False
        '
        'PictureBoxBottom
        '
        Me.PictureBoxBottom.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxBottom.Location = New System.Drawing.Point(3, 225)
        Me.PictureBoxBottom.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.PictureBoxBottom.Name = "PictureBoxBottom"
        Me.PictureBoxBottom.Size = New System.Drawing.Size(833, 399)
        Me.PictureBoxBottom.TabIndex = 2
        Me.PictureBoxBottom.TabStop = False
        '
        'PictureBoxRight
        '
        Me.PictureBoxRight.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxRight.Location = New System.Drawing.Point(311, 3)
        Me.PictureBoxRight.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.PictureBoxRight.Name = "PictureBoxRight"
        Me.PictureBoxRight.Size = New System.Drawing.Size(525, 412)
        Me.PictureBoxRight.TabIndex = 1
        Me.PictureBoxRight.TabStop = False
        '
        'Timer1
        '
        '
        'frmViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(824, 570)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.Name = "frmViewer"
        Me.Text = "A Viewer for Windows"
        CType(Me.PictureBoxPrimary, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ToolStripView.ResumeLayout(False)
        Me.ToolStripView.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        CType(Me.PictureBoxOriginal, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxBottom, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxRight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBoxPrimary As System.Windows.Forms.PictureBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusAngle As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSep08 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusZoom As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSep02 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusBrightness As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSep01 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusContrast As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusGamma As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSep03 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSep07 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSaturation As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents ToolStripView As System.Windows.Forms.ToolStrip
    Friend WithEvents tsBrightness As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsContrast As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsGamma As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsGrayscale As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsSaturation As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsRotate As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsZoom As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsCrop As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsLevel As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsReset As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsAR4W As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsPinnedToTop As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsHelp As System.Windows.Forms.ToolStripButton
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents tsEffects As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripStatusSep06 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusEffects As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tsInvert As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripStatusSep04 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusGrayscale As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusInverted As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSep05 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents PictureBoxBottom As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBoxRight As System.Windows.Forms.PictureBox
    Friend WithEvents tsViewer As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsMagnify As System.Windows.Forms.ToolStripButton
    Friend WithEvents PictureBoxOriginal As System.Windows.Forms.PictureBox
    Friend WithEvents tsMirror As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripStatusMirrored As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSep00 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tsFit As System.Windows.Forms.ToolStripButton
End Class
