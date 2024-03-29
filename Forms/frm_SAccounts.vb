﻿Imports System.Data
Public Class frm_SAccounts

    Public Sub frm_SAccounts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _dbConnection("db_lccsams")
        _loadToCombobox(slctSY, cbo_sortSY)
        _loadToCombobox(slctS, cbo_sortSem)
        _loadToCombobox(eSelect_SY, cbo_eSortSY)
        _loadToCombobox(sSelect_SY, cbo_sSortBY)
        _loadToCombobox(jSelect_SY, cbo_jSortBY)

    End Sub

    Public Sub txtb_studAcct_ID_Click(sender As Object, e As EventArgs) Handles txtb_studAcct_ID.Click


        _dbConnection("db_lccsams")
        _shareID(txtb_studAcct_ID)
        load_stud_data()
        cbo_sortSem.Text = stud_sem
        _loadcFees()
        _displayStudAccount()
        _displayTotalForEachparticulars()
    End Sub

    Sub _loadcFees()
        Dim displayFees = "Select  f.fees_name,ft.feestype_name,f.fees_amount from tbl_coll_fees f inner join tbl_student s on s.sy_id=f.sy_id and s.yl_id=f.yl_id and s.sem_id=f.sem_id and s.crs_id=f.crs_id inner join tbl_fees_type ft on f.fees_name=ft.feestype_id  where stud_id='" & stud_id & "'"
        Dim total_fees As Double = 0
        _displayRecords(displayFees, dg_studFeesType)
        For Each rowX As DataGridViewRow In dg_studFeesType.Rows
            If rowX.Cells("fees_name").Value = 2 Then
                rowX.Cells("FeesAmount").Value = rowX.Cells("FeesAmount").Value * txtb_nUnits.Text
            End If
            total_fees += rowX.Cells("FeesAmount").Value
        Next

        txtb_totalFees.Text = total_fees
    End Sub
    Sub load_stud_data()
        Try
            txtStudentName.Text = stud_name
            txtb_sy.Text = stud_sy
            txtb_sem.Text = stud_sem
            txtb_crs.Text = stud_crs
            txtb_yl.Text = stud_yl
            txtb_nUnits.Text = stud_nUnits
            txtb_rpu.Text = stud_rpu
            Dim hasScholar1 As String = "Select sl.reference_no,st.scholar_name,sl.sl_amnt from tbl_coll_scholarlist sl inner join tbl_scholar_type st on st.scholar_id=sl.sl_name  where sl.stud_id='" & stud_id & "'"

            dbConn.Open()
            sqlCommand = New MySqlCommand(hasScholar1, dbConn)
            dr = sqlCommand.ExecuteReader
            dr.Read()
            If dr.HasRows Then
                pnl_scholar.Visible = True
                scholar_No = dr(0)
                txtb_sType.Text = dr(1)
                txtb_sAmnt.Text = dr(2)
            Else
                pnl_scholar.Visible = False

            End If

        Catch ex As Exception
        Finally
            dbConn.Close()
        End Try
    End Sub



    Private Sub cbo_sortSem_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cbo_sortSem.SelectionChangeCommitted
        _displayStudAccount()
    End Sub

    Sub _displayStudAccount()
        Dim querry1 = "Select ssy_sDate,sy_eDate from  tbl_sch_year where sy_name='" & cbo_sortSY.Text & "' and sem_id='" & cbo_sortSem.SelectedValue & "'"
        Try
            _dbConnection("db_lccsams")
            dbConn.Open()
            sqlCommand = New MySqlCommand(querry1, dbConn)
            dr = sqlCommand.ExecuteReader
            While dr.Read()
                Dim sd As Date = dr(0).ToString()
                Dim ed As Date = dr(1).ToString()
                sy_sDate = sd.ToString("yyyy-MM-dd")
                sy_eDate = ed.ToString("yyyy-MM-dd")
            End While
            dbConn.Close()
            dbConn.Open()
            Dim querry2 = "Select sa.stdacct_id,sa.stdacct_tN,sa.stdacct_tDate,ft.feestype_name,sa.stdacct_tAmount,sa.stdacct_tBalance from tbl_studaccount sa inner join tbl_fees_type ft  on ft.feestype_id=sa.stdacct_tName  where sa.stdacct_id='" & stud_id & "' and sa.stdacct_tDate between '" & sy_sDate & "' and '" & sy_eDate & "' order by sa.stdacct_tDate desc , sa.stdacct_tBalance asc"
            da = New MySqlDataAdapter(querry2, dbConn)
            dt = New DataTable
            da.Fill(dt)
            dg_acctRec.DataSource = dt
            Dim tpaidByScholar As Double = 0
            For Each r As DataRow In dt.Rows
                If Integer.Parse(r.Item(1).ToString) = scholar_No Then
                    tpaidByScholar += Double.Parse(r.Item(4).ToString)
                End If

            Next
            txtb_sPaidAmnt.Text = tpaidByScholar

        Catch ex As Exception
            erromessage("error 108: Retrieve Specific Data to DataGrid " & ex.Message)
        Finally
            dbConn.Close()
        End Try
    End Sub

    Sub _displayTotalForEachparticulars()
        Try
            Dim totalStudFees As Double = 0
            _dbConnection("db_lccsams")
            dg_displayTotalFees.Rows.Clear()
            txtb_totalFeesAmount.Clear()
            txtb_totalBal.Clear()

            Dim getFeesType As String = "Select stdacct_tName from tbl_studaccount  where stdacct_id='" & stud_id & "' group by stdacct_tName having count(*) > 0 "

            dbConn.Open()
            da = New MySqlDataAdapter(getFeesType, dbConn)
            dt = New DataTable
            da.Fill(dt)
            For Each r As DataRow In dt.Rows
                Dim displayTotalFees As String = " select ft.feestype_name,sum(stdacct_tAmount)  as totalpay from tbl_studaccount inner join tbl_fees_type ft on stdacct_tName=ft.feestype_id where stdacct_id ='" & stud_id & "' and stdacct_tName='" & Integer.Parse(r.Item(0).ToString) & "' "
                sqlCommand = New MySqlCommand(displayTotalFees, dbConn)
                dr = sqlCommand.ExecuteReader
                While dr.Read()
                    dg_displayTotalFees.Rows.Add(dr("feestype_name").ToString, dr("totalpay").ToString)
                    totalStudFees += Double.Parse(dr("totalpay").ToString)
                End While
                dr.Close()
            Next
            txtb_totalFeesAmount.Text = totalStudFees
            txtb_totalBal.Text = Double.Parse(txtb_totalFees.Text) - totalStudFees
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            dbConn.Close()
        End Try

    End Sub
    ''###########################################################Elementary Section ################################################################################

    Public Sub txtb_eStud_id_Click(sender As Object, e As EventArgs) Handles txtb_eStud_id.Click
        txtb_eStud_id.Text = eStud_id
        txtb_eStud_name.Text = eStudname
        txtb_eSY.Text = eStudSY
        txtb_eGL.Text = eStudGL
        cbo_eSortSY.Text = eStudSY
        _loadeFees()
        _displayeStudAccount()
        _display_TotalForEacheSY()

    End Sub

    Sub _loadeFees()
        Dim displayFees = "Select  f.efees_id,ft.feestype_name,f.efees_amount  from tbl_elem_fees f inner join tbl_elem_students s on s.esy_id=f.esy_id inner join tbl_fees_type ft on f.efees_name=ft.feestype_id  where s.estud_id='" & eStud_id & "'"
        Dim total_fees As Double = 0
        _displayRecords(displayFees, dg_Fees)
        For Each rowX As DataGridViewRow In dg_Fees.Rows
            total_fees += rowX.Cells(2).Value
        Next
        txtb_eTotalFees.Text = total_fees
    End Sub

    Sub _displayeStudAccount()
        Dim querry1 = "SELECT esy_sdate,esy_edate FROM tbl_elem_sy where esy_name='" & cbo_eSortSY.Text & "' "
        Try
            _dbConnection("db_lccsams")
            dbConn.Open()
            sqlCommand = New MySqlCommand(querry1, dbConn)
            dr = sqlCommand.ExecuteReader
            While dr.Read()
                Dim sd As Date = dr(0).ToString()
                Dim ed As Date = dr(1).ToString()
                esy_sDate = sd.ToString("yyyy-MM-dd")
                esy_eDate = ed.ToString("yyyy-MM-dd")
            End While
            dbConn.Close()
            dbConn.Open()
            Dim querry2 = "Select sa.estud_id,sa.etan,sa.edate_paidamount,ft.feestype_name,sa.epay_amount,sa.ebal_amount from tbl_elem_accounts sa inner join tbl_fees_type ft on sa.efees_id=ft.feestype_id  where sa.estud_id='" & eStud_id & "' and sa.edate_paidamount between '" & esy_sDate & "' and '" & esy_eDate & "' order by sa.edate_paidamount desc"
            da = New MySqlDataAdapter(querry2, dbConn)
            dt = New DataTable
            da.Fill(dt)
            dg_eStudAcctRec.DataSource = dt
        Catch ex As Exception
            erromessage("error 108: Retrieve Specific Data to DataGrid " & ex.Message)
        Finally
            dbConn.Close()
        End Try
    End Sub

    Sub _display_TotalForEacheSY()
        Try
            Dim totalStudFees As Double = 0
            _dbConnection("db_lccsams")
            dg_eStudFees.Rows.Clear()
            txtb_eStudFees_TAmount.Text = 0
            txtb_eStud_TBal.Text = 0

            Dim getFeesType As String = "Select efees_id from tbl_elem_accounts  where estud_id='" & txtb_eStud_id.Text & "' group by efees_id having count(*) > 0 "
            dbConn.Open()
            da = New MySqlDataAdapter(getFeesType, dbConn)
            dt = New DataTable
            da.Fill(dt)
            For Each r As DataRow In dt.Rows
                Dim displayTotalFees As String = " select sa.efees_id,ft.feestype_name,sum(sa.epay_amount)  as totalpay from tbl_elem_accounts sa inner join tbl_fees_type ft on sa.efees_id=ft.feestype_id where sa.estud_id ='" & eStud_id & "' and sa.efees_id='" & Integer.Parse(r.Item(0).ToString) & "' "
                sqlCommand = New MySqlCommand(displayTotalFees, dbConn)
                dr = sqlCommand.ExecuteReader
                While dr.Read()
                    dg_eStudFees.Rows.Add(dr("efees_id").ToString, dr("feestype_name").ToString, dr("totalpay").ToString)
                    totalStudFees += Double.Parse(dr("totalpay").ToString)
                End While
                dr.Close()
            Next
            txtb_eStudFees_TAmount.Text = totalStudFees
            txtb_eStud_TBal.Text = Double.Parse(txtb_eTotalFees.Text) - totalStudFees
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            dbConn.Close()
        End Try

    End Sub

    '#############################################################Senior-High Section####################################################################

    Public Sub txtb_sStud_id_Click(sender As Object, e As EventArgs) Handles txtb_sStud_id.Click
        txtb_sStud_id.Text = sStud_id
        txtb_sStud_name.Text = sStudname
        txtb_sSY.Text = sStudSY
        txtb_sGL.Text = sStudGL
        _loadsFees()
        _displaysStudAccount()
        _display_TotalForEachsSY()
    End Sub
    Sub _loadsFees()
        Dim displayFees = "Select  f.sfees_id,ft.feestype_name,f.sfees_amount  from tbl_senior_fees f inner join tbl_seniorhigh_students s on s.ssy_id=f.ssy_id  inner join tbl_fees_type ft on f.sfees_name=ft.feestype_id  where sstud_id='" & sStud_id & "'"
        Dim total_fees As Double = 0
        _displayRecords(displayFees, dg_sFeesRec)
        For Each rowX As DataGridViewRow In dg_sFeesRec.Rows
            total_fees += rowX.Cells(2).Value
        Next
        txtb_sTFees.Text = total_fees
    End Sub

    Sub _displaysStudAccount()
        Dim querry1 = "SELECT ssy_sdate,ssy_edate FROM tbl_seniorhigh_sy where ssy_name='" & cbo_sSortBY.Text & "' "
        Try
            _dbConnection("db_lccsams")
            dbConn.Open()
            sqlCommand = New MySqlCommand(querry1, dbConn)
            dr = sqlCommand.ExecuteReader
            While dr.Read()
                Dim sd As Date = dr(0).ToString()
                Dim ed As Date = dr(1).ToString()
                sSy_sDate = sd.ToString("yyyy-MM-dd")
                sSy_eDate = ed.ToString("yyyy-MM-dd")
            End While
            dbConn.Close()
            dbConn.Open()
            Dim querry2 = "Select sa.sstud_id,sa.stan,sa.sdate_paidamount,ft.feestype_name,sa.spay_amount,sa.sbal_amount from tbl_senior_accounts sa inner join tbl_fees_type ft  on ft.feestype_id=sa.sfees_id  where sa.sstud_id='" & sStud_id & "' and sa.sdate_paidamount between '" & sSy_sDate & "' and '" & sSy_eDate & "' order by sa.sdate_paidamount desc"
            da = New MySqlDataAdapter(querry2, dbConn)
            dt = New DataTable
            da.Fill(dt)
            dg_sStudAcctRec.DataSource = dt
        Catch ex As Exception
            erromessage("error 108: Retrieve Specific Data to DataGrid " & ex.Message)
        Finally
            dbConn.Close()
        End Try
    End Sub

    Sub _display_TotalForEachsSY()
        Try
            Dim totalStudFees As Double = 0
            _dbConnection("db_lccsams")
            dg_sStudFees.Rows.Clear()
            txtb_sStud_TAmount.Clear()
            txtb_sStud_Bal.Clear()

            Dim getFeesType As String = "Select sfees_id from tbl_senior_accounts  where sstud_id='" & sStud_id & "' group by sfees_id having count(*) > 0 "
            dbConn.Open()
            da = New MySqlDataAdapter(getFeesType, dbConn)
            dt = New DataTable
            da.Fill(dt)
            For Each r As DataRow In dt.Rows
                Dim displayTotalFees As String = " sfees_id,select ft.feestype_name,sum(spay_amount)  as totalpay from tbl_senior_accounts inner join tbl_fees_type ft on sfees_id=ft.feestype_id where estud_id ='" & sStud_id & "' and sfees_id='" & Integer.Parse(r.Item(0).ToString) & "' "
                sqlCommand = New MySqlCommand(displayTotalFees, dbConn)
                dr = sqlCommand.ExecuteReader
                While dr.Read()
                    dg_sStudFees.Rows.Add(dr(0).ToString, dr(1).ToString, dr(2).ToString)
                    totalStudFees += Double.Parse(dr(2).ToString)
                End While
                dr.Close()
            Next
            txtb_sStud_TAmount.Text = totalStudFees
            txtb_sStud_Bal.Text = Double.Parse(txtb_sTFees.Text) - totalStudFees
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            dbConn.Close()
        End Try

    End Sub
    '#############################################################Junior-High Section####################################################################
    Public Sub txtb_jStud_ID_Click(sender As Object, e As EventArgs) Handles txtb_jStud_ID.Click
        txtb_jStud_ID.Text = jStud_id
        txtb_jStud_name.Text = jStudname
        txtb_jSY.Text = jStudSY
        txtb_jGL.Text = jStudGL
        _loadjFees()
        _displayjStudAccount()
        _display_TotalForEachjSY()
    End Sub

    Sub _loadjFees()
        Dim displayFees = "Select  f.jfees_name ,ft.feestype_name,f.jfees_amount  from tbl_junior_fees f inner join tbl_juniorhigh_students j on j.jsy_id=f.jsy_id inner join tbl_fees_type ft on f.jfees_name=ft.feestype_id  where jstud_id='" & jStud_id & "'"
        Dim total_fees As Double = 0
        _displayRecords(displayFees, dg_jFees)
        For Each rowX As DataGridViewRow In dg_jFees.Rows
            total_fees += rowX.Cells(2).Value
        Next
        txtb_jTFees.Text = total_fees
    End Sub

    Sub _displayjStudAccount()
        Dim querry1 = "SELECT jsy_sdate,jsy_edate FROM tbl_juniorhigh_sy where jsy_name='" & cbo_jSortBY.Text & "' "
        Try
            _dbConnection("db_lccsams")
            dbConn.Open()
            sqlCommand = New MySqlCommand(querry1, dbConn)
            dr = sqlCommand.ExecuteReader
            While dr.Read()
                Dim sd As Date = dr(0).ToString()
                Dim ed As Date = dr(1).ToString()
                jSy_sDate = sd.ToString("yyyy-MM-dd")
                jSy_eDate = ed.ToString("yyyy-MM-dd")
            End While
            dbConn.Close()
            dbConn.Open()
            Dim querry2 = "Select sa.jstud_id,sa.jtan,sa.jdate_paidamount,ft.feestype_name,sa.jpay_amount,sa.jbal_amount from tbl_junior_accounts sa inner join tbl_fees_type ft  on ft.feestype_id=sa.jfees_id  where sa.jstud_id='" & jStud_id & "' and sa.jdate_paidamount between '" & jSy_sDate & "' and '" & jSy_eDate & "' order by sa.jdate_paidamount desc"
            da = New MySqlDataAdapter(querry2, dbConn)
            dt = New DataTable
            da.Fill(dt)
            dg_jStudAcctRec.DataSource = dt
        Catch ex As Exception
            erromessage("error 108: Retrieve Specific Data to DataGrid " & ex.Message)
        Finally
            dbConn.Close()
        End Try
    End Sub

    Sub _display_TotalForEachjSY()
        Try
            Dim totalStudFees As Double = 0
            _dbConnection("db_lccsams")
            dg_jStudFees.Rows.Clear()
            txtb_jStudTAmount.Clear()
            txtb_jStudTBal.Clear()

            Dim getFeesType As String = "Select jfees_id from tbl_junior_accounts  where jstud_id='" & jStud_id & "' group by jfees_id having count(*) > 0 "
            dbConn.Open()
            da = New MySqlDataAdapter(getFeesType, dbConn)
            dt = New DataTable
            da.Fill(dt)
            For Each r As DataRow In dt.Rows
                Dim displayTotalFees As String = " select ja.jfees_id,ft.feestype_name,sum(ja.jpay_amount)  as totalpay from tbl_junior_accounts ja inner join tbl_fees_type ft on ja.jfees_id=ft.feestype_id where ja.jstud_id ='" & jStud_id & "' and ja.jfees_id='" & Integer.Parse(r.Item(0).ToString) & "' "
                sqlCommand = New MySqlCommand(displayTotalFees, dbConn)
                dr = sqlCommand.ExecuteReader
                While dr.Read
                    dg_jStudFees.Rows.Add(dr(0).ToString, dr(1).ToString)
                    totalStudFees += Double.Parse(dr(1).ToString)
                End While
                dr.Close()
            Next
            txtb_jStudTAmount.Text = totalStudFees
            txtb_jStudTBal.Text = Double.Parse(txtb_jTFees.Text) - totalStudFees
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            dbConn.Close()
        End Try

    End Sub


    Private Sub dg_acctRec_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg_acctRec.CellDoubleClick
        Dim i = e.RowIndex
        With dg_acctRec

        End With
    End Sub


    Private Sub cbo_eSortSY_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cbo_eSortSY.SelectionChangeCommitted
        _displayeStudAccount()
    End Sub

    Private Sub cbo_sSortBY_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cbo_sSortBY.SelectionChangeCommitted
        _displaysStudAccount()
    End Sub

    Private Sub cbo_jSortBY_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cbo_jSortBY.SelectionChangeCommitted
        _displayjStudAccount()
    End Sub


End Class
