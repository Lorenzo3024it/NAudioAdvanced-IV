Imports System.IO
Imports GTA
Imports NAudio.Wave
Imports NAudioAdvancedIV.Tools
Public NotInheritable Class Sound
    Implements IDisposable
    Public ReadOnly FolderPath As String
    Public ReadOnly FileName As String
    Public ReadOnly Property IsPlaying As Boolean
        Get
            ThrowIfDisposed()
            Dim state As SoundState = getPlaybackState()

            If state = SoundState.Playing OrElse state = SoundState.PlayingInLoop Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    ''' <summary> 
    ''' Gets the current effective volume or sets the requested volume. 
    ''' When RealTimeVolumeControl is disabled, the effective volume is 
    ''' limited by MaxVolume.
    ''' When RealTimeVolumeControl is enabled, the setter is ignored and 
    ''' the getter returns the current volume after RTVC attenuation.
    ''' </summary> 
    ''' <returns> Current effective volume, from 0 to 100. </returns> 
    Public Property Volume As Integer
        Get
            ThrowIfDisposed()
            If WaveChannel Is Nothing Then Return 0
            Return ClampInteger(CInt(Math.Round(WaveChannel.Volume * 100.0F)), 0, 100)
        End Get
        Set(value As Integer)
            ThrowIfDisposed()
            If RealTimeVolumeControl Then Exit Property 'Volume cannot be manually changed while RTVC ' is controlling the effective volume. 
            value = ClampInteger(value, 0, 100) ' Store the requested volume independently from the ' effective volume imposed by MaxVolume.
            _volume = value / 100.0F
            ApplyVolume()
        End Set
    End Property
    ''' <summary> 
    ''' Gets or sets the maximum volume allowed for this sound. 
    ''' The effective volume can never exceed this value. </summary> 
    Public Property MaxVolume As Integer
        Get
            ThrowIfDisposed()
            Return ClampInteger(CInt(Math.Round(_maxVolume * 100.0F)), 0, 100)
        End Get
        Set(value As Integer)
            ThrowIfDisposed()
            value = ClampInteger(value, 0, 100)
            _maxVolume = value / 100.0F
            ApplyVolume()
        End Set
    End Property
    ''' <summary>Gets the total duration in milliseconds.</summary>
    Public ReadOnly Property Duration As Integer
        Get
            ThrowIfDisposed()
            If Reader Is Nothing Then Return 0
            Return CInt(Reader.TotalTime.TotalMilliseconds)
        End Get
    End Property
    ''' <summary>Gets or sets the current playback position in milliseconds.</summary>
    Public Property CurrentTime As Integer
        Get
            ThrowIfDisposed()
            If Reader Is Nothing Then Return 0
            Return CInt(Reader.CurrentTime.TotalMilliseconds)
        End Get
        Set(value As Integer)
            ThrowIfDisposed()
            If Reader Is Nothing Then Exit Property

            value = ClampInteger(value, 0, CInt(Reader.TotalTime.TotalMilliseconds))
            Reader.CurrentTime = TimeSpan.FromMilliseconds(value)
        End Set
    End Property
    Public ReadOnly Property PlaybackState As SoundState
        Get
            If _disposed Then
                Return SoundState.Disposed
            Else
                Return getPlaybackState()
            End If
        End Get
    End Property
    ''' <summary>
    ''' Gets the current effective stereo balance.
    ''' The setter is ignored while RealTimeVolumeControl is enabled.
    ''' </summary>
    ''' <returns>The current effective stereo balance, from -1.0 (left) to 1.0 (right).</returns>
    Public Property BalanceLR As Single
        Get
            ThrowIfDisposed()
            If WaveChannel Is Nothing Then
                Return If(RealTimeVolumeControl, _rtvcBalance, _balance)
            End If

            Return WaveChannel.Pan
        End Get
        Set(value As Single)
            ThrowIfDisposed()
            If RealTimeVolumeControl Then Exit Property

            value = ClampFloat(value, -1.0F, 1.0F)
            _balance = value

            If WaveChannel IsNot Nothing Then
                WaveChannel.Pan = value
            End If
        End Set
    End Property
    Public Property [Loop] As Boolean
        Get
            ThrowIfDisposed()
            Return _isLoop
        End Get
        Set(value As Boolean)
            ThrowIfDisposed()
            _isLoop = value

            If LoopReader IsNot Nothing Then
                LoopReader.Enabled = value
            End If
        End Set
    End Property
    Public ReadOnly Property Position3D As GTA.Vector3
        Get
            ThrowIfDisposed()
            Return _Pos3D
        End Get
    End Property
    'Public Property StoppedTime As Double
    ''' <summary>
    ''' Max distance for a sound with 3D Position. Default value: 150.0F
    ''' </summary>
    Public Property MaxDistance As Single
        Get
            ThrowIfDisposed()
            Return _maxDistance
        End Get
        Set(value As Single)
            ThrowIfDisposed()
            If value < 0.0F Then
                _maxDistance = 0.0F
            Else
                _maxDistance = value
            End If
        End Set
    End Property
    Public Sub New(Sound As String, Optional Path As String = "")
        Tools.WriteWelcomeMessage()

        FileName = Sound
        FolderPath = Path
        _stoppedManually = False

        Dim fullPath As String

        If String.IsNullOrWhiteSpace(Path) Then
            fullPath = System.IO.Path.Combine(Game.InstallFolder, FileName)
        Else
            fullPath = System.IO.Path.Combine(Game.InstallFolder, Path, FileName)
        End If

        If Not File.Exists(fullPath) Then
            Dim ex404 As New FileNotFoundException("Sound not found.", fullPath)
            Tools.WriteLog("Error in Constructor for " & FileName & ": sound not found.", ex404)
            Throw ex404
        End If

        Try
            Reader = New AudioFileReader(fullPath)
            LoopReader = New LoopStream(Reader)
            WaveChannel = New WaveChannel32(LoopReader)
            MaxDistance = 150.0F
        Catch ex As Exception
            Tools.WriteLog("Error in Constructor for " & FileName & ": cannot create the reader.", ex)
            Throw
        End Try
    End Sub
    Public Sub Play(Optional Volume As Integer = 100, Optional PlayInLoop As Boolean = False, Optional StartTime As Integer = 0)
        Try
            ThrowIfDisposed()
            If Reader Is Nothing Then Exit Sub
            ResetPlaybackMode()

            _Pos3D = New GTA.Vector3(0, 0, 0)
            [Loop] = PlayInLoop
            Me.Volume = Volume
            Play_Internal(StartTime)
            'If OutputDevice Is Nothing Then
            '    OutputDevice = New DirectSoundOut()
            '    OutputDevice.Init(WaveChannel)
            'End If

            '_Pos3D = New GTA.Vector3(0, 0, 0)
            'RealTimeVolumeControl = False

            'If StartTime < 0 Then StartTime = 0
            'If StartTime > Reader.TotalTime.TotalMilliseconds Then StartTime = 0
            'Reader.CurrentTime = TimeSpan.FromMilliseconds(StartTime)

            '[Loop] = PlayInLoop
            'Me.Volume = Volume

            '_stoppedManually = False
            '_hasEndedNaturally = False

            'OutputDevice.Play()
            '_hasStartedPlayback = True
            'EnablePlaybackStateTick()
        Catch ex As Exception
            Tools.WriteLog("Error in Play for" & FileName, ex)
        End Try
    End Sub
    Public Sub Play(Position As GTA.Vector3, Optional PlayInLoop As Boolean = False, Optional StartTime As Integer = 0, Optional MaxDistance As Single? = Nothing)
        Try
            ThrowIfDisposed()
            If Reader Is Nothing Then Exit Sub
            ResetPlaybackMode()
            _Pos3D = Position
            RealTimeVolumeControl = True
            EnableRTVCTick()

            [Loop] = PlayInLoop
            If MaxDistance.HasValue Then Me.MaxDistance = MaxDistance.Value

            Play_Internal(StartTime)
        Catch ex As Exception
            Tools.WriteLog("Error in Play for" & FileName, ex)
        End Try
    End Sub
    ''' <summary>
    ''' Plays the sound attached to a ped. The sound will follow the ped and update
    ''' its position, distance attenuation and stereo panning automatically.
    ''' </summary>
    Public Sub Play(Ped As Ped, Optional PlayInLoop As Boolean = False, Optional StartTime As Integer = 0, Optional MaxDistance As Single? = Nothing)
        Try
            ThrowIfDisposed()
            If Ped Is Nothing OrElse Not Ped.Exists Then Exit Sub
            ResetPlaybackMode()

            _attachedPed = Ped
            _Pos3D = Ped.Position
            RealTimeVolumeControl = True
            EnableAttachedTick()
            EnableRTVCTick()
            [Loop] = PlayInLoop
            If MaxDistance.HasValue Then Me.MaxDistance = MaxDistance.Value

            Play_Internal(StartTime)
        Catch ex As Exception
            Tools.WriteLog("Error in Play for" & FileName, ex)
        End Try
    End Sub
    ''' <summary>
    ''' Plays the sound attached to a vehicle. The sound follows the vehicle position
    ''' and updates volume and direction automatically based on camera position.
    ''' </summary>
    Public Sub Play(Vehicle As Vehicle, Optional PlayInLoop As Boolean = False, Optional StartTime As Integer = 0, Optional MaxDistance As Single? = Nothing)
        Try
            ThrowIfDisposed()
            If Vehicle Is Nothing OrElse Not Vehicle.Exists Then Exit Sub
            ResetPlaybackMode()

            _attachedVeh = Vehicle
            Me._Pos3D = Vehicle.Position
            RealTimeVolumeControl = True
            EnableAttachedTick()
            EnableRTVCTick()

            [Loop] = PlayInLoop
            If MaxDistance.HasValue Then Me.MaxDistance = MaxDistance.Value

            Play_Internal(StartTime)
        Catch ex As Exception
            Tools.WriteLog("Error in Play for" & FileName, ex)
        End Try
    End Sub
    ''' <summary>
    ''' Plays the sound attached to an object/prop. The sound will automatically
    ''' follow the object's world position.
    ''' </summary>
    Public Sub Play(Obj As [Object], Optional PlayInLoop As Boolean = False, Optional StartTime As Integer = 0, Optional MaxDistance As Single? = Nothing)
        Try
            ThrowIfDisposed()
            If Obj Is Nothing OrElse Not Obj.Exists Then Exit Sub
            ResetPlaybackMode()

            _attachedObj = Obj
            Me._Pos3D = Obj.Position
            RealTimeVolumeControl = True
            EnableAttachedTick()
            EnableRTVCTick()

            [Loop] = PlayInLoop
            If MaxDistance.HasValue Then Me.MaxDistance = MaxDistance.Value

            Play_Internal(StartTime)
        Catch ex As Exception
            Tools.WriteLog("Error in Play for" & FileName, ex)
        End Try
    End Sub
    Public Sub Pause()
        Try
            ThrowIfDisposed()
            If OutputDevice Is Nothing Then Exit Sub
            If OutputDevice.PlaybackState <> NAudio.Wave.PlaybackState.Playing Then
                Exit Sub
            End If

            OutputDevice.Pause()
        Catch ex As Exception
            Tools.WriteLog("Error in Pause for " & FileName, ex)
        End Try
    End Sub

    Public Sub [Resume]()
        Try
            ThrowIfDisposed()
            If Reader Is Nothing OrElse OutputDevice Is Nothing Then Exit Sub
            If Not _hasStartedPlayback OrElse _stoppedManually Then Exit Sub

            If OutputDevice.PlaybackState <> NAudio.Wave.PlaybackState.Paused Then
                Exit Sub
            End If

            OutputDevice.Play()
        Catch ex As Exception
            Tools.WriteLog("Error in Resume for " & FileName, ex)
        End Try
    End Sub
    Public Sub [Stop]() '(Optional FadeOut As Integer = 0)
        Try
            ThrowIfDisposed()
            If Reader Is Nothing Then Exit Sub
            OutputDevice?.Stop()
            ResetPlaybackMode()

            _stoppedManually = True
            _hasEndedNaturally = False
            DisablePlaybackStateTick()

            If Reader IsNot Nothing Then Reader.Position = 0

            ' Sound.OutputDevice?.Dispose()
            ' Sound.Reader?.Dispose()
            ' Sound.WaveChannel?.Dispose()

            '  Sound.OutputDevice = Nothing
            '  Sound.Reader = Nothing
            ' Sound.WaveChannel = Nothing

            'Sound.VolumeTimer?.Dispose()
            '  Sound.FadeTimer?.Dispose()

        Catch ex As Exception
            Tools.WriteLog("Error in Stop for " & FileName, ex)
        End Try
    End Sub
    Public Sub Trigger(Optional Volume As Integer = 100, Optional StartTime As Integer = 0)
        Try
            ThrowIfDisposed()
            If Reader Is Nothing Then Exit Sub
            ResetPlaybackMode()

            If StartTime < 0 Then StartTime = 0
            If StartTime > Reader.TotalTime.TotalMilliseconds Then StartTime = 0
            Reader.CurrentTime = TimeSpan.FromMilliseconds(StartTime)

            If OutputDevice Is Nothing Then
                OutputDevice = New DirectSoundOut()
                OutputDevice.Init(WaveChannel)
            End If
            _Pos3D = Vector3.Zero
            RealTimeVolumeControl = False
            [Loop] = False
            Me.Volume = Volume

            _stoppedManually = False
            _hasEndedNaturally = False

            OutputDevice.Play()
            _hasStartedPlayback = True
            EnablePlaybackStateTick()
        Catch ex As Exception
            Tools.WriteLog("Error in Trigger for " & FileName, ex)
        End Try
    End Sub
    Public Sub Trigger(SoundPosition As GTA.Vector3, Optional StartTime As Integer = 0, Optional MaxDistance? As Single = Nothing)
        Try
            ThrowIfDisposed()
            If Reader Is Nothing Then Exit Sub
            ResetPlaybackMode()

            _Pos3D = SoundPosition
            RealTimeVolumeControl = False
            [Loop] = False
            If MaxDistance.HasValue Then Me.MaxDistance = MaxDistance.Value
            '------------------------------------------------

            If OutputDevice Is Nothing Then
                OutputDevice = New DirectSoundOut()
                OutputDevice.Init(WaveChannel)
            End If

            If StartTime < 0 Then StartTime = 0
            If StartTime > Reader.TotalTime.TotalMilliseconds Then StartTime = 0
            Reader.CurrentTime = TimeSpan.FromMilliseconds(StartTime)

            _stoppedManually = False
            _hasEndedNaturally = False

            SetVolumeAndDirection_OnTick(Me, _Pos3D, 1, Me.MaxDistance, True, True)
            OutputDevice.Play()
            _hasStartedPlayback = True
            EnablePlaybackStateTick()
        Catch ex As Exception
            Tools.WriteLog("Error in Trigger for" & FileName, ex)
        End Try
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Enum SoundState
        Disposed = -2
        [Error] = -1
        NotPlayed = 0
        Playing = 1
        PlayingInLoop = 2
        Paused = 3
        Ended = 4
        Stopped = 5
        ' FadingIn
        ' FadingOut
    End Enum


#Region "Internals"
    Private Reader As AudioFileReader
    Private LoopReader As LoopStream
    Private OutputDevice As DirectSoundOut
    Private WaveChannel As WaveChannel32
    Private _Pos3D As Vector3
    Private _balance As Single
    Private _maxDistance As Single = 150.0F
    Private _stoppedManually As Boolean
    Private _hasStartedPlayback As Boolean = False
    Private _hasEndedNaturally As Boolean = False
    Private _isLoop As Boolean
    Private _realTimeControl As Boolean
    Private _volume As Single = 1.0F
    Private _maxVolume As Single = 1.0F
    Private _rtvcVolume As Single = 1.0F
    Private _rtvcBalance As Single = 0.0F
    '-----------
    Private _rtvcHandler As Action
    Private _attachedHandler As Action
    Private _playbackStateHandler As Action
    '-----------
    Private _attachedVeh As GTA.Vehicle
    Private _attachedPed As GTA.Ped
    Private _attachedObj As GTA.Object
    '-----------
    Private _disposed As Boolean = False
    Private ReadOnly _disposeLock As New Object()
    '-----------------
    Friend Property RealTimeVolumeControl As Boolean
        Get
            Return _realTimeControl
        End Get
        Set(value As Boolean)
            If _realTimeControl = value Then Exit Property
            _realTimeControl = value
            If WaveChannel Is Nothing Then Exit Property

            If value Then
                ' RTVC
            Else
                ' last manual balance
                WaveChannel.Pan = _balance
            End If

            ApplyVolume()
        End Set
    End Property
    Private Function getPlaybackState() As SoundState
        If Reader Is Nothing Then
            Return SoundState.Error
        End If

        If _stoppedManually Then
            Return SoundState.Stopped
        End If

        If _hasEndedNaturally Then
            Return SoundState.Ended
        End If

        If Not _hasStartedPlayback Then
            Return SoundState.NotPlayed
        End If

        If OutputDevice Is Nothing Then
            Return SoundState.NotPlayed
        End If

        Try
            Select Case OutputDevice.PlaybackState

                Case NAudio.Wave.PlaybackState.Playing
                    If [Loop] Then
                        Return SoundState.PlayingInLoop
                    End If
                    Return SoundState.Playing

                Case NAudio.Wave.PlaybackState.Paused
                    Return SoundState.Paused

                Case NAudio.Wave.PlaybackState.Stopped
                    If [Loop] Then
                        Return SoundState.NotPlayed
                    End If
                    If Reader.CurrentTime >= Reader.TotalTime Then
                        MarkNaturalEnd()
                        Return SoundState.Ended
                    End If
                    Return SoundState.NotPlayed

                Case Else
                    Return SoundState.NotPlayed
            End Select
        Catch ex As Exception
            Tools.WriteLog("Error in getPlaybackState for " & FileName, ex)
            Return SoundState.Error
        End Try
    End Function
    Private Sub ThrowIfDisposed()
        If _disposed Then
            Throw New ObjectDisposedException(NameOf(Sound))
        End If
    End Sub
    Protected Sub Dispose(disposing As Boolean)
        SyncLock _disposeLock
            If _disposed Then Return

            _disposed = True

            If Not disposing Then Return

            DisablePlaybackStateTick()
            DisableAttachedTick()
            DisableRTVCTick()

            _attachedPed = Nothing
            _attachedVeh = Nothing
            _attachedObj = Nothing

            _realTimeControl = False

            If OutputDevice IsNot Nothing Then
                Try
                    OutputDevice.Stop()
                Catch ex As Exception
                    WriteLog("Error stopping OutputDevice while disposing " & FileName, ex)
                End Try

                Try
                    OutputDevice.Dispose()
                Catch ex As Exception
                    Tools.WriteLog("Error disposing OutputDevice for " & FileName, ex)
                Finally
                    OutputDevice = Nothing
                End Try
            End If

            ' Rilascia il canale audio.
            If WaveChannel IsNot Nothing Then
                Try
                    WaveChannel.Dispose()
                Catch ex As Exception
                    Tools.WriteLog("Error disposing WaveChannel for " & FileName, ex)
                Finally
                    WaveChannel = Nothing
                End Try
            End If

            LoopReader = Nothing

            If Reader IsNot Nothing Then
                Try
                    Reader.Dispose()
                Catch ex As Exception
                    Tools.WriteLog("Error disposing Reader for " & FileName, ex)
                Finally
                    Reader = Nothing
                End Try
            End If

            _hasStartedPlayback = False
            _hasEndedNaturally = False
            _stoppedManually = True

            _rtvcVolume = 1.0F
            _rtvcBalance = 0.0F

            _volume = 1.0F
            _maxVolume = 1.0F
            _balance = 0.0F

            _isLoop = False
            _Pos3D = New GTA.Vector3(0, 0, 0)
        End SyncLock
    End Sub
    Private Sub ApplyVolume()
        If WaveChannel Is Nothing Then Exit Sub
        Dim effectiveVolume As Single
        If RealTimeVolumeControl Then
            effectiveVolume = _maxVolume * _rtvcVolume
        Else
            effectiveVolume = Math.Min(_volume, _maxVolume)
        End If
        effectiveVolume = Math.Max(0.0F, Math.Min(1.0F, effectiveVolume))
        WaveChannel.Volume = effectiveVolume
    End Sub
    Private Sub ApplySpatialVolume()
        If WaveChannel Is Nothing Then Exit Sub

        Dim effectiveVolume As Single = _maxVolume * _rtvcVolume
        effectiveVolume = Math.Max(0.0F, Math.Min(1.0F, effectiveVolume))

        WaveChannel.Volume = effectiveVolume
    End Sub
    '-------------------------------------
    'Attached Entity
    '------------------
    Private Sub EnableAttachedTick()
        If _attachedHandler Is Nothing Then
            _attachedHandler = Sub()
                                   'Ped
                                   If _attachedPed IsNot Nothing AndAlso _attachedPed.Exists Then
                                       _Pos3D = _attachedPed.Position
                                   End If
                                   'Obj
                                   If _attachedObj IsNot Nothing AndAlso _attachedObj.Exists Then
                                       _Pos3D = _attachedObj.Position
                                   End If
                                   'Vehicles
                                   If _attachedVeh IsNot Nothing AndAlso _attachedVeh.Exists Then
                                       _Pos3D = _attachedVeh.Position
                                   End If
                               End Sub
            TickHelper.Add_Internal(_attachedHandler)
        End If
    End Sub
    Private Sub DisableAttachedTick()
        If _attachedHandler IsNot Nothing Then
            TickHelper.Remove_Internal(_attachedHandler)
            _attachedHandler = Nothing
        End If
    End Sub

    '-------------------------------------
    '------------------
    'Real Time Volume Control
    '------------------
    Private Sub EnableRTVCTick()
        If _rtvcHandler IsNot Nothing Then Exit Sub

        _rtvcHandler = Sub()
                           SetVolumeAndDirection_OnTick(Me, Me._Pos3D, 1.0F, Me.MaxDistance, True, False)
                       End Sub

        TickHelper.Add_Internal(_rtvcHandler)
    End Sub
    Private Sub EnablePlaybackStateTick()
        If _playbackStateHandler IsNot Nothing Then Exit Sub

        _playbackStateHandler = Sub()
                                    If Reader Is Nothing OrElse OutputDevice Is Nothing Then Exit Sub
                                    If _stoppedManually OrElse Not _hasStartedPlayback Then Exit Sub
                                    If _hasEndedNaturally OrElse [Loop] Then Exit Sub

                                    If OutputDevice.PlaybackState = NAudio.Wave.PlaybackState.Stopped Then
                                        If Reader.CurrentTime >= Reader.TotalTime Then
                                            MarkNaturalEnd()
                                        End If
                                    End If
                                End Sub

        TickHelper.Add_Internal(_playbackStateHandler)
    End Sub
    Private Sub DisablePlaybackStateTick()
        If _playbackStateHandler Is Nothing Then Exit Sub

        TickHelper.Remove_Internal(_playbackStateHandler)
        _playbackStateHandler = Nothing
    End Sub
    Private Sub DisableRTVCTick()
        If _rtvcHandler IsNot Nothing Then
            TickHelper.Remove_Internal(_rtvcHandler)
            _rtvcHandler = Nothing
        End If
    End Sub
    Private Sub MarkNaturalEnd()
        If _hasEndedNaturally Then Return

        _hasEndedNaturally = True

        DisablePlaybackStateTick()
        DisableAttachedTick()
        DisableRTVCTick()

        _attachedPed = Nothing
        _attachedVeh = Nothing
        _attachedObj = Nothing
    End Sub
    Private Sub Play_Internal(StartTime As Integer)
        ThrowIfDisposed()
        If Reader Is Nothing OrElse WaveChannel Is Nothing Then Exit Sub

        If OutputDevice Is Nothing Then
            OutputDevice = New DirectSoundOut()
            OutputDevice.Init(WaveChannel)

        ElseIf OutputDevice.PlaybackState = NAudio.Wave.PlaybackState.Playing OrElse OutputDevice.PlaybackState = NAudio.Wave.PlaybackState.Paused Then
            OutputDevice.Stop()
        End If

        If StartTime < 0 OrElse StartTime >= Reader.TotalTime.TotalMilliseconds Then StartTime = 0

        Reader.CurrentTime = TimeSpan.FromMilliseconds(StartTime)

        _stoppedManually = False
        _hasEndedNaturally = False
        _hasStartedPlayback = False

        OutputDevice.Play()
        _hasStartedPlayback = True
        EnablePlaybackStateTick()
    End Sub
    Private Sub ResetPlaybackMode()
        DisableAttachedTick()
        DisableRTVCTick()

        _attachedPed = Nothing
        _attachedVeh = Nothing
        _attachedObj = Nothing

        RealTimeVolumeControl = False

        _rtvcVolume = 1.0F
        _rtvcBalance = 0.0F

        If WaveChannel IsNot Nothing Then
            WaveChannel.Pan = _balance
        End If

        ApplyVolume()
    End Sub
    Private Shared Sub SetVolumeAndDirection_OnTick(Sound As Sound, SoundPosition As Vector3, Optional Coefficient As Single = 1.0F, Optional MaxDistance As Single? = Nothing, Optional EnableStereoPanning As Boolean = True, Optional ForceSpatialVolume As Boolean = False)
        Try
            If Sound Is Nothing OrElse Game.CurrentCamera Is Nothing Then Exit Sub
            If Sound.OutputDevice Is Nothing OrElse Sound.Reader Is Nothing Then Exit Sub

            Dim camPos As Vector3 = Game.CurrentCamera.Position
            Dim camDir As Vector3 = Game.CurrentCamera.Direction
            Dim dirToSound As Vector3 = SoundPosition - camPos
            Dim distance As Single = dirToSound.Length()

            If distance >= MaxDistance Then
                Sound._rtvcVolume = 0.0F
                If ForceSpatialVolume Then
                    Sound.ApplySpatialVolume()
                Else
                    Sound.ApplyVolume()
                End If
                Exit Sub
            End If

            Dim attenuation As Single = Coefficient / (1.0F + (distance / (MaxDistance / 5.0F)) ^ 2.0F)
            attenuation = ClampFloat(attenuation, 0.0F, 1.0F)
            Sound._rtvcVolume = attenuation
            If ForceSpatialVolume Then
                Sound.ApplySpatialVolume()
            Else
                Sound.ApplyVolume()
            End If

            '=== Panning L/R  ===
            If EnableStereoPanning Then
                dirToSound.Normalize()
                camDir.Normalize()

                Dim cross As Vector3 = Vector3.Cross(camDir, dirToSound)
                Dim dot As Single = Vector3.Dot(camDir, dirToSound)
                Dim angle As Single = Math.Atan2(cross.Z, dot) ' radiante [-π, π]

                Dim rawPan As Single = CSng(Math.Sin(angle))
                Dim smoothPan As Single = CSng(Math.Tanh(rawPan * 1.2F)) ' 1.2F regola la sensibilità

                Dim proximityFactor As Single = Math.Min(1.0F, distance / (MaxDistance.Value / 4.0F))
                smoothPan *= proximityFactor

                smoothPan = ClampFloat(smoothPan, -1.0F, 1.0F)

                Dim lerpSpeed As Single = 0.1F

                Sound._rtvcBalance += (smoothPan - Sound._rtvcBalance) * lerpSpeed

                If Sound.WaveChannel IsNot Nothing Then
                    Sound.WaveChannel.Pan = Sound._rtvcBalance
                End If
            End If

        Catch ex As Exception
            Tools.WriteLog("Error in SetVolumeAndDirection_OnTick for " & Sound.FileName, ex)
        End Try

    End Sub
    '-------------------------------------
    Private Class LoopStream
        Inherits WaveStream

        Private ReadOnly _source As WaveStream
        Public Property Enabled As Boolean
        Public Sub New(source As WaveStream)
            _source = source
            Enabled = False
        End Sub
        Public Overrides ReadOnly Property WaveFormat As WaveFormat
            Get
                Return _source.WaveFormat
            End Get
        End Property
        Public Overrides ReadOnly Property Length As Long
            Get
                Return _source.Length
            End Get
        End Property
        Public Overrides Property Position As Long
            Get
                Return _source.Position
            End Get
            Set(value As Long)
                _source.Position = value
            End Set
        End Property
        Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
            If Not Enabled Then
                Return _source.Read(buffer, offset, count)
            End If
            If _source.Length = 0 Then
                Return 0
            End If

            Dim totalRead As Integer = 0
            While totalRead < count AndAlso Enabled = True
                Dim bytesRead As Integer = _source.Read(buffer, offset + totalRead, count - totalRead)
                If bytesRead = 0 Then
                    _source.Position = 0
                    Continue While
                End If
                totalRead += bytesRead
            End While
            Return totalRead
        End Function
    End Class
#End Region

End Class