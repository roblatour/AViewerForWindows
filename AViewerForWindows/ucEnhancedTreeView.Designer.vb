<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucEnhancedTreeView
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.btnEffectRemove = New System.Windows.Forms.Button()
        Me.btnEffectAdd = New System.Windows.Forms.Button()
        Me.btnDone = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblHeader
        '
        Me.lblHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Enabled = False
        Me.lblHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader.Location = New System.Drawing.Point(66, 2)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(58, 16)
        Me.lblHeader.TabIndex = 0
        Me.lblHeader.Text = "Effect 1"
        '
        'btnEffectRemove
        '
        Me.btnEffectRemove.BackColor = System.Drawing.Color.Red
        Me.btnEffectRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnEffectRemove.Location = New System.Drawing.Point(-1, 1)
        Me.btnEffectRemove.Name = "btnEffectRemove"
        Me.btnEffectRemove.Size = New System.Drawing.Size(16, 16)
        Me.btnEffectRemove.TabIndex = 2
        Me.btnEffectRemove.Text = " "
        Me.ToolTip1.SetToolTip(Me.btnEffectRemove, "Remove this effect")
        Me.btnEffectRemove.UseVisualStyleBackColor = False
        '
        'btnEffectAdd
        '
        Me.btnEffectAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnEffectAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnEffectAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnEffectAdd.Location = New System.Drawing.Point(212, 1)
        Me.btnEffectAdd.Name = "btnEffectAdd"
        Me.btnEffectAdd.Size = New System.Drawing.Size(16, 16)
        Me.btnEffectAdd.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.btnEffectAdd, "Add another effect")
        Me.btnEffectAdd.UseVisualStyleBackColor = False
        '
        'btnDone
        '
        Me.btnDone.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDone.BackColor = System.Drawing.Color.LightSteelBlue
        Me.btnDone.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDone.Location = New System.Drawing.Point(211, 18)
        Me.btnDone.Name = "btnDone"
        Me.btnDone.Size = New System.Drawing.Size(18, 520)
        Me.btnDone.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.btnDone, "Apply Effect")
        Me.btnDone.UseVisualStyleBackColor = False
        '
        'TreeView1
        '
        Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TreeView1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TreeView1.Location = New System.Drawing.Point(1, 1)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.TreeView1.Size = New System.Drawing.Size(181, 516)
        Me.TreeView1.TabIndex = 5
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.AutoScroll = True
        Me.Panel1.AutoScrollMinSize = New System.Drawing.Size(10, 10)
        Me.Panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Panel1.Controls.Add(Me.TreeView1)
        Me.Panel1.Location = New System.Drawing.Point(13, 21)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(201, 538)
        Me.Panel1.TabIndex = 6
        '
        'ucEnhancedTreeView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.btnDone)
        Me.Controls.Add(Me.btnEffectAdd)
        Me.Controls.Add(Me.btnEffectRemove)
        Me.Controls.Add(Me.lblHeader)
        Me.DoubleBuffered = True
        Me.Name = "ucEnhancedTreeView"
        Me.Size = New System.Drawing.Size(229, 559)
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblHeader As System.Windows.Forms.Label
    Friend WithEvents btnEffectRemove As System.Windows.Forms.Button
    Friend WithEvents btnEffectAdd As System.Windows.Forms.Button
    Friend WithEvents btnDone As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
End Class
