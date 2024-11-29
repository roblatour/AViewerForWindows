Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports ImageMagick
Imports Microsoft.VisualBasic
Public Class frmViewer
    Inherits Form
    Implements IMessageFilter
    Private Enum ChangeTypeEnum
        Absolute = 0
        Relative = 1
    End Enum

    Private ChangeType As ChangeTypeEnum

    Private MyDrawingPen As Pen
    Private SelectedTool As Color
    Private UnSelectedTool As Color

    Private initialLoadUnderway As Boolean = True
    Private reloadUnderway As Boolean = False

    Dim EnhancedTreeViewBoxes(gMaxEffectsSupported) As ucEnhancedTreeView

    Friend Structure PictureBoxTransformationsStructure
        Dim ChangeBrightness As Boolean
        Dim ChangeContrast As Boolean
        Dim ChangeGamma As Boolean
        Dim ChangeGrayScale As Boolean
        Dim ChangeInvert As Boolean
        Dim ChangeMirroredHorizontal As Boolean
        Dim ChangeMirroredVertical As Boolean
        Dim ChangeSaturation As Boolean
        Dim ChangeEffects As Boolean
        Dim ChangeRotation As Boolean
        Dim ChangeZoom As Boolean
        Dim UpdatePicturebox As Boolean

    End Structure

    Friend DoAllPictureBoxTransformations As PictureBoxTransformationsStructure
    Friend DoNoPictureBoxTransformations As PictureBoxTransformationsStructure

    Private SliderControl As ucSlider

    Private ImageSizeAt100Percent As Size

