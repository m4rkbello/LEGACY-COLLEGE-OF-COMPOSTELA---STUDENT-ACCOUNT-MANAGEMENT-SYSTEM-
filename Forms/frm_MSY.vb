﻿Public Class frm_MSY
    Dim inputedV As String
    Dim strQuery3 As String
    Dim strQuerry5 As String
    Dim a As Integer = 0
    Dim sy_id As Integer
    Private Sub frm_MYear_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _dbConnection("db_lccsams")
        _loadToCombobox(slctS, cbo_semester)
        _loadToListBox(slctC, lbo_courses)
        _loadToListBox(slctYL, lbo_yearL)
        _displayRecords(s_msyR, dg_syR)

    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        Select Case TabControl1.SelectedIndex
            Case 0
                _dbConnection("db_lccsams")
                _loadToCombobox(slctS, cbo_semester)
                _loadToListBox(slctC, lbo_courses)
                _loadToListBox(slctYL, lbo_yearL)
                _displayRecords(s_msyR, dg_syR)
            Case 1
                _displayRecords(eSelect_SY, dg_esyRec)
                _loadToListBox(eSelect_GL, lbo_eglRec)
            Case 2
                _displayRecords(sSelect_SY, dg_ssyRec)
                _loadToListBox(sSelect_GL, lbo_sglRec)
            Case 3
                _displayRecords(jSelect_SY, dg_jsyRec)
                _loadToListBox(jSelect_GL, lbo_jglRec)
        End Select

    End Sub

    Private Sub btn_nCourse_Click(sender As Object, e As EventArgs) Handles btn_nCourse.Click
        dlg_newCourse.ShowDialog()
        If dlg_newCourse.DialogResult = DialogResult.Yes Then
            strQuery3 = "Insert into tbl_coll_course values ('0','" & dlg_newCourse.txtb_nCourse.Text.ToUpper & "')"
            _dbConnection("db_lccsams")
            _insertData(strQuery3)
            _loadToListBox(slctC, lbo_courses)
        End If

    End Sub


    Private Sub dg_syR_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg_syR.CellClick
        Dim i = e.RowIndex
        With dg_syR
            sy_id = .Item(0, i).Value
            esy_spSTARTEND = .Item(1, i).Value.ToString.Split("-")
            txtb_syS.Text = esy_spSTARTEND(0)
            txtb_syE.Text = esy_spSTARTEND(1)

            cbo_semester.Text = .Item(2, i).Value
            Dim sd As Date = .Item(3, i).Value
            Dim ed As Date = .Item(4, i).Value
            dp_Start.Text = sd.ToString("yyyy-MM-dd")
            dp_End.Text = ed.ToString("yyyy-MM-dd")
        End With
    End Sub

    Private Sub btn_Ucourse_Click(sender As Object, e As EventArgs) Handles btn_Ucourse.Click
        dlg_updateCourse.txtb_uCourse.Text = lbo_courses.Text
        dlg_updateCourse.ShowDialog()
        If dlg_updateCourse.DialogResult = DialogResult.Yes Then
            _updateData("Update tbl_course set crs_name='" & dlg_updateCourse.txtb_uCourse.Text & "' where crs_id='" & lbo_courses.SelectedValue & "'")
            _loadToListBox(slctC, lbo_courses)
        End If
    End Sub

    Private Sub btn_nYearlevel_Click_1(sender As Object, e As EventArgs) Handles btn_nYearlevel.Click
        dlg_insertYearLevel.ShowDialog()
        If dlg_insertYearLevel.DialogResult = DialogResult.Yes Then
            strQuerry5 = "Insert into tbl_year_level values ('0','" & dlg_insertYearLevel.txtb_yearlevel.Text & "')"
            _dbConnection("db_lccsams")
            _insertData(strQuerry5)
            _loadToListBox(slctYL, lbo_yearL)
        End If

    End Sub

    Private Sub btn_ylUpate_Click_1(sender As Object, e As EventArgs) Handles btn_ylUpate.Click
        dlg_updateYearLevel.txtb_updateYL.Text = lbo_yearL.Text
        dlg_updateYearLevel.ShowDialog()

        If dlg_updateYearLevel.DialogResult = DialogResult.Yes Then
            _updateData("Update tbl_year_level  set yl_name='" & dlg_updateYearLevel.txtb_updateYL.Text & "' where yl_id='" & lbo_yearL.SelectedValue & "'")
            _loadToListBox(slctYL, lbo_yearL)
        End If
    End Sub

    Private Sub btn_updateSchY_Click(sender As Object, e As EventArgs) Handles btn_updateSchY.Click
        If btn_updateSchY.Text = "UPDATE" Then
            If txtb_syS.Text = "" Then
                MessageBox.Show("Please select school year to update")
            Else
                If dlg_updatesy.ShowDialog() = DialogResult.OK Then
                    btn_updateSchY.Text = "CANCEL"
                    txtb_syS.Enabled = True
                    txtb_syE.Enabled = True
                    cbo_semester.Enabled = True
                    btn_save.Enabled = True
                    dg_syR.Enabled = False
                    a = 2
                End If
            End If
        ElseIf btn_updateSchY.Text = "CANCEL" Then
            btn_updateSchY.Text = "UPDATE"
            txtb_syS.Enabled = False
            txtb_syE.Enabled = False
            cbo_semester.Enabled = False
            btn_save.Enabled = False
            dg_syR.Enabled = True
        End If

    End Sub

    Private Sub btn_new_Click(sender As Object, e As EventArgs) Handles btn_new.Click
        If btn_new.Text = "NEW" Then
            If dlg_addnewsy.ShowDialog = DialogResult.OK Then
                btn_new.Text = "CANCEL"
                txtb_syS.Enabled = True
                txtb_syE.Enabled = True
                btn_save.Enabled = True
                cbo_semester.Enabled = True
                a = 1
            End If
        ElseIf btn_new.Text = "CANCEL" Then
            btn_new.Text = "NEW"
            txtb_syS.Enabled = False
            txtb_syE.Enabled = False
            btn_save.Enabled = False
            cbo_semester.Enabled = False
        End If

    End Sub

    Private Sub btn_save_Click(sender As Object, e As EventArgs) Handles btn_save.Click

        Select Case a

            Case 1
                Dim syN As String = txtb_syS.Text & "-" & txtb_syE.Text
                Dim strQueery6 As String = "Insert Into tbl_sch_year values(0,'" & syN & "','" & dp_Start.Text & "','" & dp_End.Text & "','" & cbo_semester.SelectedValue & "')"
                _dbConnection("db_lccsams")
                _insertData(strQueery6)
                _displayRecords(s_msyR, dg_syR)
                If dlg_savesuccessfully.ShowDialog() = DialogResult.OK Then
                    txtb_syS.Enabled = False
                    txtb_syE.Enabled = False
                    btn_save.Enabled = False
                    cbo_semester.Enabled = False
                End If
            Case 2
                Dim syName As String = txtb_syS.Text & "-" & txtb_syE.Text
                _dbConnection("db_lccsams")
                _updateData("update tbl_sch_year  set sy_name='" & syName & "',ssy_sDate='" & dp_Start.Text & "',sy_eDate='" & dp_End.Text & "',sem_id='" & cbo_semester.SelectedValue & "' where sy_id ='" & sy_id & "' ")
                _displayRecords(s_msyR, dg_syR)
                If UpdatedSuccessfully.ShowDialog() = DialogResult.OK Then
                    txtb_syS.Enabled = False
                    txtb_syE.Enabled = False
                    btn_save.Enabled = False
                    cbo_semester.Enabled = False
                    dg_syR.Enabled = True

                End If
        End Select
    End Sub
    ''###########################################################Elementary Section ################################################################################
    Dim b As Integer = 0
    Dim esy_id As Integer
    Dim egl_name As String

    Private Sub btn_esy_New_Click(sender As Object, e As EventArgs) Handles btn_esy_New.Click

        If dlg_addnewsy.ShowDialog = DialogResult.OK Then

            txtb_esy_start.Enabled = True
                txtb_esy_end.Enabled = True
                btn_esy_save.Enabled = True
                b = 1
            End If





    End Sub

    Private Sub btn_esy_update_Click(sender As Object, e As EventArgs) Handles btn_esy_update.Click
        If dlg_updatesy.ShowDialog = DialogResult.OK Then
            btn_esy_update.Enabled = True
            txtb_esy_end.Enabled = True
            btn_esy_save.Enabled = True
            dg_esyRec.Enabled = False
            b = 2
        End If

    End Sub

    Private Sub btn_esy_save_Click(sender As Object, e As EventArgs) Handles btn_esy_save.Click
        Select Case b
            Case 1
                Dim esyN As String = txtb_esy_start.Text & "-" & txtb_esy_end.Text
                Dim strQueery6 As String = "Insert Into tbl_elem_sy values(0,'" & esyN & "','" & dp_esy_startDate.Text & "','" & dp_esy_endDate.Text & "')"
                _dbConnection("db_lccsams")
                _insertData(strQueery6)
                _displayRecords(eSelect_SY, dg_esyRec)
                If dlg_savesuccessfully.ShowDialog() = DialogResult.OK Then
                    txtb_esy_start.Enabled = False
                    txtb_esy_end.Enabled = False
                    btn_esy_save.Enabled = False


                End If
            Case 2
                Dim esyName As String = txtb_esy_start.Text & "-" & txtb_esy_end.Text
                _dbConnection("db_lccsams")
                _updateData("update tbl_elem_sy  set esy_name='" & esyName & "',esy_sdate='" & dp_esy_startDate.Text & "',esy_edate='" & dp_esy_endDate.Text & "' where esy_id ='" & esy_id & "' ")
                _displayRecords(eSelect_SY, dg_esyRec)
                If UpdatedSuccessfully.ShowDialog() = DialogResult.OK Then
                    txtb_esy_start.Enabled = False
                    txtb_esy_end.Enabled = False
                    btn_esy_save.Enabled = False
                    dg_esyRec.Enabled = True
                End If
        End Select
    End Sub

    Private Sub btn_egl_new_Click(sender As Object, e As EventArgs) Handles btn_egl_new.Click
        dlg_inputGradeLevel.ShowDialog()
        If dlg_inputGradeLevel.DialogResult = DialogResult.Yes Then
            strQuerry5 = "Insert into tbl_elem_gradelevel values ('0','" & dlg_inputGradeLevel.txtb_gradelevel.Text & "')"
            _dbConnection("db_lccsams")
            _insertData(strQuerry5)
            _loadToListBox(eSelect_GL, lbo_eglRec)
        End If
    End Sub

    Private Sub btn_egl_update_Click(sender As Object, e As EventArgs) Handles btn_egl_update.Click
        dlg_updateGradeLevel.txtb_gradelevel.Text = lbo_eglRec.Text
        dlg_updateGradeLevel.ShowDialog()
        If dlg_updateGradeLevel.DialogResult = DialogResult.Yes Then
            _updateData("Update tbl_elem_gradelevel  set egl_name='" & dlg_updateGradeLevel.txtb_gradelevel.Text & "' where egl_id='" & lbo_eglRec.SelectedValue & "'")
            _loadToListBox(eSelect_GL, lbo_eglRec)
        End If



    End Sub

    Private Sub dg_esyRec_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg_esyRec.CellClick
        Dim i = e.RowIndex
        Dim esy_spSTARTEND() As String
        With dg_esyRec
            esy_id = .Item(0, i).Value
            esy_spSTARTEND = .Item(1, i).Value.ToString.Split("-")
            txtb_esy_start.Text = esy_spSTARTEND(0)
            txtb_esy_end.Text = esy_spSTARTEND(1)
            Dim esd As Date = .Item(2, i).Value
            Dim eed As Date = .Item(3, i).Value
            dp_esy_startDate.Text = esd.ToString("yyyy-MM-dd")
            dp_esy_endDate.Text = eed.ToString("yyyy-MM-dd")
        End With
    End Sub



    '#############################################################Senior-High PUBLIC VARIABLE#########################################################
    Dim c As Integer = 0
    Dim ssy_id As Integer
    Dim sgl_name As String

    Private Sub btn_new_sSY_Click(sender As Object, e As EventArgs) Handles btn_new_sSY.Click


        If dlg_addnewsy.ShowDialog = DialogResult.OK Then

            txtb_ssy_start.Enabled = True
                txtb_ssy_end.Enabled = True
                btn_save_ssy.Enabled = True
                c = 1
            End If

    End Sub

    Private Sub btn_update_ssy_Click(sender As Object, e As EventArgs) Handles btn_update_ssy.Click

        If dlg_updatesy.ShowDialog = DialogResult.OK Then

            txtb_ssy_start.Enabled = True
            txtb_ssy_end.Enabled = True
            btn_save_ssy.Enabled = True
            dg_ssyRec.Enabled = False
            c = 2
        End If


    End Sub

    Private Sub btn_save_ssy_Click(sender As Object, e As EventArgs) Handles btn_save_ssy.Click
        Select Case c
            Case 1
                Dim ssyN As String = txtb_ssy_start.Text & "-" & txtb_ssy_end.Text
                Dim strQueery6 As String = "Insert Into tbl_seniorhigh_sy values(0,'" & ssyN & "','" & dp_ssy_sDate.Text & "','" & dp_ssy_eDate.Text & "')"
                _dbConnection("db_lccsams")
                _insertData(strQueery6)
                If dlg_savesuccessfully.ShowDialog() = DialogResult.OK Then
                    _displayRecords(sSelect_SY, dg_ssyRec)

                    txtb_ssy_start.Enabled = False
                    txtb_ssy_end.Enabled = False
                    btn_save_ssy.Enabled = False
                End If

            Case 2
                Dim ssyName As String = txtb_ssy_start.Text & "-" & txtb_ssy_end.Text
                _dbConnection("db_lccsams")
                _updateData("update tbl_seniorhigh_sy  set ssy_name='" & ssyName & "',ssy_sdate='" & dp_ssy_sDate.Text & "',ssy_edate='" & dp_ssy_eDate.Text & "' where ssy_id ='" & ssy_id & "' ")
                If UpdatedSuccessfully.ShowDialog() = DialogResult.OK Then

                    _displayRecords(sSelect_SY, dg_ssyRec)
                    txtb_ssy_start.Enabled = False
                    txtb_ssy_end.Enabled = False
                    btn_save_ssy.Enabled = False
                    dg_ssyRec.Enabled = True
                End If

        End Select
    End Sub

    Private Sub btn_new_sGL_Click(sender As Object, e As EventArgs) Handles btn_new_sGL.Click
        dlg_inputGradeLevel.ShowDialog()
        If dlg_inputGradeLevel.DialogResult = DialogResult.Yes Then
            strQuerry5 = "Insert into tbl_seniorhigh_gl values ('0','" & dlg_inputGradeLevel.txtb_gradelevel.Text & "')"
            _dbConnection("db_lccsams")
            _insertData(strQuerry5)
            _loadToListBox(sSelect_GL, lbo_sglRec)
        End If
    End Sub

    Private Sub btn_Update_sGL_Click(sender As Object, e As EventArgs) Handles btn_Update_sGL.Click
        dlg_updateGradeLevel.txtb_gradelevel.Text = lbo_sglRec.Text
        dlg_updateGradeLevel.ShowDialog()
        If dlg_updateGradeLevel.DialogResult = DialogResult.Yes Then
            _updateData("Update tbl_seniorhigh_gl  set sgl_name='" & dlg_updateGradeLevel.txtb_gradelevel.Text & "' where sgl_id='" & lbo_sglRec.SelectedValue & "'")
            _loadToListBox(sSelect_GL, lbo_sglRec)
        End If

    End Sub

    Private Sub dg_ssyRec_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg_ssyRec.CellClick
        Try
            Dim i = e.RowIndex
            Dim ssy_spSTARTEND() As String


            With dg_ssyRec
                ssy_id = .Item(0, i).Value
                ssy_spSTARTEND = .Item(1, i).Value.ToString.Split("-")

                txtb_ssy_start.Text = ssy_spSTARTEND(0)
                txtb_ssy_end.Text = ssy_spSTARTEND(1)
                Dim ssd As Date = .Item(2, i).Value
                Dim sed As Date = .Item(3, i).Value
                dp_ssy_sDate.Text = ssd.ToString("yyyy-MM-dd")
                dp_ssy_eDate.Text = sed.ToString("yyyy-MM-dd")
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    '#############################################################Junior-High PUBLIC VARIABLE#########################################################
    Dim d As Integer = 0
    Dim jsy_id As Integer
    Dim jgl_name As String
    Private Sub btn_jsy_new_Click(sender As Object, e As EventArgs) Handles btn_jsy_new.Click


        If dlg_addnewsy.ShowDialog = DialogResult.OK Then

            txtb_jsy_start.Enabled = True
                txtb_jsy_end.Enabled = True
                btn_jsy_save.Enabled = True
                d = 1
            End If

    End Sub

    Private Sub btn_jsy_update_Click(sender As Object, e As EventArgs) Handles btn_jsy_update.Click


        If dlg_updatesy.ShowDialog = DialogResult.OK Then

                txtb_jsy_start.Enabled = True
                txtb_jsy_end.Enabled = True
                btn_jsy_save.Enabled = True
                d = 2
                dg_jsyRec.Enabled = False

            End If

    End Sub

    Private Sub btn_jsy_save_Click(sender As Object, e As EventArgs) Handles btn_jsy_save.Click
        Select Case d
            Case 1
                Dim jsyN As String = txtb_jsy_start.Text & "-" & txtb_jsy_end.Text
                Dim strQueery6 As String = "Insert Into tbl_juniorhigh_sy values(0,'" & jsyN & "','" & dp_jsy_sDate.Text & "','" & dp_jsy_eDate.Text & "')"
                _dbConnection("db_lccsams")
                _insertData(strQueery6)
                If dlg_savesuccessfully.ShowDialog() = DialogResult.OK Then
                    _displayRecords(jSelect_SY, dg_jsyRec)
                    txtb_jsy_start.Enabled = False
                    txtb_jsy_end.Enabled = False
                    btn_jsy_save.Enabled = False
                End If

            Case 2
                Dim esyName As String = txtb_jsy_start.Text & "-" & txtb_jsy_end.Text
                _dbConnection("db_lccsams")
                _updateData("update tbl_juniorhigh_sy  set jsy_name='" & jsyName & "',jsy_sdate='" & dp_jsy_sDate.Text & "',jsy_edate='" & dp_jsy_eDate.Text & "' where jsy_id ='" & esy_id & "' ")
                If UpdatedSuccessfully.ShowDialog() = DialogResult.OK Then
                    _displayRecords(jSelect_SY, dg_esyRec)
                    txtb_jsy_start.Enabled = False
                    txtb_jsy_end.Enabled = False
                    btn_jsy_save.Enabled = False
                    dg_jsyRec.Enabled = True
                End If

        End Select
    End Sub

    Private Sub dg_jsyRec_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg_jsyRec.CellClick
        Try
            Dim i = e.RowIndex
            Dim jsy_spSTARTEND() As String
            With dg_jsyRec
                jsy_id = .Item(0, i).Value
                jsy_spSTARTEND = .Item(1, i).Value.ToString.Split("-")
                txtb_jsy_start.Text = jsy_spSTARTEND(0)
                txtb_jsy_end.Text = jsy_spSTARTEND(1)
                Dim jsd As Date = .Item(2, i).Value
                Dim jed As Date = .Item(3, i).Value
                dp_jsy_sDate.Text = jsd.ToString("yyyy-MM-dd")
                dp_jsy_eDate.Text = jed.ToString("yyyy-MM-dd")
            End With
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btn_jgl_new_Click(sender As Object, e As EventArgs) Handles btn_jgl_new.Click
        dlg_inputGradeLevel.ShowDialog()
        If dlg_inputGradeLevel.DialogResult = DialogResult.Yes Then
            strQuerry5 = "Insert into tbl_juniorhigh_gradelevel values ('0','" & dlg_inputGradeLevel.txtb_gradelevel.Text & "')"
            _dbConnection("db_lccsams")
            _insertData(strQuerry5)
            _loadToListBox(jSelect_GL, lbo_jglRec)
        End If
    End Sub

    Private Sub tn_jgl_update_Click(sender As Object, e As EventArgs) Handles tn_jgl_update.Click
        dlg_updateGradeLevel.txtb_gradelevel.Text = lbo_jglRec.Text
        dlg_updateGradeLevel.ShowDialog()
        If dlg_updateGradeLevel.DialogResult = DialogResult.Yes Then
            _updateData("Update tbl_juniorhigh_gradelevel  set jgl_name='" & dlg_updateGradeLevel.txtb_gradelevel.Text & "' where jgl_id='" & lbo_jglRec.SelectedValue & "'")
            _loadToListBox(jSelect_GL, lbo_jglRec)
        End If

    End Sub

    Private Sub btn_cancelC_Click(sender As Object, e As EventArgs) Handles btn_cancelC.Click
        txtb_syS.Enabled = False
        txtb_syE.Enabled = False
        btn_save.Enabled = False
        cbo_semester.Enabled = False
        dg_syR.Enabled = True
    End Sub

    Private Sub btn_cancelE_Click(sender As Object, e As EventArgs) Handles btn_cancelE.Click
        txtb_esy_start.Enabled = False
        txtb_esy_end.Enabled = False
        btn_esy_save.Enabled = False
        dg_esyRec.Enabled = True
    End Sub

    Private Sub btn_cancelS_Click(sender As Object, e As EventArgs) Handles btn_cancelS.Click
        txtb_ssy_start.Enabled = False
        txtb_ssy_end.Enabled = False
        btn_save_ssy.Enabled = False
        dg_ssyRec.Enabled = True
    End Sub

    Private Sub btn_cancelJ_Click(sender As Object, e As EventArgs) Handles btn_cancelJ.Click
        txtb_jsy_start.Enabled = False
        txtb_jsy_end.Enabled = False
        btn_jsy_save.Enabled = False
        dg_jsyRec.Enabled = True
    End Sub

    Private Sub jh_dept_Click(sender As Object, e As EventArgs) Handles jh_dept.Click

    End Sub

    Private Sub btn_manageStudentAcct_Click(sender As Object, e As EventArgs) Handles btn_manageStudentAcct.Click
        Dashboard.BunifuFlatButton6_Click(sender, e)
        manageStudentList = 2
    End Sub

    Private Sub btn_manageFees_Click(sender As Object, e As EventArgs) Handles btn_manageFees.Click
        Dashboard.BunifuFlatButton8_Click(sender, e)
        manageFees = 2
    End Sub
End Class
