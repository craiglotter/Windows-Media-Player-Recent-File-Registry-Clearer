Imports System.IO
Imports Microsoft.Win32

Public Class Worker

    Inherits System.ComponentModel.Component

    ' Declares the variables you will use to hold your thread objects.

    Public WorkerThread As System.Threading.Thread


   

    
    Public result As String = ""

    Public Event WorkerComplete(ByVal Result As String)
    Public Event WorkerProgress(ByVal filesrenamed As Long, ByVal currentfilename As String, ByVal totalfiles As Long)



#Region " Component Designer generated code "

    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

#End Region

    Private Sub Error_Handler(ByVal exc As Exception)
        Try
            If exc.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message("The Worker Thread encountered the following problem: " & vbCrLf & exc.ToString)
                Display_Message1.ShowDialog()
                Dim dir As DirectoryInfo = New DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                Dim filewriter As StreamWriter = New StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & exc.ToString)
                filewriter.Flush()
                filewriter.Close()
            End If
        Catch ex As Exception
            MsgBox("An error occurred in Windows Media Player Recent File Registry Clearer's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

   

    Public Sub ChooseThreads(ByVal threadNumber As Integer)
        Try
            ' Determines which thread to start based on the value it receives.
            Select Case threadNumber
                Case 1
                    ' Sets the thread using the AddressOf the subroutine where
                    ' the thread will start.
                    WorkerThread = New System.Threading.Thread(AddressOf WorkerExecute)
                    ' Starts the thread.
                    WorkerThread.Start()

            End Select
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerExecute()
        Try
            result = "Success"
            Dim oReg As RegistryKey
            Dim regkey As RegistryKey
            Try
                oReg = Registry.CurrentUser



                regkey = oReg
                oReg.Close()
                Dim subs() As String = ("Software\Microsoft\MediaPlayer\Player\RecentFileList").Split("\")
                Dim continue = True
                For Each stri As String In subs
                    If continue = False Then
                        Exit For
                    End If
                    If regkey Is Nothing = False Then
                        Dim skn As String() = regkey.GetSubKeyNames()
                        Dim strin As String

                        continue = False
                        For Each strin In skn
                            If stri = strin Then
                                regkey = regkey.OpenSubKey(stri, True)
                                continue = True
                                Exit For
                            End If
                        Next

                    End If

                Next
                If continue = True Then
                    If regkey Is Nothing = False Then
                        Dim str As String() = regkey.GetValueNames()
                        Dim val As String
                        For Each val In str
                            regkey.DeleteValue(val)
                        Next

                        regkey.Close()
                    End If
                Else
                    Dim Display_Message1 As New Display_Message("""Software\Microsoft\MediaPlayer\Player\RecentFileList"" can not be found in the registry. No action has been taken.")
                    Display_Message1.ShowDialog()
                End If

            Catch ex As Exception
                Error_Handler(ex)
                result = "Failure"

            End Try

            Try
                oReg = Registry.CurrentUser
                regkey = oReg
                oReg.Close()
                Dim subs() As String = ("Software\Microsoft\MediaPlayer\AutoComplete\MediaEdit").Split("\")
                Dim continue = True
                For Each stri As String In subs
                    If continue = False Then
                        Exit For
                    End If
                    If regkey Is Nothing = False Then
                        Dim skn As String() = regkey.GetSubKeyNames()
                        Dim strin As String

                        continue = False
                        For Each strin In skn
                            If stri = strin Then
                                regkey = regkey.OpenSubKey(stri, True)
                                continue = True
                                Exit For
                            End If
                        Next

                    End If

                Next
                If continue = True Then
                    If regkey Is Nothing = False Then
                        Dim str As String() = regkey.GetValueNames()
                        Dim val As String
                        For Each val In str
                            regkey.DeleteValue(val)
                        Next

                        regkey.Close()
                    End If
                Else
                    Dim Display_Message1 As New Display_Message("""Software\Microsoft\MediaPlayer\AutoComplete\MediaEdit"" can not be found in the registry. No action has been taken.")
                    Display_Message1.ShowDialog()
                End If

            Catch ex As Exception
                Error_Handler(ex)
                result = "Failure"

            End Try

            RaiseEvent WorkerComplete(result)
        Catch ex As Exception
            result = "Failure"
            RaiseEvent WorkerComplete(result)
        End Try

        WorkerThread.Abort()
    End Sub







End Class
