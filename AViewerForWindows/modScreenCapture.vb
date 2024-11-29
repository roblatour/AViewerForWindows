Imports System
Imports System.Drawing
Imports System.Runtime.InteropServices

Module modScreenCapture

    Friend Function CaptureScreen() As Image
        Return CaptureWindow(User32.GetDesktopWindow())
    End Function

    Friend Function CaptureWindow(ByVal handle As IntPtr) As Image

        Dim hdcSrc As IntPtr = User32.GetWindowDC(handle)
        Dim windowRect As New User32.RECT
        User32.GetWindowRect(handle, windowRect)
        Dim width As Integer = windowRect.right - windowRect.left
        Dim height As Integer = windowRect.bottom - windowRect.top
        Dim hdcDest As IntPtr = GDI32.CreateCompatibleDC(hdcSrc)
        Dim hBitmap As IntPtr = GDI32.CreateCompatibleBitmap(hdcSrc, width, height)
        Dim hOld As IntPtr = GDI32.SelectObject(hdcDest, hBitmap)
        GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY)
        GDI32.SelectObject(hdcDest, hOld)
        GDI32.DeleteDC(hdcDest)
        User32.ReleaseDC(handle, hdcSrc)

        Dim img As Image = Image.FromHbitmap(hBitmap)
        GDI32.DeleteObject(hBitmap)

        Return img

    End Function

    'Friend Sub CaptureWindowToFile(ByVal handle As IntPtr, ByVal filename As String, ByVal format As ImageFormat)
    '    Dim img As Image = CaptureWindow(handle)
    '    img.Save(filename, format)
    'End Sub

    'Friend Sub CaptureScreenToFile(ByVal filename As String, ByVal format As ImageFormat)
    '    Dim img As Image = CaptureScreen()
    '    img.Save(filename, format)
    'End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Friend Sub MakeTopMostWindow(ByVal hwnd As Int64, ByVal MakeTopMostFlag As Boolean)

        Dim HWND_TOPMOST As Integer
        If MakeTopMostFlag Then
            HWND_TOPMOST = -1
        Else
            HWND_TOPMOST = -2
        End If

        Dim SWP_NOMOVE As Integer = &H2
        Dim SWP_NOSIZE As Integer = &H1
        Dim TOPMOST_FLAGS As Integer = SWP_NOMOVE Or SWP_NOSIZE
        Try
            User32.SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS)
        Catch ex As Exception
        End Try

    End Sub

    Friend Class GDI32

        Friend Shared SRCCOPY As Integer = &HCC0020

        <DllImport("gdi32.dll")> Friend Shared Function BitBlt(ByVal hObject As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hObjectSource As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As Integer) As Boolean
        End Function

        <DllImport("gdi32.dll")> Friend Shared Function CreateCompatibleBitmap(ByVal hDC As IntPtr, ByVal nWidth As Integer, ByVal nHeight As Integer) As IntPtr
        End Function

        <DllImport("gdi32.dll")> Friend Shared Function CreateCompatibleDC(ByVal hDC As IntPtr) As IntPtr
        End Function

        <DllImport("gdi32.dll")> Friend Shared Function DeleteDC(ByVal hDC As IntPtr) As Boolean
        End Function

        <DllImport("gdi32.dll")> Friend Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
        End Function

        <DllImport("gdi32.dll")> Friend Shared Function SelectObject(ByVal hDC As IntPtr, ByVal hObject As IntPtr) As IntPtr
        End Function
        <DllImport("gdi32.dll")> Friend Shared Function GetDeviceCaps(ByVal hwnd As Integer, ByVal nIndex As Integer) As Integer
        End Function

        <DllImport("gdi32.dll")>
        Public Shared Function GetPixel(ByVal hWnd As IntPtr, X As Integer, iy As Integer) As Integer
        End Function

    End Class 'GDI32 
              _
    Friend Class User32

        <StructLayout(LayoutKind.Sequential)>
        Friend Structure RECT
            Friend left As Integer
            Friend top As Integer
            Friend right As Integer
            Friend bottom As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Friend Structure DEVMODE


            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
            Friend dmDeviceName As String

            Friend dmSpecVersion As Short
            Friend dmDriverVersion As Short
            Friend dmSize As Short
            Friend dmDriverExtra As Short
            Friend dmFields As Integer

            Friend dmOrientation As Short
            Friend dmPaperSize As Short
            Friend dmPaperLength As Short
            Friend dmPaperWidth As Short
            Friend dmScale As Short
            Friend dmCopies As Short
            Friend dmDefaultSource As Short
            Friend dmPrintQuality As Short

            Friend dmPosition As Integer
            Friend dmDisplayOrientation As Integer
            Friend dmDisplayFixedOutput As Integer

            Friend dmColor As Short
            Friend dmDuplex As Short
            Friend dmYResolution As Short
            Friend dmTTOption As Short
            Friend dmCollate As Short

            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
            Friend dmFormName As String

            Friend dmLogPixels As Short
            Friend dmBitsPerPel As Integer
            Friend dmPelsWidth As Integer
            Friend dmPelsHeight As Integer

            Friend dmDisplayFlags As Integer
            Friend dmNup As Integer

            Friend dmDisplayFrequency As Integer

            Friend dmICMMethod As Integer
            Friend dmICMIntent As Integer
            Friend dmMediaType As Integer
            Friend dmDitherType As Integer

            Friend dmPanningWidth As Integer
            Friend dmPanningHeight As Integer

        End Structure
        <DllImport("user32.dll")> Friend Shared Function GetDesktopWindow() As IntPtr
        End Function

        <DllImport("user32.dll")> Friend Shared Function GetWindowDC(ByVal hWnd As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")> Friend Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")> Friend Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef rect As RECT) As IntPtr
        End Function

        <DllImport("user32.dll")> Friend Shared Function WindowFromPoint(ByVal p As Point) As IntPtr
        End Function

        <DllImport("user32.dll")> Friend Shared Function GetAsyncKeyState(ByVal vKey As Integer) As Integer
        End Function

        <DllImport("user32.dll")> Friend Shared Function SetWindowPos(ByVal hwnd As Integer, ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
        End Function

        '<DllImport("user32.dll")> Friend Shared Function EnumDisplaySettings(ByVal lpszDeviceName As String, ByVal iModeNum As Integer, ByRef lpdevMod As DEVMODE) As Boolean
        'End Function

        <DllImport("user32.dll")> Friend Shared Function GetDpiForWindow(ByVal hWnd As IntPtr) As Integer
        End Function

    End Class 'User32 

    '    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

    '        Dim sc As New ScreenShotLogic.ScreenCapture
    '        Dim img As Image = sc.CaptureScreen
    '        Me.PictureBox1.Image = img
    '        sc.CaptureWindowToFile(Me.PictureBox1.Handle, "C:\\temp2.gif", ImageFormat.Gif)

    '    End Sub

End Module
