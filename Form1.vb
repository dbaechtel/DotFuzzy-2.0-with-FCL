Imports System.IO
Imports DotFuzzy

Public Class Form1
    Dim parse As New DotFuzzy.ParseFuzzyFile

    Private Sub ParsePB_Click(sender As Object, e As EventArgs) Handles ParsePB.Click
        Dim myStream As Stream = Nothing
        Dim openFileDialog1 As New OpenFileDialog()


        openFileDialog1.InitialDirectory = "\\elyl2253\Users\e754257\Documents\Fuzzy\FCL"
        openFileDialog1.Filter = "fcl files (*.fcl)|*.fcl|All files (*.*)|*.*"
        openFileDialog1.FilterIndex = 2
        openFileDialog1.RestoreDirectory = True

        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Try
                myStream = openFileDialog1.OpenFile()
                If (myStream IsNot Nothing) Then
                    Dim filename As String = openFileDialog1.FileName
                    myStream.Close()
                    Dim Start As DateTime = DateTime.Now
                    parse.ParseFile(filename)
                    If parse.engine IsNot Nothing Then
                        Me.SavePB.Enabled = True
                        Me.LoadPB.Enabled = True

                    End If
                    Console.WriteLine(("Elapsed time = " + DateTime.Now.Subtract(Start).ToString()))

                End If

            Catch Ex As Exception
                MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
            Finally
                ' Check this again, since we need to make sure we didn't throw an exception on open.
                If (myStream IsNot Nothing) Then
                    myStream.Close()
                End If
            End Try
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.SavePB.Enabled = False
        Me.LoadPB.Enabled = True
        TempLBL.Text = TempTB.Value.ToString
        DemandLBL.Text = DemandTB.Value.ToString
    End Sub

    
    Private Sub SavePB_Click(sender As Object, e As EventArgs) Handles SavePB.Click
        Dim saveDialog As New SaveFileDialog()

        saveDialog.InitialDirectory = "\\elyl2253\Users\e754257\Documents\Fuzzy\FCL"
        saveDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
        saveDialog.FilterIndex = 2
        saveDialog.RestoreDirectory = True

        Try
            If saveDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                parse.engine.Save(saveDialog.FileName)
            End If
        Catch ex As Exception
            MessageBox.Show("Cannot save engine to disk. Original error: " & ex.Message)
        End Try

    End Sub

    Private Sub LoadPB_Click(sender As Object, e As EventArgs) Handles LoadPB.Click
        OpenFileDialog1.InitialDirectory = "\\elyl2253\Users\e754257\Documents\Fuzzy\FCL"
        OpenFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
        OpenFileDialog1.FilterIndex = 2
        OpenFileDialog1.RestoreDirectory = True

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

            Try
                Dim filename As String = OpenFileDialog1.FileName
                If IsNothing(parse.engine) Then parse.engine = New FuzzyEngine
                parse.engine.Load(filename)
                Me.SavePB.Enabled = True

            Catch Ex As Exception
                MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
            Finally
                ' Check this again, since we need to make sure we didn't throw an exception on open.
            End Try
        End If

    End Sub

    Private Sub TestPB_Click(sender As Object, e As EventArgs) Handles TestPB.Click
        CenterLBL.Text = ""
        parse.engine.Set_VAR_INPUT("temp", TempTB.Value)
        parse.engine.Set_VAR_INPUT("bias", DemandTB.Value)
        parse.engine.Fuzzify("temp")
        parse.engine.Fuzzify("bias")
        parse.engine.Inference()
        'parse.engine.DefuzzifyAll()
        Dim speed As Double = parse.engine.Defuzzify("fan") * 10
        Dim change As Double = parse.engine.Defuzzify("change")
        Dim fan As Double = Math.Min(Math.Max(speed + change, 0), 100)
        CenterLBL.Text = Math.Round(fan, 3).ToString
        TrackBar1.Value = Int(fan)
        Console.WriteLine("Testing complete.")
    End Sub

    Private Sub TempTB_Scroll(sender As Object, e As EventArgs) Handles TempTB.Scroll
        TempLBL.Text = TempTB.Value.ToString
    End Sub

    Private Sub DemandTB_Scroll(sender As Object, e As EventArgs) Handles DemandTB.Scroll
        DemandLBL.Text = DemandTB.Value.ToString
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll

    End Sub
End Class
