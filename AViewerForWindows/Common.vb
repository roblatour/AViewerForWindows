Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Module Common

    Friend Const gThisProgramName As String = "A Viewer for Windows"
    Friend gVersionInUse As String

    Friend gBetaVersion As String = String.Empty ' **** For production
    'Friend gBetaVersion As String = " beta b" ' *************************************

    Friend Const gWebpage As String = "https://rlatour.com/av4w"
    Friend gWebpageHelp As String = "https://rlatour.com/av4w/help/help_version.html"
    Friend Const gWebpageDonate As String = "https://rlatour.com/av4w/donate.html"
    Friend Const gSupportForum As String = "https://www.push2run.com/phpbb/"
    Friend Const gARulerForWindows As String = "C:\Program Files\A Ruler for Windows\ARuler.exe"
    Friend Const gARulerForWindowswebite As String = "https://arulerforwindows.com/"

    Friend Const cLetters As String = "etaoinshrdlcumwfgypbvkjxqzETAOINSHRDLCUMWFGYPBVKJXQZ"
    Friend Const cNumbers As String = "0123456789"

    Friend gucSliderReturnValue As String

    Friend gignoreFileWatcherEventUntilThisTime As DateTime
    Friend Const gFiveSeconds As Integer = 5000

    Friend gColourLiteral As String
    Friend gCentreLiteral As String

    Friend gOriginalAlpha As Integer
    Friend gCurrentAlpha As Integer

    Friend gMyTransparencyColour As Color
    Friend gMyTransparencyBrushBasedOnAnUnusedColour As SolidBrush

    Friend gMyFramingColour As Color ' = Color.FromArgb(0, 255, 255, 255) 
    Friend gMyFramingBrush As SolidBrush

    Friend gListOfAllProfiles As New List(Of String)
    Friend gListOfEnabledProfiles As New List(Of String)

    Friend gMaxEffectsSupported As Integer = 5
    Friend gCurrentNamesOfEffectsInUse(gMaxEffectsSupported) As String
    Friend gCurrentNumberOfEffectsBoxesInUse As Integer = 0
    Friend gEffectsBoxesAreHidden As Boolean = True
    Friend Const gDefaultEffectNotInUse = "*None selected*"

    Friend gRunningOnA4KMonitor As Boolean
    Friend gOpenOkedByUser As Boolean
    Friend gLinkToBeOpened As String

    Friend gTrialImage As Bitmap
    Friend gMovingPoint As Point = New Point(0, 0)

    Friend gMiniCheckerBoardImage As Bitmap
    Friend gMiniCheckerBoardTextureBrush As TextureBrush

    Friend gProfileRevalidationRequired As Boolean = False
    Friend gProfileWasChangedAccordingToFileWatcher As Boolean = False

    Friend gLatestArrowDirectionIsDown As Boolean

    Friend gRefreshingTreeView As Boolean = False

    Friend gTreeViewFolderTagDesignation As String = ":Folder:"

    Friend Const gMaxMatrixSize As Integer = 7
    Friend Enum gNamingFunctions

        add = 1
        copy = 2
        rename = 3

    End Enum

    'Friend gTransparencentProcessingOption As gTransparencentProcessingOptions

    Friend gNamingFunction As gNamingFunctions
    Friend gRenameFunctionOriginalName As String
    Friend gRenameFunctionProposedName As String
    Friend gRenameFunctionNewName As String

    Friend gMagnifierFound As Boolean
    Friend gMagnifierProgram As String = String.Empty

    Friend gValidFileTypesForReading As New List(Of String)
    Friend gValidFileTypesForWriting As New List(Of String)

    Friend gValidFileTypesReadingFilter As String
    Friend gValidFileTypesWritingFilter As String

    Friend gOriginalImageAsFirstLoaded As System.Drawing.Bitmap
    Friend gOriginalImage As System.Drawing.Bitmap

    Friend gCorrectedImage As System.Drawing.Bitmap
    Friend gDummyForRotationAndZoomingOnlyImage As System.Drawing.Bitmap

    Friend Const gBrightnessDefault As Single = 0
    Friend gCurrentBrightness As Single = gBrightnessDefault

    Friend Const gContrastDefault As Single = 1
    Friend gCurrentContrast As Single = gContrastDefault

    Friend Const gGammaDefault As Single = 1
    Friend gCurrentGamma As Single = gGammaDefault

    Friend gGrayscaleDefault As Boolean = False
    Friend gCurrentImageIsInGrayscale As Boolean = False

    Friend gInvertedDefault As Boolean = False
    Friend gCurrentImageIsInverted As Boolean = False

    Friend Const gCurrentImageIsMirroredHorizontallyDefault As Boolean = False
    Friend gCurrentImageIsMirroredHorizontally As Boolean = False

    Friend Const gCurrentImageIsMirroredVerticallyDefault As Boolean = False
    Friend gCurrentImageIsMirroredVertically As Boolean = False

    Friend Const gSaturationDefault As Single = 0.5
    Friend gCurrentSaturation As Single = gSaturationDefault

    Friend Const gRotationDefault As Single = 0
    Friend gCurrentRotation As Single = 0

    Friend Const gZoomDefault As Single = 1
    Friend gCurrentZoom As Single = gZoomDefault

    Friend gCommonWindowLocation As System.Drawing.Point

    Friend gFilePathNameAndExtention As String = String.Empty

    Friend gAlwaysOnTop As Boolean

    Friend gSuspendPictureBoxUpdates As Boolean = False

    Friend gfrmPickEffectsStartLocation As New Point

    Friend Const gMyExtention = ".av4w"
    Friend gXML_Path_Name As String

    Friend gTweakedStartingLocationDueToResizeOriginalForRotation As Point

    Friend Structure gEffectsProfileStructure
        Friend Name As String
        Friend Enabled As Boolean
        Friend MatrixType As String
        Friend MatrixSize As Integer
        Friend Sigma As Double
        Friend Amount As Double
        Friend Bias As Double
        Friend Factor As Double
        Friend Compute As Boolean
        Friend Symmetric As Boolean
        Friend MatrixString As String
        Friend MatrixTable(,) As Double
        Friend ModifyAlpha As Integer
        Friend ModifyRed As Integer
        Friend ModifyGreen As Integer
        Friend ModifyBlue As Integer
        Friend TransparencyEnabled As Boolean
        Friend TransparencyRed As Integer
        Friend TransparencyGreen As Integer
        Friend TransparencyBlue As Integer
        Friend TransparencyTolerance As Integer
        Friend Notes As String
    End Structure

    Friend gCurrentEffectsProfile As gEffectsProfileStructure

    Friend gAllEffectsProiles() As gEffectsProfileStructure

    Friend Const gDefaultCompute As Boolean = False
    Friend Const gDefaultSymmetric As Boolean = False

    Friend Const gNeutralEffectsProfileName As String = "Defaults\Neutral"

    Friend Enum ViewingOption
        PrimaryOnly = 1
        PrimaryandRight = 2
        PrimaryandBottom = 3
        OriginalOnly = 4
    End Enum

    Friend gViewChoice As ViewingOption = ViewingOption.PrimaryOnly

    Friend gCapsLock As Boolean
    Friend gNumbLock As Boolean
    Friend gScrollLock As Boolean

    Friend Event AnAV4WNotificationEvent()
    Friend Sub RaiseAnAV4WNotificationEventStemmingFromAFileWatcherNofitication()

        RaiseEvent AnAV4WNotificationEvent()

    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Friend Function QuickFilter(ByVal InputString As String, ByVal cAllowableCharacters As String) As String

        Dim returnValue As String = String.Empty

        For x As Int32 = 1 To Len(InputString)
            If cAllowableCharacters.Contains(Mid(InputString, x, 1)) Then
                returnValue = returnValue & Mid(InputString, x, 1)
            End If
        Next

        Return returnValue

    End Function

    Friend Sub GetLockKeyStates()

        gCapsLock = My.Computer.Keyboard.CapsLock
        gNumbLock = My.Computer.Keyboard.NumLock
        gScrollLock = My.Computer.Keyboard.ScrollLock

    End Sub

    Friend Function LoadEffectsMatrixTableFromString() As Boolean

        Dim returnValue As Boolean = False

        Dim WorkingEffectsMatrix(gMaxMatrixSize - 1, gMaxMatrixSize - 1) As Double

        Try

            Dim MatrixTable() As String = gCurrentEffectsProfile.MatrixString.Split(" ")

            Dim i As Integer = 0

            If MatrixTable.Length = 49 Then

                ' load a 7x7 matrix

                For x As Integer = 0 To gMaxMatrixSize - 1
                    For y As Integer = 0 To gMaxMatrixSize - 1
                        WorkingEffectsMatrix(x, y) = MatrixTable(i)
                        If WorkingEffectsMatrix(x, y) > 255 Then WorkingEffectsMatrix(x, y) = 255
                        If WorkingEffectsMatrix(x, y) < -255 Then WorkingEffectsMatrix(x, y) = -255
                        i += 1
                    Next
                Next

            Else

                ' convert 5x5 to 7x7 and load it
                For x As Integer = 0 To gMaxMatrixSize - 1
                    For y As Integer = 0 To gMaxMatrixSize - 1
                        WorkingEffectsMatrix(x, y) = 0
                    Next
                Next

                For x = 0 To 4
                    For y = 0 To 4
                        WorkingEffectsMatrix(x + 1, y + 1) = MatrixTable(i)
                        If WorkingEffectsMatrix(x + 1, y + 1) > 255 Then WorkingEffectsMatrix(x + 1, y + 1) = 255
                        If WorkingEffectsMatrix(x + 1, y + 1) < -255 Then WorkingEffectsMatrix(x + 1, y + 1) = -255
                        i += 1
                    Next
                Next

            End If

            If gCurrentEffectsProfile.Symmetric Then

                ' enforce Symmetry

                WorkingEffectsMatrix(6, 0) = WorkingEffectsMatrix(0, 0)
                WorkingEffectsMatrix(6, 6) = WorkingEffectsMatrix(0, 0)
                WorkingEffectsMatrix(0, 6) = WorkingEffectsMatrix(0, 0)

                WorkingEffectsMatrix(6, 1) = WorkingEffectsMatrix(0, 1)
                WorkingEffectsMatrix(6, 5) = WorkingEffectsMatrix(0, 1)
                WorkingEffectsMatrix(0, 5) = WorkingEffectsMatrix(0, 1)
                WorkingEffectsMatrix(1, 0) = WorkingEffectsMatrix(0, 1)
                WorkingEffectsMatrix(5, 0) = WorkingEffectsMatrix(0, 1)
                WorkingEffectsMatrix(1, 6) = WorkingEffectsMatrix(0, 1)
                WorkingEffectsMatrix(5, 6) = WorkingEffectsMatrix(0, 1)

                WorkingEffectsMatrix(6, 2) = WorkingEffectsMatrix(0, 2)
                WorkingEffectsMatrix(6, 4) = WorkingEffectsMatrix(0, 2)
                WorkingEffectsMatrix(0, 4) = WorkingEffectsMatrix(0, 2)
                WorkingEffectsMatrix(2, 0) = WorkingEffectsMatrix(0, 2)
                WorkingEffectsMatrix(4, 0) = WorkingEffectsMatrix(0, 2)
                WorkingEffectsMatrix(2, 6) = WorkingEffectsMatrix(0, 2)
                WorkingEffectsMatrix(4, 6) = WorkingEffectsMatrix(0, 2)

                WorkingEffectsMatrix(3, 0) = WorkingEffectsMatrix(0, 3)
                WorkingEffectsMatrix(6, 3) = WorkingEffectsMatrix(0, 3)
                WorkingEffectsMatrix(3, 6) = WorkingEffectsMatrix(0, 3)

                WorkingEffectsMatrix(5, 0) = WorkingEffectsMatrix(1, 0)
                WorkingEffectsMatrix(5, 6) = WorkingEffectsMatrix(1, 0)
                WorkingEffectsMatrix(1, 6) = WorkingEffectsMatrix(1, 0)

                WorkingEffectsMatrix(5, 1) = WorkingEffectsMatrix(1, 1)
                WorkingEffectsMatrix(5, 5) = WorkingEffectsMatrix(1, 1)
                WorkingEffectsMatrix(1, 5) = WorkingEffectsMatrix(1, 1)

                WorkingEffectsMatrix(5, 2) = WorkingEffectsMatrix(1, 2)
                WorkingEffectsMatrix(5, 4) = WorkingEffectsMatrix(1, 2)
                WorkingEffectsMatrix(1, 4) = WorkingEffectsMatrix(1, 2)
                WorkingEffectsMatrix(2, 1) = WorkingEffectsMatrix(1, 2)
                WorkingEffectsMatrix(4, 1) = WorkingEffectsMatrix(1, 2)
                WorkingEffectsMatrix(4, 5) = WorkingEffectsMatrix(1, 2)
                WorkingEffectsMatrix(2, 5) = WorkingEffectsMatrix(1, 2)

                WorkingEffectsMatrix(3, 1) = WorkingEffectsMatrix(1, 3)
                WorkingEffectsMatrix(5, 3) = WorkingEffectsMatrix(1, 3)
                WorkingEffectsMatrix(3, 5) = WorkingEffectsMatrix(1, 3)

                WorkingEffectsMatrix(4, 2) = WorkingEffectsMatrix(2, 2)
                WorkingEffectsMatrix(4, 4) = WorkingEffectsMatrix(2, 2)
                WorkingEffectsMatrix(2, 4) = WorkingEffectsMatrix(2, 2)

                WorkingEffectsMatrix(3, 2) = WorkingEffectsMatrix(2, 3)
                WorkingEffectsMatrix(4, 3) = WorkingEffectsMatrix(2, 3)
                WorkingEffectsMatrix(3, 4) = WorkingEffectsMatrix(2, 3)

            End If

            returnValue = True

        Catch ex As Exception

            LoadNeutralEffectsProfile(True)
            LoadEffectsMatrixTableFromString()

        End Try

        ReDim gCurrentEffectsProfile.MatrixTable(gMaxMatrixSize, gMaxMatrixSize)  ' enure matrix is 7x7
        gCurrentEffectsProfile.MatrixTable = WorkingEffectsMatrix

        Return returnValue

    End Function

    Friend Sub LoadAndSaveNeutralEffectsProfile(ByVal UpdateSettings As Boolean)

        LoadNeutralEffectsProfile(UpdateSettings)
        SaveCurrentEffectsProfile()

    End Sub

    Const gNeutralMatrixString7x7 As String = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
    Const gNeutralMatrixString5x5 As String = "0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0"
    Const gNeutralMatrixString3x3 As String = "0 0 0 0 1 0 0 0 0"
    Friend Sub LoadNeutralEffectsProfile(ByVal UpdateSettings As Boolean)

        With gCurrentEffectsProfile

            .Name = gNeutralEffectsProfileName
            .Enabled = False
            .MatrixType = "Effect"
            .MatrixSize = 7
            .Sigma = 1
            .Amount = 1
            .Bias = 0
            .Factor = 1
            .Compute = False
            .Symmetric = True
            .MatrixString = gNeutralMatrixString7x7
            .MatrixTable = New Double(,) {{0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 1, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0}}
            .ModifyAlpha = 0
            .ModifyRed = 0
            .ModifyGreen = 0
            .ModifyBlue = 0
            .TransparencyEnabled = False
            .TransparencyRed = 0
            .TransparencyGreen = 0
            .TransparencyBlue = 0
            .TransparencyTolerance = 0
            .Notes = ""

        End With

        If UpdateSettings Then

            My.Settings.EffectsProfileName = gNeutralEffectsProfileName
            My.Settings.Save()

        End If

    End Sub

    Friend Sub SaveCurrentEffectsProfile()

        Try

            Dim XML_File_Name As String = gXML_Path_Name & "\" & gCurrentEffectsProfile.Name & gMyExtention

            If Directory.Exists(Path.GetDirectoryName(XML_File_Name)) Then
            Else
                gignoreFileWatcherEventUntilThisTime = Now.AddMilliseconds(gFiveSeconds)
                Directory.CreateDirectory(Path.GetDirectoryName(XML_File_Name))
            End If

            Dim SaveProfile As EffectsProfile = New EffectsProfile

            With SaveProfile

                .Name = gCurrentEffectsProfile.Name
                .Enabled = gCurrentEffectsProfile.Enabled
                .MatrixType = gCurrentEffectsProfile.MatrixType
                .MatrixSize = gCurrentEffectsProfile.MatrixSize
                .Sigma = gCurrentEffectsProfile.Sigma
                .Amount = gCurrentEffectsProfile.Amount
                .Bias = gCurrentEffectsProfile.Bias
                .Factor = gCurrentEffectsProfile.Factor
                .Compute = gCurrentEffectsProfile.Compute
                .Symmetric = gCurrentEffectsProfile.Symmetric
                .Matrix = gCurrentEffectsProfile.MatrixString
                .ModifyAlpha = gCurrentEffectsProfile.ModifyAlpha
                .ModifyRed = gCurrentEffectsProfile.ModifyRed
                .ModifyGreen = gCurrentEffectsProfile.ModifyGreen
                .ModifyBlue = gCurrentEffectsProfile.ModifyBlue
                .TransparencyEnabled = gCurrentEffectsProfile.TransparencyEnabled
                .TransparencyRed = gCurrentEffectsProfile.TransparencyRed
                .TransparencyGreen = gCurrentEffectsProfile.TransparencyGreen
                .TransparencyBlue = gCurrentEffectsProfile.TransparencyBlue
                .TransparencyTolerance = gCurrentEffectsProfile.TransparencyTolerance
                .Notes = gCurrentEffectsProfile.Notes

            End With

            XMLSave(SaveProfile, XML_File_Name)

            SaveProfile = Nothing

        Catch ex As Exception
            MessageBox.Show("Save failed" & vbCrLf & vbCrLf & ex.Message.ToString, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try

    End Sub

    Private Sub XMLSave(ByVal SaveProfile As EffectsProfile, ByVal Filename As String)

        Try

            gignoreFileWatcherEventUntilThisTime = Now.AddMilliseconds(gFiveSeconds)
            XML.ObjectXMLSerializer(Of EffectsProfile).Save(SaveProfile, Filename)

        Catch ex As Exception

            MessageBox.Show("Save failed" & vbCrLf & vbCrLf & ex.Message.ToString, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Try

    End Sub

    Friend Function LoadEffectsProfile(ByVal XML_File_Name As String, Optional ByVal DontUpdateFilename As Boolean = False, Optional ByRef LoadedProfileName As String = "") As Boolean

        Dim returnValue As Boolean = False

        If XML_File_Name Is Nothing Then Return False

        Try

            If DontUpdateFilename Then
            Else
                XML_File_Name = ConvertProfileNameToXMLFileName(XML_File_Name)
            End If

            If System.IO.File.Exists(XML_File_Name) Then

                Dim EffectsProfile As EffectsProfile

                Dim FileIsCorrupt As Boolean

                Try
                    EffectsProfile = XML.ObjectXMLSerializer(Of EffectsProfile).Load(XML_File_Name)
                    FileIsCorrupt = False
                Catch ex As Exception
                    FileIsCorrupt = True
                End Try

                If FileIsCorrupt Then

                    returnValue = False

                Else

                    With gCurrentEffectsProfile

                        .Name = EffectsProfile.Name
                        .Enabled = EffectsProfile.Enabled

                        If EffectsProfile.MatrixType Is Nothing Then
                            EffectsProfile.MatrixType = "Effect" ' ' required for conversion between v1.6 and v1.7
                            EffectsProfile.Sigma = 1
                        End If
                        .MatrixType = EffectsProfile.MatrixType

                        If EffectsProfile.MatrixSize = 0 Then EffectsProfile.MatrixSize = 5  ' required for conversion between v1.6 and v1.7
                        .MatrixSize = EffectsProfile.MatrixSize

                        .Sigma = EffectsProfile.Sigma
                        .Amount = EffectsProfile.Amount
                        .Bias = EffectsProfile.Bias
                        .Factor = EffectsProfile.Factor
                        .Compute = EffectsProfile.Compute
                        .Symmetric = EffectsProfile.Symmetric
                        .MatrixString = EffectsProfile.Matrix
                        .ModifyAlpha = EffectsProfile.ModifyAlpha
                        .ModifyRed = EffectsProfile.ModifyRed
                        .ModifyBlue = EffectsProfile.ModifyBlue
                        .ModifyGreen = EffectsProfile.ModifyGreen
                        .TransparencyEnabled = EffectsProfile.TransparencyEnabled
                        .TransparencyRed = EffectsProfile.TransparencyRed
                        .TransparencyGreen = EffectsProfile.TransparencyGreen
                        .TransparencyBlue = EffectsProfile.TransparencyBlue
                        .TransparencyTolerance = EffectsProfile.TransparencyTolerance
                        .Notes = EffectsProfile.Notes

                    End With

                    LoadedProfileName = EffectsProfile.Name

                    returnValue = LoadEffectsMatrixTableFromString()

                End If

            Else

                returnValue = False

            End If

        Catch ex As Exception

        End Try

        Return returnValue

    End Function

    Friend Function IsThisProfileEnabled(ByVal XML_File_Name As String) As Boolean

        Dim returnValue As Boolean = False

        If XML_File_Name Is Nothing Then Return False

        Try

            XML_File_Name = ConvertProfileNameToXMLFileName(XML_File_Name)

            If System.IO.File.Exists(XML_File_Name) Then

                Dim EffectsProfile As EffectsProfile

                Dim FileIsCorrupt As Boolean

                Try
                    EffectsProfile = XML.ObjectXMLSerializer(Of EffectsProfile).Load(XML_File_Name)
                    FileIsCorrupt = False
                Catch ex As Exception
                    FileIsCorrupt = True
                End Try

                If FileIsCorrupt Then

                    returnValue = False

                Else

                    returnValue = EffectsProfile.Enabled

                End If

            End If

        Catch ex As Exception

        End Try

        Return returnValue

    End Function

    Friend Sub ValidateEffectsProfiles()

        gListOfAllProfiles.Clear()
        gListOfEnabledProfiles.Clear()

        Static Dim DoOncePerSession As Boolean = True

        gXML_Path_Name = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) & "\AV4W"

        Dim ValidProfiles As New List(Of String)

        Try

            ' make sure the path for storing effects profiles is establised
            ' as well, ensure default profiles are made available

            If Directory.Exists(gXML_Path_Name) Then
            Else
                Directory.CreateDirectory(gXML_Path_Name)
            End If

            If Directory.Exists(gXML_Path_Name & "\Defaults") Then
            Else
                Directory.CreateDirectory(gXML_Path_Name & "\Defaults")
            End If

            If DoOncePerSession Then
                DoOncePerSession = False
                ConvertLegacySharpeningFileNames()
                CopyIncludeFiles()
            End If

            Dim CorruptedProfileIdentier As String = "Corrupted " & Format(Now, "yyyy-MM-dd HH mm ss") & " "

            ' invetory all Profiles

            Dim allProfiles As New List(Of String)
            allProfiles.AddRange(Directory.GetFiles(gXML_Path_Name, "*.av4w", SearchOption.AllDirectories))
            allProfiles.RemoveAll(Function(s) s.Contains("Corrupted"))

            ' if there are no profiles, create the base profile directly
            ' also copy the Emboss and Gausian Blurr files
            ' both methods should work, however the two approaches below will help guarentee at least one file gets moved over

            If allProfiles.Count = 0 Then
                LoadAndSaveNeutralEffectsProfile(True)
                ValidProfiles.Add(gNeutralEffectsProfileName)
                Exit Try
            End If

            ' look for and address corruped profile files

            Dim EffectsProfile As New EffectsProfile
            Dim WorkingProfileName As String

            For Each fullfilename As String In allProfiles

                WorkingProfileName = fullfilename.Remove(0, gXML_Path_Name.Length + 1)
                WorkingProfileName = WorkingProfileName.Replace(gMyExtention, "")

                If WorkingProfileName.Length > 0 Then

                    Dim FileIsCorrupt As Boolean

                    Try
                        EffectsProfile = XML.ObjectXMLSerializer(Of EffectsProfile).Load(fullfilename)
                        FileIsCorrupt = False
                    Catch ex As Exception
                        FileIsCorrupt = True
                    End Try

                    If FileIsCorrupt Then

                        Beep()

                        Dim oldFilenameWithPath = fullfilename
                        Dim newFilenameWithoutPath As String = CorruptedProfileIdentier & Path.GetFileName(fullfilename)
                        Dim newFilenameWithPath = Path.GetDirectoryName(fullfilename) & "\" & newFilenameWithoutPath

                        If File.Exists(newFilenameWithPath) Then
                        Else
                            Try
                                My.Computer.FileSystem.RenameFile(oldFilenameWithPath, newFilenameWithoutPath)  'old filename contains path, new filenamedoes not
                            Catch ex As Exception
                            End Try
                        End If

                        Dim Dummy = MessageBox.Show("The Effects Profile:" & vbCrLf & vbCrLf &
                                fullfilename & vbCrLf & vbCrLf &
                                "is corrupt and cannot be loaded. It has beed renamed to:" & vbCrLf & vbCrLf &
                                 newFilenameWithoutPath, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                    Else

                        ValidProfiles.Add(WorkingProfileName)

                    End If

                End If

            Next

            ' Make all internal Profile Names match external file names
            ' Check to see that at least one of them is enabled

            Dim AtLeastOneEnabledProfileWasFound As Boolean = False
            Dim XMLFullFilename As String

            For Each ProfileName In ValidProfiles

                XMLFullFilename = ConvertProfileNameToXMLFileName(ProfileName)

                EffectsProfile = XML.ObjectXMLSerializer(Of EffectsProfile).Load(XMLFullFilename)

                If EffectsProfile.Enabled Then
                    AtLeastOneEnabledProfileWasFound = True
                    gListOfEnabledProfiles.Add(ProfileName)
                End If

                If EffectsProfile.Name <> ProfileName Then
                    EffectsProfile.Name = ProfileName
                    XMLSave(EffectsProfile, XMLFullFilename)
                End If

            Next

            ' If no enabled base profile was found, (re)create, save, and enable the base profile
            If AtLeastOneEnabledProfileWasFound Then
            Else
                LoadAndSaveNeutralEffectsProfile(True)
            End If

        Catch ex As Exception

        End Try

        'if the list does not contain the current user's default profile then make the base thier default profile
        If ValidProfiles.Contains(My.Settings.EffectsProfileName) Then
        Else
            My.Settings.EffectsProfileName = gNeutralEffectsProfileName
            LoadAndSaveNeutralEffectsProfile(True)
        End If

        gListOfEnabledProfiles = gListOfEnabledProfiles.Distinct.ToList
        gListOfEnabledProfiles.Sort()

        ValidProfiles = ValidProfiles.Distinct.ToList 'Remove duplicates (in the default was added a second time)
        ValidProfiles.Sort()

        ' gListOfAllProfiles will list all the profiles in sub directories first, then add the profiles in the root directory
        ' this to mimic how the effects box works on the viewer windows

        Dim rootProfiles As List(Of String) = ValidProfiles.ToList
        rootProfiles.RemoveAll(Function(s) s.Contains("\"))
        rootProfiles.Sort()

        gListOfAllProfiles = ValidProfiles.ToList
        gListOfAllProfiles.RemoveAll(Function(s) Not s.Contains("\"))
        gListOfAllProfiles.AddRange(rootProfiles)

        gProfileRevalidationRequired = False

    End Sub
    Friend Function ConvertProfileNameToXMLFileName(ByVal ProfileName As String) As String

        If ProfileName Is Nothing Then
            Return String.Empty
        Else
            If ProfileName.StartsWith(gXML_Path_Name) Then
                Return ProfileName ' already in the right format
            Else
                Return gXML_Path_Name & "\" & ProfileName & gMyExtention
            End If
        End If

    End Function

    Friend Function ConvertXMLFileNameToProfileName(ByVal XML_Path_Name As String) As String

        If XML_Path_Name.StartsWith(gXML_Path_Name) Then
            Dim result As String = XML_Path_Name.Remove(0, gXML_Path_Name.Length + 1)
            result = result.Remove(result.LastIndexOf(gMyExtention))
            Return result
        Else
            Return XML_Path_Name

        End If

    End Function

    Private Sub CopyIncludeFiles()

        CopyAnIncludedFile("Emboss")
        CopyAnIncludedFile("Gaussian Blur")
        CopyAnIncludedFile("Neutral")
        CopyAnIncludedFile("Sharpening")

    End Sub

    Private Sub CopyAnIncludedFile(ByVal ProfileName As String)

        Try

            Dim Filename As String = ProfileName & ".av4w"

            Dim SourceFile As String = System.AppDomain.CurrentDomain.BaseDirectory & "\" & Filename

            Dim TargetFile As String = gXML_Path_Name & "\Defaults\" & Filename

            If File.Exists(SourceFile) Then
                If File.Exists(TargetFile) Then
                Else
                    File.Copy(SourceFile, TargetFile)
                End If
            End If

        Catch ex As Exception

        End Try

    End Sub

    Friend Function GetDimensionsOfString(ByVal handle As IntPtr, ByVal text As String, ByVal Font As Font) As RectangleF

        Dim path As GraphicsPath = New GraphicsPath()

        Dim size_in_pixels As Single = Font.SizeInPoints / 72 * User32.GetDpiForWindow(handle)

        path.AddString(text, Font.FontFamily, Font.Style, size_in_pixels, New Point(0, 0), New StringFormat())

        Return path.GetBounds()

    End Function

    Private Sub ConvertLegacySharpeningFileNames()

        Try

            Const LegacyProfileIdentier As String = "SharpeningProfile_"
            Const CurrentProfileIdentier As String = ""
            Dim WorkingFileName As String

            Dim di As New IO.DirectoryInfo(gXML_Path_Name)
            Dim aryFi As IO.FileInfo() = di.GetFiles(LegacyProfileIdentier & "*" & gMyExtention)
            Dim fi As IO.FileInfo


            Dim WorkingSharpenProfile As New SharpenProfile
            Dim WorkingEffectsProfile As New EffectsProfile


            For Each fi In aryFi

                WorkingFileName = fi.FullName.Replace(LegacyProfileIdentier, CurrentProfileIdentier)

                If File.Exists(WorkingFileName) Then
                Else

                    'cannot use a simple copy because the old profile contains the words 'ShapeningProfile'
                    'and the new profile needs to contain the words 'EffectsProfile'
                    'IO.File.Copy(fi.FullName, WorkingFileName)

                    ' so convert it this way (alternative would be to read the file, replace the text) but this
                    ' is safer as the words SharpeningProfile could be in the notes or the name of the profile as well

                    WorkingSharpenProfile = XML.ObjectXMLSerializer(Of SharpenProfile).Load(fi.FullName)

                    With WorkingEffectsProfile
                        .Name = WorkingSharpenProfile.Name
                        .MatrixType = "Effect"
                        .MatrixSize = 5
                        .Enabled = WorkingSharpenProfile.Enabled
                        .Bias = WorkingSharpenProfile.Bias
                        .Factor = WorkingSharpenProfile.Factor
                        .Matrix = WorkingSharpenProfile.Matrix
                        .Compute = gDefaultCompute
                        .Symmetric = gDefaultSymmetric
                        .Notes = WorkingSharpenProfile.Notes
                        .Sigma = 1
                        .Amount = 0
                    End With

                    XMLSave(WorkingEffectsProfile, WorkingFileName)

                    IO.File.Delete(fi.FullName)

                End If

            Next

        Catch ex As Exception

        End Try

    End Sub

    'Friend Function GetTrimmedBoardersFromBitmap(ByRef img As Bitmap) As Rectangle

    '    Dim boarder As Rectangle = New Rectangle(-1, -1, -1, -1)

    '    Dim c As Color = Color.FromArgb(0, 0, 0, 0) ' boarder colour

    '    Try

    '        Dim w As Integer = img.Width
    '        Dim h As Integer = img.Height

    '        ' Find left boarder
    '        For x As Integer = 0 To w - 1
    '            For y As Integer = 0 To h - 1

    '                If c = img.GetPixel(x, y) Then
    '                Else
    '                    boarder.X = x
    '                    Exit For
    '                End If
    '            Next

    '            If boarder.X > -1 Then Exit For

    '        Next


    '        ' Find right boarder
    '        For x As Integer = w - 1 To 0 Step -1
    '            For y As Integer = 0 To h - 1

    '                If c = img.GetPixel(x, y) Then
    '                Else
    '                    boarder.Width = x - boarder.X + 1
    '                    Exit For
    '                End If

    '            Next
    '            If boarder.Width > -1 Then Exit For
    '        Next


    '        ' Find top boarder
    '        For y As Integer = 0 To h - 1
    '            For x As Integer = 0 To w - 1
    '                If c = img.GetPixel(x, y) Then
    '                Else
    '                    boarder.Y = y
    '                    Exit For
    '                End If
    '            Next
    '            If boarder.Y > -1 Then Exit For
    '        Next

    '        ' Find bottom boarder
    '        For y As Integer = h - 1 To 0 Step -1
    '            For x As Integer = 0 To w - 1
    '                If c = img.GetPixel(x, y) Then
    '                Else
    '                    boarder.Height = y - boarder.Y + 1
    '                    Exit For
    '                End If
    '            Next
    '            If boarder.Height > -1 Then Exit For
    '        Next

    '        If boarder.X = -1 Then boarder.X = 0
    '        If boarder.Y = -1 Then boarder.Y = 0

    '        If (boarder.Width + boarder.X) > img.Width Then boarder.Width = img.Width - boarder.X
    '        If (boarder.Height + boarder.Y) > img.Height Then boarder.Height = img.Height - boarder.Y

    '    Catch ex As Exception

    '        boarder.X = 0
    '        boarder.Y = 0
    '        boarder.Width = img.Width
    '        boarder.Height = img.Height

    '    End Try

    '    Return boarder

    'End Function


    Friend Function GetTrimmedBoardersFromBitmap(ByRef inputBitmap As Bitmap) As Rectangle

        Dim boarder As Rectangle = New Rectangle(-1, -1, -1, -1)

        Dim bmData As BitmapData = inputBitmap.LockBits(New Rectangle(0, 0, inputBitmap.Width, inputBitmap.Height), ImageLockMode.ReadWrite, inputBitmap.PixelFormat)

        Dim aStride As Integer = Math.Abs(bmData.Stride)
        Dim bytes As Integer = aStride * inputBitmap.Height

        If bmData.Stride < 0 Then
            bmData.Scan0 = bmData.Scan0 + (inputBitmap.Height - 1) * bmData.Stride
        End If

        Dim rgbValues As Byte() = New Byte(bytes - 1) {}

        Marshal.Copy(bmData.Scan0, rgbValues, 0, rgbValues.Length)

        Try

            Dim AdvanceBy As Integer

            Dim rgb As Integer

            Dim imgPixelFormatString As String = inputBitmap.PixelFormat.ToString
            If imgPixelFormatString.EndsWith("bppPArgb") Then
                AdvanceBy = 5
            ElseIf imgPixelFormatString.EndsWith("bppArgb") Then
                AdvanceBy = 4
            Else
                AdvanceBy = 3
            End If

            Dim w As Integer = inputBitmap.Width
            Dim h As Integer = inputBitmap.Height

            ' Find left boarder
            For x As Integer = 0 To w - 1
                For y As Integer = 0 To h - 1

                    rgb = y * aStride + AdvanceBy * x

                    If (rgbValues(rgb) = 0) AndAlso (rgbValues(rgb + 1) = 0) AndAlso (rgbValues(rgb + 2) = 0) AndAlso (rgbValues(rgb + 3) = 0) Then
                    Else
                        boarder.X = x
                        Exit For
                    End If
                Next

                If boarder.X > -1 Then Exit For

            Next


            ' Find right boarder
            For x As Integer = w - 1 To 0 Step -1
                For y As Integer = 0 To h - 1

                    rgb = y * aStride + AdvanceBy * x

                    If (rgbValues(rgb) = 0) AndAlso (rgbValues(rgb + 1) = 0) AndAlso (rgbValues(rgb + 2) = 0) AndAlso (rgbValues(rgb + 3) = 0) Then
                    Else
                        boarder.Width = x - boarder.X + 1
                        Exit For
                    End If

                Next
                If boarder.Width > -1 Then Exit For
            Next

            ' Find top boarder
            For y As Integer = 0 To h - 1
                For x As Integer = 0 To w - 1

                    rgb = y * aStride + AdvanceBy * x

                    If (rgbValues(rgb) = 0) AndAlso (rgbValues(rgb + 1) = 0) AndAlso (rgbValues(rgb + 2) = 0) AndAlso (rgbValues(rgb + 3) = 0) Then
                    Else
                        boarder.Y = y
                        Exit For
                    End If

                Next
                If boarder.Y > -1 Then Exit For
            Next

            ' Find bottom boarder
            For y As Integer = h - 1 To 0 Step -1
                For x As Integer = 0 To w - 1

                    rgb = y * aStride + AdvanceBy * x

                    If (rgbValues(rgb) = 0) AndAlso (rgbValues(rgb + 1) = 0) AndAlso (rgbValues(rgb + 2) = 0) AndAlso (rgbValues(rgb + 3) = 0) Then
                    Else
                        boarder.Height = y - boarder.Y + 1
                        Exit For
                    End If

                Next
                If boarder.Height > -1 Then Exit For
            Next

            If boarder.X = -1 Then boarder.X = 0
            If boarder.Y = -1 Then boarder.Y = 0

            If (boarder.Width + boarder.X) > inputBitmap.Width Then boarder.Width = inputBitmap.Width - boarder.X
            If (boarder.Height + boarder.Y) > inputBitmap.Height Then boarder.Height = inputBitmap.Height - boarder.Y

        Catch ex As Exception

            boarder.X = 0
            boarder.Y = 0
            boarder.Width = inputBitmap.Width
            boarder.Height = inputBitmap.Height

        End Try

        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmData.Scan0, rgbValues.Length)
        inputBitmap.UnlockBits(bmData)

        bmData = Nothing

        ReDim rgbValues(1)
        rgbValues = Nothing

        GarbageCollect()
        Application.DoEvents()

        Return boarder

    End Function

    Friend Sub CloseAR4W()

        Dim ARulerForWindowsIsRunning As Boolean = IsARulerForWindowsRunning()

        If ARulerForWindowsIsRunning Then

            Const gFileWatcherFilterName As String = "ar4w_close.txt"
            Dim gFileWatcherPathName As String = String.Empty
            Dim gFileWatcherPathAndFileName As String = String.Empty

            gFileWatcherPathName = System.IO.Path.GetTempPath()
            gFileWatcherPathAndFileName = gFileWatcherPathName & gFileWatcherFilterName

            If File.Exists(gFileWatcherPathAndFileName) Then
                System.IO.File.Delete(gFileWatcherPathAndFileName)
                Application.DoEvents()
                Threading.Thread.Sleep(250)
            End If

            System.IO.File.WriteAllText(gFileWatcherPathAndFileName, "close")
            Application.DoEvents()

            Threading.Thread.Sleep(750)

        End If

    End Sub

    Friend Function IsARulerForWindowsRunning() As Boolean

        Return Diagnostics.Process.GetProcessesByName("aruler").Length > 0

    End Function

    Friend Sub EffectsNow(ByRef bmp As Bitmap, Optional ByVal OverrideProfileName As String = "*Not Overridden*")

        'Dim sw As New Diagnostics.Stopwatch
        'sw.Start()

        If bmp IsNot Nothing Then

            Effects_Apply_Matrix(bmp, OverrideProfileName)

            GarbageCollect()

        End If

        'Console.WriteLine(sw.ElapsedMilliseconds)

    End Sub

    Friend Sub Effects_Apply_Matrix(ByRef bmp As Bitmap, ByVal OverrideProfileName As String)

        ' if OverrideProfileName is set, the request to run this routine is coming from the PickEffects Window and only one update will be required
        '
        ' otherwise process all effect updates here (to save processing time the locking and unlocking are only done once)

        Dim NumberOfEffectsToApply As Integer

        If OverrideProfileName = "*Not Overridden*" Then

            For x = 1 To gCurrentNumberOfEffectsBoxesInUse
                If gCurrentNamesOfEffectsInUse(x) <> gDefaultEffectNotInUse Then NumberOfEffectsToApply += 1
            Next

        Else

            NumberOfEffectsToApply = 1

        End If

        ' Early for no effects to apply 

        If NumberOfEffectsToApply = 0 Then Exit Sub

        ' Early out for neutral profile 

        If NumberOfEffectsToApply = 1 Then

            '  use current effects profile unless overridden
            If OverrideProfileName = "*Not Overridden*" Then
                Call LoadEffectsProfile(gCurrentNamesOfEffectsInUse(1))
            End If

            If (gCurrentEffectsProfile.MatrixString = gNeutralMatrixString3x3) OrElse (gCurrentEffectsProfile.MatrixString = gNeutralMatrixString5x5) OrElse (gCurrentEffectsProfile.MatrixString = gNeutralMatrixString7x7) Then
                If Not gCurrentEffectsProfile.TransparencyEnabled Then
                    If (gCurrentEffectsProfile.ModifyRed = 0) AndAlso (gCurrentEffectsProfile.ModifyGreen = 0) AndAlso (gCurrentEffectsProfile.ModifyBlue = 0) AndAlso (gCurrentEffectsProfile.ModifyAlpha = 0) Then
                        Exit Sub
                    End If
                End If
            End If

        End If

        'Dim sw As New Diagnostics.Stopwatch
        'sw.Start()

        If bmp IsNot Nothing Then

            ' For efficiency determine boaders so we don't have to go thru the Effects logic for the frame
            ' these values to be used later in processing

            Dim CoreImage As Rectangle = GetTrimmedBoardersFromBitmap(bmp)

            Dim ProfileName As String

            Dim width As Integer = CoreImage.Width
            Dim height As Integer = CoreImage.Height
            Dim WidthMiunsOne As Integer = width - 1
            Dim HeightMiunsOne As Integer = height - 1

            Dim bmData As BitmapData
            Dim imgPixelFormat As PixelFormat = bmp.PixelFormat

            Dim AdvanceBy As Integer

            Dim imgPixelFormatString As String = imgPixelFormat.ToString

            Dim SupportsAlpha As Boolean

            If imgPixelFormatString.EndsWith("bppPArgb") Then  ' to do test this another day
                SupportsAlpha = True
                AdvanceBy = 5
            ElseIf imgPixelFormatString.EndsWith("bppArgb") Then
                SupportsAlpha = True
                AdvanceBy = 4
            Else
                SupportsAlpha = False
                AdvanceBy = 3
            End If

            If SupportsAlpha Then
            Else

                'Convert image to argb if needed

                Dim IsAnRGBConversionNeeded As Boolean = False
                If OverrideProfileName = "*Not Overridden*" Then

                    For x = 1 To gCurrentNumberOfEffectsBoxesInUse

                        ProfileName = gCurrentNamesOfEffectsInUse(x)
                        Call LoadEffectsProfile(ProfileName)

                        IsAnRGBConversionNeeded = (gCurrentEffectsProfile.TransparencyEnabled OrElse (gCurrentEffectsProfile.ModifyAlpha <> 0))

                        If IsAnRGBConversionNeeded Then
                            Exit For
                        End If

                    Next

                Else

                    IsAnRGBConversionNeeded = (gCurrentEffectsProfile.TransparencyEnabled OrElse (gCurrentEffectsProfile.ModifyAlpha <> 0))

                End If

                If IsAnRGBConversionNeeded Then
                    Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
                    bmp = New Bitmap(bmp.Clone(rect, PixelFormat.Format32bppArgb))
                End If

                bmp.MakeTransparent()
                SupportsAlpha = True
                AdvanceBy = 3

            End If

            bmp.MakeTransparent()

            bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, imgPixelFormat)

            'lock bits for processing 

            Dim bytes As Integer = bmData.Stride * bmp.Height
            Dim rgbValues As Byte() = New Byte(bytes - 1) {}
            Marshal.Copy(bmData.Scan0, rgbValues, 0, rgbValues.Length)

            Dim red As Double, green As Double, blue As Double
            Dim result As Color(,) = New Color(bmp.Width - 1, bmp.Height - 1) {}

            Dim theFirstColourToUseHasBeenEstablished As Boolean = False

            Dim firstRed, firstGreen, firstBlue As Integer

            Dim ChangeAlpha As Boolean

            For z = 1 To NumberOfEffectsToApply

                'theFirstColourToUseHasBeenEstablished = False

                If OverrideProfileName = "*Not Overridden*" Then
                    ProfileName = gCurrentNamesOfEffectsInUse(z)
                    Call LoadEffectsProfile(ProfileName)
                Else
                    ProfileName = OverrideProfileName
                    ' will use the current profile
                End If

                If (gCurrentEffectsProfile.MatrixString = gNeutralMatrixString3x3) OrElse (gCurrentEffectsProfile.MatrixString = gNeutralMatrixString5x5) OrElse (gCurrentEffectsProfile.MatrixString = gNeutralMatrixString7x7) Then
                    If Not gCurrentEffectsProfile.TransparencyEnabled Then
                        If (gCurrentEffectsProfile.ModifyRed = 0) AndAlso (gCurrentEffectsProfile.ModifyGreen = 0) AndAlso (gCurrentEffectsProfile.ModifyBlue = 0) AndAlso (gCurrentEffectsProfile.ModifyAlpha = 0) Then
                            GoTo NeutralProfileByPass
                        End If
                    End If
                End If


                Dim filterWidth As Integer = gMaxMatrixSize
                Dim filterHeight As Integer = gMaxMatrixSize

                Dim filter As Double(,) = New Double(filterWidth - 1, filterHeight - 1) {}

                Dim bias As Double = gCurrentEffectsProfile.Bias
                Dim factor As Double = gCurrentEffectsProfile.Factor

                ' load the gCurrentEffectsProfile.MatrixTable depending on the Matrix Type

                ' the is nothing test below is for legacy profiles not yet saved in the current format

                Dim mSize As Integer = gCurrentEffectsProfile.MatrixSize

                LoadEffectsMatrixTableFromString()

                Dim matrixOffset As Integer
                Dim shiftingOffset As Integer = 0

                ' for effeciency check to see the matrix can be made a 1x1 filter

                Dim CanBeMadeA1x1Filter As Boolean = True

                For x = 0 To gMaxMatrixSize - 1
                    For y = 0 To gMaxMatrixSize - 1
                        If (x = 3) AndAlso (y = 3) Then
                        Else
                            If gCurrentEffectsProfile.MatrixTable(x, y) <> 0 Then
                                CanBeMadeA1x1Filter = False
                                Exit For
                            End If
                        End If
                    Next
                Next

                If CanBeMadeA1x1Filter Then

                    filterWidth = 1
                    filterHeight = 1

                    filter(0, 0) = gCurrentEffectsProfile.MatrixTable(3, 3)
                    filter(0, 0) += bias
                    filter(0, 0) *= factor

                    matrixOffset = 0

                Else

                    filterWidth = gCurrentEffectsProfile.MatrixSize
                    filterHeight = gCurrentEffectsProfile.MatrixSize

                    Select Case gCurrentEffectsProfile.MatrixSize
                        Case = 3
                            matrixOffset = -1
                            shiftingOffset = 2
                        Case = 5
                            matrixOffset = -2
                            shiftingOffset = 1
                        Case = 7
                            matrixOffset = -3
                            shiftingOffset = 0
                    End Select

                    For x = 0 To gCurrentEffectsProfile.MatrixSize - 1
                        For y = 0 To gCurrentEffectsProfile.MatrixSize - 1
                            filter(x, y) = gCurrentEffectsProfile.MatrixTable(x + shiftingOffset, y + shiftingOffset)
                            filter(x, y) += bias
                            filter(x, y) *= factor
                        Next
                    Next

                End If

                ChangeAlpha = SupportsAlpha AndAlso (gCurrentEffectsProfile.ModifyAlpha <> 0)

                ' now to work with the core image (within the frame)

                Dim filterHeightMinus1 As Integer = filterHeight - 1
                Dim filterWitdthMinus1 As Integer = filterWidth - 1

                Dim rgb As Integer

                Dim imageX As Integer
                Dim imageY As Integer

                Dim xMax As Integer = Math.Min(CoreImage.X + CoreImage.Width, bmp.Width - 1)
                Dim yMax As Integer = Math.Min(CoreImage.Y + CoreImage.Height, bmp.Height - 1)

                Dim CIXWM1 As Integer = CoreImage.X + WidthMiunsOne
                Dim CIYHM1 As Integer = CoreImage.Y + HeightMiunsOne

                Dim filterValue As Double

                Dim r, g, b As Integer

                Dim mr As Double = gCurrentEffectsProfile.ModifyRed
                Dim mg As Double = gCurrentEffectsProfile.ModifyGreen
                Dim mb As Double = gCurrentEffectsProfile.ModifyBlue

                Dim rgb_yValue As Integer

                For y As Integer = CoreImage.Y To yMax

                    For x As Integer = CoreImage.X To xMax

                        red = 0
                        green = 0
                        blue = 0

                        For filterY As Integer = 0 To filterHeightMinus1

                            imageY = y + filterY + matrixOffset

                            If imageY > CIYHM1 Then
                                imageY = CIYHM1
                            End If

                            rgb_yValue = imageY * bmData.Stride

                            For filterX As Integer = 0 To filterWitdthMinus1

                                imageX = x + filterX + matrixOffset

                                If imageX > CIXWM1 Then
                                    imageX = CIXWM1
                                End If

                                rgb = rgb_yValue + AdvanceBy * imageX

                                filterValue = filter(filterX, filterY)

                                red += CDbl(rgbValues(rgb + 2)) * filterValue + mr
                                green += CDbl(rgbValues(rgb + 1)) * filterValue + mg
                                blue += CDbl(rgbValues(rgb)) * filterValue + mb

                            Next filterX

                            r = Math.Min(Math.Max(CInt(red), 0), 255)
                            g = Math.Min(Math.Max(CInt(green), 0), 255)
                            b = Math.Min(Math.Max(CInt(blue), 0), 255)

                            result(x, y) = Color.FromArgb(r, g, b)

                        Next filterY

                        rgb = y * bmData.Stride + AdvanceBy * x

                        If gCurrentEffectsProfile.TransparencyEnabled Then

                            Dim CurrentPixel As Color = Color.FromArgb(result(x, y).R, result(x, y).G, result(x, y).B)

                            If result(x, y).R = 255 AndAlso result(x, y).G = 179 AndAlso result(x, y).B = 33 Then
                                Dim xxx = 1
                            End If

                            If AreColorsSimilar(CurrentPixel, Color.FromArgb(gCurrentEffectsProfile.TransparencyRed, gCurrentEffectsProfile.TransparencyGreen, gCurrentEffectsProfile.TransparencyBlue), gCurrentEffectsProfile.TransparencyTolerance) Then

                                'if there is a requirement in any of the profiles being applied to make a colour transparent then
                                'use the colour of the first requirement to make a colour transparent as the colour to be used in all cases when making a pixel transparent
                                'as we can only have one transparent colour per image

                                If theFirstColourToUseHasBeenEstablished Then
                                Else
                                    firstRed = gCurrentEffectsProfile.TransparencyRed
                                    firstGreen = gCurrentEffectsProfile.TransparencyGreen
                                    firstBlue = gCurrentEffectsProfile.TransparencyBlue
                                    theFirstColourToUseHasBeenEstablished = True
                                End If

                                result(x, y) = Color.FromArgb(firstRed, firstGreen, firstBlue)

                            End If

                        End If

                        If ChangeAlpha Then

                            Dim a As Byte = rgbValues(rgb + 3)

                            Dim newAlpha As Integer = Math.Min(Math.Max((CInt(a) + gCurrentEffectsProfile.ModifyAlpha), 1), 255)  ' don't let alpha be set tp 0 rather only 1; 0 is for fully transparent

                            result(x, y) = Color.FromArgb(newAlpha, result(x, y).R, result(x, y).G, result(x, y).B)

                        End If

                    Next x

                Next y

                If ChangeAlpha Then

                    For x As Integer = CoreImage.X + 1 To CoreImage.X + WidthMiunsOne

                        For y As Integer = CoreImage.Y + 1 To CoreImage.Y + HeightMiunsOne

                            rgb = y * bmData.Stride + AdvanceBy * x

                            rgbValues(rgb + 3) = result(x, y).A
                            rgbValues(rgb + 2) = result(x, y).R
                            rgbValues(rgb + 1) = result(x, y).G
                            rgbValues(rgb) = result(x, y).B

                        Next

                    Next

                Else

                    For x As Integer = CoreImage.X + 1 To CoreImage.X + WidthMiunsOne

                        For y As Integer = CoreImage.Y + 1 To CoreImage.Y + HeightMiunsOne

                            rgb = y * bmData.Stride + AdvanceBy * x

                            rgbValues(rgb + 2) = result(x, y).R
                            rgbValues(rgb + 1) = result(x, y).G
                            rgbValues(rgb) = result(x, y).B

                        Next

                    Next

                End If

NeutralProfileByPass:

            Next

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmData.Scan0, rgbValues.Length)
            bmp.UnlockBits(bmData)

            If theFirstColourToUseHasBeenEstablished Then
                bmp.MakeTransparent(Color.FromArgb(firstRed, firstGreen, firstBlue))
            End If

        End If

        'sw.Stop()
        'Console.WriteLine(sw.ElapsedMilliseconds.ToString)

    End Sub

    Friend Function AreColorsSimilar(ByVal c1 As Color, ByVal c2 As Color, ByVal tolerance As Integer) As Boolean

        Return (Math.Abs(CInt(c1.R) - CInt(c2.R)) <= tolerance) AndAlso (Math.Abs(CInt(c1.G) - CInt(c2.G)) <= tolerance) AndAlso (Math.Abs(CInt(c1.B) - CInt(c2.B)) <= tolerance)

    End Function


    'Friend Sub Effects_Apply_Transparency(ByRef bmp As Bitmap, ByVal OverrideProfileName As String)

    '    Dim NumberOfEffectsToApply As Integer

    '    Dim ProfileName As String

    '    If OverrideProfileName = "*Not Overridden*" Then

    '        For x = 1 To gCurrentNumberOfEffectsBoxesOnScreen
    '            If gCurrentNamesOfEffectsInUse(x) <> gDefaultEffectNotInUse Then NumberOfEffectsToApply += 1
    '        Next

    '    Else
    '        NumberOfEffectsToApply = 1

    '    End If

    '    For z = 1 To NumberOfEffectsToApply

    '        If OverrideProfileName = "*Not Overridden*" Then
    '            ProfileName = gCurrentNamesOfEffectsInUse(z)
    '            Call LoadEffectsProfile(ProfileName)
    '        Else
    '            ' use the current profile
    '            ProfileName = OverrideProfileName
    '        End If

    '        If gCurrentEffectsProfile.TransparencyEnabled Then
    '            bmp.MakeTransparent(Color.FromArgb(gCurrentEffectsProfile.TransparencyRed, gCurrentEffectsProfile.TransparencyGreen, gCurrentEffectsProfile.TransparencyBlue))
    '        End If

    '    Next

    'End Sub

    Friend Sub ImageCheckForTransparencyAndOpactiy(ByRef inputBitmap As Bitmap, ByRef transparencyFound As Boolean, ByRef opacityFound As Boolean)

        transparencyFound = False
        opacityFound = False

        Dim imgPixelFormatString As String = inputBitmap.PixelFormat.ToString

        If imgPixelFormatString.EndsWith("bppPArgb") OrElse imgPixelFormatString.EndsWith("bppArgb") Then
        Else
            'bitmap does not contain alphas, and are work here is done
            Exit Sub
        End If

        Dim bmData As BitmapData = inputBitmap.LockBits(New Rectangle(0, 0, inputBitmap.Width, inputBitmap.Height), ImageLockMode.ReadWrite, inputBitmap.PixelFormat)

        Dim aStride As Integer = Math.Abs(bmData.Stride)
        Dim bytes As Integer = aStride * inputBitmap.Height

        If bmData.Stride < 0 Then
            bmData.Scan0 = bmData.Scan0 + (inputBitmap.Height - 1) * bmData.Stride
        End If

        Dim rgbValues As Byte() = New Byte(bytes - 1) {}

        Marshal.Copy(bmData.Scan0, rgbValues, 0, rgbValues.Length)

        Try

            Dim Alpha As Byte

            Dim AdvanceBy As Integer

            Dim rgb As Integer

            If imgPixelFormatString.EndsWith("bppPArgb") Then
                AdvanceBy = 5
            ElseIf imgPixelFormatString.EndsWith("bppArgb") Then
                AdvanceBy = 4
            End If

            Dim r, g, b As Integer
            Dim testColour As Color

            ' for this test to provide reliable results, ignore a 2 pixel wide boarder around the image

            Dim startX As Integer = 2
            Dim endX As Integer = inputBitmap.Width - 3
            Dim startY As Integer = 2
            Dim endY As Integer = inputBitmap.Height - 3

            For y As Integer = startY To endY

                For x As Integer = startX To endX

                    rgb = y * aStride + AdvanceBy * x

                    Alpha = rgbValues(rgb + 3)

                    If Not transparencyFound Then transparencyFound = (Alpha = 0)

                    If Not opacityFound Then opacityFound = (Alpha > 0) AndAlso (Alpha < 255)

                    If transparencyFound AndAlso opacityFound Then
                        Exit For
                    End If

                Next

            Next

        Catch ex As Exception

        End Try

        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmData.Scan0, rgbValues.Length)
        inputBitmap.UnlockBits(bmData)

        bmData = Nothing

        ReDim rgbValues(1)
        rgbValues = Nothing

        GarbageCollect()
        Application.DoEvents()

    End Sub

    Friend Function MakeACheckboardImage() As Bitmap

        Const CheckerSize As Integer = 7

        Const CheckerBoardSize As Integer = CheckerSize * 2

        Dim CheckerColour As Color = Color.FromArgb(233, 233, 233)

        Dim pb As New PictureBox

        pb.Size = New Size(CheckerBoardSize, CheckerBoardSize)

        Dim bmp As Bitmap = New Bitmap(CheckerSize * 2, CheckerSize * 2)

        Using brush As SolidBrush = New SolidBrush(CheckerColour)

            Using G As Graphics = Graphics.FromImage(bmp)
                G.FillRectangle(Brushes.White, 0, 0, CheckerSize * 2, CheckerSize * 2)
                G.FillRectangle(brush, 0, 0, CheckerSize, CheckerSize)
                G.FillRectangle(brush, CheckerSize, CheckerSize, CheckerSize, CheckerSize)
            End Using

        End Using

        pb.BackgroundImage = bmp
        pb.BackgroundImageLayout = ImageLayout.Tile

        Dim rv As Bitmap = New Bitmap(pb.BackgroundImage)

        pb.Dispose()

        Return rv

    End Function

    <System.Diagnostics.DebuggerStepThrough()>
    Friend Sub GarbageCollect()

        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub

    Friend Function FindAnUnusedColour(ByRef inputBitmap As Bitmap) As Color

        Dim ReturnValue As Color = Color.FromArgb(255, 1, 1, 1)

        Dim colourTable(255, 255, 255) As Boolean  'table of used colours

        Dim bmData As BitmapData = inputBitmap.LockBits(New Rectangle(0, 0, inputBitmap.Width, inputBitmap.Height), ImageLockMode.ReadWrite, inputBitmap.PixelFormat)

        Dim aStride As Integer = Math.Abs(bmData.Stride)
        Dim bytes As Integer = aStride * inputBitmap.Height

        If bmData.Stride < 0 Then
            'ref: https://stackoverflow.com/questions/6835006/how-can-i-copy-the-pixel-data-from-a-bitmap-with-negative-stride/10360753#10360753
            bmData.Scan0 = bmData.Scan0 + (inputBitmap.Height - 1) * bmData.Stride
        End If

        Dim rgbValues As Byte() = New Byte(bytes - 1) {}

        Marshal.Copy(bmData.Scan0, rgbValues, 0, rgbValues.Length)

        Try

            Dim AdvanceBy As Integer

            Dim rgb As Integer

            Dim imgPixelFormatString As String = inputBitmap.PixelFormat.ToString
            If imgPixelFormatString.EndsWith("bppPArgb") Then
                AdvanceBy = 5
            ElseIf imgPixelFormatString.EndsWith("bppArgb") Then
                AdvanceBy = 4
            Else
                AdvanceBy = 3
            End If

            For y As Integer = 0 To inputBitmap.Height - 1

                For x As Integer = 0 To inputBitmap.Width - 1

                    rgb = y * aStride + AdvanceBy * x

                    If AdvanceBy > 3 Then
                        If rgbValues(rgb + 3) = 255 Then 'only look at alpha 255 pixels
                            colourTable(rgbValues(rgb), rgbValues(rgb + 1), rgbValues(rgb + 2)) = True
                        End If
                    Else
                        colourTable(rgbValues(rgb), rgbValues(rgb + 1), rgbValues(rgb + 2)) = True
                    End If

                Next

            Next

            ' Find a color where r, g and b are all the same; 
            ' I'm not sure why but in testing if they were different then 
            ' oddly the primary screen would be locked out under the ruler also '
            ' I needed to start at 2,2,2 as 0,0,0 is black (too common) and 1,1,1 seems to be treated as black as well

            For x As Integer = 2 To 255
                If colourTable(x, x, x) Then
                Else
                    ReturnValue = Color.FromArgb(255, x, x, x)
                    Exit For
                End If
            Next

        Catch ex As Exception

        End Try

        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmData.Scan0, rgbValues.Length)
        inputBitmap.UnlockBits(bmData)

        ReDim colourTable(1, 1, 1)
        colourTable = Nothing

        bmData = Nothing

        ReDim rgbValues(1)
        rgbValues = Nothing

        GarbageCollect()
        Application.DoEvents()

        Return ReturnValue

    End Function

    Friend Sub SetGlobalTransparencyColour(ByVal c As Color)

        gMyTransparencyColour = Color.FromArgb(0, c.R, c.G, c.B)
        gMyTransparencyBrushBasedOnAnUnusedColour = New SolidBrush(gMyTransparencyColour)

    End Sub

    Friend Sub SetFramingColour()

        gMyFramingColour = Color.FromArgb(0, 255, 255, 255)
        gMyFramingBrush = New SolidBrush(gMyFramingColour)

    End Sub

    Friend Function AreTheseTwoProfilesTheSame(ByVal Profile1 As gEffectsProfileStructure, ByVal Profile2 As gEffectsProfileStructure, ByVal ExcludeName As Boolean, ByVal ExcludeMatrixType As Boolean, ByVal ExcludeMatrixSize As Boolean, ByVal ExcludeEnabled As Boolean, ByVal ExcludeNotes As Boolean, ByVal ExcludesSymmetric As Boolean, ByVal ExcludesCompute As Boolean) As Boolean

        Dim returnValue As Boolean

        Try

            With Profile2

                If ExcludeName Then
                Else
                    returnValue = Profile1.Name = .Name
                End If

                If returnValue Then

                    If ExcludeEnabled Then
                    Else
                        returnValue = Profile1.Enabled = .Enabled
                    End If

                Else
                    Exit Try
                End If

                If returnValue Then

                    If ExcludeMatrixType Then
                    Else
                        returnValue = Profile1.MatrixType = .MatrixType
                    End If

                Else
                    Exit Try
                End If

                If returnValue Then

                    If ExcludeMatrixSize Then
                    Else
                        returnValue = Profile1.MatrixSize = .MatrixSize
                    End If

                Else
                    Exit Try
                End If

                If returnValue Then

                    If ExcludesCompute Then
                    Else
                        returnValue = Profile1.Compute = .Compute
                    End If

                Else
                    Exit Try
                End If

                If returnValue Then

                    If ExcludesSymmetric Then
                    Else
                        returnValue = Profile1.Symmetric = .Symmetric
                    End If

                Else
                    Exit Try
                End If

                If returnValue Then

                    If ExcludeNotes Then
                    Else
                        returnValue = Profile1.Notes = .Notes
                    End If

                Else
                    Exit Try
                End If

                'returnValue = (Profile1.Sigma = .Sigma)
                'If returnValue Then returnValue = (Profile1.Amount = .Amount)
                'If returnValue Then returnValue = (Profile1.Factor = .Factor)
                'If returnValue Then returnValue = (Profile1.Bias = .Bias)
                'If returnValue Then returnValue = (Profile1.MatrixString = .MatrixString)
                'If returnValue Then returnValue = (Profile1.ModifyAlpha = .ModifyAlpha)
                'If returnValue Then returnValue = (Profile1.ModifyRed = .ModifyRed)
                'If returnValue Then returnValue = (Profile1.ModifyGreen = .ModifyGreen)
                'If returnValue Then returnValue = (Profile1.ModifyBlue = .ModifyBlue)
                'If returnValue Then returnValue = (Profile1.TransparencyEnabled = .TransparencyEnabled)
                'If returnValue Then returnValue = (Profile1.TransparencyRed = .TransparencyRed)
                'If returnValue Then returnValue = (Profile1.TransparencyGreen = .TransparencyGreen)
                'If returnValue Then returnValue = (Profile1.TransparencyBlue = .TransparencyBlue)

                returnValue = (Profile1.Sigma = .Sigma) AndAlso (Profile1.Amount = .Amount) AndAlso
                              (Profile1.Factor = .Factor) AndAlso (Profile1.Bias = .Bias) AndAlso (Profile1.MatrixString = .MatrixString) AndAlso
                              (Profile1.ModifyAlpha = .ModifyAlpha) AndAlso (Profile1.ModifyRed = .ModifyRed) AndAlso (Profile1.ModifyGreen = .ModifyGreen) AndAlso (Profile1.ModifyBlue = .ModifyBlue) AndAlso
                              (Profile1.TransparencyEnabled = .TransparencyEnabled) AndAlso (Profile1.TransparencyRed = .TransparencyRed) AndAlso (Profile1.TransparencyGreen = .TransparencyGreen) AndAlso (Profile1.TransparencyBlue = .TransparencyBlue)

            End With

        Catch ex As Exception

        End Try

        Return returnValue

    End Function

    Friend Sub AddFoldersAndFilesToTreeview(ByVal CurrentNode As TreeNode, ByVal StartingDirectory As String, ByVal EnabledOnly As Boolean)

        'uses recurrsion 

        Try

            Dim xmlPathNameLength = gXML_Path_Name.Length

            CurrentNode.Nodes.Clear()

            ' Folders

            For Each folderString As String In Directory.GetDirectories(StartingDirectory)

                Dim newNode As TreeNode = CurrentNode.Nodes.Add(Path.GetFileName(folderString))
                newNode.Tag = gTreeViewFolderTagDesignation

                AddFoldersAndFilesToTreeview(newNode, StartingDirectory & "\" & newNode.Text, EnabledOnly)

            Next

            ' Files

            Dim allfiles As String() = Directory.GetFiles(StartingDirectory)

            Array.Sort(allfiles, StringComparer.CurrentCulture)

            For Each fileString As String In allfiles

                Dim ProfileName As String = Path.GetFileName(fileString)
                Dim ProfileNameLower As String = ProfileName.ToLower

                If ProfileNameLower.EndsWith(".av4w") Then

                    ProfileName = ProfileName.Remove(ProfileName.ToLower.LastIndexOf("av4w") - 1)

                    Dim newNode As TreeNode

                    If EnabledOnly Then

                        If IsThisProfileEnabled(fileString) Then
                            newNode = CurrentNode.Nodes.Add(ProfileName)
                            newNode.Tag = fileString
                            newNode.Checked = True ' see comment below
                        Else
                            ' skip
                        End If

                    Else

                        newNode = CurrentNode.Nodes.Add(ProfileName)
                        newNode.Tag = fileString

                        '
                        ' the node.checked value is otherwise unused in this program, so it is being used as a 'Tag' like attribute (the 'Tag' is already being used so this is a hack alternative)
                        ' if the profile is enabled the node.checked value will be true, otherwise it will be false
                        ' this saves the program from needing to read the profile to determine if it is enabled or not
                        ' it has been built as a work around to an issue with the TreeView1.DrawNode in frmEffectsDesign which would otherwise need to read the profile to determine if it is enabled or not
                        ' 
                        newNode.Checked = IsThisProfileEnabled(fileString)

                    End If

                End If

            Next

        Catch ex As Exception

        End Try

    End Sub

    Friend Sub EnsureVisableAndOptionalySelelectATreeNode(ByVal searchCriteria As String, ByRef TreeView As TreeView, ByVal SelectRequired As Boolean)

        'seach criteria can be either a profile name, or an xml file name

        searchCriteria = ConvertProfileNameToXMLFileName(searchCriteria)

        Dim n As TreeNode
        For Each n In TreeView.Nodes
            If SearchRecursive(searchCriteria, TreeView, n, SelectRequired) Then
                Exit For
            End If
        Next

    End Sub

    Private Function SearchRecursive(ByVal searchCriteria As String, ByRef TreeView As TreeView, ByVal n As TreeNode, ByVal SelectRequired As Boolean) As Boolean

        If n.Tag = searchCriteria Then

            n.EnsureVisible()

            If SelectRequired Then
                TreeView.SelectedNode = n
            End If

            Return True

        End If

        Dim aNode As TreeNode
        For Each aNode In n.Nodes
            If SearchRecursive(searchCriteria, TreeView, aNode, SelectRequired) Then Return True
        Next

        Return False

    End Function

    Friend Sub SelectLastChildInTreeViewNode(ByRef tv As Windows.Forms.TreeView)

        Dim n As TreeNode = tv.SelectedNode

        If n.Nodes.Count = 0 Then

        Else

            tv.SelectedNode = n.Nodes(n.Nodes.Count - 1)

        End If

    End Sub

    Friend Sub SelectFirstChildInTreeViewNode(ByRef tv As Windows.Forms.TreeView)

        Dim n As TreeNode = tv.SelectedNode

        If n.Nodes.Count = 0 Then

        Else

            tv.SelectedNode = n.Nodes(0)

        End If

    End Sub

    Friend gArrayOfExistingTreeNodesExcludingFolders() As String
    Private ArrayOfExistingTreeNodesExcludingFoldersIndex As Integer

    Friend Sub BuildATreeNodeArray(ByVal tv As TreeView)

        ReDim gArrayOfExistingTreeNodesExcludingFolders(1000)
        ArrayOfExistingTreeNodesExcludingFoldersIndex = 0

        BuildArrayNow(tv, tv.Nodes(0))

        ReDim Preserve gArrayOfExistingTreeNodesExcludingFolders(ArrayOfExistingTreeNodesExcludingFoldersIndex - 1)

    End Sub

    Private Sub BuildArrayNow(ByVal tv As TreeView, ByVal n As TreeNode)

        If n.Tag = gTreeViewFolderTagDesignation Then

        Else

            gArrayOfExistingTreeNodesExcludingFolders(ArrayOfExistingTreeNodesExcludingFoldersIndex) = n.Tag
            ArrayOfExistingTreeNodesExcludingFoldersIndex += 1

        End If

        Dim aNode As TreeNode
        For Each aNode In n.Nodes
            BuildArrayNow(tv, aNode)
        Next

    End Sub

    Friend Function ProfileNameValidation(ByRef CandiateProfileName As String) As Boolean

        Dim returnValue As Boolean = False

        Try

            CandiateProfileName = CandiateProfileName.Trim

            Dim TestName As String = QuickFilter(CandiateProfileName, "/:*?""<>|")  ' \ is allowed to speficy directory

            'auto clean up 

            ' ref: https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file

            If CandiateProfileName.StartsWith("\") Then
                CandiateProfileName = CandiateProfileName.Remove(0, 1)
            End If

            While CandiateProfileName.Contains(" \") OrElse CandiateProfileName.Contains("\ ") OrElse CandiateProfileName.Contains("\\") OrElse CandiateProfileName.Contains(".\") OrElse CandiateProfileName.Contains("\.") OrElse CandiateProfileName.Contains("..")
                CandiateProfileName = CandiateProfileName.Replace(" \", "\").Replace("\ ", "\").Replace("\\", "\").Replace(".\", "\").Replace("\.", "\").Replace("..", ".").Trim
            End While

            ' name edits

            ' **

            If CandiateProfileName.EndsWith("\") Then
                Beep()
                Dim ReturnResult = MessageBox.Show("Name cannot end with a backslash ('\').", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Try
            End If

            ' **

            If CandiateProfileName.ToLower.EndsWith(".av4w") Then
                Beep()
                Dim ReturnResult = MessageBox.Show("Name cannot end with .av4w", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Try
            End If

            ' **

            If CandiateProfileName.ToLower.EndsWith(".") Then
                Beep()
                Dim ReturnResult = MessageBox.Show("Name cannot end with a period.", gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Try
            End If

            ' **

            Dim notallowed As List(Of String) = New List(Of String)(New String() {"CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"})

            Dim Ending As String
            If CandiateProfileName.Contains("\") Then
                Ending = CandiateProfileName.Remove(0, CandiateProfileName.LastIndexOf("\") + 1)
            Else
                Ending = CandiateProfileName
            End If

            If notallowed.Contains(Ending.ToUpper) Then
                Beep()
                Dim restricted As String = vbCrLf
                For Each rn In notallowed
                    restricted &= rn & vbTab & "    "
                Next
                Dim ReturnResult = MessageBox.Show("Name cannot be any of the following: " & restricted, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Try
            End If

            ' **

            If Ending.Contains(".") Then

                Ending = Ending.Remove(Ending.IndexOf("."))

                If notallowed.Contains(Ending.ToUpper) Then
                    Beep()
                    Dim restricted As String = vbCrLf
                    For Each rn In notallowed
                        restricted &= rn & vbTab & "    "
                    Next
                    Dim ReturnResult = MessageBox.Show("Name cannot be any of the following" & vbCrLf & "(even when followed by a period):" & vbCrLf & restricted, gThisProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Try
                End If

            End If

            returnValue = True

        Catch ex As Exception

        End Try

        Return returnValue

    End Function


    ' ***************************************************************** v.17

    'ref: https://www.geeksforgeeks.org/gaussian-filter-generation-c/

    'Friend Sub Driver()

    '    Dim matrixSize As Integer = 3
    '    Dim sigma As Double = 1.5

    '    Dim GKernel(matrixSize, matrixSize) As Double

    '    FilterCreation(matrixSize, sigma, GKernel)

    '    For i As Integer = 0 To matrixSize - 1
    '        For j As Integer = 0 To matrixSize - 1
    '            Console.Write(GKernel(i, j))
    '            Console.Write(vbTab)
    '        Next j
    '        Console.Write(ControlChars.Lf)
    '    Next i

    'End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Friend Sub CreateABlurMatrix(ByVal matrixSize As Integer, ByVal sigma As Double, ByRef GKernel(,) As Double)

        Dim r As Double
        Dim s As Double = 2.0 * sigma * sigma

        ' sum is for normalization
        Dim sum As Double = 0.0


        Select Case matrixSize

            Case Is = 3

                ' generating 3x3 kernel
                For x As Integer = -1 To 1
                    For y As Integer = -1 To 1
                        r = Math.Sqrt(x * x + y * y)
                        GKernel(x + 1, y + 1) = (Math.Exp(-(r * r) / s)) / (Math.PI * s)
                        sum += GKernel(x + 1, y + 1)
                    Next y
                Next x

                ' normalising the Kernel
                For i As Integer = 0 To 2
                    For j As Integer = 0 To 2
                        GKernel(i, j) /= sum
                    Next j
                Next i


            Case Is = 5

                ' generating 5x5 kernel
                For x As Integer = -2 To 2
                    For y As Integer = -2 To 2
                        r = Math.Sqrt(x * x + y * y)
                        GKernel(x + 2, y + 2) = (Math.Exp(-(r * r) / s)) / (Math.PI * s)
                        sum += GKernel(x + 2, y + 2)
                    Next y
                Next x

                ' normalising the Kernel
                For i As Integer = 0 To 4
                    For j As Integer = 0 To 4
                        GKernel(i, j) /= sum
                    Next j
                Next i

            Case Is = 7

                ' generating 7x7 kernel
                For x As Integer = -3 To 3
                    For y As Integer = -3 To 3
                        r = Math.Sqrt(x * x + y * y)
                        GKernel(x + 3, y + 3) = (Math.Exp(-(r * r) / s)) / (Math.PI * s)
                        sum += GKernel(x + 3, y + 3)
                    Next y
                Next x

                ' normalising the Kernel
                For i As Integer = 0 To 6
                    For j As Integer = 0 To 6
                        GKernel(i, j) /= sum
                    Next j
                Next i

        End Select


    End Sub


#Region "Setting the Cursor"
    Friend Enum CursorState
        Normal = 0
        Wait = 1
    End Enum
    Friend Sub SeCursor(ByRef MeObject As Object, ByVal CursorState As CursorState)

        If CursorState = CursorState.Normal Then
            MeObject.Invoke(New SetNormalCursorCallback(AddressOf SetNormalCursor), MeObject)
        Else
            MeObject.Invoke(New SetWaitCursorCallback(AddressOf SetWaitCursor), MeObject)
        End If

    End Sub

    Delegate Sub SetWaitCursorCallback(ByRef MeObject As Object)
    Private Sub SetWaitCursor(ByRef MeObject As Object)
        MeObject.Cursor = Cursors.WaitCursor
    End Sub

    Delegate Sub SetNormalCursorCallback(ByRef MeObject As Object)
    Private Sub SetNormalCursor(ByRef MeObject As Object)
        MeObject.Cursor = Cursors.Default
    End Sub

#End Region


#Region "Unused code"

'Friend Sub MakeFrameTransparent(ByRef bmp As Bitmap)

'    Dim bmData As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat)

'    Dim bytes As Integer = bmData.Stride * bmp.Height
'    Dim rgbValues As Byte() = New Byte(bytes - 1) {}
'    Marshal.Copy(bmData.Scan0, rgbValues, 0, rgbValues.Length)

'    Try

'        Dim AdvanceBy As Integer

'        Dim imgPixelFormatString As String = bmp.PixelFormat.ToString
'        If imgPixelFormatString.EndsWith("bppPArgb") Then
'            AdvanceBy = 5
'        ElseIf imgPixelFormatString.EndsWith("bppArgb") Then
'            AdvanceBy = 4
'        Else
'            AdvanceBy = 3
'        End If

'        Dim rgb As Integer

'        For y As Integer = 0 To bmp.Height - 1

'            For x As Integer = 0 To bmp.Width - 1

'                rgb = y * bmData.Stride + AdvanceBy * x

'                If rgbValues(rgb + 3) = 0 Then

'                    Dim pc As Color = bmp.GetPixel(x, y)

'                    rgbValues(rgb + 3) = 0
'                    rgbValues(rgb + 2) = 0
'                    rgbValues(rgb + 1) = 0
'                    rgbValues(rgb) = 0

'                End If

'            Next

'        Next

'    Catch ex As Exception

'    End Try

'    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmData.Scan0, rgbValues.Length)
'    bmp.UnlockBits(bmData)

'    GarbageCollect()

'End Sub


'Friend Sub ReplaceTransparentWithCheckerBoard(ByRef bmp As Bitmap)

'    Dim CoreImage As Rectangle = GetTrimmedBoardersFromBitmap(bmp)

'    Dim bmData As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat)

'    Dim bytes As Integer = bmData.Stride * bmp.Height
'    Dim rgbValues As Byte() = New Byte(bytes - 1) {}
'    Marshal.Copy(bmData.Scan0, rgbValues, 0, rgbValues.Length)

'    Dim AdvanceBy As Integer

'    Dim imgPixelFormatString As String = bmp.PixelFormat.ToString
'    If imgPixelFormatString.EndsWith("bppPArgb") Then
'        AdvanceBy = 5
'    ElseIf imgPixelFormatString.EndsWith("bppArgb") Then
'        AdvanceBy = 4
'    Else
'        AdvanceBy = 3
'    End If

'    Dim rgb As Integer

'    Dim xMax As Integer = Math.Min(CoreImage.X + CoreImage.Width, bmp.Width - 1)
'    Dim yMax As Integer = Math.Min(CoreImage.Y + CoreImage.Height, bmp.Height - 1)

'    Dim MiniCheckerBoard As Bitmap = MakeACheckboardImage()
'    Dim CheckerBoardSize = MiniCheckerBoard.Width

'    For y As Integer = CoreImage.Y To yMax

'        For x As Integer = CoreImage.X To xMax

'            rgb = y * bmData.Stride + AdvanceBy * x

'            If rgbValues(rgb + 3) = 0 Then

'                Dim pc As Color = MiniCheckerBoard.GetPixel(x Mod CheckerBoardSize, y Mod CheckerBoardSize)

'                rgbValues(rgb + 3) = 254
'                rgbValues(rgb + 2) = pc.R
'                rgbValues(rgb + 1) = pc.G
'                rgbValues(rgb) = pc.B

'            End If

'        Next

'    Next

'    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmData.Scan0, rgbValues.Length)
'    bmp.UnlockBits(bmData)

'    MiniCheckerBoard.Dispose()

'    GarbageCollect()

'End Sub

'Friend Sub Effects_Apply_Alpha(ByRef image As Bitmap)

'    If gCurrentEffectsProfile.ModifyAlpha = 0 Then Exit Sub

'    gCurrentAlpha = image.GetPixel(image.Width / 2, image.Height / 2).A

'    Dim NewAlpha As Integer = Math.Min(Math.Max(gCurrentAlpha + gCurrentEffectsProfile.ModifyAlpha, 0), 255)

'    If NewAlpha = gCurrentAlpha Then

'    Else

'        Dim bmp As Bitmap = New Bitmap(image.Width, image.Height)

'        Using g As Graphics = Graphics.FromImage(bmp)
'            Dim colormatrix As ColorMatrix = New ColorMatrix()
'            colormatrix.Matrix33 = NewAlpha / 255.0F
'            Dim imgAttribute As ImageAttributes = New ImageAttributes()
'            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.[Default], ColorAdjustType.Bitmap)
'            g.DrawImage(image, New Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAttribute)
'        End Using

'        image = New Bitmap(bmp)

'        gCurrentAlpha = NewAlpha

'        bmp.Dispose()

'    End If

'End Sub


#End Region

End Module