#Region "Setup"
    Public Sub New()

        InitializeComponent()
        Application.AddMessageFilter(Me)

    End Sub
    Private Sub FinalForm_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load

        Try

            initialLoadUnderway = True

            Me.Location = gCommonWindowLocation

            If gRunningOnA4KMonitor Then
                SplitContainer1_MaxWidth = 117
            Else
                SplitContainer1_MaxWidth = 86
            End If

            Dim activeScreenDimensions = Screen.FromControl(Me).Bounds

            Const WidthAdjustementForBoarders As Integer = 17
            Const HeightAdjustementForBoarders As Integer = 66

            Dim width As Integer = System.Math.Min(activeScreenDimensions.Width - 32, gOriginalImage.Size.Width + ToolStripView.Size.Width + WidthAdjustementForBoarders)
            Dim height As Integer = System.Math.Min(activeScreenDimensions.Height - 80, gOriginalImage.Size.Height + HeightAdjustementForBoarders)

            width = System.Math.Max(width, 860) ' ensures all of the status bar is visiable on small image
            height = System.Math.Max(height, 600) ' ensures all controls are visiable on small image

            If ((Me.Location.X + width) > activeScreenDimensions.Width) OrElse ((Me.Location.Y + height) > activeScreenDimensions.Height) Then

                Dim x As Integer = (activeScreenDimensions.Width - width)
                If x < 0 Then x = 0
                Dim y As Integer = (activeScreenDimensions.Height - height - WidthAdjustementForBoarders - HeightAdjustementForBoarders)
                If y < 0 Then y = 0

                Me.Location = New Point(x, y)

            End If

            Dim UnusedColour As Color = FindAnUnusedColour(gOriginalImage)

            SetGlobalTransparencyColour(UnusedColour)

            SetPictureBoxBackGrounds()

            gOriginalImageAsFirstLoaded = New Bitmap(gOriginalImage)

            ImageSizeAt100Percent = New Size(gOriginalImage.Size)

            ResizeOriginalForRotation()
            gCorrectedImage = New Bitmap(gOriginalImage)

            movingPoint = gTweakedStartingLocationDueToResizeOriginalForRotation

            Me.MinimumSize = New Drawing.Size(700, 350)

            Me.Size = New System.Drawing.Size(width, height)

            Me.Text = gThisProgramName & gVersionInUse & " - " & Path.GetFileName(gFilePathNameAndExtention)

            Me.PictureBoxPrimary.AutoScrollOffset = New Drawing.Point(10, 10)

            Dim DrawingColor As Color = Color.Orange
            Dim LineThickness As Integer = 3

            MyDrawingPen = New Pen(DrawingColor, LineThickness)
            MyDrawingPen.Alignment = PenAlignment.Inset
            MyDrawingPen.DashStyle = DashStyle.Dash
            SelectedTool = DrawingColor
            UnSelectedTool = Color.Black

            With DoAllPictureBoxTransformations
                .ChangeBrightness = True
                .ChangeContrast = True
                .ChangeGamma = True
                .ChangeGrayScale = True
                .ChangeInvert = True
                .ChangeBrightness = True
                .ChangeMirroredHorizontal = True
                .ChangeMirroredVertical = True
                .ChangeSaturation = True
                .ChangeEffects = True
                .ChangeRotation = True
                .ChangeZoom = True
                .UpdatePicturebox = True
            End With

            SetAllDefaults()

            With DoNoPictureBoxTransformations
                .ChangeBrightness = False
                .ChangeContrast = False
                .ChangeGamma = False
                .ChangeGrayScale = False
                .ChangeInvert = False
                .ChangeBrightness = False
                .ChangeMirroredHorizontal = False
                .ChangeMirroredVertical = False
                .ChangeSaturation = False
                .ChangeEffects = False
                .ChangeRotation = False
                .ChangeZoom = False
                .UpdatePicturebox = True
            End With

            UpdatePinnedToTop()

            LoadEffectsMatrixTableFromString()

            PictureBoxPrimary.BackColor = gMyTransparencyColour

            If My.Settings.ViewChoice = ViewingOption.OriginalOnly Then
                'Original only is reserved for when the user holds down the middle mouse button only, it shoudl not be an opening value
                My.Settings.ViewChoice = ViewingOption.PrimaryOnly
                My.Settings.Save()
            End If

            gViewChoice = My.Settings.ViewChoice

            SetLookOfWindows()
            Application.DoEvents()

            If (gViewChoice = ViewingOption.PrimaryandBottom) OrElse (gViewChoice = ViewingOption.PrimaryandRight) Then
                SetLookOfWindows()
                Application.DoEvents()
            End If

            SetupForMagnifier()
            tsMagnify.Visible = gMagnifierFound

            LoadNeutralEffectsProfile(False)

            Timer1.Interval = 500 ' timer to redraw zoom after mousewheel stops spinning

            GarbageCollect()

            initialLoadUnderway = False

            PictureBoxPrimary.Invalidate()

            gCurrentNumberOfEffectsBoxesInUse = 0

            FreshLoadOfEnhancedTreeViewBoxes()

            UpdateLookOfSplitContainer(My.Settings.ShowingSplitContainerText, True)

            AddHandler AnAV4WNotificationEvent, AddressOf ReactToAnAV4WNotificationEvent

            FitLogic(Nothing, True, False, False)

        Catch ex As Exception

            Beep()

            If ex.Message = "Out of memory." Then

                MessageBox.Show("Sorry." & vbCrLf & vbCrLf & "This image is too big for " & gThisProgramName & " to work with." & vbCrLf & vbCrLf, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            Else

                MessageBox.Show("Sorry." & vbCrLf & vbCrLf & gThisProgramName & " cannot work with this image." & vbCrLf & vbCrLf, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            End If

            GarbageCollect()

            Me.Close()

        End Try

    End Sub
    Private Sub ReactToAnAV4WNotificationEvent()

        If Me.Visible Then

            If gCurrentNumberOfEffectsBoxesInUse > 0 Then
                Dim h As String() = gCurrentNamesOfEffectsInUse
                RebuildViewerScreen()
                PictureBoxPrimary.Invalidate()
            End If

        End If

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

    'Private Sub LoadSliderControl()

    '    SliderControl = New ucSlider
    '    SliderControl.Visible = False

    '    SplitContainer1.Panel2.Controls.Add(SliderControl)

    '    AddHandler SliderControl.TextBox1.TextChanged, AddressOf SliderControl_TextBox1_TextChanged

    'End Sub
    'Private Sub SliderControl_TextBox1_TextChanged(sender As Object, e As EventArgs)

    'End Sub


    Private Sub FreshLoadOfEnhancedTreeViewBoxes()

        For x = 1 To gMaxEffectsSupported

            EnhancedTreeViewBoxes(x) = New ucEnhancedTreeView
            EnhancedTreeViewBoxes(x).Name = "etv" & x
            EnhancedTreeViewBoxes(x).lblHeader.Text = "Effect " & x
            EnhancedTreeViewBoxes(x).Tag = x
            EnhancedTreeViewBoxes(x).btnEffectAdd.Tag = x
            EnhancedTreeViewBoxes(x).btnEffectRemove.Tag = x
            EnhancedTreeViewBoxes(x).TreeView1.Tag = x
            EnhancedTreeViewBoxes(x).Visible = False
            EnhancedTreeViewBoxes(x).TreeView1.Scrollable = True
            EnhancedTreeViewBoxes(x).btnEffectAdd.Visible = False
            EnhancedTreeViewBoxes(x).TreeView1.TabStop = False           ' prevents the root node from being Computeally selected

            SplitContainer1.Panel2.Controls.Add(EnhancedTreeViewBoxes(x))

            gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse

            AddHandler EnhancedTreeViewBoxes(x).btnEffectAdd.Click, AddressOf EnhancedTreeViewBoxes_btnEffectAdd_Click
            AddHandler EnhancedTreeViewBoxes(x).btnEffectRemove.Click, AddressOf EnhancedTreeViewBoxes_btnEffectRemove_Click
            AddHandler EnhancedTreeViewBoxes(x).btnDone.Click, AddressOf EnhancedTreeViewBoxes_btnDone_Click

            AddHandler EnhancedTreeViewBoxes(x).TreeView1.BeforeSelect, AddressOf EnhancedTreeViewBoxes_TreeView1_BeforeSelect
            AddHandler EnhancedTreeViewBoxes(x).TreeView1.AfterSelect, AddressOf EnhancedTreeViewBoxes_TreeView1_AfterSelect
            AddHandler EnhancedTreeViewBoxes(x).TreeView1.DoubleClick, AddressOf EnhancedTreeViewBoxes_TreeView1_DoubleClick

            ' only the first enchancetreebox can be moved 
            AddHandler EnhancedTreeViewBoxes(x).MouseDown, AddressOf EnhancedTreeViewBoxes_MouseDown
            AddHandler EnhancedTreeViewBoxes(x).MouseUp, AddressOf EnhancedTreeViewBoxes_MouseUp
            AddHandler EnhancedTreeViewBoxes(x).MouseMove, AddressOf EnhancedTreeViewBoxes_MouseMove

            enhancedControlOffset(x) = New Point(0, 0)

        Next

        gCurrentNumberOfEffectsBoxesInUse = 0

    End Sub


    Private W10_32_MagnifierProgram As String = "C:\Windows\System32\Magnify.exe"
    Private W10_64_MagnifierProgram As String = "c:\Windows\Sysnative\Magnfiy.exe"
    Private Sub SetupForMagnifier()

        If File.Exists(W10_64_MagnifierProgram) Then
            gMagnifierProgram = W10_64_MagnifierProgram
        ElseIf File.Exists(W10_32_MagnifierProgram) Then
            gMagnifierProgram = W10_32_MagnifierProgram
        End If

        gMagnifierFound = (gMagnifierProgram.Length > 0)

    End Sub

#End Region

#Region "Level tool"

    Private LevelingToolClicksAreNeeded As Integer = 0
    Private CropToolClicksAreNeeded As Integer = 0

    Private LevelingToolPoints(2) As Point
    Private CropToolPoints(2) As Point

    Private Sub tsLevel_MouseDown(sender As Object, e As MouseEventArgs) Handles tsLevel.MouseDown

        If LevelingToolClicksAreNeeded = 0 Then
            MouseIsInTheLevelingToolBox = True
            LevelingToolClicksAreNeeded = 1
            Cursor = LevelingToolCursor
            tsLevel.ForeColor = SelectedTool
        Else
            LevelingToolClicksAreNeeded = 0
            Cursor = Cursors.Default
            tsLevel.ForeColor = UnSelectedTool
        End If

    End Sub
    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBoxPrimary.MouseDown, PictureBoxPrimary.MouseUp

        Select Case LevelingToolClicksAreNeeded

            Case Is = 1 ' first click

                LevelingToolPoints(LevelingToolClicksAreNeeded) = e.Location
                LevelingToolClicksAreNeeded = 2

                Exit Select

            Case Is = 2 ' second click

                LevelingToolPoints(LevelingToolClicksAreNeeded) = e.Location

                LevelingToolClicksAreNeeded = 0

                Dim OffsetAngle As Single = 0

                If LevelingToolPoints(1) = LevelingToolPoints(2) Then

                Else

                    Dim LeftMostPoint As New Point
                    Dim RightMostPoint As New Point

                    If LevelingToolPoints(1).X < LevelingToolPoints(2).X Then
                        LeftMostPoint = LevelingToolPoints(1)
                        RightMostPoint = LevelingToolPoints(2)
                    Else
                        LeftMostPoint = LevelingToolPoints(2)
                        RightMostPoint = LevelingToolPoints(1)
                    End If

                    Dim xDiff As Double = RightMostPoint.X - LeftMostPoint.X
                    Dim yDiff As Double = RightMostPoint.Y - LeftMostPoint.Y

                    OffsetAngle = CSng(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI) * -1

                End If

                ChangeRotation(OffsetAngle, ChangeTypeEnum.Relative)

                If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

                tsLevel.ForeColor = UnSelectedTool

                Cursor = Cursors.Default

                Exit Select

            Case Else

                Exit Select

        End Select

        Select Case CropToolClicksAreNeeded

            'crop moving point 1 = point of mouse down on image considering pan
            'crop moving point 2 = point of mouse up on image considering pan for pan


            Case Is = 1 'first click

                CropToolPoints(CropToolClicksAreNeeded) = e.Location
                CropToolClicksAreNeeded = 2

                Exit Select

            Case Is = 2 'second click

                Try

                    CropToolPoints(CropToolClicksAreNeeded) = e.Location
                    CropToolClicksAreNeeded = 0

                    If CropToolPoints(1) = CropToolPoints(2) Then

                    Else

                        Dim LeftMostPoint As New Point
                        Dim RightMostPoint As New Point

                        Dim TopMostPoint As New Point
                        Dim BottomMostPoint As New Point

                        Dim ImageWasMovedWithinThePicturebox As Boolean = (CropToolPoints(1) <> CropMovingPoint(1))

                        If ImageWasMovedWithinThePicturebox Then

                            CropToolPoints(2).X = CropToolPoints(2).X - (CropToolPoints(1).X - CropMovingPoint(1).X)
                            CropToolPoints(2).Y = CropToolPoints(2).Y - (CropToolPoints(1).Y - CropMovingPoint(1).Y)

                            CropToolPoints(1).X = CropMovingPoint(1).X
                            CropToolPoints(1).Y = CropMovingPoint(1).Y

                        End If

                        If CropToolPoints(1).X < CropToolPoints(2).X Then
                            LeftMostPoint = CropToolPoints(1)
                            RightMostPoint = CropToolPoints(2)
                        Else
                            LeftMostPoint = CropToolPoints(2)
                            RightMostPoint = CropToolPoints(1)
                        End If

                        If CropToolPoints(1).Y < CropToolPoints(2).Y Then
                            TopMostPoint = CropToolPoints(1)
                            BottomMostPoint = CropToolPoints(2)
                        Else
                            TopMostPoint = CropToolPoints(2)
                            BottomMostPoint = CropToolPoints(1)
                        End If

                        Dim width As Integer = RightMostPoint.X - LeftMostPoint.X
                        Dim height As Integer = BottomMostPoint.Y - TopMostPoint.Y

                        width = Math.Min(width, gCorrectedImage.Width - LeftMostPoint.X)
                        height = Math.Min(height, gCorrectedImage.Height - TopMostPoint.Y) ' xx

                        Dim CropRect As New Rectangle(LeftMostPoint.X, TopMostPoint.Y, width, height)

                        ' MakeSureCropPointsAreFullyInsideImage(CropRect, gOriginalImage)  ' testing here
                        MakeSureCropPointsAreFullyInsideImage(CropRect, gCorrectedImage)

                        Dim FullBitmap As Bitmap = New Bitmap(gCorrectedImage)

                        Dim format As PixelFormat = FullBitmap.PixelFormat

                        Dim CropBitMap As Bitmap = FullBitmap.Clone(CropRect, format)

                        FullBitmap.Dispose()
                        GarbageCollect()

                        gOriginalImage = New Bitmap(CropBitMap)
                        ResizeOriginalForRotation()

                        gCorrectedImage = New Bitmap(gOriginalImage)

                        ImageSizeAt100Percent = gCorrectedImage.Size

                        gCurrentZoom = gZoomDefault
                        gCurrentRotation = gRotationDefault

                        movingPoint = New Point(0, 0)
                        If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

                        CropBitMap.Dispose()

                        GarbageCollect()

                    End If

                    tsCrop.ForeColor = UnSelectedTool

                    Cursor = Cursors.Default

                    Exit Select

                Catch ex As Exception

                    Beep()

                End Try

            Case Else

                Exit Select

        End Select

    End Sub

    Private Sub MakeSureCropPointsAreFullyInsideImage(ByRef CropRect As Rectangle, ByRef Img As Image)

        If CropRect.X < 0 Then CropRect.X = 0
        If CropRect.X > Img.Width Then CropRect.X = Img.Width

        If CropRect.Y < 0 Then CropRect.Y = 0
        If CropRect.Y > Img.Width Then CropRect.X = Img.Width

        If (CropRect.X + CropRect.Width) > Img.Width Then CropRect.Width = Img.Width - CropRect.X
        If (CropRect.Y + CropRect.Height) > Img.Height Then CropRect.Height = Img.Height - CropRect.Y

    End Sub

    Private MouseIsInThePictureBox As Boolean = False
    Private MouseIsInTheLevelingToolBox As Boolean = False

    Private LevelingToolCursor As Cursor = Cursors.Cross

    Private Sub PictureBox1_Enter(sender As Object, e As EventArgs) Handles PictureBoxPrimary.MouseEnter

        MouseIsInThePictureBox = True

        If LevelingToolClicksAreNeeded > 0 OrElse CropToolClicksAreNeeded > 0 Then
            Cursor = LevelingToolCursor
        End If

    End Sub
    Private Sub PictureBox1_MouseLeave2(sender As Object, e As EventArgs) Handles PictureBoxPrimary.MouseLeave

        MouseIsInThePictureBox = False
        If LevelingToolClicksAreNeeded > 0 OrElse CropToolClicksAreNeeded > 0 Then
            Cursor = Cursors.Default
        End If


    End Sub

    Private Sub ToolStripMenuLevelingTool_Enter(sender As Object, e As EventArgs) Handles tsLevel.MouseEnter

        MouseIsInTheLevelingToolBox = True

        If LevelingToolClicksAreNeeded > 0 Then
            Cursor = LevelingToolCursor
        End If

    End Sub

    Private Sub ToolStripMenuCropTool_Enter(sender As Object, e As EventArgs) Handles tsCrop.MouseEnter

        MouseIsInTheCropToolBox = True

        If CropToolClicksAreNeeded > 0 Then
            Cursor = LevelingToolCursor
        End If

    End Sub
    Private Sub ToolStripMenuLevelingTool_MouseLeave(sender As Object, e As EventArgs)

        MouseIsInTheLevelingToolBox = False

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        Application.RemoveMessageFilter(Me)
    End Sub


    Private Const WM_LBUTTONUP As Integer = &H202
    Private Const WM_LBUTTONDOWN As Integer = &H201
    'Private Const WM_RBUTTONDOWN As Integer = &H204
    'Private Const WM_MBUTTONDOWN As Integer = &H207
    'Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    'Private Const WM_NCRBUTTONDOWN As Integer = &HA4
    'Private Const WM_NCMBUTTONDOWN As Integer = &HA7
    <System.Diagnostics.DebuggerStepThrough()>
    Friend Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean Implements System.Windows.Forms.IMessageFilter.PreFilterMessage

        If (m.Msg = WM_LBUTTONUP) OrElse (m.Msg = WM_LBUTTONDOWN) Then

            Dim ctl As Control = Control.FromHandle(m.HWnd)

            If ctl IsNot Nothing Then

                If LevelingToolClicksAreNeeded > 1 Then

                    If (MouseIsInThePictureBox) OrElse (MouseIsInTheLevelingToolBox) Then
                    Else
                        LevelingToolClicksAreNeeded = 0
                        Beep()
                        Cursor = Cursors.Default
                    End If

                End If

            End If

            Return False

        End If

    End Function

#End Region

#Region "Change Rotation"
    Private Sub tsRotate_MouseDown(sender As Object, e As MouseEventArgs) Handles tsRotate.MouseDown

        tsRotate_MouseDown_MyHandler(e, False, False, False)

        'SliderControl.initialValue = gCurrentRotation
        'SliderControl.resetValue = 0
        'SliderControl.minimumValue = 0
        'SliderControl.maximumValue = 360
        'SliderControl.Location = New Point(0, Me.SplitContainer1.Panel2.Height / 2)
        'SliderControl.Width = Me.SplitContainer1.Panel2.Width
        'SliderControl.SliderRefresh()
        'SliderControl.BringToFront()

    End Sub

    Private Sub tsRotate_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        SeCursor(Me, CursorState.Wait)

        MouseIsDown = True

        Dim DelayTime As Integer = 350 ' first delay is longer - allowing for a single click only

        ' rather than rebuild the image on an ongoing basis while rotating it (to avoid the blurr effect)
        ' make a copy of the fully rendered image (all effects applied) and rotate it instead
        ' Genius!

        gDummyForRotationAndZoomingOnlyImage = New Bitmap(gCorrectedImage)

        gDummyForRotationAndZoomingOnlyImage = RotateImage(gDummyForRotationAndZoomingOnlyImage, -gCurrentRotation)

        While MouseIsDown

            If My.Computer.Keyboard.AltKeyDown Then

                Dim remainder As Integer
                Dim quotiant As Integer

                quotiant = Math.DivRem(CInt(gCurrentRotation), 45, remainder)

                gCurrentRotation = quotiant * 45

                If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

                    gCurrentRotation = ((quotiant - 1) * 45) Mod 360
                    ChangeDummyRotation(gCurrentRotation, ChangeTypeEnum.Absolute)
                    DelayTime = 333

                ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

                    gCurrentRotation = gRotationDefault
                    Exit While

                ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then

                    gCurrentRotation = ((quotiant + 1) * 45) Mod 360
                    ChangeDummyRotation(gCurrentRotation, ChangeTypeEnum.Absolute)
                    DelayTime = 333

                End If

            Else

                Dim Multiplier As Single
                Multiplier = 1
                Multiplier = IIf(My.Computer.Keyboard.ShiftKeyDown, 2, 1)
                Multiplier *= IIf(My.Computer.Keyboard.CtrlKeyDown, 5, 1)

                If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then
                    ChangeDummyRotation(-0.1 * Multiplier, ChangeTypeEnum.Relative)

                ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then
                    gCurrentRotation = gRotationDefault
                    Exit While

                ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then
                    ChangeDummyRotation(0.1 * Multiplier, ChangeTypeEnum.Relative)

                End If

            End If

            Application.DoEvents()
            System.Threading.Thread.Sleep(DelayTime)
            DelayTime = 0

            GarbageCollect()

            Application.DoEvents()

        End While

        gDummyForRotationAndZoomingOnlyImage.Dispose()

        ' Resets the real image now that the rotations are done 
        ' cleans up the image after a long series of changes caused by the mouse being held down

        UpdatePictureBox(DoAllPictureBoxTransformations)

        SeCursor(Me, CursorState.Normal)

    End Sub

    Private Sub ChangeDummyRotation(ByVal ChangeAmount As Single, ChangeType As ChangeTypeEnum)

        Select Case ChangeType

            Case Is = ChangeTypeEnum.Absolute
                gCurrentRotation = ChangeAmount

            Case Is = ChangeTypeEnum.Relative
                gCurrentRotation += ChangeAmount

        End Select

        gCurrentRotation = CSng(CInt(gCurrentRotation * 100) / 100) Mod 360.0F

        gCorrectedImage = RotateImage(gDummyForRotationAndZoomingOnlyImage, gCurrentRotation)

        'force the image to be redrawn in the picturebox
        PictureBoxPrimary.Invalidate()
        Application.DoEvents()

    End Sub

    Friend Function RotateImage(ByRef bmp As Bitmap, ByVal angle As Single) As Bitmap

        ' https://stackoverflow.com/questions/12024406/how-can-i-rotate-an-image-by-any-degree

        Dim returnValue As Bitmap

        If angle = 0 Then

            returnValue = New Bitmap(bmp)

        Else

            angle = angle Mod 360.0F

            returnValue = New Bitmap(bmp.Width, bmp.Height)

            returnValue.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution)

            Using g As Graphics = Graphics.FromImage(returnValue)

                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.SmoothingMode = SmoothingMode.HighQuality

                g.TranslateTransform(CSng(bmp.Width / 2), CSng(bmp.Height / 2))
                g.RotateTransform(angle)
                g.TranslateTransform(CSng(-bmp.Width / 2), CSng(-bmp.Height / 2))

                g.DrawImage(bmp, New Point(0, 0))

            End Using

        End If

        Return returnValue

    End Function

    Private Sub ChangeRotation(ByVal ChangeAmount As Single, ChangeType As ChangeTypeEnum)

        Select Case ChangeType

            Case Is = ChangeTypeEnum.Absolute
                gCurrentRotation = ChangeAmount

            Case Is = ChangeTypeEnum.Relative
                gCurrentRotation += ChangeAmount

        End Select

        gCurrentRotation = CSng(CInt(gCurrentRotation * 100) / 100) Mod 360.0F

        gCorrectedImage = RotateImage(gCorrectedImage, ChangeAmount)

    End Sub

#End Region

#Region "Fit"

    Private Sub tsFit_MouseDown(sender As Object, e As MouseEventArgs) Handles tsFit.MouseDown

        tsFit_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsFit_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        SeCursor(Me, CursorState.Wait)

        MouseIsDown = True

        gDummyForRotationAndZoomingOnlyImage = New Bitmap(gCorrectedImage).Clone
        If gCurrentZoom = 1 Then
        Else
            gDummyForRotationAndZoomingOnlyImage = New Bitmap(gDummyForRotationAndZoomingOnlyImage, Convert.ToInt32(gDummyForRotationAndZoomingOnlyImage.Width * (1 / gCurrentZoom)), Convert.ToInt32(gCorrectedImage.Height * (1 / gCurrentZoom)))
        End If

        FitLogic(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

        GarbageCollect()

        ' Resets the real image now that the rotations are done 
        ' cleans up the image after a long series of changes caused by the mouse being held down

        UpdatePictureBox(DoAllPictureBoxTransformations)
        ' If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

        SeCursor(Me, CursorState.Normal)

    End Sub

    Private Sub FitLogic(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

            ' fit to width and height

            Dim WidthAdjustement As Single

            If gViewChoice = ViewingOption.PrimaryandBottom OrElse gViewChoice = ViewingOption.PrimaryOnly Then
                WidthAdjustement = CSng(Me.SplitContainer1.Panel2.Width)
            Else
                WidthAdjustement = CSng((Me.SplitContainer1.Panel2.Width) / 2)
            End If

            Dim Ratio1 As Single = CSng(WidthAdjustement / CSng(ImageSizeAt100Percent.Width))
            Dim Ratio2 As Single = CSng(PictureBoxPrimary.Height / ImageSizeAt100Percent.Height)

            If Ratio1 < Ratio2 Then
                ChangeDummyZoom(Ratio1, ChangeType.Absolute)
            Else
                ChangeDummyZoom(Ratio2, ChangeType.Absolute)
            End If

        ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

            ' fit to width only

            Dim WidthAdjustement As Single

            If gViewChoice = ViewingOption.PrimaryandBottom OrElse gViewChoice = ViewingOption.PrimaryOnly Then
                WidthAdjustement = CSng(Me.SplitContainer1.Panel2.Width)
            Else
                WidthAdjustement = CSng((Me.SplitContainer1.Panel2.Width) / 2)
            End If

            ChangeDummyZoom(CSng(WidthAdjustement / CSng(ImageSizeAt100Percent.Width)), ChangeType.Absolute)


        Else

            ' fit to height only

            ChangeDummyZoom(CSng(PictureBoxPrimary.Height / ImageSizeAt100Percent.Height), ChangeType.Absolute)

        End If

        UpdatePictureBox(DoAllPictureBoxTransformations)

        Dim RectCorrected As Rectangle = GetTrimmedBoardersFromBitmap(New Bitmap(gCorrectedImage))

        'movingPoint = New Point(-1 * RectCorrected.X, -1 * RectCorrected.Y)

        Dim WidthCorrection As Integer
        Dim HeightCorrection As Integer

        If gViewChoice = ViewingOption.PrimaryandBottom OrElse gViewChoice = ViewingOption.PrimaryOnly Then
            WidthCorrection = (Me.SplitContainer1.Panel2.Width - RectCorrected.Width) / 2
        Else
            WidthCorrection = (CSng((Me.SplitContainer1.Panel2.Width) / 2) - RectCorrected.Width) / 2
        End If

        If gViewChoice = ViewingOption.PrimaryandRight OrElse gViewChoice = ViewingOption.PrimaryOnly Then
            HeightCorrection = (Me.SplitContainer1.Panel2.Height - RectCorrected.Height) / 2
        Else
            HeightCorrection = (CSng((Me.SplitContainer1.Panel2.Height) / 2) - RectCorrected.Height) / 2
        End If

        movingPoint = New Point(-1 * RectCorrected.X + WidthCorrection, -1 * RectCorrected.Y + HeightCorrection)

    End Sub

#End Region

#Region "Zoom"

    Private Sub tsZoom_MouseDown(sender As Object, e As MouseEventArgs) Handles tsZoom.MouseDown

        tsZoom_MouseDown_MyHandler(e, False, False, False)

    End Sub
    Private Sub tsZoom_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        SeCursor(Me, CursorState.Wait)

        MouseIsDown = True

        Const IdealMinimumZoomDelay As Integer = 12

        Dim InitialDelayNeeded As Boolean = True

        gDummyForRotationAndZoomingOnlyImage = New Bitmap(gCorrectedImage).Clone
        If gCurrentZoom = 1 Then
        Else
            gDummyForRotationAndZoomingOnlyImage = New Bitmap(gDummyForRotationAndZoomingOnlyImage, Convert.ToInt32(gDummyForRotationAndZoomingOnlyImage.Width * (1 / gCurrentZoom)), Convert.ToInt32(gCorrectedImage.Height * (1 / gCurrentZoom)))
        End If

        Try

            While MouseIsDown

                Dim sw As New Diagnostics.Stopwatch
                sw.Start()

                Dim Multiplier As Single
                Multiplier = 1
                Multiplier = IIf(My.Computer.Keyboard.ShiftKeyDown, 2, 1)
                Multiplier *= IIf(My.Computer.Keyboard.CtrlKeyDown, 5, 1)

                If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then
                    ChangeDummyZoom(-0.01 * Multiplier, ChangeType.Relative)

                ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then
                    ChangeDummyZoom(gZoomDefault, ChangeType.Absolute)
                    Exit While

                ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then
                    ChangeDummyZoom(+0.01 * Multiplier, ChangeType.Relative)

                End If

                If InitialDelayNeeded Then
                    InitialDelayNeeded = False
                    Application.DoEvents()

                    Dim ddelay = 350 - sw.ElapsedMilliseconds
                    If ddelay > 0 Then
                        System.Threading.Thread.Sleep(ddelay)
                    End If


                End If

                sw.Stop()

                If IdealMinimumZoomDelay > sw.ElapsedMilliseconds Then
                    System.Threading.Thread.Sleep(IdealMinimumZoomDelay - CInt(sw.ElapsedMilliseconds))
                End If

                'force the image to be redrawn in the picturebox

                PictureBoxPrimary.Invalidate()

                Application.DoEvents()

            End While

            gDummyForRotationAndZoomingOnlyImage.Dispose()

        Catch ex As Exception

        End Try

        GarbageCollect()

        ' Resets the real image now that the rotations are done 
        ' cleans up the image after a long series of changes caused by the mouse being held down

        UpdatePictureBox(DoAllPictureBoxTransformations)
        ' If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

        SeCursor(Me, CursorState.Normal)

    End Sub

    Private Sub ChangeDummyZoom(ByVal ChangeAmount As Single, ByVal ChangeType As ChangeTypeEnum)

        Try

            Const High As Single = 10
            Const Low As Single = 0.1 ' 10%

            Static Dim BeepNeeded As Boolean = True

            Select Case ChangeType

                Case Is = ChangeTypeEnum.Absolute
                    gCurrentZoom = ChangeAmount
                    gCorrectedImage = New Bitmap(gCorrectedImage, Convert.ToInt32(gCorrectedImage.Width * ChangeAmount), Convert.ToInt32(gCorrectedImage.Height * ChangeAmount))

                Case Is = ChangeTypeEnum.Relative

                    If (gCurrentZoom = High And ChangeAmount > 0) OrElse ((gCurrentZoom = Low And ChangeAmount < 0)) Then
                        If BeepNeeded Then
                            Beep()
                            BeepNeeded = False
                        End If
                        Exit Sub
                    End If
                    BeepNeeded = True

                    Dim UpdateRequired As Boolean = (gCurrentZoom > Low) AndAlso (gCurrentZoom < High)

                    gCurrentZoom += ChangeAmount

                    If gCurrentZoom > High Then
                        gCurrentZoom = High
                        If Not UpdateRequired Then Exit Sub

                    ElseIf gCurrentZoom < Low Then
                        gCurrentZoom = Low
                        If Not UpdateRequired Then Exit Sub

                    End If

                    gCorrectedImage = New Bitmap(gDummyForRotationAndZoomingOnlyImage, Convert.ToInt32(gDummyForRotationAndZoomingOnlyImage.Width * gCurrentZoom), Convert.ToInt32(gDummyForRotationAndZoomingOnlyImage.Height * gCurrentZoom))

                    GarbageCollect()

                    Application.DoEvents()

            End Select

        Catch ex As Exception
        End Try

    End Sub

    Private Sub FinalForm_MouseWheel(sender As Object, e As MouseEventArgs) Handles PictureBoxPrimary.MouseWheel, PictureBoxBottom.MouseWheel, PictureBoxRight.MouseWheel

        If gSuspendPictureBoxUpdates Then Exit Sub

        Dim Multiplier As Single
        Multiplier = 1
        Multiplier = IIf(My.Computer.Keyboard.ShiftKeyDown, 2, 1)
        Multiplier *= IIf(My.Computer.Keyboard.CtrlKeyDown, 5, 1)

        gDummyForRotationAndZoomingOnlyImage = New Bitmap(gCorrectedImage)
        If gCurrentZoom = 1 Then
        Else
            gDummyForRotationAndZoomingOnlyImage = New Bitmap(gDummyForRotationAndZoomingOnlyImage, Convert.ToInt32(gDummyForRotationAndZoomingOnlyImage.Width * (1 / gCurrentZoom)), Convert.ToInt32(gCorrectedImage.Height * (1 / gCurrentZoom)))
        End If

        If e.Delta < 0 Then

            Timer1.Stop()
            ChangeDummyZoom(-0.01 * Multiplier, ChangeType.Relative)
            If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()
            Timer1.Start()

        ElseIf e.Delta > 0 Then

            Timer1.Stop()
            ChangeDummyZoom(+0.01 * Multiplier, ChangeType.Relative)
            If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()
            Timer1.Start()

        End If

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Timer1.Stop()

        gDummyForRotationAndZoomingOnlyImage.Dispose()
        UpdatePictureBox(DoAllPictureBoxTransformations)
        If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

    End Sub

    Private Sub ChangeZoom(ByVal ChangeAmount As Single, ByVal ChangeType As ChangeTypeEnum)

        Try

            Const High As Single = 10
            Const Low As Single = 0.01

            Static Dim BeepNeeded As Boolean = True

            Select Case ChangeType

                Case Is = ChangeTypeEnum.Absolute
                    gCurrentZoom = ChangeAmount
                    gCorrectedImage = New Bitmap(gCorrectedImage, Convert.ToInt32(gCorrectedImage.Width * gCurrentZoom), Convert.ToInt32(gCorrectedImage.Height * gCurrentZoom))

                Case Is = ChangeTypeEnum.Relative

                    If (gCurrentZoom = High And ChangeAmount > 0) OrElse ((gCurrentZoom = Low And ChangeAmount < 0)) Then
                        If BeepNeeded Then
                            Beep()
                            BeepNeeded = False
                        End If
                        Exit Sub
                    End If
                    BeepNeeded = True

                    Dim UpdateRequired As Boolean = (gCurrentZoom > Low) AndAlso (gCurrentZoom < High)

                    gCurrentZoom += ChangeAmount

                    If gCurrentZoom > High Then
                        gCurrentZoom = High
                        If Not UpdateRequired Then Exit Sub

                    ElseIf gCurrentZoom < Low Then
                        gCurrentZoom = Low
                        If Not UpdateRequired Then Exit Sub

                    End If

                    gCorrectedImage = New Bitmap(gCorrectedImage, Convert.ToInt32(gCorrectedImage.Width * (1 + ChangeAmount)), Convert.ToInt32(gCorrectedImage.Height * (1 + ChangeAmount)))

                    GarbageCollect()

            End Select

        Catch ex As Exception
        End Try

    End Sub


#End Region

#Region "Crop tool"

    Private MouseIsInTheCropToolBox As Boolean = False

    Private Sub CropToolStripMenuItem_MouseEnter(sender As Object, e As EventArgs)
        MouseIsInTheCropToolBox = True
    End Sub

    Private Sub CropToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs)
        MouseIsInTheCropToolBox = False
    End Sub

    Private CropToolCursor As Cursor = Cursors.Cross
    Private Sub tsCrop_MouseDown(sender As Object, e As MouseEventArgs) Handles tsCrop.MouseDown

        If CropToolClicksAreNeeded = 0 Then

            tsCrop.ForeColor = SelectedTool
            MouseIsInTheCropToolBox = True
            CropToolClicksAreNeeded = 1
            Cursor = CropToolCursor

        Else

            tsCrop.ForeColor = UnSelectedTool
            CropToolClicksAreNeeded = 0
            Cursor = Cursors.Default

        End If

    End Sub

#End Region

#Region "Flip"
    Private Sub tsMirror_MouseDown(sender As Object, e As MouseEventArgs) Handles tsMirror.MouseDown

        tsFlip_MouseDown_MyHandler(e, False, False, False)

    End Sub


    Private Sub tsFlip_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

            If gCurrentImageIsMirroredHorizontally Then

                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeMirroredHorizontal = False
                UpdatePictureBox(RequiredUpdates)
                gCurrentImageIsMirroredHorizontally = False

            Else

                gCurrentImageIsMirroredHorizontally = True
                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                UpdatePictureBox(RequiredUpdates)

            End If

        ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then


            If gCurrentImageIsMirroredVertically Then

                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeMirroredVertical = False
                UpdatePictureBox(RequiredUpdates)
                gCurrentImageIsMirroredVertically = False

            Else

                gCurrentImageIsMirroredVertically = True
                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                UpdatePictureBox(RequiredUpdates)

            End If

        ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

            Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
            RequiredUpdates.ChangeMirroredHorizontal = False
            RequiredUpdates.ChangeMirroredVertical = False
            UpdatePictureBox(RequiredUpdates)
            gCurrentImageIsMirroredHorizontally = gCurrentImageIsMirroredHorizontallyDefault
            gCurrentImageIsMirroredVertically = gCurrentImageIsMirroredVerticallyDefault

        End If

        UpdateStatusBar()

    End Sub
    Friend Sub MirrorHorizontally()

        Try

            Dim tempimage As Bitmap = New Bitmap(gCorrectedImage).Clone
            tempimage.RotateFlip(RotateFlipType.RotateNoneFlipX)
            gCorrectedImage = New Bitmap(tempimage)

            If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

            tempimage.Dispose()

        Catch ex As Exception

        End Try

    End Sub
    Friend Sub MirrorVertically()

        Try

            Dim tempimage As Bitmap = New Bitmap(gCorrectedImage).Clone
            tempimage.RotateFlip(RotateFlipType.RotateNoneFlipY)
            gCorrectedImage = New Bitmap(tempimage)
            If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()
            tempimage.Dispose()

        Catch ex As Exception

        End Try

    End Sub
    Private Sub FlipHorizontalAndVertical()

        Try

            Dim tempimage As Bitmap = New Bitmap(gCorrectedImage).Clone
            tempimage.RotateFlip(RotateFlipType.RotateNoneFlipXY)
            gCorrectedImage = New Bitmap(tempimage)
            If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()
            tempimage.Dispose()

        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Change to Invert"

    Private Sub tsInvert_MouseDown(sender As Object, e As MouseEventArgs) Handles tsInvert.MouseDown

        tsInvert_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsInvert_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        If LeftMouseButtonDown OrElse RightMouseButtonDown OrElse (e.Button = MouseButtons.Left) OrElse (e.Button = MouseButtons.Right) Then

            If gCurrentImageIsInverted Then
                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeInvert = False
                UpdatePictureBox(RequiredUpdates)
                gCurrentImageIsInverted = False
            Else
                Invert()
                gCurrentImageIsInverted = True
            End If

        ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

            Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
            RequiredUpdates.ChangeInvert = False
            UpdatePictureBox(RequiredUpdates)
            gCurrentImageIsInverted = False

        End If

        UpdateStatusBar()

    End Sub

    Private Sub ChangeInvert()

        If gCurrentImageIsInverted Then
            Invert()
        End If

    End Sub

    Friend Sub Invert()

        ' ref: https://stackoverflow.com/questions/11428724/how-to-create-inverse-png-image/11429982#11429982

        Dim nWidth As Integer = gCorrectedImage.Width
        Dim nHeight As Integer = gCorrectedImage.Height

        Dim bmData As BitmapData = Nothing

        Try

            Dim tempimg As Bitmap = New Bitmap(gCorrectedImage)

            Dim imgPixelFormat As PixelFormat = tempimg.PixelFormat
            bmData = tempimg.LockBits(New Rectangle(0, 0, tempimg.Width, tempimg.Height), ImageLockMode.ReadWrite, imgPixelFormat)

            Dim scan0 As IntPtr = bmData.Scan0
            Dim stride As Integer = bmData.Stride

            Dim pf((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, pf, 0, pf.Length)

            Dim pos As Integer = 0

            Parallel.[For](0, nHeight - 1, Sub(y)

                                               Dim pfpos As Integer = y * stride

                                               For x As Integer = 0 To nWidth - 1

                                                   Dim red As Byte = pf(pfpos + 2)
                                                   Dim green As Byte = pf(pfpos + 1)
                                                   Dim blue As Byte = pf(pfpos)

                                                   red = 255 - red
                                                   green = 255 - green
                                                   blue = 255 - blue

                                                   Dim p As HSLData = RGBtoHSL(red, green, blue)

                                                   Dim nPixel As PixelData = HSLtoRGB(p.Hue, p.Saturation, p.Luminance)

                                                   pf(pfpos + 2) = nPixel.red
                                                   pf(pfpos + 1) = nPixel.green
                                                   pf(pfpos) = nPixel.blue

                                                   pfpos += 4

                                               Next

                                           End Sub)


            Marshal.Copy(pf, 0, bmData.Scan0, pf.Length)

            tempimg.UnlockBits(bmData)

            gCorrectedImage = New Bitmap(tempimg)
            If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()
            tempimg.Dispose()

        Catch ex As Exception

        End Try


    End Sub


#End Region

#Region "Change Brightness"

    Private Sub tsBrightness_MouseDown(sender As Object, e As MouseEventArgs) Handles tsBrightness.MouseDown

        tsBrightness_MouseDown_MyHandler(e, False, False, False)

    End Sub


    Private Sub tsBrightness_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        MouseIsDown = True

        Dim DelayTime As Integer = 350 ' first delay is longer - allowing for a single click only

        While MouseIsDown

            Dim Multiplier As Single
            Multiplier = 1
            Multiplier = IIf(My.Computer.Keyboard.ShiftKeyDown, 2, 1)
            Multiplier *= IIf(My.Computer.Keyboard.CtrlKeyDown, 5, 1)

            If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

                ChangeBrightness(-0.01 * Multiplier, ChangeTypeEnum.Relative)


            ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeBrightness = False
                UpdatePictureBox(RequiredUpdates)


            ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then

                ChangeBrightness(+0.01 * Multiplier, ChangeTypeEnum.Relative)

            End If

            Application.DoEvents()
            System.Threading.Thread.Sleep(DelayTime)
            DelayTime = 0
            Application.DoEvents()

        End While

    End Sub
    Private Sub ChangeBrightness(ByVal ChangeAmount As Single, ByVal ChangeType As ChangeTypeEnum)

        ' ref: https://stackoverflow.com/questions/1269627/brightness-exposure-function-with-vb-net

        Const High As Single = 1
        Const Low As Single = -1

        Static Dim BeepNeeded As Boolean = True

        Select Case ChangeType

            Case Is = ChangeTypeEnum.Absolute
                gCurrentBrightness = ChangeAmount

            Case Is = ChangeTypeEnum.Relative

                If (gCurrentBrightness = High And ChangeAmount > 0) OrElse ((gCurrentBrightness = Low And ChangeAmount < 0)) Then
                    If BeepNeeded Then
                        Beep()
                        BeepNeeded = False
                    End If
                    Exit Sub
                End If
                BeepNeeded = True

                Dim UpdateRequired As Boolean = (gCurrentBrightness > Low) AndAlso (gCurrentBrightness < High)

                gCurrentBrightness += ChangeAmount

                If gCurrentBrightness > High Then
                    gCurrentBrightness = High
                    If Not UpdateRequired Then Exit Sub

                ElseIf gCurrentBrightness < Low Then
                    gCurrentBrightness = Low
                    If Not UpdateRequired Then Exit Sub

                End If

        End Select

        ' Reset all but Brightness
        gSuspendPictureBoxUpdates = True
        Dim hgCurrentBrightness As Single = gCurrentBrightness
        Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
        RequiredUpdates.ChangeBrightness = False
        RequiredUpdates.ChangeContrast = False
        RequiredUpdates.ChangeGamma = False
        RequiredUpdates.UpdatePicturebox = False
        UpdatePictureBox(RequiredUpdates)
        gCurrentBrightness = hgCurrentBrightness
        gSuspendPictureBoxUpdates = False

        ChangeBrightnessContrastGamma(gCurrentBrightness, gCurrentContrast)

    End Sub

#End Region

#Region "Change Contrast"
    Private Sub tsContrast_MouseDown(sender As Object, e As MouseEventArgs) Handles tsContrast.MouseDown

        tsContrast_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsContrast_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        MouseIsDown = True

        Dim DelayTime As Integer = 350 ' first delay is longer - allowing for a single click only

        While MouseIsDown

            Dim Multiplier As Single
            Multiplier = 1
            Multiplier = IIf(My.Computer.Keyboard.ShiftKeyDown, 2, 1)
            Multiplier *= IIf(My.Computer.Keyboard.CtrlKeyDown, 5, 1)

            If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

                ChangeContrast(-0.005 * Multiplier, ChangeTypeEnum.Relative)

            ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeContrast = False
                UpdatePictureBox(RequiredUpdates)

            ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then

                ChangeContrast(0.005 * Multiplier, ChangeTypeEnum.Relative)

            End If

            Application.DoEvents()
            System.Threading.Thread.Sleep(DelayTime)
            DelayTime = 0
            Application.DoEvents()

        End While

    End Sub

    Private Sub ChangeContrast(ByVal ChangeAmount As Single, ByVal ChangeType As ChangeTypeEnum)

        ' ref: https://stackoverflow.com/questions/23865511/contrast-with-color-matrix

        Const High As Single = 3
        Const Low As Single = 0

        Static Dim BeepNeeded As Boolean = True

        Select Case ChangeType

            Case Is = ChangeTypeEnum.Absolute
                gCurrentContrast = ChangeAmount

            Case Is = ChangeTypeEnum.Relative

                If (gCurrentContrast = High And ChangeAmount > 0) OrElse ((gCurrentContrast = Low And ChangeAmount < 0)) Then
                    If BeepNeeded Then
                        Beep()
                        BeepNeeded = False
                    End If
                    Exit Sub
                End If
                BeepNeeded = True

                Dim UpdateRequired As Boolean = (gCurrentContrast > Low) AndAlso (gCurrentContrast < High)

                gCurrentContrast += ChangeAmount

                If gCurrentContrast > High Then
                    gCurrentContrast = High
                    If Not UpdateRequired Then Exit Sub

                ElseIf gCurrentContrast < Low Then
                    gCurrentContrast = Low
                    If Not UpdateRequired Then Exit Sub

                End If

        End Select

        'reset all but Contrast
        gSuspendPictureBoxUpdates = True
        Dim hgCurrentContrast As Single = gCurrentContrast
        Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
        RequiredUpdates.ChangeBrightness = False
        RequiredUpdates.ChangeContrast = False
        RequiredUpdates.ChangeGamma = False
        RequiredUpdates.UpdatePicturebox = False
        UpdatePictureBox(RequiredUpdates)
        gCurrentContrast = hgCurrentContrast
        gSuspendPictureBoxUpdates = False

        ChangeBrightnessContrastGamma(gCurrentBrightness, gCurrentContrast)

    End Sub

#End Region

#Region "Change Gamma"
    Private Sub tsGamma_MouseDown(sender As Object, e As MouseEventArgs) Handles tsGamma.MouseDown

        tsGamma_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsGamma_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        MouseIsDown = True

        Dim DelayTime As Integer = 350 ' first delay is longer - allowing for a single click only

        While MouseIsDown

            Dim Multiplier As Single
            Multiplier = 1
            Multiplier = IIf(My.Computer.Keyboard.ShiftKeyDown, 2, 1)
            Multiplier *= IIf(My.Computer.Keyboard.CtrlKeyDown, 5, 1)

            If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

                ChangeGamma(-0.01 * Multiplier, ChangeTypeEnum.Relative)

            ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeGamma = False
                UpdatePictureBox(RequiredUpdates)

            ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then

                ChangeGamma(0.01 * Multiplier, ChangeTypeEnum.Relative)

            End If

            Application.DoEvents()
            System.Threading.Thread.Sleep(DelayTime)
            DelayTime = 0
            Application.DoEvents()

        End While

    End Sub
    Private Sub ChangeGamma(ByVal ChangeAmount As Single, ByVal ChangeType As ChangeTypeEnum)

        ' ref: https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.imageattributes.setgamma?view=dotnet-plat-ext-6.0

        Const High As Single = 6.99
        Const Low As Single = 0.01

        Static Dim BeepNeeded As Boolean = True

        Select Case ChangeType

            Case Is = ChangeTypeEnum.Absolute
                gCurrentGamma = ChangeAmount
                Exit Sub

            Case Is = ChangeTypeEnum.Relative

                If (gCurrentGamma = High And ChangeAmount > 0) OrElse ((gCurrentGamma = Low And ChangeAmount < 0)) Then
                    If BeepNeeded Then
                        Beep()
                        BeepNeeded = False
                    End If
                    Exit Sub
                End If
                BeepNeeded = True

                Dim UpdateRequired As Boolean = (gCurrentGamma > Low) AndAlso (gCurrentGamma < High)

                gCurrentGamma += ChangeAmount

                If gCurrentGamma > High Then
                    gCurrentGamma = High
                    If Not UpdateRequired Then Exit Sub

                ElseIf gCurrentGamma < Low Then
                    gCurrentGamma = Low
                    If Not UpdateRequired Then Exit Sub

                End If

        End Select

        If ChangeType = ChangeTypeEnum.Absolute Then

        Else

            'reset all but Gamma
            gSuspendPictureBoxUpdates = True
            Dim hgCurrentGamma As Single = gCurrentGamma
            Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
            RequiredUpdates.ChangeBrightness = False
            RequiredUpdates.ChangeContrast = False
            RequiredUpdates.ChangeGamma = False
            RequiredUpdates.UpdatePicturebox = False
            UpdatePictureBox(RequiredUpdates)
            gCurrentGamma = hgCurrentGamma
            gSuspendPictureBoxUpdates = False

            ChangeBrightnessContrastGamma(gCurrentBrightness, gCurrentContrast)

        End If

    End Sub

#End Region

#Region "Shared Brightness Contrast Gamma"
    Private Sub ChangeBrightnessContrastGamma(ByVal Bightness As Single, ByVal Contrast As Single)

        Dim g As Graphics = Graphics.FromImage(gCorrectedImage)

        Dim r As Rectangle = New Rectangle(0, 0, gCorrectedImage.Width, gCorrectedImage.Height)

        g.DrawImage(gCorrectedImage, r)

        Dim colorMatrixVal As Single()() = {
     New Single() {Contrast, 0, 0, 0, 0},
     New Single() {0, Contrast, 0, 0, 0},
     New Single() {0, 0, Contrast, 0, 0},
     New Single() {0, 0, 0, 1, 0},
     New Single() {Bightness, Bightness, Bightness, 0, 1}}

        Dim colorMatrix As New ColorMatrix(colorMatrixVal)

        Dim ia As New ImageAttributes

        ia.SetGamma(gCurrentGamma)

        g.DrawImage(gCorrectedImage, r, 0, 0, gCorrectedImage.Width, gCorrectedImage.Height, GraphicsUnit.Pixel, ia)

        ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)

        g.DrawImage(gCorrectedImage, r, 0, 0, gCorrectedImage.Width, gCorrectedImage.Height, GraphicsUnit.Pixel, ia)

        If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

        g.Dispose()

        UpdateStatusBar()

    End Sub

#End Region

#Region "Change Saturation"

    Private Sub tsSaturation_MouseDown(sender As Object, e As MouseEventArgs) Handles tsSaturation.MouseDown

        tsSaturation_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsSaturation_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        SeCursor(Me, CursorState.Wait)

        MouseIsDown = True

        Dim DelayTime As Integer = 350 ' first delay is longer - allowing for a single click only

        While MouseIsDown

            Dim Multiplier As Single
            Multiplier = 1
            Multiplier = IIf(My.Computer.Keyboard.ShiftKeyDown, 2, 1)
            Multiplier *= IIf(My.Computer.Keyboard.CtrlKeyDown, 5, 1)

            If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

                ChangeSaturation(-0.01 * Multiplier, ChangeTypeEnum.Relative)

            ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeSaturation = False
                UpdatePictureBox(RequiredUpdates)

            ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then

                ChangeSaturation(0.01 * Multiplier, ChangeTypeEnum.Relative)

            End If

            Application.DoEvents()
            System.Threading.Thread.Sleep(DelayTime)
            DelayTime = 0
            Application.DoEvents()

        End While


        '        UpdatePictureBox(DoAllPictureBoxTransformations) 'v2.0 testing here  add this in???

        SeCursor(Me, CursorState.Normal)


    End Sub

    Private Sub ChangeSaturation(ByVal ChangeAmount As Single, ByVal ChangeType As ChangeTypeEnum)

        ' ref https://stackoverflow.com/questions/14364716/faster-algorithm-to-change-hue-saturation-lightness-in-a-bitmap
        ' rer http://www.graficaobscura.com/matrix/index.html

        Const High As Single = 1
        Const Low As Single = 0

        Static Dim BeepNeeded As Boolean = True

        Select Case ChangeType

            Case Is = ChangeTypeEnum.Absolute
                gCurrentSaturation = ChangeAmount

            Case Is = ChangeTypeEnum.Relative

                If (gCurrentSaturation = High And ChangeAmount > 0) OrElse ((gCurrentSaturation = Low And ChangeAmount < 0)) Then
                    If BeepNeeded Then
                        Beep()
                        BeepNeeded = False
                    End If
                    Exit Sub
                End If
                BeepNeeded = True

                Dim UpdateRequired As Boolean = (gCurrentSaturation > Low) AndAlso (gCurrentSaturation < High)

                gCurrentSaturation += ChangeAmount

                If gCurrentSaturation > High Then
                    gCurrentSaturation = High
                    If Not UpdateRequired Then Exit Sub

                ElseIf gCurrentSaturation < Low Then
                    gCurrentSaturation = Low
                    If Not UpdateRequired Then Exit Sub

                End If

        End Select

        ChangeSaturationNow(ChangeAmount, ChangeType)

        If ChangeType = ChangeTypeEnum.Absolute Then
        Else
            If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()
        End If

    End Sub

    Private Structure HSLData
        Public Hue As Single
        Public Saturation As Single
        Public Luminance As Single
    End Structure

    Private Structure PixelData
        Public blue As Byte
        Public green As Byte
        Public red As Byte
        Public alpha As Byte
    End Structure
    Private Sub ChangeSaturationNow(ByVal fSaturation As Single, ByVal ChangeType As ChangeTypeEnum)

        ' ref https://social.msdn.microsoft.com/Forums/vstudio/en-US/1d403576-2aa4-4942-b0f0-f22f77805196/how-to-get-saturation-image-using-vbnet?forum=vbgeneral

        Dim bmData As BitmapData = Nothing

        Try

            If ChangeType = ChangeTypeEnum.Relative Then
                Dim hgCurrentSaturation As Single = gCurrentSaturation
                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeSaturation = False
                RequiredUpdates.UpdatePicturebox = False
                UpdatePictureBox(RequiredUpdates)
                gCurrentSaturation = hgCurrentSaturation
            End If

            Dim tempimg As Bitmap = New Bitmap(gCorrectedImage)

            Dim nWidth As Integer = tempimg.Width
            Dim nHeight As Integer = tempimg.Height

            Dim imgPixelFormat As PixelFormat = tempimg.PixelFormat
            bmData = tempimg.LockBits(New Rectangle(0, 0, tempimg.Width, tempimg.Height), ImageLockMode.ReadWrite, imgPixelFormat)

            Dim scan0 As IntPtr = bmData.Scan0
            Dim stride As Integer = bmData.Stride

            Dim pf((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, pf, 0, pf.Length)

            Dim pos As Integer = 0

            Parallel.[For](0, nHeight - 1, Sub(y)

                                               Dim pfpos As Integer = y * stride

                                               For x As Integer = 0 To nWidth - 1

                                                   Dim red As Byte = pf(pfpos + 2)
                                                   Dim green As Byte = pf(pfpos + 1)
                                                   Dim blue As Byte = pf(pfpos)

                                                   Dim p As HSLData = RGBtoHSL(red, green, blue)

                                                   Dim nPixel As PixelData = HSLtoRGB(p.Hue, gCurrentSaturation, p.Luminance)

                                                   pf(pfpos + 2) = nPixel.red
                                                   pf(pfpos + 1) = nPixel.green
                                                   pf(pfpos) = nPixel.blue

                                                   pfpos += 4

                                               Next

                                           End Sub)

            Marshal.Copy(pf, 0, bmData.Scan0, pf.Length)

            tempimg.UnlockBits(bmData)
            gCorrectedImage = New Bitmap(tempimg)
            tempimg.Dispose()

        Catch

        End Try

    End Sub

    Private Function RGBtoHSL(Red As Integer, Green As Integer, Blue As Integer) As HSLData

        Dim hsl As New HSLData()

        Dim c As Color = Color.FromArgb(255, Red, Green, Blue)
        hsl.Hue = c.GetHue()
        hsl.Saturation = c.GetSaturation()
        hsl.Luminance = c.GetBrightness()

        Return hsl

    End Function

    Private Function HSLtoRGB(H As Double, S As Double, L As Double) As PixelData
        Dim Temp1 As Double = 0.0, Temp2 As Double = 0.0
        Dim r As Double = 0.0, g As Double = 0.0, b As Double = 0.0

        If S = 0 Then
            r = L
            g = L
            b = L
        Else
            If L < 0.5 Then
                Temp2 = L * (1.0 + S)
            Else
                Temp2 = (L + S) - (S * L)
            End If

            Temp1 = 2.0 * L - Temp2

            Dim hTmp As Double = H / 360.0
            Dim rTmp As Double, gTmp As Double, bTmp As Double

            rTmp = hTmp + (1.0 / 3.0)
            gTmp = hTmp
            bTmp = hTmp - (1.0 / 3.0)

            If rTmp < 0.0 Then rTmp += 1.0
            If gTmp < 0.0 Then gTmp += 1.0
            If bTmp < 0.0 Then bTmp += 1.0
            If rTmp > 1.0 Then rTmp -= 1.0
            If gTmp > 1.0 Then gTmp -= 1.0
            If bTmp > 1.0 Then bTmp -= 1.0

            If 6.0 * rTmp < 1.0 Then
                r = Temp1 + (Temp2 - Temp1) * 6.0 * rTmp
            ElseIf 2.0 * rTmp < 1.0 Then
                r = Temp2
            ElseIf 3.0 * rTmp < 2.0 Then
                r = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - rTmp) * 6.0
            Else
                r = Temp1
            End If

            If 6.0 * gTmp < 1.0 Then
                g = Temp1 + (Temp2 - Temp1) * 6.0 * gTmp
            ElseIf 2.0 * gTmp < 1.0 Then
                g = Temp2
            ElseIf 3.0 * gTmp < 2.0 Then
                g = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - gTmp) * 6.0
            Else
                g = Temp1
            End If

            If 6.0 * bTmp < 1.0 Then
                b = Temp1 + (Temp2 - Temp1) * 6.0 * bTmp
            ElseIf 2.0 * bTmp < 1.0 Then
                b = Temp2
            ElseIf 3.0 * bTmp < 2.0 Then
                b = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - bTmp) * 6.0
            Else
                b = Temp1
            End If

        End If

        Dim RGB As New PixelData()

        r *= 255.0
        g *= 255.0
        b *= 255.0

        RGB.red = CByte(CInt(r))
        RGB.green = CByte(CInt(g))
        RGB.blue = CByte(CInt(b))

        Return RGB

    End Function

