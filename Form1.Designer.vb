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
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ParsePB = New System.Windows.Forms.Button()
        Me.SavePB = New System.Windows.Forms.Button()
        Me.LoadPB = New System.Windows.Forms.Button()
        Me.TestPB = New System.Windows.Forms.Button()
        Me.CenterLBL = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TrackBar1 = New System.Windows.Forms.TrackBar()
        Me.DemandTB = New System.Windows.Forms.TrackBar()
        Me.TempTB = New System.Windows.Forms.TrackBar()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TempLBL = New System.Windows.Forms.Label()
        Me.DemandLBL = New System.Windows.Forms.Label()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DemandTB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TempTB, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'ParsePB
        '
        Me.ParsePB.Location = New System.Drawing.Point(271, 326)
        Me.ParsePB.Name = "ParsePB"
        Me.ParsePB.Size = New System.Drawing.Size(75, 23)
        Me.ParsePB.TabIndex = 0
        Me.ParsePB.Text = "Parse File"
        Me.ParsePB.UseVisualStyleBackColor = True
        '
        'SavePB
        '
        Me.SavePB.Enabled = False
        Me.SavePB.Location = New System.Drawing.Point(27, 326)
        Me.SavePB.Name = "SavePB"
        Me.SavePB.Size = New System.Drawing.Size(75, 23)
        Me.SavePB.TabIndex = 1
        Me.SavePB.Text = "Save"
        Me.SavePB.UseVisualStyleBackColor = True
        '
        'LoadPB
        '
        Me.LoadPB.Location = New System.Drawing.Point(150, 326)
        Me.LoadPB.Name = "LoadPB"
        Me.LoadPB.Size = New System.Drawing.Size(75, 23)
        Me.LoadPB.TabIndex = 2
        Me.LoadPB.Text = "Load"
        Me.LoadPB.UseVisualStyleBackColor = True
        '
        'TestPB
        '
        Me.TestPB.Location = New System.Drawing.Point(389, 326)
        Me.TestPB.Name = "TestPB"
        Me.TestPB.Size = New System.Drawing.Size(75, 23)
        Me.TestPB.TabIndex = 3
        Me.TestPB.Text = "Test"
        Me.TestPB.UseVisualStyleBackColor = True
        '
        'CenterLBL
        '
        Me.CenterLBL.AutoSize = True
        Me.CenterLBL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.CenterLBL.Location = New System.Drawing.Point(556, 247)
        Me.CenterLBL.MinimumSize = New System.Drawing.Size(75, 20)
        Me.CenterLBL.Name = "CenterLBL"
        Me.CenterLBL.Size = New System.Drawing.Size(75, 20)
        Me.CenterLBL.TabIndex = 4
        Me.CenterLBL.Text = "   "
        Me.CenterLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(509, 250)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Center:"
        '
        'TrackBar1
        '
        Me.TrackBar1.Location = New System.Drawing.Point(68, 238)
        Me.TrackBar1.Maximum = 100
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(417, 45)
        Me.TrackBar1.TabIndex = 6
        '
        'DemandTB
        '
        Me.DemandTB.Location = New System.Drawing.Point(68, 187)
        Me.DemandTB.Maximum = 100
        Me.DemandTB.Minimum = -100
        Me.DemandTB.Name = "DemandTB"
        Me.DemandTB.Size = New System.Drawing.Size(430, 45)
        Me.DemandTB.TabIndex = 7
        '
        'TempTB
        '
        Me.TempTB.Location = New System.Drawing.Point(68, 136)
        Me.TempTB.Maximum = 110
        Me.TempTB.Minimum = -10
        Me.TempTB.Name = "TempTB"
        Me.TempTB.Size = New System.Drawing.Size(430, 45)
        Me.TempTB.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(509, 136)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(37, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Temp:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(509, 187)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Bias:"
        '
        'TempLBL
        '
        Me.TempLBL.AutoSize = True
        Me.TempLBL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TempLBL.Location = New System.Drawing.Point(556, 136)
        Me.TempLBL.MinimumSize = New System.Drawing.Size(75, 20)
        Me.TempLBL.Name = "TempLBL"
        Me.TempLBL.Size = New System.Drawing.Size(75, 20)
        Me.TempLBL.TabIndex = 11
        Me.TempLBL.Text = "   "
        Me.TempLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DemandLBL
        '
        Me.DemandLBL.AutoSize = True
        Me.DemandLBL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DemandLBL.Location = New System.Drawing.Point(556, 183)
        Me.DemandLBL.MinimumSize = New System.Drawing.Size(75, 20)
        Me.DemandLBL.Name = "DemandLBL"
        Me.DemandLBL.Size = New System.Drawing.Size(75, 20)
        Me.DemandLBL.TabIndex = 12
        Me.DemandLBL.Text = "   "
        Me.DemandLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(643, 361)
        Me.Controls.Add(Me.DemandLBL)
        Me.Controls.Add(Me.TempLBL)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TempTB)
        Me.Controls.Add(Me.DemandTB)
        Me.Controls.Add(Me.TrackBar1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.CenterLBL)
        Me.Controls.Add(Me.TestPB)
        Me.Controls.Add(Me.LoadPB)
        Me.Controls.Add(Me.SavePB)
        Me.Controls.Add(Me.ParsePB)
        Me.Name = "Form1"
        Me.Text = "Test DotFuzzy"
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DemandTB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TempTB, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents ParsePB As Button
    Friend WithEvents SavePB As System.Windows.Forms.Button
    Friend WithEvents LoadPB As System.Windows.Forms.Button
    Friend WithEvents TestPB As System.Windows.Forms.Button
    Friend WithEvents CenterLBL As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
    Friend WithEvents DemandTB As System.Windows.Forms.TrackBar
    Friend WithEvents TempTB As System.Windows.Forms.TrackBar
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TempLBL As System.Windows.Forms.Label
    Friend WithEvents DemandLBL As System.Windows.Forms.Label
End Class
