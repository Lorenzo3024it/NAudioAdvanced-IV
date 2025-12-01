Imports System.IO
Imports GTA
Imports GTA.Native.Function
Imports NAudio.Wave

Public NotInheritable Class Tools
    'Log Writing
    Private Shared ReadOnly FilePath As String = "NAudioAdvanced-IV.log"
    Friend Shared _hasWrittenWelcomeMessage As Boolean = False
    Private Shared _firstClean As Boolean = False
    Private Shared _isWriting As Boolean = False

    'Friend Shared CamPoint As New Native.Pointer(GetType(Integer))
    'Friend Shared CamPosPoint1 As New Native.Pointer(GetType(Single))
    'Friend Shared CamPosPoint2 As New Native.Pointer(GetType(Single))
    'Friend Shared CamPosPoint3 As New Native.Pointer(GetType(Single))
    'Friend Shared CamDirPoint1 As New Native.Pointer(GetType(Single))
    'Friend Shared CamDirPoint2 As New Native.Pointer(GetType(Single))
    'Friend Shared CamDirPoint3 As New Native.Pointer(GetType(Single))
    ''-----------------------------------------------------------------------
    'Public Shared Function GetCurrentCamera() As Integer
    '    [Call]("GET_GAME_CAM", CamPoint)
    '    Return CamPoint.Value
    'End Function
    'Public Shared Function GetCurrentCameraPosition() As Vector3
    '    [Call]("GET_CAM_POS", GetCurrentCamera, CamPosPoint1, CamPosPoint2, CamPosPoint3)
    '    Return New Vector3(CamPosPoint1.Value, CamPosPoint2.Value, CamPosPoint3.Value)
    'End Function
    'Public Shared Function GetCurrentCameraDirection() As Vector3
    '    [Call]("GET_CAM_POS", GetCurrentCamera, CamDirPoint1, CamDirPoint2, CamDirPoint3)
    '    Return New Vector3(CamDirPoint1.Value, CamDirPoint2.Value, CamDirPoint3.Value)
    'End Function
    Friend Shared Sub WriteLog(Message As String, Optional WriteDateAndTime As Boolean = False, Optional OverwriteFile As Boolean = False)
        Try
            ' Evita chiamate ricorsive o doppie scritture
            If _isWriting Then Return
            _isWriting = True

            If Not _hasWrittenWelcomeMessage Then
                WriteWelcomeMessage()
                _firstClean = True
            End If

            Dim completeLogPath As String = Game.InstallFolder & "\" & FilePath

            Dim overwrite As Boolean
            If OverwriteFile Then
                overwrite = True
                _firstClean = True
            Else
                overwrite = Not _firstClean
                _firstClean = True
            End If

            Using sw As New StreamWriter(completeLogPath, Not OverwriteFile)
                If WriteDateAndTime Then
                    sw.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {Message}")
                Else
                    sw.WriteLine(Message)
                End If
            End Using

        Catch

        Finally
            _isWriting = False
        End Try
    End Sub
    Friend Shared Sub WriteLogAdvanced(Message As String, Exception As Exception, Optional OverwriteFile As Boolean = False)
        Try
            If _isWriting Then Return
            _isWriting = True

            If Not _hasWrittenWelcomeMessage Then
                WriteWelcomeMessage()
                _firstClean = True
            End If
            Dim completeLogPath As String = Game.InstallFolder & "\" & FilePath

            Dim overwrite As Boolean
            If OverwriteFile Then
                overwrite = True
                _firstClean = True
            Else
                overwrite = Not _firstClean
                _firstClean = True
            End If

            Using sw As New StreamWriter(completeLogPath, Not overwrite)
                sw.WriteLine("[" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & "]")
                If Not String.IsNullOrEmpty(Message) Then sw.WriteLine(Message)
                WriteExceptionRecursive(Exception, sw)
                sw.WriteLine(New String("-"c, 80))
            End Using

        Catch

        Finally
            _isWriting = False
        End Try
    End Sub
    Private Shared Sub WriteExceptionRecursive(ex As Exception, sw As StreamWriter, Optional level As Integer = 0)
        Try
            Dim indent As String = New String(" "c, level * 2)
            sw.WriteLine($"{indent}Exception Type: {ex.GetType().FullName}")
            sw.WriteLine($"{indent}Message: {ex.Message}")
            sw.WriteLine($"{indent}Stack Trace: {ex.StackTrace}")
            If ex.InnerException IsNot Nothing Then
                sw.WriteLine(indent & "Inner Exception:")
                WriteExceptionRecursive(ex.InnerException, sw, level + 1)
            End If
        Catch

        End Try
    End Sub
    Friend Shared Sub WriteWelcomeMessage()
        If _hasWrittenWelcomeMessage = False Then
            Try
                Dim logPath As String = Game.InstallFolder & "\" & FilePath
                Using sw As New StreamWriter(logPath, False)
                    WriteLog("   ___________________________________________________________", False, True)
                    WriteLog(" _/===========================================================\_")
                    WriteLog("| ---------  NAudio Advanced IV 1.0 by Lorenzo3024it  --------- |")
                    WriteLog("| -----------     Thank you for using my mods :)    ----------- |")
                    WriteLog("| -------      If you need help please contact me       ------- |")
                    WriteLog("| ---  bttf4thebigrelase@gmail.com / lorenzo3024@hotmail.it --- |")
                    WriteLog("=================================================================")
                    WriteLog("")
                    WriteLog("NAudioAdvanced-IV initialized!", True)
                End Using
                _hasWrittenWelcomeMessage = True
            Catch

            End Try
        End If
    End Sub
    '--------------------------------------------------------------------------------------------------------------------
    Public Class AdvancedAudioTools
        Public ReadOnly Property Reader As AudioFileReader
        Public ReadOnly Property WaveChannel As WaveChannel32
        Public ReadOnly Property OutputDevice As DirectSoundOut
        Public Sub New(Reader As AudioFileReader, WC As WaveChannel32, DSO As DirectSoundOut)
            Me.Reader = Reader
            WaveChannel = WC
            OutputDevice = DSO
        End Sub
    End Class
End Class
