<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.ShapeContainer2 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
        Me.JoystickLine = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.JoystickCircle = New Microsoft.VisualBasic.PowerPacks.OvalShape()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ConnectCOM = New System.Windows.Forms.Button()
        Me.InfoBox = New System.Windows.Forms.GroupBox()
        Me.TargetAngle = New System.Windows.Forms.Label()
        Me.RunTime = New System.Windows.Forms.Label()
        Me.BatteryLevel = New System.Windows.Forms.Label()
        Me.MCU = New System.Windows.Forms.Label()
        Me.FWVersion = New System.Windows.Forms.Label()
        Me.EEPROMVersion = New System.Windows.Forms.Label()
        Me.AppVersion = New System.Windows.Forms.Label()
        Me.PID_D = New System.Windows.Forms.Label()
        Me.PID_I = New System.Windows.Forms.Label()
        Me.PID_P = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ShapeContainer1 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
        Me.LineShape1 = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.MouseReleaseTimer = New System.Windows.Forms.Timer(Me.components)
        Me.dataGraphTimer = New System.Windows.Forms.Timer(Me.components)
        Me.TabControl1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.InfoBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Location = New System.Drawing.Point(931, 18)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox1.Size = New System.Drawing.Size(317, 342)
        Me.TextBox1.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Location = New System.Drawing.Point(228, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(697, 360)
        Me.TabControl1.TabIndex = 2
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(689, 334)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Settings"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.ShapeContainer2)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(689, 334)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Joystick"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'ShapeContainer2
        '
        Me.ShapeContainer2.Location = New System.Drawing.Point(3, 3)
        Me.ShapeContainer2.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer2.Name = "ShapeContainer2"
        Me.ShapeContainer2.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.JoystickLine, Me.JoystickCircle})
        Me.ShapeContainer2.Size = New System.Drawing.Size(683, 328)
        Me.ShapeContainer2.TabIndex = 0
        Me.ShapeContainer2.TabStop = False
        '
        'JoystickLine
        '
        Me.JoystickLine.BorderColor = System.Drawing.Color.DodgerBlue
        Me.JoystickLine.BorderWidth = 2
        Me.JoystickLine.Enabled = False
        Me.JoystickLine.Name = "JoystickLine"
        Me.JoystickLine.X1 = 167
        Me.JoystickLine.X2 = 167
        Me.JoystickLine.Y1 = 167
        Me.JoystickLine.Y2 = 3
        '
        'JoystickCircle
        '
        Me.JoystickCircle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.JoystickCircle.BorderWidth = 3
        Me.JoystickCircle.Location = New System.Drawing.Point(3, 2)
        Me.JoystickCircle.Name = "JoystickCircle"
        Me.JoystickCircle.SelectionColor = System.Drawing.Color.White
        Me.JoystickCircle.Size = New System.Drawing.Size(323, 323)
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.PictureBox1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(689, 334)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Live Data"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(689, 334)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'ConnectCOM
        '
        Me.ConnectCOM.Location = New System.Drawing.Point(6, 18)
        Me.ConnectCOM.Name = "ConnectCOM"
        Me.ConnectCOM.Size = New System.Drawing.Size(197, 23)
        Me.ConnectCOM.TabIndex = 3
        Me.ConnectCOM.Text = "Connect"
        Me.ConnectCOM.UseVisualStyleBackColor = True
        '
        'InfoBox
        '
        Me.InfoBox.Controls.Add(Me.TargetAngle)
        Me.InfoBox.Controls.Add(Me.RunTime)
        Me.InfoBox.Controls.Add(Me.BatteryLevel)
        Me.InfoBox.Controls.Add(Me.MCU)
        Me.InfoBox.Controls.Add(Me.FWVersion)
        Me.InfoBox.Controls.Add(Me.EEPROMVersion)
        Me.InfoBox.Controls.Add(Me.AppVersion)
        Me.InfoBox.Controls.Add(Me.PID_D)
        Me.InfoBox.Controls.Add(Me.PID_I)
        Me.InfoBox.Controls.Add(Me.PID_P)
        Me.InfoBox.Controls.Add(Me.Label1)
        Me.InfoBox.Controls.Add(Me.ConnectCOM)
        Me.InfoBox.Controls.Add(Me.ShapeContainer1)
        Me.InfoBox.Location = New System.Drawing.Point(13, 12)
        Me.InfoBox.Name = "InfoBox"
        Me.InfoBox.Size = New System.Drawing.Size(209, 50)
        Me.InfoBox.TabIndex = 4
        Me.InfoBox.TabStop = False
        Me.InfoBox.Text = "Info"
        '
        'RunTime
        '
        Me.RunTime.AutoSize = True
        Me.RunTime.Location = New System.Drawing.Point(13, 147)
        Me.RunTime.Name = "RunTime"
        Me.RunTime.Size = New System.Drawing.Size(49, 13)
        Me.RunTime.TabIndex = 15
        Me.RunTime.Text = "Run Time:"
        '
        'BatteryLevel
        '
        Me.BatteryLevel.AutoSize = True
        Me.BatteryLevel.Location = New System.Drawing.Point(13, 129)
        Me.BatteryLevel.Name = "BatteryLevel"
        Me.BatteryLevel.Size = New System.Drawing.Size(68, 13)
        Me.BatteryLevel.TabIndex = 14
        Me.BatteryLevel.Text = "Battery Level:"
        '
        'MCU
        '
        Me.MCU.AutoSize = True
        Me.MCU.Location = New System.Drawing.Point(13, 111)
        Me.MCU.Name = "MCU"
        Me.MCU.Size = New System.Drawing.Size(34, 13)
        Me.MCU.TabIndex = 13
        Me.MCU.Text = "MCU:"
        '
        'EEPROMVersion
        '
        Me.EEPROMVersion.AutoSize = True
        Me.EEPROMVersion.Location = New System.Drawing.Point(13, 93)
        Me.EEPROMVersion.Name = "EEPROMVersion"
        Me.EEPROMVersion.Size = New System.Drawing.Size(90, 13)
        Me.EEPROMVersion.TabIndex = 12
        Me.EEPROMVersion.Text = "EEPROM Version:"
        '
        'FWVersion
        '
        Me.FWVersion.AutoSize = True
        Me.FWVersion.Location = New System.Drawing.Point(13, 75)
        Me.FWVersion.Name = "FWVersion"
        Me.FWVersion.Size = New System.Drawing.Size(90, 13)
        Me.FWVersion.TabIndex = 11
        Me.FWVersion.Text = "Firmware Version:"
        '
        'AppVersion
        '
        Me.AppVersion.AutoSize = True
        Me.AppVersion.Location = New System.Drawing.Point(13, 57)
        Me.AppVersion.Name = "AppVersion"
        Me.AppVersion.Size = New System.Drawing.Size(67, 13)
        Me.AppVersion.TabIndex = 10
        Me.AppVersion.Text = "App Version:"
        '
        'TargetAngle
        '
        Me.TargetAngle.AutoSize = True
        Me.TargetAngle.Location = New System.Drawing.Point(13, 252)
        Me.TargetAngle.Name = "TargetAngle"
        Me.TargetAngle.Size = New System.Drawing.Size(71, 13)
        Me.TargetAngle.TabIndex = 9
        Me.TargetAngle.Text = "Target Angle:"
        '
        'PID_D
        '
        Me.PID_D.AutoSize = True
        Me.PID_D.Location = New System.Drawing.Point(13, 234)
        Me.PID_D.Name = "PID_D"
        Me.PID_D.Size = New System.Drawing.Size(18, 13)
        Me.PID_D.TabIndex = 8
        Me.PID_D.Text = "D:"
        '
        'PID_I
        '
        Me.PID_I.AutoSize = True
        Me.PID_I.Location = New System.Drawing.Point(13, 216)
        Me.PID_I.Name = "PID_I"
        Me.PID_I.Size = New System.Drawing.Size(13, 13)
        Me.PID_I.TabIndex = 7
        Me.PID_I.Text = "I:"
        '
        'PID_P
        '
        Me.PID_P.AutoSize = True
        Me.PID_P.Location = New System.Drawing.Point(13, 198)
        Me.PID_P.Name = "PID_P"
        Me.PID_P.Size = New System.Drawing.Size(20, 13)
        Me.PID_P.TabIndex = 6
        Me.PID_P.Text = "P: "
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 175)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Current PID values"
        '
        'ShapeContainer1
        '
        Me.ShapeContainer1.Location = New System.Drawing.Point(3, 16)
        Me.ShapeContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer1.Name = "ShapeContainer1"
        Me.ShapeContainer1.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape1})
        Me.ShapeContainer1.Size = New System.Drawing.Size(203, 31)
        Me.ShapeContainer1.TabIndex = 4
        Me.ShapeContainer1.TabStop = False
        '
        'LineShape1
        '
        Me.LineShape1.Name = "LineShape1"
        Me.LineShape1.X1 = 13
        Me.LineShape1.X2 = 190
        Me.LineShape1.Y1 = 173
        Me.LineShape1.Y2 = 173
        '
        'MouseReleaseTimer
        '
        Me.MouseReleaseTimer.Interval = 150
        '
        'dataGraphTimer
        '
        Me.dataGraphTimer.Interval = 10
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1269, 368)
        Me.Controls.Add(Me.InfoBox)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.TextBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Balanduino Windows App"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.InfoBox.ResumeLayout(False)
        Me.InfoBox.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents ConnectCOM As System.Windows.Forms.Button
    Friend WithEvents InfoBox As System.Windows.Forms.GroupBox
    Friend WithEvents RunTime As System.Windows.Forms.Label
    Friend WithEvents BatteryLevel As System.Windows.Forms.Label
    Friend WithEvents MCU As System.Windows.Forms.Label
    Friend WithEvents FWVersion As System.Windows.Forms.Label
    Friend WithEvents EEPROMVersion As System.Windows.Forms.Label
    Friend WithEvents AppVersion As System.Windows.Forms.Label
    Friend WithEvents PID_D As System.Windows.Forms.Label
    Friend WithEvents PID_I As System.Windows.Forms.Label
    Friend WithEvents PID_P As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Friend WithEvents LineShape1 As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents TargetAngle As System.Windows.Forms.Label
    Friend WithEvents ShapeContainer2 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Friend WithEvents JoystickCircle As Microsoft.VisualBasic.PowerPacks.OvalShape
    Friend WithEvents JoystickLine As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents MouseReleaseTimer As System.Windows.Forms.Timer
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents dataGraphTimer As System.Windows.Forms.Timer

End Class