#End Region

#Region "Change to Grayscale"

    Private Sub tsGrayscale_MouseDown(sender As Object, e As MouseEventArgs) Handles tsGrayscale.MouseDown

        tsGrayScale_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsGrayScale_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        If LeftMouseButtonDown OrElse RightMouseButtonDown OrElse (e.Button = MouseButtons.Left) OrElse (e.Button = MouseButtons.Right) Then

            If gCurrentImageIsInGrayscale Then
                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeGrayScale = False
                UpdatePictureBox(RequiredUpdates)
                gCurrentImageIsInGrayscale = False
            Else
                ConvertToGrayscale()
                gCurrentImageIsInGrayscale = True
            End If

        ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

            Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
            RequiredUpdates.ChangeGrayScale = False
            UpdatePictureBox(RequiredUpdates)
            gCurrentImageIsInGrayscale = False

        End If

        UpdateStatusBar()

    End Sub

    Private Sub ChangeGrayScale()

        If gCurrentImageIsInGrayscale Then
            ConvertToGrayscale()
        End If

    End Sub

    Private Sub ConvertToGrayscale()

        ' ref http://www.graficaobscura.com/matrix/index.html

        Dim Saturation As Single = 0.1

        Dim g As Graphics = Graphics.FromImage(gCorrectedImage)
        Dim r As Rectangle = New Rectangle(0, 0, gCorrectedImage.Width, gCorrectedImage.Height)

        Const rwgt As Single = 0.3086
        Const gwgt As Single = 0.6094
        Const bwgt As Single = 0.082

        Dim ColorMatrix As ColorMatrix = New ColorMatrix()

        Dim baseSat As Single = 1.0F - Saturation

        ColorMatrix(0, 0) = baseSat * rwgt + Saturation
        ColorMatrix(0, 1) = baseSat * rwgt
        ColorMatrix(0, 2) = baseSat * rwgt
        ColorMatrix(1, 0) = baseSat * gwgt
        ColorMatrix(1, 1) = baseSat * gwgt + Saturation
        ColorMatrix(1, 2) = baseSat * gwgt
        ColorMatrix(2, 0) = baseSat * bwgt
        ColorMatrix(2, 1) = baseSat * bwgt
        ColorMatrix(2, 2) = baseSat * bwgt + Saturation

        Dim ia As New ImageAttributes

        ia.SetColorMatrix(ColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)

        g.DrawImage(gCorrectedImage, r, 0, 0, gCorrectedImage.Width, gCorrectedImage.Height, GraphicsUnit.Pixel, ia)

        If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

        g.Dispose()

        System.Threading.Thread.Sleep(500)

    End Sub


