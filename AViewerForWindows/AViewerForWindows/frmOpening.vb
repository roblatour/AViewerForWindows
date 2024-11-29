Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports ImageMagick

Public Class frmOpening
    Inherits Form
    Structure HotSpotStructure
        Dim X As Integer
        Dim ToolTip As String
    End Structure

    Private HotSpot(4) As HotSpotStructure
    Public Sub New()
        InitializeComponent()
    End Sub

    Private NoSnipHotSpotXCoordinateThresholds() As Integer = {0, 150, 245, 327, 999}
    Private SnipHotSpotXCoordinateThresholds() As Integer = {0, 150, 245, 334, 417}
    Private Sub OpeningForm_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Hide form during initial load
        Dim FrmOpeningNormalSize As Size = Me.Size
        Me.Visible = False
        Me.FormBorderStyle = FormBorderStyle.None
        Me.Opacity = 0.01
        Me.MinimumSize = New Size(1, 1)
        Me.Size = Me.MinimumSize

        Try

            ' prevent the form from being resized

            Me.MinimumSize = Me.Size
            Me.MaximumSize = Me.Size

            Me.MenuStrip1.BackColor = Color.White

            Me.StatusStrip1.BackColor = Color.White
            Me.ToolStripTip.BackColor = Color.White
            Me.StatusStrip1.SizingGrip = False

            gRunningOnA4KMonitor = (System.Windows.SystemParameters.FullPrimaryScreenWidth >= 3840)

            ' get the version number and add it to the title bar for the program

            Dim a As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()

            If My.Settings.CurrentVersion = a.ToString Then
            Else
                My.Settings.Upgrade()
                My.Settings.CurrentVersion = a.ToString
                My.Settings.Save()
            End If

            Dim versionnumber As String = a.GetName.Version.Major & "." & a.GetName.Version.Minor & "." & a.GetName.Version.Build & "." & a.GetName.Version.Revision

            gVersionInUse = " v" & versionnumber
            gVersionInUse = ShortenVersionNumber(gVersionInUse)
            If gBetaVersion.Length > 0 Then gVersionInUse &= gBetaVersion

            gWebpageHelp = gWebpageHelp.Replace("version", versionnumber.Replace(".", "_"))

            Me.Text = gThisProgramName & gVersionInUse

            EstabishReadingAndWritingFilters()

            MakeWindowAlwaysOnTopAsNeeded()

            SetupForSnippingTool()

            SetFramingColour()

            ' setup the hotspots

            If W10SnippingToolFound OrElse W11SnippingToolFound Then

                HotSpot(0).X = SnipHotSpotXCoordinateThresholds(0)
                HotSpot(1).X = SnipHotSpotXCoordinateThresholds(1)
                HotSpot(2).X = SnipHotSpotXCoordinateThresholds(2)
                HotSpot(3).X = SnipHotSpotXCoordinateThresholds(3)
                HotSpot(4).X = SnipHotSpotXCoordinateThresholds(4)

                HotSpot(0).ToolTip = "Drag and drop an image file to open it"
                HotSpot(1).ToolTip = "Click to open an image file"
                HotSpot(2).ToolTip = "Click to open an image in your clipboard"
                HotSpot(3).ToolTip = "Click to select an image using Microsft's Snipping tool"
                HotSpot(4).ToolTip = "Click to select an image already shown on your screen"

            Else

                HotSpot(0).X = NoSnipHotSpotXCoordinateThresholds(0)
                HotSpot(1).X = NoSnipHotSpotXCoordinateThresholds(1)
                HotSpot(2).X = NoSnipHotSpotXCoordinateThresholds(2)
                HotSpot(3).X = NoSnipHotSpotXCoordinateThresholds(3)
                HotSpot(4).X = NoSnipHotSpotXCoordinateThresholds(4)

                HotSpot(0).ToolTip = "Drag and drop an image file to open it"
                HotSpot(1).ToolTip = "Click to open an image file"
                HotSpot(2).ToolTip = "Click to open an image in your clipboard"
                HotSpot(3).ToolTip = "Click to select an image using Microsft's Snipping tool"
                HotSpot(4).ToolTip = "" 'unused when there is no snipping tool found

            End If

            gColourLiteral = IIf(System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToUpper = "USA", "Color", "Colour")
            gCentreLiteral = IIf(System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToUpper = "USA", "Center", "Centre")

            tsCheckForUpdates.Checked = My.Settings.CheckForUpdates

            gignoreFileWatcherEventUntilThisTime = Now.AddDays(-1)

            gXML_Path_Name = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) & "\AV4W"

            If LoadANewProfileFromTheCommandLine() Then

                'ref:  https://stackoverflow.com/questions/731068/closing-a-form-from-the-load-handler/71713576#71713576

                BeginInvoke(New MethodInvoker(AddressOf Me.Close))
                Exit Sub

            Else

                Call ValidateEffectsProfiles()

                LoadNeutralEffectsProfile(False)

                gMiniCheckerBoardImage = New Bitmap(MakeACheckboardImage)
                gMiniCheckerBoardTextureBrush = New TextureBrush(gMiniCheckerBoardImage)

                CheckForUpdates()

                SetupForFileWatcherMonitoring()

                If LoadAnImageFromTheCommandLine() Then
                    Safely_ProcessFile_Public(gFilePathNameAndExtention)
                End If

            End If

        Catch ex As Exception

        End Try

        ' restore original look of form
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.Opacity = 1
        Me.MaximumSize = FrmOpeningNormalSize
        Me.MinimumSize = FrmOpeningNormalSize
        Me.Size = FrmOpeningNormalSize

        Me.Visible = True

    End Sub

    Private Sub EstabishReadingAndWritingFilters()

        ' Establish the valid file types and filters to be used in open and save window dialogues

        ' example of a basic filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"

        ' Part 1 generate all supported read formats according to magick
        ' note: magick file types for writing are not used; most are silly; only native writing filetypes in part 2 will be used

        Dim AllImageFileReadingFilterPart1 As String = "All Image Files ("
        Dim AllImageFileReadingFilterPart1a As String = ""

        Dim AllImageFileWritingFilterPart1 As String = "All Image Files ("
        Dim AllImageFileWritingFilterPart1a As String = ""

        Const skipTheseFileTypesAsTheyNeedGhostScript As String = "EPI;EPS;EPS2;EPS3;EPSF;EPSI';EPT;PDF;PS;PS2;PS3"

        For Each supportedFormat As MagickFormatInfo In MagickNET.SupportedFormats

            If skipTheseFileTypesAsTheyNeedGhostScript.Contains(supportedFormat.Format.ToString.ToUpper) Then

            Else

                ' Set reading filters

                If supportedFormat.SupportsReading Then

                    AllImageFileReadingFilterPart1a &= "*." & supportedFormat.Format.ToString.ToLower & ";"

                    gValidFileTypesForReading.Add("." & supportedFormat.Format.ToString.ToLower)

                End If

                ' set writing filters

                'If supportedFormat.IsWritable Then

                '    AllImageFileWritingFilterPart1a &= "*." & supportedFormat.Format.ToString.ToLower & ";"

                '    gValidFileTypesForWriting.Add("." & supportedFormat.Format.ToString.ToLower)

                'End If

            End If

        Next

        AllImageFileReadingFilterPart1a = AllImageFileReadingFilterPart1a.TrimEnd(";"c)
        AllImageFileReadingFilterPart1 &= AllImageFileReadingFilterPart1a & ")|" & AllImageFileReadingFilterPart1a

        'Part 2  generate files types that can be read or written without the use of Magick

        Dim AllImageFileReadingAndWritingFilterPart2 As String = "|"

        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageEncoders()

        Dim sep As String = String.Empty
        Dim sep1 As String = String.Empty
        Dim addon As String = String.Empty

        For Each c As ImageCodecInfo In codecs

            sep = String.Empty
            sep1 = String.Empty
            addon = String.Empty
            Dim extentsions As String = c.FilenameExtension.Replace("*", "").Replace(";", " ")
            For Each extension In extentsions.Split(" "c)

                addon &= sep1 & "*" & extension.ToLower
                sep1 = ";"

            Next

            Dim codecName As String = c.CodecName.Substring(8).Replace("Codec", "Files").Trim()

            AllImageFileReadingAndWritingFilterPart2 &= String.Format("{0}{1}{2} ({3})|{3}", gValidFileTypesReadingFilter, sep, codecName, c.FilenameExtension.ToLower) & "|"

            sep = "|"

        Next

        gValidFileTypesReadingFilter = AllImageFileReadingFilterPart1 & AllImageFileReadingAndWritingFilterPart2.TrimEnd("|"c)
        gValidFileTypesForReading.Sort()

        gValidFileTypesWritingFilter = "Image Files(*.bmp;*.emf;*exif;*.gif;*.icon;*.jpg;*.png;*.tiff;*.wmf)|*.bmp;*.emf;*exif;*.gif;*.icon;*.jpg;*.png;*.tiff;*.wmf"
        gValidFileTypesForWriting.Add(".bmp")
        gValidFileTypesForWriting.Add(".emf")
        gValidFileTypesForWriting.Add(".exif")
        gValidFileTypesForWriting.Add(".gif")
        gValidFileTypesForWriting.Add(".icon")
        gValidFileTypesForWriting.Add(".jpg")
        gValidFileTypesForWriting.Add(".png")
        gValidFileTypesForWriting.Add(".tiff")
        gValidFileTypesForWriting.Add(".wmf")

    End Sub
    Private Function LoadANewProfileFromTheCommandLine() As Boolean

        ' returns true if a profile was succesfully loaded from the command line

        Dim returnValue As Boolean = False

        Dim ErrorFound As Boolean = True

        Dim LoadRequired As Boolean = False

        Try

            'Get parameters passed into this program

            Dim InputFileName As String = Microsoft.VisualBasic.Command.ToString.Trim

            '#If DEBUG Then

            '            InputFileName = """C:\Users\Rob Latour\Desktop\SE Vertical Sobel Edge Detector.av4w"""

            '#End If


            If InputFileName.Length = 0 Then
                ErrorFound = False
                Exit Try
            End If

            If InputFileName.ToLower.Contains(gMyExtention) Then
            Else
                ErrorFound = False
                Exit Try
            End If
            '***************************************************************************

            Dim CandidateProfile As String = InputFileName

            If CandidateProfile.StartsWith("""") Then
                CandidateProfile = CandidateProfile.Remove(0, 1)
            Else
                ErrorFound = False
                Exit Try
            End If

            If CandidateProfile.EndsWith("""") Then
                CandidateProfile = CandidateProfile.Remove(CandidateProfile.Length - 1, 1)
            Else
                ErrorFound = False
                Exit Try
            End If

            InputFileName = CandidateProfile ' same as before but with the quotes removed

            Dim ext As String = System.IO.Path.GetExtension(CandidateProfile)
            If (String.Compare(ext, gMyExtention, True) <> 0) Then Exit Try

            If CandidateProfile.ToUpper.EndsWith(gMyExtention.ToUpper) Then
                CandidateProfile = CandidateProfile.Remove(CandidateProfile.Length - 5)
            Else
                Beep()
                MsgBox("The profile '" & CandidateProfile.Trim & "' filename does not end with .av4w; profile has not been added.", MsgBoxStyle.OkOnly, gThisProgramName & gVersionInUse)
                Me.DialogResult = DialogResult.None ' keeps form from closing
                ErrorFound = False
                Exit Try
            End If

            If CandidateProfile.ToUpper.EndsWith(".AV4W") Then
                Beep()
                MsgBox("The profile '" & CandidateProfile.Trim & "' filename ends with .av4w more than once; profile has not been added.", MsgBoxStyle.OkOnly, gThisProgramName & gVersionInUse)
                Me.DialogResult = DialogResult.None ' keeps form from closing
                ErrorFound = False
                Exit Try
            End If

            CandidateProfile = CandidateProfile.Remove(0, CandidateProfile.LastIndexOf("\") + 1).Trim

            'Convert all %20s to spaces
            CandidateProfile = CandidateProfile.Replace("%20", " ")

            ' Remove the [1], [2], etc. that may have been tacked onto the end of the candiate profile name by helpful browser software
            Dim TestForIEAddedExtention As String

            For x As Integer = 1 To 25

                TestForIEAddedExtention = "[" & CType(x, String).Trim & "]"
                If CandidateProfile.EndsWith(TestForIEAddedExtention) Then
                    CandidateProfile = CandidateProfile.Remove(CandidateProfile.LastIndexOf(TestForIEAddedExtention))
                End If

                TestForIEAddedExtention = "(" & CType(x, String).Trim & ")"
                If CandidateProfile.EndsWith(TestForIEAddedExtention) Then
                    CandidateProfile = CandidateProfile.Remove(CandidateProfile.LastIndexOf(TestForIEAddedExtention))
                End If

            Next

            CandidateProfile = CandidateProfile.Trim

            Const legacyEffectsProfilePrefix As String = "EffectsProfile_"

            If CandidateProfile.ToUpper.StartsWith(legacyEffectsProfilePrefix.ToUpper) Then
                CandidateProfile = CandidateProfile.Remove(0, legacyEffectsProfilePrefix.Length).Trim
            End If

            Dim NewPathAndFileName As String = gXML_Path_Name & "\" & CandidateProfile & gMyExtention




            Dim LoadedProfileName As String
            If LoadEffectsProfile(InputFileName, True, LoadedProfileName) Then

                Dim MyNewlyLoadedEffects As gEffectsProfileStructure = gCurrentEffectsProfile

                Dim MyNewLoadedProfilesXMLFilename As String = ConvertProfileNameToXMLFileName(LoadedProfileName)

                'Check to see if that name is already in use

                ErrorFound = False

                Dim ReplaceFlag As Boolean = False

                If System.IO.File.Exists(MyNewLoadedProfilesXMLFilename) Then

                    LoadEffectsProfile(MyNewLoadedProfilesXMLFilename)

                    Dim MyCurrentEffectsProfile As gEffectsProfileStructure = gCurrentEffectsProfile

                    If AreTheseTwoProfilesTheSame(MyCurrentEffectsProfile, MyNewlyLoadedEffects, False, False, False, False, False, False, False) Then

                        Beep()
                        MakeTopMostWindow(Me.Handle, True)

                        System.Windows.MessageBox.Show("The profile '" & LoadedProfileName & "' which was to be loaded is an exact duplicate of one you already have." & vbCrLf & vbCrLf &
                        "It will not be loaded.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                        LoadRequired = False
                        returnValue = True ' causes this instance to close near completion of load

                    Else

                        Beep()
                        MakeTopMostWindow(Me.Handle, True)

                        If MsgBox("The profile '" & LoadedProfileName & "' already exists, but the one you are loading differs from the one you have." & vbCrLf & vbCrLf &
                              "Would you like to replace the one you have with the one you are loading?",
                               MsgBoxStyle.YesNo Or MsgBoxStyle.Question, Me.Text = gThisProgramName & gVersionInUse) = MsgBoxResult.Yes Then

                            gCurrentEffectsProfile = MyNewlyLoadedEffects

                            ReplaceFlag = True
                            LoadRequired = True

                        Else

                            LoadRequired = False
                            returnValue = True

                        End If

                    End If

                Else

                    gCurrentEffectsProfile = MyNewlyLoadedEffects
                    LoadRequired = True

                End If

                If LoadRequired Then

                    If File.Exists(NewPathAndFileName) Then
                        ConfirmCurrentFileHasReadWriteAuthority(NewPathAndFileName)
                        System.IO.File.Delete(NewPathAndFileName)
                    End If

                    SaveCurrentEffectsProfile()

                    Beep()
                    MakeTopMostWindow(Me.Handle, True)

                    Dim msg As String = "The profile '" & CandidateProfile & "' has been "
                    If ReplaceFlag Then
                        msg &= "replaced."
                    Else
                        msg &= "added."
                    End If

                    MsgBox(msg, MsgBoxStyle.OkOnly, gThisProgramName & gVersionInUse)
                    Me.DialogResult = DialogResult.None ' keeps form from closing

                    returnValue = True

                End If

            End If


        Catch ex As Exception

            Beep()
            MakeTopMostWindow(Me.Handle, True)
            MsgBox(gThisProgramName & gVersionInUse & " - Error", MsgBoxStyle.Critical, "Could not load this profile") : Me.DialogResult = 0

        End Try

        If ErrorFound Then

            Beep()
            MakeTopMostWindow(Me.Handle, True)
            MsgBox(gThisProgramName & gVersionInUse & " - Error", MsgBoxStyle.Critical, "Problem trying to load this profile") : Me.DialogResult = 0

        End If

        Return returnValue

    End Function

    Private Function LoadAnImageFromTheCommandLine() As Boolean

        ' returns true if a profile was succesfully loaded from the command line

        Dim returnValue As Boolean = False

        Try

            'Get parameters passed into this program

            Dim InputFileName As String = Microsoft.VisualBasic.Command.ToString.Trim

            '#If DEBUG Then

            '            InputFileName = """E:\Documents\VBNet\AViewerForWindows\Testing av4w\kitten.jpg"""

            '#End If

            If InputFileName.Length = 0 Then Exit Try

            Dim CandidateImage As String = InputFileName

            If CandidateImage.StartsWith("""") Then
                CandidateImage = CandidateImage.Remove(0, 1)
            Else
                Exit Try
            End If

            If CandidateImage.EndsWith("""") Then
                CandidateImage = CandidateImage.Remove(CandidateImage.Length - 1, 1)
            Else
                Exit Try
            End If

            If InputFileName.Length = 0 Then Exit Try

            InputFileName = CandidateImage ' same as before but with the quotes removed

            Dim ext As String = System.IO.Path.GetExtension(CandidateImage).ToString

            If ext = String.Empty Then Exit Try

            If gValidFileTypesForReading.Contains(ext) Then
            Else
                Exit Try
            End If

            If File.Exists(CandidateImage) Then
                gFilePathNameAndExtention = CandidateImage
            End If

            returnValue = True

        Catch ex As Exception

        End Try

        Return returnValue

    End Function

    Friend Sub ConfirmCurrentFileHasReadWriteAuthority(ByVal XML_File_Name As String)

        Try

            Dim fi As New IO.FileInfo(XML_File_Name)
            If (fi.Attributes And IO.FileAttributes.ReadOnly) = IO.FileAttributes.ReadOnly Then
                fi.Attributes = fi.Attributes And (Not IO.FileAttributes.ReadOnly) 'Clear ReadOnly attribute
            End If

        Catch ex As Exception
        End Try

    End Sub

    Private Function ShortenVersionNumber(ByVal VersionNumber As String) As String

        Dim returnValue As String = String.Empty

        Try

            For x = 1 To 3
                If VersionNumber.EndsWith(".0") Then
                    VersionNumber = VersionNumber.Remove(VersionNumber.Length - 2)
                Else
                    Exit For
                End If
            Next

            returnValue = VersionNumber

        Catch ex As Exception

        End Try

        Return returnValue

    End Function


    Private W10_32_SnipperProgram As String = "C:\Windows\System32\SnippingTool.exe"
    Private W10_64_SnipperProgram As String = "c:\Windows\Sysnative\SnippingTool.exe"
    Private W10_SnipperProgram As String = String.Empty

    Private W11SnipperProgram As String = "C:\Users\username\AppData\Local\Microsoft\WindowsApps\SnippingTool.exe"

    Private W10SnippingToolFound As Boolean = False
    Private W11SnippingToolFound As Boolean = False
    Private Sub SetupForSnippingTool()

        'Look for W10 Snipping tool

        W10SnippingToolFound = File.Exists(W10_64_SnipperProgram)

        If W10SnippingToolFound Then

            W10_SnipperProgram = W10_64_SnipperProgram

        Else

            W10SnippingToolFound = File.Exists(W10_32_SnipperProgram)

            If W10SnippingToolFound Then

                W10_SnipperProgram = W10_32_SnipperProgram

            End If

        End If

        'Look for W11 snipping tool

        W11SnipperProgram = W11SnipperProgram.Replace("username", Environment.UserName.Trim)
        W11SnippingToolFound = File.Exists(W11SnipperProgram)

        Dim BaseSize As Size = New Size(24, 98)

        If W10SnippingToolFound OrElse W11SnippingToolFound Then

            Me.MaximumSize = AddTwoSizes(BaseSize, Resources.MainWindowWithSnip.Size)
            Me.MinimumSize = Me.MaximumSize
            Me.Size = Me.MaximumSize
            Me.PictureBox1.Image = Resources.MainWindowWithSnip

        Else

            Me.MaximumSize = AddTwoSizes(BaseSize, Resources.MainWindowWithoutSnip.Size)
            Me.MinimumSize = Me.MaximumSize
            Me.Size = Me.MaximumSize
            Me.PictureBox1.Image = Resources.MainWindowWithoutSnip

        End If

    End Sub

    Private Function AddTwoSizes(ByVal size1 As Size, ByVal size2 As Size) As Size

        Return New Size(size1.Width + size2.Width, size1.Height + size2.Height)

    End Function

#Region "Check for update"

    Private Sub tsCheckForUpdates_Click(sender As Object, e As EventArgs) Handles tsCheckForUpdates.Click

        tsCheckForUpdates.Checked = Not tsCheckForUpdates.Checked
        My.Settings.CheckForUpdates = tsCheckForUpdates.Checked
        My.Settings.Save()

    End Sub

    Private Const Rackspace As String = "https://6ec1f0a2f74d4d0c2019-591364a760543a57f40bab2c37672676.ssl.cf5.rackcdn.com/"
    Private WebPageVerionCheck As String = Rackspace & "av4wCurrentVersion.txt"

    Private Structure FileVersion
        Dim Major As Integer
        Dim Minor As Integer
        Dim Build As Integer
        Dim Revision As Integer
    End Structure

    Private gCurrentlyRunningVersion As FileVersion
    Private gCurrentVersionAccordingToWebsite As FileVersion

    Private gCurrentVersionAccordingToWebsiteFriendlyName As String

    Private gWindowUpgradePromptIsOpen As Boolean = False

    Private Enum RunningVersionRelativeToCurrentWebSiteVersion
        Unknown = 0
        RunningVersionIsNewerThanCurrentVersion = 1
        RunningVersionIsTheSameAsTheCurrentVersion = 2
        RunningVerionsIsOlderThanTheCurrentVersion = 3
    End Enum

    Private gRunningVersionRelativeToCurrentVersion As RunningVersionRelativeToCurrentWebSiteVersion

    Private Sub CheckForUpdates()

        If My.Settings.CheckForUpdates Then
        Else
            Exit Sub
        End If

        'validate next version check date 
        If (My.Settings.NextVersionCheckDate = Nothing) OrElse (Not IsDate(My.Settings.NextVersionCheckDate)) Then
            My.Settings.NextVersionCheckDate = Today.AddYears(-1)
            My.Settings.Save()
        End If

        Try

            Me.Cursor = Cursors.WaitCursor

            If My.Settings.NextVersionCheckDate > Now Then
                'try again tomorrow
                Exit Try
            End If

            Dim lRunningVersionRelativeToCurrentWebSiteVersion As RunningVersionRelativeToCurrentWebSiteVersion = GetRunningVersionRelativeToCurrentWebSiteVersion()

            Me.Cursor = Cursors.Default

            My.Settings.NextVersionCheckDate = Today.Date.AddDays(1)
            My.Settings.Save()

            If (lRunningVersionRelativeToCurrentWebSiteVersion = RunningVersionRelativeToCurrentWebSiteVersion.RunningVerionsIsOlderThanTheCurrentVersion) Then
                ' upgrade required 
            Else
                Exit Try 'try again tomorrow
            End If

#If DEBUG Then

            'Used for testing only
            'My.Settings.SkipUpdateFor = "not yet set"
            'My.Settings.Save()

#End If

            If My.Settings.SkipUpdateFor = gCurrentVersionAccordingToWebsiteFriendlyName Then
                Exit Sub
            End If

            ' update required 

            ' if this window is on top, temp make it not on top so the message box won't be hidden
            If My.Settings.PinnedToTop Then
                MakeTopMostWindow(Me.Handle.ToInt64, True)
                Me.BringToFront()
                Application.DoEvents()
                MakeTopMostWindow(Me.Handle.ToInt64, False)
            End If

            Beep()

            Dim Result As System.Windows.MessageBoxResult = System.Windows.MessageBox.Show("A new version of " & gThisProgramName & " is available!" & vbCrLf & vbCrLf &
                    "You are now running " & (gThisProgramName & gVersionInUse).Replace("(", "").Replace(")", "") & vbCrLf & vbCrLf &
                    "The most current version is v" & ShortenVersionNumber(gCurrentVersionAccordingToWebsiteFriendlyName) & vbCrLf & vbCrLf &
                    "Please click:" & vbCrLf & vbCrLf &
                    "[Yes]    to visit the " & gThisProgramName & " website where you can get the latest version," & vbCrLf & vbCrLf &
                    "[No]     to not be reminded about this particuluar update again, or" & vbCrLf & vbCrLf &
                    "[Cancel] to be reminded again after today.", gThisProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation)

            Select Case Result

                Case Windows.MessageBoxResult.Yes

                    System.Diagnostics.Process.Start(gWebpage)
                    Application.DoEvents()
                    Threading.Thread.Sleep(2000)

                Case Windows.MessageBoxResult.No

                    My.Settings.SkipUpdateFor = gCurrentVersionAccordingToWebsiteFriendlyName
                    My.Settings.Save()

                Case Windows.MessageBoxResult.Cancel
                    ' do nothing (next update date already set above)

            End Select

            MakeWindowAlwaysOnTopAsNeeded()

        Catch ex As Exception

        End Try

        Me.Cursor = Cursors.Default

    End Sub

    Private Function GetRunningVersionRelativeToCurrentWebSiteVersion() As RunningVersionRelativeToCurrentWebSiteVersion

        ' also updates gCurrentVersionAccordingToWebsite

        Dim returnValue As RunningVersionRelativeToCurrentWebSiteVersion = RunningVersionRelativeToCurrentWebSiteVersion.Unknown

        Try

            Dim CurrentRunningVersion As String = My.Application.Info.Version.ToString

            Dim myWebClient As System.Net.WebClient = New System.Net.WebClient
            Dim CurrentWebSiteVersionDataFileContents As String = myWebClient.DownloadString(WebPageVerionCheck)
            myWebClient.Dispose()

            'fix incase retrieved file has vblf and not vbcrlf between first and second row in file

            Dim Entries() As String = Split(CurrentWebSiteVersionDataFileContents, vbCrLf)
            If Entries.Length = 2 Then
            Else
                Entries = Split(CurrentWebSiteVersionDataFileContents, vbLf)
            End If

            Dim CurrentVersion As String = Entries(0).Trim ' the top most number is the current version text file on the web

            gCurrentVersionAccordingToWebsiteFriendlyName = CurrentVersion

            Dim FormatedCurrentVerison As String = FormattedNumber(CurrentVersion)
            Dim FormatedRunningVersion As String = FormattedNumber(CurrentRunningVersion)

            If (FormatedRunningVersion = FormatedCurrentVerison) Then

                returnValue = RunningVersionRelativeToCurrentWebSiteVersion.RunningVersionIsTheSameAsTheCurrentVersion

            ElseIf (FormatedRunningVersion > FormatedCurrentVerison) Then

                returnValue = RunningVersionRelativeToCurrentWebSiteVersion.RunningVersionIsNewerThanCurrentVersion

            Else

                returnValue = RunningVersionRelativeToCurrentWebSiteVersion.RunningVerionsIsOlderThanTheCurrentVersion

            End If

            Dim Working() As String = FormatedCurrentVerison.Split(".")
            With gCurrentVersionAccordingToWebsite
                .Major = CInt(Working(0))
                .Minor = CInt(Working(1))
                .Build = CInt(Working(2))
                .Revision = CInt(Working(3))
            End With

            Entries = Nothing
            Working = Nothing

        Catch ex As Exception

            With gCurrentVersionAccordingToWebsite
                .Major = 0
                .Minor = 0
                .Build = 0
                .Revision = 0
            End With

        End Try

        Return returnValue

    End Function

    Private Function FormattedNumber(ByVal UnformattedVersionNumber As String) As String

        On Error Resume Next
        Dim Piece() As String = Split(UnformattedVersionNumber, ".")

        Return Piece(0).PadLeft(3, "0"c) & "." & Piece(1).PadLeft(3, "0"c) & "." & Piece(2).PadLeft(3, "0"c) & "." & Piece(3).PadLeft(3, "0"c)

    End Function

#End Region
    Private Sub MakeWindowAlwaysOnTopAsNeeded()

        If My.Settings.PinnedToTop Then

            MakeTopMostWindow(Me.Handle.ToInt64, True)
            Me.BringToFront()

        Else

            MakeTopMostWindow(Me.Handle.ToInt64, True)
            Me.BringToFront()
            Application.DoEvents()
            MakeTopMostWindow(Me.Handle.ToInt64, False)

        End If

    End Sub

    Dim Lock As New Object
    Private Sub OpeningForm_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged

        SyncLock Lock

            Me.AboutToolStripMenuItem.Padding = New System.Windows.Forms.Padding(0, 0, 4, 0)
            Application.DoEvents()

            Dim SpaceAvailable As Integer = Me.MenuStrip1.Width
            For Each control As ToolStripItem In MenuStrip1.Items
                SpaceAvailable -= control.Width
            Next

            SpaceAvailable -= 15

            Me.AboutToolStripMenuItem.Padding = New System.Windows.Forms.Padding(SpaceAvailable, 0, 4, 0)
            Me.AboutToolStripMenuItem.Alignment = ToolStripItemAlignment.Left

        End SyncLock

    End Sub
    Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles PictureBox1.MouseLeave
        StatusStrip1.Visible = False
    End Sub

    Private Sub PictureBox1_MouseEnter(sender As Object, e As EventArgs) Handles PictureBox1.MouseEnter
        StatusStrip1.Visible = True
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove

        ' show tool tip based on where the mouse is in the picture box

        Static Dim lasti As Integer = -1

        Dim mouse_x As Integer = e.X

        For i = 4 To 0 Step -1

            If mouse_x > HotSpot(i).X Then

                If i = lasti Then
                Else
                    lasti = i
                    Me.ToolStripTip.Text = HotSpot(i).ToolTip
                End If

                Exit For

            End If

        Next

    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.Click

        If ContinueWaitingForImage Then
            ' user is quiting the snipping function
            ContinueWaitingForImage = False
            Exit Sub
        End If

        Dim MyHotSpots() As Integer

        If W10SnippingToolFound OrElse W11SnippingToolFound Then
            MyHotSpots = SnipHotSpotXCoordinateThresholds
        Else
            MyHotSpots = NoSnipHotSpotXCoordinateThresholds
        End If

        Dim mouse_x As Integer = e.X

        For x = 4 To 0 Step -1

            If mouse_x > MyHotSpots(x) Then

                Select Case x

                    Case Is = 4
                        LoadFromImageOnScreen()

                    Case Is = 3
                        If W10SnippingToolFound OrElse W11SnippingToolFound Then
                            LoadFromSnipper()
                        Else
                            LoadFromImageOnScreen()
                        End If

                    Case Is = 2
                        LoadFromClipboard()

                    Case Is = 1
                        LoadByOpeningAFile()

                    Case Is = 0
                        ' Drag and drop

                End Select

                Exit For

            End If

        Next


    End Sub

#Region "Load by dragging and dropping"

    Private Sub MainForm_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter

        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            e.Effect = DragDropEffects.Copy
        End If

    End Sub

    Private Sub MainForm_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop

        e.Effect = DragDropEffects.None

        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)

        If files.Length = 1 Then

            Dim FileName As String = files(0)

            FileName = GetLnkTargetPathAsNeeded(FileName)

            Dim extension As String = System.IO.Path.GetExtension(FileName).ToLower

            If gValidFileTypesForReading.Contains(extension) Then

                gFilePathNameAndExtention = FileName

                Safely_ProcessFile_Public(gFilePathNameAndExtention)

            Else

                InvalidateFileTypeMessage(extension)

            End If

        Else

            Beep()
            MessageBox.Show("You may only drag and drop one file.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End If

    End Sub

    Private Function GetLnkTargetPathAsNeeded(ByVal Filename As String) As String

        Dim returnValue As String = Filename

        Try

            If IO.Path.GetExtension(Filename).ToLower = ".lnk" Then

                Using br = New BinaryReader(System.IO.File.OpenRead(Filename))
                    br.ReadBytes(&H14)
                    Dim lflags As UInteger = br.ReadUInt32()

                    If (lflags And &H1) = 1 Then
                        br.ReadBytes(&H34)
                        Dim skip = br.ReadUInt16()
                        br.ReadBytes(skip)
                    End If

                    Dim length = br.ReadUInt32()
                    br.ReadBytes(&HC)
                    Dim lbpos = br.ReadUInt32()
                    br.ReadBytes(CInt(lbpos) - &H14)
                    Dim size = length - lbpos - &H2
                    Dim bytePath = br.ReadBytes(CInt(size))
                    Dim path = Encoding.UTF8.GetString(bytePath, 0, bytePath.Length)
                    returnValue = path

                End Using

            End If

        Catch ex As Exception

        End Try

        Return returnValue

    End Function

    Private Sub InvalidateFileTypeMessage(ByVal FileType As String)

        Beep()

        Dim msg As String = gThisProgramName & " is unable to open files having a file type of " & FileType & vbCrLf & vbCrLf & "The file types which " & gThisProgramName & " can open are:" & vbCrLf
        For Each vft In gValidFileTypesForReading
            msg &= vft & " "
        Next

        MessageBox.Show(msg, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    End Sub

#End Region

#Region "Load by opening a file"

    Delegate Sub LoadByOpeningAFile_Delegate()
    Private Sub LoadByOpeningAFile()

        If Directory.Exists(My.Settings.DefaultOpenDirectory) Then
        Else
            My.Settings.DefaultOpenDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop
            My.Settings.Save()
        End If

        gFilePathNameAndExtention = String.Empty

        Dim PromptForFileName As Boolean = True

        While PromptForFileName

            Dim ofd = New OpenFileDialog With {
                    .Filter = gValidFileTypesReadingFilter,
                    .InitialDirectory = My.Settings.DefaultOpenDirectory,
                    .FilterIndex = 1
                }

            If ofd.ShowDialog() = DialogResult.OK Then

                Dim FileName As String = ofd.FileName

                FileName = GetLnkTargetPathAsNeeded(FileName)

                Dim extension As String = Path.GetExtension(FileName).ToLower

                If gValidFileTypesForReading.Contains(extension) Then

                    gFilePathNameAndExtention = FileName
                    Safely_ProcessFile_Public(gFilePathNameAndExtention)
                    PromptForFileName = False

                Else

                    InvalidateFileTypeMessage(extension)

                End If

                My.Settings.DefaultOpenDirectory = Path.GetDirectoryName(FileName)
                My.Settings.Save()

            Else

                PromptForFileName = False

            End If

        End While

    End Sub

#End Region

    Private Sub LoadFromClipboard()

        If Clipboard.ContainsImage() Then
            gOriginalImage = Clipboard.GetImage()
            gFilePathNameAndExtention = "In memory from clipboard"
            Safely_ProcessFile_Public(gFilePathNameAndExtention)
        Else
            '  Beep()
            MessageBox.Show("Clipboard does not contain an image.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If


    End Sub

    Private Sub LoadFromSnipper()

        Me.WindowState = FormWindowState.Minimized

        If W10SnippingToolFound Then

            If W10SnippingLoad() Then
            Else
                Beep()
            End If

        Else

            If W11SnippingLoad() Then
            Else
                Beep()
            End If

        End If

        Me.WindowState = FormWindowState.Normal

    End Sub

    Private Function W10SnippingLoad() As Boolean

        Clipboard.SetText(" ")

        Dim returnValue As Boolean = False

        Try

            System.Diagnostics.Process.Start(W10_SnipperProgram, "/clip")
            Threading.Thread.Sleep(1000)

            returnValue = WaitForImage("W10")

        Catch ex As Exception

        End Try

        Return returnValue

    End Function

    Private Function W11SnippingLoad() As Boolean

        Try

            Clipboard.SetText(" ")

            SetPriority(ProcessPriorityClass.High)
            SendTheTransmissionString("{SHIFT}{WIN}s") 'open the microsoft snipping tool; if started in the same way the W10 process is started an extra unwanted snipper screen appears
            SetPriority(ProcessPriorityClass.Normal)

            Return WaitForImage("W11")

        Catch ex As Exception

            Return False

        End Try

    End Function

    Private ContinueWaitingForImage As Boolean = False
    Private Function WaitForImage(ByVal Method As String) As Boolean

        Dim returnValue As Boolean

        ContinueWaitingForImage = True

        Dim WaitForProgramToStart As Boolean = True

        'to do put an escape for this loop me.normal>?
        While WaitForProgramToStart

            If Method = "W10" Then
                WaitForProgramToStart = (Diagnostics.Process.GetProcessesByName("SnippingTool").Length = 0)
            Else
                WaitForProgramToStart = (Diagnostics.Process.GetProcessesByName("ScreenClippingHost").Length = 0)
            End If

            Application.DoEvents()
            System.Threading.Thread.Sleep(20)

        End While

        'wait for the snipping tool to end or for an image to be put in the clipboard

        While ContinueWaitingForImage

            If Clipboard.ContainsImage() Then
                gOriginalImage = Clipboard.GetImage()
                Exit While
            End If

            If Method = "W11" Then

                If Diagnostics.Process.GetProcessesByName("ScreenClippingHost").Length = 0 Then
                    Exit While
                End If

            Else

                If Diagnostics.Process.GetProcessesByName("SnippingTool").Length = 0 Then
                    Exit While
                End If

            End If

            If Me.WindowState = FormWindowState.Normal Then
                Exit While
            End If

            Threading.Thread.Sleep(50)
            Application.DoEvents()

        End While

        ContinueWaitingForImage = False ' leave this line here, this value is importantant to picturebox1.click

        If Clipboard.ContainsImage() Then
            gFilePathNameAndExtention = "In memory from clipboard"
            Safely_ProcessFile_Public(gFilePathNameAndExtention)
            returnValue = True
        Else
            returnValue = False
        End If

        Return returnValue

    End Function


#Region "Load by opening an image On the screen"

    Private Sub LoadFromImageOnScreen()

        Me.WindowState = FormWindowState.Minimized

        While Me.WindowState <> FormWindowState.Minimized
            System.Threading.Thread.Sleep(10)
            Application.DoEvents()
        End While

        'wait for click; also be sure to use Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.click

        While (User32.GetAsyncKeyState(1) = 0) AndAlso (User32.GetAsyncKeyState(2) = 0)

            System.Threading.Thread.Sleep(10)
            Application.DoEvents()

        End While

        'Get the handle of the control under the mouse 
        Dim HandleOfDesktop As IntPtr = User32.GetDesktopWindow
        Dim HandleOfControlUnderMouse As IntPtr = User32.WindowFromPoint(MousePosition)

        Try

            ' Step 1 - Crop the image under the mouse out of the full screen image

            ' Saddly the following does not work:

            ' Dim WindowImage As Image = CaptureWindow(HandleOfControlUnderMouse)
            ' Dim WindowBitmap As Bitmap = New Bitmap(WindowImage)
            ' WindowBitmap.Save(TempFileName, ImageFormat.Bmp)

            ' Howevever, a full screen capture does work. 
            ' So take a full screen capture and carve out of it the window of the HandleOfControlUnderMouse

            Dim FullScreenImage As Image = CaptureScreen()

            Dim RectangleOfFullScreen As User32.RECT
            User32.GetWindowRect(HandleOfDesktop, RectangleOfFullScreen)

            Dim RectangleUnderMouse As User32.RECT
            User32.GetWindowRect(HandleOfControlUnderMouse, RectangleUnderMouse)

            If RectangleUnderMouse.top < 0 Then RectangleUnderMouse.top = 0
            If RectangleUnderMouse.bottom < 0 Then RectangleUnderMouse.bottom = 0
            If RectangleUnderMouse.left < 0 Then RectangleUnderMouse.left = 0
            If RectangleUnderMouse.right < 0 Then RectangleUnderMouse.right = 0

            Dim FullScreenBitmap As Bitmap = New Bitmap(FullScreenImage)

            Dim format As PixelFormat = FullScreenImage.PixelFormat

            Dim CutOutRect As New Rectangle(RectangleUnderMouse.left, RectangleUnderMouse.top, RectangleUnderMouse.right - RectangleUnderMouse.left, RectangleUnderMouse.bottom - RectangleUnderMouse.top)
            Dim CutOutBitMap As Bitmap = FullScreenBitmap.Clone(CutOutRect, format)

            FullScreenImage.Dispose()
            FullScreenBitmap.Dispose()

            GarbageCollect()

            gOriginalImage = CType(New Bitmap(CutOutBitMap), Bitmap)

            'Step 2 - Crop away any boarders surrounding the image under the mouse

            CutOutRect = GetTrimmedBoardersFromBitmap(gOriginalImage)

            If CutOutRect = New Rectangle(-1, -1, -1, -1) Then
                'image was to small
            Else
                CutOutBitMap = CutOutBitMap.Clone(CutOutRect, format)
                gOriginalImage = New Bitmap(CutOutBitMap)
            End If

            gFilePathNameAndExtention = "In memory screenshot"
            Safely_ProcessFile_Public(gFilePathNameAndExtention)

            CutOutBitMap.Dispose()

            GarbageCollect()

        Catch ex As Exception

            Beep()

        End Try

        Me.WindowState = FormWindowState.Normal

    End Sub

#End Region

#Region "Process the loaded image"

    Public Sub Safely_ProcessFile_Public(ByVal Filename As String)

        DelegateFileName = Filename
        Call Me.BeginInvoke(Me.Safely_ProcessFile_Private)

    End Sub

    Private DelegateFileName As String = String.Empty
    Private Safely_ProcessFile_Private As New MethodInvoker(AddressOf Me.ProcessFile)

    Private Sub ProcessFile()

        SeCursor(Me, CursorState.Wait)

        Try

            If DelegateFileName.StartsWith("In memory") Then

            Else

                Dim MagickReadingSettings = New MagickReadSettings

                'MagickReadingSettings.Format = MagickFormat.Cr2 

                Dim Defines = New Formats.DngReadDefines
                Defines.UseCameraWhitebalance = True
                Defines.UseAutoWhitebalance = True
                Defines.DisableAutoBrightness = True

                MagickReadingSettings.Defines = Defines

                Using image = New MagickImage(gFilePathNameAndExtention, MagickReadingSettings)

                    image.Quality = 100

                    image.Settings.Compression = CompressionMethod.NoCompression
                    image.Format = MagickFormat.Bmp

                    Using memStream = New MemoryStream
                        image.Write(memStream)
                        gOriginalImage = New Bitmap(memStream)
                    End Using

                    ' image.Write("c:\temp\converted_with_magickimage.jpg")

                End Using

            End If

            'If DelegateFileName.StartsWith("In memory") Then

            'Else

            '    GarbageCollect()
            '    gOriginalImage = CType(Image.FromFile(gFilePathNameAndExtention), Bitmap)

            'End If

            gCommonWindowLocation = Me.Location

            Me.Hide()

            Dim FinalForm As frmViewer = New frmViewer
            FinalForm.ShowDialog()

            If gOriginalImage IsNot Nothing Then gOriginalImage.Dispose()
            If gCorrectedImage IsNot Nothing Then gCorrectedImage.Dispose()
            If gOriginalImageAsFirstLoaded IsNot Nothing Then gOriginalImageAsFirstLoaded.Dispose()

            Me.Show()

            MakeWindowAlwaysOnTopAsNeeded()

            GarbageCollect()

        Catch ex As Exception

            Beep()
            MessageBox.Show(ex.Message, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Try

        SeCursor(Me, CursorState.Normal)

    End Sub

    Private Function GetTrimmedBoardersFromBitmap(ByRef img As Bitmap) As Rectangle

        Const IgnoreEdge As Integer = 3

        Dim w As Integer = img.Width
        Dim h As Integer = img.Height

        Dim boarder As Rectangle = New Rectangle(-1, -1, -1, -1)

        If (w < (2 * IgnoreEdge)) OrElse (h < (2 * IgnoreEdge)) Then


        Else

            ' Find left boarder
            For x = IgnoreEdge To w - IgnoreEdge

                Dim TopMostPixelColour As Color = img.GetPixel(x, IgnoreEdge)
                Dim TestPixelColour As Color

                For y = IgnoreEdge To h - IgnoreEdge

                    TestPixelColour = img.GetPixel(x, y)

                    If Not AreColorsEqual(TopMostPixelColour, TestPixelColour) Then
                        boarder.X = x
                        Exit For
                    End If

                Next

                If boarder.X > 0 Then Exit For

            Next

            ' Find right boarder
            For x = w - IgnoreEdge To boarder.X Step -1

                Dim BottomMostPixelColour As Color = img.GetPixel(x, IgnoreEdge)
                Dim TestPixelColour As Color

                For y = IgnoreEdge To h - IgnoreEdge

                    TestPixelColour = img.GetPixel(x, y)

                    If Not AreColorsEqual(BottomMostPixelColour, TestPixelColour) Then
                        boarder.Width = x - boarder.X
                        Exit For
                    End If

                Next

                If boarder.Width > 0 Then Exit For

            Next

            ' Find top boarder

            For y = IgnoreEdge To h - IgnoreEdge

                Dim TopMostPixelColour As Color = img.GetPixel(boarder.X, y)
                Dim TestPixelColour As Color

                For x = boarder.X To boarder.X + boarder.Width

                    TestPixelColour = img.GetPixel(x, y)

                    If Not AreColorsEqual(TopMostPixelColour, TestPixelColour) Then
                        boarder.Y = y
                        Exit For
                    End If

                Next

                If boarder.Y > 0 Then Exit For

            Next

            ' Find bottom boarder

            For y = h - IgnoreEdge To boarder.Y Step -1

                Dim TopMostPixelColour As Color = img.GetPixel(boarder.X, y)
                Dim TestPixelColour As Color

                For x = boarder.X To boarder.X + boarder.Width

                    TestPixelColour = img.GetPixel(x, y)

                    If Not AreColorsEqual(TopMostPixelColour, TestPixelColour) Then
                        boarder.Height = y - boarder.Y
                        Exit For
                    End If

                Next

                If boarder.Height > 0 Then Exit For

            Next

        End If

        Return boarder

    End Function

    Private Function AreColorsEqual(ByVal c1 As Color, ByVal c2 As Color) As Boolean

        Const threshold As Integer = 3

        Return (Math.Abs(CInt(c1.R) - CInt(c2.R)) < threshold) AndAlso (Math.Abs(CInt(c1.G) - CInt(c2.G)) < threshold) AndAlso (Math.Abs(CInt(c1.B) - CInt(c2.B)) < threshold)

    End Function

#End Region


#Region "Filewatcher"

    Private Sub SetupForFileWatcherMonitoring()

        Try

            Const gFileWatcherFilterName As String = "*.*"  ' must use *.* to pick up chnages impated the whole directory directory changes - for example a directory delete; unfortunately cannot restrict to .av4w files only 
            Dim gFileWatcherPathName As String = gXML_Path_Name
            Dim gFileWatcherPathAndFileName As String = gFileWatcherPathName & "\" & gFileWatcherFilterName

            Dim fw As New FileSystemWatcher
            fw.Path = gFileWatcherPathName
            fw.IncludeSubdirectories = True
            fw.Filter = gFileWatcherFilterName
            fw.NotifyFilter = NotifyFilters.Attributes Or
                                       NotifyFilters.CreationTime Or
                                       NotifyFilters.DirectoryName Or
                                       NotifyFilters.FileName Or
                                       NotifyFilters.LastAccess Or
                                       NotifyFilters.LastWrite Or
                                       NotifyFilters.Security Or
                                       NotifyFilters.Size

            AddHandler fw.Created, AddressOf FileWatcherDetectedAFileWasCreated
            AddHandler fw.Deleted, AddressOf FileWatcherDetectedAFileWasDeleted
            AddHandler fw.Renamed, AddressOf FileWatcherDetectedAFileWasRenamed
            AddHandler fw.Changed, AddressOf FileWatcherDetectedAFileWasChanged

            'AddHandler fw.Error, AddressOf FileWatcherError

            fw.EnableRaisingEvents = True

        Catch ex As Exception
        End Try

    End Sub

    Friend Sub FileWatcherDetectedAFileWasCreated(ByVal source As Object, ByVal e As FileSystemEventArgs)

        If Now < gignoreFileWatcherEventUntilThisTime Then
            ' ignore file watcher events generated by this instance
        Else

            If MultipleFileWatcherEventsTimer.Enabled Then
            Else
                ' wait for three seconds for any additional file watcher notifications to be come in (and ignored)
                ' this allows time, for example, for a bulk copy to be completed
                BeginInvoke(New MethodInvoker(AddressOf WaitForThreeSeconds))
            End If

        End If

    End Sub

    Friend Sub FileWatcherDetectedAFileWasDeleted(ByVal source As Object, ByVal e As FileSystemEventArgs)

        If Now < gignoreFileWatcherEventUntilThisTime Then
            ' ignore file watcher events generated by this instance
        Else

            If MultipleFileWatcherEventsTimer.Enabled Then
            Else
                ' wait for three seconds for any additional file watcher notifications to be come in (and ignored)
                ' this allows time, for example, for a bulk copy to be completed
                BeginInvoke(New MethodInvoker(AddressOf WaitForThreeSeconds))
            End If

        End If

    End Sub

    Friend Sub FileWatcherDetectedAFileWasRenamed(sender As Object, e As RenamedEventArgs)

        If Now < gignoreFileWatcherEventUntilThisTime Then
            ' ignore file watcher events generated by this instance
        Else

            If MultipleFileWatcherEventsTimer.Enabled Then
            Else
                ' wait for three seconds for any additional file watcher notifications to be come in (and ignored)
                ' this allows time, for example, for a bulk copy to be completed
                BeginInvoke(New MethodInvoker(AddressOf WaitForThreeSeconds))
            End If

        End If

    End Sub

    Friend Sub FileWatcherDetectedAFileWasChanged(ByVal sender As Object, e As FileSystemEventArgs)

        If Now < gignoreFileWatcherEventUntilThisTime Then
        Else

            If MultipleFileWatcherEventsTimer.Enabled Then
            Else
                BeginInvoke(New MethodInvoker(AddressOf WaitForThreeSeconds))
            End If

        End If

    End Sub

    'Friend Sub FileWatcherError(ByVal source As Object, ByVal e As System.IO.ErrorEventArgs)

    'End Sub

    Private Sub WaitForThreeSeconds()

        MultipleFileWatcherEventsTimer.Enabled = True
        MultipleFileWatcherEventsTimer.Interval = 3000
        MultipleFileWatcherEventsTimer.Start()

    End Sub

    Private Sub MultipleFileWatcherEventsTimer_Tick(sender As Object, e As EventArgs) Handles MultipleFileWatcherEventsTimer.Tick

        MultipleFileWatcherEventsTimer.Stop()
        MultipleFileWatcherEventsTimer.Enabled = False

        If Me.Visible Then

            ' nothing to do if opening window is visible

        Else

            If Now < gignoreFileWatcherEventUntilThisTime Then

            Else
                gProfileRevalidationRequired = True
                RaiseAnAV4WNotificationEventStemmingFromAFileWatcherNofitication()
            End If

        End If

    End Sub

#End Region

    Private Sub CopyrightAndLicenseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles tsCopyrightAndLicense.Click

        MessageBox.Show("A Viewer for Windows" & vbCrLf &
                        "Copyright Rob Latour, 2024" & vbCrLf & vbCrLf &
                        "License: " & vbCrLf &
                        "MIT" & vbCrLf & vbCrLf &
                        "Credits:" & vbCrLf &
                        "A Viewer for Window uses ImageMagick: Magick.NET" & vbCrLf &
                        "ImageMagick: Magick.NET is licensed under Apache License Version 2.0." & vbCrLf & vbCrLf &
                        "Source:" & vbCrLf &
                        "https://github.com/roblatour/AViewerForWindows",
                        gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Sub WebsiteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles tsWebsite.Click

        System.Diagnostics.Process.Start(gWebpage)

    End Sub

    Private Sub DonateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles tsDonate.Click

        System.Diagnostics.Process.Start(gWebpageDonate)

    End Sub

    Private Sub HelpOnlineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpOnlineToolStripMenuItem.Click

        System.Diagnostics.Process.Start(gWebpageHelp)

    End Sub

    Private Sub CommunitySupportForumToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles CommunitySupportForumToolStripMenuItem.Click

        System.Diagnostics.Process.Start(gSupportForum)

    End Sub

    Private Sub QuickHelpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QuickHelpToolStripMenuItem.Click

        MessageBox.Show("A Viewer for Windows is a free lightweight tool which helps you view, change and save images." & vbCrLf & vbCrLf &
                     "The program has three primary windows. " & vbCrLf & vbCrLf &
                     "The first allows you to open an image, the second allows you to view, change, and save it, and the third allows you to manage your 'Effects' (like bluring, sharpening and embossing)." & vbCrLf & vbCrLf &
                     "The balance of this write-up covers the help for the first window. Help for the second and third is covered by clicking on 'Quick Help' on those windows (or by holding down the 'Alt' key and clicking on 'Quick Help' to view the help online)." & vbCrLf & vbCrLf &
                     "There are five ways to open an image. The first three are quite common: drag and drop, open from an existing file using a file selector, and open from your clipboard." & vbCrLf & vbCrLf &
                     "The fourth way is to use Microsoft's snipping tool.  Just click the image of the scissors, and then A Viewer for Windows' first window will minimize and Microsoft's snipping tool will appear.  Once you have snipped the image it will load into the second window." & vbCrLf & vbCrLf &
                     "The fifth is a somewhat experimental.  Basically, it assumes you already have an image open in another program, like a browser, on your primary screen." &
                     " If you do then just click where it says 'Click here to select an image already shown on the screen'." &
                     " Once done, the first window will be minimized." &
                     " Next, just click on the image that is already being shown on your primary screen and it will Computeally load." & vbCrLf & vbCrLf &
                     "If however A Viewer for Windows cannot load the image the original window will reappear." & vbCrLf & vbCrLf &
                     "You can learn more about A Viewer for Windows by clicking the other options under the About/Help menu." & vbCrLf & vbCrLf &
                     "You can also check the option to 'Automatatically check for updates' so the program can let you know if a new version becomes available." & vbCrLf & vbCrLf &
                     "I hope A Viewer for Windows will be of good use to you, and you are welcome to use it for free on as many computers as you like." & vbCrLf & vbCrLf &
                     "Rob Latour",
                     gThisProgramName & gVersionInUse, MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

End Class

