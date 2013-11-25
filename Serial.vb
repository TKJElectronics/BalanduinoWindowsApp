Option Strict On
Option Explicit On
Option Infer Off

Imports System.IO.Ports
Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Text.RegularExpressions
Imports Microsoft.Win32

''' <summary>
''' Provides a wrapper for a SerialPort which automatically reads data from the port and processes it via assigned Func(Of Byte(), Integer, Integer) delegates.
''' Also provides "Send" methods for writing character, byte, hex, and string data to the port.
''' </summary>
''' <remarks></remarks>
Public Class SerialDevice
    Private WithEvents _Port As SerialPort
    Private _DataQueue As New ConcurrentQueue(Of Byte)
    Private _CancelSource As CancellationTokenSource
    Private _Task As System.Threading.Tasks.Task
    Private _Waiter As System.Threading.ManualResetEventSlim

    ''' <summary>
    ''' Gets a value determining if the serial port is open.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Connected As Boolean
        Get
            Return _Port.IsOpen
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the COM Port name that the physical device is connected to, or virtual COM Port name created by the physical device.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ComPort As String
        Get
            Return _Port.PortName
        End Get
        Set(ByVal value As String)
            _Port.PortName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value determining whether or not the serial port will use data-terminal-ready signals (default is False).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DtrEnable As Boolean
        Get
            Return _Port.DtrEnable
        End Get
        Set(ByVal value As Boolean)
            _Port.DtrEnable = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the text encoding used to convert between character strings and byte data (default is ASCII).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Encoding As System.Text.Encoding
        Get
            Return _Port.Encoding
        End Get
        Set(ByVal value As System.Text.Encoding)
            _Port.Encoding = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the string used to represent a line terminator (default CrLf).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NewLine As String
        Get
            Return _Port.NewLine
        End Get
        Set(ByVal value As String)
            _Port.NewLine = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value specifying the baud rate to use.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Baud As Integer
        Get
            Return _Port.BaudRate
        End Get
        Set(ByVal value As Integer)
            _Port.BaudRate = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value specifying the data bits to use.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataBits As Integer
        Get
            Return _Port.DataBits
        End Get
        Set(ByVal value As Integer)
            _Port.DataBits = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value specifying the stop bits to use.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StopBits As StopBits
        Get
            Return _Port.StopBits
        End Get
        Set(ByVal value As StopBits)
            _Port.StopBits = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value specifying the parity to use.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Parity As Parity
        Get
            Return _Port.Parity
        End Get
        Set(ByVal value As Parity)
            _Port.Parity = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value specifying the hardware handshaking to use.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FlowControl As Handshake
        Get
            Return _Port.Handshake
        End Get
        Set(ByVal value As Handshake)
            _Port.Handshake = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value determining whether or not the serial port uses ready-to-send signals (default False).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RtsEnable As Boolean
        Get
            Return _Port.RtsEnable
        End Get
        Set(ByVal value As Boolean)
            _Port.RtsEnable = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the processing mode. In ByteMode, the CheckMessageComplete delegate will be invoked for every byte received.
    ''' In BlockMode, available data will be read at once and sent to CheckMessageComplete in chunks, as available.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MessageMode As MessageProcessingMode

    ''' <summary>
    ''' Gets or sets a delegate function which will be called to determine if a complete message has been queued.
    ''' This function must return zero if the bytes received so far do not constitute a complete message, or a positive
    ''' integer value representing the number of bytes which make up the complete message.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <example>
    ''' The following example defines a simple check message delegate function and sets it
    ''' to the CheckMessageComplete property.
    ''' <code>
    ''' mySerialDevice.CheckMessageComplete = AddressOf(DoCheckMessage)
    '''
    ''' Function DoCheckMessage(data As Byte(), count As Integer) As Integer
    '''    '1) Determine if data contains complete message
    '''    '2) Return number of bytes in message if complete, or return 0 if not complete
    ''' End Function
    ''' </code>
    ''' </example>
    Public Property CheckMessageComplete As Func(Of Byte(), Integer, Integer)

    ''' <summary>
    ''' An optional Action(Of PortState, SerialError) delegate to be invoked when a serial port error occurs.
    ''' Only as reliable as the SerialPort.ErrorReceived event. See MSDN documentation for details.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProcessError As Action(Of PortState, SerialError)
    ''' <summary>
    ''' A required Action(Of Byte(), Integer) delegate to be invoked when the CheckMessageComplete delegate call returns a positive value.
    ''' This delegate method performs the actual work of processing the message data received from the serial device.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProcessMessage As Action(Of Byte(), Integer)
    ''' <summary>
    ''' An optional Action(Of PortState, SerialPinChange) delegate to be invokde when a serial pin change event occurs.
    ''' See the MSDN documentation for SerialPort.PinChanged event for more information.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProcessPinChange As Action(Of PortState, SerialPinChange)

    ''' <summary>
    ''' Configures the serial port with the default baud rate (9800), 8 data bits, no parity, one stop bit, and no flow control.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ConfigurePort()
        ConfigurePort(CommonBaudRate.Default, 8, Parity.None, StopBits.One, Handshake.None)
    End Sub

    Public Sub ConfigurePort(ByVal baud As CommonBaudRate)
        ConfigurePort(CInt(baud), 8, Parity.None, StopBits.One, Handshake.None)
    End Sub

    Public Sub ConfigurePort(ByVal baud As Integer)
        ConfigurePort(baud, 8, Parity.None, StopBits.One, Handshake.None)
    End Sub

    Public Sub ConfigurePort(ByVal baud As CommonBaudRate, ByVal dataBits As Integer, ByVal parity As Parity, ByVal stopBits As StopBits, ByVal flowControl As Handshake)
        ConfigurePort(CInt(baud), dataBits, parity, stopBits, flowControl)
    End Sub

    Public Sub ConfigurePort(ByVal baud As Integer, ByVal dataBits As Integer, ByVal parity As Parity, ByVal stopBits As StopBits, ByVal flowControl As Handshake)
        _Port.BaudRate = baud
        _Port.DataBits = dataBits
        _Port.Handshake = flowControl
        _Port.Parity = parity
        _Port.StopBits = stopBits
    End Sub

    ''' <summary>
    ''' Creates a new instance of SerialDevice using the specified existing SerialPort instance.
    ''' </summary>
    ''' <param name="serialPort"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal serialPort As SerialPort)
        _Port = serialPort
        ConfigurePort()
    End Sub

    ''' <summary>
    ''' Creates a new instance of SerialDevice with its own internal SerialPort.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Me.New(New SerialPort())
    End Sub

    ''' <summary>
    ''' Opens the SerialPort and connects to the physical device if the PortName, CheckMessageComplete delegate, and ProcessMessage delegate have all been specified.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Connect() As Boolean
        If _Task Is Nothing Then
            If String.IsNullOrEmpty(_Port.PortName) Then Throw New InvalidOperationException("Cannot connect when COM Port is not specified.")
            If CheckMessageComplete Is Nothing Then Throw New InvalidOperationException("Cannot connect when no IsMessageComplete delegate has been specified.")
            If ProcessMessage Is Nothing Then Throw New InvalidOperationException("Cannot connect when no ProcessMessage delegate has been specified.")

            Try
                _Port.Open()
                If _Port.IsOpen Then
                    _CancelSource = New CancellationTokenSource
                    _Waiter = New ManualResetEventSlim
                    _Task = System.Threading.Tasks.Task.Factory.StartNew(New Action(Of Object)(AddressOf ProcessData), _MessageMode, _CancelSource.Token)
                    Return True
                End If
            Catch ex As Exception
                If (ex.ToString.ToLower.Contains("timeout")) Then
                    MsgBox("Timeout occoured trying to connect to the device", MsgBoxStyle.Critical, "Timeout")
                Else
                    MsgBox(ex.ToString, MsgBoxStyle.Critical, "Error")
                End If

                Return False
            End Try
        End If

        Return False
    End Function

    ''' <summary>
    ''' Ends processing of the SerialDevice and closes the underlying serial port.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Disconnect()
        If _Task IsNot Nothing Then
            _CancelSource.Cancel()
            _Waiter.Set()
            _Task.Wait()
            _Port.Close()
            _Task.Dispose()
            _CancelSource.Dispose()
            _Waiter.Dispose()
            _Task = Nothing
            _CancelSource = Nothing
            _Waiter = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Sends a single character to the serial port.
    ''' </summary>
    ''' <param name="c"></param>
    ''' <remarks></remarks>
    Public Sub SendChar(ByVal c As Char)
        _Port.Write({c}, 0, 1)
    End Sub

    ''' <summary>
    ''' Sends a byte or series of bytes to the serial port.
    ''' </summary>
    ''' <param name="bytes"></param>
    ''' <remarks></remarks>
    Public Sub SendData(ByVal ParamArray bytes() As Byte)
        If _Port.IsOpen Then
            _Port.Write(bytes, 0, bytes.Length)
        Else
            MessageBox.Show("The port is not open.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ''' <summary>
    ''' Sends a byte or series of bytes in hexadecimal format to the serial port.
    ''' </summary>
    ''' <param name="hexString"></param>
    ''' <remarks></remarks>
    Public Sub SendHex(ByVal hexString As String)
        hexString = hexString.Replace(" ", "").ToUpper
        If Not hexString.Length Mod 2 = 0 Then
            hexString = "0" & hexString
        End If
        Dim result(CInt(hexString.Length / 2) - 1) As Byte
        Dim delta As Byte
        For i As Integer = 0 To result.Length - 1
            If Byte.TryParse(hexString.Substring(i * 2, 2), Globalization.NumberStyles.AllowHexSpecifier, Nothing, delta) Then
                result(i) = delta
            Else
                Throw New ArgumentException("Invalid hex string.")
            End If
        Next
        SendData(result)
    End Sub

    ''' <summary>
    ''' Sends a string to the serial port using the current text encoding.
    ''' </summary>
    ''' <param name="text"></param>
    ''' <remarks></remarks>
    Public Sub SendString(ByVal text As String)
        _Port.Write(text)
    End Sub

    Private Sub _Port_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles _Port.DataReceived
        Dim portRef As SerialPort = DirectCast(sender, SerialPort)
        Dim data(portRef.BytesToRead - 1) As Byte
        Dim count As Integer = portRef.Read(data, 0, data.Length)
        For i As Integer = 0 To count - 1
            _DataQueue.Enqueue(data(i))
        Next
        _Waiter.Set()
    End Sub

    Private Sub _Port_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles _Port.Disposed
        _CancelSource.Cancel()
    End Sub

    Private Sub _Port_ErrorReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialErrorReceivedEventArgs) Handles _Port.ErrorReceived
        If ProcessError IsNot Nothing Then
            _ProcessError(New PortState(_Port), e.EventType)
        End If
    End Sub

    Private Sub _Port_PinChanged(ByVal sender As Object, ByVal e As System.IO.Ports.SerialPinChangedEventArgs) Handles _Port.PinChanged
        If ProcessPinChange IsNot Nothing Then
            _ProcessPinChange(New PortState(_Port), e.EventType)
        End If
    End Sub

    Protected Sub ProcessData(ByVal modeObj As Object)
        Dim mode As MessageProcessingMode = DirectCast(modeObj, MessageProcessingMode)
        Dim currentMessage As New List(Of Byte)
        Do While Not _CancelSource.IsCancellationRequested
            Dim delta As Integer = currentMessage.Count
            Dim available As Integer = _DataQueue.Count
            If available > 0 Then
                Dim b As Byte
                If mode = MessageProcessingMode.ByteMode Then
                    If _DataQueue.TryDequeue(b) Then
                        currentMessage.Add(b)
                        CheckAndProcess(currentMessage)
                    End If
                ElseIf mode = MessageProcessingMode.BlockMode Then
                    While available > 0
                        If _DataQueue.TryDequeue(b) Then
                            currentMessage.Add(b)
                            available -= 1
                        End If
                    End While
                    If Not delta = currentMessage.Count Then
                        CheckAndProcess(currentMessage)
                    End If
                End If
            Else
                _Waiter.Wait()
                _Waiter.Reset()
            End If
        Loop
    End Sub

    Protected Sub CheckAndProcess(ByVal currentMessage As List(Of Byte))
        Dim removeCount As Integer = _CheckMessageComplete(currentMessage.ToArray, currentMessage.Count)
        If removeCount > 0 Then
            _ProcessMessage(currentMessage.Take(removeCount).ToArray, removeCount)
            currentMessage.RemoveRange(0, removeCount)
        End If
    End Sub

End Class

''' <summary>
''' Provides information about the state of a serial port.
''' </summary>
''' <remarks></remarks>
Public Structure PortState
    Public ReadOnly BreakState As Boolean
    Public ReadOnly CDHolding As Boolean
    Public ReadOnly CTSHolding As Boolean
    Public ReadOnly DSRHolding As Boolean
    Public ReadOnly IsOpen As Boolean

    Public Sub New(ByVal port As SerialPort)
        BreakState = port.BreakState
        CDHolding = port.CDHolding
        CTSHolding = port.CtsHolding
        DSRHolding = port.DsrHolding
        IsOpen = port.IsOpen
    End Sub
End Structure

''' <summary>
''' Provides a list of common serial baud rates.
''' </summary>
''' <remarks></remarks>
Public Enum CommonBaudRate
    [Default] = 9600
    bps2400 = 2400
    bps4800 = 4800
    bps9600 = 9600
    bps14400 = 14400
    bps19200 = 19200
    bps28800 = 28800
    bps38400 = 38400
    bps57600 = 57600
    bps115200 = 115200
End Enum

''' <summary>
''' Specifies the processing mode for messages.
''' </summary>
''' <remarks></remarks>
Public Enum MessageProcessingMode
    ''' <summary>
    ''' The CheckMessageComplete delegate will be invoked for every byte received.
    ''' </summary>
    ''' <remarks></remarks>
    ByteMode
    ''' <summary>
    ''' The CheckMessageComplete delegate will be invoked after all available data is read on each data received event.
    ''' </summary>
    ''' <remarks></remarks>
    BlockMode
End Enum

Module ComPort
    Public Names As List(Of String)
    Public NamesUUID(2) As List(Of String)

    Public Function LoadUUIDs() As List(Of String)
        Dim COMPorts As List(Of String) = New List(Of String)()
        Dim UUIDs As List(Of String) = New List(Of String)()
        Dim rk1 As RegistryKey = Registry.LocalMachine
        Dim rk2 As RegistryKey = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\BTHENUM")
        For Each s3 As String In rk2.GetSubKeyNames()
            Dim rk3 As RegistryKey = rk2.OpenSubKey(s3)
            For Each s4 As String In rk3.GetSubKeyNames()
                Dim rk4 As RegistryKey = rk3.OpenSubKey(s4)
                On Error Resume Next
                If (rk4.GetValue("Service").ToString = "BTHMODEM") Then
                    Dim rk5 As RegistryKey = rk4.OpenSubKey("Device Parameters")
                    If (rk5.GetValue("PortName").ToString().Contains("COM")) Then
                        UUIDs.Add(rk5.GetValue("Bluetooth_UniqueID").ToString())
                        COMPorts.Add(rk5.GetValue("PortName").ToString())
                    End If
                End If
            Next
        Next

        NamesUUID(0) = UUIDs
        NamesUUID(1) = COMPorts
    End Function

    Public Function LoadNames() As List(Of String)
        Dim COMPorts As List(Of String) = New List(Of String)()

        On Error GoTo FinishNameLoading
        Dim rk1 As RegistryKey = Registry.LocalMachine
        Dim rk2 As RegistryKey = rk1.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM")        
        For Each s3 As String In rk2.GetValueNames()
            If (s3.ToUpper.Contains("BTHMODEM")) Then
                If (rk2.GetValue(s3).ToString().Contains("COM")) Then
                    COMPorts.Add(rk2.GetValue(s3).ToString())
                End If
            End If
        Next

FinishNameLoading:
        Names = COMPorts
    End Function
End Module

