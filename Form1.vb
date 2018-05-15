
Imports System.IO
Imports Newtonsoft.Json


Public Class Form1

    Private Sub CreateCSVfile(ByVal _PostID As String, _filename As String)

        Dim objWriter As IO.StreamWriter = IO.File.AppendText(_filename)
        If IO.File.Exists(_filename) Then
            objWriter.Write(_PostID)
            objWriter.Write(Environment.NewLine)
        End If
        objWriter.Close()
    End Sub

    Private Sub CreateDetailCSVfile(ByVal _PostID As String, ByVal _Title As String, ByVal _Privacy As String,
                        ByVal _Likes As String, ByVal _Views As String, ByVal _Comments As String, ByVal _Timestamp As String, ByVal _filename As String)
        Dim objWriter As IO.StreamWriter = IO.File.AppendText(_filename)
        If IO.File.Exists(_filename) Then

            objWriter.Write(_PostID & ",")
            objWriter.Write(_Title & ",")
            objWriter.Write(_Privacy & ",")
            objWriter.Write(_Likes & ",")
            objWriter.Write(_Views & ",")
            objWriter.Write(_Comments & ",")
            objWriter.Write(_Timestamp)
            objWriter.Write(Environment.NewLine)
        End If
        objWriter.Close()
    End Sub

    Private Function GetInputFile() As String

        OpenFileDialog1.Title = "Please Select The Location Of posts.csv"
        OpenFileDialog1.Filter = "CSV Files (*.csv*)|*.csv"
        Dim result As DialogResult = OpenFileDialog1.ShowDialog()
        Dim _filename As String = OpenFileDialog1.FileName
        Return _filename

    End Function

    Private Function GetOutputFile(fname As String) As String
        Dim outfile As String = Nothing
        SaveFileDialog1.Filter = "CSV Files (*.csv*)|*.csv"
        SaveFileDialog1.FileName = fname
        SaveFileDialog1.OverwritePrompt = False


        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            outfile = SaveFileDialog1.FileName
            If File.Exists(outfile) Then
                File.Delete(outfile)
            End If
        End If

        Return outfile
    End Function

    Private Sub btnTopPosts_Click(sender As Object, e As EventArgs) Handles btnTopPosts.Click
        Dim _infile As String = GetInputFile()
        MsgBox(_infile)
        Dim _outfile As String = GetOutputFile("top_posts")

        Try
            Dim tfp As New FileIO.TextFieldParser(_infile)
            tfp.Delimiters = New String() {","}
            tfp.HasFieldsEnclosedInQuotes = True
            Dim fields() As String = tfp.ReadFields

            While Not tfp.EndOfData
                fields = tfp.ReadFields
                For i As Integer = 0 To fields.Length - 1
                    fields(i) = fields(i).Replace(",", " ").Replace("""", " ")
                Next

                If fields(2) = "public" And CInt(fields(5)) > 10 And CInt(fields(4)) > 9000 And Len(fields(1)) < 40 Then
                    If RadioButton2.Checked = True Then
                        CreateDetailCSVfile(fields(0), fields(1), fields(2), fields(3), fields(4), fields(5), fields(6), _outfile)
                    ElseIf RadioButton1.Checked = True Then
                        CreateCSVfile(fields(0), _outfile)
                    End If
                End If

            End While
            tfp.Close()
            MsgBox("File '" & _outfile & "' created successfully!")

        Catch ex As Exception
            MsgBox("Incorrect input file - please select file 'posts.csv'")
            Exit Sub
        End Try

    End Sub

    Private Sub btnOtherPosts_Click(sender As Object, e As EventArgs) Handles btnOtherPosts.Click
        Dim _infile As String = GetInputFile()
        Dim _outfile As String = GetOutputFile("other_posts")

        Try
            Dim tfp As New FileIO.TextFieldParser(_infile)
            tfp.Delimiters = New String() {","}
            tfp.HasFieldsEnclosedInQuotes = True
            Dim fields() As String = tfp.ReadFields

            While Not tfp.EndOfData
                fields = tfp.ReadFields
                For i As Integer = 0 To fields.Length - 1
                    fields(i) = fields(i).Replace(",", " ").Replace("""", " ")
                Next

                If fields(2) = "public" And CInt(fields(5)) > 10 And CInt(fields(4)) > 9000 And Len(fields(1)) < 40 Then
                    'do nothing
                Else
                    If RadioButton2.Checked = True Then
                        CreateDetailCSVfile(fields(0), fields(1), fields(2), fields(3), fields(4), fields(5), fields(6), _outfile)
                    ElseIf RadioButton1.Checked = True Then
                        CreateCSVfile(fields(0), _outfile)
                    End If
                End If

            End While
            tfp.Close()
            MsgBox("File '" & _outfile & "' created successfully!")

        Catch ex As Exception
            MsgBox("Incorrect input file - please select file 'posts.csv'")
            Exit Sub
        End Try

    End Sub

    Private Sub btnDailyTop_Click(sender As Object, e As EventArgs) Handles btnDailyTop.Click

        Dim _infile As String = GetInputFile()
        Dim _outfile As String = GetOutputFile("daily_top_posts")
        Dim dt As DataTable = New DataTable()
        Dim row As DataRow

        Dim tfp As New FileIO.TextFieldParser(_infile)
        tfp.Delimiters = New String() {","}
        tfp.HasFieldsEnclosedInQuotes = True
        Dim fields() As String = tfp.ReadFields

        For Each s As String In fields
            dt.Columns.Add(New DataColumn())
        Next

        While Not tfp.EndOfData
            fields = tfp.ReadFields
            For i As Integer = 0 To fields.Length - 1
                fields(i) = fields(i).Replace(",", " ").Replace("""", " ")
            Next
            row = dt.NewRow()
            row.ItemArray = fields
            dt.Rows.Add(row)
        End While
        tfp.Close()

        Dim _MaxVal As Integer
        _MaxVal = FindMaxDataTableValue(dt)

        If RadioButton2.Checked = True Then
            For x = 0 To dt.Rows.Count - 1
                If dt.Rows(x).Item(3) = _MaxVal Then
                    CreateDetailCSVfile(dt.Rows(x).Item(0), dt.Rows(x).Item(1), dt.Rows(x).Item(2), dt.Rows(x).Item(3), dt.Rows(x).Item(4),
                    dt.Rows(x).Item(5), dt.Rows(x).Item(6), _outfile)
                End If
            Next
        ElseIf RadioButton1.Checked = True Then
            For x = 0 To dt.Rows.Count - 1
                If dt.Rows(x).Item(3) = _MaxVal Then
                    CreateCSVfile(dt.Rows(x).Item(0), _outfile)
                End If
            Next
        End If
        MsgBox("File '" & _outfile & "' created successfully!")

    End Sub

    Private Function FindMaxDataTableValue(ByRef dt As DataTable) As Integer
        Dim currentValue As Integer, maxValue As Integer
        Dim dv As DataView = dt.DefaultView
        For c As Integer = 0 To dt.Rows.Count - 1
            dv.Sort = dt.Columns(3).ColumnName + " asc"
            currentValue = CInt(dv(c).Item(3))
            If currentValue > maxValue Then maxValue = currentValue
        Next
        Return maxValue
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox1.Clear()
        Dim dt As DataTable = New DataTable()
        Dim row As DataRow
        Dim _infile As String = GetInputFile()
        Dim tfp As New FileIO.TextFieldParser(_infile)
        tfp.Delimiters = New String() {","}
        tfp.HasFieldsEnclosedInQuotes = True
        Dim fields() As String = tfp.ReadFields

        For Each s As String In fields
            dt.Columns.Add(New DataColumn())
        Next

        While Not tfp.EndOfData
            fields = tfp.ReadFields
            For i As Integer = 0 To fields.Length - 1
                fields(i) = fields(i).Replace(",", " ").Replace("""", " ")
            Next
            row = dt.NewRow()
            row.ItemArray = fields
            dt.Rows.Add(row)
        End While
        tfp.Close()

        Dim json As String = JsonConvert.SerializeObject(dt, Formatting.Indented)
        TextBox1.Text = json
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        RadioButton1.Checked = True
    End Sub


End Class
