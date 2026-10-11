''' <summary>
''' Easy TickHelper to process all loops and real time volume position balancing.
''' </summary>
Public NotInheritable Class TickHelper
    Friend Shared ReadOnly _TickActions As New List(Of Action)
    Private Shared _lastProcessed As DateTime = DateTime.MinValue
    Private Shared _lastProcessHadTicks As Boolean = False
    Private Shared _lastTickTime As DateTime = DateTime.UtcNow
    Private Sub New() : End Sub
    ''' <summary>
    ''' Return True if ProcessAll was ticked in last 500ms.
    ''' </summary>
    Public Shared ReadOnly Property IsRunning As Boolean
        Get
            If _lastProcessed = DateTime.MinValue Then
                Return False
            End If
            Return (DateTime.UtcNow - _lastProcessed).TotalMilliseconds < 500.0
        End Get
    End Property

    ''' <summary>
    ''' Returns time elapsed (milliseconds) after last tick and if 
    ''' TickHelper had any action to process.
    ''' </summary>
    Friend Shared ReadOnly Property LastTick As (Integer, Boolean)
        Get
            Dim elapsed As Integer
            If _lastTickTime = DateTime.MinValue Then
                elapsed = Integer.MaxValue
            Else
                Dim milliseconds As Double =
                    (DateTime.UtcNow - _lastTickTime).TotalMilliseconds
                If milliseconds < 0.0 Then
                    milliseconds = 0.0
                End If
                If milliseconds > Integer.MaxValue Then
                    elapsed = Integer.MaxValue
                Else
                    elapsed = CInt(milliseconds)
                End If
            End If
            Return (elapsed, _lastProcessHadTicks)
        End Get
    End Property
    ''' <summary>
    ''' Process all ticks added to the TickHelper.
    ''' Works with internal ticks and NativeTimer instances.
    ''' </summary>
    Public Shared Sub ProcessAll()
        _lastProcessed = DateTime.UtcNow
        _lastTickTime = _lastProcessed
        _lastProcessHadTicks = (_TickActions.Count > 0)

        If _TickActions.Count = 0 Then Exit Sub

        For Each OnTickHandler As Action In _TickActions.ToArray()
            If OnTickHandler Is Nothing Then
                Continue For
            End If
            Try
                OnTickHandler.Invoke()
            Catch ex As Exception
                Tools.WriteLog("Error in TickHelper.ProcessAll.", ex)
            End Try
        Next
    End Sub
    '------------------------------------------------------------------------------------------------------------------------
    Friend Shared Sub Add_Internal(OnTickHandler As Action)
        If OnTickHandler Is Nothing Then Exit Sub

        Try
            If Not _TickActions.Contains(OnTickHandler) Then
                _TickActions.Add(OnTickHandler)
            End If
        Catch ex As Exception
            Dim HandlerName As String = "Nothing"
            Try
                HandlerName = OnTickHandler.ToString()
            Catch
            End Try
            Tools.WriteLog("Error in TickHelper.Add_Internal: cannot add " & HandlerName.ToString & "OnTick void.", ex)
        End Try
    End Sub
    Friend Shared Function ContainsHandler_Internal(OnTickHandler As Action) As Boolean
        If OnTickHandler Is Nothing Then Return False

        Try
            Return _TickActions.Contains(OnTickHandler)
        Catch ex As Exception
            Dim HandlerName As String = "Nothing"
            Try
                HandlerName = OnTickHandler.ToString()
            Catch
            End Try
            Tools.WriteLog("Error in TickHelper.ContainsHandler_Internal on void " & HandlerName.ToString, ex)
            Return False
        End Try
    End Function
    Friend Shared Sub Remove_Internal(OnTickHandler As Action)
        If OnTickHandler Is Nothing Then Exit Sub

        Try
            If _TickActions.Contains(OnTickHandler) Then
                _TickActions.Remove(OnTickHandler)
            End If
        Catch ex As Exception
            Dim HandlerName As String = "Nothing"
            Try
                HandlerName = OnTickHandler.ToString()
            Catch
            End Try
            Tools.WriteLog("Error in TickHelper.Remove_Internal: cannot remove " & OnTickHandler.ToString & "OnTick void.", ex)
        End Try
    End Sub
End Class
