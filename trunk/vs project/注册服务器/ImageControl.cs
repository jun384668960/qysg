using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace register_server
{
    public static class ImageControl
    {
        /// <summary>   
        /// 设置控件区域   
        /// </summary>   
        /// <param name="control">要设置的控件</param>   
        /// <param name="bitmap">要使用的图像</param>   
        /// <remarks>Control的扩展方法</remarks>   
        public static void SetRegion(Control control, Bitmap bitmap)
        {
            // 判断是否存在控件和位图   
            if (control == null || bitmap == null)
                return;

            // 设置控件大小为位图大小   
            control.Size = bitmap.Size;
            if (control is Form)
            {   // 当控件是Form时   
                // 强制转换为Form   
                Form ImageForm = control as Form;

                // 当Form的边界FormBorderStyle不为NONE时，应将FORM的大小设置成比位图大小稍大一点   
                ImageForm.Size = control.Size;

                // 去掉边界   
                ImageForm.FormBorderStyle = FormBorderStyle.None;

                // 将位图设置成窗体背景图片   
                ImageForm.BackgroundImage = bitmap;

                // 计算位图中不透明部分的边界   
                GraphicsPath graphicsPath = CreateGraphicsPath(bitmap);

                // 应用新的区域   
                ImageForm.Region = new Region(graphicsPath);
            }
            else if (control is Button)
            {   // 当控件是Button时   
                // 强制转换为Button   
                Button ImageButton = control as Button;

                // 不显示Button Text   
                ImageButton.Text = System.String.Empty;

                // 设置Button的背景图片   
                ImageButton.BackgroundImage = bitmap;

                // 计算位图中不透明部分的边界   
                GraphicsPath graphicsPath = CreateGraphicsPath(bitmap);

                // 应用新的区域   
                ImageButton.Region = new Region(graphicsPath);
            }
        }

        /// <summary>   
        /// 创建图层路径   
        /// </summary>   
        /// <param name="bitmap">用于创建图层路径的位图</param>   
        /// <returns>图层路径</returns>   
        /// <remarks>Bitmap的扩展方法</remarks>   
        public static GraphicsPath CreateGraphicsPath(Bitmap bitmap)
        {
            return CreateGraphicsPath(bitmap, Color.Empty);
        }

        /// <summary>   
        /// 创建图层路径   
        /// </summary>   
        /// <param name="bitmap">用于创建图层路径的位图</param>   
        /// <param name="colorTransparent">透明色，如果为Color.Empty，则使用第一点作为透明色</param>   
        /// <returns>图层路径</returns>   
        /// <remarks>Bitmap的扩展方法</remarks>   
        public static GraphicsPath CreateGraphicsPath(Bitmap bitmap, Color colorTransparent)
        {
            // 创建GraphicsPath   
            GraphicsPath graphicsPath = new GraphicsPath();

            if (colorTransparent == Color.Empty)
            {   // 使用左上角第一点的颜色作为透明色   
                colorTransparent = bitmap.GetPixel(0, 0);
            }

            // 遍历所有行（Y方向）   
            for (int y = 0; y < bitmap.Height; y++)
            {
                // 遍历所有列（X方向）   
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // 如果是不需要透明处理的点则标记，然后继续偏历   
                    if (bitmap.GetPixel(x, y) != colorTransparent)
                    {
                        // 记录当前   
                        int xBegin = x;
                        int xEnd = x + 1;

                        // 从找到的不透明点开始，继续寻找不透明点,一直到找到或则达到图片宽度    
                        for (; xEnd < bitmap.Width; xEnd++)
                        {
                            if (bitmap.GetPixel(xEnd, y) == colorTransparent)
                            {
                                break;
                            }
                        }

                        // 将不透明点加到GraphicsPath
                        graphicsPath.AddRectangle(new Rectangle(xBegin, y, xEnd - xBegin, 1));
                        x = xEnd;
                    }
                }
            }

            return graphicsPath;
        }
    }  
}
