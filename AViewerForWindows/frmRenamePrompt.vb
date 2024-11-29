Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Public Class frmRenamePrompt

    Private Sub frmRenamePrompt_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load

        MakeTopMostWindow(Me.Handle, True)

        tbName.Text = gRenameFunctionProposedName
        tbName.Focus()
        tbName.Select(tbName.Text.Length, 0)

        gRenameFunctionNewName = String.Empty


        Select Case gNamingFunction

            Case gNamingFunctions.add

                Me.Text = gThisProgramName & gVersionInUse & " - Effect Add"
                Label1.Text = "Please enter the name of the profile you would like to add:"

            Case gNamingFunctions.copy

                Me.Text = gThisProgramName & gVersionInUse & " - Effect Copy"
                Label1.Text = "Please enter a new name for the copy of the '" & gRenameFunctionOriginalName & "' profile:"

            Case gNamingFunctions.rename
                Me.Text = gThisProgramName & gVersionInUse & " - Effect Rename"
                Label1.Text = "Please enter a new name for the '" & gRenameFunctionOriginalName & "' profile:"

        End Select

    End Sub

    Private Sub btnOk_Click(sender As Object, e As System.EventArgs) Handles btnOk.Click

        tbName.Text = tbName.Text.Trim

        If tbName.Text = String.Empty Then

            Beep()
            Dim ReturnResult = MessageBox.Show("New name cannot be blank", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub

        End If


        If tbName.Text = gRenameFunctionOriginalName Then

            If gNamingFunction = gNamingFunctions.copy Then

                Beep()
                Dim ReturnResult = MessageBox.Show("The original name of this profile is '" & tbName.Text & "' so to copy it you need to give it another name.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub

            ElseIf gNamingFunction = gNamingFunctions.rename Then

                Beep()
                Dim ReturnResult = MessageBox.Show("The original name of this profile is already '" & tbName.Text & "'." & vbCrLf & vbCrLf &
                                                   "If you would like to rename it, please give it a new name in the Effect Rename window." & vbCrLf & vbCrLf &
                                                   "Otherwise, please click 'Cancel' in the Effect Rename window.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub

            End If

        End If

        Dim MatchFound As Boolean = False
        For Each Name As String In gListOfAllProfiles
            If Name = tbName.Text Then
                MatchFound = True
                Exit For
            End If
        Next

        If MatchFound Then
            Beep()
            Dim ReturnResult = MessageBox.Show("The profile name '" & tbName.Text & "' is already in use and cannot be used as the new name for this profile.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        If ProfileNameValidation(tbName.Text) Then
        Else
            Exit Sub
        End If

        gRenameFunctionNewName = tbName.Text

        Me.Close()

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As System.EventArgs) Handles btnCancel.Click

        gRenameFunctionNewName = String.Empty
        Me.Close()

    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles tbName.KeyPress

        If "/:*?""<>|".Contains(e.KeyChar.ToString) Then
            Beep()
            Dim ReturnResult = MessageBox.Show("Name cannot contain a character which would be invalid for use in a filename." & vbCrLf & vbCrLf &
                                               "Characters which are invalid for use in a filename include:" & vbCrLf & "/:*?""<>|", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            e.Handled = True
        End If


    End Sub

End Class