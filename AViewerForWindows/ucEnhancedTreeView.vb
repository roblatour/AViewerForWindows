Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

Public Class ucEnhancedTreeView

    Friend minHeight As Integer = 50
    Friend minWidth As Integer = 100

    Friend maxWidth As Integer = 0

    Private Sub EnhancedTreeView_Load(sender As Object, e As EventArgs) Handles Me.Load

        TreeView1.HideSelection = False                          ' keeps selected item selected when treeview looses focus

        TreeView1.DrawMode = TreeViewDrawMode.OwnerDrawText      ' allows TreeView1_DrawNode to set the colours of the selected node
        '                                                          also requires the TreeView1_DrawNode subroutine directly below

    End Sub
    Private Sub TreeView1_DrawNode(sender As Object, e As DrawTreeNodeEventArgs) Handles TreeView1.DrawNode

        Try

            SyncLock TreeView1

                'ref: https://stackoverflow.com/questions/10034714/c-sharp-winforms-highlight-treenode-when-treeview-doesnt-have-focus

                Dim treeState As TreeNodeStates = e.State
                Dim treeFont As Font = If(e.Node.NodeFont, e.Node.TreeView.Font)
                ' Dim disabledFont As Font = New Font(treeFont, FontStyle.Italic)
                Dim foreColor As Color = e.Node.ForeColor
                Dim strDeselectedColor As String = "#0078d7", strSelectedColor As String = "#94C7FC"
                Dim selectedColor As Color = System.Drawing.ColorTranslator.FromHtml(strSelectedColor)
                Dim deselectedColor As Color = System.Drawing.ColorTranslator.FromHtml(strDeselectedColor)
                Dim selectedTreeBrush As SolidBrush = New SolidBrush(selectedColor)
                Dim deselectedTreeBrush As SolidBrush = New SolidBrush(deselectedColor)
                If foreColor = Color.Empty Then foreColor = e.Node.TreeView.ForeColor

                If e.Node.IsSelected Then

                    If Me.Focused Then
                        foreColor = SystemColors.HighlightText
                        e.Graphics.FillRectangle(selectedTreeBrush, e.Bounds)
                        ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, foreColor, SystemColors.Highlight)
                        TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, foreColor, TextFormatFlags.GlyphOverhangPadding)
                    Else
                        foreColor = SystemColors.HighlightText
                        e.Graphics.FillRectangle(deselectedTreeBrush, e.Bounds)
                        ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, foreColor, SystemColors.Highlight)
                        TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, foreColor, TextFormatFlags.GlyphOverhangPadding)
                    End If

                Else

                    If (e.State And TreeNodeStates.Hot) = TreeNodeStates.Hot Then

                        e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds)
                        TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, System.Drawing.Color.Black, TextFormatFlags.GlyphOverhangPadding)

                    Else

                        e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds)

                        If e.Node.Tag = gTreeViewFolderTagDesignation Then

                            TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, System.Drawing.Color.Black, TextFormatFlags.GlyphOverhangPadding)

                        Else

                            TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, Color.MediumSlateBlue, TextFormatFlags.GlyphOverhangPadding)

                        End If

                    End If

                End If

            End SyncLock

        Catch ex As Exception

        End Try

    End Sub

    Private Sub EnhancedTreeViewBox_SelectedValueChanged(sender As Object, e As EventArgs)

        btnDone.Enabled = True

    End Sub

    Friend resizingUnderway As Boolean = False

    Public Sub ResizeControl(ByVal InitialControl As Boolean)

        If resizingUnderway Then Exit Sub

        resizingUnderway = True

        Try

            If InitialControl Then

                ' calculate width of lisbox based on its contents
                ' height is be Compute in FinalForm 

                Dim workingBounds As RectangleF

                Dim workingWidth As Single
                Dim optimalWidth As Single = 150 ' minimal optimal width

                Dim ListOfTreeNodes As List(Of String) = GetChildren(TreeView1.Nodes(0))

                Dim maxwidthincharacters As Integer = 0  ' this is a performance hack

                For Each item In ListOfTreeNodes

                    If item.Length > maxwidthincharacters Then

                        maxwidthincharacters = item.Length

                        workingBounds = GetDimensionsOfString(Me.Handle, item, TreeView1.Font)
                        workingWidth = workingBounds.Width

                        If workingWidth > optimalWidth Then optimalWidth = workingWidth

                    End If

                Next

                Dim candidateWidth As Integer = CInt(optimalWidth + 5) + btnDone.Width

                If candidateWidth < minWidth Then
                    candidateWidth = minWidth
                End If

                If maxWidth > 0 Then
                    If candidateWidth > maxWidth Then
                        Me.Width = maxWidth
                    Else
                        Me.Width = candidateWidth
                    End If
                End If

            End If

            'TreeView1.Width = Me.Width + 500 ' force scroll bar to appear
            'TreeView1.Height = Me.Height + 500

            'If btnDone.Visible Then
            '    Me.Panel1.Width = Me.Width - btnDone.Width - 20
            'Else
            '    Me.Panel1.Width = Me.Width - 20
            'End If

            If btnDone.Visible Then
                Me.Panel1.Width = Me.Width - btnDone.Width - 2
            Else
                Me.Panel1.Width = Me.Width - 20
            End If

            Me.Panel1.Height = Me.Height - 20

            Application.DoEvents()

            TreeView1.Width = Me.Panel1.Width - 20
            TreeView1.Height = Me.Panel1.Height - 15

            btnDone.Height = Me.Height - 20 ' force button height 

            ' Centre header 

            Dim BoldFontFactor As Single = 1.2

            Dim w1 As Integer = CInt(GetDimensionsOfString(Me.Handle, lblHeader.Text, lblHeader.Font).Width * BoldFontFactor)

            lblHeader.Location = New Point(CInt(Me.Width / 2) - CInt(w1 / 2), lblHeader.Location.Y)  ' centre header

            btnDone.BringToFront()

        Catch ex As Exception

        End Try

        resizingUnderway = False

    End Sub

    Function GetChildren(parentNode As TreeNode) As List(Of String)
        Dim nodes As List(Of String) = New List(Of String)
        GetAllChildren(parentNode, nodes)
        Return nodes
    End Function

    Sub GetAllChildren(parentNode As TreeNode, nodes As List(Of String))

        For Each childNode As TreeNode In parentNode.Nodes

            nodes.Add(childNode.Text)
            GetAllChildren(childNode, nodes)

        Next

    End Sub

    Private Sub TreeView1_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TreeView1.KeyDown

        If e.KeyCode = Keys.Down Then
            gLatestArrowDirectionIsDown = True
        ElseIf e.KeyCode = Keys.Up Then
            gLatestArrowDirectionIsDown = False
        End If

    End Sub

End Class
