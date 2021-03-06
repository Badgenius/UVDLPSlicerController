﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine3D;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlMove : ctlUserPanel
    {
        public ctlMove()
        {
            InitializeComponent();
        }


        protected void MoveObject(ctlTextBox var, float x, float y, float z)
        {
            try
            {
                if (UVDLPApp.Instance().SelectedObject == null)
                    return;
                float val = var.FloatVal;
                x *= val;
                y *= val;
                z *= val;
                UVDLPApp.Instance().SelectedObject.Translate(x, y, z);
                UVDLPApp.Instance().m_undoer.SaveTranslation(UVDLPApp.Instance().SelectedObject, x, y, z);
                UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
                //ShowObjectInfo();
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void buttXMinus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveX, -1, 0, 0);
        }

        private void buttYMinus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveY, 0, -1, 0);
        }

        private void buttZMinus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveZ, 0, 0, -1);
        }

        private void buttXPlus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveX, 1, 0, 0);
        }

        private void buttYPlus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveY, 0, 1, 0);
        }

        private void buttZPlus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveZ, 0, 0, 1);
        }

        private void buttCenter_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null) return;
            Point3d center = UVDLPApp.Instance().SelectedObject.CalcCenter();
            UVDLPApp.Instance().SelectedObject.Translate((float)-center.x, (float)-center.y, (float)-center.z);
            UVDLPApp.Instance().m_undoer.SaveTranslation(UVDLPApp.Instance().SelectedObject, (float)-center.x, (float)-center.y, (float)-center.z);
            UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
        }

        private void buttOnPlatform_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null)
                return;
            Point3d center = UVDLPApp.Instance().SelectedObject.CalcCenter();
            UVDLPApp.Instance().SelectedObject.FindMinMax();
            float zlev = (float)UVDLPApp.Instance().SelectedObject.m_min.z;
            float epsilon = .05f; // add in a the level of 1 slice
            float zmove = -zlev - epsilon;
            //UVDLPApp.Instance().SelectedObject.Translate((float)0, (float)0, (float)-zlev);
            //UVDLPApp.Instance().SelectedObject.Translate((float)0, (float)0, (float)-epsilon);
            UVDLPApp.Instance().SelectedObject.Translate(0, 0, zmove);
            UVDLPApp.Instance().m_undoer.SaveTranslation(UVDLPApp.Instance().SelectedObject, 0, 0, zmove);
            UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
        }

        public override void ApplyTheme(ControlTheme ct)
        {
            base.ApplyTheme(ct);
            if (ct.ForeColor != ControlTheme.NullColor)
            {
                label8.ForeColor = ct.ForeColor;
                textMoveX.ForeColor = ct.ForeColor;
                textMoveY.ForeColor = ct.ForeColor;
                textMoveZ.ForeColor = ct.ForeColor;
            }
            if (ct.BackColor != ControlTheme.NullColor)
            {
                BackColor = ct.BackColor;
                flowLayoutPanel2.BackColor = ct.BackColor;
                textMoveX.BackColor = ct.BackColor;
                textMoveY.BackColor = ct.BackColor;
                textMoveZ.BackColor = ct.BackColor;
            }
            if (ct.FrameColor != ControlTheme.NullColor)
            {
                flowLayoutPanel1.BackColor = ct.FrameColor;
                flowLayoutPanel7.BackColor = ct.FrameColor;
                flowLayoutPanel8.BackColor = ct.FrameColor;
                flowLayoutPanel10.BackColor = ct.FrameColor;
            }

        }

    }
}