#End Region

#Region "Effects"

    Private Sub tsEffects_MouseDown(sender As Object, e As MouseEventArgs) Handles tsEffects.MouseDown

        tsEffects_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private frmPickEffectsMatrixOpen As Boolean = False

    Private ChosenProfileName As String

    Private Sub lbEnabledProfiles_Click(sender As Object, e As EventArgs)

        ChosenProfileName = sender.text

    End Sub

    Private Sub tsEffects_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        Static BeingProcessed As Boolean = False

        If BeingProcessed Then Exit Sub
        BeingProcessed = True


        Try

            If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

                Dim EffectBoxesAreVisable As Boolean = EnhancedTreeViewBoxes(1).Visible

                LoadEffectsProfile(My.Settings.EffectsProfileName)

                LoadAndProcessTheEffectsWindow()

                RebuildViewerScreen()

                If EffectBoxesAreVisable Then
                    PopUpEffectsSelectionBoxes()
                End If


            ElseIf MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

                ResetEffects()

                For x = 1 To gMaxEffectsSupported
                    EnhancedTreeViewBoxes(x).Visible = False
                Next

                Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
                RequiredUpdates.ChangeEffects = False
                UpdatePictureBox(RequiredUpdates)


            ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then

                ' if there are enhanced lists boxes on the screen, close them
                ' otherwise create the first one

                gEffectsBoxesAreHidden = False
                If EnhancedTreeViewBoxes(1).Visible Then

                    EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).btnDone.PerformClick()

                Else

                    PopUpEffectsSelectionBoxes()
                    FinalForm_Resize(Nothing, Nothing)
                    PictureBoxPrimary.Invalidate()

                End If

            End If

        Catch ex As Exception

        End Try

        GarbageCollect()

        BeingProcessed = False

    End Sub

    Private Sub RebuildViewerScreen()

        Try

            If gProfileRevalidationRequired Then
                ValidateEffectsProfiles()
            End If

            If gCurrentNumberOfEffectsBoxesInUse > 0 Then

                'remove all effects which are no longer available; for example if they were disabled or removed via the Effects Design window
                For x = 1 To gCurrentNumberOfEffectsBoxesInUse

                    gCurrentNamesOfEffectsInUse(x) = gCurrentNamesOfEffectsInUse(x)

                    If (gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse) OrElse gListOfAllProfiles.Contains(ConvertXMLFileNameToProfileName(gCurrentNamesOfEffectsInUse(x))) Then
                    Else
                        RemoveAnEffect_Part1(x)
                    End If

                Next

                'rebuild the effects from the ground up in case the user also changed a value in one of the effects
                For x = 1 To gMaxEffectsSupported
                    EnhancedTreeViewBoxes(x).Visible = False
                Next

                PopUpEffectsSelectionBoxes()

            End If

            UpdateButtonAndToolTip()
            FinalForm_Resize(Nothing, Nothing)
            Application.DoEvents()

            UpdatePictureBox(DoAllPictureBoxTransformations)
            UpdateStatusBar()

        Catch ex As Exception

        End Try

    End Sub
    Private Sub ResetEffects()

        For x As Integer = 1 To gCurrentNumberOfEffectsBoxesInUse
            gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse
        Next
        gCurrentNumberOfEffectsBoxesInUse = 0

        gEffectsBoxesAreHidden = True

    End Sub

    Private Sub LoadAndProcessTheEffectsWindow()

        Try

            TempHideAR4W(True)

            Dim hgViewChoice As ViewingOption = gViewChoice

            If My.Settings.PinnedToTop Then
                MakeTopMostWindow(Me.Handle, False)
            End If

            Me.Hide()

            'Dim RequiredUpdates As PictureBoxTransformationsStructure = DoAllPictureBoxTransformations
            'RequiredUpdates.ChangeEffects = False
            'UpdatePictureBox(RequiredUpdates)

            gfrmPickEffectsStartLocation = Me.Location

            Dim frmPickEffectsMatrix As frmEffectsDesign = New frmEffectsDesign

            frmPickEffectsMatrixOpen = True
            frmPickEffectsMatrix.ShowDialog()
            frmPickEffectsMatrixOpen = False

            gViewChoice = hgViewChoice

            Me.Show()

            frmPickEffectsMatrix.Dispose()

            GarbageCollect()

            If My.Settings.PinnedToTop Then
                MakeTopMostWindow(Me.Handle, True)
            End If

            Application.DoEvents()

            TempHideAR4W(False)

            MouseIsDown = False


        Catch ex As Exception

        End Try

    End Sub

    Private HeightToTheEffectControl As Integer = 0

    Private Sub PopUpEffectsSelectionBoxes()

        Try

            HeightToTheEffectControl = ToolStripView.Location.Y - 1

            'find the height to the "TsEffects" control
            For Each c As ToolStripItem In ToolStripView.Items

                If c.Name = "tsEffects" Then
                    Exit For
                Else
                    HeightToTheEffectControl += c.Height + 4 ' the +4 is for padding around the controls
                End If

            Next

            HeightToTheEffectControl -= 5  ' tweak to centre of work 'Effects'

            ' load enhanced tree nodes

            EnhancedTreeViewBoxes(1).TreeView1.BeginUpdate()

            EnhancedTreeViewBoxes(1).TreeView1.Nodes.Clear()
            EnhancedTreeViewBoxes(1).TreeView1.Nodes.Add("Effect Profiles")
            EnhancedTreeViewBoxes(1).TreeView1.Nodes(0).Expand()
            EnhancedTreeViewBoxes(1).TreeView1.Nodes(0).Tag = gTreeViewFolderTagDesignation

            Dim currentNode As TreeNode = EnhancedTreeViewBoxes(1).TreeView1.Nodes(0)
            AddFoldersAndFilesToTreeview(currentNode, gXML_Path_Name, True)
            currentNode = Nothing

            EnhancedTreeViewBoxes(1).TreeView1.EndUpdate()

            For x = 2 To gMaxEffectsSupported

                EnhancedTreeViewBoxes(x).TreeView1.BeginUpdate()

                EnhancedTreeViewBoxes(x).TreeView1.Nodes.Clear()

                For Each node As TreeNode In EnhancedTreeViewBoxes(1).TreeView1.Nodes
                    EnhancedTreeViewBoxes(x).TreeView1.Nodes.Add(node.Clone)
                Next
                EnhancedTreeViewBoxes(x).TreeView1.Nodes(0).Expand()

                EnhancedTreeViewBoxes(x).TreeView1.EndUpdate()

            Next

            If gCurrentNumberOfEffectsBoxesInUse = 0 Then
                AddAnEnhnacedTreeViewbox(1)
                EnhancedTreeViewBoxes(1).TreeView1.Nodes(0).Expand()
            End If

            EnhancedTreeViewBoxes(1).TreeView1.SelectedNode = Nothing

            'restore previousily built selections

            reloadUnderway = True

            For x = 1 To gCurrentNumberOfEffectsBoxesInUse

                EnhancedTreeViewBoxes(x).Visible = Not gEffectsBoxesAreHidden

                EnhancedTreeViewBoxes(x).TreeView1.Nodes(0).Expand()

                EnsureVisableAndOptionalySelelectATreeNode(gCurrentNamesOfEffectsInUse(x), EnhancedTreeViewBoxes(x).TreeView1, True)
                EnhancedTreeViewBoxes(x).TreeView1.Focus()

            Next

            reloadUnderway = False

            UpdateButtonAndToolTip()

            SetHeightOfEnhancedTreeBoxesOnScreen()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub SetHeightOfEnhancedTreeBoxesOnScreen()

        'set height to a constant height for all boxes

        Application.DoEvents()

        ' determine the maximum visiable entries across all open boxes
        Dim VisableEntries As Integer = 0
        For x = 1 To gCurrentNumberOfEffectsBoxesInUse
            EnhancedTreeViewBoxes(x).TreeView1.Nodes(0).Expand()
            Dim Total As Integer = CountVisableEntries(EnhancedTreeViewBoxes(x).TreeView1.Nodes(0))
            If Total > VisableEntries Then VisableEntries = Total
        Next

        Dim maximumHeight As Integer = Me.Height - 150
        Dim minumumHeight As Integer = 150

        Dim candidateHeight As Integer = Math.Min(Math.Max((VisableEntries + 3) * EnhancedTreeViewBoxes(1).TreeView1.ItemHeight * 1.25, minumumHeight), maximumHeight)

        Application.DoEvents()

        For x = 1 To gCurrentNumberOfEffectsBoxesInUse

            If enhancedControlOffset(x).X > 0 OrElse enhancedControlOffset(x).Y > 0 Then  'v1.6 testing here

            Else

                EnhancedTreeViewBoxes(x).Location = New Point(EnhancedTreeViewBoxes(x).Location.X - (x - 1), Math.Max(HeightToTheEffectControl + CInt((tsEffects.Height / 2)) - CInt(EnhancedTreeViewBoxes(x).Height / 2), 0))

                EnhancedTreeViewBoxes(x).TreeView1.Height = candidateHeight - 30

                If x = gCurrentNumberOfEffectsBoxesInUse Then
                    EnhancedTreeViewBoxes(x).TreeView1.Width = EnhancedTreeViewBoxes(x).Width - 15
                Else
                    EnhancedTreeViewBoxes(x).TreeView1.Width = EnhancedTreeViewBoxes(x).Width
                End If

                EnhancedTreeViewBoxes(x).Height = candidateHeight

            End If

        Next

        Application.DoEvents()

    End Sub
    Private Function CountVisableEntries(ByVal n As TreeNode) As Integer

        ' uses recursion

        Dim Count As Integer = 0

        If n.IsVisible Then
            Count += 1
        End If

        For Each cn As TreeNode In n.Nodes
            Count += CountVisableEntries(cn)
        Next

        Return Count

    End Function
    Private Sub EnhancedTreeViewBoxes_btnDone_Click(ByVal sender As Button, ByVal e As System.EventArgs)

        Dim OkToProceed As Boolean = True

        For x = 1 To gCurrentNumberOfEffectsBoxesInUse

            If gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse Then
                OkToProceed = False
                Exit For
            End If

        Next

        If OkToProceed Then

            For x = 1 To gCurrentNumberOfEffectsBoxesInUse
                EnhancedTreeViewBoxes(x).Visible = False
            Next
            gEffectsBoxesAreHidden = True

        Else

            Beep()

            Dim msg As String

            If gCurrentNumberOfEffectsBoxesInUse = 1 Then
                msg = "Please select a profile in the Effect Selection box, or click the red box in the top left of the Effect Selection Box to remove it."
            Else
                msg = "Please select a profile in each of the Effect Selection Boxes, or click the red box in the top left of each of the Effect Selection Boxes where you no longer want to select an Effect."
            End If

            Dim Dummy = MessageBox.Show(msg, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End If

        UpdateStatusBar()

    End Sub

    Private Sub EnhancedTreeViewBoxes_btnEffectAdd_Click(ByVal sender As Button, ByVal e As System.EventArgs)

        gEffectsBoxesAreHidden = False
        AddAnEnhnacedTreeViewbox(CInt(sender.Tag) + 1)
        FinalForm_Resize(Nothing, Nothing)  'v1.6 improved box creation

    End Sub

    Private Sub AddAnEnhnacedTreeViewbox(ByVal IndexOfControlToBeInsert As Integer)

        'make room for the new value
        For x As Integer = gMaxEffectsSupported To IndexOfControlToBeInsert + 1 Step -1
            gCurrentNamesOfEffectsInUse(x) = gCurrentNamesOfEffectsInUse(x - 1)
        Next

        gCurrentNamesOfEffectsInUse(IndexOfControlToBeInsert) = gDefaultEffectNotInUse

        gCurrentNumberOfEffectsBoxesInUse += 1

        UpdateButtonAndToolTip()

        ' when opening an enhnacetreeview box beyond the first an enhnacetreeview box,
        ' position (but do not select) the treeview for the new enhnacetreeview box at the same tree node as the prior one

        ' the following selects a node, but we don't what that at this point so clear it, rigth afterwards

        FinalForm_Resize(Nothing, Nothing)

        EnhancedTreeViewBoxes(IndexOfControlToBeInsert).ResizeControl(True)
        EnhancedTreeViewBoxes(IndexOfControlToBeInsert).TreeView1.SelectedNode = Nothing

        If IndexOfControlToBeInsert > 1 Then

            If EnhancedTreeViewBoxes(IndexOfControlToBeInsert - 1).TreeView1.SelectedNode IsNot Nothing Then

                If EnhancedTreeViewBoxes(IndexOfControlToBeInsert - 1).TreeView1.SelectedNode.Tag = gTreeViewFolderTagDesignation Then

                Else
                    EnhancedTreeViewBoxes(IndexOfControlToBeInsert - 1).TreeView1.SuspendLayout()
                    EnsureVisableAndOptionalySelelectATreeNode(EnhancedTreeViewBoxes(IndexOfControlToBeInsert - 1).TreeView1.SelectedNode.Tag, EnhancedTreeViewBoxes(IndexOfControlToBeInsert).TreeView1, False)
                    EnhancedTreeViewBoxes(IndexOfControlToBeInsert).TreeView1.SelectedNode = Nothing
                    EnhancedTreeViewBoxes(IndexOfControlToBeInsert - 1).TreeView1.ResumeLayout()
                End If

            End If

        End If

    End Sub

    Private Sub EnhancedTreeViewBoxes_btnEffectRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim IndexOfClickedControl As Integer = CInt(sender.tag)

        RemoveAnEffect_Part1(IndexOfClickedControl)
        RemoveAnEffect_Part2()

        If gCurrentNumberOfEffectsBoxesInUse > 0 Then

            PopUpEffectsSelectionBoxes()
            ' FinalForm_Resize(Nothing, Nothing)

        Else

            FreshLoadOfEnhancedTreeViewBoxes()

        End If

        gEffectsBoxesAreHidden = (gCurrentNumberOfEffectsBoxesInUse < 1)

        FinalForm_Resize(Nothing, Nothing)

    End Sub

    Private Sub RemoveAnEffect_Part1(ByVal IndexToBeRemoved As Integer)

        gCurrentNamesOfEffectsInUse(IndexToBeRemoved) = "*remove me*"
        gCurrentNumberOfEffectsBoxesInUse -= 1

        'build a new result containing only the items which were not removed

        Dim Result(gMaxEffectsSupported) As String

        Dim index As Integer = 0

        For x = 1 To gMaxEffectsSupported

            If gCurrentNamesOfEffectsInUse(x) = "*remove me*" Then
            Else
                index += 1
                Result(index) = gCurrentNamesOfEffectsInUse(x)
            End If

        Next

        For x = 1 To gCurrentNumberOfEffectsBoxesInUse
            gCurrentNamesOfEffectsInUse(x) = Result(x)
        Next

        For x = gCurrentNumberOfEffectsBoxesInUse + 1 To gMaxEffectsSupported
            gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse
        Next

        If gCurrentNumberOfEffectsBoxesInUse < 1 Then
            LoadNeutralEffectsProfile(False)
        End If

    End Sub

    Private Sub RemoveAnEffect_Part2()

        UpdateButtonAndToolTip()
        FinalForm_Resize(Nothing, Nothing)
        UpdateStatusBar()
        Application.DoEvents()

        UpdatePictureBox(DoAllPictureBoxTransformations)

    End Sub

    Private NavigatingTreeViewWithMouse As Boolean = False
    Private Sub EnhancedTreeViewBoxes_TreeView1_BeforeSelect(ByVal sender As TreeView, e As TreeViewCancelEventArgs)

        NavigatingTreeViewWithMouse = (User32.GetAsyncKeyState(Keys.LButton) > 0 And &H8000)

    End Sub

    Private Sub EnhancedTreeViewBoxes_TreeView1_AfterSelect(ByVal sender As TreeView, e As TreeViewEventArgs)

        If reloadUnderway Then Exit Sub

        If sender.SelectedNode.Tag = gTreeViewFolderTagDesignation Then

            If NavigatingTreeViewWithMouse Then

                If sender.SelectedNode.IsExpanded Then
                    sender.SelectedNode.Collapse()
                Else
                    sender.SelectedNode.Expand()
                End If

                sender.SelectedNode = Nothing

            Else

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

        Else

            gCurrentNamesOfEffectsInUse(CInt(sender.Tag)) = sender.SelectedNode.Tag

        End If

        If reloadUnderway Then Exit Sub

        UpdateButtonAndToolTip()

        UpdatePictureBox(DoAllPictureBoxTransformations)


    End Sub

    Private Sub EnhancedTreeViewBoxes_TreeView1_DoubleClick(ByVal sender As TreeView, e As MouseEventArgs)

        If sender.SelectedNode IsNot Nothing Then

            If sender.SelectedNode.Tag = gTreeViewFolderTagDesignation Then

            Else

                My.Settings.EffectsProfileName = ConvertXMLFileNameToProfileName(sender.SelectedNode.Tag)
                My.Settings.Save()

                Dim EffectBoxesAreVisable As Boolean = EnhancedTreeViewBoxes(1).Visible

                LoadAndProcessTheEffectsWindow()

                RebuildViewerScreen()

                If EffectBoxesAreVisable Then
                    PopUpEffectsSelectionBoxes()
                End If

            End If

        End If

    End Sub

    'Private Sub EnhancedTreeViewBoxes_lblHeader(sender As Object, e As EventArgs)

    '    Dim index As Integer = CInt(sender.text.replace("Effect", "").trim)

    '    EnhancedTreeViewBoxes(index).TreeView1.ExpandAll()

    'End Sub

    Private Sub UpdateButtonAndToolTip()

        If gCurrentNumberOfEffectsBoxesInUse > 0 Then

            Dim ButtonCanBeEnabled As Boolean = True

            For x = 1 To gCurrentNumberOfEffectsBoxesInUse

                If gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse Then
                    ButtonCanBeEnabled = False
                    Exit For
                End If

            Next

            ' EnhancedTreeViewBoxes(activeListBoxes).btnDone.Enabled = ButtonCanBeEnabled

            If ButtonCanBeEnabled Then

                EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).btnDone.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(120, Byte), Integer), CType(CType(215, Byte), Integer))

                If gCurrentNumberOfEffectsBoxesInUse = 1 Then
                    EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).ToolTip1.SetToolTip(EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).btnDone, "Click to apply the chosen effect")
                Else
                    EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).ToolTip1.SetToolTip(EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).btnDone, "Click to apply the chosen effects")
                End If

            Else

                EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).btnDone.BackColor = Color.LightSteelBlue

                If gCurrentNumberOfEffectsBoxesInUse = 1 Then
                    EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).ToolTip1.SetToolTip(EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).btnDone, "Please choose an effect")
                Else
                    EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).ToolTip1.SetToolTip(EnhancedTreeViewBoxes(gCurrentNumberOfEffectsBoxesInUse).btnDone, "Please choose one effect in each Effect box")
                End If

            End If

        End If

    End Sub

    Private Sub ChangeEffects()

        EffectsNow(gCorrectedImage)

        If Not gSuspendPictureBoxUpdates Then PictureBoxPrimary.Invalidate()

    End Sub

