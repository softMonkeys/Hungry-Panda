using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hungry_Panda
{
    static class Common
    {
        //static ui references
//        static Dictionary<string, FrameworkElement> ancestors;
        static Dictionary<string, Point> offsets;
//        static Dictionary<string, UserControl> controls;
        static Dictionary<string, TranslateTransform> transforms;
        //ui state data
//        static Dictionary<string, Point> positions;
//        static bool mouseMoved;


        //data for current drag action
        static Point clickAnchor;
        static bool LeftPressed;
        static public bool dragging;
//        static Point maxOffset;
        static Point lastPos;

        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static void updateOffset(Point p, FrameworkElement view)
        {
            if (offsets != null && offsets.ContainsKey(view.Uid))
                offsets[view.Uid] = p;
        }

        public static TranslateTransform getTransform(string Uid)
        {
            if (!transforms.ContainsKey(Uid))
            {
                transforms.Add(Uid, new TranslateTransform());
            }
            return transforms[Uid];
        }

        public static bool TransformIsRegistered(FrameworkElement view)
        {
            return transforms != null && transforms.ContainsKey(view.Uid);
        }

        public static void registerTransformNamed<T>(Point maxOffset, UserControl source, string name)
            where T : FrameworkElement
        {
            if (!TransformIsRegistered(source))
                Common.registerTransform(maxOffset, source);
        }

        public static void registerTransform(Point maxOffset, UserControl source)
        {
            if (offsets == null)
                offsets = new Dictionary<string, Point>();
            if (transforms == null)
                transforms = new Dictionary<string, TranslateTransform>();
            if (!transforms.ContainsKey(source.Uid))
            {
                Trace.WriteLine(string.Format("registering object wth Uid {0}", source.Uid));
                offsets.Add(source.Uid, maxOffset);
                if (transforms.ContainsKey(source.Uid))
                    transforms[source.Uid] = new TranslateTransform();
                else
                    transforms.Add(source.Uid, new TranslateTransform());
            }
        }

        public static void registerTransform(Point maxOffset, string uid)
        {
            if (offsets == null)
                offsets = new Dictionary<string, Point>();
            if (transforms == null)
                transforms = new Dictionary<string, TranslateTransform>();
            if (!transforms.ContainsKey(uid))
            {
                Trace.WriteLine(string.Format("registering object wth Uid {0}", uid));
                offsets.Add(uid, maxOffset);
                if (transforms.ContainsKey(uid))
                    transforms[uid] = new TranslateTransform();
                else
                    transforms.Add(uid, new TranslateTransform());
            }
        }

        public static TranslateTransform getTransform(UserControl view)
        {
            if (transforms.ContainsKey(view.Uid))
                return transforms[view.Uid];
            else
                return null;
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T TryFindAncestor<T>(this DependencyObject child)
            where T : DependencyObject
        {//http://www.hardcodet.net/2008/02/find-wpf-parent
            //get parent item
            DependencyObject parentObject = GetAncestorObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindAncestor<T>(parentObject);
            }
        }

        public static T TryFindNamedAncestor<T>(this DependencyObject child, string name)
            where T : DependencyObject
        {//http://www.hardcodet.net/2008/02/find-wpf-parent
            //get parent item
            DependencyObject parentObject = GetAncestorObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null && ((FrameworkElement)parentObject).Name == name)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindNamedAncestor<T>(parentObject,name);
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject GetAncestorObject(this DependencyObject child)
        {//http://www.hardcodet.net/2008/02/find-wpf-parent
            if (child == null) return null;

            //handle content elements separately
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }
        /// <summary>
        /// Tries to locate a given item within the visual tree,
        /// starting with the dependency object at a given position. 
        /// </summary>
        /// <typeparam name="T">The type of the element to be found
        /// on the visual tree of the element at the given location.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        public static T TryFindAncestorFromPoint<T>(UIElement reference, Point point)
          where T : DependencyObject
        {//http://www.hardcodet.net/2008/02/find-wpf-parent
            DependencyObject element = reference.InputHitTest(point)
                                         as DependencyObject;
            if (element == null) return null;
            else if (element is T) return (T)element;
            else return TryFindAncestor<T>(element);
        }

        public static T TryFindNamedAncestorFromPoint<T>(UIElement reference, Point point, string name)
          where T : DependencyObject
        {//http://www.hardcodet.net/2008/02/find-wpf-parent
            DependencyObject element = reference.InputHitTest(point)
                                         as DependencyObject;
            if (element == null) return null;
            else if (element is T && ((FrameworkElement)element) == null) return null;
            else if (element is T && ((FrameworkElement)element).Name == name) return (T)element;
            else return TryFindNamedAncestor<T>(element,name);
        }

        internal static void Control_GotFocus(object sender, RoutedEventArgs e, ViewMainWindowTemplate viewMainWindowTemplate)
        {
            throw new NotImplementedException();
        }

        internal static void Control_LostFocus(object sender, RoutedEventArgs e, ViewMainWindowTemplate viewMainWindowTemplate)
        {
            throw new NotImplementedException();
        }

        public static DependencyObject GetAncestorFromPoint(UIElement reference, Point point)
        {//http://www.hardcodet.net/2008/02/find-wpf-parent
            DependencyObject element = reference.InputHitTest(point)
                                         as DependencyObject;
            if (element == null) return null;
            else return GetAncestorObject(element);
        }

        public static void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e, FrameworkElement view)
        {
            if (LeftPressed) return;//already set this stuff, don't reset
            LeftPressed = true;
            clickAnchor = Mouse.GetPosition(view);
            dragging = false;
//            Trace.WriteLine("down: leftPressed, clickAnchored, not dragging");
        }

        public static void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e, FrameworkElement view, FrameworkElement ancestor)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
            LeftPressed = false;
            clickAnchor = new Point(0, 0);
            if (e.Handled = dragging)//probably also captured!
            {
                dragging = false;
                //ancestor = ancestors.GetValue(view.Uid, null);
                //                Trace.WriteLine(string.Format("release capture         mouseLeftUp object wth Uid {0}", view.Uid));
                if (ancestor != null && ancestor.IsMouseCaptured)
                    ancestor.ReleaseMouseCapture();
                //                Trace.WriteLine(string.Format("clearing drag state info"));
                clickAnchor = new Point();
                ancestor = null;
                lastPos = new Point();
            }
            Trace.WriteLine(string.Format("Common.MouseUp handled? {0}", e.Handled));
        }
        public static void Control_Leave(object sender, RoutedEventArgs e, FrameworkElement view, FrameworkElement ancestor)
        {
//            Trace.WriteLine(string.Format("lost focus called"));
            LeftPressed = false;
            clickAnchor = new Point(0, 0);
            dragging = false;
//            Trace.WriteLine(string.Format("release capture         Focus Lost object wth Uid {0}", view.Uid));
            clickAnchor = new Point();
            ancestor = null;
            lastPos = new Point();
            if (ancestor != null && ancestor.IsMouseCaptured)
                ancestor.ReleaseMouseCapture();
        }
        public static void Control_MouseMove(object sender, MouseEventArgs e, FrameworkElement view, FrameworkElement ancestor)
        {//https://stackoverflow.com/questions/1495408/how-to-drag-a-usercontrol-inside-a-canvas
//            Trace.Write("Move: ");
            if (!LeftPressed)
            {
//                Trace.WriteLine("not leftPressed");
                return;//not attempting a drag currently;
            }
            Point currentPos = Mouse.GetPosition(view);
            if (!dragging && (Math.Abs(clickAnchor.X - currentPos.X) + Math.Abs(clickAnchor.Y - currentPos.Y) > 4))
            {
                dragging = true;
                lastPos.X = clickAnchor.X;
                lastPos.Y = clickAnchor.Y;
//                Trace.Write("dragging, leftPressed");
            }
            else if (!dragging)
            {
//                Trace.WriteLine("not dragging");
                return;//have not moved enough
            }
            
            //assume we've been registered;
            if (offsets == null)//fresh drag move, retrieve ancestor info;
                return;
            if (!offsets.ContainsKey(view.Uid)) return;
            Point maxOffset = offsets[view.Uid];
            if (maxOffset.Y + ancestor.DesiredSize.Height < 0 || maxOffset.X + ancestor.DesiredSize.Height < 0)
                return;//too small to move
            if (maxOffset == null) return;
            if (transforms == null)return;
            TranslateTransform translate = transforms[view.Uid];
            if (maxOffset.X == -1 || maxOffset.Y == -1)
            {
                maxOffset = offsets[view.Uid];
                if (maxOffset == null) return;
            }
            if (lastPos.X + lastPos.Y - currentPos.X - currentPos.Y == 0)
            {
//                Trace.WriteLine(String.Format(", LastPos({0},{1}) == CurrentPos({2},{3}), do nothing",lastPos.X,lastPos.Y,currentPos.X,currentPos.Y));
                return;//haven't changed since last visit;  will be same on very first visit as well most likely
            }

            //doMove - if here, we can complete the action now;
            translate.Y = Math.Max(maxOffset.Y, Math.Min(0, translate.Y + currentPos.Y - lastPos.Y));
            translate.X = Math.Max(maxOffset.X, Math.Min(0, translate.X + currentPos.X - lastPos.X));
            ancestor.RenderTransform = translate;
            lastPos.X = currentPos.X;
            lastPos.Y = currentPos.Y;
        }
    }
}
