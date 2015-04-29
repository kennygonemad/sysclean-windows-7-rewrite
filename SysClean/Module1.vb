Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports System.Xml

Module Module1
    Dim GS_ShowRemovedInLog As Boolean = False
    Dim GS_Greyscale As Boolean = False
    Dim GS_LogDir As String = "C:\Program Files\SysClean\SysCleanLog_" & My.Computer.Name & ".log"
    Dim GS_SpaceShow As Boolean = True

    Dim GS_SkipList As New ArrayList

    Dim GS_CustomDeleteFolderList As New ArrayList
    Dim GS_CustomDeleteFileList As New ArrayList

    Dim LS_RemoveOK As ConsoleColor = ConsoleColor.Green
    Dim LS_RemoveFail As ConsoleColor = ConsoleColor.Red
    Dim LS_Forecolor As ConsoleColor = ConsoleColor.Cyan
    Dim LS_BackColor As ConsoleColor = ConsoleColor.DarkCyan
    Dim LS_SkipColor As ConsoleColor = ConsoleColor.Yellow

    Dim LS_QUITONCOMPLETE As Boolean = True

    Dim TS_CBatchSizeSave As ULong = 0
    Dim TS_DBatchSizeSave As ULong = 0

    Dim OSVersion As Integer = 0


    ReadOnly CV_APPLICATIONLOGHEADER As String = CV_APPLICATIONLOGHEADER
    ReadOnly CV_CONSOLEHEADER As String = "                                  System Cleaner" & Environment.NewLine & _
                                          "                                Version: " & My.Application.Info.Version.ToString & Environment.NewLine


    Private Function SetupColors(Optional ByVal forceGreyscale As Boolean = False) As String
        'Setup Colors
        If forceGreyscale Then
            LS_BackColor = ConsoleColor.Black
            LS_Forecolor = ConsoleColor.Gray
            LS_RemoveFail = ConsoleColor.DarkGray
            LS_RemoveOK = ConsoleColor.Gray
            LS_SkipColor = ConsoleColor.White
        Else
            If GS_Greyscale Then
                LS_BackColor = ConsoleColor.Black
                LS_Forecolor = ConsoleColor.Gray
                LS_RemoveFail = ConsoleColor.DarkGray
                LS_RemoveOK = ConsoleColor.Gray
                LS_SkipColor = ConsoleColor.White
            Else
                LS_BackColor = ConsoleColor.DarkCyan
                LS_Forecolor = ConsoleColor.Cyan
                LS_RemoveFail = ConsoleColor.Red
                LS_RemoveOK = ConsoleColor.Green
                LS_SkipColor = ConsoleColor.Yellow
            End If
        End If

        Console.BackgroundColor = LS_BackColor
        Console.ForegroundColor = LS_Forecolor

        Console.Clear()
        Console.WriteLine(CV_CONSOLEHEADER)

        Return Nothing
    End Function

    Private Sub SetupLog()
        Try
            My.Computer.FileSystem.CreateDirectory(GS_LogDir.Substring(0, GS_LogDir.Length - IO.Path.GetFileName(GS_LogDir).Length))
        Catch ex As Exception
        End Try

        'Delete the Previous File if it exists
        If My.Computer.FileSystem.FileExists(GS_LogDir.Substring(0, GS_LogDir.Length - IO.Path.GetFileName(GS_LogDir).Length) & "SysCleanLog_" & My.Computer.Name & "-PREVIOUS.log") Then
            My.Computer.FileSystem.DeleteFile(GS_LogDir.Substring(0, GS_LogDir.Length - IO.Path.GetFileName(GS_LogDir).Length) & "SysCleanLog_" & My.Computer.Name & "-PREVIOUS.log")
        End If

        'Rename the current log (add -PREVIOUS to the end of the filename)
        If My.Computer.FileSystem.FileExists(GS_LogDir) Then
            My.Computer.FileSystem.RenameFile(GS_LogDir, "SysCleanLog_" & My.Computer.Name & "-PREVIOUS.log")
        End If
    End Sub

    Sub Main()
        If (Environment.OSVersion.Version.Major = 6) Then
            OSVersion = 1
        End If

        ' Setup a trap to catch any gremlins so the end user doesnt see them
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledExceptionTrapper

        ' Load Skip List
        GetSkipList()

        ' Set Console Title
        Console.Title = "System Cleaner V" & My.Application.Info.Version.ToString & " - Written By Nathan Payne"

        ' CoLoUrS
        SetupColors()

        ' Viewer friendly list of commands
        Dim CommandLines As String = String.Empty

        If My.Application.CommandLineArgs.Count > 0 Then
            ' Set the custom log path if it is specified
            Console.Write(My.Application.CommandLineArgs.Count)
            If My.Application.CommandLineArgs.Item(My.Application.CommandLineArgs.Count - 1).ToLower.StartsWith("/log:") Then
                GS_LogDir = My.Application.CommandLineArgs.Item(My.Application.CommandLineArgs.Count - 1).ToLower.Substring(5)
                GS_LogDir = GS_LogDir.TrimEnd(ControlChars.Quote)

                'trim \ off file path if it exists
                If GS_LogDir.EndsWith("\") Then
                    GS_LogDir = GS_LogDir.Substring(0, GS_LogDir.Length - 1)
                End If
                GS_LogDir += "\SysCleanLog_" & My.Computer.Name & ".log"
            End If

            ' Setup the log ready to use
            SetupLog()

            ' Write the log header
            My.Computer.FileSystem.WriteAllText(GS_LogDir, CV_APPLICATIONLOGHEADER, False)
            CommandLines = "Command Line Arguments: "
            For Each Ci As String In My.Application.CommandLineArgs
                CommandLines += Ci & " "
            Next
            My.Computer.FileSystem.WriteAllText(GS_LogDir, "Command Line Arguments Detected, Starting in silent mode." & Environment.NewLine & CommandLines & Environment.NewLine, True)

        Else

            ' Setup the log ready to use
            SetupLog()

            ' Write the log header
            My.Computer.FileSystem.WriteAllText(GS_LogDir, CV_APPLICATIONLOGHEADER, False)

        End If

        For i As Integer = 0 To My.Application.CommandLineArgs.Count - 1
            Select Case My.Application.CommandLineArgs.Item(i).ToLower
                Case "/fc"
                    'Do A full System Clean
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    AUC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    RB_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    NCC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    CTC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    CDC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    FTPC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    IETFAU_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    IECAU_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    ReturnSpaceRecoveredTotal()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    SpaceStat_Routine()

                Case "/fcnoncc"
                    'Do A full System Clean
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    AUC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    CTC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    RB_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    CDC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    FTPC_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    IETFAU_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    IECAU_Routine()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    ReturnSpaceRecoveredTotal()
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    SpaceStat_Routine()

                Case "/uc", "/userclean"
                    'Clean the current users temp
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    UC_Routine()

                Case "/auc", "/alluserclean"
                    'Clean the current users temp
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    AUC_Routine()

                Case "/rb"
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    RB_Routine()

                Case "/ncc"
                    'Clear the reg entry
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    NCC_Routine()

                Case "/autoreset"
                    'Clear the reg entry
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, "Auto mode has been reset. Next run will be an all user clean" & Environment.NewLine, True)
                    My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\Software\SysCleaner\", "AutoMode", False)

                Case "/auto"
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    Auto_Routine()

                Case "/spacestat"
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    SpaceStat_Routine()

                Case "/iecau"
                    'Clean the current users temp
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    IECAU_Routine()

                Case "/ieccu"
                    'Clean the current users temp
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    IECCU_Routine()

                Case "/custom"
                    'Clean the current users temp
                    My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                    CUSTOM_Routine()

                Case "/forcegreyscale"
                    ' Force the greyscale colour scheme
                    GS_Greyscale = True

                Case "/logremoveditems"
                    ' Force the greyscale colour scheme
                    GS_ShowRemovedInLog = True

                Case "/noquit"
                    LS_QUITONCOMPLETE = False

                Case Else
                    If My.Application.CommandLineArgs.Item(0).ToLower.StartsWith("/log:") Then
                        ' Don't log as an invalid command as it is handled before anything else
                    Else
                        ' Log that the specific command is not a valid one
                        My.Computer.FileSystem.WriteAllText(GS_LogDir, "The command '" & My.Application.CommandLineArgs.Item(i) & "' is an invalid. Please run the help command to see a list of the valid commands" & Environment.NewLine, True)
                        Console.WriteLine("The command '" & My.Application.CommandLineArgs.Item(i) & "' is an invalid. Please run the help command to see a list of the valid commands")
                    End If


            End Select

            If i = My.Application.CommandLineArgs.Count - 1 Then
                If LS_QUITONCOMPLETE = True Then
                    GoTo Quit
                End If
            End If

        Next

        SetupColors()

        If CommandLines IsNot String.Empty Then
            Console.WriteLine(CommandLines)
        End If

