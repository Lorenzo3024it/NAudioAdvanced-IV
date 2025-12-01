Public Class TickHelper
    Friend Shared ReadOnly _TickActions As New List(Of Action)
    Private Shared _lastProcessed As DateTime = DateTime.MinValue
    ''' <summary>
    ''' Return True if ProcessAll was ticked in last 100ms.
    ''' </summary>
    Public Shared ReadOnly Property isRunning As Boolean
        Get
            Return (DateTime.UtcNow - _lastProcessed).TotalMilliseconds < 100
        End Get
    End Property
    ''' <summary>
    ''' Process loops, real time volume control and direction and fade in/out.
    ''' Must be called in a Tick (suggested 1 to 10 ms interval).
    ''' </summary>
    Public Sub ProcessAll()
        If _TickActions.Count = 0 Then
            Exit Sub
        End If

        For Each OnTickHandler In _TickActions.ToArray()
            Try
                OnTickHandler.Invoke()
            Catch ex As Exception
                Tools.WriteLogAdvanced("Error in TickHelper.ProcessAll.", ex)
            End Try
        Next
    End Sub
    '-------------------------------------------------------------------------------------------------
    Friend Shared Sub Add_Internal(OnTickHandler As Action)
        Try
            If OnTickHandler IsNot Nothing AndAlso Not _TickActions.Contains(OnTickHandler) Then
                _TickActions.Add(OnTickHandler)
            End If
        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in TickHelper.Add_Internal: cannot add " & OnTickHandler.ToString & "OnTick void.", ex)
        End Try
    End Sub
    Friend Shared Function ContainsHandler_Internal(OnTickHandler As Action) As Boolean
        Try
            Return _TickActions.Contains(OnTickHandler)
        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in TickHelper.ContainsHandler_Internal on void " & OnTickHandler.ToString, ex)
            Return False
        End Try
    End Function
    Friend Shared Sub Remove_Internal(OnTickHandler As Action)
        Try
            If OnTickHandler IsNot Nothing AndAlso _TickActions.Contains(OnTickHandler) Then
                _TickActions.Remove(OnTickHandler)
            End If
        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in TickHelper.Remove_Internal: cannot remove " & OnTickHandler.ToString & "OnTick void.", ex)
        End Try
    End Sub
End Class
