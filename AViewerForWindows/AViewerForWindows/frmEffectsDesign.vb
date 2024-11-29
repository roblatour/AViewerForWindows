Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Public Class frmEffectsDesign

    Private LoadUnderway As Boolean = True

    Private FirstActivation As Boolean = True

    Private BeforeSelectProfile As String = String.Empty
    Private PriorProfileBeingWorkedOn As String = String.Empty

    Private complicatedCancel As Boolean = False
    Private AV4WsCurrentProfileVersionForComplicatedCancel As gEffectsProfileStructure

    Private cbName_Text As String = String.Empty
    Private Sub frmPickEffectsMatrix_Load(sender As Object, e As EventArgs) Handles Me.Load

        Try

            Me.Text = gThisProgramName & gVersionInUse & " - Effects Design"
            Me.Location = gfrmPickEffectsStartLocation

            If gRunningOnA4KMonitor Then
                SplitContainer1_MaxWidth = 112
            Else
                SplitContainer1_MaxWidth = 85
            End If

            If My.Settings.PinnedToTop Then
                MakeTopMostWindow(Me.Handle.ToInt64, True)
            Else
                MakeTopMostWindow(Me.Handle.ToInt64, False)
            End If

            Me.BringToFront()

            SetPictureBoxBackGrounds()

            gOriginalAlpha = gOriginalImage.GetPixel(gOriginalImage.Width / 2, gOriginalImage.Height / 2).A

            gMovingPoint = gTweakedStartingLocationDueToResizeOriginalForRotation

            If gViewChoice = ViewingOption.OriginalOnly Then
                gViewChoice = ViewingOption.PrimaryOnly
            End If

            SetLookOfWindows(True)
            RefreshAllProfileNames()

            tsSave.Enabled = False

            UpdateLookOfSplitContainer(True, True)

            tsMagnify.Visible = gMagnifierFound

            PriorProfileBeingWorkedOn = cbName_Text

            If System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToUpper = "USA" Then
                gbModifyColours.Text = gbModifyColours.Text.Replace("Colours", "Colors")
                gbTransparency.Text = gbTransparency.Text.Replace("Colours", "Colors")
                ToolTip1.SetToolTip(btnFind, ToolTip1.GetToolTip(btnFind).Replace("colour", "color"))
            End If

            SetTransparencyColours()

            UpdateButtonAvailability()

            AddHandler AnAV4WNotificationEvent, AddressOf ReactToAnAV4WNotificationEvent

            Application.DoEvents()

        Catch ex As Exception

        End Try

        LoadUnderway = False

        MasterScreenUpdate(True)
        RefreshTrialImage()

        TreeView1.HideSelection = False

        If TreeView1.SelectedNode Is Nothing Then
            TreeView1.SelectedNode = TreeView1.Nodes(0)
        End If

        TreeView1.SelectedNode.EnsureVisible()

        EnsureVisableAndOptionalySelelectATreeNode(TreeView1.SelectedNode.Text, TreeView1, True)

        TreeView1_Focus()

        tsSave.Enabled = False

    End Sub
    Private Sub ReactToAnAV4WNotificationEvent()

        ' check to see if the effect currently being edited has changed and if so pop-up a warning

        If My.Settings.PinnedToTop Then
            MakeTopMostWindow(Me.Handle.ToInt64, False)
        End If

        Try

            Dim htsSaveEnable As Boolean = tsSave.Enabled

            TempSaveAndRestoreProfileOnScreen(True)
            RefreshAllProfileNames()
            TempSaveAndRestoreProfileOnScreen(False)

            tsSave.Enabled = htsSaveEnable

            AV4WsCurrentProfileVersionForComplicatedCancel = gCurrentEffectsProfile

            If LoadEffectsProfile(ConvertProfileNameToXMLFileName(AV4WsCurrentProfileVersionForComplicatedCancel.Name)) Then

                If AreTheseTwoProfilesTheSame(AV4WsCurrentProfileVersionForComplicatedCancel, gCurrentEffectsProfile, False, False, False, False, False, False, False) Then

                    Exit Try

                Else

                    If tsSave.Enabled Then

                        complicatedCancel = True

                        Beep()
                        If MessageBox.Show("The Effects Profile '" & AV4WsCurrentProfileVersionForComplicatedCancel.Name & "' has just been changed in your data folder." & vbCrLf & vbCrLf &
                                           "However, the information in your data folder no longer matches the information which you began with when changing this profile." & vbCrLf & vbCrLf &
                                           "Click 'Yes' if you would like to continue working on this profile as it is on your screen." & vbCrLf & vbCrLf &
                                           "Click 'No' to cancel the changes on your screen and load the new version from your data folder.", gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then



                        Else

                            LoadScreenFromCurrentEffectsProfile()
                            tsSave.Enabled = False
                            Exit Try

                        End If

                    Else

                        MessageBox.Show("The Effects Profile '" & AV4WsCurrentProfileVersionForComplicatedCancel.Name & "' has just been changed in your data folder." & vbCrLf & vbCrLf &
                                        "This window will now be updated accordingly.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        LoadScreenFromCurrentEffectsProfile()
                        tsSave.Enabled = False
                        Exit Try

                    End If

                End If

            Else

                Beep()
                If MessageBox.Show("The Effects Profile '" & AV4WsCurrentProfileVersionForComplicatedCancel.Name & "' has just been removed from your data folder." & vbCrLf & vbCrLf &
                                "Click 'Yes' to save the profile you are working on so that it will not be lost." & vbCrLf & vbCrLf &
                                "Click 'No' to cancel your changes and advance to the next available Profile.", gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.Yes Then

                    tsSave.PerformClick()
                    Exit Try

                Else

                    tsSave.Enabled = False
                    Exit Try

                End If

            End If

        Catch ex As Exception

        End Try

        Try
            EnsureVisableAndOptionalySelelectATreeNode(gCurrentEffectsProfile.Name, TreeView1, True)
            TreeView1.Refresh()
        Catch ex As Exception

        End Try

        If My.Settings.PinnedToTop Then
            MakeTopMostWindow(Me.Handle.ToInt64, True)
        End If

    End Sub

    Friend Sub TempSaveAndRestoreProfileOnScreen(ByVal Save As Boolean)

        Static Dim Name, Sigma, Amount, Bias, Factor, MA, MR, MG, MB, TR, TG, TB, TT As String
        Static Dim tb00s, tb01s, tb02s, tb03s, tb04s, tb05s, tb06s, tb10s, tb11s, tb12s, tb13s, tb14s, tb15s, tb16s, tb20s, tb21s, tb22s, tb23s, tb24s, tb25s, tb26s, tb30s, tb31s, tb32s, tb33s, tb34s, tb35s, tb36s, tb40s, tb41s, tb42s, tb43s, tb44s, tb45s, tb46s, tb50s, tb51s, tb52s, tb53s, tb54s, tb55s, tb56s, tb60s, tb61s, tb62s, tb63s, tb64s, tb65s, tb66s As String
        Static Dim ProfileEnabled, Blur, Matrix3x3, Compute, Symmetric, TransparencyEnabled As Boolean
        Static Dim rtb As Windows.Forms.RichTextBox

        Try

            If Save Then
                ' save current window to temp profile

                Name = cbName_Text
                ProfileEnabled = cbEnabled.Checked

                Matrix3x3 = rb3x3.Checked

                Sigma = tbSigma.Text
                Amount = tbAmount.Text
                Bias = tbBias.Text
                Factor = tbFactor.Text

                Compute = cbCompute.Checked
                Symmetric = cbSymmetric.Checked

                tb00s = tb00.Text
                tb01s = tb01.Text
                tb02s = tb02.Text
                tb03s = tb03.Text
                tb04s = tb04.Text
                tb05s = tb05.Text
                tb06s = tb06.Text

                tb10s = tb10.Text
                tb11s = tb11.Text
                tb12s = tb12.Text
                tb13s = tb14.Text
                tb14s = tb13.Text
                tb15s = tb15.Text
                tb16s = tb16.Text

                tb20s = tb20.Text
                tb21s = tb21.Text
                tb22s = tb22.Text
                tb23s = tb23.Text
                tb24s = tb24.Text
                tb25s = tb25.Text
                tb26s = tb26.Text

                tb30s = tb30.Text
                tb31s = tb31.Text
                tb32s = tb32.Text
                tb33s = tb33.Text
                tb34s = tb34.Text
                tb35s = tb35.Text
                tb36s = tb36.Text

                tb40s = tb40.Text
                tb41s = tb41.Text
                tb42s = tb42.Text
                tb43s = tb43.Text
                tb44s = tb44.Text
                tb45s = tb45.Text
                tb46s = tb46.Text

                tb50s = tb50.Text
                tb51s = tb51.Text
                tb52s = tb52.Text
                tb53s = tb53.Text
                tb54s = tb54.Text
                tb55s = tb55.Text
                tb56s = tb56.Text

                tb60s = tb60.Text
                tb61s = tb61.Text
                tb62s = tb62.Text
                tb63s = tb63.Text
                tb64s = tb64.Text
                tb65s = tb65.Text
                tb66s = tb66.Text

                MA = tbModifyAlpha.Text
                MR = tbModifyRed.Text
                MG = tbModifyGreen.Text
                MB = tbModifyBlue.Text

                TransparencyEnabled = cbTransparencyEnabled.Checked

                TR = tbTransparencyRed.Text
                TG = tbTransparencyGreen.Text
                TB = tbTransparencyBlue.Text
                TT = tbTransparencyTolerance.Text

                rtb = rtbNotes

            Else

                LoadUnderway = True

                cbName_Text = Name
                cbEnabled.Checked = ProfileEnabled

                rb3x3.Checked = Matrix3x3
                rb5x5.Checked = Not Matrix3x3

                tbSigma.Text = Sigma
                tbAmount.Text = Amount
                tbBias.Text = Bias
                tbFactor.Text = Factor  ' bug fixed in v1.7

                cbCompute.Checked = Compute
                cbSymmetric.Checked = Symmetric

                tb00.Text = tb00s
                tb01.Text = tb01s
                tb02.Text = tb02s
                tb03.Text = tb03s
                tb04.Text = tb04s
                tb05.Text = tb05s
                tb06.Text = tb06s

                tb10.Text = tb10s
                tb11.Text = tb11s
                tb12.Text = tb12s
                tb14.Text = tb13s
                tb13.Text = tb14s
                tb15.Text = tb15s
                tb16.Text = tb16s

                tb20.Text = tb20s
                tb21.Text = tb21s
                tb22.Text = tb22s
                tb23.Text = tb23s
                tb24.Text = tb24s
                tb25.Text = tb25s
                tb26.Text = tb26s

                tb30.Text = tb30s
                tb31.Text = tb31s
                tb33.Text = tb32s
                tb32.Text = tb33s
                tb34.Text = tb34s
                tb35.Text = tb35s
                tb36.Text = tb36s

                tb40.Text = tb40s
                tb41.Text = tb41s
                tb43.Text = tb42s
                tb42.Text = tb43s
                tb44.Text = tb44s
                tb45.Text = tb45s
                tb46.Text = tb46s

                tb50.Text = tb60s
                tb51.Text = tb61s
                tb53.Text = tb62s
                tb52.Text = tb63s
                tb54.Text = tb64s
                tb55.Text = tb65s
                tb56.Text = tb66s

                tb60.Text = tb60s
                tb61.Text = tb61s
                tb63.Text = tb62s
                tb62.Text = tb63s
                tb64.Text = tb64s
                tb65.Text = tb65s
                tb66.Text = tb66s

                MA = tbModifyAlpha.Text
                MR = tbModifyRed.Text
                MG = tbModifyGreen.Text
                MB = tbModifyBlue.Text

                TransparencyEnabled = cbTransparencyEnabled.Checked

                TR = tbTransparencyRed.Text
                TG = tbTransparencyGreen.Text
                TB = tbTransparencyBlue.Text
                TT = tbTransparencyTolerance.Text

                rtb = rtbNotes

                LoadUnderway = False

            End If

        Catch ex As Exception

        End Try

    End Sub
    Friend Sub SetPictureBoxBackGrounds()

        PictureBoxPrimary.BackgroundImage = gMiniCheckerBoardImage
        PictureBoxBottom.BackgroundImage = PictureBoxPrimary.BackgroundImage
        PictureBoxRight.BackgroundImage = PictureBoxPrimary.BackgroundImage
        PictureBoxOriginal.BackgroundImage = PictureBoxPrimary.BackgroundImage

        PictureBoxPrimary.BackgroundImageLayout = ImageLayout.Tile
        PictureBoxBottom.BackgroundImageLayout = ImageLayout.Tile
        PictureBoxRight.BackgroundImageLayout = ImageLayout.Tile
        PictureBoxOriginal.BackgroundImageLayout = ImageLayout.Tile

    End Sub

    Private NavigatingTreeViewWithMouse As Boolean = False

    Private Sub TreeView1_KeyDown(sender As Object, e As KeyEventArgs) Handles TreeView1.KeyDown

        NavigatingTreeViewWithMouse = False

        If e.KeyCode = Keys.Down Then
            gLatestArrowDirectionIsDown = True
        ElseIf e.KeyCode = Keys.Up Then
            gLatestArrowDirectionIsDown = False
        End If

    End Sub

    Private Sub TreeView1_MouseDown(sender As Object, e As MouseEventArgs) Handles TreeView1.MouseDown

        NavigatingTreeViewWithMouse = True

        ' special hack to trigger TreeView1_AfterSelect if the user clicks on a folder that is already selected (TreeView1_AfterSelect would not normally fire in this circumstance)
        If sender.selectednode IsNot Nothing Then

            If sender.selectednode.tag = gTreeViewFolderTagDesignation Then

                If sender.selectednode.bounds.Contains(e.Location) Then
                    TreeView1_AfterSelect(sender, Nothing)
                End If

            End If

        End If

    End Sub
    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect

        If LoadUnderway Then Exit Sub

        If gRefreshingTreeView Then Exit Sub

        Static Dim Processing As Boolean = False

        If Processing Then Exit Sub

        Processing = True

        Me.Cursor = Cursors.WaitCursor

        Try

            Dim CanadiateToChangeTo As String

            If IgnoreTsSaveEnabled Then
            Else

                If tsSave.Enabled Then

                    LoadUnderway = True

                    Dim AfterSelectProfile As String = TreeView1.SelectedNode.Tag

                    cbName_Text = ConvertXMLFileNameToProfileName(BeforeSelectProfile)

                    CanadiateToChangeTo = TreeView1.SelectedNode.Text
                    cbName_Text = PriorProfileBeingWorkedOn

                    Dim response As DialogResult = MessageBox.Show("There are currently unsaved changes within the '" & ConvertXMLFileNameToProfileName(BeforeSelectProfile) & "' profile." & vbCrLf & vbCrLf &
                                "Please click:" & vbCrLf & vbCrLf & "'Yes' to save the changes and move," & vbCrLf & vbCrLf & "'No' to discard the changes and move, or" & vbCrLf & vbCrLf & "'Cancel' to keep everything as it is.",
                               gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)


                    Select Case response

                        Case Is = DialogResult.Yes

                            cbName_Text = PriorProfileBeingWorkedOn

                            If SaveScreenToCurrentRecord() Then

                                cbName_Text = CanadiateToChangeTo
                                tsSave.Enabled = False

                            Else

                                MessageBox.Show("Could not complete this request as there were errors in the current profile.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)


                                cbName_Text = ConvertXMLFileNameToProfileName(BeforeSelectProfile)
                                EnsureVisableAndOptionalySelelectATreeNode(BeforeSelectProfile, TreeView1, True)
                                TreeView1.Refresh()
                                tsSave.Enabled = True
                                Exit Try

                            End If

                        Case Is = DialogResult.No

                            cbName_Text = CanadiateToChangeTo
                            tsSave.Enabled = False

                        Case Is = DialogResult.Cancel

                            cbName_Text = ConvertXMLFileNameToProfileName(BeforeSelectProfile)
                            EnsureVisableAndOptionalySelelectATreeNode(BeforeSelectProfile, TreeView1, True)
                            TreeView1.Refresh()
                            tsSave.Enabled = True
                            Exit Try

                    End Select

                    LoadUnderway = False

                End If

            End If


            If sender.SelectedNode.Tag = gTreeViewFolderTagDesignation Then

                If NavigatingTreeViewWithMouse Then

                    If sender.SelectedNode.IsExpanded Then
                        sender.SelectedNode.Collapse()
                    Else
                        sender.SelectedNode.Expand()
                    End If

                    'sender.SelectedNode = Nothing
                    TreeView1.SelectedNode.EnsureVisible()

                Else

                    ' navigation with up and down arrow keyes

                    If sender.SelectedNode.IsExpanded Then
                    Else

                        sender.SelectedNode.Expand()

                        If gLatestArrowDirectionIsDown Then
                            SelectFirstChildInTreeViewNode(sender)
                        Else
                            SelectLastChildInTreeViewNode(sender)
                        End If

                    End If

                End If

                gCurrentNamesOfEffectsInUse(sender.Tag) = gDefaultEffectNotInUse

                tsRemove.Enabled = False
                tsRename.Enabled = False

                LoadNeutralEffectsProfile(False)
                LoadScreenFromCurrentEffectsProfile()

            Else

                gCurrentNamesOfEffectsInUse(CInt(sender.Tag)) = sender.SelectedNode.Tag

                If ConvertXMLFileNameToProfileName(TreeView1.SelectedNode.Tag) = gCurrentEffectsProfile.Name Then

                Else

                    LoadUnderway = True ' testing here

                    If LoadEffectsProfile(TreeView1.SelectedNode.Tag) Then  ' stop 1
                        LoadScreenFromCurrentEffectsProfile()
                    Else
                        LoadAndSaveNeutralEffectsProfile(True)
                        LoadScreenFromCurrentEffectsProfile()
                    End If
                    LoadUnderway = False

                End If

            End If

            RefreshTheScreen()

            tsSave.Enabled = False

        Catch ex As Exception

        End Try

        LoadUnderway = False

        UpdateButtonAvailability()

        gRefreshingTreeView = True

        TreeView1_Focus()

        gRefreshingTreeView = False

        Me.Refresh()

        Me.Cursor = Cursors.Default

        Processing = False

    End Sub
    Private Sub UpdateButtonAvailability()

        Dim Enabled As Boolean

        If TreeView1.SelectedNode Is Nothing Then

            Enabled = False

        Else

            If (TreeView1.SelectedNode.Tag = gTreeViewFolderTagDesignation) OrElse (TreeView1.SelectedNode.Tag = ConvertProfileNameToXMLFileName("Defaults\Neutral")) Then

                Enabled = False

            Else

                Enabled = True

            End If

        End If

        tsCopy.Enabled = Enabled
        tsRename.Enabled = Enabled
        tsRemove.Enabled = Enabled
        cbEnabled.Enabled = Enabled
        gbMatrix.Enabled = Enabled
        gbModifyColours.Enabled = Enabled
        gbTransparency.Enabled = Enabled
        gbNotes.Enabled = Enabled

        If Enabled Then
            tsTest.Enabled = tsSave.Enabled
            tsCancel.Enabled = tsSave.Enabled
        Else
            tsTest.Enabled = Enabled
            tsCancel.Enabled = Enabled
        End If

        If TreeView1.SelectedNode IsNot Nothing Then
            If TreeView1.SelectedNode.Tag = ConvertProfileNameToXMLFileName("Defaults\Neutral") Then
                cbEnabled.Enabled = True
            End If
        End If

    End Sub
    Public Sub RefreshAllProfileNames()

        If gRefreshingTreeView Then Exit Sub

        gRefreshingTreeView = True

        ValidateEffectsProfiles()

        LoadTreeView()

        LoadScreenFromCurrentEffectsProfile()

        Application.DoEvents()

        gRefreshingTreeView = False

    End Sub

    Friend Sub LoadScreenFromCurrentEffectsProfile()

        Try

            With gCurrentEffectsProfile

                cbName_Text = .Name

                cbEnabled.Checked = .Enabled

                rbBlur.Checked = ((.MatrixType = "Blur") OrElse (.MatrixType = "1"))
                rbEffect.Checked = ((.MatrixType = "Effect") OrElse (.MatrixType = "Sharpen") OrElse (.MatrixType = "0"))

                If .MatrixSize = 3 Then
                    rb3x3.Checked = True
                    rb5x5.Checked = False
                    rb7x7.Checked = False
                ElseIf .MatrixSize = 5 Then
                    rb3x3.Checked = False
                    rb5x5.Checked = True
                    rb7x7.Checked = False
                Else
                    rb3x3.Checked = False
                    rb5x5.Checked = False
                    rb7x7.Checked = True
                End If

                If (.Sigma.ToString = String.Empty) OrElse (.Sigma = 0) Then .Sigma = 1
                tbSigma.Text = .Sigma.ToString

                If (.Amount.ToString = String.Empty) Then .Amount = 0
                tbAmount.Text = .Amount.ToString

                tbBias.Text = .Bias.ToString
                tbBias.ForeColor = Drawing.Color.Black

                tbFactor.Text = .Factor.ToString
                tbFactor.ForeColor = Drawing.Color.Black

                cbCompute.Checked = .Compute
                cbSymmetric.Checked = .Symmetric

                LoadEffectsMatrixTableFromString()

                LoadEffectsMatrixTableIntoTextboxes()

                tbModifyAlpha.Text = .ModifyAlpha
                tbModifyRed.Text = .ModifyRed
                tbModifyGreen.Text = .ModifyGreen
                tbModifyBlue.Text = .ModifyBlue

                cbTransparencyEnabled.Checked = .TransparencyEnabled

                tbTransparencyRed.Text = .TransparencyRed
                tbTransparencyGreen.Text = .TransparencyGreen
                tbTransparencyBlue.Text = .TransparencyBlue
                tbTransparencyTolerance.Text = .TransparencyTolerance

                rtbNotes.Text = gCurrentEffectsProfile.Notes

            End With

            RefreshTrialImage()

        Catch ex As Exception

            LoadNeutralEffectsProfile(True)
            LoadScreenFromCurrentEffectsProfile()

        End Try

    End Sub

    Private Sub LoadEffectsMatrixTableIntoTextboxes()

        gCurrentEffectsProfile = gCurrentEffectsProfile

        With gCurrentEffectsProfile

            tb00.Text = .MatrixTable(0, 0).ToString
            tb01.Text = .MatrixTable(0, 1).ToString
            tb02.Text = .MatrixTable(0, 2).ToString
            tb03.Text = .MatrixTable(0, 3).ToString
            tb04.Text = .MatrixTable(0, 4).ToString
            tb05.Text = .MatrixTable(0, 5).ToString
            tb06.Text = .MatrixTable(0, 6).ToString

            tb10.Text = .MatrixTable(1, 0).ToString
            tb11.Text = .MatrixTable(1, 1).ToString
            tb12.Text = .MatrixTable(1, 2).ToString
            tb13.Text = .MatrixTable(1, 3).ToString
            tb14.Text = .MatrixTable(1, 4).ToString
            tb15.Text = .MatrixTable(1, 5).ToString
            tb16.Text = .MatrixTable(1, 6).ToString

            tb20.Text = .MatrixTable(2, 0).ToString
            tb21.Text = .MatrixTable(2, 1).ToString
            tb22.Text = .MatrixTable(2, 2).ToString
            tb23.Text = .MatrixTable(2, 3).ToString
            tb24.Text = .MatrixTable(2, 4).ToString
            tb25.Text = .MatrixTable(2, 5).ToString
            tb26.Text = .MatrixTable(2, 6).ToString

            tb30.Text = .MatrixTable(3, 0).ToString
            tb31.Text = .MatrixTable(3, 1).ToString
            tb32.Text = .MatrixTable(3, 2).ToString
            tb33.Text = .MatrixTable(3, 3).ToString
            tb34.Text = .MatrixTable(3, 4).ToString
            tb35.Text = .MatrixTable(3, 5).ToString
            tb36.Text = .MatrixTable(3, 6).ToString

            tb40.Text = .MatrixTable(4, 0).ToString
            tb41.Text = .MatrixTable(4, 1).ToString
            tb42.Text = .MatrixTable(4, 2).ToString
            tb43.Text = .MatrixTable(4, 3).ToString
            tb44.Text = .MatrixTable(4, 4).ToString
            tb45.Text = .MatrixTable(4, 5).ToString
            tb46.Text = .MatrixTable(4, 6).ToString

            tb50.Text = .MatrixTable(5, 0).ToString
            tb51.Text = .MatrixTable(5, 1).ToString
            tb52.Text = .MatrixTable(5, 2).ToString
            tb53.Text = .MatrixTable(5, 3).ToString
            tb54.Text = .MatrixTable(5, 4).ToString
            tb55.Text = .MatrixTable(5, 5).ToString
            tb56.Text = .MatrixTable(5, 6).ToString

            tb60.Text = .MatrixTable(6, 0).ToString
            tb61.Text = .MatrixTable(6, 1).ToString
            tb62.Text = .MatrixTable(6, 2).ToString
            tb63.Text = .MatrixTable(6, 3).ToString
            tb64.Text = .MatrixTable(6, 4).ToString
            tb65.Text = .MatrixTable(6, 5).ToString
            tb66.Text = .MatrixTable(6, 6).ToString

        End With

        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

            c.ForeColor = Drawing.Color.Black

            If c.Text = "0" Then
                c.Text = ""
            End If

        Next

    End Sub

    Private Sub tsViewer_Click(sender As Object, e As MouseEventArgs) Handles tsViewer.MouseDown

        If e.Button = MouseButtons.Left Then

            gViewChoice = ViewingOption.PrimaryandBottom
            SetLookOfWindows()

        ElseIf e.Button = MouseButtons.Middle Then

            gViewChoice = ViewingOption.PrimaryOnly

        ElseIf e.Button = MouseButtons.Right Then

            gViewChoice = ViewingOption.PrimaryandRight
            SetLookOfWindows()

        End If

        'needs to be done twice to get the seperating line to appear
        'it will have been done once before for the PrimaryandRight and the PrimaryandBottom
        'it will now be done again for either of those, or for the first time for the PrimaryOnly windows
        SetLookOfWindows()

    End Sub

    Private Sub SetLookOfWindows(Optional ByVal SkipImageRefresh As Boolean = False)

        Try

            Panel1.Location = New Point(5, 15)
            PictureBoxPrimary.Location = New Point(0, 0)

            Dim OverallContainer As Size = New Size(Panel1.Size.Width, Panel1.Size.Height)

            Select Case gViewChoice

                Case = ViewingOption.PrimaryOnly

                    PictureBoxRight.Visible = False
                    PictureBoxBottom.Visible = False
                    PictureBoxPrimary.Visible = True
                    PictureBoxOriginal.Visible = False

                    PictureBoxPrimary.Size = OverallContainer

                Case = ViewingOption.PrimaryandRight

                    PictureBoxRight.Visible = True
                    PictureBoxBottom.Visible = False
                    PictureBoxPrimary.Visible = True
                    PictureBoxOriginal.Visible = False

                    Panel1.Invalidate()

                    Dim Rounding As Integer = CInt(OverallContainer.Width / 2) Mod 2

                    PictureBoxRight.Location = New Point(OverallContainer.Width / 2 + Rounding, 0)

                    Dim NewSize As Size = New Size(OverallContainer.Width / 2 - 2, OverallContainer.Height)
                    PictureBoxPrimary.Size = NewSize
                    PictureBoxRight.Size = NewSize

                    PictureBoxBottom.Image = Nothing
                    PictureBoxRight.Invalidate()

                Case = ViewingOption.PrimaryandBottom

                    PictureBoxRight.Visible = False
                    PictureBoxBottom.Visible = True
                    PictureBoxPrimary.Visible = True
                    PictureBoxOriginal.Visible = False

                    Panel1.Invalidate()

                    Dim Rounding As Integer = CInt(OverallContainer.Height / 2) Mod 2

                    PictureBoxBottom.Location = New Point(0, OverallContainer.Height / 2 + Rounding)

                    Dim NewSize As Size = New Size(OverallContainer.Width, OverallContainer.Height / 2 - 2)
                    PictureBoxPrimary.Size = NewSize
                    PictureBoxBottom.Size = NewSize

                    PictureBoxRight.Image = Nothing
                    PictureBoxBottom.Invalidate()

                Case = ViewingOption.OriginalOnly

                    PictureBoxOriginal.Location = New Point(0, 0)

                    PictureBoxRight.Visible = False
                    PictureBoxBottom.Visible = False
                    PictureBoxPrimary.Visible = False
                    PictureBoxOriginal.Visible = True

                    PictureBoxOriginal.Image = New Bitmap(gOriginalImage)
                    PictureBoxOriginal.Size = OverallContainer

            End Select

        Catch ex As Exception

        End Try

        GarbageCollect()

    End Sub

#Region "Pan"

    Private startingPoint As Point = New Point(0, 0)
    Private panning As Boolean = False
    Private viewingOriginalOnly As Boolean = False

    Private Sub pictureBox1_MouseDown2(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBoxPrimary.MouseDown, PictureBoxRight.MouseDown, PictureBoxBottom.MouseDown

        If e.Button = e.Button.Middle Then

            Dim hViewChoice As ViewingOption = gViewChoice

            viewingOriginalOnly = True

            gViewChoice = gViewChoice.OriginalOnly
            SetLookOfWindows()
            While viewingOriginalOnly
                Threading.Thread.Sleep(20)
                Application.DoEvents()
            End While

            viewingOriginalOnly = False

            gViewChoice = hViewChoice
            SetLookOfWindows()

        Else

            panning = True
            startingPoint = New Point(e.Location.X - gMovingPoint.X, e.Location.Y - gMovingPoint.Y)
            Cursor = Cursors.Hand

        End If

    End Sub

    Private Sub pictureBox1_MouseUp2(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBoxPrimary.MouseUp, PictureBoxRight.MouseUp, PictureBoxBottom.MouseUp

        panning = False
        viewingOriginalOnly = False
        Cursor = Cursors.Default

    End Sub

    Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles PictureBoxPrimary.MouseLeave, PictureBoxRight.MouseLeave, PictureBoxBottom.MouseLeave, PictureBoxOriginal.MouseLeave

        viewingOriginalOnly = False

    End Sub

    Private Sub pictureBox1_MouseMove2(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBoxPrimary.MouseMove, PictureBoxBottom.MouseMove, PictureBoxRight.MouseMove

        Try

            If panning Then
                gMovingPoint = New Point(e.Location.X - startingPoint.X, e.Location.Y - startingPoint.Y)
                PictureBoxPrimary.Invalidate()
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub PictureBoxPrimary_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles PictureBoxPrimary.Paint

        Try

            e.Graphics.DrawImage(gTrialImage, gMovingPoint)

            If (gViewChoice = ViewingOption.PrimaryandRight) OrElse (gViewChoice = ViewingOption.PrimaryandBottom) OrElse (gViewChoice = ViewingOption.OriginalOnly) Then
                RefreshShadowBoxWindow()
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub RefreshShadowBoxWindow()

        Select Case gViewChoice

            Case = ViewingOption.PrimaryOnly
                ' done

            Case = ViewingOption.PrimaryandRight
                Me.PictureBoxRight.Invalidate()

            Case = ViewingOption.PrimaryandBottom
                Me.PictureBoxBottom.Invalidate()

            Case = ViewingOption.OriginalOnly
                Me.PictureBoxPrimary.Invalidate() ' this is PictureBoxPrimary as that is where the original image will be shown

        End Select

        GarbageCollect()

    End Sub

    Private Sub Shadowbox_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles PictureBoxRight.Paint, PictureBoxBottom.Paint, PictureBoxOriginal.Paint

        e.Graphics.DrawImage(gOriginalImage, gMovingPoint)

    End Sub

    Private Function SaveScreenToCurrentRecord(Optional ByVal TestWithoutSavingToAFileOrMySettings As Boolean = False, Optional ByVal AddedConfirmation As Boolean = True) As Boolean

        If My.Settings.PinnedToTop Then
            MakeTopMostWindow(Me.Handle.ToInt64, False)
        End If

        Dim returnValue As Boolean = False

        tbSigma.ForeColor = Drawing.Color.Black
        tbAmount.ForeColor = Drawing.Color.Black
        tbBias.ForeColor = Drawing.Color.Black
        tbFactor.ForeColor = Drawing.Color.Black

        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
            c.ForeColor = Drawing.Color.Black
        Next

        tbModifyAlpha.ForeColor = Drawing.Color.Black
        tbModifyRed.ForeColor = Drawing.Color.Black
        tbModifyGreen.ForeColor = Drawing.Color.Black
        tbModifyBlue.ForeColor = Drawing.Color.Black
        tbTransparencyRed.ForeColor = Drawing.Color.Black
        tbTransparencyGreen.ForeColor = Drawing.Color.Black
        tbTransparencyBlue.ForeColor = Drawing.Color.Black
        tbTransparencyTolerance.ForeColor = Drawing.Color.Black

        Dim ErrorFound As Boolean = False

        Dim BeepOnError As Boolean = True

        Dim RecordPreExisted As Boolean = False

        'check there is at least one other enabled profile

        If cbEnabled.Checked Then
        Else

            Dim EnabledProfileFound As Boolean = False

            ' locate the profile that is enabled
            BuildATreeNodeArray(TreeView1)

            Dim HoldofThenCurrentEffectsProfile As gEffectsProfileStructure = gCurrentEffectsProfile

            For x = 0 To gArrayOfExistingTreeNodesExcludingFolders.Length - 1

                If gArrayOfExistingTreeNodesExcludingFolders(x) = ConvertProfileNameToXMLFileName(HoldofThenCurrentEffectsProfile.Name) Then
                    ' don't count the Effects Profile that is being changed 
                Else
                    Call LoadEffectsProfile(gArrayOfExistingTreeNodesExcludingFolders(x))
                    If gCurrentEffectsProfile.Enabled Then
                        EnabledProfileFound = True
                        Exit For
                    End If
                End If

            Next

            gCurrentEffectsProfile = HoldofThenCurrentEffectsProfile

            If EnabledProfileFound Then

                ' all good

            Else

                Beep()
                BeepOnError = False
                Dim ReturnResult = MessageBox.Show("At least one profile must be enabled and currently there are no other profiles that are enabled.  Accordingly, this profile cannot be saved unless it is first enabled.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                RecordPreExisted = True
                ErrorFound = True
                TreeView1_Focus()
                GoTo ContinueProcessing

            End If

        End If

        ' Sigma

        If tbAmount.Text.Trim = "0" Then

        Else

            ' Sigma

            tbSigma.Text = tbSigma.Text.Trim

            Try

                Dim dummy As Double = CDbl(tbSigma.Text)

            Catch ex As Exception
                ErrorFound = True
                tbSigma.ForeColor = Drawing.Color.Red
                tbSigma.Select(0, 0)
                tbSigma.Focus()
                GoTo ContinueProcessing
            End Try

            Dim DummyRadiius As Double = CDbl(tbSigma.Text)
            If (DummyRadiius = 0) Then

                Beep()
                Dim ReturnResult = MessageBox.Show("The 'Sigma' may not be zero.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                ErrorFound = True
                tbSigma.Focus()
                GoTo ContinueProcessing

            End If

            ' Amount

            tbAmount.Text = tbAmount.Text.Trim

            Try

                Dim dummy As Double = CDbl(tbAmount.Text)

            Catch ex As Exception
                ErrorFound = True
                tbAmount.ForeColor = Drawing.Color.Red
                tbAmount.Select(0, 0)
                tbAmount.Focus()
                GoTo ContinueProcessing
            End Try

            Dim DummyAmount As Double = CDbl(tbAmount.Text)
            If (DummyAmount = 0) Then

                Beep()
                Dim ReturnResult = MessageBox.Show("The 'Amount' may not be zero.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                ErrorFound = True
                tbAmount.Focus()
                GoTo ContinueProcessing

            End If

        End If

        ' Bias 

        tbBias.Text = tbBias.Text.Trim

        Try

            Dim dummy As Double = CDbl(tbBias.Text)

        Catch ex As Exception
            ErrorFound = True
            tbBias.ForeColor = Drawing.Color.Red
            tbBias.Select(0, 0)
            tbBias.Focus()
            GoTo ContinueProcessing
        End Try

        Dim DummyBias As Double = CDbl(tbBias.Text)
        If (DummyBias < -255) OrElse (DummyBias > 255) Then

            Beep()
            Dim ReturnResult = MessageBox.Show("The 'Bias' must to be a number between -255 and 255 inclusive.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ErrorFound = True
            tbBias.Focus()
            GoTo ContinueProcessing

        End If

        ' Factor 

        tbFactor.Text = tbFactor.Text.Trim
        Try

            Dim dummy As Double = CDbl(tbFactor.Text)
            If dummy = 0 Then
                ErrorFound = True
                tbFactor.ForeColor = Drawing.Color.Red
                tbFactor.Select(0, 0)
                tbFactor.Focus()
                GoTo ContinueProcessing
            End If

        Catch ex As Exception
            ErrorFound = True
            tbFactor.ForeColor = Drawing.Color.Red
            tbFactor.Select(0, 0)
            tbFactor.Focus()
            GoTo ContinueProcessing
        End Try

        Dim DummyFactor As Double = CDbl(tbFactor.Text)
        If (DummyFactor = 0) OrElse (DummyFactor < -255) OrElse (DummyFactor > 255) Then

            Beep()
            Dim ReturnResult = MessageBox.Show("The 'Factor' must to be a number between -255 and 255 inclusive (excluding zero).", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ErrorFound = True
            tbFactor.Focus()
            GoTo ContinueProcessing

        End If

        ' Matrix

        Dim WorkingEffectsMatrix(6, 6) As Double

        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

            Dim x As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 3, 1))
            Dim y As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 4, 1))

            Try

                If c.Visible Then

                    c.Text = c.Text.Trim

                    If c.Text = String.Empty Then
                        WorkingEffectsMatrix(x, y) = CDbl(0)
                    Else
                        WorkingEffectsMatrix(x, y) = CDbl(c.Text)
                    End If

                    If (WorkingEffectsMatrix(x, y) = -255) OrElse (WorkingEffectsMatrix(x, y) > 255) Then
                        ErrorFound = True
                        c.ForeColor = Drawing.Color.Red
                        c.Select(0, 0)
                        c.Focus()
                        GoTo ContinueProcessing
                    End If

                End If

            Catch ex As Exception
                ErrorFound = True
                c.ForeColor = Drawing.Color.Red
                c.Select(0, 0)
                c.Focus()
                GoTo ContinueProcessing
            End Try

        Next

        ' Modify Alpha, Red, Green, and Blue

        If ValidateModifyChangeColour(tbModifyAlpha) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        If ValidateModifyChangeColour(tbModifyRed) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        If ValidateModifyChangeColour(tbModifyGreen) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        If ValidateModifyChangeColour(tbModifyBlue) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        ' Transparency Red, Green, Blue, and Tolerance

        If ValidateTransparencyColour(tbTransparencyRed) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        If ValidateTransparencyColour(tbTransparencyGreen) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        If ValidateTransparencyColour(tbTransparencyBlue) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        If ValidateTransparencyColour(tbTransparencyTolerance) Then
        Else
            ErrorFound = True
            GoTo ContinueProcessing
        End If

        ' Final edit

        If TestWithoutSavingToAFileOrMySettings Then

        Else

            Dim Sum As Double = 0

            For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

                If c.Text.Trim = String.Empty Then
                Else
                    Sum += (CDbl(c.Text) + CDbl(tbBias.Text.Trim)) * CDbl(tbFactor.Text.Trim)
                End If

            Next

            Sum = Math.Round(Sum, 14, MidpointRounding.ToEven)
            If Sum = 1 Then
            Else

                Beep()

                Dim SumTotal As String = Sum.ToString.TrimEnd(".") & "."

                If rbBlur.Checked Then


                    ' Blur

                    Dim response As DialogResult = MessageBox.Show("Generally the cells values should all add up to 1." & vbCrLf & vbCrLf &
                                        "However, the total comes to " & SumTotal & vbCrLf & vbCrLf &
                                        "Please click:" & vbCrLf & vbCrLf &
                                        "'Yes' to proceed anyway, " & vbCrLf & vbCrLf &
                                        "'No' to normalize the cells." & vbCrLf &
                                        "    Normalized cells are calculated such that" & vbCrLf &
                                        "    the relative weight of each cell is preserved and" & vbCrLf &
                                        "    all cells add up to 1." & vbCrLf & vbCrLf &
                                        "'Cancel' to keep everything as it is.", gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

                    If response = DialogResult.Cancel Then

                        ErrorFound = False
                        GoTo ContinueProcessing
                    End If

                    If response = DialogResult.No Then

                        LoadUnderway = True

                        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

                            If c.Visible Then

                                Dim x As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 3, 1))
                                Dim y As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 4, 1))

                                If c.Text = "" Then c.Text = 0
                                WorkingEffectsMatrix(x, y) = (CDbl(c.Text) / Sum)
                                c.Text = WorkingEffectsMatrix(x, y).ToString

                            End If

                        Next

                        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                            c.Enabled = False
                        Next
                        SetLookOfCells()

                        LoadUnderway = False

                    End If



                    'Else

                    ''' commented out - option for normalization of effect; if I ever put this back it I'll have to update the test button so it does this same sort of this when an effect is selected

                    '' Effect

                    'Dim response As DialogResult = MessageBox.Show("Generally the cells values, including consideration for the Bias and Factor, should all add up to 1." & vbCrLf & vbCrLf &
                    '                    "However, the total comes to " & SumTotal & vbCrLf & vbCrLf &
                    '                    "Please click:" & vbCrLf & vbCrLf &
                    '                    "'Yes' to proceed anyway, " & vbCrLf & vbCrLf &
                    '                    "'No' to normalize the cells." & vbCrLf &
                    '                    "    Normalized cells are calculated such that" & vbCrLf &
                    '                    "    the relative weight of each cell is preserved and" & vbCrLf &
                    '                    "    all cells add up to 1." & vbCrLf &
                    '                    "    Also, as needed the Bias and Factor will be adjusted." & vbCrLf & vbCrLf &
                    '                    "'Cancel' to keep everything as it is.", gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

                    'If response = DialogResult.Cancel Then
                    '    ErrorFound = False
                    '    GoTo ContinueProcessing
                    'End If

                    'If response = DialogResult.No Then

                    '    LoadUnderway = True

                    '    For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

                    '        If c.Visible Then

                    '            Dim x As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 3, 1))
                    '            Dim y As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 4, 1))

                    '            If c.Text = "" Then c.Text = 0
                    '            WorkingEffectsMatrix(x, y) = (((CDbl(c.Text) + CDbl(tbBias.Text.Trim)) * CDbl(tbFactor.Text.Trim)) / Sum)
                    '            c.Text = WorkingEffectsMatrix(x, y).ToString

                    '        End If

                    '    Next

                    '    tbBias.Text = "0"
                    '    tbFactor.Text = "1"

                    '    For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                    '        c.Enabled = False
                    '    Next
                    '    SetLookOfCells()

                    '    LoadUnderway = False

                    ' End If

                End If

            End If

        End If

ContinueProcessing:

        If ErrorFound Then

            If BeepOnError Then Beep()

        Else

            Dim Result As String = String.Empty

            For x = 0 To gMaxMatrixSize - 1
                For y = 0 To gMaxMatrixSize - 1
                    Result &= WorkingEffectsMatrix(x, y) & " "
                Next
            Next

            With gCurrentEffectsProfile

                .Name = gCurrentEffectsProfile.Name
                .Enabled = cbEnabled.Checked

                .MatrixType = IIf(rbBlur.Checked, "Blur", "Effect")

                If rb3x3.Checked Then
                    .MatrixSize = 3
                Else
                    If rb5x5.Checked Then
                        .MatrixSize = 5
                    Else
                        .MatrixSize = 7
                    End If
                End If

                .Sigma = tbSigma.Text
                .Amount = tbAmount.Text
                .Bias = CDbl(tbBias.Text)
                .Factor = CDbl(tbFactor.Text)
                .Compute = cbCompute.Checked
                .Symmetric = cbSymmetric.Checked
                .MatrixString = Result.Trim

                .ModifyAlpha = tbModifyAlpha.Text
                .ModifyRed = tbModifyRed.Text
                .ModifyGreen = tbModifyGreen.Text
                .ModifyBlue = tbModifyBlue.Text

                .TransparencyEnabled = cbTransparencyEnabled.Checked
                .TransparencyRed = tbTransparencyRed.Text
                .TransparencyGreen = tbTransparencyGreen.Text
                .TransparencyBlue = tbTransparencyBlue.Text
                .TransparencyTolerance = tbTransparencyTolerance.Text

                .Notes = rtbNotes.Text.Trim

                .MatrixTable = WorkingEffectsMatrix

            End With

            If TestWithoutSavingToAFileOrMySettings Then

            Else

                SaveCurrentEffectsProfile()

                My.Settings.EffectsProfileName = gCurrentEffectsProfile.Name
                My.Settings.Save()

                ' locate the profile being added
                BuildATreeNodeArray(TreeView1)

                Dim CurrentProfileFileName As String = ConvertProfileNameToXMLFileName(gCurrentEffectsProfile.Name)
                For x = 0 To gArrayOfExistingTreeNodesExcludingFolders.Length - 1

                    If gArrayOfExistingTreeNodesExcludingFolders(x) = CurrentProfileFileName Then
                        RecordPreExisted = True
                        Exit For
                    End If

                Next

                If RecordPreExisted Then
                Else

                    If AddedConfirmation Then

                        Beep()
                        Dim Dummy = MessageBox.Show("The profile '" & cbName_Text & "' has been added.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Question)

                    End If

                End If

                Application.DoEvents()

                RefreshAllProfileNames()

                returnValue = True

            End If

        End If

        LoadUnderway = False

        If returnValue Then

            tsSave.Enabled = False

            cbName_Text = gCurrentEffectsProfile.Name
            EnsureVisableAndOptionalySelelectATreeNode(gCurrentEffectsProfile.Name, TreeView1, True)
            TreeView1.Refresh()

            My.Settings.EffectsProfileName = gCurrentEffectsProfile.Name
            My.Settings.Save()

            TreeView1_Focus()

        End If

        If My.Settings.PinnedToTop Then
            MakeTopMostWindow(Me.Handle.ToInt64, True)
        End If

        Return returnValue

    End Function

    Friend Function ValidateModifyChangeColour(ByRef tbModify As Windows.Forms.TextBox) As Boolean

        Dim returnValue As Boolean = True

        Dim WorkingColour As String = tbModify.Name.Replace("tbModify", "")

        tbModify.Text = tbModify.Text.Trim
        If tbModify.Text = String.Empty Then
            tbModify.Text = "0"
        End If

        Dim dummyi As Integer
        Dim dummys As Single

        Try

            dummyi = CInt(tbModify.Text)
            dummys = CSng(tbModify.Text)

            If (dummyi <> dummys) OrElse (dummyi < -255) OrElse (dummyi > 255) Then
                returnValue = False
            End If

        Catch ex As Exception
            returnValue = False
        End Try

        If returnValue Then
        Else
            Beep()
            Dim ReturnResult = MessageBox.Show("The modify " & WorkingColour & " " & gColourLiteral.ToLower & " value must be a whole number between -255 and 255 inclusive.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            tbModify.ForeColor = Drawing.Color.Red
            tbModify.Select(0, 0)
            tbModify.Focus()

        End If

        Return returnValue

    End Function

    Friend Function ValidateTransparencyColour(ByRef tbModify As Windows.Forms.TextBox) As Boolean

        Dim returnValue As Boolean = True

        Dim WorkingColour As String = tbModify.Name.Replace("tbTransparency", "")

        tbModify.Text = tbModify.Text.Trim

        tbModify.Text = tbModify.Text.Trim
        If tbModify.Text = String.Empty Then
            tbModify.Text = "0"
        End If

        Dim dummyi As Integer
        Dim dummys As Single

        Try

            dummyi = CInt(tbModify.Text)
            dummys = CSng(tbModify.Text)

            If (dummyi <> dummys) OrElse (dummyi < 0) OrElse (dummyi > 255) Then
                returnValue = False
            End If

        Catch ex As Exception
            returnValue = False
        End Try

        If returnValue Then
        Else
            Beep()

            If WorkingColour = "Tolerance" Then
            Else
                WorkingColour &= " " & gColourLiteral.ToLower
            End If

            Dim ReturnResult = MessageBox.Show("The Transparency " & WorkingColour & " value must be a whole number between 0 and 255 inclusive.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            tbModify.ForeColor = Drawing.Color.Red
            tbModify.Select(0, 0)
            tbModify.Focus()

        End If

        Return returnValue

    End Function

    Private Sub tsAdd_Click(sender As Object, e As EventArgs) Handles tsAdd.Click

        Try

            If tsSave.Enabled Then

                Beep()

                Dim response As DialogResult = MessageBox.Show("There are currently unsaved changes within the '" & cbName_Text.Trim & "' profile." & vbCrLf & vbCrLf &
                                "Please click:" & vbCrLf & vbCrLf & "'Yes' to save the changes and then add a new profile," & vbCrLf & vbCrLf & "'No' to discard the changes and then add a new profile , or" & vbCrLf & vbCrLf & "'Cancel' to keep everything as it is.",
                                gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

                If response = DialogResult.Yes Then

                    If SaveScreenToCurrentRecord() Then

                        tsSave.Enabled = False
                        DoAdd()

                    Else

                        MessageBox.Show("Could not add another profile as there were errors in the current one." & vbCrLf & vbCrLf &
                                        "Please correct the errors in the current profile before adding another profile.",
                                        gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                        Exit Try

                    End If

                ElseIf response = DialogResult.No Then

                    tsSave.Enabled = False

                    LoadScreenFromCurrentEffectsProfile()

                    DoAdd()

                Else

                    Exit Try

                End If

            Else

                DoAdd()

            End If

        Catch ex As Exception

        End Try

        TreeView1.Refresh()

    End Sub

    Private Sub DoAdd()

        gRenameFunctionOriginalName = String.Empty
        gRenameFunctionProposedName = String.Empty

        Dim NewName As String = GetANewName(gRenameFunctionOriginalName, gRenameFunctionProposedName, gNamingFunction.add)

        If NewName.Length > 0 Then

            CreateDirectoriesForNewNameAsNeeded(NewName)

            LoadNeutralEffectsProfile(False)

            With gCurrentEffectsProfile
                .Name = NewName
                .Enabled = True
                .Sigma = 1
                .Amount = 1
            End With

            SaveCurrentEffectsProfile()
            LoadEffectsMatrixTableFromString()
            LoadEffectsMatrixTableIntoTextboxes()

            RefreshAllProfileNames()

            EnsureVisableAndOptionalySelelectATreeNode(gCurrentEffectsProfile.Name, TreeView1, True)
            tsSave.Enabled = False

        End If

        TreeView1_Focus()

        LoadScreenFromCurrentEffectsProfile()
        Application.DoEvents()
        MasterScreenUpdate(True)
        RefreshTrialImage()

        tsSave.Enabled = False

    End Sub

    Private Sub CreateDirectoriesForNewNameAsNeeded(ByVal newname As String)

        If newname.Contains("\") Then

            Dim PathFromProfileName As String = newname.Remove(newname.LastIndexOf("\"))

            If PathFromProfileName.StartsWith("\") Then
            Else
                PathFromProfileName = "\" & PathFromProfileName
            End If

            Dim DirectoryName As String = gXML_Path_Name & PathFromProfileName

            If Directory.Exists(DirectoryName) Then
            Else
                gignoreFileWatcherEventUntilThisTime = Now.AddMilliseconds(gFiveSeconds)
                Directory.CreateDirectory(DirectoryName)
            End If

        End If

    End Sub

    Private Sub tsCopy_Click(sender As Object, e As EventArgs) Handles tsCopy.Click

        If tsSave.Enabled Then

            Beep()

            Dim response As DialogResult = MessageBox.Show("There are currently unsaved changes within the '" & cbName_Text.Trim & "' profile." & vbCrLf & vbCrLf &
                                "Please click:" & vbCrLf & vbCrLf & "'Yes' to save the changes and copy," & vbCrLf & vbCrLf & "'No' to discard the changes and copy, or" & vbCrLf & vbCrLf & "'Cancel' to keep everything as it is.",
                               gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

            If response = DialogResult.Yes Then

                If SaveScreenToCurrentRecord() Then

                    tsSave.Enabled = False
                    DoCopy()

                Else

                    MessageBox.Show("Could not copy the current profile as there were errors in it." & vbCrLf & vbCrLf &
                                        "Please correct the errors before copying it.",
                                   gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                    Exit Sub

                End If

            ElseIf response = DialogResult.No Then

                tsSave.Enabled = False

                LoadScreenFromCurrentEffectsProfile()

                DoCopy()

            Else

                Exit Sub

            End If

        Else

            DoCopy()

        End If

    End Sub

    Private Sub DoCopy()

        gRenameFunctionOriginalName = gCurrentEffectsProfile.Name
        gRenameFunctionProposedName = gRenameFunctionOriginalName & " - copy"

        Dim NewName As String = GetANewName(gRenameFunctionOriginalName, gRenameFunctionProposedName, gNamingFunction.copy)

        If NewName.Length > 0 Then

            CreateDirectoriesForNewNameAsNeeded(NewName)

            gCurrentEffectsProfile.Name = NewName
            SaveCurrentEffectsProfile()

            RefreshAllProfileNames()

            TreeView1.Refresh()

            EnsureVisableAndOptionalySelelectATreeNode(gCurrentEffectsProfile.Name, TreeView1, True)

            TreeView1_Focus()

            PriorProfileBeingWorkedOn = gCurrentEffectsProfile.Name

            tsSave.Enabled = False

        End If

        RefreshTheScreen()

    End Sub

    Private Sub tsSave_Click(sender As Object, e As EventArgs) Handles tsSave.Click

        Me.Cursor = Cursors.WaitCursor

        SaveScreenToCurrentRecord()
        SetTransparencyColours()
        RefreshTrialImage()

        Me.Cursor = Cursors.Default

    End Sub

    Private Sub tsRemove_Click(sender As Object, e As EventArgs) Handles tsRemove.Click

        Dim ProfileToRemove As String = gCurrentEffectsProfile.Name

        If ProfileToRemove = gNeutralEffectsProfileName Then
            MessageBox.Show("This is a special profile, it may not be removed.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Dim FileToRemove As String = ConvertProfileNameToXMLFileName(ProfileToRemove)

        For x As Integer = 1 To gCurrentNumberOfEffectsBoxesInUse

            If gCurrentNamesOfEffectsInUse(x) = ConvertProfileNameToXMLFileName(ProfileToRemove) Then

                Beep()
                If MessageBox.Show("The profile '" & ProfileToRemove & "' is currently being applied to your image." & vbCrLf & vbCrLf &
                               "Are you sure you want to remove it?",
                                gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then


                    For y = 1 To gCurrentNumberOfEffectsBoxesInUse

                        If gCurrentNamesOfEffectsInUse(x) = ConvertProfileNameToXMLFileName(ProfileToRemove) Then
                            gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse
                        End If

                    Next

                    Exit For

                Else

                    Exit Sub

                End If

            End If

        Next

        If MessageBox.Show("Are you sure you want to remove the profile '" & ProfileToRemove & "'?", gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
        Else
            Exit Sub
        End If

        ' Once the profile is removed we will have to reposition the selected item in the tree view someplace
        ' this determines where that will be 

        ' get all filenames in listview (in gArrayOfExistingTreeNodesExcludingFolders) (this is done because the sort order of the list view differs from the profile list of string)
        BuildATreeNodeArray(TreeView1)

        ' locate the profile being removed
        Dim CurrentProfileFileName As String = ConvertProfileNameToXMLFileName(ProfileToRemove)
        Dim CurrentIndex As Integer

        For x = 0 To gArrayOfExistingTreeNodesExcludingFolders.Length - 1

            If gArrayOfExistingTreeNodesExcludingFolders(x) = CurrentProfileFileName Then
                CurrentIndex = x
                Exit For
            End If
        Next

        ' locate the profile to be the new selected item once the profile being removed is removed

        Dim RepositionToProfileFileName As String

        If (CurrentIndex + 1) <= (gArrayOfExistingTreeNodesExcludingFolders.Length - 1) Then

            'ideally position under the current profile
            RepositionToProfileFileName = gArrayOfExistingTreeNodesExcludingFolders(CurrentIndex + 1)

        Else

            ' otherwise position above the profile being deleted

            If gArrayOfExistingTreeNodesExcludingFolders.Length = 1 Then
                RepositionToProfileFileName = String.Empty
            Else
                RepositionToProfileFileName = gArrayOfExistingTreeNodesExcludingFolders(CurrentIndex - 1)
            End If

        End If

        gignoreFileWatcherEventUntilThisTime = Now.AddMilliseconds(gFiveSeconds)
        System.IO.File.Delete(FileToRemove)

        LoadUnderway = True
        RefreshAllProfileNames()
        TreeView1.Refresh()

        EnsureVisableAndOptionalySelelectATreeNode(RepositionToProfileFileName, TreeView1, True)

        LoadEffectsProfile(RepositionToProfileFileName)
        LoadScreenFromCurrentEffectsProfile()

        LoadUnderway = False
        tsSave.Enabled = False

        RefreshTheScreen()

    End Sub

    Private Sub tsLocate_Click(sender As Object, e As EventArgs) Handles tsLocate.Click

        Dim procstart As New ProcessStartInfo("explorer")
        procstart.Arguments = gXML_Path_Name
        Process.Start(procstart)

        Threading.Thread.Sleep(1500)
        Application.DoEvents()

    End Sub

    Private IgnoreTsSaveEnabled As Boolean = False

    Private Sub RefreshTrialImage()

        If LoadUnderway Then Exit Sub

        gTrialImage = New Bitmap(gOriginalImage)

        EffectsNow(gTrialImage, gCurrentEffectsProfile.Name)

        PictureBoxPrimary.Invalidate()

        GarbageCollect()

    End Sub

    Private Sub tsTest_Click(sender As Object, e As EventArgs) Handles tsTest.Click

        NormalizeAsneeded()
        RefreshTheScreen()

    End Sub

    Private Sub NormalizeAsneeded()

        Try

            If rbBlur.Checked AndAlso (Not cbCompute.Checked) Then

                Dim Sum As Double = 0

                For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                    If c.Text.Trim = String.Empty Then
                    Else
                        Sum += CDbl(c.Text)
                    End If
                Next

                Sum = Math.Round(Sum, 14, MidpointRounding.ToEven)
                If Sum = 1 Then
                Else

                    If My.Settings.PinnedToTop Then
                        MakeTopMostWindow(Me.Handle.ToInt64, False)
                    End If

                    Beep()

                    Dim SumTotal As String = Sum.ToString.TrimEnd(".") & "."

                    Dim response As DialogResult = MessageBox.Show("Generally for a Blur test the cells should all add up to 1." & vbCrLf & vbCrLf &
                                            "However, the total comes to " & SumTotal & vbCrLf & vbCrLf &
                                            "Please click:" & vbCrLf & vbCrLf &
                                            "'Yes' to proceed with the test anyway, " & vbCrLf & vbCrLf &
                                            "'No' to normalize the cells before doing the test." & vbCrLf &
                                            "    Normalized cells are calculated such that" & vbCrLf &
                                            "    the relative weight of each cell is preserved and" & vbCrLf &
                                            "    all cells add up to 1." & vbCrLf & vbCrLf &
                                            "'Cancel' to keep everything as it is.", gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

                    If My.Settings.PinnedToTop Then
                        MakeTopMostWindow(Me.Handle.ToInt64, True)
                    End If

                    If response = DialogResult.Cancel Then
                        Exit Sub
                    End If

                    If response = DialogResult.No Then

                        '  ConvertingFromBlurToEffectRequiresNormalization = True

                        LoadUnderway = True

                        Dim WorkingEffectsMatrix(6, 6) As Double

                        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

                            If c.Visible Then

                                Dim x As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 3, 1))
                                Dim y As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 4, 1))

                                If c.Text = "" Then c.Text = 0
                                WorkingEffectsMatrix(x, y) = CDbl(c.Text) / Sum
                                c.Text = WorkingEffectsMatrix(x, y).ToString

                            End If

                        Next

                        gCurrentEffectsProfile.MatrixTable = WorkingEffectsMatrix

                        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                            c.Enabled = False
                        Next

                        SetLookOfCells()

                        LoadUnderway = False

                    End If

                End If

            End If

        Catch ex As Exception

            Beep()

        End Try

    End Sub

    Private Sub RefreshTheScreen()

        Dim HoldtsSaveEnabled As Boolean = tsSave.Enabled

        MasterScreenUpdate(True)

        Application.DoEvents()
        Call SaveScreenToCurrentRecord(True)
        RefreshTrialImage()

        tsSave.Enabled = HoldtsSaveEnabled

    End Sub

    Private Sub tsCancel_Click(sender As Object, e As EventArgs) Handles tsCancel.Click

        CancelLogic(False)
        tsSave.Enabled = False

    End Sub

    Private Sub CancelLogic(ByVal UserHasChosenToCloseTheWindow As Boolean)

        If My.Settings.PinnedToTop Then
            MakeTopMostWindow(Me.Handle.ToInt64, False)
        End If

        Try

            If tsSave.Enabled Then

                If complicatedCancel Then

                    complicatedCancel = False

                    Beep()

                    Dim response As DialogResult

                    If UserHasChosenToCloseTheWindow Then

                        response = MessageBox.Show("As you may recall, earlier you chose to continue to work on this profile even though it had been changed in the data folder." & vbCrLf & vbCrLf &
                                       "Given you have now chosen to cancel, A Viewer for Windows needs to know what you would like to happen next." & vbCrLf & vbCrLf &
                                       "Please click:" & vbCrLf & vbCrLf &
                                       "'Yes' to save the original version of this profile which you started working with in your data folder." & vbCrLf & vbCrLf &
                                       "'No' to leave the new version of this profile in your data folder.", gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)

                        If response = DialogResult.Yes Then

                            gCurrentEffectsProfile = AV4WsCurrentProfileVersionForComplicatedCancel
                            SaveCurrentEffectsProfile()
                            Exit Try

                        ElseIf response = DialogResult.No Then

                            Exit Try

                        End If


                    Else

                        response = MessageBox.Show("As you may recall, earlier you chose to continue to work on this profile even though it had been changed in the data folder." & vbCrLf & vbCrLf &
                                       "Given you have now chosen to cancel, A Viewer for Windows needs to know what you would like to happen next." & vbCrLf & vbCrLf &
                                       "Please click:" & vbCrLf & vbCrLf &
                                       "'Yes' to revert to the original version of this profile which you started working with." & vbCrLf & vbCrLf &
                                       "'No' to revert to the new version of this profile now in your data folder." & vbCrLf & vbCrLf &
                                       "'Cancel' to leave everything on your screen as is.", gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)


                        If response = DialogResult.Yes Then

                            gCurrentEffectsProfile = AV4WsCurrentProfileVersionForComplicatedCancel
                            LoadScreenFromCurrentEffectsProfile()
                            tsSave.Enabled = False
                            Exit Try

                        ElseIf response = DialogResult.No Then

                            tsSave.Enabled = False
                            Exit Try

                        Else

                            complicatedCancel = True
                            Exit Try

                        End If

                    End If


                Else

                    Beep()

                    Dim msg As String

                    If cbName_Text = String.Empty Then
                        msg = "If you cancel, the changes made to the as of yet unnamed proile will be lost."
                    Else
                        msg = "If you cancel, the changes made to the '" & cbName_Text & "' proile will be lost."
                    End If

                    If MessageBox.Show("If you cancel, the changes made to the '" & cbName_Text & "' proile will be lost." & vbCrLf & vbCrLf &
                                       "Would you like to cancel?",
                                       gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then

                        LoadUnderway = True

                        cbName_Text = gCurrentEffectsProfile.Name
                        BeforeSelectProfile = cbName_Text

                        tsSave.Enabled = False

                        If LoadEffectsProfile(BeforeSelectProfile) Then
                        Else
                            tsSave.Enabled = False
                            LoadEffectsProfile(gNeutralEffectsProfileName)
                        End If

                        tsSave.Enabled = False  ' leave
                        LoadUnderway = False    ' this 
                        tsSave.Enabled = False  ' alone ???

                    Else

                        Exit Try

                    End If

                End If

            End If

        Catch ex As Exception

        End Try


        If UserHasChosenToCloseTheWindow Then
        Else

            LoadUnderway = True 'testing here
            LoadScreenFromCurrentEffectsProfile()
            LoadUnderway = False

            RefreshTheScreen()

            If My.Settings.PinnedToTop Then
                MakeTopMostWindow(Me.Handle.ToInt64, True)
            End If

        End If

    End Sub

    Private Sub rtbNotes_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbNotes.LinkClicked

        Dim OKToOpen As Boolean = False

        If My.Settings.ShowHyperlinkWarning Then

            Dim frmHyperlinkWarning As frmHyperlinkWarning = New frmHyperlinkWarning

            gOpenOkedByUser = False
            gLinkToBeOpened = e.LinkText
            frmHyperlinkWarning.ShowDialog()
            frmHyperlinkWarning.Dispose()
            OKToOpen = gOpenOkedByUser

        Else

            OKToOpen = True

        End If

        If OKToOpen Then
            System.Diagnostics.Process.Start(e.LinkText)
        End If

        gLinkToBeOpened = String.Empty
        GarbageCollect()

    End Sub
    Private Sub Me_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        Dim UpdateEffectsProfileInMySettings As Boolean = False

        RemoveHandler AnAV4WNotificationEvent, AddressOf ReactToAnAV4WNotificationEvent

        If tsSave.Enabled Then

            If complicatedCancel Then

                CancelLogic(True)

            Else

                If MessageBox.Show("Would you like to save the changes you made to the '" & cbName_Text & "' profile?",
                               gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                    SaveScreenToCurrentRecord()
                    UpdateEffectsProfileInMySettings = True

                End If

            End If

        Else

            If cbName_Text <> My.Settings.EffectsProfileName Then
                UpdateEffectsProfileInMySettings = True
            End If

        End If

        'update if the code directly above or the Accept button was clicked 
        If UpdateEffectsProfileInMySettings Then
            My.Settings.EffectsProfileName = gCurrentEffectsProfile.Name
            My.Settings.Save()
        End If

        'Reset Alpha
        LoadNeutralEffectsProfile(False)

    End Sub

#End Region

#Region "Split Container control"

    Const SplitContainer1_MinWidth As Integer = 25
    Const SplitContainer1_ThresholdForIconsOnly As Integer = 30
    Private SplitContainer1_MaxWidth As Integer = 117

    Private gShowingSplitContainerText As Boolean = True
    Private gCurrentSplitContainerWidth As Integer = -1

    Dim gIgnorChangesToSplitContainerWidth As Boolean = False
    Private Sub SplitContainer1_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles SplitContainer1.SplitterMoved

        ' this code is a hack to ensure the split line does change when the form is resized
        ' the 200 represents a distance far enought away from the split line to indicate the mouse is not near the split line and actively resizing it
        ' rather more likely it far away in the bottom right corner its resizing the entire window

        If SplitContainer1.Panel1.PointToClient(MousePosition).X > 200 Then
            If gShowingSplitContainerText Then
                SplitContainer1.SplitterDistance = SplitContainer1_MaxWidth
            Else
                SplitContainer1.SplitterDistance = SplitContainer1_MinWidth
            End If
            Exit Sub
        End If


        Static Dim LastWindowsState As Windows.WindowState = WindowState.Maximized

        If (Me.WindowState = WindowState.Maximized) AndAlso (LastWindowsState = WindowState.Normal) Then
            gIgnorChangesToSplitContainerWidth = True
        End If

        LastWindowsState = Me.WindowState

        If gIgnorChangesToSplitContainerWidth Then Exit Sub

        gIgnorChangesToSplitContainerWidth = True

        If SplitContainer1.Panel1.Width >= SplitContainer1_ThresholdForIconsOnly Then

            SplitContainer1.SplitterDistance = SplitContainer1_MaxWidth
            UpdateLookOfSplitContainer(True, False)

        ElseIf SplitContainer1.Panel1.Width <= SplitContainer1_MinWidth Then

            SplitContainer1.SplitterDistance = SplitContainer1_MinWidth
            UpdateLookOfSplitContainer(False, False)

        End If

        gIgnorChangesToSplitContainerWidth = False

    End Sub

    Private Sub UpdateLookOfSplitContainer(ByVal ShowText As Boolean, ByVal ForceChange As Boolean)

        gIgnorChangesToSplitContainerWidth = True

        If ShowText Then

            If ForceChange OrElse (Not gShowingSplitContainerText) Then

                gShowingSplitContainerText = True
                SplitContainer1.SplitterDistance = SplitContainer1_MaxWidth

                For Each item As ToolStripItem In ToolStrip1.Items
                    item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                Next

            End If

        Else

            If ForceChange OrElse gShowingSplitContainerText Then

                gShowingSplitContainerText = False
                SplitContainer1.SplitterDistance = SplitContainer1_MinWidth

                If gViewChoice = ViewingOption.PrimaryandBottom OrElse gViewChoice = ViewingOption.PrimaryOnly Then
                    PictureBoxPrimary.Width = PictureBoxPrimary.Width + (SplitContainer1_MaxWidth - SplitContainer1_MinWidth)
                    PictureBoxBottom.Width = PictureBoxPrimary.Width + (SplitContainer1_MaxWidth - SplitContainer1_MinWidth)
                Else
                    PictureBoxPrimary.Width = PictureBoxPrimary.Width + (SplitContainer1_MaxWidth - SplitContainer1_MinWidth) / 2
                    PictureBoxRight.Width = PictureBoxPrimary.Width + (SplitContainer1_MaxWidth - SplitContainer1_MinWidth) / 2
                End If

                For Each item As ToolStripItem In ToolStrip1.Items
                    item.DisplayStyle = ToolStripItemDisplayStyle.Image
                Next

            End If

        End If

        gIgnorChangesToSplitContainerWidth = False

    End Sub

#End Region

    Private Sub frmPickEffectsMatrix_Resize(sender As Object, e As EventArgs) Handles Me.Resize

        SetLookOfWindows()

    End Sub

#Region "Magnify"

    Private Sub tsMagnify_MouseDown(sender As Object, e As MouseEventArgs) Handles tsMagnify.MouseDown

        tsMagnify_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsMagnify_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        tsMagnify.Enabled = False

        Dim MagniferIsRunning As Boolean = (Diagnostics.Process.GetProcessesByName("magnify").Length > 0)

        If MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) OrElse MagniferIsRunning Then

            'close the magnfier
            SendTheTransmissionString("{WIN}{ESC}")

        Else

            'start the magnifier
            System.Diagnostics.Process.Start(gMagnifierProgram)
            Threading.Thread.Sleep(250)

        End If

        tsMagnify.Enabled = True

    End Sub

    Private SymmetricUnderway As Boolean = False

    Private Sub cbTransparencyEnabled_CheckedChanged(sender As Object, e As EventArgs) Handles cbTransparencyEnabled.CheckedChanged

        If LoadUnderway Then Exit Sub

        tsSave.Enabled = True

    End Sub

    Private Sub SetTransparencyColours()

        Try

            SetGlobalTransparencyColour(Color.FromArgb(gCurrentEffectsProfile.TransparencyRed, gCurrentEffectsProfile.TransparencyGreen, gCurrentEffectsProfile.TransparencyBlue))
            If gCurrentEffectsProfile.TransparencyEnabled Then
                AllowTransparency = True
            Else
                AllowTransparency = False
            End If

            UpdateBtnFindBackColour()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub UpdateBtnFindBackColour()

        Try

            btnFind.BackColor = Color.FromArgb(CInt(tbTransparencyRed.Text), CInt(tbTransparencyGreen.Text), CInt(tbTransparencyBlue.Text))

        Catch ex As Exception

            btnFind.BackColor = Color.FromArgb(255, 255, 255)

        End Try

    End Sub

    Private Sub some_TextChanged(sender As Object, e As EventArgs) Handles cbEnabled.CheckedChanged, rtbNotes.TextChanged, cbTransparencyEnabled.CheckedChanged

        If SymmetricUnderway OrElse LoadUnderway Then Exit Sub

        tsSave.Enabled = True

    End Sub

    Private ConvertingFromBlurToEffect As Boolean = False
    Private ConvertingFromBlurToEffectRequiresNormalization As Boolean = False

    Private Sub BlurChanged(sender As Object, e As EventArgs) Handles rbBlur.CheckedChanged

        ConvertingFromBlurToEffectRequiresNormalization = False

        If SymmetricUnderway OrElse LoadUnderway Then Exit Sub

        Try

            tsSave.Enabled = True

            If rbEffect.Checked Then

                ConvertingFromBlurToEffect = True

                If CDbl(tbAmount.Text.Trim) = 0 Then

                    If My.Settings.PinnedToTop Then
                        MakeTopMostWindow(Me.Handle.ToInt64, False)
                    End If

                    Dim response As DialogResult = MessageBox.Show("Generally, when converting from a Blur to an Effect the value for 'Amount' should not be zero." & vbCrLf & vbCrLf &
                                         "Please click:" & vbCrLf & "'Yes' to proceed anyway, or" & vbCrLf & "'No' to keep everything as it is.",
                                         gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)

                    If My.Settings.PinnedToTop Then
                        MakeTopMostWindow(Me.Handle.ToInt64, True)
                    End If

                    If response = DialogResult.No Then
                        rbBlur.Checked = True
                        Exit Sub
                    End If

                End If

                If Not cbCompute.Checked Then

                    Dim Sum As Double = 0

                    For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                        If c.Text.Trim = String.Empty Then
                        Else
                            Sum += (CDbl(c.Text) + CDbl(tbBias.Text.Trim)) * CDbl(tbFactor.Text.Trim)
                        End If
                    Next

                    Sum = Math.Round(Sum, 14, MidpointRounding.ToEven)
                    If Sum = 1 Then
                    Else

                        If My.Settings.PinnedToTop Then
                            MakeTopMostWindow(Me.Handle.ToInt64, False)
                        End If

                        Beep()

                        Dim SumTotal As String = Sum.ToString.TrimEnd(".") & "."

                        Dim response As DialogResult = MessageBox.Show("Generally the cells values should all add up to 1." & vbCrLf & vbCrLf &
                                        "However, the total comes to " & SumTotal & vbCrLf & vbCrLf &
                                        "Please click:" & vbCrLf & vbCrLf &
                                        "'Yes' to proceed anyway, " & vbCrLf & vbCrLf &
                                        "'No' to normalize the cells." & vbCrLf &
                                        "    Normalized cells are calculated such that" & vbCrLf &
                                        "    the relative weight of each cell is preserved and" & vbCrLf &
                                        "    all cells add up to 1." & vbCrLf & vbCrLf &
                                        "'Cancel' to keep everything as it is.", gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)


                        If My.Settings.PinnedToTop Then
                            MakeTopMostWindow(Me.Handle.ToInt64, True)
                        End If

                        If response = DialogResult.Cancel Then
                            rbBlur.Checked = True
                            Exit Sub
                        End If

                        If response = DialogResult.No Then

                            ConvertingFromBlurToEffectRequiresNormalization = True

                            LoadUnderway = True

                            Dim WorkingEffectsMatrix(6, 6) As Double

                            For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

                                If c.Visible Then

                                    Dim x As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 3, 1))
                                    Dim y As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 4, 1))

                                    If c.Text = "" Then c.Text = 0
                                    WorkingEffectsMatrix(x, y) = CDbl(c.Text) / Sum
                                    c.Text = WorkingEffectsMatrix(x, y).ToString

                                End If

                            Next

                            tbBias.Text = "0"
                            tbFactor.Text = "1"

                            gCurrentEffectsProfile.MatrixTable = WorkingEffectsMatrix

                            For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                                c.Enabled = False
                            Next
                            SetLookOfCells()

                            LoadUnderway = False

                        End If

                    End If

                End If

            End If

        Catch ex As Exception

            rbBlur.Checked = True
            Beep()
            MessageBox.Show("A problem was found when converting the blur." & vbCrLf & vbCrLf &
                            "Likely a cell, the Sigma or Amount field contains an invalid number.",
                            gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)


        End Try

        RefreshTheScreen()

    End Sub

    Private Sub MatrixSizeOrComputeChanged(sender As Object, e As EventArgs) Handles rb3x3.CheckedChanged, rb5x5.CheckedChanged, rb7x7.CheckedChanged, cbCompute.CheckedChanged

        If SymmetricUnderway OrElse LoadUnderway Then Exit Sub

        tsSave.Enabled = True

        MasterScreenUpdate(True)

    End Sub

    Private Sub MasterScreenUpdate(ByVal UpdateCellValuesAsRequired As Boolean)

        If SymmetricUnderway OrElse LoadUnderway Then Exit Sub

        Me.SuspendLayout()

        tsSave.Enabled = True

        SetLookOfCells()

        ' make visible, or not, Compute based on matrix type
        cbCompute.Visible = rbBlur.Checked

        'make visible, or not, Ammount and Sigma based on matrix type

        lblSigma.Visible = rbBlur.Checked
        tbSigma.Visible = rbBlur.Checked

        lblAmount.Visible = rbBlur.Checked
        tbAmount.Visible = rbBlur.Checked

        lblBias.Visible = rbEffect.Checked
        tbBias.Visible = rbEffect.Checked

        lblFactor.Visible = rbEffect.Checked
        tbFactor.Visible = rbEffect.Checked

        ' autofill cells if blur and Compute are selected 

        Application.DoEvents()

        If cbCompute.Checked OrElse (rbEffect.Checked AndAlso ConvertingFromBlurToEffect) Then

            If rbBlur.Checked Then

                'Create a simple blur matrix

                Dim matrixSize As Integer
                If rb3x3.Checked Then
                    matrixSize = 3
                ElseIf rb5x5.Checked Then
                    matrixSize = 5
                Else
                    matrixSize = 7
                End If

                Dim sigma As Double = CDbl(tbSigma.Text)

                Dim blurMatrix(matrixSize - 1, matrixSize - 1) As Double
                CreateABlurMatrix(matrixSize, sigma, blurMatrix)

                gCurrentEffectsProfile.MatrixTable = blurMatrix

            Else

                'Create an effect matrix

                Dim matrixSize As Integer
                Dim offset As Integer

                If rb3x3.Checked Then
                    matrixSize = 3
                    offset = 2
                ElseIf rb5x5.Checked Then
                    matrixSize = 5
                    offset = 1
                Else
                    matrixSize = 7
                    offset = 0
                End If

                Dim blurMatrix(matrixSize - 1, matrixSize - 1) As Double

                If (rbEffect.Checked AndAlso ConvertingFromBlurToEffect) Then

                    If ConvertingFromBlurToEffectRequiresNormalization Then
                        ConvertingFromBlurToEffectRequiresNormalization = False
                        For x = 0 To matrixSize - 1
                            For y = 0 To matrixSize - 1

                                blurMatrix(x, y) = gCurrentEffectsProfile.MatrixTable(x + offset, y + offset)

                            Next
                        Next

                    Else

                        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

                            If c.Visible Then

                                Dim x As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 3, 1)) - offset
                                Dim y As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 4, 1)) - offset

                                If c.Text = "" Then c.Text = 0
                                blurMatrix(x, y) = CDbl(c.Text.ToString)

                            End If

                        Next


                    End If


                Else

                    Dim sigma As Double = CDbl(tbSigma.Text)

                    ' Create the matrices needed in the calucations
                    CreateABlurMatrix(matrixSize, sigma, blurMatrix)

                End If

                ' create the identity matrix

                Dim identity7x7Matrix = New Double(,) {{0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 1, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}}

                Dim identity5x5Matrix = New Double(,) {{0, 0, 0, 0, 0}, {0, 0, 0, 0, 0}, {0, 0, 1, 0, 0}, {0, 0, 0, 0, 0}, {0, 0, 0, 0, 0}}

                Dim identity3x3Matrix = New Double(,) {{0, 0, 0}, {0, 1, 0}, {0, 0, 0}}

                Dim identityMatrix(,) As Double

                If matrixSize = 3 Then
                    identityMatrix = identity3x3Matrix
                ElseIf matrixSize = 5 Then
                    identityMatrix = identity5x5Matrix
                Else
                    identityMatrix = identity7x7Matrix
                End If

                ' create the resultMatrix
                Dim resultMatrix(matrixSize - 1, matrixSize - 1) As Double

                ' do the math

                For x = 0 To matrixSize - 1
                    For y = 0 To matrixSize - 1
                        resultMatrix(x, y) = identityMatrix(x, y) - blurMatrix(x, y)
                        resultMatrix(x, y) *= CDbl(tbAmount.Text)
                        resultMatrix(x, y) = Math.Round(resultMatrix(x, y), 14, MidpointRounding.ToEven)  ' needed to ensure numbers like .9999999 don't show up
                        resultMatrix(x, y) += identityMatrix(x, y)
                    Next
                Next

                '' for testing populates matrix with numbering
                'For x = 0 To matrixSize - 1
                '    For y = 0 To matrixSize - 1
                '        resultMatrix(x, y) = x + y / 10
                '    Next
                'Next

                gCurrentEffectsProfile.MatrixTable = resultMatrix

            End If

        End If


        ' load the numbers on the screen 

        If UpdateCellValuesAsRequired Then

            If (rbBlur.Checked AndAlso cbCompute.Checked) OrElse (rbEffect.Checked AndAlso ConvertingFromBlurToEffect) Then

                Dim matrixSize As Integer

                If rb3x3.Checked Then
                    matrixSize = 3
                ElseIf rb5x5.Checked Then
                    matrixSize = 5
                Else
                    matrixSize = 7
                End If

                'transform a 3x3 matrix to a 7x7 matrix
                If gCurrentEffectsProfile.MatrixTable.Length = 9 Then

                    Dim WorkingMatrixTable(gMaxMatrixSize, gMaxMatrixSize) As Double

                    For x = 0 To 2
                        For y = 0 To 2
                            WorkingMatrixTable(x + 2, y + 2) = gCurrentEffectsProfile.MatrixTable(x, y)
                        Next
                    Next

                    ReDim gCurrentEffectsProfile.MatrixTable(gMaxMatrixSize, gMaxMatrixSize)
                    gCurrentEffectsProfile.MatrixTable = WorkingMatrixTable

                End If

                'transform a 5x5 matrix to a 7x7 matrix

                If gCurrentEffectsProfile.MatrixTable.Length = 25 Then

                    Dim WorkingMatrixTable(gMaxMatrixSize, gMaxMatrixSize) As Double

                    For x = 0 To 4
                        For y = 0 To 4
                            WorkingMatrixTable(x + 1, y + 1) = gCurrentEffectsProfile.MatrixTable(x, y)
                        Next
                    Next

                    ReDim gCurrentEffectsProfile.MatrixTable(gMaxMatrixSize, gMaxMatrixSize)
                    gCurrentEffectsProfile.MatrixTable = WorkingMatrixTable

                End If


                For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

                    Try

                        Dim x As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 3, 1))
                        Dim y As Integer = CInt(Microsoft.VisualBasic.Mid(c.Name, 4, 1))

                        If matrixSize = 3 Then

                            If (x > 1 AndAlso x < 5) AndAlso (y > 1 AndAlso y < 5) Then
                                c.Text = gCurrentEffectsProfile.MatrixTable(x, y).ToString("F20").TrimEnd("0"c).TrimEnd("."c)
                            End If

                        ElseIf matrixSize = 5 Then

                            If (x > 0 AndAlso x < 6) AndAlso (y > 0 AndAlso y < 6) Then
                                c.Text = gCurrentEffectsProfile.MatrixTable(x, y).ToString("F20").TrimEnd("0"c).TrimEnd("."c)
                            End If

                        Else

                            c.Text = gCurrentEffectsProfile.MatrixTable(x, y).ToString("F20").TrimEnd("0"c).TrimEnd("."c)

                        End If

                    Catch ex As Exception

                    End Try

                Next

            End If

        End If

        ConvertingFromBlurToEffect = False

        ' reset the error colours in the cells
        ' reset zeros to spaces

        For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls

            c.ForeColor = Drawing.Color.Black

            Try

                c.Text = c.Text.Trim
                If c.Text = String.Empty Then
                Else

                    If (CDbl(c.Text) > 255) OrElse (CDbl(c.Text) < -255) Then
                        c.ForeColor = Drawing.Color.Red
                    End If

                End If

                If c.Text = "0" Then
                    c.Text = ""
                End If

            Catch ex As Exception
                c.ForeColor = Drawing.Color.Red
            End Try

        Next


        'set the transparency colours
        SetTransparencyColours()

        Me.ResumeLayout()

    End Sub


    Private Sub SetLookOfCells()

        ' make visible, or not, cells based on matrix size

        tb00.Visible = rb7x7.Checked
        tb01.Visible = rb7x7.Checked
        tb02.Visible = rb7x7.Checked
        tb03.Visible = rb7x7.Checked
        tb04.Visible = rb7x7.Checked
        tb05.Visible = rb7x7.Checked
        tb06.Visible = rb7x7.Checked

        tb10.Visible = rb7x7.Checked
        tb11.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb12.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb13.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb14.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb15.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb16.Visible = rb7x7.Checked

        tb20.Visible = rb7x7.Checked
        tb21.Visible = rb7x7.Checked OrElse rb5x5.Checked

        tb25.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb26.Visible = rb7x7.Checked

        tb30.Visible = rb7x7.Checked
        tb31.Visible = rb7x7.Checked OrElse rb5x5.Checked

        tb35.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb36.Visible = rb7x7.Checked

        tb40.Visible = rb7x7.Checked
        tb41.Visible = rb7x7.Checked OrElse rb5x5.Checked

        tb45.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb46.Visible = rb7x7.Checked

        tb50.Visible = rb7x7.Checked
        tb51.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb52.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb53.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb54.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb55.Visible = rb7x7.Checked OrElse rb5x5.Checked
        tb56.Visible = rb7x7.Checked

        tb60.Visible = rb7x7.Checked
        tb61.Visible = rb7x7.Checked
        tb62.Visible = rb7x7.Checked
        tb63.Visible = rb7x7.Checked
        tb64.Visible = rb7x7.Checked
        tb65.Visible = rb7x7.Checked
        tb66.Visible = rb7x7.Checked

        ' enable all cells if blur and not Compute
        If rbBlur.Checked AndAlso (Not cbCompute.Checked) Then

            For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                c.Enabled = True
            Next

        Else

            ' enable or disable all cells based on matrix type 
            For Each c As System.Windows.Forms.TextBox In Me.TableLayoutPanel1.Controls
                c.Enabled = rbEffect.Checked
            Next

        End If

        ' enable or disable cells based on symmetry

        ' note: cells are in tbyx format

        If cbSymmetric.Checked Then

            tb04.Enabled = False
            tb05.Enabled = False
            tb06.Enabled = False

            tb10.Enabled = False
            tb14.Enabled = False
            tb15.Enabled = False
            tb16.Enabled = False

            tb20.Enabled = False
            tb21.Enabled = False
            tb24.Enabled = False
            tb25.Enabled = False
            tb26.Enabled = False

            tb30.Enabled = False
            tb31.Enabled = False
            tb32.Enabled = False
            tb34.Enabled = False
            tb35.Enabled = False
            tb36.Enabled = False

            tb40.Enabled = False
            tb41.Enabled = False
            tb42.Enabled = False
            tb43.Enabled = False
            tb44.Enabled = False
            tb45.Enabled = False
            tb46.Enabled = False

            tb50.Enabled = False
            tb51.Enabled = False
            tb52.Enabled = False
            tb53.Enabled = False
            tb54.Enabled = False
            tb55.Enabled = False
            tb56.Enabled = False

            tb60.Enabled = False
            tb61.Enabled = False
            tb62.Enabled = False
            tb63.Enabled = False
            tb64.Enabled = False
            tb65.Enabled = False
            tb66.Enabled = False

        End If

    End Sub

    Private Sub somemore_KeyDownChanged(sender As Object, e As KeyEventArgs) Handles tb00.KeyDown, tb10.KeyDown, tb20.KeyDown, tb30.KeyDown, tb40.KeyDown,
                                                                           tb01.KeyDown, tb11.KeyDown, tb21.KeyDown, tb31.KeyDown, tb41.KeyDown,
                                                                           tb02.KeyDown, tb12.KeyDown, tb22.KeyDown, tb33.KeyDown, tb43.KeyDown,
                                                                           tb03.KeyDown, tb14.KeyDown, tb23.KeyDown, tb32.KeyDown, tb42.KeyDown,
                                                                           tb04.KeyDown, tb13.KeyDown, tb24.KeyDown, tb34.KeyDown, tb44.KeyDown,
                                                                           tbSigma.KeyDown, tbAmount.KeyDown,
                                                                           tbBias.KeyDown, tbFactor.KeyDown,
                                                                           tbModifyAlpha.KeyDown, tbModifyRed.KeyDown, tbModifyGreen.KeyDown, tbModifyBlue.KeyDown,
                                                                           tbTransparencyRed.KeyDown, tbTransparencyGreen.KeyDown, tbTransparencyBlue.KeyDown, tbTransparencyTolerance.KeyDown

        'restrict for the most part entry to only what it should be for a numeric field; it is not perfect but will help

        If (e.Modifiers = Keys.Shift) Or (e.Modifiers = Keys.Alt) Then GoTo NotAllowedNoBeep

        'allow copy and paste (this will let bad characters come in but they will be edited later)
        If (e.Modifiers = Keys.Control) Then
            If (e.KeyCode.ToString = "C") OrElse (e.KeyCode.ToString = "X") OrElse (e.KeyCode.ToString = "V") OrElse (e.KeyCode.ToString = "A") Then
                GoTo Allowed
            Else
                GoTo NotAllowedNoBeep
            End If
        End If

        If "0123456789 ".Contains(Chr(e.KeyValue)) Then GoTo Allowed

        Select Case e.KeyValue

            Case Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
                 Keys.Delete, Keys.Back, Keys.Right, Keys.Left, Keys.Home, Keys.End, Keys.Insert, Keys.NumLock, Keys.Scroll
                GoTo Allowed

        End Select

        If sender.name.startswith("tbTransparency") Then

            ' no additional characters allowed

        ElseIf sender.name.startswith("tbModify") Then


            Select Case e.KeyValue

                Case Keys.OemMinus, Keys.Subtract
                    GoTo Allowed

            End Select

        Else

            Select Case e.KeyValue

                Case Keys.OemMinus, Keys.Subtract, Keys.Decimal, Keys.OemPeriod
                    GoTo Allowed

            End Select

        End If

        ' fall thru to not allowed with anything else

NotAllowed:

        Beep()

NotAllowedNoBeep:

        e.SuppressKeyPress = True

        sender.ForeColor = Drawing.Color.Black

        Me.Focus()

        Exit Sub

Allowed:

        'If BeforeSelectProfileName = String.Empty Then
        tsSave.Enabled = True
        'End If

        sender.ForeColor = Drawing.Color.Black

        Me.Focus()

    End Sub

    Private Sub Textbox_TextChanged(sender As Object, e As EventArgs) Handles tb00.TextChanged, tb01.TextChanged, tb02.TextChanged, tb03.TextChanged,
                                                                                                tb11.TextChanged, tb12.TextChanged, tb13.TextChanged,
                                                                                                                  tb22.TextChanged, tb23.TextChanged
        If SymmetricUnderway OrElse LoadUnderway Then Exit Sub

        SymmetricUnderway = True
        Symmetric()
        SymmetricUnderway = False

    End Sub
    Private Sub SymmetryChagned(sender As Object, e As EventArgs) Handles cbSymmetric.CheckedChanged

        If SymmetricUnderway OrElse LoadUnderway Then Exit Sub

        tsSave.Enabled = True

        MasterScreenUpdate(False)
        Symmetric()

    End Sub


    Private Sub Symmetric()

        If cbSymmetric.Checked Then

            Dim HoldSymmetricUnderway As Boolean = SymmetricUnderway
            SymmetricUnderway = True

            tb60.Text = tb00.Text
            tb66.Text = tb00.Text
            tb06.Text = tb00.Text

            tb61.Text = tb01.Text
            tb65.Text = tb01.Text
            tb05.Text = tb01.Text
            tb10.Text = tb01.Text
            tb50.Text = tb01.Text
            tb16.Text = tb01.Text
            tb56.Text = tb01.Text

            tb62.Text = tb02.Text
            tb64.Text = tb02.Text
            tb04.Text = tb02.Text
            tb20.Text = tb02.Text
            tb40.Text = tb02.Text
            tb26.Text = tb02.Text
            tb46.Text = tb02.Text

            tb30.Text = tb03.Text
            tb63.Text = tb03.Text
            tb36.Text = tb03.Text

            tb50.Text = tb10.Text
            tb56.Text = tb10.Text
            tb16.Text = tb10.Text

            tb51.Text = tb11.Text
            tb55.Text = tb11.Text
            tb15.Text = tb11.Text

            tb52.Text = tb12.Text
            tb54.Text = tb12.Text
            tb14.Text = tb12.Text
            tb21.Text = tb12.Text
            tb41.Text = tb12.Text
            tb45.Text = tb12.Text
            tb25.Text = tb12.Text

            tb31.Text = tb13.Text
            tb53.Text = tb13.Text
            tb35.Text = tb13.Text

            tb42.Text = tb22.Text
            tb44.Text = tb22.Text
            tb24.Text = tb22.Text

            tb32.Text = tb23.Text
            tb43.Text = tb23.Text
            tb34.Text = tb23.Text

            SymmetricUnderway = HoldSymmetricUnderway

        End If

    End Sub

#End Region


#Region "Renaming"

    Private Sub tsSave_EnabledChanged(sender As Object, e As EventArgs) Handles tsSave.EnabledChanged

        If LoadUnderway Then Exit Sub

        If tsSave.Enabled Then
            BeforeSelectProfile = cbName_Text
        Else
            BeforeSelectProfile = String.Empty
            complicatedCancel = False
        End If

        UpdateButtonAvailability()

    End Sub

    Private Function GetANewName(ByVal OriginalName As String, ByVal ProposedName As String, ByVal NamingFunction As gNamingFunctions) As String

        Dim returnValue As String

        gNamingFunction = NamingFunction
        gRenameFunctionOriginalName = OriginalName
        gRenameFunctionProposedName = ProposedName

        Dim frmRenamePrompt As New frmRenamePrompt

        frmRenamePrompt.Location = New Point(Me.Location.X + SplitContainer1.Panel2.Location.X + gbProfile.Location.X, Me.Location.Y + SplitContainer1.Panel2.Location.Y + gbProfile.Location.Y)
        frmRenamePrompt.ShowDialog()

        If gRenameFunctionNewName = String.Empty Then
            returnValue = OriginalName
        Else
            returnValue = gRenameFunctionNewName
        End If

        frmRenamePrompt.Dispose()

        Return returnValue

    End Function

    Private Sub tbTransparencyBlue_TextChanged(sender As Object, e As EventArgs) Handles tbTransparencyRed.TextChanged, tbTransparencyGreen.TextChanged, tbTransparencyBlue.TextChanged

        If LoadUnderway Then Exit Sub

        Try

            btnFind.BackColor = Color.FromArgb(CInt(tbTransparencyRed.Text), CInt(tbTransparencyGreen.Text), CInt(tbTransparencyBlue.Text))

        Catch ex As Exception

            btnFind.BackColor = Color.FromArgb(255, 255, 255)

        End Try

    End Sub


    Private ColourPickerActive As Boolean = False
    Private ColourPickerCurrentColour As Color

    Private Sub btnFind_Click(sender As Object, e As EventArgs) Handles btnFind.Click

        If ColourPickerActive Then

            ColourPickerActive = False

            Dim tt As String = "To set the transparency colour," & vbCrLf & "click once here And then again" & vbCrLf & "on a colour on the image"
            If System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToUpper = "USA" Then tt = tt.Replace("colour", "color")
            ToolTip1.SetToolTip(btnFind, tt)

            Cursor = Cursors.Default



        Else

            Try
                ColourPickerCurrentColour = Color.FromArgb(CInt(tbTransparencyRed.Text), CInt(tbTransparencyGreen.Text), CInt(tbTransparencyBlue.Text))
            Catch ex As Exception
                ColourPickerCurrentColour = Color.FromArgb(255, 255, 255)
            End Try

            ColourPickerActive = True

            Dim tt As String = "Click here again to cancel setting the transparency colour"
            If System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToUpper = "USA" Then tt = tt.Replace("colour", "color")
            ToolTip1.SetToolTip(btnFind, tt)

            Cursor = Cursors.Cross

        End If

    End Sub

    Private Sub PictureBoxPrimary_Mousemove(sender As Object, e As MouseEventArgs) Handles PictureBoxPrimary.MouseMove, PictureBoxBottom.MouseMove, PictureBoxRight.MouseMove

        If ColourPickerActive Then

            Dim Handle As IntPtr = User32.GetWindowDC(IntPtr.Zero)
            Dim ColourRGBInt As Integer = GDI32.GetPixel(Handle, Cursor.Position.X, Cursor.Position.Y)
            Dim Result As Integer = User32.ReleaseDC(Handle, IntPtr.Zero)

            Dim r As Integer = ColourRGBInt And &HFF
            Dim g As Integer = (ColourRGBInt And &HFF00) >> 8
            Dim b As Integer = (ColourRGBInt And &HFF0000) >> 16

            tbTransparencyRed.Text = r
            tbTransparencyGreen.Text = g
            tbTransparencyBlue.Text = b

        End If

    End Sub

    Private Sub PictureBoxPrimary_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBoxPrimary.MouseClick, PictureBoxBottom.MouseClick, PictureBoxRight.MouseClick

        If ColourPickerActive Then

            Dim Handle As IntPtr = User32.GetWindowDC(IntPtr.Zero)
            Dim ColourRGBInt As Integer = GDI32.GetPixel(Handle, Cursor.Position.X, Cursor.Position.Y)
            Dim Result As Integer = User32.ReleaseDC(Handle, IntPtr.Zero)

            Dim r As Integer = ColourRGBInt And &HFF
            Dim g As Integer = (ColourRGBInt And &HFF00) >> 8
            Dim b As Integer = (ColourRGBInt And &HFF0000) >> 16

            Try

                If (r = ColourPickerCurrentColour.R) AndAlso (g = ColourPickerCurrentColour.G) And (b = ColourPickerCurrentColour.B) Then
                Else
                    tsSave.Enabled = True
                End If

            Catch ex As Exception

                tsSave.Enabled = True

            End Try

            tbTransparencyRed.Text = r
            tbTransparencyGreen.Text = g
            tbTransparencyBlue.Text = b

            ColourPickerActive = False

            Cursor = Cursors.Default

        End If

    End Sub

#End Region

#Region "Tree stuff"

    Private Sub LoadTreeView()

        ' load enhanced tree nodes

        Try

            SyncLock TreeView1

                TreeView1.BeginUpdate()

                TreeView1.Nodes.Clear()
                TreeView1.Nodes.Add("Effect Profiles")
                TreeView1.Nodes(0).Expand()
                TreeView1.Nodes(0).Tag = gTreeViewFolderTagDesignation

                Me.TreeView1.Scrollable = False

                TreeView1.Sorted = False

                Dim currentNode As TreeNode = TreeView1.Nodes(0)
                AddFoldersAndFilesToTreeview(currentNode, gXML_Path_Name, False)
                currentNode = Nothing

                TreeView1.Nodes(0).Expand()

                TreeView1.SelectedNode = Nothing
                EnsureVisableAndOptionalySelelectATreeNode(ConvertProfileNameToXMLFileName(gCurrentEffectsProfile.Name), TreeView1, True)

                TreeView1.HideSelection = False                          ' keeps selected item selected when treeview looses focus

                TreeView1.Scrollable = True

                TreeView1.DrawMode = TreeViewDrawMode.OwnerDrawText      ' allows TreeView1_DrawNode to set the colours of the selected node
                '                                                          also requires the TreeView1_DrawNode subroutine directly below

                TreeView1.EndUpdate()

            End SyncLock

        Catch ex As Exception

        End Try

        TreeView1_Focus()

    End Sub

    Private Sub TreeView1_DrawNode(sender As Object, e As DrawTreeNodeEventArgs) Handles TreeView1.DrawNode

        If gRefreshingTreeView Then Exit Sub

        SyncLock TreeView1
            Dim Dummy As Integer = 1
        End SyncLock

        Try

            'SyncLock TreeView1
            'ref: https://stackoverflow.com/questions/10034714/c-sharp-winforms-highlight-treenode-when-treeview-doesnt-have-focus

            Dim treeState As TreeNodeStates = e.State
                Dim treeFont As Font = If(e.Node.NodeFont, e.Node.TreeView.Font)
                Dim disabledFont As Font = New Font(treeFont, FontStyle.Italic)
                Dim foreColor As Color = e.Node.ForeColor
                'Dim strDeselectedColor As String = "#6B6E77", strSelectedColor As String = "#94C7FC"
                Dim strDeselectedColor As String = "#0078d7", strSelectedColor As String = "#94C7FC"   ' changed the selected colour; the deselected colour doesn't seem to matter
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

                        If e.Node.Checked OrElse (e.Node.Tag = gTreeViewFolderTagDesignation) Then
                            TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, foreColor, TextFormatFlags.GlyphOverhangPadding)
                        Else
                            TextRenderer.DrawText(e.Graphics, e.Node.Text, disabledFont, e.Bounds, foreColor, TextFormatFlags.GlyphOverhangPadding)
                        End If

                    Else

                        foreColor = SystemColors.HighlightText
                        e.Graphics.FillRectangle(deselectedTreeBrush, e.Bounds)
                        ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, foreColor, SystemColors.Highlight)

                        If e.Node.Checked OrElse (e.Node.Tag = gTreeViewFolderTagDesignation) Then
                            TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, foreColor, TextFormatFlags.GlyphOverhangPadding)
                        Else
                            TextRenderer.DrawText(e.Graphics, e.Node.Text, disabledFont, e.Bounds, foreColor, TextFormatFlags.GlyphOverhangPadding)
                        End If

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

                            If e.Node.Checked Then
                                TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, Color.MediumSlateBlue, TextFormatFlags.GlyphOverhangPadding)
                            Else
                                TextRenderer.DrawText(e.Graphics, e.Node.Text, disabledFont, e.Bounds, Color.MediumSlateBlue, TextFormatFlags.GlyphOverhangPadding)
                            End If

                        End If

                    End If

                End If

            'End SyncLock

        Catch ex As Exception

        End Try

    End Sub

    Private Sub TreeView1_Focus()

        SplitContainer1.Focus()
        SplitContainer1.ActiveControl = TreeView1

    End Sub

#End Region

#Region "Help"

    Private ShowingHelp As Boolean = False
    Private Sub tsRename_Click(sender As Object, e As EventArgs) Handles tsRename.Click

        Try

            If tsSave.Enabled Then

                Dim response As DialogResult = MessageBox.Show("There are currently unsaved changes within the '" & gCurrentEffectsProfile.Name & "' profile." & vbCrLf & vbCrLf &
                                    "Please click:" & vbCrLf & vbCrLf & "'Yes' to save the changes and then rename," & vbCrLf & vbCrLf & "'No' to discard the changes and then rename , or" & vbCrLf & vbCrLf & "'Cancel' to keep everything as it is.",
                                   gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)


                Select Case response

                    Case Is = DialogResult.Yes

                        If SaveScreenToCurrentRecord() Then

                            tsSave.Enabled = False

                        Else

                            MessageBox.Show("Could not rename this profile as there were errors in it." & vbCrLf & vbCrLf &
                                            "Please correct the errors before renaming this profile.",
                                   gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                            Exit Try

                        End If

                    Case Is = DialogResult.No

                        LoadEffectsProfile(gCurrentEffectsProfile.Name)
                        LoadScreenFromCurrentEffectsProfile()
                        tsSave.Enabled = False
                        Application.DoEvents()
                        Exit Try

                    Case Is = DialogResult.Cancel

                        cbName_Text = gCurrentEffectsProfile.Name
                        Exit Try

                End Select

            End If


StartRenaming:

            gRenameFunctionOriginalName = gCurrentEffectsProfile.Name

            Dim NewName As String = GetANewName(gRenameFunctionOriginalName, gRenameFunctionOriginalName, gNamingFunction.rename)

            If NewName = gRenameFunctionOriginalName Then
                'name did not change, i.e. the user canceled
                TreeView1_Focus()
                Exit Try

            Else

                ' add with new name
                cbName_Text = NewName

                gCurrentEffectsProfile.Name = NewName
                SaveScreenToCurrentRecord(False, False)

                For x As Integer = 1 To gCurrentNumberOfEffectsBoxesInUse
                    If gCurrentNamesOfEffectsInUse(x) = ConvertProfileNameToXMLFileName(gRenameFunctionOriginalName) Then
                        gCurrentNamesOfEffectsInUse(x) = ConvertProfileNameToXMLFileName(NewName)
                    End If
                Next

                ' remove old name
                Dim FileToRemove As String = gXML_Path_Name & "\" & gRenameFunctionOriginalName & gMyExtention

                If IO.File.Exists(FileToRemove) Then
                    gignoreFileWatcherEventUntilThisTime = Now.AddMilliseconds(gFiveSeconds)
                    System.IO.File.Delete(FileToRemove)
                End If

                LoadUnderway = True
                RefreshAllProfileNames()
                LoadEffectsProfile(NewName)
                LoadScreenFromCurrentEffectsProfile()
                LoadUnderway = False

                TreeView1.Refresh()

                EnsureVisableAndOptionalySelelectATreeNode(ConvertProfileNameToXMLFileName(NewName), TreeView1, True)

                tsSave.Enabled = False

            End If

        Catch ex As Exception

        End Try

        LoadUnderway = False

        Dim HoldTsSave As Boolean = tsSave.Enabled

        MasterScreenUpdate(True)

        TreeView1_Focus()

        PriorProfileBeingWorkedOn = cbName_Text

        RefreshTheScreen()

        TreeView1_Focus()

        tsSave.Enabled = HoldTsSave

    End Sub

    Private Sub tsHelp_Click(sender As Object, e As EventArgs) Handles tsHelp.Click

        If My.Computer.Keyboard.AltKeyDown Then

            System.Diagnostics.Process.Start(gWebpageHelp)

        Else

            If ShowingHelp Then Exit Sub

            ShowingHelp = True

            Dim HelpText1 As String = "This window lets you set custom Effects Profiles (profiles)." & vbCrLf & vbCrLf &
             "The left portion of this window contains buttons to manage your profiles and update the images you see.  The centre portion of the screen lets you define your profiles. The right part of the screen lets you see the results of your profiles." & vbCrLf & vbCrLf &
             "The left portion of the Effects Design window:" & vbCrLf &
             "The 'Add' button adds a new profile." & vbCrLf & vbCrLf &
             "The 'Copy' button copies the current profile using the name of the current profile with ' - copy' added to the end of it." & vbCrLf & vbCrLf &
             "The 'Save' button will be enabled if you are adding a new Profile, copying one, or have made changes to an existing profile.  Clicking the 'Save' button saves the changes to the profile you have made." & vbCrLf & vbCrLf &
             "The 'Rename' button allows you to rename the current profile." & vbCrLf & vbCrLf &
             "The 'Remove' button allows you to delete an existing profile." & vbCrLf & vbCrLf &
             "The 'Locate' button allows find where on your computer your profiles are stored.  It is a good idea to periodically back these up. You may also share profiles you create with others.  This can be done by you sending them your profile files, and them double clicking on those files to have them Computeally added to their profile files folder." & vbCrLf & vbCrLf &
             "The 'Test' button lets you see the results of the profile setting on your screen without having to save them.  If you are happy with your testing results, click 'Save' to save them, or click 'Cancel'." & vbCrLf & vbCrLf &
             "The 'Cancel' button cancels that you have been making and reverts to the original profile." & vbCrLf & vbCrLf &
             "The 'Viewer' button behaves the same way as on the main window.  However, rather than seeing the original image and the image you are changing, you will see the changed image at the time the Effects Profile window was open (without any Effects applied) and the impacts of the profile you see in the centre of the screen applied to it. This allows you to first apply other modifications, such as changing its zoom, rotation, brightness, contrast, etc., to your image and then use this window to select the best profile for use with your image." & vbCrLf & vbCrLf &
             "(more ...)"

            Dim HelpText2 As String = "The centre portion of the Effects Design window:" & vbCrLf &
             "Displays the name and values associated with a profile. You can quickly change the profile you are working with by selecting it from the tree-view box.  Also, you can navigate it with the 'Home', 'Down Arrow', 'Up Arrow', and 'End' keys." & vbCrLf & vbCrLf &
             "Hovering your mouse pointer over any of the text box fields on this window will pop-up a tool tip describing the values which can be entered in it." & vbCrLf & vbCrLf &
             "Click either 'Blur' or 'Effect' to choose the filter (matrix) type.  Additional context, including the relationship between these two types, is provided in the online help.  If you will be using the 'Blur' type, reviewing the online help is recommended." & vbCrLf & vbCrLf &
             "With 'Blur' selected the 'Compute' option can also be selected to have the cells automatically calculated.  To manually enter the cells uncheck the 'Compute' option." & vbCrLf & vbCrLf &
             "If you have a symmetric matrix which you are entering manually then checking the 'Symmetric' option reduces the number of numbers you will need to enter." & vbCrLf & vbCrLf &
             "The 'Sigma' value is used as the basis for automatically computing cells.  The 'Amount' value is used in converting a 'Blur' filter to an 'Effect' filter." & vbCrLf & vbCrLf &
             "The 'Bias' value adds to each cell in the matrix when producing the final effect. The 'Factor' value multiplies each cell in the matrix when production the final effect." & vbCrLf & vbCrLf &
             "When both the bias and factor are used, the bias is first added to a matrix cell, and then that sum is multiplied by the factor.  So, in short, the actual filter used = ( what is shown in the matrix + the bias ) x the factor." & vbCrLf & vbCrLf &
             "The 'Modify Colours' section allows you to change the opacity (the amount you can see through an image) and or its hue (in other words, its red, green, blue (rgb) values)." & vbCrLf & vbCrLf &
             "The 'Transparency Colour' section allows to select a colour on the image to become transparent.  You can do this by entering its rgb values or by clicking once on the coloured box and once again on any colour in the image. " &
             "The 'Tolerance' value lets you includes other hues, within +/-  tolerance value, to also be made transparent." & vbCrLf & vbCrLf &
             "The 'Notes' section is a free form area where you can record your own notes/comments related to the profile." & vbCrLf & vbCrLf &
             "The right portion of the Effects Design window:" & vbCrLf &
             "Displays images as already described. The images you see can be panned, but not further zoomed or rotated in this window." & vbCrLf & vbCrLf &
             "When the mouse pointer is over an image holding down the middle button shows the image without any filtering (only) until you lift up on the middle mouse button again." & vbCrLf & vbCrLf &
             "The line between the button images and text can be dragged left or right to hide/unhide the words associated with the buttons." & vbCrLf & vbCrLf &
             "To view the online help, hold the 'Alt' key down while clicking on 'Quick Help'."

            If gCentreLiteral = "Center" Then
                HelpText1 = HelpText1.Replace("centre", "center").Replace("Colour", "Color").Replace("colour", "color")
                HelpText2 = HelpText2.Replace("centre", "center").Replace("Colour", "Color").Replace("colour", "color")

            End If

            MessageBox.Show(HelpText1, gThisProgramName & gVersionInUse & " - Quick Help - 1 of 2", MessageBoxButtons.OK, MessageBoxIcon.Information)

            MessageBox.Show(HelpText2, gThisProgramName & gVersionInUse & " - Quick Help - 2 of 2", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ShowingHelp = False

        End If

    End Sub

#End Region

End Class