﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Runtime.InteropServices;

namespace AutoCADPlug
{
    public  class SelectObjOperation
    {
        /// <summary>
        /// 类型过滤枚举类
        /// </summary>
        public enum FilterType
        {
            Curve, Dimension, Polyline, BlockRef, Circle, Line, Arc, Text, Mtext, Polyline3d, LWPOLYLINE
        }

        /// <summary>
        /// 选择单个实体
        /// </summary>
        /// <param name="message">选择提示</param>
        /// <returns>实体对象</returns>
        public static Entity SelectEntity(string message)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            PromptEntityResult ent = ed.GetEntity(message);
            if (ent.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    entity = (Entity)transaction.GetObject(ent.ObjectId, OpenMode.ForRead, true);
                    transaction.Commit();
                }
            }
            return entity;
        }

        /// <summary>
        /// 过滤选择单个实体
        /// </summary>
        /// <param name="optionsWord">过滤提示</param>
        /// <param name="optionsMessage">错误提示</param>
        /// <param name="word">选择提示</param>
        /// <param name="tp">过滤类型</param>
        /// <param name="bo">true表示不包括其基类，false则表示包括其基类</param>
        /// <returns></returns>
        public static Entity SelectEntity(string optionsWord, string optionsMessage, string word, Type tp, bool bo)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            PromptEntityOptions entops = new PromptEntityOptions(optionsWord);
            entops.SetRejectMessage(optionsMessage);
            entops.AddAllowedClass(tp, bo); //此处的true表示不包括其基类，false则表示包括其基类
            PromptEntityResult ent = ed.GetEntity(word);
            if (ent.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    entity = (Entity)transaction.GetObject(ent.ObjectId, OpenMode.ForWrite, true);
                    transaction.Commit();
                }
            }
            return entity;
        }

        /// <summary>
        /// 选择集合
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection SelectCollection()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.GetSelection();
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet SS = ents.Value;
                    foreach (ObjectId id in SS.GetObjectIds())
                    {
                        entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite, true);
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择集合
        /// </summary>
        /// <param name="tps">过滤类型</param>
        /// <returns>ObjectId数组</returns>
        public static DBObjectCollection SelectCollection(FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionOptions selops = new PromptSelectionOptions();
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            // 建立过滤器
            SelectionFilter filter = new SelectionFilter(filList);
            // 按照过滤器进行选择
            PromptSelectionResult ents = ed.GetSelection(selops, filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet SS = ents.Value;
                    foreach (ObjectId id in SS.GetObjectIds())
                    {
                        entity = (Entity)transaction.GetObject(id, OpenMode.ForWrite, true);
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择所有对象
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection SelectAll()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectAll();
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择所有对象
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection SelectAll(FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            SelectionFilter filter = new SelectionFilter(filList);
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectAll(filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择多边形内对象
        /// </summary>
        /// <param name="pc">多边形顶点</param>
        /// <returns></returns>
        public static DBObjectCollection SelectCrossingPolygon(Point3dCollection pc)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingPolygon(pc);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择多边形内对象
        /// </summary>
        /// <param name="pc">多边形顶点</param>
        /// <param name="tps">类型过滤集合</param>
        /// <returns></returns>
        public static DBObjectCollection SelectCrossingPolygon(Point3dCollection pc, FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            SelectionFilter filter = new SelectionFilter(filList);
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingPolygon(pc, filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择在窗口区域中的对象
        /// </summary>
        /// <param name="pt1">窗口角点1</param>
        /// <param name="pt2">窗口角点2</param>
        /// <returns></returns>
        public static DBObjectCollection SelectCrossingWindow(Point3d pt1, Point3d pt2)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingWindow(pt1, pt2);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 过滤选择在窗口区域中的对象
        /// </summary>
        /// <param name="pt1">窗口角点1</param>
        /// <param name="pt2">窗口角点2</param>
        /// <param name="tps">类型过滤集合</param>
        /// <returns></returns>
        public static DBObjectCollection SelectCrossingWindow(Point3d pt1, Point3d pt2, FilterType[] tps)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, tps[i].ToString());
            }
            SelectionFilter filter = new SelectionFilter(filList);
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectCrossingWindow(pt1, pt2, filter);
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 选择所有隐藏对象
        /// </summary>
        /// <returns></returns>
        public static DBObjectCollection SelectImplied()
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Entity entity = null;
            DBObjectCollection EntityCollection = new DBObjectCollection();
            PromptSelectionResult ents = ed.SelectImplied();
            if (ents.Status == PromptStatus.OK)
            {
                using (Transaction transaction = db.TransactionManager.StartTransaction())
                {
                    SelectionSet ss = ents.Value;
                    foreach (ObjectId id in ss.GetObjectIds())
                    {
                        entity = transaction.GetObject(id, OpenMode.ForWrite, true) as Entity;
                        if (entity != null)
                            EntityCollection.Add(entity);
                    }
                    transaction.Commit();
                }
            }
            return EntityCollection;
        }

        /// <summary>
        /// 将焦点转化到CAD主窗口
        /// </summary>
        public static void SelectCADWindows()
        {
            SetFocus(Application.DocumentManager.MdiActiveDocument.Window.Handle);
        }

        /// <summary>
        /// 通过XData，过滤包含字符串的实体
        /// </summary>
        /// <param name="data"></param>
        public static ObjectIdCollection SelectXdataObj(string data)
        {
            ObjectIdCollection objIds = new ObjectIdCollection();

            //获取当前文档编辑器
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            //创建 TypedValue 数组，定义过滤条件
            TypedValue[] tValue = new TypedValue[2];
            tValue.SetValue(new TypedValue((int)DxfCode.Start, "Circle"), 0);
            tValue.SetValue(new TypedValue((int)DxfCode.ExtendedDataAsciiString,
            data), 1);
            //将过滤条件赋给 SelectionFilter 对象
            SelectionFilter sFilter = new SelectionFilter(tValue);
            //请求在图形区域选择对象
            PromptSelectionResult psr;
            psr = editor.GetSelection(sFilter);
            //如果提示状态 OK，说明已选对象
            if (psr.Status == PromptStatus.OK)
            {
                SelectionSet ss = psr.Value;
                Application.ShowAlertDialog("Number of objects selected: " +
                ss.Count.ToString());

                foreach (ObjectId id in ss.GetObjectIds())
                {
                    objIds.Add(id);
                }
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }

            return objIds;
        }


















        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        private static extern int SetFocus(IntPtr hWnd);

    }
}