WaitForInput:
        Console.Write("SysClean> ")
        Dim Input As String = String.Empty
        Input = Console.ReadLine
        Select Case Input.ToLower
            Case "help"
                Console.WriteLine(" --- Cleaning Commands ---")
                Console.WriteLine(" fc       - Full System Clean - This command carries out all other commands")
                Console.WriteLine(" fcnoncc  - Full System Clean - Same as fc except ncc is not run")
                Console.WriteLine(" uc       - Cleans the current users 'D:\USERDATA\%username%\TEMP' folder and")
                Console.WriteLine("             the C:\Documents and Settings\%username%\Local Settings\Temp")
                Console.WriteLine(" auc      - Same as 'uc' only for all user folders on the computer")
                Console.WriteLine(" rb       - Empties the recycler folder on C:\ and D:\")
                Console.WriteLine(" ncc      - NAL Cache Clean - Cleans all the files from the nal cache")
                Console.WriteLine(" ctc      - Clean C Drive Temps - C:\Windows\Temp")
                Console.WriteLine(" ftpc     - Cleans the FTP Dir in D:\")
                Console.WriteLine(" cdc      - Cleans all users CD burning staging folder")
                Console.WriteLine(" ietfau   - Cleans all users temporary internet files")
                Console.WriteLine(" ietfcu   - Cleans current users temporary internet files")
                Console.WriteLine(" ieccu    - Cleans current users ie cookies folder")
                Console.WriteLine(" iecau    - Cleans all users ie cookies files")
                Console.WriteLine(" custom   - Deletes all files and folders specified in Sysclean.CustomDeleteList")
                Console.WriteLine(" auto     - Either runs an all user clean or a user clean depending on whether ")
                Console.WriteLine("             the command has been run before")
                Console.WriteLine(" autoreset- Resets the auto clean so that a all user clean is run next time")
                Console.WriteLine(" spacestat- Shows the space free on C and D drives")
                Console.WriteLine("")
                Console.WriteLine(" Hint:      FC, FCNONCC, UC, AUC, AUTO, AUTORESET, RB, NCC, SPACESTAT can be ")
                Console.WriteLine("             passed as command line arguments to automate")
                Console.WriteLine("             system cleanups just add a '/'")
                Console.WriteLine("            Don't want SysCleaner to exit after running some commands on startup")
                Console.WriteLine("             just add /noquit to you parameter list")
                Console.WriteLine("")
                Console.WriteLine(" --- System Commands ---")
                Console.WriteLine(" cls      - Clear the screen")
                Console.WriteLine(" exit     - does just that")
                Console.WriteLine("")
            Case "fc" ' Full clean... runs all other commands
                TS_CBatchSizeSave = 0
                TS_DBatchSizeSave = 0
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                AUC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                RB_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                NCC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                CTC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                CDC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                FTPC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IETFAU_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IECAU_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                ReturnSpaceRecoveredTotal()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                Console.WriteLine()
                SpaceStat_Routine()
                Console.WriteLine("See the log located at '" & GS_LogDir & "'")
            Case "fcnoncc" ' Full clean... runs all other commands except ncc
                TS_CBatchSizeSave = 0
                TS_DBatchSizeSave = 0
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                AUC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                RB_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                CTC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                CDC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                FTPC_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IETFAU_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IECAU_Routine()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                ReturnSpaceRecoveredTotal()
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                Console.WriteLine()
                SpaceStat_Routine()
                Console.WriteLine("See the log located at '" & GS_LogDir & "'")
            Case "uc" ' Current User Temp Clean
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                UC_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "auc" 'All User Clean
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                AUC_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "ncc" 'Nal Cache Clean
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                NCC_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "ctc" 'Clean C Drive Temps
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                CTC_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "ftpc" 'Clean FTP Folder
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                FTPC_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "cdc" 'Clean CD Stage
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                CDC_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "ietfau" 'Clean All Users Temporary Internet Cache
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IETFAU_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "ietfcu" 'Clean All Users Temporary Internet Cache
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IETFCU_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "iecau" 'Clean All Users IE Cookies Folder
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IECAU_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "ieccu" 'Clean Current Users Cookies Folder
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                IECCU_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "custom" 'Delete all files and folders listed in the custom delete list
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                CUSTOM_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "auto"  'Clean All or just current user depending on registry entry
                My.Computer.FileSystem.WriteAllText(GS_LogDir, CV_APPLICATIONLOGHEADER, False)
                Auto_Routine()
            Case "rb"
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                RB_Routine()
                Console.WriteLine(Environment.NewLine & "See the log located at '" & GS_LogDir & "'")
            Case "viewlog" 'View Current Log
                ViewLog_Routine()
            Case "viewpreviouslog" 'View Previous Log
                ViewPreviousLog_Routine()
            Case "spacestat" 'Return the Drive Free Used Spaces
                SpaceStat_Routine()
            Case "autoreset"
                'Clear the reg entry
                My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "Auto mode has been reset. Next run will be an all user clean" & Environment.NewLine, True)
                My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\Software\SysCleaner\", "AutoMode", False)
                Console.WriteLine(Environment.NewLine & "The auto command has been reset. Next run will be an all user clean.")
            Case "cls" 'Clear Screen
                TS_CBatchSizeSave = 0
                TS_DBatchSizeSave = 0
                Console.Clear()
                Console.WriteLine(CV_CONSOLEHEADER)
            Case "exit" 'Quit Program
                GoTo Quit
            Case "settings"
                Settings_Routine()
                Console.Clear()
                Console.WriteLine(CV_CONSOLEHEADER)
                GoTo WaitForInput
            Case Else
                Console.WriteLine("'" & Input & "' is an unknown command type 'help' for a list of commands")
        End Select
        GoTo WaitForInput
Quit:
    End Sub

    Private Sub Settings_Routine()
SettingsPage:
        SetupColors()
        Console.BackgroundColor = LS_BackColor
        Console.ForegroundColor = LS_Forecolor
        Console.Clear()
        Console.WriteLine(CV_CONSOLEHEADER)
        Console.WriteLine("Application Settings:")
        Console.WriteLine(OSVersion)
        Console.WriteLine(" 1. Log Removed Items as well as failed items (Currently: " & GS_ShowRemovedInLog & ")")
        Console.WriteLine(" 2. Greyscale Mode (Currently: " & GS_Greyscale & ")")
        Console.WriteLine(" 3. Log Path (Currently: " & GS_LogDir & ")")
        Console.WriteLine(" 4. Show space recovered (Currently: " & GS_SpaceShow & ")")
        Console.WriteLine("")
        Console.WriteLine(" 9. Exit settings")
        Console.WriteLine("")
        Console.Write("    Enter the number you want to change > ")

        Try
            Dim Changer As Integer = CInt(Console.ReadLine)
            Select Case Changer
                Case 1
                    If GS_ShowRemovedInLog = True Then
                        GS_ShowRemovedInLog = False
                    Else
                        GS_ShowRemovedInLog = True
                    End If
                    GoTo SettingsPage
                Case 2
                    If GS_Greyscale = True Then
                        GS_Greyscale = False
                    Else
                        GS_Greyscale = True
                    End If
                    GoTo SettingsPage
                Case 3
SetPath:
                    Console.WriteLine("Enter the folder where the logfile is to be created or leave blank to cancel:")
                    Dim Path As String = Console.ReadLine
                    If Path = "" Then
                        GoTo SettingsPage
                    End If
                    Try
                        My.Computer.FileSystem.CreateDirectory(Path)
                        If My.Computer.FileSystem.DirectoryExists(Path) = False Then
                            Throw New DirectoryNotFoundException
                        End If

                        Try
                            If Path.Substring(Path.Length - 2, 1) = "\" Then
                            Else
                                Path += "\"
                            End If
                        Catch ex As Exception
                            Console.WriteLine("Invalid Path!!!")
                            GoTo SetPath
                        End Try


                        GS_LogDir = Path & "SysCleanLog_" & My.Computer.Name & ".log"

                        GoTo SettingsPage
                    Catch ex As Exception
                        Console.WriteLine("Invalid Path!!!")
                        GoTo SetPath
                    End Try
                Case 0
                    'Go back to main code
                Case Else
                    GoTo SettingsPage
            End Select
        Catch ex As Exception
            GoTo SettingsPage
        End Try

    End Sub

    Private Sub Auto_Routine()
        Dim AutoMode As Boolean = False
        Try
            AutoMode = CBool(My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\SysCleaner\", "AutoMode", False))
        Catch ex As Exception
            AutoMode = False
        End Try

        If AutoMode = True Then
            ' Do a quick clean
            TS_CBatchSizeSave = 0
            TS_DBatchSizeSave = 0

            My.Computer.FileSystem.WriteAllText(GS_LogDir, "AUTO Mode: Quick Clean" & Environment.NewLine, True)
            Console.WriteLine("AUTO Mode: Quick Clean")
            UC_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            FTPC_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            CTC_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            IETFCU_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            ReturnSpaceRecoveredTotal()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            SpaceStat_Routine()

        Else
            ' Do A full System Clean
            TS_CBatchSizeSave = 0
            TS_DBatchSizeSave = 0

            My.Computer.FileSystem.WriteAllText(GS_LogDir, "AUTO Mode: Full Clean" & Environment.NewLine, True)
            Console.WriteLine("AUTO Mode: Full Clean")
            AUC_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            CTC_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            CDC_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            FTPC_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            IETFAU_Routine()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            ReturnSpaceRecoveredTotal()
            My.Computer.FileSystem.WriteAllText(GS_LogDir, Environment.NewLine, True)
            SpaceStat_Routine()
            My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\Software\SysCleaner", "AutoMode", True)

        End If


    End Sub

    Private Sub UC_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Current User Cleaner: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Current User Cleaner -- ")
        If (OSVersion = 0) Then
            Dim Items As New ArrayList
            Items.Add("D:\USERDATA\" & Environment.UserName & "\Temp")
            Items.Add("C:\Documents and Settings\" & Environment.UserName & "\Local Settings\Temp")
            MultiFolderClear(Items)
        Else
            Dim Items As New ArrayList
            Items.Add("D:\USERS\" & Environment.UserName & "\AppData\Local\Temp")
            'Items.Add("C:\Documents and Settings\" & Environment.UserName & "\Local Settings\Temp")
            MultiFolderClear(Items)
        End If
    End Sub

    Private Sub AUC_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "All User Cleaner: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- All User Cleaner -- ")
        Dim FolderList As New ArrayList
        If (OSVersion = 0) Then
            Try
                For Each di In My.Computer.FileSystem.GetDirectories("D:\USERDATA", FileIO.SearchOption.SearchTopLevelOnly)
                    FolderList.Add(di & "\Temp")
                Next
            Catch ex As Exception
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
            End Try

            Try
                For Each di In My.Computer.FileSystem.GetDirectories("C:\Documents and Settings", FileIO.SearchOption.SearchTopLevelOnly)
                    FolderList.Add(di & "\Local Settings\Temp")
                Next
            Catch ex As Exception
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
            End Try
        Else
            Try
                For Each di In My.Computer.FileSystem.GetDirectories("D:\USERS", FileIO.SearchOption.SearchTopLevelOnly)
                    FolderList.Add(di & "\AppData\Local\Temp")
                Next
            Catch ex As Exception
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
            End Try
        End If

        MultiFolderClear(FolderList)

    End Sub

    Private Sub RB_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Recycle Bin Cleaner: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Recycle Bin Cleaner -- ")
        Dim Items As New ArrayList
        If (OSVersion = 0) Then
            Items.Add("C:\Recycler")
            Items.Add("D:\Recycler")
        Else
            Items.Add("c:\$Recycle.Bin")
        End If

        MultiFolderClear(Items)
    End Sub

    Private Sub IETFAU_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Clean All Users Temporary Internet Files: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Clean All Users Temporary Internet Files -- ")
        Dim FolderList As New ArrayList

        If (OSVersion = 0) Then

            Try
                For Each di In My.Computer.FileSystem.GetDirectories("C:\Documents and Settings", FileIO.SearchOption.SearchTopLevelOnly)
                    FolderList.Add(di & "\Local Settings\Temporary Internet Files")
                Next
            Catch ex As Exception
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
            End Try
        Else

            Try
                For Each di In My.Computer.FileSystem.GetDirectories("D:\users", FileIO.SearchOption.SearchTopLevelOnly)
                    FolderList.Add(di & "\AppData\Local\Microsoft\Windows\Temporary Internet Files")
                Next
            Catch ex As Exception
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
            End Try

        End If


        MultiFolderClear(FolderList)

    End Sub

    Private Sub IETFCU_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Clean Current Users Temporary Internet Files: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Clean Current Users Temporary Internet Files -- ")
        Dim Items As New ArrayList
        If (OSVersion = 0) Then
            Items.Add("C:\Documents and Settings\" & Environment.UserName & "\Local Settings\Temporary Internet Files")
        Else
            Items.Add("d:\users\" & Environment.UserName & "\AppData\Local\Microsoft\Windows\Temporary Internet Files")
        End If
        MultiFolderClear(Items)

    End Sub

    Private Sub IECAU_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Clean All Users Cookies Folders: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Clean All Users Cookies Folders -- ")
        Dim FolderList As New ArrayList
        If (OSVersion = 0) Then
            Try
                For Each di In My.Computer.FileSystem.GetDirectories("D:\USERDATA", FileIO.SearchOption.SearchTopLevelOnly)
                    FolderList.Add(di & "\Application Data\Microsoft\Internet Explorer\Cookies")
                Next
            Catch ex As Exception
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
            End Try
        Else
            Try
                For Each di In My.Computer.FileSystem.GetDirectories("D:\users", FileIO.SearchOption.SearchTopLevelOnly)
                    FolderList.Add(di & "\AppData\Roaming\Microsoft\Windows\Cookies")
                Next
            Catch ex As Exception
                My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
            End Try
        End If

        MultiFolderClear(FolderList)

    End Sub

    Private Sub IECCU_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Clean Current Users Cookies Folder: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Clean Current Users Cookies Folder -- ")
        Dim Items As New ArrayList
        If (OSVersion = 0) Then
            Items.Add("D:\USERDATA\" & Environment.UserName & "\Application Data\Microsoft\Internet Explorer\Cookies")
        Else
            Items.Add("D:\users\" & Environment.UserName & "\AppData\Roaming\Microsoft\Windows\Cookies")
        End If

        MultiFolderClear(Items)
    End Sub

    Private Sub NCC_Routine()
        If(OSVersion = 0) then
            My.Computer.FileSystem.WriteAllText(GS_LogDir, "NALCACHE Cleaner: " & Environment.NewLine, True)
            Console.WriteLine(Environment.NewLine & " -- NALCACHE Cleaner -- ")
            Dim Items As New ArrayList
            Items.Add("C:\NALCACHE\QLDHEALTH")
            MultiFolderClear(Items)
        End If

    End Sub

    Private Sub FTPC_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "FTP Dir Cleaner: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- FTP Dir Cleaner -- ")
        Dim Items As New ArrayList
        Items.Add("D:\FTP")
        MultiFolderClear(Items)
    End Sub

    Private Sub CDC_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "CD Burning Temp Dir Cleaner: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- CD Burning Temp Dir Cleaner -- ")
        Dim FolderList As New ArrayList

        Try
            For Each di In My.Computer.FileSystem.GetDirectories("C:\Documents and Settings", FileIO.SearchOption.SearchTopLevelOnly)
                FolderList.Add(di & "\Local Settings\Application Data\Microsoft\CD Burning")
            Next
        Catch ex As Exception
            My.Computer.FileSystem.WriteAllText(GS_LogDir, "ERROR:" & ex.Message & Environment.NewLine, True)
        End Try

        MultiFolderClear(FolderList)
    End Sub

    Private Sub CTC_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "System Temp Cleaner: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- System Temp Cleaner -- ")
        Dim Items As New ArrayList
        Items.Add("C:\WINDOWS\Temp")
        MultiFolderClear(Items)
    End Sub

    Private Sub CUSTOM_Routine()
        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Delete Items In Custom Delete List: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Delete Items In Custom Delete List -- ")

        GetCustomDeleteList()

        My.Computer.FileSystem.WriteAllText(GS_LogDir, " Removing Folders: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Removing Folders -- ")
        MultiFolderClear(GS_CustomDeleteFolderList)

        My.Computer.FileSystem.WriteAllText(GS_LogDir, " Removing Files: " & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Removing Files -- ")
        MultiFileClear(GS_CustomDeleteFileList)


    End Sub

    Private Sub ViewLog_Routine()
        Console.WriteLine(" -- Opening Log File -- ")
        Shell("C:\Program Files\Windows NT\Accessories\wordpad.exe " & ControlChars.Quote & GS_LogDir & ControlChars.Quote, AppWinStyle.NormalNoFocus, False)
    End Sub

    Private Sub ViewPreviousLog_Routine()
        Console.WriteLine(" -- Opening Previous Log File -- ")
        Dim PreviousLogPath As String = GS_LogDir.Substring(0, GS_LogDir.Length - 4) & "-PREVIOUS.log"

        Shell("C:\Program Files\Windows NT\Accessories\wordpad.exe " & ControlChars.Quote & PreviousLogPath & ControlChars.Quote, AppWinStyle.NormalNoFocus, False)
    End Sub

    Private Sub SpaceStat_Routine()
        Console.WriteLine(" -- System Space Statistics -- ")
        Console.WriteLine(" -- C:\ ('" & My.Computer.FileSystem.GetDriveInfo("C").VolumeLabel & "') -- ")
        Console.WriteLine(ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("C").AvailableFreeSpace)) &
                          " (" & CDec((My.Computer.FileSystem.GetDriveInfo("C").AvailableFreeSpace / My.Computer.FileSystem.GetDriveInfo("C").TotalSize) * 100).ToString("0.##") & "%) Free out of " & ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("C").TotalSize)))
        Console.WriteLine(Environment.NewLine & " -- D:\ ('" & My.Computer.FileSystem.GetDriveInfo("D").VolumeLabel & "') -- ")
        Console.WriteLine(ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("D").AvailableFreeSpace)) &
                          " (" & CDec((My.Computer.FileSystem.GetDriveInfo("D").AvailableFreeSpace / My.Computer.FileSystem.GetDriveInfo("D").TotalSize) * 100).ToString("0.##") & "%) Free out of " & ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("D").TotalSize)) & Environment.NewLine)

        My.Computer.FileSystem.WriteAllText(GS_LogDir, " -- System Space Statistics -- " & Environment.NewLine, True)
        My.Computer.FileSystem.WriteAllText(GS_LogDir, " -- C:\ ('" & My.Computer.FileSystem.GetDriveInfo("C").VolumeLabel & "') -- " & Environment.NewLine, True)
        My.Computer.FileSystem.WriteAllText(GS_LogDir, ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("C").AvailableFreeSpace)) &
                                            " (" & CDec((My.Computer.FileSystem.GetDriveInfo("C").AvailableFreeSpace / My.Computer.FileSystem.GetDriveInfo("C").TotalSize) * 100).ToString("0.##") & "%) Free out of " & ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("C").TotalSize)) & Environment.NewLine, True)
        My.Computer.FileSystem.WriteAllText(GS_LogDir, " -- D:\ ('" & My.Computer.FileSystem.GetDriveInfo("D").VolumeLabel & "') -- " & Environment.NewLine, True)
        My.Computer.FileSystem.WriteAllText(GS_LogDir, ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("D").AvailableFreeSpace)) &
                                            " (" & CDec((My.Computer.FileSystem.GetDriveInfo("D").AvailableFreeSpace / My.Computer.FileSystem.GetDriveInfo("D").TotalSize) * 100).ToString("0.##") & "%) Free out of " & ReturnParsedSize(CULng(My.Computer.FileSystem.GetDriveInfo("D").TotalSize)) & Environment.NewLine, True)

    End Sub

    Private Sub MultiFileClear(ByVal FilePaths As ArrayList)

        Dim TC As Long = 0
        Dim FC As Long = 0
        Dim SC As Long = 0
        Dim FL As New ArrayList
        Dim RS_C As ULong = 0
        Dim RS_D As ULong = 0

        For Each Fi As String In FilePaths

            Try

                ' Get File Size
                Dim TSize As ULong
                If GS_SpaceShow = True Then
                    TSize = CULng(My.Computer.FileSystem.GetFileInfo(Fi).Length)
                End If

                ' Check if the file fits a skip list item
                If CheckForSkipFlag(Fi) = False Then

                    ' If Not
                    ' Delete the File
                    My.Computer.FileSystem.DeleteFile(Fi)

                    ' Create a block
                    Console.ForegroundColor = LS_RemoveOK
                    Console.Write("▒")
                    Console.ForegroundColor = LS_Forecolor

                    ' If showing removed files log it
                    If GS_ShowRemovedInLog Then
                        My.Computer.FileSystem.WriteAllText(GS_LogDir, "Removed: " & Fi & Environment.NewLine, True)
                    End If

                    ' Add space to recovered space count
                    If Fi.StartsWith("C:\") Then
                        RS_C += TSize
                    Else
                        RS_D += TSize
                    End If
                Else
                    ' If it is on the list
                    ' Create a block
                    Console.ForegroundColor = LS_SkipColor
                    Console.Write("▒")
                    Console.ForegroundColor = LS_Forecolor

                    My.Computer.FileSystem.WriteAllText(GS_LogDir, "Skipped: " & Fi & Environment.NewLine, True)
                    SC += 1
                End If

            Catch ex As Exception
                Console.ForegroundColor = LS_RemoveFail
                Console.Write("▒")
                Console.ForegroundColor = LS_Forecolor

                My.Computer.FileSystem.WriteAllText(GS_LogDir, ex.Message & Environment.NewLine, True)
                FC += 1
                FL.Add(ex.Message)
            End Try

            TC += 1

        Next

        Console.WriteLine("")

        'Compile the files removed string
        Dim crStr As String = String.Empty

        If FC > 0 Then
            crStr = FC & " item(s) failed to delete. "
        End If

        If SC > 0 Then
            crStr += SC & " item(s) were skipped."
        End If

        crStr = CStr((TC - FC) - SC) & " item(s) were deleted. " & crStr

        ' Write to the console
        Console.WriteLine(crStr)

        ' Write to the logfile
        My.Computer.FileSystem.WriteAllText(GS_LogDir, crStr & Environment.NewLine, True)

        ' Show space recovered
        If GS_SpaceShow = True Then
            My.Computer.FileSystem.WriteAllText(GS_LogDir, "Space Recovered In This Stage:" & Environment.NewLine & "C:\ - " & ReturnParsedSize(RS_C) & Environment.NewLine & "D:\ - " & ReturnParsedSize(RS_D) & Environment.NewLine, True)
            Console.WriteLine("Space Recovered In This Stage:" & Environment.NewLine & "C:\ - " & ReturnParsedSize(RS_C) & Environment.NewLine & "D:\ - " & ReturnParsedSize(RS_D))
        End If

        ' Show removed items
        If GS_ShowRemovedInLog = True Then
            My.Computer.FileSystem.WriteAllText(GS_LogDir, "The following files could not be removed: " & Environment.NewLine, True)
            For Each ffi As String In FL
                My.Computer.FileSystem.WriteAllText(GS_LogDir, ffi & Environment.NewLine, True)
            Next
        End If

        ' Total file space recovered count
        TS_CBatchSizeSave += RS_C
        TS_DBatchSizeSave += RS_D
    End Sub

    Private Sub MultiFolderClear(ByVal Paths As ArrayList)
        Dim TC As Long = 0
        Dim FC As Long = 0
        Dim SC As Long = 0
        Dim FL As New ArrayList
        Dim RS_C As ULong = 0
        Dim RS_D As ULong = 0

        For Each item As String In Paths
            Try
                For Each Fi As String In My.Computer.FileSystem.GetFiles(item, FileIO.SearchOption.SearchAllSubDirectories)
                    Try

                        ' Get File Size
                        Dim TSize As ULong
                        If GS_SpaceShow = True Then
                            TSize = CULng(My.Computer.FileSystem.GetFileInfo(Fi).Length)
                        End If

                        ' Check if the file fits a skip list item
                        If CheckForSkipFlag(Fi) = False Then

                            ' If Not
                            ' Delete the File
                            My.Computer.FileSystem.DeleteFile(Fi)

                            ' Create a block
                            Console.ForegroundColor = LS_RemoveOK
                            Console.Write("▒")
                            Console.ForegroundColor = LS_Forecolor

                            ' If showing removed files log it
                            If GS_ShowRemovedInLog Then
                                My.Computer.FileSystem.WriteAllText(GS_LogDir, "Removed: " & Fi & Environment.NewLine, True)
                            End If

                            ' Add space to recovered space count
                            If Fi.StartsWith("C:\") Then
                                RS_C += TSize
                            Else
                                RS_D += TSize
                            End If
                        Else
                            ' If it is on the list
                            ' Create a block
                            Console.ForegroundColor = LS_SkipColor
                            Console.Write("▒")
                            Console.ForegroundColor = LS_Forecolor

                            My.Computer.FileSystem.WriteAllText(GS_LogDir, "Skipped: " & Fi & Environment.NewLine, True)
                            SC += 1
                        End If

                    Catch ex As Exception
                        Console.ForegroundColor = LS_RemoveFail
                        Console.Write("▒")
                        Console.ForegroundColor = LS_Forecolor

                        My.Computer.FileSystem.WriteAllText(GS_LogDir, ex.Message & Environment.NewLine, True)
                        FC += 1
                        FL.Add(ex.Message)
                    End Try

                    TC += 1
                Next

                For Each Di As String In My.Computer.FileSystem.GetDirectories(item, FileIO.SearchOption.SearchAllSubDirectories)

                    Try
                        If CheckForSkipFlag(Di) = False Then
                            ' If not a match delete it
                            My.Computer.FileSystem.DeleteDirectory(Di, FileIO.DeleteDirectoryOption.DeleteAllContents)
                            Console.ForegroundColor = LS_RemoveOK
                            Console.Write("▒")
                            Console.ForegroundColor = LS_Forecolor

                            If GS_ShowRemovedInLog Then
                                My.Computer.FileSystem.WriteAllText(GS_LogDir, "Removed: " & Di & Environment.NewLine, True)
                            End If
                        Else
                            ' If its a match dont delete it. Just log it
                            Console.ForegroundColor = LS_SkipColor
                            Console.Write("▒")
                            Console.ForegroundColor = LS_Forecolor

                            My.Computer.FileSystem.WriteAllText(GS_LogDir, "Skipped: " & Di & Environment.NewLine, True)
                            SC += 1
                        End If

                    Catch ex As DirectoryNotFoundException
                        Console.ForegroundColor = LS_RemoveOK
                        Console.Write("▒")
                        Console.ForegroundColor = LS_Forecolor

                        If GS_ShowRemovedInLog Then
                            My.Computer.FileSystem.WriteAllText(GS_LogDir, "Removed: " & Di & Environment.NewLine, True)
                        End If
                    Catch ex As IOException
                        Console.ForegroundColor = LS_RemoveFail
                        Console.Write("▒")
                        Console.ForegroundColor = LS_Forecolor

                        My.Computer.FileSystem.WriteAllText(GS_LogDir, "The Directory '" & Di & "' is not empty." & Environment.NewLine, True)
                    Catch ex As Exception
                        Console.ForegroundColor = LS_RemoveFail
                        Console.Write("▒")
                        Console.ForegroundColor = LS_Forecolor

                        My.Computer.FileSystem.WriteAllText(GS_LogDir, ex.Message & Environment.NewLine, True)
                        FL.Add(ex.Message)
                        FC += 1
                    End Try

                    TC += 1

                Next

            Catch ex As Exception
                Console.ForegroundColor = LS_RemoveFail
                Console.Write("▒")
                Console.ForegroundColor = LS_Forecolor

                My.Computer.FileSystem.WriteAllText(GS_LogDir, ex.Message & Environment.NewLine, True)
                FL.Add(ex.Message)
                FC += 1
                TC += 1
            End Try

        Next

        Console.WriteLine("")

        'Compile the files removed string
        Dim crStr As String = String.Empty

        If FC > 0 Then
            crStr = FC & " item(s) failed to delete. "
        End If

        If SC > 0 Then
            crStr += SC & " item(s) were skipped."
        End If

        crStr = CStr((TC - FC) - SC) & " item(s) were deleted. " & crStr

        ' Write to the console
        Console.WriteLine(crStr)

        ' Write to the logfile
        My.Computer.FileSystem.WriteAllText(GS_LogDir, crStr & Environment.NewLine, True)

        ' Show space recovered
        If GS_SpaceShow = True Then
            My.Computer.FileSystem.WriteAllText(GS_LogDir, "Space Recovered In This Stage:" & Environment.NewLine & "C:\ - " & ReturnParsedSize(RS_C) & Environment.NewLine & "D:\ - " & ReturnParsedSize(RS_D) & Environment.NewLine, True)
            Console.WriteLine("Space Recovered In This Stage:" & Environment.NewLine & "C:\ - " & ReturnParsedSize(RS_C) & Environment.NewLine & "D:\ - " & ReturnParsedSize(RS_D))
        End If

        ' Show removed items
        If GS_ShowRemovedInLog = True Then
            My.Computer.FileSystem.WriteAllText(GS_LogDir, "The following files could not be removed: " & Environment.NewLine, True)
            For Each ffi As String In FL
                My.Computer.FileSystem.WriteAllText(GS_LogDir, ffi & Environment.NewLine, True)
            Next
        End If

        ' Total file space recovered count
        TS_CBatchSizeSave += RS_C
        TS_DBatchSizeSave += RS_D
    End Sub

    Function ReturnSpaceRecoveredTotal() As String
        My.Computer.FileSystem.WriteAllText(GS_LogDir, " -- Total Space Recovered -- " & Environment.NewLine & "C:\ - " & ReturnParsedSize(TS_CBatchSizeSave) & Environment.NewLine & "D:\ - " & ReturnParsedSize(TS_DBatchSizeSave) & Environment.NewLine, True)
        Console.WriteLine(Environment.NewLine & " -- Total Space Recovered -- " & Environment.NewLine & "C:\ - " & ReturnParsedSize(TS_CBatchSizeSave) & Environment.NewLine & "D:\ - " & ReturnParsedSize(TS_DBatchSizeSave))
        Return Nothing
    End Function

    Function ReturnParsedSize(ByVal bytesize As ULong) As String
        Dim ps As String = Nothing

        Select Case bytesize
            Case Is >= 1125899906842624 'a pb
                ps = String.Format("{0:0.##} PB", bytesize / 1024 / 1024 / 1024 / 1024 / 1024)
                Return ps
            Case Is >= 1099511627776  'a tb
                ps = String.Format("{0:0.##} TB", bytesize / 1024 / 1024 / 1024 / 1024)
                Return ps
            Case Is >= 1073741824  'a gb
                ps = String.Format("{0:0.##} GB", bytesize / 1024 / 1024 / 1024)
                Return ps
            Case Is >= 1048576  'a mb
                ps = String.Format("{0:0.##} MB", bytesize / 1024 / 1024)
                Return ps
            Case Is >= 1024  'a kb
                ps = String.Format("{0:0.##} KB", bytesize / 1024)
                Return ps
            Case Else
                Return bytesize & " B"
        End Select

    End Function

    Function CheckForSkipFlag(ByVal Path As String) As Boolean
        Dim DeleteResult As Boolean = False

        For Each sPath As String In GS_SkipList

            If Path.ToLower.StartsWith(sPath.ToLower) Then
                DeleteResult = True
                Exit For
            End If

        Next

        Return DeleteResult
    End Function

    Private Sub GetSkipList()

        Dim xDoc As New XmlDocument
        Dim xNodes As XmlNodeList

        Try
            xDoc.Load(My.Application.Info.DirectoryPath & "\SysClean.SkipList.xml")
        Catch ex As Exception
        End Try

        Try
            xNodes = xDoc.SelectNodes("/skiplist/items")
            For Each xN As XmlNode In xNodes
                GS_SkipList.Add(xN.FirstChild.InnerText)
            Next
        Catch ex As Exception
        End Try


    End Sub

    Private Sub GetCustomDeleteList()

        Dim xDoc As New XmlDocument
        Dim xNodes As XmlNodeList
        Dim xNode As XmlNode

        Try
            xDoc.Load(My.Application.Info.DirectoryPath & "\SysClean.CustomDeleteList.xml")
        Catch ex As Exception
        End Try

        Try
            xNodes = xDoc.SelectNodes("/deletelist/items/item")
            For Each xNode In xNodes
                If xNode.Attributes("type").Value.ToString = "file" Then
                    GS_CustomDeleteFileList.Add(xNode.InnerText)
                ElseIf xNode.Attributes("type").Value.ToString = "folder" Then
                    GS_CustomDeleteFolderList.Add(xNode.InnerText)
                Else
                    ' Ignore the rubbish :/
                End If
            Next
        Catch ex As Exception
            Console.Error.Write(ex.Message)
        End Try


    End Sub

    Private Sub UnhandledExceptionTrapper(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Console.WriteLine(e.ExceptionObject.ToString())

        My.Computer.FileSystem.WriteAllText("C:\FTP\SysCleanException.log", e.ExceptionObject.ToString, True)

        Environment.Exit(1)
    End Sub

End Module
