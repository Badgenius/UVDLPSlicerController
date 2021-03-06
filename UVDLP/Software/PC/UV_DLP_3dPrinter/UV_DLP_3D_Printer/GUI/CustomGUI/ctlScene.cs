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
    public partial class ctlScene : ctlAnchorable
    {
        public ctlScene()
        {
            InitializeComponent();
            UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEventDel);
            UVDLPApp.Instance().m_supportgenerator.SupportEvent += new SupportGeneratorEvent(SupEvent);
            UpdateSceneTree();
        }
        public void SupEvent(SupportEvent ev, string message, Object obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { SupEvent(ev, message, obj); }));
            }
            else
            {
                try
                {
                    switch (ev)
                    {
                        case SupportEvent.eCompleted:
                            UpdateSceneTree();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.Instance().LogError(ex.Message);
                }
            }
        }

        private void AppEventDel(eAppEvent ev, String Message) 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { AppEventDel(ev, Message); }));
            }
            else
            {
                switch (ev) 
                {
                    case eAppEvent.eObjectSelected:
                    case eAppEvent.eModelRemoved: 
                    case eAppEvent.eModelAdded:
                        UpdateSceneTree();
                        break;
                }
                Refresh();
            }
            
        }

        public override void ApplyTheme(ControlTheme ct)
        {
            base.ApplyTheme(ct);
            if (ct.ForeColor != ControlTheme.NullColor)
            {
                lblTitle.ForeColor = ct.ForeColor;
                treeScene.ForeColor = ct.ForeColor;
                treeScene.LineColor = ct.ForeColor;
            }
            if (ct.BackColor != ControlTheme.NullColor)
            {
                BackColor = ct.BackColor;
                manipObject.BackColor = ct.BackColor;
            }
            if (ct.FrameColor != ControlTheme.NullColor)
            {
                treeScene.BackColor = ct.FrameColor;
            }

        }


        #region Scene tree

        public void UpdateSceneTree()
        {
            SetupSceneTree();
        }

        private void SetupSceneTree()
        {
            treeScene.Nodes.Clear();//clear the old

            TreeNode scenenode = new TreeNode("Scene");
            treeScene.Nodes.Add(scenenode);
            TreeNode support3d = new TreeNode("3d Supports");
            treeScene.Nodes.Add(support3d);
            TreeNode selNode = null;

            foreach (Object3d obj in UVDLPApp.Instance().Engine3D.m_objects)
            {
                if (obj.tag == Object3d.OBJ_SUPPORT)
                {
                    TreeNode objnode = new TreeNode(obj.Name);
                    objnode.Tag = obj;
                    support3d.Nodes.Add(objnode);
                    if (obj == UVDLPApp.Instance().SelectedObject)  // expand this node
                    {
                        //objnode.BackColor = Color.LightBlue;
                        //treeScene.SelectedNode = objnode;
                        selNode = objnode;
                    }
                }
                else
                {
                    TreeNode objnode = new TreeNode(obj.Name);
                    objnode.Tag = obj;
                    scenenode.Nodes.Add(objnode);
                    if (obj == UVDLPApp.Instance().SelectedObject)  // expand this node
                    {
                        //objnode.BackColor = Color.LightBlue;
                        //treeScene.SelectedNode = objnode;
                        selNode = objnode;
                    }
                }
            }
            if (selNode != null)
                selNode.BackColor = Color.Green;
            scenenode.Expand();
            treeScene.SelectedNode = selNode;
        }

        private void cmdRemoveObject_Click(object sender, EventArgs e)
        {
            // delete the current selected object
            if (UVDLPApp.Instance().SelectedObject != null)
            {
                UVDLPApp.Instance().RemoveCurrentModel();
                SetupSceneTree();
            }
        }

        private void cmdRemoveAllSupports_Click(object sender, EventArgs e)
        {
            // remove all supports
            //iterate through objects, remove all supports
            UVDLPApp.Instance().RemoveAllSupports();
            SetupSceneTree();
        }

        /*
          This function does 2 things,
          * when a node is click that is an object node, it sets
          * the App current object to be the clicked object
          * when an obj in the tree view is right clicked, it shows the context menu
          */
        private void treeScene_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (e.Node.Tag != null)            
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Node.Tag != null)
            {
                UVDLPApp.Instance().SelectedObject = (Object3d)e.Node.Tag;
                // objectInfoPanel.FillObjectInfo(UVDLPApp.Instance().SelectedObject);
                SetupSceneTree();
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)  // we right clicked a menu item, check and see if it has a tag
            {
                if (e.Node.Text.Equals("3d Supports"))
                {
                    contextMenuSupport.Show(treeScene, e.Node.Bounds.Left, e.Node.Bounds.Top);
                }
                else
                {
                    if (e.Node.Tag != null)
                    {
                        contextMenuObject.Show(treeScene, e.Node.Bounds.Left, e.Node.Bounds.Top);
                    }
                }
            }
        }
            #endregion Scene tree

    }
}