#End Region

#Region "Reset"

    Dim gResetDown As Boolean = False
    Private Sub tsReset_MouseDown(sender As Object, e As MouseEventArgs) Handles tsReset.MouseDown

        Const HoldTimeForResetToWork As Integer = 1000 ' one second

        Dim sw As New Diagnostics.Stopwatch
        sw.Reset()
        sw.Start()

        MouseIsDown = True
        While MouseIsDown

            Application.DoEvents()
            Threading.Thread.Sleep(10)

            If sw.ElapsedMilliseconds > HoldTimeForResetToWork Then
                Exit While
            End If

        End While

        If sw.ElapsedMilliseconds > HoldTimeForResetToWork Then

            movingPoint = New Point(0, 0)

            gOriginalImage = New Bitmap(gOriginalImageAsFirstLoaded)
            ResizeOriginalForRotation()
            movingPoint = gTweakedStartingLocationDueToResizeOriginalForRotation

            SetAllDefaults()

            UpdatePictureBox(DoNoPictureBoxTransformations)

            Beep()

        End If

        sw.Stop()

    End Sub

    Private Sub SetAllDefaults()

        gCurrentImageIsMirroredHorizontally = gCurrentImageIsMirroredHorizontallyDefault
        gCurrentImageIsMirroredVertically = gCurrentImageIsMirroredVerticallyDefault
        gCurrentBrightness = gBrightnessDefault
        gCurrentContrast = gContrastDefault
        gCurrentGamma = gGammaDefault
        gCurrentImageIsInGrayscale = gGrayscaleDefault
        gCurrentImageIsInverted = gInvertedDefault
        gCurrentSaturation = gSaturationDefault
        gCurrentRotation = gRotationDefault
        gCurrentZoom = gZoomDefault
        ResetEffects()

    End Sub

#End Region

#Region "Viewer"
    Private Sub tsViewer_Click(sender As Object, e As MouseEventArgs) Handles tsViewer.MouseDown

        tsViewer_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsViewer_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        If LeftMouseButtonDown OrElse ((e IsNot Nothing) AndAlso (e.Button = MouseButtons.Left)) Then

            gViewChoice = ViewingOption.PrimaryandBottom
            SetLookOfWindows()

        ElseIf MiddleMouseButtonDown OrElse ((e IsNot Nothing) AndAlso (e.Button = MouseButtons.Middle)) Then

            gViewChoice = ViewingOption.PrimaryOnly

        ElseIf RightMouseButtonDown OrElse ((e IsNot Nothing) AndAlso (e.Button = MouseButtons.Right)) Then

            gViewChoice = ViewingOption.PrimaryandRight
            SetLookOfWindows()

        End If

        'needs to be done twice to get the seperating line to appear
        'it will have been done once before for the PrimaryandRight and the PrimaryandBottom
        'it will now be done again for either of those, or for the first time for the PrimaryOnly windows
        SetLookOfWindows()

    End Sub

    Private Sub SetLookOfWindows()

        PictureBoxPrimary.Location = New Point(0, 0)
        Panel1.Location = New Point(0, 0)

        Dim ExtraWidthAvailableAsToolBarIsNotShowingText As Integer = 0
        If gShowingSplitContainerText Then
        Else
            ExtraWidthAvailableAsToolBarIsNotShowingText = 38
        End If

        Dim OverallContainer As Size = New Size(Me.Size.Width - 79 + ExtraWidthAvailableAsToolBarIsNotShowingText, Me.Size.Height - 58)

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

            Case = ViewingOption.OriginalOnly

                PictureBoxOriginal.Location = New Point(0, 0)

                PictureBoxRight.Visible = False
                PictureBoxBottom.Visible = False
                PictureBoxPrimary.Visible = False
                PictureBoxOriginal.Visible = True

                PictureBoxOriginal.Image = New Bitmap(gOriginalImage)
                PictureBoxOriginal.Size = OverallContainer

        End Select

        PictureBoxPrimary.Invalidate()

    End Sub

#End Region

#Region "Pin to top"

    Private IgnorePinToTop As Boolean = False

    Private Sub tsPinnedToTop_MouseDown(sender As Object, e As MouseEventArgs) Handles tsPinnedToTop.MouseDown

        If IgnorePinToTop Then Exit Sub

        IgnorePinToTop = True

        My.Settings.PinnedToTop = Not My.Settings.PinnedToTop
        My.Settings.Save()

        UpdatePinnedToTop()

        IgnorePinToTop = False

    End Sub

    Private Sub UpdatePinnedToTop()

        If My.Settings.PinnedToTop Then

            tsPinnedToTop.Image = Resources.Pin
            MakeTopMostWindow(Me.Handle.ToInt64, True)
            Me.BringToFront()

        Else

            tsPinnedToTop.Image = Resources.unpin
            MakeTopMostWindow(Me.Handle.ToInt64, True)
            Me.BringToFront()
            Application.DoEvents()
            MakeTopMostWindow(Me.Handle.ToInt64, False)

        End If

    End Sub

#End Region

#Region "A Ruler for Windows"
    Private Enum AR4WMode
        AsLastUsed = 0
        ReadingGuide = 1
        Ruler = 2
    End Enum

    Private Sub tsAR4W_MouseDown(sender As Object, e As MouseEventArgs) Handles tsAR4W.MouseDown

        tsAR4W_MouseDown_MyHandler(e, False, False, False)

    End Sub

    Private Sub tsAR4W_MouseDown_MyHandler(ByVal e As MouseEventArgs, ByVal LeftMouseButtonDown As Boolean, ByVal MiddleMouseButtonDown As Boolean, ByVal RightMouseButtonDown As Boolean)

        Cursor = Cursors.WaitCursor

        tsAR4W.Enabled = False

        Dim ARulerForWindowsIsRunning As Boolean = IsARulerForWindowsRunning()

        If MiddleMouseButtonDown OrElse (e.Button = MouseButtons.Middle) Then

            CloseAR4W()

        Else

            If LeftMouseButtonDown OrElse (e.Button = MouseButtons.Left) Then

                If ARulerForWindowsIsRunning Then CloseAR4W()
                Application.DoEvents()
                OpenAR4W(AR4WMode.ReadingGuide)

            ElseIf RightMouseButtonDown OrElse (e.Button = MouseButtons.Right) Then

                If ARulerForWindowsIsRunning Then CloseAR4W()
                Application.DoEvents()
                OpenAR4W(AR4WMode.Ruler)

            End If

        End If

        tsAR4W.Enabled = True

        Cursor = Cursors.Default

    End Sub

    Private Sub OpenAR4W(ByVal Mode As AR4WMode)

        Static Dim LastMode As AR4WMode

        Try

            Dim DPI As Integer = User32.GetDpiForWindow(Me.Handle)
            Dim Scale As Single = CSng(DPI) / CSng(96)

            If File.Exists(gARulerForWindows) Then

                'Inventory active screens
                Dim Screens As Screen() = Screen.AllScreens
                Dim MinX As Integer = 0
                Dim MinY As Integer = 0
                Dim MaxWidth As Integer = 0
                Dim MaxHeight As Integer = 0

                For x As Int16 = 0 To Screens.Length - 1
                    MinX = Math.Min(MinX, Screens(x).WorkingArea.X)
                    MinY = Math.Min(MinY, Screens(x).WorkingArea.Y)
                    MaxWidth += Screens(x).Bounds.Width
                    MaxHeight += Screens(x).Bounds.Height
                Next

                Dim PrimaryScreenHome = New Point(-MinX, -MinY)

                Dim ScalingFactor As Single = 1

                Dim ScaledLocation As Point = New Point(CInt(Me.Location.X * ScalingFactor), CInt(Me.Location.X * ScalingFactor))

                Dim ThisScreen As Screen = Screen.FromControl(Me)

                Dim TweakWidthForContainers_width As Integer = -105
                Dim TweakWidthForContainers_location As Integer = 8

                If gShowingSplitContainerText Then
                Else
                    TweakWidthForContainers_width = -50
                    TweakWidthForContainers_location = 20
                End If

                Dim DesiredWidth As Integer = CInt((Me.Size.Width + TweakWidthForContainers_width) * ScalingFactor)
                Dim DesiredLocationX As Integer = CInt((Me.Location.X + ToolStripView.Size.Width + TweakWidthForContainers_location) * ScalingFactor)
                Dim DesiredLocationY As Integer = CInt((Me.Location.Y + Me.Size.Height / 2) * ScalingFactor)
                Dim Parms As String = " Or=horizontal width=" & DesiredWidth.ToString.Trim & " location=(" & DesiredLocationX.ToString.Trim & "," & DesiredLocationY.ToString.Trim & ")"

                Select Case Mode

                    Case = AR4WMode.AsLastUsed

                        'need to set the mode in the parm so that the ruler can be Computeally closed when this window is closed
                        'however leave the orientation and lenght alone, this way the ruler/rg will open up as it was before
                        'i.e. just set the mode

                        If LastMode = AR4WMode.Ruler Then
                            Parms = " mode= ruler scale=True"
                        Else
                            Parms = " mode=rg scale=True"
                        End If

                    Case = AR4WMode.ReadingGuide

                        Parms &= " mode=rg scale=True"
                        LastMode = AR4WMode.ReadingGuide

                    Case = AR4WMode.Ruler

                        Parms &= " mode=ruler scale=True"
                        LastMode = AR4WMode.Ruler

                End Select


                'way 1
                System.Diagnostics.Process.Start(gARulerForWindows, Parms)


                'way 2 (not used)
                'Dim quote As String = Chr(34)

                'Dim TryThis As String = "REM this file was created by A Viewer for Windows" & vbCrLf & "start " & quote & quote & " " & quote & gARulerForWindows & quote & " " & quote & Parms & quote & vbCrLf & "exit"
                'Dim TempFilename As String = System.IO.Path.GetTempPath() & "av4wtemp.bat"

                'My.Computer.FileSystem.WriteAllText(TempFilename, TryThis, False)

                'Dim NewThread As Thread = New Thread(AddressOf OpenARuler_Async)
                'NewThread.Start(TempFilename)
                'NewThread.Start()


                'way 3 (not used)
                ''Dim myProcess As New Process
                ''    myProcess.EnableRaisingEvents = False

                ''    With myProcess.StartInfo

                ''        .CreateNoWindow = True
                ''        .WindowStyle = ProcessWindowStyle.Hidden
                ''        .UseShellExecute = True
                ''        .WorkingDirectory = Path.GetDirectoryName(TempFilename)
                ''        .FileName = TempFilename
                ''        ' If Parms.Length > 0 Then .Arguments = Parms
                ''        '.RedirectStandardOutput = False

                ''    End With

                ''    Dim ProcessStarted As Boolean = myProcess.Start()

                Thread.Sleep(1000)

            Else

                Beep()

                If MessageBox.Show("A Ruler For Windows does not appear to be installed on this computer." & vbCrLf & vbCrLf &
                                "To download your free copy, please visit:" & vbCrLf &
                                gARulerForWindowswebite & vbCrLf & vbCrLf &
                                "If you would like to do this now, just click 'Yes'.", gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then

                    System.Diagnostics.Process.Start(gARulerForWindowswebite)
                    Application.DoEvents()
                    Threading.Thread.Sleep(2000)

                End If

            End If

        Catch ex As Exception

        End Try

    End Sub

    'Public Enum DeviceCap As Integer

    '    ''' <summary>
    '    ''' Device driver version
    '    ''' </summary>
    '    DRIVERVERSION = 0
    '    ''' <summary>
    '    ''' Device classification
    '    ''' </summary>
    '    TECHNOLOGY = 2
    '    ''' <summary>
    '    ''' Horizontal size in millimeters
    '    ''' </summary>
    '    HORZSIZE = 4
    '    ''' <summary>
    '    ''' Vertical size in millimeters
    '    ''' </summary>
    '    VERTSIZE = 6
    '    ''' <summary>
    '    ''' Horizontal width in pixels
    '    ''' </summary>
    '    HORZRES = 8
    '    ''' <summary>
    '    ''' Vertical height in pixels
    '    ''' </summary>
    '    VERTRES = 10
    '    ''' <summary>
    '    ''' Number of bits per pixel
    '    ''' </summary>
    '    BITSPIXEL = 12
    '    ''' <summary>
    '    ''' Number of planes
    '    ''' </summary>
    '    PLANES = 14
    '    ''' <summary>
    '    ''' Number of brushes the device has
    '    ''' </summary>
    '    NUMBRUSHES = 16
    '    ''' <summary>
    '    ''' Number of pens the device has
    '    ''' </summary>
    '    NUMPENS = 18
    '    ''' <summary>
    '    ''' Number of markers the device has
    '    ''' </summary>
    '    NUMMARKERS = 20
    '    ''' <summary>
    '    ''' Number of fonts the device has
    '    ''' </summary>
    '    NUMFONTS = 22
    '    ''' <summary>
    '    ''' Number of colors the device supports
    '    ''' </summary>
    '    NUMCOLORS = 24
    '    ''' <summary>
    '    ''' Size required for device descriptor
    '    ''' </summary>
    '    PDEVICESIZE = 26
    '    ''' <summary>
    '    ''' Curve capabilities
    '    ''' </summary>
    '    CURVECAPS = 28
    '    ''' <summary>
    '    ''' Line capabilities
    '    ''' </summary>
    '    LINECAPS = 30
    '    ''' <summary>
    '    ''' Polygonal capabilities
    '    ''' </summary>
    '    POLYGONALCAPS = 32
    '    ''' <summary>
    '    ''' Text capabilities
    '    ''' </summary>
    '    TEXTCAPS = 34
    '    ''' <summary>
    '    ''' Clipping capabilities
    '    ''' </summary>
    '    CLIPCAPS = 36
    '    ''' <summary>
    '    ''' Bitblt capabilities
    '    ''' </summary>
    '    RASTERCAPS = 38
    '    ''' <summary>
    '    ''' Length of the X leg
    '    ''' </summary>
    '    ASPECTX = 40
    '    ''' <summary>
    '    ''' Length of the Y leg
    '    ''' </summary>
    '    ASPECTY = 42
    '    ''' <summary>
    '    ''' Length of the hypotenuse
    '    ''' </summary>
    '    ASPECTXY = 44
    '    ''' <summary>
    '    ''' Shading and Blending caps
    '    ''' </summary>
    '    SHADEBLENDCAPS = 45

    '    ''' <summary>
    '    ''' Logical pixels inch in X
    '    ''' </summary>
    '    LOGPIXELSX = 88
    '    ''' <summary>
    '    ''' Logical pixels inch in Y
    '    ''' </summary>
    '    LOGPIXELSY = 90

    '    ''' <summary>
    '    ''' Number of entries in physical palette
    '    ''' </summary>
    '    SIZEPALETTE = 104
    '    ''' <summary>
    '    ''' Number of reserved entries in palette
    '    ''' </summary>
    '    NUMRESERVED = 106
    '    ''' <summary>
    '    ''' Actual color resolution
    '    ''' </summary>
    '    COLORRES = 108

    '    ' Printing related DeviceCaps. These replace the appropriate Escapes

    '    ''' <summary>
    '    ''' Physical Width in device units
    '    ''' </summary>
    '    PHYSICALWIDTH = 110
    '    ''' <summary>
    '    ''' Physical Height in device units
    '    ''' </summary>
    '    PHYSICALHEIGHT = 111
    '    ''' <summary>
    '    ''' Physical Printable Area x margin
    '    ''' </summary>
    '    PHYSICALOFFSETX = 112
    '    ''' <summary>
    '    ''' Physical Printable Area y margin
    '    ''' </summary>
    '    PHYSICALOFFSETY = 113
    '    ''' <summary>
    '    ''' Scaling factor x
    '    ''' </summary>
    '    SCALINGFACTORX = 114
    '    ''' <summary>
    '    ''' Scaling factor y
    '    ''' </summary>
    '    SCALINGFACTORY = 115

    '    ''' <summary>
    '    ''' Current vertical refresh rate of the display device (for displays only) in Hz
    '    ''' </summary>
    '    VREFRESH = 116
    '    ''' <summary>
    '    ''' Vertical height of entire desktop in pixels
    '    ''' </summary>
    '    DESKTOPVERTRES = 117
    '    ''' <summary>
    '    ''' Horizontal width of entire desktop in pixels
    '    ''' </summary>
    '    DESKTOPHORZRES = 118
    '    ''' <summary>
    '    ''' Preferred blt alignment
    '    ''' </summary>
    '    BLTALIGNMENT = 119

    'End Enum

    'Private Function GetDisplayScalingFactor() As Single

    '    ' ref: https://stackoverflow.com/questions/5977445/how-to-get-windows-display-settings

    '    Dim returnValue As Single = 1

    '    Try



    '        'Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero)

    '        '    'Dim hdc As IntPtr = g.GetHdc()

    '        '    'For Each cap As DeviceCap In [Enum].GetValues(GetType(DeviceCap))
    '        '    '    Dim result As Integer = User32.GetDeviceCaps(hdc, cap)
    '        '    '    Console.WriteLine(String.Format("{0}: {1}", cap, result))
    '        '    'Next


    '        '    Dim Desktop As IntPtr = g.GetHdc()
    '        '    Dim LogicalScreenHeight As Integer = GDI32.GetDeviceCaps(Desktop, DeviceCap.VERTRES)
    '        '    Dim PhysicalScreenHeight As Integer = GDI32.GetDeviceCaps(Desktop, DeviceCap.DESKTOPVERTRES)

    '        '    Dim logpixelsy As Integer = GDI32.GetDeviceCaps(Desktop, DeviceCap.LOGPIXELSY)
    '        '    Dim screenScalingFactor As Single = CSng(PhysicalScreenHeight) / CSng(LogicalScreenHeight)

    '        '    Dim dpiScalingFactor As Single = CSng(logpixelsy) / CSng(96)

    '        '    'Console.WriteLine("Graphics.DpiX: " & g.DpiX)
    '        '    'Console.WriteLine("Graphics.DpiY: " & g.DpiY)
    '        '    'Console.WriteLine("Scaling factor: " & dpiScalingFactor)
    '        '    'Console.WriteLine()

    '        '    ReturnValue = dpiScalingFactor

    '        '    g.ReleaseHdc()

    '        'End Using



    '        'Dim dpiX, dpiY As Integer

    '        'Dim Graphics As Graphics = Me.CreateGraphics()
    '        'dpiX = Graphics.DpiX
    '        'dpiY = Graphics.DpiY
    '        'Graphics.Dispose()

    '        'MsgBox(dpiX)
    '        'Console.WriteLine(dpiY)


    '        'Dim screenList() As Screen = Screen.AllScreens

    '        'For Each screen In screenList

    '        '    Dim dm As User32.DEVMODE = New User32.DEVMODE()

    '        '    'dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

    '        '    User32.EnumDisplaySettings(screen.DeviceName, -1, dm)

    '        '    Dim scalingFactor As Single = Math.Round(Decimal.Divide(dm.dmPelsWidth, screen.Bounds.Width), 2)

    '        '    Console.WriteLine("dm.dmyResolution: " & dm.dmYResolution)
    '        '    Console.WriteLine("dm.dmPelsWidth: " & dm.dmPelsWidth)
    '        '    Console.WriteLine("screen.Bounds.Width: " & screen.Bounds.Width)
    '        '    Console.WriteLine("dm.dmscale: " & dm.dmScale)

    '        '    MsgBox("Scaling factor: " & scalingFactor)

    '        'Next





    '        'Dim gScreens As Screen()
    '        'Dim gMaxWidth As Integer = 0
    '        'Dim gMaxHeight As Integer = 0

    '        'Dim gMinX As Integer = 0
    '        'Dim gMinY As Integer = 0
    '        ''Inventory active screens
    '        'gScreens = Screen.AllScreens
    '        'For x As Int16 = 0 To gScreens.Length - 1
    '        '    gMinX = Math.Min(gMinX, gScreens(x).WorkingArea.X)
    '        '    gMinY = Math.Min(gMinY, gScreens(x).WorkingArea.Y)
    '        '    gMaxWidth += gScreens(x).Bounds.Width
    '        '    gMaxHeight += gScreens(x).Bounds.Height

    '        '    MsgBox(gScreens(x).WorkingArea.Width)
    '        'Next

    '        '' gPrimaryScreenHome = New Point(-gMinX, -gMinY)

    '        'MsgBox(Screen.FromControl(Me).Bounds.Width)
    '        'MsgBox(Screen.PrimaryScreen.Bounds.Width)
    '        'MsgBox(Windows.SystemParameters.PrimaryScreenWidth)
    '        'MsgBox(ReturnValue)



    '        ' ReturnValue = (CSng(100 * Screen.PrimaryScreen.Bounds.Width / Windows.SystemParameters.PrimaryScreenWidth)) / CSng(100)


    '        Dim DPI As Integer = User32.GetDpiForWindow(Me.Handle)

    '        ReturnValue = CSng(DPI) / CSng(96)

    '    Catch ex As Exception

    '    End Try

    '    Return ReturnValue

    'End Function

    'Private Async Sub OpenARuler_Async(ByVal data As Object)

    '    Try

    '        Dim TempFilename = data.ToString

    '        Dim myProcess As New Process
    '        myProcess.EnableRaisingEvents = False

    '        With myProcess.StartInfo

    '            .CreateNoWindow = True
    '            .WindowStyle = ProcessWindowStyle.Hidden
    '            .UseShellExecute = True
    '            .WorkingDirectory = Path.GetDirectoryName(TempFilename)
    '            .FileName = TempFilename

    '        End With

    '        Dim ProcessStarted As Boolean = myProcess.Start()

    '        Threading.Thread.Sleep(2000)

    '    Catch ex As Exception

    '    End Try

    'End Sub

