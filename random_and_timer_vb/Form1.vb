Public Class Form1
    Private ReadOnly generator As Random = New Random()
    Private ReadOnly interval As Integer = 1500
    Private ReadOnly precision As Integer = 1
    Private Shared ReadOnly defaultN As Integer = 4
    Private n_data As Integer = defaultN
    Private current_data As Integer()
    Private current_mean As Single = 0.0F

    Public Shared Function getDefaultN() As Integer
        Return defaultN
    End Function

    Private Sub InitDataAndMean()
        Me.current_data = New Integer(n_data - 1) {}
        Me.current_mean = 0.0F
    End Sub

    Private Sub GenerateDataAndComputeMean()
        Me.InitDataAndMean()

        For i As Integer = 0 To Me.n_data - 1
            Dim current_data = Me.generator.[Next](0, 100)
            Me.current_data(i) = current_data
            Me.current_mean = (Me.current_mean * i + current_data) / (i + 1)
        Next
    End Sub

    Private Sub ShowDataAndMean()
        For i As Integer = 0 To n_data - 1
            Me.RichTextBox1.Text += "(" & (i + 1) & ")" & " " & Me.current_data(i).ToString("n" & Me.precision) & vbLf
        Next

        Me.RichTextBox1.Text += "mean: " & Me.current_mean.ToString("n" & Me.precision) & vbLf & vbLf
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Timer1.Interval = interval
        Me.Timer1.Start()
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Me.RichTextBox1.Clear()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs)
    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles Timer1.Tick
        Me.GenerateDataAndComputeMean()
        Me.ShowDataAndMean()
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Timer1.[Stop]()
    End Sub

    Private Sub NumericUpDown1_ValueChanged_1(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        If Me.NumericUpDown1.Value <= 0 Then
            Me.NumericUpDown1.Value = defaultN
        Else
            Me.n_data = CInt(Me.NumericUpDown1.Value)
        End If
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Timer1.[Stop]()
        Me.GenerateDataAndComputeMean()
        Me.ShowDataAndMean()
    End Sub
End Class
