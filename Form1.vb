Imports System.Windows.Forms

Imports InTheHand.Net
Imports InTheHand.Net.Bluetooth
Imports InTheHand.Net.Sockets
Imports InTheHand.Windows.Forms
Imports System.IO
Imports System.Threading

Imports System.Drawing.Drawing2D

Public Class Form1
    Public Bluetooth As SerialDevice = New SerialDevice
    Dim waitForMouseRelease As Boolean = False
    Dim JoystickX, JoystickY As Double

    Private Sub LoadCOMPorts()
        Dim ports As List(Of String) = ComPort.Names
        Dim i As Integer

        For i = 0 To ports.Count - 1
            ports(i) = ports(i).Substring(3)
        Next

        Dim q = From a In ports
                Where CInt(a) > 0
                Order By CInt(a) Ascending
                Select a

        'PortSelector.Items.Clear()
        'For Each port As String In q
        ' PortSelector.Items.Add("COM" & port)
        'Next
    End Sub

    Private Function IsMessageComplete(ByVal data As Byte(), ByVal length As Integer) As Integer
        Dim index As Integer = Array.IndexOf(data, CByte(13))
        If index > -1 Then
            Return index + 1
        End If
        Return 0
    End Function

    Private Sub ProcessMessage(ByVal data As Byte(), ByVal length As Integer)
        Dim message As String = System.Text.Encoding.ASCII.GetString(data.ToArray)
        If message.Substring(0, 1) = Chr(10) Then
            message = message.Substring(1, message.Length - 2) & ","
        Else
            message = message.Substring(0, message.Length - 1) & ","
        End If

        Dim splitMessage() As String = message.Split(",")
        Select Case splitMessage(0).ToUpper

            Case "I"
                SetObjectText(FWVersion, "Firmware Version: " & splitMessage(1))
                SetObjectText(MCU, "MCU: " & splitMessage(2))
                SetObjectText(BatteryLevel, "Battery Level: " & splitMessage(3))
                Dim rawMinutes As Double = Convert.ToDouble(splitMessage(4).Replace(".", ","))
                Dim minutes, seconds As Integer
                minutes = Math.Floor(rawMinutes)
                seconds = (rawMinutes - minutes) * 60
                SetObjectText(RunTime, "Run Time: " & minutes & " min " & seconds & " sec")

                Exit Select

            Case "P"
                SetObjectText(PID_P, "P: " & splitMessage(1))
                SetObjectText(PID_I, "I: " & splitMessage(2))
                SetObjectText(PID_D, "D: " & splitMessage(3))
                SetObjectText(TargetAngle, "Target Angle: " & splitMessage(4) & Chr(176))
                Exit Select

            Case "S"
                Exit Select

            Case "K"
                Exit Select

            Case "V"
                accPoints(writePos) = Convert.ToDouble(splitMessage(1).Replace(".", ","))
                gyroPoints(writePos) = Convert.ToDouble(splitMessage(2).Replace(".", ","))
                kalmanPoints(writePos) = Convert.ToDouble(splitMessage(3).Replace(".", ","))
                writePos += 1
                If writePos = PictureBox1.Width Then
                    writePos = 0
                End If
                readPos += 1
                If readPos = PictureBox1.Width Then
                    readPos = 0
                End If
                Exit Select

        End Select

        Invoke(New Action(Of String)(AddressOf AppendOutput), message)
    End Sub

    Private Sub AppendOutput(ByVal message As String)
        If TextBox1.TextLength > 0 Then TextBox1.AppendText(ControlChars.NewLine)
        TextBox1.AppendText(message)
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Bluetooth.Connected Then
            RunTimer.Stop()
            RunTimer.Enabled = False
            MouseReleaseTimer.Stop()
            MouseReleaseTimer.Enabled = False
            dataGraphTimer.Stop()
            dataGraphTimer.Enabled = False
            'MsgBox("Please disconnect before closing the program", MsgBoxStyle.Exclamation)
            ConnectCOM_Click(Nothing, Nothing)
            'e.Cancel = True
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadCOMPorts()

        JoystickLine.X1 = JoystickCircle.Location.X + (JoystickCircle.Width / 2)
        JoystickLine.X2 = JoystickLine.X1

        JoystickLine.Y1 = JoystickCircle.Location.Y + (JoystickCircle.Height / 2)
        JoystickLine.Y2 = JoystickLine.Y1
    End Sub

    Private Delegate Sub SetObjectTextDelegate(ByVal aControl As Control, ByVal text As String)
    Private Sub SetObjectText(ByVal aControl As Control, ByVal someText As String)
        If ConnectCOM.InvokeRequired Then
            Dim del As New SetObjectTextDelegate(AddressOf SetObjectText)
            Me.Invoke(del, New Object() {aControl, someText})
        Else
            aControl.Text = someText
        End If
    End Sub

    Private Delegate Sub SetObjectHeightDelegate(ByVal aControl As Control, ByVal aHeight As Integer)
    Private Sub SetObjectHeight(ByVal aControl As Control, ByVal aHeight As Integer)
        If InfoBox.InvokeRequired Then
            Dim del As New SetObjectHeightDelegate(AddressOf SetObjectHeight)
            Me.Invoke(del, New Object() {aControl, aHeight})
        Else
            aControl.Height = aHeight
        End If
    End Sub

    Private BluetoothConnectThread As Thread = New Thread(AddressOf BluetoothConnectTask)
    Private Sub BluetoothConnectTask()
        If (Bluetooth.Connect() And Bluetooth.Connected) Then
            Bluetooth.SendString("GI;")
            Bluetooth.SendString("GP;")
            Bluetooth.SendString("GS;")
            Bluetooth.SendString("GK;")
            ConnectCOM.Image = Nothing
            SetObjectText(ConnectCOM, "Disconnect")
            SetObjectHeight(InfoBox, 272)
            'StatusLabel.Text = "Connected"
        Else
            ConnectCOM.Image = Nothing
            SetObjectText(ConnectCOM, "Connect")
            SetObjectHeight(InfoBox, 50)
        End If
    End Sub

    Private Sub ConnectCOM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConnectCOM.Click
        Dim i As Integer
        Dim port As String = ""
        Dim bldialog As SelectBluetoothDeviceDialog = New SelectBluetoothDeviceDialog()

        bldialog.ShowAuthenticated = True
        bldialog.ShowRemembered = True
        bldialog.ShowUnknown = True

        If (Bluetooth.Connected = False And Not BluetoothConnectThread.IsAlive) Then
            If (bldialog.ShowDialog() = DialogResult.OK) Then
                ComPort.LoadUUIDs()
                For i = 0 To ComPort.NamesUUID(0).Count - 1
                    If (ComPort.NamesUUID(0).Item(i).Contains(bldialog.SelectedDevice.DeviceAddress.ToString())) Then
                        port = ComPort.NamesUUID(1).Item(i)
                        Exit For
                    End If
                Next

                If port.Contains("COM") Then
                    Dim authenticator As BluetoothWin32Authentication = New BluetoothWin32Authentication(bldialog.SelectedDevice.DeviceAddress, "0000")

                    Bluetooth.CheckMessageComplete = AddressOf IsMessageComplete
                    Bluetooth.ProcessMessage = AddressOf ProcessMessage
                    Bluetooth.ConfigurePort(CommonBaudRate.bps115200)
                    Bluetooth.ComPort = port
                    Bluetooth.MessageMode = MessageProcessingMode.BlockMode
                    ConnectCOM.Text = ""
                    ConnectCOM.Image = My.Resources.loading

                    BluetoothConnectThread = New Thread(AddressOf BluetoothConnectTask)
                    BluetoothConnectThread.IsBackground = True
                    BluetoothConnectThread.Start()
                Else
                    MsgBox("The selected device has not been paired!" & vbNewLine & "Pairing procedure initiated - please wait till enumeration has finished and try to connect again", MsgBoxStyle.Exclamation)

                    Dim device As BluetoothDeviceInfo = New BluetoothDeviceInfo(bldialog.SelectedDevice.DeviceAddress)  ' Or from discovery etc
                    Dim state As Boolean = True
                    device.SetServiceState(BluetoothService.SerialPort, state, True)
                End If
            End If
        ElseIf (Not BluetoothConnectThread.IsAlive) Then
            Bluetooth.SendString("IS;")
            Bluetooth.Disconnect()
            ConnectCOM.Text = "Connect"
            InfoBox.Height = 50
            'StatusLabel.Text = "Disconnected"
        End If
    End Sub

    Private Sub RunTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunTimer.Tick
        If Bluetooth.Connected Then
            Bluetooth.SendString("GI;")
        End If
    End Sub

    Private Sub JoystickCircle_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles JoystickCircle.MouseDown
        If (e.Button = Windows.Forms.MouseButtons.Left) Then
            JoystickLine.X2 = e.X
            JoystickLine.Y2 = e.Y
            waitForMouseRelease = True

            If (Bluetooth.Connected) Then
                JoystickX = 2 * (JoystickLine.X2 - JoystickLine.X1) / JoystickCircle.Width
                JoystickY = 2 * (JoystickLine.Y1 - JoystickLine.Y2) / JoystickCircle.Height
                Bluetooth.SendString("CJ," & Math.Round(JoystickX, 4).ToString().Replace(",", ".") & "," & Math.Round(JoystickY, 4).ToString().Replace(",", ".") & ";")
                Invoke(New Action(Of String)(AddressOf AppendOutput), "CJ," & Math.Round(JoystickX, 4).ToString().Replace(",", ".") & "," & Math.Round(JoystickY, 4).ToString().Replace(",", ".") & ";")
            End If
        End If
    End Sub

    Private Sub JoystickCircle_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles JoystickCircle.MouseMove
        If (e.Button = Windows.Forms.MouseButtons.Left) Then
            JoystickLine.X2 = e.X
            JoystickLine.Y2 = e.Y
            waitForMouseRelease = True

            If (Bluetooth.Connected) Then
                JoystickX = 2 * (JoystickLine.X2 - JoystickLine.X1) / JoystickCircle.Width
                JoystickY = 2 * (JoystickLine.Y1 - JoystickLine.Y2) / JoystickCircle.Height
            End If
        End If
    End Sub

    Private Sub JoystickCircle_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles JoystickCircle.MouseUp
        JoystickLine.X2 = JoystickLine.X1
        JoystickLine.Y2 = JoystickLine.Y1
        waitForMouseRelease = False

        If (Bluetooth.Connected) Then
            JoystickX = 0
            JoystickY = 0
            Bluetooth.SendString("CS;")
        End If
    End Sub

    Private Sub OvalShape1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles JoystickCircle.Resize
        If (JoystickCircle.Width < 200) Then
            JoystickCircle.Width = 200
        End If
        If (JoystickCircle.Height < 200) Then
            JoystickCircle.Height = 200
        End If

        If JoystickCircle.Width > JoystickCircle.Height Then
            JoystickCircle.Width = JoystickCircle.Height
        End If
        If JoystickCircle.Height > JoystickCircle.Width Then
            JoystickCircle.Width = JoystickCircle.Height
        End If

        JoystickLine.X1 = JoystickCircle.Location.X + (JoystickCircle.Width / 2)
        JoystickLine.X2 = JoystickLine.X1

        JoystickLine.Y1 = JoystickCircle.Location.Y + (JoystickCircle.Height / 2)
        JoystickLine.Y2 = JoystickLine.X1
    End Sub

    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal uFlags As Integer) As Integer
    Private Sub MouseReleaseTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MouseReleaseTimer.Tick
        If (waitForMouseRelease = True) Then
            If (GetAsyncKeyState(1)) Then
                If (Bluetooth.Connected) Then
                    Bluetooth.SendString("CJ," & Math.Round(JoystickX, 4).ToString().Replace(",", ".") & "," & Math.Round(JoystickY, 4).ToString().Replace(",", ".") & ";")
                    Invoke(New Action(Of String)(AddressOf AppendOutput), "CJ," & Math.Round(JoystickX, 4).ToString().Replace(",", ".") & "," & Math.Round(JoystickY, 4).ToString().Replace(",", ".") & ";")
                End If                
            Else
                JoystickLine.X2 = JoystickLine.X1
                JoystickLine.Y2 = JoystickLine.Y1
                waitForMouseRelease = False

                If (Bluetooth.Connected) Then
                    JoystickX = 0
                    JoystickY = 0
                    Bluetooth.SendString("CS;")
                End If
            End If
        End If


    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Form2.Show()
    End Sub




    Private _bmp As Bitmap = Nothing
    Private magnifier As Integer = 20
    Private readPos, writePos As Integer
    Private _d As Double

    Private accPoints(), gyroPoints(), kalmanPoints() As Double


    Private Sub Plot(ByVal d As Double)
        Dim tempReadPos As Integer
        Dim virtualY As Double
        If _bmp IsNot Nothing Then
            _bmp.Dispose()
            _bmp = Nothing
        End If

        _bmp = New Bitmap(PictureBox1.Width, PictureBox1.Height)


        'getData
        Dim accPath, gyroPath, kalmanPath As New GraphicsPath()

        Dim pts1 As New List(Of PointF)()
        Dim pts2 As New List(Of PointF)()
        Dim pts3 As New List(Of PointF)()
        tempReadPos = readPos
        For x As Integer = -_bmp.Width / 2 To _bmp.Width / 2 - 1
            If accPoints(tempReadPos) > 0 Then
                virtualY = (((accPoints(tempReadPos) - 180) / 180) * _bmp.Height) / 2
                pts1.Add(New PointF(x, virtualY))
            Else
                If pts1.Count > 0 Then
                    accPath.AddLines(pts1.ToArray())
                    pts1 = New List(Of PointF)()
                End If
            End If

            If gyroPoints(tempReadPos) > 0 Then
                virtualY = (((gyroPoints(tempReadPos) - 180) / 180) * _bmp.Height) / 2
                pts2.Add(New PointF(x, virtualY))
            Else
                If pts2.Count > 0 Then
                    gyroPath.AddLines(pts2.ToArray())
                    pts2 = New List(Of PointF)()
                End If
            End If

            If kalmanPoints(tempReadPos) > 0 Then
                virtualY = (((kalmanPoints(tempReadPos) - 180) / 180) * _bmp.Height) / 2
                pts3.Add(New PointF(x, virtualY))
            Else
                If pts3.Count > 0 Then
                    kalmanPath.AddLines(pts3.ToArray())
                    pts3 = New List(Of PointF)()
                End If
            End If
            tempReadPos += 1
            If tempReadPos = PictureBox1.Width Then
                tempReadPos = 0
            End If
        Next

        If pts1.Count > 0 Then
            accPath.AddLines(pts1.ToArray())
            pts1 = New List(Of PointF)()
        End If
        If pts2.Count > 0 Then
            gyroPath.AddLines(pts2.ToArray())
            pts2 = New List(Of PointF)()
        End If
        If pts3.Count > 0 Then
            kalmanPath.AddLines(pts3.ToArray())
            pts3 = New List(Of PointF)()
        End If


        'pts = New List(Of PointF)()
        'For x As Integer = -_bmp.Width / 2 To _bmp.Width / 2 - 1
        ' 'pts.Add(New PointF(x, CSng(Math.Sin(x / CDbl(magnifier)) * CDbl(magnifier) * 10)))
        ' pts.Add(New PointF(x, CSng(Math.Sin(((x - d - 5) / 2.0) / CDbl(magnifier)) * CDbl(magnifier) * 10)))
        ' Next
        'gyroPath.AddLines(pts.ToArray())

        'pts = New List(Of PointF)()
        'For x As Integer = -_bmp.Width / 2 To _bmp.Width / 2 - 1
        ' 'pts.Add(New PointF(x, CSng(Math.Sin(x / CDbl(magnifier)) * CDbl(magnifier) * 10)))
        ' pts.Add(New PointF(x, CSng(Math.Sin(((x - d - 10) / 3.0) / CDbl(magnifier)) * CDbl(magnifier) * 10)))
        ' Next
        'kalmanPath.AddLines(pts.ToArray())

        DrawGridAndAxAndGraph(_bmp, accPath, gyroPath, kalmanPath)
        accPath.Dispose()
        gyroPath.Dispose()
        kalmanPath.Dispose()
    End Sub

    Private Sub DrawText(ByVal text As String, ByVal x As Integer, ByVal y As Integer, ByRef g As Graphics)
        ' Save the current graphics state.
        Dim state As GraphicsState = g.Save()

        'Dim scale_x As Integer = IIf(x, -1, 1)
        'Dim scale_y As Integer = IIf(x, -1, 1)
        'g.ResetTransform()
        g.ScaleTransform(1.0F, -1.0F)

        ' Figure out where to draw.
        Dim txt_size As SizeF = g.MeasureString(text, New Font("Arial Black", 10))
        x = x
        y = -y - txt_size.Height

        g.DrawString(text, New Font("Arial Black", 10), New SolidBrush(Color.Black), x, y)

        ' Restore the original graphics state.
        g.Restore(state)
    End Sub

    Private Sub DrawGridAndAxAndGraph(ByVal bmp As Bitmap, ByVal accPath As GraphicsPath, ByVal gyroPath As GraphicsPath, ByVal kalmanPath As GraphicsPath)
        Dim virtualY As Double        

        Using g As Graphics = Graphics.FromImage(bmp)
            Dim fontHeight As Single = g.MeasureString("test", New Font("Arial Black", 10)).Height
            g.Transform = New Matrix(1, 0, 0, -1, bmp.Width / 2, bmp.Height / 2)

            'magnifier size
            Dim i As Integer = 0
            While i < bmp.Width / 2
                g.DrawLine(Pens.Gray, New Point(i, -bmp.Height / 2), New Point(i, bmp.Height / 2))
                i += magnifier
            End While

            i = 0
            While i < bmp.Height / 2
                g.DrawLine(Pens.Gray, New Point(-bmp.Width / 2, i), New Point(bmp.Width / 2, i))
                If (i < ((bmp.Height / 2) - fontHeight)) Then
                    virtualY = ((2 * i / bmp.Height) * 180) + 180
                    virtualY = Math.Round(virtualY, 0)
                    DrawText(virtualY.ToString(), -bmp.Width / 2, i, g)
                End If
                i += magnifier
            End While

            i = magnifier
            While i > -bmp.Width / 2
                g.DrawLine(Pens.Gray, New Point(i, -bmp.Height / 2), New Point(i, bmp.Height / 2))
                i -= magnifier
            End While

            i = magnifier
            While i > -bmp.Height / 2
                g.DrawLine(Pens.Gray, New Point(-bmp.Width / 2, i), New Point(bmp.Width / 2, i))
                virtualY = ((2 * i / bmp.Height) * 180) + 180
                virtualY = Math.Round(virtualY, 0)
                DrawText(virtualY.ToString(), -bmp.Width / 2, i, g)
                i -= magnifier
            End While

            g.DrawLine(Pens.Blue, New Point(0, -bmp.Height / 2), New Point(0, bmp.Height / 2))
            g.DrawLine(Pens.Blue, New Point(-bmp.Width / 2, 0), New Point(bmp.Width / 2, 0))

            g.SmoothingMode = SmoothingMode.AntiAlias
            g.DrawPath(New Pen(Brushes.Red, 2), accPath)
            g.DrawPath(New Pen(Brushes.Green, 2), gyroPath)
            g.DrawPath(New Pen(Brushes.Blue, 2), kalmanPath)
        End Using
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DoubleBuffered = True
        dataGraphTimer.Start()
    End Sub

    Private Sub dataGraphTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dataGraphTimer.Tick
        dataGraphTimer.Stop()
        _d -= 2
        Plot(_d)
        PictureBox1.Invalidate()
        dataGraphTimer.Start()
    End Sub

    Private Sub Form2_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not _bmp Is Nothing Then
            _bmp.Dispose()
        End If
    End Sub

    Private Sub PictureBox1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint
        If Not _bmp Is Nothing Then
            e.Graphics.DrawImage(_bmp, 0, 0)
        End If
    End Sub

    Private Sub PictureBox1_Resize1(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.Resize
        accPoints = New Double(PictureBox1.Width) {}
        gyroPoints = New Double(PictureBox1.Width) {}
        kalmanPoints = New Double(PictureBox1.Width) {}

        writePos = 0
        readPos = 1
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        RunTimer.Enabled = False
        MouseReleaseTimer.Enabled = False

        If (dataGraphTimer.Enabled = True) Then
            If (Bluetooth.Connected) Then
                Bluetooth.SendString("IS;")
            End If
            dataGraphTimer.Enabled = False
        End If

        If TabControl1.SelectedIndex = 0 Then
            RunTimer.Enabled = True
        ElseIf TabControl1.SelectedIndex = 1 Then
            MouseReleaseTimer.Enabled = True
        ElseIf TabControl1.SelectedIndex = 2 Then
            accPoints = New Double(PictureBox1.Width) {}
            gyroPoints = New Double(PictureBox1.Width) {}
            kalmanPoints = New Double(PictureBox1.Width) {}

            writePos = 0
            readPos = 1

            If (Bluetooth.Connected) Then
                Bluetooth.SendString("IB;")
            End If
            dataGraphTimer.Enabled = True
        End If
    End Sub

End Class