#End Region

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

#End Region

#Region "Copy to clipboard"
    Private Sub tsCopy_MouseDown(sender As Object, e As MouseEventArgs) Handles tsCopy.MouseDown

        If gCurrentZoom = 0 Then
            Clipboard.SetImage(gCorrectedImage)
        Else

            Dim tempImg As Bitmap = New Bitmap(TrimCorrectedImageForSaving)

            If gCurrentRotation = 0 Then
            Else

                If gCurrentEffectsProfile.TransparencyEnabled Then
                    tempImg.MakeTransparent(Color.FromArgb(gCurrentEffectsProfile.TransparencyRed, gCurrentEffectsProfile.TransparencyGreen, gCurrentEffectsProfile.TransparencyBlue))
                End If
                'SmoothJaggedEdges(tempImg, True) ' transparent
                'SmoothJaggedEdges(tempImg, False) ' borrow

            End If

            Clipboard.SetImage(tempImg)
            tempImg.Dispose()
        End If

    End Sub


#End Region

#Region "Save"
    Private Sub tsSave_MouseDown(sender As Object, e As MouseEventArgs) Handles tsSave.MouseDown

        If My.Settings.PinnedToTop Then
            MakeTopMostWindow(Me.Handle, False)
        End If

        Try

            Dim extension As String = System.IO.Path.GetExtension(gFilePathNameAndExtention)

            Dim NewFileName As String

            If extension.Length = 0 Then
                extension = ".jpg"
                NewFileName = gFilePathNameAndExtention & " adjusted by A Viewer for Windows" & extension
            Else
                NewFileName = gFilePathNameAndExtention.Remove(gFilePathNameAndExtention.Length - extension.Length) & " adjusted by A Viewer for Windows" & extension
            End If

            Dim FilterIndex As Integer = 0

            ' Find numbeer to be used as the filter index in the save dialogue window by matching on the extention type
            Dim working = gValidFileTypesReadingFilter.Split("|"c)
            Dim workingindex As Integer = 0

            For x = 2 To working.Length - 1 Step 2
                workingindex += 1
                If working(x).Contains(extension) Then
                    FilterIndex = workingindex
                    Exit For
                End If
            Next

            If Directory.Exists(My.Settings.DefaultSaveDirectory) Then
            Else
                My.Settings.DefaultSaveDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop
                My.Settings.Save()
            End If

            Dim SaveFileDialog As New SaveFileDialog()
            With SaveFileDialog
                .InitialDirectory = My.Settings.DefaultSaveDirectory
                .FileName = System.IO.Path.GetFileNameWithoutExtension(NewFileName)
                .Filter = gValidFileTypesWritingFilter
                .FilterIndex = FilterIndex
                .RestoreDirectory = True
            End With

            If SaveFileDialog.ShowDialog() = DialogResult.OK Then

                Me.Cursor = Cursors.WaitCursor
                Application.DoEvents()

                Dim SaveFileName As String = SaveFileDialog.FileName

                Dim DesiredFileType As String = IO.Path.GetExtension(SaveFileName).ToLower

                Dim tempImg As Bitmap = New Bitmap(TrimCorrectedImageForSaving)

                If (DesiredFileType = ".png") OrElse (DesiredFileType = ".gif") Then
                Else

                    Dim transparencyFound As Boolean
                    Dim opacityFound As Boolean

                    ImageCheckForTransparencyAndOpactiy(tempImg, transparencyFound, opacityFound)

                    If transparencyFound OrElse opacityFound Then

                        Dim msg As String = "The image you are about to save "

                        If transparencyFound AndAlso opacityFound Then
                            msg &= "is opaque and also contains a transparent colour."
                        Else
                            If transparencyFound Then
                                msg &= "contains a transparent colour."
                            Else
                                msg &= "is opaque."
                            End If
                        End If

                        msg &= vbCrLf & vbCrLf & "However, saving an image with "

                        If transparencyFound AndAlso opacityFound Then
                            msg &= "these features"
                        Else
                            msg &= "this feature"
                        End If

                        msg &= " is unsupported by the chosen file type of '" & DesiredFileType.Remove(0, 1) & "'." & vbCrLf & vbCrLf &
                        "Would you rather save this file using a file type of 'png' which does support "

                        If transparencyFound AndAlso opacityFound Then
                            msg &= "these features?"
                        Else
                            msg &= "this feature?"
                        End If

                        If gColourLiteral = "Color" Then
                            msg = msg.Replace("colour", "color")
                        End If

                        If MessageBox.Show(msg, gThisProgramName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then

                            SaveFileName = SaveFileName.Replace(DesiredFileType, ".png")
                            DesiredFileType = ".png"

                            If File.Exists(SaveFileName) Then

                                If MessageBox.Show(Path.GetFileName(SaveFileName) & " already exists." & vbCrLf & "Do you want to replace it?", "Confirm Save As", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.No Then
                                    Exit Try
                                End If
                            End If

                        End If

                    End If

                End If


                Select Case DesiredFileType

                    Case Is = ".bmp"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Bmp)

                    Case Is = ".emf"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Emf)

                    Case Is = ".exif"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Exif)

                    Case Is = ".gif"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Gif)

                    Case Is = ".icon"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Icon)

                    Case Is = ".jpg"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Jpeg)

                    Case Is = ".png"

                        If gCurrentRotation = 0 Then
                            tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Png)
                        Else
                            'save with transparancy
                            Dim rect As Rectangle = New Rectangle(0, 0, tempImg.Width, tempImg.Height)
                            Dim output As Bitmap = tempImg.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                            If gCurrentEffectsProfile.TransparencyEnabled Then
                                output.MakeTransparent(Color.FromArgb(gCurrentEffectsProfile.TransparencyRed, gCurrentEffectsProfile.TransparencyGreen, gCurrentEffectsProfile.TransparencyBlue))
                            End If
                            'SmoothJaggedEdges(output, False)
                            output.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Png)
                            output.Dispose()
                        End If

                    Case Is = ".tiff"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Tiff)

                    Case Is = ".wmf"
                        tempImg.Save(SaveFileName, System.Drawing.Imaging.ImageFormat.Wmf)

                    Case Else

                        If gValidFileTypesForWriting.Contains(DesiredFileType) Then

                            ' SaveUsingMagicK(SaveFileName)

                        Else

                            Beep()

                            Dim msg As String = gThisProgramName & " is unable to save files having a file type of " & DesiredFileType & vbCrLf & vbCrLf & "The file types which " & gThisProgramName & " can save are:" & vbCrLf
                            For Each vft In gValidFileTypesForWriting
                                msg &= vft & " "
                            Next

                            MessageBox.Show(msg, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                        End If

                End Select

                tempImg.Dispose()

                My.Settings.DefaultSaveDirectory = IO.Path.GetDirectoryName(SaveFileName)
                My.Settings.Save()

                tempImg.Dispose()

                Me.Cursor = Cursors.Default

            End If

        Catch ex As Exception

            Me.Cursor = Cursors.Default

        End Try

        If My.Settings.PinnedToTop Then
            MakeTopMostWindow(Me.Handle, True)
        End If

        GarbageCollect()

    End Sub

    Private Sub SaveUsingMagicK(ByVal filename As String)

        Dim MagickReadingSettings = New MagickReadSettings

        Dim Defines = New Formats.DngReadDefines
        Defines.UseCameraWhitebalance = True
        Defines.UseAutoWhitebalance = True
        Defines.DisableAutoBrightness = True

        MagickReadingSettings.Defines = Defines

        Using image = New MagickImage(gFilePathNameAndExtention, MagickReadingSettings)

            image.Quality = 100

            image.Settings.Compression = CompressionMethod.NoCompression
            image.Format = MagickFormat.Rgb

            Using memStream = New MemoryStream
                'image.Write(memStream)
                'gOriginalImage = New Bitmap(memStream)
                image.Write("c:\temp\converted_with_magickimage.rgb")
            End Using


        End Using

    End Sub

#End Region

#Region "Temp Hide Ruler"

    Private Sub TempHideAR4W(ByVal Hide As Boolean)

        Static Dim WasARulerForWindowsRunningAtTheTimeOfTheRequest As Boolean

        If Hide Then

            WasARulerForWindowsRunningAtTheTimeOfTheRequest = IsARulerForWindowsRunning()

            If WasARulerForWindowsRunningAtTheTimeOfTheRequest Then
                CloseAR4W()
            End If

            tsPinnedToTop.Image = Resources.unpin
            MakeTopMostWindow(Me.Handle.ToInt64, True)
            Me.BringToFront()
            Application.DoEvents()
            MakeTopMostWindow(Me.Handle.ToInt64, False)

        Else

            If WasARulerForWindowsRunningAtTheTimeOfTheRequest Then
                OpenAR4W(AR4WMode.AsLastUsed)
            End If

            UpdatePinnedToTop()

        End If

    End Sub

#End Region

#Region "Panning"

    Private startingPoint As Point = New Point(0, 0)
    Private movingPoint As Point = New Point(0, 0)
    Private panningUnderway As Boolean = False
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

            If (CropToolClicksAreNeeded > 0) OrElse (LevelingToolClicksAreNeeded > 0) Then

                panningUnderway = False
                CropMovingPoint(1) = New Point(e.Location.X - movingPoint.X, e.Location.Y - movingPoint.Y)

            Else

                panningUnderway = True
                startingPoint = New Point(e.Location.X - movingPoint.X, e.Location.Y - movingPoint.Y)
                Cursor = Cursors.Hand

            End If

        End If

    End Sub

    Private Sub pictureBox1_MouseUp2(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBoxPrimary.MouseUp, PictureBoxRight.MouseUp, PictureBoxBottom.MouseUp, PictureBoxOriginal.MouseUp

        panningUnderway = False
        viewingOriginalOnly = False

        If (CropToolClicksAreNeeded > 0) OrElse (LevelingToolClicksAreNeeded > 0) Then

            CropMovingPoint(2) = New Point(e.Location.X - movingPoint.X, e.Location.Y - movingPoint.Y)

        Else

            Cursor = Cursors.Default

        End If

    End Sub
    Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles PictureBoxPrimary.MouseLeave, PictureBoxRight.MouseLeave, PictureBoxBottom.MouseLeave, PictureBoxOriginal.MouseLeave

        viewingOriginalOnly = False

    End Sub

    Private gRedefiningWindowsUnderway As Boolean = False
    Private Sub pictureBox1_MouseMove2(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBoxPrimary.MouseMove, PictureBoxBottom.MouseMove, PictureBoxRight.MouseMove

        If gRedefiningWindowsUnderway Then Exit Sub

        Try

            If (CropToolClicksAreNeeded > 0) OrElse (LevelingToolClicksAreNeeded > 0) Then

                panningUnderway = False

            Else

                If panningUnderway Then
                    movingPoint = New Point(e.Location.X - startingPoint.X, e.Location.Y - startingPoint.Y)
                    PictureBoxPrimary.Invalidate()

                End If

            End If

        Catch ex As Exception

        End Try

    End Sub
    Private Sub PictureBoxPrimary_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles PictureBoxPrimary.Paint

        If initialLoadUnderway Then Exit Sub

        e.Graphics.DrawImage(gCorrectedImage, movingPoint)

        RefreshShadowBoxWindow()

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


        If (gCurrentZoom = gZoomDefault) AndAlso (gCurrentRotation = gRotationDefault) Then

            e.Graphics.DrawImage(gOriginalImage, movingPoint)

        Else

            Dim tempimg As New Bitmap(RotateImage(gOriginalImage, gCurrentRotation))

            If gCurrentZoom = gZoomDefault Then
            Else
                tempimg = New Bitmap(tempimg, Convert.ToInt32(tempimg.Width * gCurrentZoom), Convert.ToInt32(tempimg.Height * gCurrentZoom))
            End If

            e.Graphics.DrawImage(tempimg, movingPoint)

            tempimg.Dispose()

            GarbageCollect()

        End If

    End Sub


#End Region

#Region "Split Container control"

    Const SplitContainer1_MinWidth As Integer = 25
    Const SplitContainer1_ThresholdForIconsOnly As Integer = 30
    Private SplitContainer1_MaxWidth As Integer = 86

    Private gShowingSplitContainerText As Boolean = True
    Private gCurrentSplitContainerWidth As Integer = -1

    Dim IgnorChangesToSplitContainerWidth As Boolean = False
    Private Sub SplitContainer1_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles SplitContainer1.SplitterMoved

        Static Dim LastWindowsState As Windows.WindowState = WindowState.Maximized

        If (Me.WindowState = WindowState.Maximized) AndAlso (LastWindowsState = WindowState.Normal) Then
            IgnorChangesToSplitContainerWidth = True
        End If

        LastWindowsState = Me.WindowState

        If IgnorChangesToSplitContainerWidth Then Exit Sub

        IgnorChangesToSplitContainerWidth = True

        If SplitContainer1.Panel1.Width >= SplitContainer1_ThresholdForIconsOnly Then

            SplitContainer1.SplitterDistance = SplitContainer1_MaxWidth
            UpdateLookOfSplitContainer(True, False)

        ElseIf SplitContainer1.Panel1.Width <= SplitContainer1_MinWidth Then

            SplitContainer1.SplitterDistance = SplitContainer1_MinWidth
            UpdateLookOfSplitContainer(False, False)

        End If

        IgnorChangesToSplitContainerWidth = False

    End Sub

    Private Sub UpdateLookOfSplitContainer(ByVal ShowText As Boolean, ByVal ForceChange As Boolean)

        IgnorChangesToSplitContainerWidth = True

        If ShowText Then

            If ForceChange OrElse (Not gShowingSplitContainerText) Then

                gShowingSplitContainerText = True
                SplitContainer1.SplitterDistance = SplitContainer1_MaxWidth

                For Each item As ToolStripItem In ToolStripView.Items
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

                For Each item As ToolStripItem In ToolStripView.Items
                    item.DisplayStyle = ToolStripItemDisplayStyle.Image
                Next

            End If

        End If

        IgnorChangesToSplitContainerWidth = False


    End Sub


#End Region

#Region "Common stuff for this window"

    Private MouseIsDown As Boolean = False

    Private Sub Dummy(sender As Object, e As System.EventArgs) Handles tsBrightness.MouseUp, tsBrightness.MouseEnter, tsBrightness.MouseLeave,
                                                                       tsContrast.MouseUp, tsContrast.MouseEnter, tsContrast.MouseLeave,
                                                                       tsGamma.MouseUp, tsGamma.MouseEnter, tsGamma.MouseLeave,
                                                                       tsMirror.MouseUp, tsMirror.MouseEnter, tsMirror.MouseLeave,
                                                                       tsInvert.MouseUp, tsInvert.MouseEnter, tsInvert.MouseLeave,
                                                                       tsSaturation.MouseUp, tsSaturation.MouseEnter, tsSaturation.MouseLeave,
                                                                       tsEffects.MouseUp, tsEffects.MouseEnter, tsEffects.MouseLeave,
                                                                       tsRotate.MouseUp, tsRotate.MouseEnter, tsRotate.MouseLeave,
                                                                       tsFit.MouseUp, tsFit.MouseEnter, tsFit.MouseLeave,
                                                                       tsZoom.MouseUp, tsZoom.MouseEnter, tsZoom.MouseLeave,
                                                                       tsReset.MouseUp, tsReset.MouseEnter, tsReset.MouseLeave

        MouseIsDown = False

    End Sub

    Private Sub UpdatePictureBox(ByVal Transformations As PictureBoxTransformationsStructure)

        Cursor = Cursors.WaitCursor

        Try

            If (CropToolClicksAreNeeded > 0) OrElse (LevelingToolClicksAreNeeded > 0) Then Exit Sub

            gCorrectedImage = New Bitmap(gOriginalImage)

            ' Mirror

            If Transformations.ChangeMirroredHorizontal AndAlso gCurrentImageIsMirroredHorizontally AndAlso Transformations.ChangeMirroredVertical AndAlso gCurrentImageIsMirroredVertically Then

                FlipHorizontalAndVertical()

            Else

                If Transformations.ChangeMirroredHorizontal AndAlso gCurrentImageIsMirroredHorizontally Then
                    MirrorHorizontally()
                End If

                If Transformations.ChangeMirroredVertical AndAlso gCurrentImageIsMirroredVertically Then
                    MirrorVertically()
                End If

            End If

            ' for best quality work on the largest image possible
            ' so enlarge it here zoom >=0
            ' otherwise shrink it as the last step further below

            If gCurrentZoom >= 1 Then

                If Transformations.ChangeZoom Then
                    If gCurrentZoom = gZoomDefault Then
                    Else
                        ChangeZoom(gCurrentZoom, ChangeTypeEnum.Absolute)
                    End If
                Else
                    gCurrentZoom = gZoomDefault
                End If

            End If

            ' brightness, contrast, gama

            If Transformations.ChangeBrightness OrElse Transformations.ChangeContrast OrElse Transformations.ChangeGamma Then

                If (gCurrentBrightness = gBrightnessDefault) AndAlso (gCurrentContrast = gContrastDefault) AndAlso (gCurrentGamma = gGammaDefault) Then
                Else

                    If Not Transformations.ChangeBrightness Then gCurrentBrightness = gBrightnessDefault
                    If Not Transformations.ChangeContrast Then gCurrentContrast = gContrastDefault
                    If Not Transformations.ChangeGamma Then gCurrentGamma = gGammaDefault

                    ChangeBrightnessContrastGamma(gCurrentBrightness, gCurrentContrast)

                End If

            End If

            ' saturation

            If Transformations.ChangeSaturation Then
                If gCurrentSaturation = gSaturationDefault Then
                Else
                    ChangeSaturation(gCurrentSaturation, ChangeTypeEnum.Absolute)
                End If
            Else
                gCurrentSaturation = gSaturationDefault
            End If

            ' invert

            If Transformations.ChangeInvert AndAlso gCurrentImageIsInverted Then
                Invert()
            End If

            ' grayscale

            If Transformations.ChangeGrayScale AndAlso gCurrentImageIsInGrayscale Then
                ConvertToGrayscale()
            End If

            ' effects

            If Transformations.ChangeEffects Then
                ChangeEffects()
            End If

            ' rotation - done after effects so as not to influence effects

            If Transformations.ChangeRotation Then
                If gCurrentRotation = gRotationDefault Then
                Else
                    ChangeRotation(gCurrentRotation, ChangeTypeEnum.Absolute)
                End If
            Else
                gCurrentRotation = gRotationDefault
            End If

            ' zoom

            If gCurrentZoom < 1 Then

                If Transformations.ChangeZoom Then
                    If gCurrentZoom = gZoomDefault Then
                    Else
                        ChangeZoom(gCurrentZoom, ChangeTypeEnum.Absolute)
                    End If
                Else
                    gCurrentZoom = gZoomDefault
                End If

            End If

            Application.DoEvents()

            If Transformations.UpdatePicturebox Then
                PictureBoxPrimary.Invalidate()
            End If

            Application.DoEvents()

            GarbageCollect()

        Catch ex As Exception

        End Try

        Cursor = Cursors.Default

    End Sub
    Private Sub ResizeOriginalForRotation()

        Dim Hypotenuse As Integer = CInt(Math.Sqrt(gOriginalImage.Width ^ 2 + gOriginalImage.Height ^ 2)) + 1

        gTweakedStartingLocationDueToResizeOriginalForRotation = New Point(-1 * (CInt(Hypotenuse / 2) - CInt(gOriginalImage.Width / 2)), -1 * (CInt(Hypotenuse / 2) - CInt(gOriginalImage.Height / 2)))

        Dim UpdatedBitmap = New Bitmap(Hypotenuse, Hypotenuse, PixelFormat.Format32bppArgb)

        Dim srcRect As Rectangle = New Rectangle(0, 0, gOriginalImage.Width, gOriginalImage.Height)

        Dim destRect As Rectangle = New Rectangle(CInt(Hypotenuse / 2) - CInt(gOriginalImage.Width / 2), CInt(Hypotenuse / 2) - CInt(gOriginalImage.Height / 2), gOriginalImage.Width, gOriginalImage.Height)

        GarbageCollect()

        Using g As Graphics = Graphics.FromImage(UpdatedBitmap)

            g.Clear(gMyTransparencyColour)  ' add transparent background
            g.DrawImage(gOriginalImage, destRect, srcRect, GraphicsUnit.Pixel)

        End Using

        GarbageCollect()

        gOriginalImage = New Bitmap(UpdatedBitmap)

        UpdatedBitmap.Dispose()
        GarbageCollect()

    End Sub

    Private Function TrimCorrectedImageForSaving() As Image

        Dim UpdatedBitmap As Image

        Dim format As PixelFormat = gOriginalImage.PixelFormat

        Dim CutOutRectCorrected As Rectangle = GetTrimmedBoardersFromBitmap(New Bitmap(gCorrectedImage))

        Dim CutOutBitMap As Bitmap = gCorrectedImage.Clone(CutOutRectCorrected, format)

        UpdatedBitmap = New Bitmap(CutOutBitMap)

        CutOutBitMap.Dispose()

        GarbageCollect()

        Return UpdatedBitmap

    End Function

    Private Sub UpdateStatusBar() Handles PictureBoxPrimary.Paint

        If gSuspendPictureBoxUpdates Then Exit Sub

        If gCurrentImageIsMirroredHorizontally Then

            If gCurrentImageIsMirroredVertically Then

                Me.ToolStripStatusMirrored.Text = "Mirrored horizontally and vertically"

            Else

                Me.ToolStripStatusMirrored.Text = "Mirrored horizontally"

            End If

        Else

            If gCurrentImageIsMirroredVertically Then

                Me.ToolStripStatusMirrored.Text = "Mirrored vertically"

            Else

                Me.ToolStripStatusMirrored.Text = "Not mirrored"

            End If

        End If

        Me.ToolStripStatusAngle.Text = "Rotated by " & (-1 * CInt(gCurrentRotation * 100) / 100).ToString("F1") & Chr(186)

        Me.ToolStripStatusZoom.Text = "Zoom " & CInt(gCurrentZoom * 100) & "% "

        Me.ToolStripStatusInverted.Text = IIf(gCurrentImageIsInverted, "Inverted        ", "Not inverted")

        Me.ToolStripStatusBrightness.Text = "Brightness " & (((gCurrentBrightness * 100) + 100) / 2).ToString("F1") & "% "

        Me.ToolStripStatusContrast.Text = "Contrast " & ((gCurrentContrast / 3) * 100).ToString("F1") & "% "

        Me.ToolStripStatusGamma.Text = "Gamma " & (((gCurrentGamma - 0.01) / 6.98) * 100).ToString("F1") & "% "

        Me.ToolStripStatusSaturation.Text = "Saturation " & (gCurrentSaturation * 100).ToString("F1") & "%"

        Me.ToolStripStatusGrayscale.Text = IIf(gCurrentImageIsInGrayscale, "Grayscale on ", "Grayscale off")

        Dim effectsStatus As String

        Dim effectsInUse As Integer = 0
        For x = 1 To gCurrentNumberOfEffectsBoxesInUse
            If gCurrentNamesOfEffectsInUse(x) <> gDefaultEffectNotInUse Then effectsInUse += 1
        Next

        Select Case effectsInUse
            Case = 0
                effectsStatus = "No effects are in use"
            Case = 1
                effectsStatus = "One effect is in use"
            Case = 2
                effectsStatus = "Two effects are in use"
            Case = 3
                effectsStatus = "Three effects are in use"
            Case = 4
                effectsStatus = "Four effects are in use"
            Case = 5
                effectsStatus = "Five effects are in use"
        End Select

        Me.ToolStripStatusEffects.Text = effectsStatus

    End Sub

    Private lock As New Object

    Private CropMovingPoint(2) As Point
    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBoxPrimary.MouseMove

        SyncLock lock

            If LevelingToolClicksAreNeeded = 2 Then

                PictureBoxPrimary.Refresh()

                Dim graphics1 As Graphics = PictureBoxPrimary.CreateGraphics()
                graphics1.DrawLine(MyDrawingPen, LevelingToolPoints(1), e.Location)
                graphics1.Dispose()

            ElseIf CropToolClicksAreNeeded = 2 Then

                Dim LeftMostPoint As New Point
                Dim RightMostPoint As New Point

                Dim TopMostPoint As New Point
                Dim BottomMostPoint As New Point

                If CropToolPoints(1).X < e.X Then
                    LeftMostPoint = CropToolPoints(1)
                    RightMostPoint = e.Location
                Else
                    LeftMostPoint = e.Location
                    RightMostPoint = CropToolPoints(1)
                End If

                If CropToolPoints(1).Y < e.Y Then
                    TopMostPoint = CropToolPoints(1)
                    BottomMostPoint = e.Location
                Else
                    TopMostPoint = e.Location
                    BottomMostPoint = CropToolPoints(1)
                End If

                Dim width As Integer = RightMostPoint.X - LeftMostPoint.X
                Dim height As Integer = BottomMostPoint.Y - TopMostPoint.Y

                Dim CropBox As Rectangle = New Rectangle(LeftMostPoint.X, TopMostPoint.Y, width, height)

                PictureBoxPrimary.Refresh()

                Dim graphics1 As Graphics = PictureBoxPrimary.CreateGraphics()
                graphics1.DrawRectangle(MyDrawingPen, CropBox)
                graphics1.Dispose()

            End If

        End SyncLock

    End Sub

    Private Sub FinalForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown, PictureBoxPrimary.KeyDown

        If e.KeyCode = Keys.Escape Then

            If CropToolClicksAreNeeded > 0 Then
                CropToolClicksAreNeeded = 0
                tsCrop.ForeColor = UnSelectedTool
                PictureBoxPrimary.Refresh()
                Cursor = Cursors.Default

            ElseIf LevelingToolClicksAreNeeded > 0 Then
                LevelingToolClicksAreNeeded = 0
                tsLevel.ForeColor = UnSelectedTool
                PictureBoxPrimary.Refresh()
                Cursor = Cursors.Default
            End If

        End If

    End Sub

    Private Sub FinalForm_Shown(sender As Object, e As EventArgs) Handles Me.SizeChanged

        If Me.WindowState = WindowState.Normal Then

            If My.Settings.PinnedToTop Then
            Else
                MakeTopMostWindow(Me.Handle.ToInt64, True)
                Me.BringToFront()
                Application.DoEvents()
                MakeTopMostWindow(Me.Handle.ToInt64, False)
            End If

        End If

        UpdateLookOfSplitContainer(gShowingSplitContainerText, True)

    End Sub

    Private Sub FinalForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        RemoveHandler AnAV4WNotificationEvent, AddressOf ReactToAnAV4WNotificationEvent

        ' RemoveHandler SliderControl.TextBox1.TextChanged, AddressOf SliderControl_TextBox1_TextChanged

        For x = 1 To gMaxEffectsSupported

            If EnhancedTreeViewBoxes(x) IsNot Nothing Then 'v1.8

                RemoveHandler EnhancedTreeViewBoxes(x).btnEffectAdd.Click, AddressOf EnhancedTreeViewBoxes_btnEffectAdd_Click
                RemoveHandler EnhancedTreeViewBoxes(x).btnEffectRemove.Click, AddressOf EnhancedTreeViewBoxes_btnEffectRemove_Click
                RemoveHandler EnhancedTreeViewBoxes(x).btnDone.Click, AddressOf EnhancedTreeViewBoxes_btnDone_Click

                RemoveHandler EnhancedTreeViewBoxes(x).TreeView1.BeforeSelect, AddressOf EnhancedTreeViewBoxes_TreeView1_BeforeSelect
                RemoveHandler EnhancedTreeViewBoxes(x).TreeView1.AfterSelect, AddressOf EnhancedTreeViewBoxes_TreeView1_AfterSelect
                RemoveHandler EnhancedTreeViewBoxes(x).TreeView1.DoubleClick, AddressOf EnhancedTreeViewBoxes_TreeView1_DoubleClick

                RemoveHandler EnhancedTreeViewBoxes(x).MouseDown, AddressOf EnhancedTreeViewBoxes_MouseDown
                RemoveHandler EnhancedTreeViewBoxes(x).MouseUp, AddressOf EnhancedTreeViewBoxes_MouseUp
                RemoveHandler EnhancedTreeViewBoxes(x).MouseMove, AddressOf EnhancedTreeViewBoxes_MouseMove

            End If

        Next

        CloseAR4W()

        My.Settings.ShowingSplitContainerText = gShowingSplitContainerText
        My.Settings.ViewChoice = gViewChoice
        My.Settings.Save()

        Me.Hide()

        gFilePathNameAndExtention = String.Empty
        gOriginalImage = Nothing
        PictureBoxPrimary.Dispose()

    End Sub

#End Region

#Region "Code to deal with clicking on a control and the form becoming active at the same time"

    Const VK_LBUTTON = &H1
    Const VK_RBUTTON = &H2
    Const VK_MBUTTON = &H4

    Private Sub FinalForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated

        If frmPickEffectsMatrixOpen Then Exit Sub

        Select Case DidTheUserClickInTheControlBox()

            Case = ControlBoxClick.ClickRedX
                Me.Close()
                Exit Sub

            Case = ControlBoxClick.ClickMaximize
                Me.WindowState = FormWindowState.Maximized
                Exit Sub

            Case = ControlBoxClick.ClickMinimize
                Me.WindowState = FormWindowState.Minimized
                Exit Sub

        End Select

        For Each tsItem As ToolStripItem In ToolStripView.Items

            Dim tsbutton = New ToolStripButton
            tsbutton = TryCast(tsItem, ToolStripButton)

            If MouseIsOverToolStripButton(tsbutton) Then

                Dim LeftMouseButtonDown As Boolean = (User32.GetAsyncKeyState(VK_LBUTTON) <> 0)
                Dim MiddleMouseButtonDown As Boolean = (User32.GetAsyncKeyState(VK_MBUTTON) <> 0)
                Dim RightMouseButtonDown As Boolean = (User32.GetAsyncKeyState(VK_RBUTTON) <> 0)

                e = New MouseEventArgs(New MouseButtons(), 0, 0, 0, 0)

                Select Case tsItem.Name

                    Case = "tsLevel"
                        tsLevel_MouseDown(Nothing, e)

                    Case = "tsRotate"
                        tsRotate_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsFit"
                        tsFit_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsZoom"
                        tsZoom_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsCrop"
                        tsCrop_MouseDown(Nothing, e)

                    Case = "tsInvert"
                        tsInvert_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsBrightness"
                        tsBrightness_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsContrast"
                        tsContrast_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsGamma"
                        tsGamma_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsSaturation"
                        tsSaturation_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsGrayscale"
                        tsGrayScale_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsEffects"
                        tsEffects_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsReset"
                        tsReset_MouseDown(Nothing, e)

                    Case = "tsViewer"
                        tsViewer_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsPinnedToTop"
                        tsPinnedToTop_MouseDown(Nothing, e)

                    Case = "tsAR4W"
                        tsAR4W_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsMagnify"
                        tsMagnify_MouseDown_MyHandler(e, LeftMouseButtonDown, MiddleMouseButtonDown, RightMouseButtonDown)

                    Case = "tsCopy"
                        tsCopy_MouseDown(Nothing, e)

                    Case = "tsSave"
                        tsSave_MouseDown(Nothing, e)

                    Case = "tsHelp"
                        tsHelp_MouseDown(Nothing, e)

                    Case Else
                        Beep()
                End Select

            End If

        Next

    End Sub

    Private Enum ControlBoxClick
        ClickOutside = 0
        ClickMinimize = 1
        ClickMaximize = 2
        ClickRedX = 3

    End Enum
    Private Function DidTheUserClickInTheControlBox() As ControlBoxClick

        Dim Offset As Integer = 15 ' for some reason width is being reported as 15 pixels wider than it should be

        Dim RedXBoxSize = New Size(45, 29)
        Dim MaxBoxSize = New Size(45, 29)
        Dim MinBoxSize = New Size(45, 29)

        Dim RedXBox As Rectangle = New Rectangle(Me.Location.X + Me.Width - Offset - RedXBoxSize.Width, Me.Location.Y, RedXBoxSize.Width, RedXBoxSize.Height)
        Dim MaxBox As Rectangle = New Rectangle(RedXBox.X - MaxBoxSize.Width, Me.Location.Y, MaxBoxSize.Width, MaxBoxSize.Height)
        Dim MinBox As Rectangle = New Rectangle(MaxBox.X - MinBoxSize.Width, Me.Location.Y, MinBoxSize.Width, MinBoxSize.Height)

        Dim MouseClickedAt As Point = New Point(Control.MousePosition.X, Control.MousePosition.Y)

        If RedXBox.Contains(MouseClickedAt) Then
            Return ControlBoxClick.ClickRedX

        ElseIf MaxBox.Contains(MouseClickedAt) Then
            Return ControlBoxClick.ClickMaximize

        ElseIf MinBox.Contains(MouseClickedAt) Then
            Return ControlBoxClick.ClickMinimize

        Else
            Return ControlBoxClick.ClickOutside

        End If

    End Function

    Friend Function MouseIsOverToolStripButton(ByVal tsb As ToolStripButton) As Boolean

        If tsb Is Nothing Then
            Return Nothing
        Else
            Return tsb.Bounds.Contains(PointToClient(Cursor.Position))
        End If

    End Function

#End Region

#Region "Help"

    Private ShowingHelp As Boolean = False

    Private Sub tsHelp_MouseDown(sender As Object, e As MouseEventArgs) Handles tsHelp.MouseDown

        If My.Computer.Keyboard.AltKeyDown Then

            System.Diagnostics.Process.Start(gWebpageHelp)

        Else

            If ShowingHelp Then Exit Sub

            ShowingHelp = True

            Dim HelpText1 As String = "The image you see Is a copy of the original. The original will not be changed unless you choose to save over it." & vbCrLf & vbCrLf &
             "The buttons on the left are used to update the image you see." & vbCrLf & vbCrLf &
             "Left click the 'Mirror' button to horizontally mirror the image, or right click the 'Mirror’ button to vertically mirror it.  Clicking the middle mouse button to removes any mirroring." & vbCrLf & vbCrLf &
             "To quickly de-skew an image, click the 'Level' button, then press the mouse button down on one point and lift it up on a second point where those two points would be on the same horizon should the image be level. For best results, use two points which are as far apart horizontally as possible.  If you want to stop the leveling process, just click on the 'Level' button again." & vbCrLf & vbCrLf &
             "By default when you first open an image it will be automatically zoomed within the Viewer window so that you can see all of it.  However, if you resize the Viewer window you can left click the 'Fit' button to re-fit the image in the viewer.  Middle clicking the 'Fit' button will fit the image horizontally, and left clicking the 'Fit' button will fit the image vertically.  To see the image at its actual size, middle click the 'Zoom' button." & vbCrLf & vbCrLf &
             "For the 'Rotate', 'Zoom','Brightness', Contrast' 'Gamma', and 'Saturation' buttons:" & vbCrLf &
             "Holding the right mouse button down over the button progressively increases its value, while holding the left mouse button down over it progressively decreases its value. " &
             "Holding the Shift key downs the rate of change by two, holding the Ctrl key downs the rate of change by five, holding both downs the rate of change by 10. " &
             "Clicking the middle mouse button over a button resets the value of that button to its original value." & vbCrLf & vbCrLf &
             "The status bar at the bottom of the windows shows the current values associated with each button." & vbCrLf & vbCrLf &
             "(more ...)"


            Dim HelpText2 As String = "To crop an image, click on the 'Crop' button and then use the mouse to draw a box around what you would like to crop.  If you want to stop the cropping process, just click on the 'Crop' button again." & vbCrLf & vbCrLf &
             "Clicking the 'Invert' button toggles the image between being inverted or not." & vbCrLf & vbCrLf &
             "Clicking the 'Grayscale' button toggles the image between grayscale and colour." & vbCrLf & vbCrLf &
             "Left clicking the 'Effects' button opens the Effects Design window where you can manage your effects (more help is available on that window)." & vbCrLf & vbCrLf &
             "Right clicking the 'Effects' button opens an Effect Selection box that lets you choose which effects you would like applied to your image.  Of note: only enabled effects are shown (effects are enabled/disabled in the Effects Deisgn window). " & "In the Effect Selection box: clicking the name of the effect specifies it for use, double clicking the name of the effect opens it in the Effect Design window, clicking the red box in the upper left hand corner removes the specified effect from use, clicking the green box in the upper right hand corner lets you add another specified effect (to the right), and clicking the long blue box accepts all specified effects for use." & vbCrLf & vbCrLf &
             "To move the Effect Selection boxes, click and hold your mouse button down on the 'Effect 1' selection box's title bar and dragging it to a new location." & vbCrLf & vbCrLf &
             "Middle clicking on the 'Effects' button removes all effects." & vbCrLf & vbCrLf &
             "Holding a mouse button down on the 'Reset' button for one second resets the image to what it looked like when it was first shown." & vbCrLf & vbCrLf &
             "(more ...)"

            Dim HelpText3 As String = "Right clicking on the 'Viewer' button opens a second window on the right containing the original image. This image will be kept in sync with the changed image as you pan, rotate or zoom it.  Left clicking on the 'Viewer' button will put the original image under the changing one.  Middle clicking the 'Viewer' button will hide the original image." & vbCrLf & vbCrLf &
             "Clicking the 'Pin to top' button will toggle if the viewer window stays on top of other windows." & vbCrLf & vbCrLf &
             "Left clicking the 'AR4W' button will launch A Ruler for Windows (if it is installed on the computer).  The left mouse button opens the program in reading guide mode, the right mouse button opens it in ruler mode, the middle mouse button closes A Ruler for Windows." & vbCrLf & vbCrLf &
             "Clicking the 'Magnify' button opens Microsoft's Magnify tool if it is closed or closes it if it is open.  When the magnifier is open it will be restored to its last used settings." & vbCrLf & vbCrLf &
             "Clicking the 'Copy' button copies the image to the computer's clipboard." & vbCrLf & vbCrLf &
             "Clicking the 'Save' button allows you to save the image to your computer or network." & vbCrLf & vbCrLf &
             "When the mouse pointer is over the image, clicking and holding any mouse button allows you to pan the image." & vbCrLf & vbCrLf &
             "When the mouse pointer is over the image, spinning the mouse wheel will zoom in / out on the image. " &
             "Also holding down the middle button over an image shows the original image (only) until you lift up on the middle mouse button again." & vbCrLf & vbCrLf &
             "Holding the 'Alt' key down when rotating the image, jumps it to the next 45 degree increment." & vbCrLf & vbCrLf &
             "The line between the button images and text can be dragged left or right to hide/unhide the words associated with the buttons." & vbCrLf & vbCrLf &
             "To view the online help, hold the 'Alt' key down while clicking on 'Quick Help'."

            If gColourLiteral = "Color" Then
                HelpText1 = HelpText1.Replace("colour", "color")
                HelpText2 = HelpText2.Replace("colour", "color")
            End If

            TempHideAR4W(True)

            MessageBox.Show(HelpText1, gThisProgramName & gVersionInUse & " - Quick Help - 1 of 3", MessageBoxButtons.OK, MessageBoxIcon.Information)

            tsPinnedToTop.Image = Resources.unpin
            MakeTopMostWindow(Me.Handle.ToInt64, True)
            Me.BringToFront()
            Application.DoEvents()
            MakeTopMostWindow(Me.Handle.ToInt64, False)

            MessageBox.Show(HelpText2, gThisProgramName & gVersionInUse & " - Quick Help - 2 of 3", MessageBoxButtons.OK, MessageBoxIcon.Information)

            tsPinnedToTop.Image = Resources.unpin
            MakeTopMostWindow(Me.Handle.ToInt64, True)
            Me.BringToFront()
            Application.DoEvents()
            MakeTopMostWindow(Me.Handle.ToInt64, False)

            MessageBox.Show(HelpText3, gThisProgramName & gVersionInUse & " - Quick Help - 3 of 3", MessageBoxButtons.OK, MessageBoxIcon.Information)

            TempHideAR4W(False)

            ShowingHelp = False

        End If

    End Sub

#End Region

    Private ehnacedControlCapturingMoves As Boolean = False
    Const enhancedControlBoarderHeight As Integer = 20
    Const enhanceControlButtonWidth As Integer = 14

    Private enhancedControlPointOnScreen As Point
    Private enhancedControlPointFromOriginOfWorkingArea As Point
    Private enhancedControlPointOnTitleBar As Point

    Private enhancedControlOffset(gMaxEffectsSupported) As Point

    Friend Sub EnhancedTreeViewBoxes_MouseDown(ByVal sender As Object, ByVal e As Windows.Forms.MouseEventArgs)

        ' note only the first EnhancedTreeViewBoxes can be moved, the others will follows the lead of the first

        If sender.tag = "1" Then
        Else
            Me.Cursor = Cursors.No
            Exit Sub
        End If

        Me.Cursor = Cursors.Hand

        If e.Button = MouseButtons.Left Then

            If e.Y <= enhancedControlBoarderHeight Then

                If (e.X > enhanceControlButtonWidth) AndAlso (e.X < (Me.Width - enhanceControlButtonWidth)) Then

                    ehnacedControlCapturingMoves = True
                    enhancedControlPointOnScreen = New Point(Cursor.Position.X, Cursor.Position.Y)
                    enhancedControlPointFromOriginOfWorkingArea = New Point(sender.parent.location.x, sender.parent.location.y)
                    enhancedControlPointOnTitleBar = New Point(sender.location.x, sender.location.y)

                    Exit Sub

                End If

            End If

        End If

    End Sub

    Friend Sub EnhancedTreeViewBoxes_MouseUp(ByVal sender As Object, ByVal e As Windows.Forms.MouseEventArgs)

        ' note only the first EnhancedTreeViewBoxes can be moved, the others will follows the lead of the first

        Me.Cursor = Cursors.Default

        If sender.tag = "1" Then
        Else

            'Beep()
            'MessageBox.Show("You can only reposition the 'Effect 1' selection box." & vbCrLf & vbCrLf & "However, when you repostion the 'Effect 1' selection box, all subsequent selection boxes will be Computeally repositioned to fall in line behind it." & vbCrLf & vbCrLf, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            'Dim EnhancedTreeViewBox_1_PointOnFrom As Point = EnhancedTreeViewBoxes(1).PointToScreen(Point.Empty)
            ' Cursor.Position = New Point(EnhancedTreeViewBox_1_PointOnFrom.X + CInt(EnhancedTreeViewBoxes(1).Width / 2), EnhancedTreeViewBox_1_PointOnFrom.Y + 5)

            Exit Sub

        End If

        ehnacedControlCapturingMoves = False

        EnhancedTreeViewBoxes(CInt(sender.tag)).TreeView1.ResumeLayout()

        FinalForm_Resize(Nothing, Nothing)

        PictureBoxPrimary.Invalidate()

        Me.Refresh()

    End Sub

    Friend Sub EnhancedTreeViewBoxes_MouseMove(ByVal sender As Object, ByVal e As Windows.Forms.MouseEventArgs)

        ' note only the first EnhancedTreeViewBoxes can be moved, the others will follows the lead of the first

        If ehnacedControlCapturingMoves Then

            EnhancedTreeViewBoxes(CInt(sender.tag)).TreeView1.SuspendLayout()

            sender.location = New Point(Cursor.Position.X - enhancedControlPointOnScreen.X + enhancedControlPointFromOriginOfWorkingArea.X + enhancedControlPointOnTitleBar.X - sender.width / 4, Cursor.Position.Y - enhancedControlPointOnScreen.Y + enhancedControlPointFromOriginOfWorkingArea.Y + enhancedControlPointOnTitleBar.Y)

            enhancedControlOffset((sender.tag)) = sender.Location

        End If

    End Sub

    Private Sub FinalForm_Resize(sender As Object, e As EventArgs) Handles Me.Resize

        If initialLoadUnderway Then Exit Sub

        Me.SuspendLayout()

        Try

            reloadUnderway = True

            If gCurrentSplitContainerWidth > 0 Then

                If SplitContainer1.Width = gCurrentSplitContainerWidth Then
                Else
                    IgnorChangesToSplitContainerWidth = True
                    SplitContainer1.Width = gCurrentSplitContainerWidth
                    IgnorChangesToSplitContainerWidth = False
                End If

            End If

            SetLookOfWindows()

            If gCurrentNumberOfEffectsBoxesInUse < 1 Then
                gEffectsBoxesAreHidden = True
            End If

            If gEffectsBoxesAreHidden Then

                For x = 1 To gMaxEffectsSupported

                    If EnhancedTreeViewBoxes(x) IsNot Nothing Then
                        EnhancedTreeViewBoxes(x).Visible = False
                    End If

                Next

            Else

                If gCurrentNumberOfEffectsBoxesInUse > 0 Then

                    If (gCurrentNumberOfEffectsBoxesInUse > 0) Then

                        For x = 1 To gCurrentNumberOfEffectsBoxesInUse

                            EnhancedTreeViewBoxes(x).maxWidth = CInt((SplitContainer1.Panel2.Width - 2 * EnhancedTreeViewBoxes(1).btnDone.Width) / gCurrentNumberOfEffectsBoxesInUse)

                            If x = 1 Then

                                EnhancedTreeViewBoxes(x).ResizeControl(True)

                                If x = gCurrentNumberOfEffectsBoxesInUse Then
                                Else
                                    EnhancedTreeViewBoxes(x).TreeView1.Width += EnhancedTreeViewBoxes(1).btnDone.Width
                                End If

                                If (EnhancedTreeViewBoxes(x).Location.Y + EnhancedTreeViewBoxes(x).Height) > (SplitContainer1.Panel2.Location.Y + SplitContainer1.Panel2.Height) Then

                                    Dim reduction As Integer = (EnhancedTreeViewBoxes(x).Location.Y + EnhancedTreeViewBoxes(x).Height) - (SplitContainer1.Panel2.Location.Y + SplitContainer1.Panel2.Height)  'test
                                    EnhancedTreeViewBoxes(x).Location = New Point(EnhancedTreeViewBoxes(x).Location.X, EnhancedTreeViewBoxes(x).Location.Y - reduction)

                                End If


                            ElseIf x < gCurrentNumberOfEffectsBoxesInUse Then

                                EnhancedTreeViewBoxes(x).Width = EnhancedTreeViewBoxes(x - 1).Width
                                EnhancedTreeViewBoxes(x).Height = EnhancedTreeViewBoxes(x - 1).Height
                                EnhancedTreeViewBoxes(x).TreeView1.Width = EnhancedTreeViewBoxes(x - 1).Width
                                EnhancedTreeViewBoxes(x).TreeView1.Height = EnhancedTreeViewBoxes(x - 1).Height
                                EnhancedTreeViewBoxes(x).ResizeControl(False)
                                EnhancedTreeViewBoxes(x).Location = New Point(EnhancedTreeViewBoxes(x - 1).Location.X + EnhancedTreeViewBoxes(x - 1).Width + 1, EnhancedTreeViewBoxes(x - 1).Location.Y)

                            Else

                                EnhancedTreeViewBoxes(x).Width = EnhancedTreeViewBoxes(x - 1).Width
                                EnhancedTreeViewBoxes(x).Height = EnhancedTreeViewBoxes(x - 1).Height
                                EnhancedTreeViewBoxes(x).TreeView1.Height = EnhancedTreeViewBoxes(x - 1).Height
                                EnhancedTreeViewBoxes(x).ResizeControl(False)
                                EnhancedTreeViewBoxes(x).Width += EnhancedTreeViewBoxes(x).btnDone.Width
                                EnhancedTreeViewBoxes(x).Location = New Point(EnhancedTreeViewBoxes(x - 1).Location.X + EnhancedTreeViewBoxes(x - 1).Width + 1, EnhancedTreeViewBoxes(x - 1).Location.Y)

                            End If

                            ' put the done box on the last control

                            EnhancedTreeViewBoxes(x).btnDone.Visible = (x = gCurrentNumberOfEffectsBoxesInUse)

                            EnsureVisableAndOptionalySelelectATreeNode(gCurrentNamesOfEffectsInUse(x), EnhancedTreeViewBoxes(x).TreeView1, True)
                            EnhancedTreeViewBoxes(x).TreeView1.Focus()

                            EnhancedTreeViewBoxes(x).Visible = True
                            EnhancedTreeViewBoxes(x).BringToFront()

                            Application.DoEvents()

                        Next

                        If gCurrentNumberOfEffectsBoxesInUse > 0 Then

                            'v1.6

                            'make sure box 1 starts at location.x postion = 0
                            ' adds a y offset if the box has been moved by the user
                            If enhancedControlOffset(1).Y > 0 Then
                                EnhancedTreeViewBoxes(1).Location = New Point(0, enhancedControlOffset(1).Y)
                            Else
                                EnhancedTreeViewBoxes(1).Location = New Point(0, EnhancedTreeViewBoxes(1).Location.Y)
                            End If

                            'make sure box 1 doesn't end up above point (0,0)
                            If EnhancedTreeViewBoxes(1).Location.Y < 0 Then
                                EnhancedTreeViewBoxes(1).Location = New Point(EnhancedTreeViewBoxes(1).Location.X, 0)
                            End If

                            'make sure box 1 doesn't start beyond the bottom of the (rotated) corrected image 
                            If EnhancedTreeViewBoxes(1).Location.Y > gCorrectedImage.Height Then
                                EnhancedTreeViewBoxes(1).Location = New Point(0, gCorrectedImage.Height)
                            End If

                            For x = 2 To gCurrentNumberOfEffectsBoxesInUse

                                'arrange the boxes along the x axis (i.e. box 1 is followed by box 2 is followed by box 3 etc.)
                                EnhancedTreeViewBoxes(x).Location = New Point(EnhancedTreeViewBoxes(x - 1).Location.X + EnhancedTreeViewBoxes(x - 1).Width, EnhancedTreeViewBoxes(x).Location.Y)

                                'make box tops line up along the y axis
                                EnhancedTreeViewBoxes(x).Location = New Point(EnhancedTreeViewBoxes(x).Location.X, EnhancedTreeViewBoxes(x - 1).Location.Y)

                            Next

                            ' add or remove the add button as needed 
                            For x = 1 To gCurrentNumberOfEffectsBoxesInUse
                                EnhancedTreeViewBoxes(x).btnEffectAdd.Visible = (gCurrentNumberOfEffectsBoxesInUse < gMaxEffectsSupported)
                            Next

                        End If

                    End If

                    For x = gCurrentNumberOfEffectsBoxesInUse + 1 To gMaxEffectsSupported

                        EnhancedTreeViewBoxes(x).Visible = False
                        gCurrentNamesOfEffectsInUse(x) = gDefaultEffectNotInUse

                    Next

                End If

            End If

        Catch ex As Exception

        End Try

        Me.ResumeLayout()

        reloadUnderway = False

        Me.Refresh()

    End Sub

End Class

#Region "Unused code "

'Friend Sub SmoothJaggedEdges(ByRef tempimg As Bitmap, ByVal MakeTranspent As Boolean)

'    ' there there are two options in processing
'    ' make transparent = true ... the edge pixel will be made transparent (reduces image size by one pixel on each edge)
'    ' make transparent = false ... the colour of the edge pixel will borrow from the agcent pixel  (preseves image size)

'    ' there are no jagged edges at incrments of 45 degrees

'    If gCurrentRotation Mod 45 = 0 Then Exit Sub

'    ' assumes image has been rotated and background behind rotation is set to gMyTransparanceyColour

'    Dim nWidth As Integer = tempimg.Width
'    Dim nHeight As Integer = tempimg.Height

'    Dim bmData As BitmapData = Nothing

'    Try

'        Dim imgPixelFormat As PixelFormat = tempimg.PixelFormat
'        bmData = tempimg.LockBits(New Rectangle(0, 0, tempimg.Width, tempimg.Height), ImageLockMode.ReadWrite, imgPixelFormat)

'        Dim scan0 As IntPtr = bmData.Scan0
'        Dim stride As Integer = bmData.Stride

'        Dim pf((bmData.Stride * bmData.Height) - 1) As Byte
'        Marshal.Copy(bmData.Scan0, pf, 0, pf.Length)

'        Dim pos As Integer = 0

'        Dim ta As Byte = gMyTransparanceyColour.A
'        Dim tr As Byte = gMyTransparanceyColour.R
'        Dim tg As Byte = gMyTransparanceyColour.G
'        Dim tb As Byte = gMyTransparanceyColour.B

'        Parallel.[For](0, nHeight - 1, Sub(y)


'                                           Dim pfpos As Integer

'                                           'remove jagged edge on left

'                                           pfpos = y * stride

'                                           For x As Integer = 0 To nWidth - 2

'                                               'Dim alpha As Byte = pf(pfpos + 3)
'                                               'Dim red As Byte = pf(pfpos + 2)
'                                               'Dim green As Byte = pf(pfpos + 1)
'                                               'Dim blue As Byte = pf(pfpos)

'                                               'search towards the left and convert the first pixel that doesn't match the transparancy colour to the transparency color

'                                               If (pf(pfpos + 3) <> ta) AndAlso (pf(pfpos + 2) <> tr) AndAlso (pf(pfpos + 1) <> tg) AndAlso (pf(pfpos) <> tb) Then

'                                                   If MakeTranspent Then
'                                                       pf(pfpos + 3) = ta
'                                                       pf(pfpos + 2) = tr
'                                                       pf(pfpos + 1) = tg
'                                                       pf(pfpos) = tb
'                                                   Else
'                                                       pf(pfpos + 3) = pf(pfpos + 7)
'                                                       pf(pfpos + 2) = pf(pfpos + 6)
'                                                       pf(pfpos + 1) = pf(pfpos + 5)
'                                                       pf(pfpos) = pf(pfpos + 4)
'                                                   End If

'                                                   Exit For

'                                               End If

'                                               pfpos += 4

'                                           Next

'                                           'remove jagged edge on right

'                                           'search towards the right and convert the first pixel that doesn't match the transparancy colou to the transparency color

'                                           pfpos = y * stride + 4 * (nWidth - 1)

'                                           For x As Integer = nWidth - 1 To 1 Step -1

'                                               If (pf(pfpos + 3) <> ta) AndAlso (pf(pfpos + 2) <> tr) AndAlso (pf(pfpos + 1) <> tg) AndAlso (pf(pfpos) <> tb) Then

'                                                   If MakeTranspent Then
'                                                       pf(pfpos + 3) = ta
'                                                       pf(pfpos + 2) = tr
'                                                       pf(pfpos + 1) = tg
'                                                       pf(pfpos) = tb
'                                                   Else
'                                                       pf(pfpos + 3) = pf(pfpos - 7)
'                                                       pf(pfpos + 2) = pf(pfpos - 6)
'                                                       pf(pfpos + 1) = pf(pfpos - 5)
'                                                       pf(pfpos) = pf(pfpos - 4)
'                                                   End If

'                                                   Exit For

'                                               End If

'                                               pfpos -= 4

'                                           Next

'                                       End Sub)


'        Parallel.[For](0, nWidth - 1, Sub(x)

'                                          Dim pfpos As Integer

'                                          'remove jagged edge on top

'                                          For y As Integer = 0 To nHeight - 2

'                                              pfpos = y * stride + 4 * x

'                                              'search towards the bottom and convert the first pixel that doesn't match the transparancy colou to the transparency color

'                                              If (pf(pfpos + 3) <> ta) AndAlso (pf(pfpos + 2) <> tr) AndAlso (pf(pfpos + 1) <> tg) AndAlso (pf(pfpos) <> tb) Then

'                                                  If MakeTranspent Then
'                                                      pf(pfpos + 3) = ta
'                                                      pf(pfpos + 2) = tr
'                                                      pf(pfpos + 1) = tg
'                                                      pf(pfpos) = tb
'                                                  Else
'                                                      pf(pfpos + 3) = pf(pfpos + 3 + stride)
'                                                      pf(pfpos + 2) = pf(pfpos + 2 + stride)
'                                                      pf(pfpos + 1) = pf(pfpos + 1 + stride)
'                                                      pf(pfpos) = pf(pfpos + stride)
'                                                  End If

'                                                  Exit For

'                                              End If

'                                          Next

'                                          'remove jagged edge on bottom

'                                          'search towards the top and convert the first pixel that doesn't match the transparancy colou to the transparency color

'                                          For y As Integer = nHeight - 1 To 1 Step -1

'                                              pfpos = y * stride + 4 * x

'                                              If (pf(pfpos + 3) <> ta) AndAlso (pf(pfpos + 2) <> tr) AndAlso (pf(pfpos + 1) <> tg) AndAlso (pf(pfpos) <> tb) Then

'                                                  If MakeTranspent Then
'                                                      pf(pfpos + 3) = ta
'                                                      pf(pfpos + 2) = tr
'                                                      pf(pfpos + 1) = tg
'                                                      pf(pfpos) = tb
'                                                  Else
'                                                      pf(pfpos + 3) = pf(pfpos + 3 - stride)
'                                                      pf(pfpos + 2) = pf(pfpos + 2 - stride)
'                                                      pf(pfpos + 1) = pf(pfpos + 1 - stride)
'                                                      pf(pfpos) = pf(pfpos - stride)
'                                                  End If

'                                                  Exit For

'                                              End If

'                                          Next

'                                      End Sub)


'        Marshal.Copy(pf, 0, bmData.Scan0, pf.Length)

'        tempimg.UnlockBits(bmData)

'    Catch ex As Exception

'    End Try


'End Sub

#End Region
