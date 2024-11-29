Imports System
Imports System.Xml.Serialization

' keep for legacy conversion support

' added <System.Reflection.ObfuscationAttribute(Feature:="renaming")> to turn of obsfucation for this class - as it cause problems
<System.Reflection.ObfuscationAttribute(Feature:="renaming")> <XmlRootAttribute("SharpenProfile", [Namespace]:="", IsNullable:=False)>
Public Class SharpenProfile

    Implements IDisposable

    Public Sub New()
    End Sub

    Public Name As String
    Public Enabled As Boolean
    Public Bias As Double
    Public Factor As Double
    Public Matrix As String
    Public Notes As String

#Region "IDisposable Support"

    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Name = String.Empty
                Matrix = String.Empty
                Bias = 0
                Factor = 0
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
