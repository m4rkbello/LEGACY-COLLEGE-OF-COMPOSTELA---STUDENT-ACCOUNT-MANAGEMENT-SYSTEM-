﻿Imports System.Windows.Forms

Public Class dlg_fillup

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub dlg_addsdnt_yes_Click(sender As Object, e As EventArgs) Handles dlg_addsdnt_yes.Click
        Close()
    End Sub
End Class
