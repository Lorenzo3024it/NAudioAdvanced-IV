Imports System.ComponentModel
Imports System.IO
Imports GTA
Imports System.Net.Http.Headers
Imports NAudio.Wave
'Public Class NAudioAdvancedOLD
'    Private Reader As AudioFileReader
'    Private OutputDevice As DirectSoundOut
'    Private WaveChannel As WaveChannel32

'    Public ReadOnly Property FolderPath As String
'    Public ReadOnly Property FileName As String
'    Public Property Volume As Integer
'        Get
'            Return CInt(WaveChannel.Volume * 100)
'        End Get
'        Set(value As Integer)
'            If value < 0 Then value = 0
'            If value > 100 Then value = 100

'            WaveChannel.Volume = CSng(value / 100)
'        End Set
'    End Property
'    Public ReadOnly Property isEnded As Boolean
'    Public Property isLoop As Boolean
'    Public Property RealTimeVolumeControl As Boolean
'    Public Property Position3D As Vector3
'    Public Property StoppedTime As Double
'    'Public posMFR As Long
'    Public Sub New(Sound As String, Optional Path As String = "")
'        FileName = Sound
'        FolderPath = Path
'        StoppedTime = 0

'        'Reader = New AudioFileReader(Game.InstallFolder & "\" & Path & FileName)
'        'WaveChannel = New WaveChannel32(Reader)
'        'OutputDevice = New DirectSoundOut()
'        'OutputDevice.Init(Reader)

'        Dim fullPath As String = Game.InstallFolder & "\" & Path & FileName
'        If Not File.Exists(fullPath) Then
'            Throw New FileNotFoundException("File audio non trovato.", Game.InstallFolder & "\" & Path & FileName)
'        End If

'        Try
'            ' Crea solo il Reader qui. L’OutputDevice lo creeremo solo quando serva (Play)
'            Reader = New AudioFileReader(fullPath)
'            WaveChannel = New WaveChannel32(Reader)
'        Catch ex As Exception
'            Console.WriteLine($"[NAudioAdvanced] Errore durante la creazione del reader per {FileName}: {ex.Message}")
'            Reader = Nothing
'            WaveChannel = Nothing
'        End Try
'    End Sub
'    Public Shared Sub PlayTEST(Sound As NAudioAdvanced, Optional Volume As Integer = 100, Optional ResumeAudio As Boolean = True, Optional PlayInLoop As Boolean = False)
'        Try
'            If Sound Is Nothing OrElse Sound.Reader Is Nothing Then
'                Console.WriteLine("[NAudioAdvanced] Nessun suono valido da riprodurre.")
'                Exit Sub
'            End If

'            ' Crea il dispositivo solo se non esiste
'            If Sound.OutputDevice Is Nothing Then
'                Sound.OutputDevice = New DirectSoundOut()
'                Sound.OutputDevice.Init(Sound.WaveChannel)
'            End If

'            If ResumeAudio Then Sound.Reader.Position = 0
'            Sound.isLoop = PlayInLoop
'            Sound.WaveChannel.Volume = CSng(Volume / 100)

'            Sound.OutputDevice.Play()

'        Catch ex As Exception
'            Console.WriteLine($"[NAudioAdvanced] Errore durante PlayAudio: {ex.Message}")
'        End Try
'    End Sub

'    Public Shared Sub PlayAudio(Sound As NAudioAdvanced, Optional Volume As Integer = 100, Optional ResumeAudio As Boolean = True, Optional PlayInLoop As Boolean = False)
'        If ResumeAudio Then Sound.Reader.Position = 0
'        If PlayInLoop Then Sound.isLoop = True
'        Sound.WaveChannel.Volume = Volume / 100
'        Sound.OutputDevice.Play()
'        'Sound.WOE.Play()
'    End Sub
'    Public Shared Sub PlayAudio(Sound As NAudioAdvanced, SoundPosition As Vector3, Optional MaxDistance_50F As Boolean = False, Optional RealTimeVolumeControl As Boolean = False, Optional ResumeAudio As Boolean = True, Optional PlayInLoop As Boolean = False)
'        Dim DistanceToCam As Double = Game.CurrentCamera.Position.DistanceTo(SoundPosition)
'        Dim VolumeMinus As Integer
'        If DistanceToCam <= 25 Then
'            VolumeMinus = DistanceToCam * 2
'        ElseIf DistanceToCam > 25 AndAlso DistanceToCam <= 50 Then
'            VolumeMinus = DistanceToCam * 1.75
'        ElseIf DistanceToCam > 50 AndAlso DistanceToCam < 150 AndAlso MaxDistance_50F = False Then
'            VolumeMinus = 87.5 + ((DistanceToCam - 50) / 12.5)
'        ElseIf DistanceToCam >= 150 Or MaxDistance_50F = True Then
'            VolumeMinus = 100
'        End If
'        Sound.WaveChannel.Volume = (100 - VolumeMinus) / 100
'        If ResumeAudio Then
'            '    Sound.DSO.Stop()
'            '    Sound.DSO.Dispose()
'            Sound.Reader.Position = 0
'        End If
'        If PlayInLoop Then Sound.isLoop = True
'        Sound.RealTimeVolumeControl = RealTimeVolumeControl
'        ' Sound.DSO.Play()
'        Sound.OutputDevice.Play()
'        Sound.Position3D = SoundPosition
'        ' soundpo
'        'Game.DisplayText(IsPlaying(Sound), 5000)
'    End Sub
'    Public Shared Sub PauseAudio(Sound As NAudioAdvanced)
'        'Sound.DSO.Pause()
'        Sound.OutputDevice.Pause()
'    End Sub
'    Public Shared Sub StopAudio(Sound As NAudioAdvanced)
'        If Sound.isLoop Then Sound.isLoop = False
'        Sound.OutputDevice.Stop()
'        Sound.Reader.Position = 0
'        'Sound.DSO.Dispose()
'        'Sound.WOE.Stop()
'    End Sub
'    Public Shared Function isPlaying(Sound As NAudioAdvanced) As Boolean
'        Dim flag As Boolean = ((Sound.Reader.Position <> 0) AndAlso (Sound.Reader.Position <= Sound.Reader.Length))
'        'Sound.MFRPosition = Sound.MFR.Position
'        If flag = False Then
'            If Sound.StoppedTime < 100 Then
'                Sound.StoppedTime = Sound.StoppedTime + 20 '10
'            End If
'        Else
'            Sound.StoppedTime = 0
'        End If
'        Return (Sound.StoppedTime < 100)
'        'Dim flag As Boolean = ((Sound.MFR.Position <> 0) AndAlso (Sound.MFR.Position <> Sound.MFRPosition))
'        'Sound.MFRPosition = Sound.MFR.Position
'        'If flag = False Then
'        '    If Sound.StoppedTime < 100 Then
'        '        Sound.StoppedTime = Sound.StoppedTime + 20 '10
'        '    End If
'        'Else
'        '    Sound.StoppedTime = 0
'        'End If
'        'Return (Sound.StoppedTime < 100)
'        'Dim Flag As Integer
'        'If ((Sound.MFR.Position <> 0) AndAlso (Sound.MFR.Position <= Sound.MFR.Length)) Then
'        '    Flag = 1 'is playing
'        'ElseIf Sound.MFR.Position = 0 Then
'        '    Flag = 0 'not playing
'        'ElseIf Sound.MFR.Position > Sound.MFR.Length Then
'        '    If Sound.IsLoop Then
'        '        Flag = 2 'just ended, play again in loop 
'        '    Else
'        '        Flag = 0 'ended
'        '    End If
'        'End If
'        'Select Case Flag
'        '    Case 0
'        '        'Sound.isEnded = False
'        '        'Sound.MFR.Position = 0
'        '        Return False
'        '    Case 1
'        '        Return True
'        '    Case 2
'        '        Sound.MFR.Position = 0
'        '        'Sound.isEnded = False
'        'End Select

'        '------------------------
'        'TEST CON LOOP
'        'Dim Flag As Boolean = ((Sound.MFR.Position > 0) AndAlso (Sound.MFR.Position <= Sound.MFR.Length))
'        'If Flag = False Then
'        '    If Sound.IsLoop Then
'        '        'Sound.posMFR = 0
'        '        Sound.MFR.Position = 0
'        '        Sound.isEnded = False
'        '    Else
'        '        Sound.MFR.Position = 0
'        '        Sound.isEnded = True
'        '    End If
'        'Else
'        '    If Sound.IsLoop = False Then
'        '        'Sound.StoppedTime = 0
'        '        Sound.MFR.Position = 0
'        '        'Sound.DSO.Stop()
'        '    End If
'        '    'Sound.isEnded = True
'        'End If

'        'If Sound.MFR.Position = 0 AndAlso Sound.IsLoop Then
'        '    Sound.WFR.Position = 1
'        'End If
'        'Return Flag
'    End Function
'    Public Shared Function isRealTimeVolumeControl(Sound As NAudioAdvanced)
'        Return Sound.RealTimeVolumeControl
'    End Function
'    Public Shared Function isPlayingLoop(Sound As NAudioAdvanced)
'        Return Sound.isLoop
'    End Function
'    '------------------------------------------------------------------------------------------------------------------------------------
'    Public Shared Sub TriggerAudio(Sound As NAudioAdvanced, Optional Volume As Integer = 100)
'        Dim Reader = New AudioFileReader(Game.InstallFolder & Sound.FolderPath & Sound.FileName)
'        Dim WaveChannel = New WaveChannel32(Reader)
'        'Dim WOE = New WaveOutEvent
'        Dim OutputDevice = New DirectSoundOut
'        'WOE.Init(WC)
'        OutputDevice.Init(Reader)
'        'OutputDevice.Volume = Volume / 100
'        WaveChannel.Volume = Volume / 100
'        'WOE.Play()
'        OutputDevice.Play()
'    End Sub
'    Public Shared Sub TriggerAudio(Sound As NAudioAdvanced, SoundPosition As Vector3, Optional MaxDistance_50F As Boolean = False)
'        Dim Reader = New AudioFileReader(Game.InstallFolder & Sound.FolderPath & Sound.FileName)
'        Dim WaveChannel = New WaveChannel32(Reader)
'        'Dim WOE = New WaveOutEvent
'        Dim OutputDevice = New DirectSoundOut
'        OutputDevice.Init(Reader)
'        '------------------------------------------------
'        'Calculating volume related to distance
'        Dim DistanceToCam As Double = Game.CurrentCamera.Position.DistanceTo(SoundPosition)
'        Dim VolumeMinus As Integer
'        If DistanceToCam <= 25 Then
'            VolumeMinus = DistanceToCam * 2
'        ElseIf DistanceToCam > 25 AndAlso DistanceToCam <= 50 Then
'            VolumeMinus = DistanceToCam * 1.75
'        ElseIf DistanceToCam > 50 AndAlso DistanceToCam < 150 AndAlso MaxDistance_50F = False Then
'            VolumeMinus = 87.5 + ((DistanceToCam - 50) / 12.5)
'        ElseIf DistanceToCam >= 150 Or MaxDistance_50F = True Then
'            VolumeMinus = 100
'        End If
'        WaveChannel.Volume = (100 - VolumeMinus) / 100
'        OutputDevice.Play()
'    End Sub
'End Class

Public Class NAudioAdvanced
    Private Reader As AudioFileReader
    Private OutputDevice As DirectSoundOut
    Private WaveChannel As WaveChannel32
    Private PrivateSound As NAudioAdvanced
    Private _balance As Single
    Private _stoppedManually As Boolean
    Private _isLoop As Boolean
    Private _realTimeControl As String
    '-----------
    Private _loopHandler As Action
    Private _rtvcHandler As Action
    Private _attachedHandler As Action
    '-----------
    Private _attachedVeh As GTA.Vehicle
    Private _attachedPed As GTA.Ped
    Private _attachedObj As GTA.Object
    '-----------

    '  Private _isLoop As Boolean
    ' Private VolumeTimer As Timer
    ' Private FadeTimer As Timer
    ' Private FadeLock As New Object()
    Public ReadOnly FolderPath As String
    Public ReadOnly FileName As String
    ' Public Shared SoundsList As New List(Of NAudioAdvanced)
    Public ReadOnly Property Advanced As Tools.AdvancedAudioTools
    'Public ReadOnly Property IsEnded As Boolean
    Public Property Volume As Integer
        Get
            If WaveChannel Is Nothing Then Return 100
            Return CInt(WaveChannel.Volume * 100)
        End Get
        Set(value As Integer)
            If WaveChannel Is Nothing Then Exit Property
            If value < 0 Then value = 0
            If value > 100 Then value = 100
            WaveChannel.Volume = CSng(value / 100)
        End Set
    End Property
    ''' <returns>Total duration in milliseconds</returns>
    Public ReadOnly Property Duration As Integer
        Get
            Return Reader.TotalTime.TotalMilliseconds
        End Get
    End Property
    ''' <returns>Current time in milliseconds</returns>
    Public Property CurrentTime As Integer
        Get
            Return Reader.CurrentTime.TotalMilliseconds
        End Get
        Set(value As Integer)
            If WaveChannel Is Nothing Then Exit Property
            If Reader Is Nothing Then Exit Property
            If value < 0 Then value = 0
            If value > 100 Then value = 100
            Reader.CurrentTime = TimeSpan.FromMilliseconds(value)
        End Set
    End Property
    Public ReadOnly Property PlaybackState As State
        Get
            Return getPlaybackState()
        End Get
    End Property
    ''' <summary>
    ''' Regola il bilanciamento stereo: -1.0 = sinistra, 0.0 = centro, +1.0 = destra.
    ''' </summary>
    Public Property BalanceLR As Single
        Get
            Return _balance
        End Get
        Set(value As Single)
            If WaveChannel Is Nothing Then Exit Property

            If value < -1.0F Then value = -1.0F
            If value > 1.0F Then value = 1.0F

            _balance = value

            Dim leftVol As Single = 1.0F
            Dim rightVol As Single = 1.0F

            If _balance < 0 Then
                rightVol = 1.0F + _balance
            ElseIf _balance > 0 Then
                leftVol = 1.0F - _balance
            ElseIf _balance = 0 Then
                WaveChannel.Pan = 0
            End If

            WaveChannel.Pan = _balance
        End Set
    End Property
    Public Property [Loop] As Boolean
    Public Property RealTimeVolumeControl As Boolean
        Get
            Return _realTimeControl
        End Get
        Set(value As Boolean)
            If value = True Then
            Else
                _realTimeControl = False
            End If
        End Set
    End Property
    Public Property Position3D As GTA.Vector3
    'Public Property StoppedTime As Double
    Public Property MaxDistance As Single = 150.0
    Public Sub New(Sound As String, Optional Path As String = "")
        Tools.WriteWelcomeMessage()

        FileName = Sound
        FolderPath = Path
        _stoppedManually = False

        Dim fullPath As String = GTA.Game.InstallFolder & "\" & Path & "\" & FileName
        If Not File.Exists(fullPath) Then
            Dim ex404 = New FileNotFoundException("Sound not found.", fullPath)
            Throw ex404
            Tools.WriteLogAdvanced("Error in Constructor for " & FileName & ": sound not found.", ex404)
        End If

        Try
            Reader = New AudioFileReader(fullPath)
            WaveChannel = New WaveChannel32(Reader)
            'SoundsList.Add(Me)

            Advanced = New Tools.AdvancedAudioTools(Reader, WaveChannel, OutputDevice)
        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in Constructor for " & FileName & ": cannot create the reader.", ex)
        End Try
    End Sub
    Private Function getPlaybackState() As State
        If Reader Is Nothing Then
            Return State.Error
        End If

        Try
            'If Sound.FadeTimer IsNot Nothing Then
            '    If Sound.WaveChannel.Volume < 0.01F Then
            '        Return PlaybackStateAdvanced.FadingOut
            '    Else
            '        Return PlaybackStateAdvanced.FadingIn
            '    End If
            'End If

            If OutputDevice IsNot Nothing AndAlso OutputDevice.PlaybackState = NAudio.Wave.PlaybackState.Paused Then
                Return State.Paused
            End If

            If Reader.CurrentTime >= Reader.TotalTime Then
                If [Loop] Then
                    Return State.PlayingInLoop
                Else
                    Return State.Ended
                End If
            End If
            If OutputDevice IsNot Nothing AndAlso OutputDevice.PlaybackState = NAudio.Wave.PlaybackState.Playing Then
                If [Loop] Then
                    Return State.PlayingInLoop
                Else
                    Return State.Playing
                End If
            End If

            If _stoppedManually Then
                Return State.Stopped
            Else
                Return State.NotPlayed
            End If
            'Return PlaybackStateAdvanced.Stopped

        Catch
            If _stoppedManually Then
                Return State.Stopped
            Else
                Return State.NotPlayed
            End If
        End Try
    End Function
    Public Function isPlaying() As Boolean
        If PlaybackState = 1 Or PlaybackState = 2 Or PlaybackState = 3 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Sub Play(Optional Volume As Integer = 100, Optional PlayInLoop As Boolean = False, Optional StartTime As Integer = 0)
        Try
            If Reader Is Nothing Then
                Exit Sub
            End If

            If OutputDevice Is Nothing Then
                OutputDevice = New DirectSoundOut()
                OutputDevice.Init(WaveChannel)
            End If

            Position3D = New GTA.Vector3(0, 0, 0)

            Dim Start As Integer
            If StartTime < 0 Then Start = 0
            Reader.CurrentTime = TimeSpan.FromMilliseconds(Start)
            [Loop] = PlayInLoop
            WaveChannel.Volume = CSng(Volume / 100)

            _stoppedManually = False
            OutputDevice.Play()

        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in Play for" & FileName, ex)
        End Try
    End Sub
    Public Sub Play(Position As GTA.Vector3, Optional MaxDistance As Single = 150.0, Optional PlayInLoop As Boolean = False,
                    Optional StartTime As Integer = 0, Optional RealTimeVolumeControl As Boolean = True)


        Try
            If Reader Is Nothing Then Exit Sub
            Me.MaxDistance = MaxDistance
            Position3D = Position
            [Loop] = PlayInLoop
            Me.RealTimeVolumeControl = RealTimeVolumeControl

            If PlayInLoop Then
                EnableLoopTick()
            Else
                DisableLoopTick()
            End If

            If RealTimeVolumeControl Then
                EnableRTVCTick()
            Else
                DisableRTVCTick()
            End If

            If OutputDevice Is Nothing Then
                OutputDevice = New DirectSoundOut()
                OutputDevice.Init(WaveChannel)
            End If

            If StartTime >= 0 Then
                Reader.CurrentTime = TimeSpan.FromMilliseconds(StartTime)
            Else
                Reader.CurrentTime = TimeSpan.Zero
            End If

            _stoppedManually = False
            OutputDevice.Play()

        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in Play for" & FileName, ex)
        End Try

    End Sub
    ''' <summary>
    ''' Plays the sound attached to a ped. The sound will follow the ped and update
    ''' its position, distance attenuation and stereo panning automatically.
    ''' </summary>
    Public Sub Play(Ped As Ped, Optional MaxDistance As Single = 150.0F, Optional PlayInLoop As Boolean = False,
                            Optional StartTime As Integer = 0, Optional RealTimeVolumeControl As Boolean = True)

        If Ped Is Nothing OrElse Not Ped.Exists Then Exit Sub

        _attachedPed = Ped
        Me.Position3D = Ped.Position

        EnableAttachedTick()

        Play(Me.Position3D, MaxDistance, RealTimeVolumeControl, StartTime, PlayInLoop)
    End Sub
    ''' <summary>
    ''' Plays the sound attached to a vehicle. The sound follows the vehicle position
    ''' and updates volume and direction automatically based on camera position.
    ''' </summary>
    Public Sub Play(Vehicle As Vehicle, Optional MaxDistance As Single = 150.0F, Optional PlayInLoop As Boolean = False,
                            Optional StartTime As Integer = 0, Optional RealTimeVolumeControl As Boolean = True)

        If Vehicle Is Nothing OrElse Not Vehicle.Exists Then Exit Sub

        _attachedVeh = Vehicle
        Me.Position3D = Vehicle.Position

        EnableAttachedTick()

        Play(Me.Position3D, MaxDistance, RealTimeVolumeControl, StartTime, PlayInLoop)
    End Sub
    ''' <summary>
    ''' Plays the sound attached to an object/prop. The sound will automatically
    ''' follow the object's world position.
    ''' </summary>
    Public Sub Play(Obj As [Object], Optional MaxDistance As Single = 150.0F, Optional PlayInLoop As Boolean = False,
                            Optional StartTime As Integer = 0, Optional RealTimeVolumeControl As Boolean = True)

        If Obj Is Nothing OrElse Not Obj.Exists Then Exit Sub

        _attachedObj = Obj
        Me.Position3D = Obj.Position

        EnableAttachedTick()

        Play(Me.Position3D, MaxDistance, RealTimeVolumeControl, StartTime, PlayInLoop)
    End Sub

    '''' <param name="StartTime">Start time in milliseconds</param>
    'Public Sub PlayLoopNow(Optional StartTime As Integer = 0)
    '    If StartTime >= 0 Then
    '        Reader.CurrentTime = TimeSpan.FromMilliseconds(StartTime)
    '    Else
    '        Reader.Position = 0
    '    End If
    '    [Loop] = True
    '    _stoppedManually = False
    '    Play()
    'End Sub
    '''' <param name="FadeOut">Fade out duration in milliseconds</param>
    Public Sub [Stop]() '(Optional FadeOut As Integer = 0)
        Try
            If Reader Is Nothing Then Exit Sub
            Reader.Position = 0
            _stoppedManually = True
            OutputDevice?.Stop()
            ' Sound.OutputDevice?.Dispose()
            ' Sound.Reader?.Dispose()
            ' Sound.WaveChannel?.Dispose()

            '  Sound.OutputDevice = Nothing
            '  Sound.Reader = Nothing
            ' Sound.WaveChannel = Nothing

            'Sound.VolumeTimer?.Dispose()
            '  Sound.FadeTimer?.Dispose()

        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in Stop for " & FileName, ex)
        End Try
    End Sub
    Public Sub Trigger(Optional Volume As Integer = 100, Optional StartTime As Integer = 0)
        Try
            If Reader Is Nothing Then
                Exit Sub
            End If

            Dim Start As Integer
            If StartTime < 0 Then Start = 0
            Reader.CurrentTime = TimeSpan.FromMilliseconds(Start)

            If OutputDevice Is Nothing Then
                OutputDevice = New DirectSoundOut()
                OutputDevice.Init(WaveChannel)
            End If
            Position3D = Vector3.Zero
            Reader.Position = 0
            [Loop] = False
            WaveChannel.Volume = CSng(Volume / 100)
            OutputDevice.Play()
        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in Trigger for " & FileName, ex)
        End Try
    End Sub
    Public Sub Trigger(SoundPosition As GTA.Vector3, Optional MaxDistance As Single = 150.0, Optional StartTime As Integer = 0)
        Try
            If Reader Is Nothing Then
                Exit Sub
            End If

            Position3D = SoundPosition

            RealTimeVolumeControl = False
            Me.MaxDistance = MaxDistance
            '------------------------------------------------

            If OutputDevice Is Nothing Then
                OutputDevice = New DirectSoundOut()
                OutputDevice.Init(WaveChannel)
            End If

            Dim Start As Integer
            If StartTime < 0 Then Start = 0
            Reader.CurrentTime = TimeSpan.FromMilliseconds(Start)

            OutputDevice.Play()
            SetVolumeAndDirection_OnTick(Me, Position3D, 1, MaxDistance, True)
        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in Trigger for" & FileName, ex)
        End Try
    End Sub
    'Public Shared Sub StopAllSounds(Optional FadeOut As Integer = 0, Optional DisposeAfterStop As Boolean = False, Optional StopOnlyLoops As Boolean = False, Optional StopOnlyNonLoops As Boolean = False)
    '    Try
    '        If NAudioAdvanced.SoundsList Is Nothing OrElse NAudioAdvanced.SoundsList.Count = 0 Then Exit Sub

    '        For Each snd As NAudioAdvanced In NAudioAdvanced.SoundsList.ToList()

    '            If snd Is Nothing Then Continue For

    '            If StopOnlyLoops AndAlso Not snd.Loop Then Continue For
    '            If StopOnlyNonLoops AndAlso snd.Loop Then Continue For

    '            If snd.OutputDevice Is Nothing Then Continue For

    '            If FadeOut >= 0 Then
    '                Try
    '                    Dim initialVol As Single = snd.Volume
    '                    Dim steps As Integer = 20
    '                    For i As Integer = 0 To steps
    '                        snd.Volume = initialVol * (1 - (i / steps))
    '                        Threading.Thread.Sleep(FadeOut \ steps)
    '                    Next
    '                Catch ex As Exception
    '                    WriteLogAdvanced("Error in StopAllSounds: cannot fade out.", ex)
    '                End Try
    '            End If

    '            Try
    '                snd.OutputDevice.Stop()
    '                snd.Loop = False
    '            Catch ex As Exception
    '                WriteLogAdvanced("Error in StopAllSounds: cannot stop sound.", ex)
    '            End Try

    '            If DisposeAfterStop Then
    '                Try
    '                    snd.OutputDevice.Dispose()
    '                    snd.Reader.Dispose()
    '                Catch ex As Exception
    '                    WriteLogAdvanced("Error in StopAllSounds: cannot dispose sounds.", ex)
    '                End Try
    '            End If

    '        Next

    '    Catch ex As Exception
    '        WriteLogAdvanced("Error in StopAllSounds", ex)
    '    End Try
    'End Sub
    '''' <summary>
    '''' The function to update all sounds' loop, volume and let/right pan. Must be called in a Tick (cycle)
    '''' or added to the TickHelper.
    '''' </summary>
    'Public Shared Sub UpdateAllSounds_OnTick()
    '    Try
    '        For Each Sound As NAudioAdvanced In SoundsList.ToList()
    '            If Sound Is Nothing Then Continue For
    '            If Sound.OutputDevice Is Nothing OrElse Sound.Reader Is Nothing Then Continue For

    '            ' Controlla e aggiorna il loop
    '            Sound.CheckLoop()

    '            ' Controlla il volume in tempo reale solo se abilitato e se il suono sta suonando
    '            If Sound.RealTimeVolumeControl AndAlso Sound.isPlaying() Then
    '                SetVolumeAndDirection_OnTick(Sound, Sound.Position3D)
    '            End If
    '        Next
    '    Catch ex As Exception
    '        WriteLog(ex, "Error in NAudioAdvanced.UpdateAllSounds_OnTick")
    '    End Try
    'End Sub
    Private Shared Sub SetVolumeAndDirection_OnTick(Sound As NAudioAdvanced, SoundPosition As Vector3, Optional Coefficient As Single = 1.0F, Optional MaxDistance As Single = 150.0F, Optional EnableStereoPanning As Boolean = True)
        Try
            If Sound Is Nothing OrElse Game.CurrentCamera Is Nothing Then Exit Sub
            If Sound.OutputDevice Is Nothing OrElse Sound.Reader Is Nothing Then Exit Sub

            Sound.CheckLoop()

            Dim camPos As Vector3 = Game.CurrentCamera.Position
            Dim camDir As Vector3 = Game.CurrentCamera.Direction
            Dim dirToSound As Vector3 = SoundPosition - camPos
            Dim distance As Single = dirToSound.Length()

            If distance >= MaxDistance Then
                Sound.Volume = 0
                Exit Sub
            End If

            Dim attenuation As Single = Coefficient / (1.0F + (distance / (MaxDistance / 5.0F)) ^ 2.0F)
            attenuation = Math.Max(0.0F, Math.Min(1.0F, attenuation))
            Sound.Volume = attenuation * 100.0F

            '=== Panning L/R  ===
            If EnableStereoPanning Then
                dirToSound.Normalize()
                camDir.Normalize()

                Dim cross As Vector3 = Vector3.Cross(camDir, dirToSound)
                Dim dot As Single = Vector3.Dot(camDir, dirToSound)
                Dim angle As Single = Math.Atan2(cross.Z, dot) ' radiante [-π, π]

                Dim rawPan As Single = CSng(Math.Sin(angle))
                Dim smoothPan As Single = CSng(Math.Tanh(rawPan * 1.2F)) ' 1.2F regola la sensibilità

                Dim proximityFactor As Single = Math.Min(1.0F, distance / (MaxDistance / 4.0F))
                smoothPan *= proximityFactor

                smoothPan = Math.Max(-1.0F, Math.Min(1.0F, smoothPan))

                Static lastPan As Single = 0.0F
                Dim lerpSpeed As Single = 0.1F
                Dim finalPan As Single = lastPan + (smoothPan - lastPan) * lerpSpeed
                lastPan = finalPan

                If Sound.WaveChannel IsNot Nothing Then
                    Sound.WaveChannel.Pan = finalPan
                End If
            End If

        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in SetVolumeAndDirection_OnTick for " & Sound.FileName, ex)
        End Try
    End Sub
    Public Enum State
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
    '==================================================================================================
    ' ---- TICKs ----
    '------------------
    'Loop
    Private Sub EnableLoopTick()
        If _loopHandler Is Nothing Then
            _loopHandler = Sub() Me.CheckLoop()
            TickHelper.Add_Internal(_loopHandler)
        End If
    End Sub
    Private Sub DisableLoopTick()
        If _loopHandler IsNot Nothing Then
            TickHelper.Remove_Internal(_loopHandler)
            _loopHandler = Nothing
        End If
    End Sub
    Private Sub CheckLoop()
        Try
            If Not [Loop] Then Return
            If Reader Is Nothing OrElse WaveChannel Is Nothing Then Return

            If OutputDevice Is Nothing Then
                Try
                    OutputDevice = New DirectSoundOut()
                    OutputDevice.Init(WaveChannel)
                Catch ex As Exception
                    Tools.WriteLogAdvanced("Error in CheckLoop for " & FileName & ": cannot init OutputDevice.", ex)
                    Return
                End Try
            End If

            Dim atEnd As Boolean = False
            Try
                If Reader.CurrentTime >= Reader.TotalTime Then
                    atEnd = True
                ElseIf Reader.Position >= Reader.Length Then
                    atEnd = True
                End If
            Catch
                atEnd = False
            End Try

            Dim stoppedState As Integer = PlaybackState.Stopped
            If atEnd OrElse stoppedState = 4 Then
                Try
                    Reader.Position = 0
                Catch ex As Exception
                    Tools.WriteLogAdvanced("Error in CheckLoop for " & FileName & ": cannot reset Reader.Position", ex)
                    Return
                End Try
                Try
                    OutputDevice.Play()
                Catch ex As Exception
                    Try
                        OutputDevice.Stop()
                    Catch
                    End Try
                    Try
                        OutputDevice.Dispose()
                    Catch
                    End Try
                    OutputDevice = Nothing

                    Try
                        OutputDevice = New DirectSoundOut()
                        OutputDevice.Init(WaveChannel)
                        OutputDevice.Play()
                    Catch ex2 As Exception
                        Tools.WriteLogAdvanced("Error in CheckLoop for " & FileName & ": cannot restart OutputDevice after End.", ex2)
                        Return
                    End Try
                End Try
            End If
        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in CheckLoop for " & FileName & ": error during check loop.", ex)
        End Try
    End Sub
    '------------------
    'Real Time Volume Control
    Private Sub EnableRTVCTick()
        If _rtvcHandler Is Nothing Then
            _rtvcHandler = Sub()
                               SetVolumeAndDirection_OnTick(Me, Me.Position3D)
                           End Sub

            TickHelper.Add_Internal(_rtvcHandler)
        End If
    End Sub
    Private Sub DisableRTVCTick()
        If _rtvcHandler IsNot Nothing Then
            TickHelper.Remove_Internal(_rtvcHandler)
            _rtvcHandler = Nothing
        End If
    End Sub
    Private Sub SetVolumeAndDirection(SoundPosition As Vector3, Optional MaxDistance As Single = 150.0F, Optional Coefficient As Single = 1.0F, Optional EnableStereoPanning As Boolean = True)
        Try
            If Me Is Nothing OrElse Game.CurrentCamera Is Nothing Then Exit Sub
            If OutputDevice Is Nothing OrElse Reader Is Nothing Then Exit Sub

            CheckLoop()

            Dim camPos As Vector3 = Game.CurrentCamera.Position
            Dim camDir As Vector3 = Game.CurrentCamera.Direction
            Dim dirToSound As Vector3 = SoundPosition - camPos
            Dim distance As Single = dirToSound.Length()

            If distance >= MaxDistance Then
                Volume = 0
                Exit Sub
            End If

            Dim attenuation As Single = Coefficient / (1.0F + (distance / (MaxDistance / 5.0F)) ^ 2.0F)
            attenuation = Math.Max(0.0F, Math.Min(1.0F, attenuation))
            Volume = attenuation * 100.0F

            '=== Panning L/R  ===
            If EnableStereoPanning Then
                dirToSound.Normalize()
                camDir.Normalize()

                Dim cross As Vector3 = Vector3.Cross(camDir, dirToSound)
                Dim dot As Single = Vector3.Dot(camDir, dirToSound)
                Dim angle As Single = Math.Atan2(cross.Z, dot)

                Dim rawPan As Single = CSng(Math.Sin(angle))
                Dim smoothPan As Single = CSng(Math.Tanh(rawPan * 1.2F))

                Dim proximityFactor As Single = Math.Min(1.0F, distance / (MaxDistance / 4.0F))
                smoothPan *= proximityFactor

                smoothPan = Math.Max(-1.0F, Math.Min(1.0F, smoothPan))

                Static lastPan As Single = 0.0F
                Dim lerpSpeed As Single = 0.1F
                Dim finalPan As Single = lastPan + (smoothPan - lastPan) * lerpSpeed
                lastPan = finalPan

                If WaveChannel IsNot Nothing Then
                    WaveChannel.Pan = finalPan
                End If
            End If

        Catch ex As Exception
            Tools.WriteLogAdvanced("Error in SetVolumeAndDirection for " & FileName, ex)
        End Try
    End Sub
    '------------------
    'Attached Entity
    Private Sub EnableAttachedTick()
        If _attachedHandler Is Nothing Then
            _attachedHandler = Sub()
                                   'Ped
                                   If _attachedPed IsNot Nothing AndAlso _attachedPed.Exists Then
                                       Position3D = _attachedPed.Position
                                   End If
                                   'Obj
                                   If _attachedObj IsNot Nothing AndAlso _attachedObj.Exists Then
                                       Position3D = _attachedObj.Position
                                   End If
                                   'Vehicles
                                   If _attachedVeh IsNot Nothing AndAlso _attachedVeh.Exists Then
                                       Position3D = _attachedVeh.Position
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
    '------------------
End Class
