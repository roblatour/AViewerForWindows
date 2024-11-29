Imports System
Imports System.Xml.Serialization

' added <System.Reflection.ObfuscationAttribute(Feature:="renaming")> to turn of obsfucation for this class - as it cause problems
<System.Reflection.ObfuscationAttribute(Feature:="renaming")> <XmlRootAttribute("EffectsProfile", [Namespace]:="", IsNullable:=False)>
Public Class EffectsProfile

    Implements IDisposable

    Public Sub New()
    End Sub

    Public Name As String
    Public Enabled As Boolean
    Public MatrixType As String
    Public MatrixSize As Integer
    Public Sigma As Double
    Public Amount As Double
    Public Bias As Double
    Public Factor As Double
    Public Compute As Boolean
    Public Symmetric As Boolean
    Public Matrix As String
    Public ModifyAlpha As Integer
    Public ModifyRed As Integer
    Public ModifyGreen As Integer
    Public ModifyBlue As Integer
    Public TransparencyEnabled As Boolean
    Public TransparencyRed As Integer
    Public TransparencyGreen As Integer
    Public TransparencyBlue As Integer
    Public TransparencyTolerance As Integer
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
