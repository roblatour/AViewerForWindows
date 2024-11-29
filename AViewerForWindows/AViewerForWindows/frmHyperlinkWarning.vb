Imports System

Public Class frmHyperlinkWarning

    Private Sub frmHyperlinkWarning_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.Text = gThisProgramName & gVersionInUse

        gOpenOkedByUser = False

        TextBox1.Text = TextBox1.Text.Replace("***", gLinkToBeOpened)

        MakeTopMostWindow(Me.Handle, True)

        btnNo.Focus()

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged

        My.Settings.ShowHyperlinkWarning = Not CheckBox1.Checked
        My.Settings.Save()

    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click

        Me.Close()

    End Sub

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click

        gOpenOkedByUser = True
        Me.Close()

    End Sub
End Class