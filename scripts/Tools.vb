Imports System.IO
Imports System.Reflection
Imports GTA
Imports GTA.Native.Function
Imports NAudio.Wave

Friend NotInheritable Class Tools
    Private Sub New() : End Sub
    Private Shared ReadOnly LogFile As String = "NAudioAdvanced-IV.log"
    Friend Shared _hasWrittenWelcomeMessage As Boolean = False
    Private Shared _firstClean As Boolean = False
    Private Shared _isWriting As Boolean = False
    Private Shared _gamePath As String
    Public Shared ReadOnly Property GamePath As String
        Get
            If String.IsNullOrEmpty(_gamePath) Then
                _gamePath = GamePathResolver.ResolveGamePath()
            End If

            Return _gamePath
        End Get
    End Property
    Friend Shared Function ResolveGamePath() As String
        Try
            Dim asm As Assembly = Assembly.GetAssembly(GetType(GamePathResolver))
            If asm Is Nothing Then
                Tools.WriteLog("Error in NAudioAdvanced....ResolveGamePath (Assembly.GetAssembly).")
                Return String.Empty
            End If
            Dim location As String = asm.Location
            If String.IsNullOrEmpty(location) Then
                Tools.WriteLog("Error in NAudioAdvanced....ResolveGamePath: Assembly location is empty.")
                Return String.Empty
            End If
            Return Path.GetDirectoryName(location)
        Catch ex As Exception
            Tools.WriteLog("Error in NAudioAdvanced....ResolveGamePath.", ex)
            Return String.Empty
        End Try
    End Function
    Friend Shared Function ClampInteger(Value As Integer, Min As Integer, Max As Integer) As Integer
        If Value < Min Then Return Min
        If Value > Max Then Return Max
        Return Value
    End Function
    Friend Shared Function ClampFloat(Value As Single, Min As Single, Max As Single) As Single
        If Value < Min Then Return Min
        If Value > Max Then Return Max
        Return Value
    End Function
    Friend Shared Sub WriteLog(Message As String, Optional WriteDateAndTime As Boolean = False)
        Try
            If _isWriting Then Return
            _isWriting = True

            If Not _hasWrittenWelcomeMessage Then
                WriteWelcomeMessage()
                _firstClean = True
            End If
            Dim completeLogPath As String = Path.Combine(GamePath, LogFile)


            Using sw As New StreamWriter(completeLogPath, True)
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
    Friend Shared Sub WriteLog(Message As String, Exception As Exception)
        Try
            If _isWriting Then Return
            _isWriting = True

            If Not _hasWrittenWelcomeMessage Then
                WriteWelcomeMessage()
                _firstClean = True
            End If
            Dim completeLogPath As String = Path.Combine(GamePath, LogFile)

            Using sw As New StreamWriter(completeLogPath, True)
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
        If _hasWrittenWelcomeMessage Then Return

        Try
            Dim logPath As String = System.IO.Path.Combine(GamePath, LogFile)
            Using sw As New StreamWriter(logPath, False)
                sw.WriteLine("   ___________________________________________________________")
                sw.WriteLine(" _/===========================================================\_")
                sw.WriteLine("| ---------  NAudio Advanced IV 1.0 by Lorenzo3024it  --------- |")
                sw.WriteLine("| -----------     Thank you for using my mods :)    ----------- |")
                sw.WriteLine("| -------      If you need help please contact me       ------- |")
                sw.WriteLine("| ---  bttf4thebigrelase@gmail.com / lorenzo3024@hotmail.it --- |")
                sw.WriteLine("=================================================================")
                sw.WriteLine()
                sw.WriteLine("[" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & "] NAudioAdvanced-IV initialized!")
            End Using
            _hasWrittenWelcomeMessage = True
            _firstClean = True
        Catch

        End Try
    End Sub


    Friend NotInheritable Class GamePathResolver
        Private Sub New() : End Sub
        Friend Shared Function ResolveGamePath() As String
            Try
                Dim asm As Assembly = Assembly.GetAssembly(GetType(GamePathResolver))

                If asm Is Nothing Then
                    ' DEBUG: NativeExtended assembly could not be resolved
                    Return String.Empty
                End If

                Dim location As String = asm.Location

                If String.IsNullOrEmpty(location) Then
                    ' DEBUG: NativeExtended assembly location is empty
                    Return String.Empty
                End If

                Return Path.GetDirectoryName(location)

            Catch ex As Exception
                ' DEBUG: Exception while resolving game path
                Return String.Empty
            End Try
        End Function
    End Class
End Class
