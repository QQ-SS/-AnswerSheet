using System;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;

/// <summary>
/// 图像处理工具类
/// https://www.psvmc.cn/article/2022-07-18-opencv-csharp.html
/// </summary>
public static partial class CvUtils
{
    static int showStep = 0;

    public static Mat Show(this Mat source, string? title = null)
    {
        showStep++;
        var name = title ?? $"{showStep}";
        Cv2.NamedWindow(name, WindowFlags.KeepRatio);
        Cv2.ImShow(name, source);
        Cv2.ResizeWindow(name, new Size(source.Width / 4, source.Height / 4));
        Console.WriteLine($"{name} {showStep} 宽 * 高：{source.Cols} * {source.Rows} 通道数：{source.Channels()}");
        return source;
    }

    /// <summary>
    /// 灰度
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat Gray(this Mat source)
    {
        Mat resultMat = new();
        Cv2.CvtColor(source, resultMat, ColorConversionCodes.BGR2GRAY);
        return resultMat;
    }

    /// <summary>
    /// 二值化
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat Binary(this Mat source)
    {
        Mat resultMat = new();
        Cv2.Threshold(source, resultMat, 200, 255, ThresholdTypes.Binary);
        return resultMat;
    }

    /// <summary>
    /// 通过形态学闭运算将二维码区域连城一片，以便后续轮廓查找
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat Morphology(this Mat source, int width = 5)
    {
        Mat resultMat = new(source.Rows, source.Cols, source.Type());
        Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(width, width), new Point(-1, -1));
        Cv2.MorphologyEx(source, resultMat, MorphTypes.Close, element, new Point(-1, -1), 1, BorderTypes.Default, new Scalar());
        return resultMat;
    }

    /// <summary>
    /// 膨胀
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat Dilation(this Mat source)
    {
        Mat resultMat = new(source.Rows, source.Cols, source.Type());
        Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
        Cv2.Dilate(source, resultMat, element);
        return resultMat;
    }

    /// <summary>
    /// 腐蚀
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat Eroding(this Mat source)
    {
        Mat resultMat = new(source.Rows, source.Cols, source.Type());
        Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
        Cv2.Erode(source, resultMat, element);
        return resultMat;
    }

    /// <summary>
    /// 反转
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat BitwiseNot(this Mat source)
    {
        Mat resultMat = new();
        Cv2.BitwiseNot(source, resultMat, new Mat());
        return resultMat;
    }

    /// <summary>
    /// 美颜磨皮 双边滤波
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat BilateralFilter(this Mat source)
    {
        Mat resultMat = new();
        Cv2.BilateralFilter(source, resultMat, 15, 35d, 35d);
        return resultMat;
    }

    /// <summary>
    /// 高斯模糊
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat GaussianBlur(this Mat source)
    {
        Mat resultMat = new();
        Cv2.GaussianBlur(source, resultMat, new OpenCvSharp.Size(11, 11), 4, 4);
        return resultMat;
    }

    /// <summary>
    /// 图片缩放
    /// </summary>
    /// <param name="source"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Mat Resize(this Mat source, int width, int height)
    {
        Mat resultMat = new();
        Cv2.Resize(source, resultMat, new Size(width, height));
        return resultMat;
    }

    /// <summary>
    /// 图片缩放
    /// </summary>
    /// <param name="source"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Mat Resize(this Mat source, int width)
    {
        Mat resultMat = new();
        var height = source.Rows * width / source.Cols;
        Cv2.Resize(source, resultMat, new Size(width, height));
        return resultMat;
    }

    /// <summary>
    /// 图片缩放
    /// </summary>
    /// <param name="source"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Mat Resize(this Mat source, double scale)
    {
        Mat resultMat = new();
        Cv2.Resize(source, resultMat, new OpenCvSharp.Size(), scale, scale, InterpolationFlags.Linear);
        return resultMat;
    }

    /// <summary>
    /// 旋转
    /// </summary>
    /// <param name="source"></param>
    /// <param name="angle">角度</param>
    /// <returns></returns>
    public static Mat Rotate(this Mat source, double angle = 90)
    {
        Mat resultMat = new();
        Point center = new(source.Cols / 2, source.Rows / 2); //旋转中心
        double scale = 1.0; //缩放系数
        Mat rotMat = Cv2.GetRotationMatrix2D(center, angle, scale);
        Cv2.WarpAffine(source, resultMat, rotMat, source.Size());

        return resultMat;
    }

    /// <summary>
    /// 逆时针旋转90
    /// </summary>
    public static Mat Rotate90Counter(this Mat source)
    {
        Mat resultMat = new();
        Cv2.Rotate(source, resultMat, RotateFlags.Rotate90Counterclockwise);
        return resultMat;
    }

    /// <summary>
    /// 顺时针旋转90
    /// </summary>
    public static Mat Rotate90(this Mat source)
    {
        Mat resultMat = new();
        Cv2.Rotate(source, resultMat, RotateFlags.Rotate90Clockwise);
        return resultMat;
    }

    /// <summary>
    /// 旋转180
    /// </summary>
    public static Mat Rotate180(this Mat source)
    {
        Mat resultMat = new();
        Cv2.Rotate(source, resultMat, RotateFlags.Rotate180);
        return resultMat;
    }

    /// <summary>
    /// 获取四个顶点
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Point[] GetAllPoints(this Mat source)
    {
        Point[] potArr = new Point[4];
        for (int i = 0; i < 4; i++)
        {
            potArr[i] = new Point(-1, -1);
        }
        // 距离四个角的距离
        int[] spaceArr = [-1, -1, -1, -1];
        int cols = source.Cols;
        int rows = source.Rows;
        int x1 = cols / 3;
        int x2 = cols * 2 / 3;
        int y1 = rows / 3;
        int y2 = rows * 2 / 3;
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (x > x1 && x < x2 && y > y1 && y < y2)
                {
                    continue;
                }

                Vec3b color = source.Get<Vec3b>(y, x);

                if (color.Item0 == 0)
                {
                    if (spaceArr[0] == -1)
                    {
                        potArr[0].X = x;
                        potArr[0].Y = y;
                        potArr[1].X = x;
                        potArr[1].Y = y;
                        potArr[2].X = x;
                        potArr[2].Y = y;
                        potArr[3].X = x;
                        potArr[3].Y = y;
                        spaceArr[0] = GetSpace(0, 0, x, y);
                        spaceArr[1] = GetSpace(cols, 0, x, y);
                        spaceArr[2] = GetSpace(cols, rows, x, y);
                        spaceArr[3] = GetSpace(0, rows, x, y);
                    }
                    else
                    {
                        int s0 = GetSpace(0, 0, x, y);
                        int s1 = GetSpace(cols, 0, x, y);
                        int s2 = GetSpace(cols, rows, x, y);
                        int s3 = GetSpace(0, rows, x, y);
                        if (s0 < spaceArr[0])
                        {
                            spaceArr[0] = s0;
                            potArr[0].X = x;
                            potArr[0].Y = y;
                        }
                        if (s1 < spaceArr[1])
                        {
                            spaceArr[1] = s1;
                            potArr[1].X = x;
                            potArr[1].Y = y;
                        }
                        if (s2 < spaceArr[2])
                        {
                            spaceArr[2] = s2;
                            potArr[2].X = x;
                            potArr[2].Y = y;
                        }
                        if (s3 < spaceArr[3])
                        {
                            spaceArr[3] = s3;
                            potArr[3].X = x;
                            potArr[3].Y = y;
                        }
                    }
                }
            }
        }
        return potArr;
    }

    /// <summary>
    /// 获取四个顶点(优化版本)
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Point[] GetAnchorPointArr(this Mat source)
    {
        // 忽略周围的像素
        Point[] potArr = new Point[4];
        for (int i = 0; i < 4; i++)
        {
            potArr[i] = new Point(-1, -1);
        }
        // 距离四个角的距离
        //左上 右上 右下  左下
        int[] spaceArr = [-1, -1, -1, -1];
        int rows = source.Rows;
        int cols = source.Cols;
        //获取定位块
        Point[][] pointAll = FindContours(source);
        List<Point[]> rectVec2 = [];
        foreach (Point[] pointArr in pointAll)
        {
            if (IsSquare(pointArr))
            {
                rectVec2.Add(pointArr);
            }
        }
        Mat img7 = new(source.Rows, source.Cols, MatType.CV_8UC3, new Scalar(255, 255, 255));
        Cv2.DrawContours(img7, rectVec2, -1, new Scalar(0, 0, 255));
        foreach (Point[] points in rectVec2)
        {
            foreach (Point point in points)
            {
                int x = point.X;
                int y = point.Y;
                if (spaceArr[0] == -1)
                {
                    potArr[0].X = x;
                    potArr[0].Y = y;
                    potArr[1].X = x;
                    potArr[1].Y = y;
                    potArr[2].X = x;
                    potArr[2].Y = y;
                    potArr[3].X = x;
                    potArr[3].Y = y;
                    spaceArr[0] = GetSpace(0, 0, x, y);
                    spaceArr[1] = GetSpace(cols, 0, x, y);
                    spaceArr[2] = GetSpace(cols, rows, x, y);
                    spaceArr[3] = GetSpace(0, rows, x, y);
                }
                else
                {
                    int s0 = GetSpace(0, 0, x, y);
                    int s1 = GetSpace(cols, 0, x, y);
                    int s2 = GetSpace(cols, rows, x, y);
                    int s3 = GetSpace(0, rows, x, y);
                    if (s0 < spaceArr[0])
                    {
                        spaceArr[0] = s0;
                        potArr[0].X = x;
                        potArr[0].Y = y;
                    }
                    if (s1 < spaceArr[1])
                    {
                        spaceArr[1] = s1;
                        potArr[1].X = x;
                        potArr[1].Y = y;
                    }
                    if (s2 < spaceArr[2])
                    {
                        spaceArr[2] = s2;
                        potArr[2].X = x;
                        potArr[2].Y = y;
                    }
                    if (s3 < spaceArr[3])
                    {
                        spaceArr[3] = s3;
                        potArr[3].X = x;
                        potArr[3].Y = y;
                    }
                }
            }
        }
        return potArr;
    }

    static bool IsSquare(Point[] pointArr)
    {
        Rect rect2 = Cv2.BoundingRect(pointArr);
        //区域必须大于10 小于100 并且近似于正方形的
        int width = rect2.Width;
        int height = rect2.Height;
        if (width < 10 || height < 10 || width > 100 || height > 100)
        {
            return false;
        }
        double contourArea = Cv2.ContourArea(pointArr);
        //用面积大于长乘以宽的0.8来判断近似矩形
        if (contourArea < 0.8 * width * height)
        {
            return false;
        }
        double ratio = 1.0 * width / height;
        return Math.Abs(1 - ratio) <= 0.2; // 长宽比例误差小于0.2认为是正方形
    }

    /// <summary>
    /// 判断外接矩形是否为正方形
    /// </summary>
    /// <param name="pointArr"></param>
    /// <returns></returns>
    static bool IsSquare2(Point[] pointArr)
    {
        RotatedRect boundingRect = Cv2.MinAreaRect(pointArr);
        Point2f[] vertices = boundingRect.Points();
        // 编写逻辑判断外接矩形是否为正方形
        // 根据外接矩形的长宽比例来判断是否为正方形
        double width = vertices[0].DistanceTo(vertices[1]);
        double height = vertices[1].DistanceTo(vertices[2]);
        //宽高小于10的排除
        if (width < 10 || height < 10 || width > 100 || height > 100)
        {
            return false;
        }
        double contourArea = Cv2.ContourArea(pointArr);
        if (contourArea < 0.8 * width * height)
        {
            return false;
        }
        double ratio = width / height;
        return Math.Abs(1 - ratio) <= 0.2; // 长宽比例误差小于0.2认为是正方形
    }

    static bool IsRectangle(Point[] pointArr)
    {
        Rect rect2 = Cv2.BoundingRect(pointArr);
        int width = rect2.Width;
        int height = rect2.Height;
        var area = width * height;
        double contourArea = Cv2.ContourArea(pointArr);
        if (Math.Abs(contourArea - area) > 0.2 * area)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 计算两点之间的距离
    /// </summary>
    public static int GetSpace(Point pt1, Point pt2)
    {
        int xspace = Math.Abs(pt1.X - pt2.X);
        int yspace = Math.Abs(pt1.Y - pt2.Y);
        return (int)Math.Sqrt(Math.Pow(xspace, 2) + Math.Pow(yspace, 2));
    }

    /// <summary>
    /// 计算两点之间的距离
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public static int GetSpace(int x1, int y1, int x2, int y2)
    {
        int xspace = Math.Abs(x1 - x2);
        int yspace = Math.Abs(y1 - y2);
        return (int)Math.Sqrt(Math.Pow(xspace, 2) + Math.Pow(yspace, 2));
    }

    /// <summary>
    /// 对图像进行矫正(转为鸟瞰图，删除多余边界)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="contour"></param>
    /// <returns></returns>
    public static Mat WarpPerspective(this Mat source, Point[] contour)
    {
        //使用DP算法拟合答题卡的几何轮廓,保存点集pts并顺时针排序
        double result_length = Cv2.ArcLength(contour, true);
        Point[] pts = Cv2.ApproxPolyDP(contour, result_length * 0.02, true);
        int width = 0;
        int height = 0;
        if (pts.Length == 4)
        {
            if (pts[1].X < pts[3].X)
            {
                //说明当前为逆时针存储，改为顺时针存储（交换第2、4点）
                Point p = new Point();
                p = pts[1];
                pts[1] = pts[3];
                pts[3] = p;
            }
            if (Math.Abs(pts[0].X - pts[3].X) > 100)
            {
                Point temp = pts[pts.Length - 1];
                for (int i = pts.Length - 1; i >= 0; i--)
                {
                    if (i == 0)
                        pts[i] = temp;
                    else
                        pts[i] = pts[i - 1];
                }
            }
            //进行透视变换
            //1.确定变化尺寸的宽度

            float width1 = (pts[0].X - pts[1].X) * (pts[0].X - pts[1].X) + (pts[0].Y - pts[1].Y) * (pts[0].Y - pts[1].Y);
            float width2 = (pts[2].X - pts[3].X) * (pts[2].X - pts[3].X) + (pts[2].Y - pts[3].Y) * (pts[2].Y - pts[3].Y);
            width = width1 > width2 ? (int)Math.Sqrt(width1) : (int)Math.Sqrt(width2);
            //2.确定变化尺寸的高度

            float height1 = (pts[0].X - pts[3].X) * (pts[0].X - pts[3].X) + (pts[0].Y - pts[3].Y) * (pts[0].Y - pts[3].Y);
            float height2 = (pts[2].X - pts[1].X) * (pts[2].X - pts[1].X) + (pts[2].Y - pts[1].Y) * (pts[2].Y - pts[1].Y);
            height = height1 > height2 ? (int)Math.Sqrt(height1) : (int)Math.Sqrt(height2);
        }


        Point2f[] pts_src = Array.ConvertAll(pts.ToArray(), new Converter<Point, Point2f>(PointToPointF));
        Point2f[] pts_target = new Point2f[] { new Point2f(0, 0), new Point2f(width - 1, 0), new Point2f(width - 1, height - 1), new Point2f(0, height - 1) };

        //4.计算透视变换矩阵
        //4.1类型转化
        Mat data = Cv2.GetPerspectiveTransform(pts_src, pts_target);
        //5.进行透视变换
        Mat birdMat = new Mat();
        //进行透视操作
        // Mat mat_Perspective = new Mat();
        // Mat src_gray = new Mat();
        // Cv2.CvtColor(source, src_gray, ColorConversionCodes.BGR2GRAY);
        Cv2.WarpPerspective(source, birdMat, data, new Size(width, height));
        return birdMat;
    }

    /// <summary>
    /// 透视变换/顶点变换
    /// </summary>
    /// <param name="source"></param>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Mat WarpPerspective1(this Mat source, Point[] points)
    {
        //设置原图变换顶点
        List<Point2f> affinePoints0 = [points[0], points[1], points[2], points[3]];
        //设置目标图像变换顶点
        List<Point2f> affinePoints1 =
        [
            new Point(0, 0),
            new Point(source.Width, 0),
            new Point(source.Width, source.Height),
            new Point(0, source.Height)
        ];
        //计算变换矩阵
        Mat trans = Cv2.GetPerspectiveTransform(affinePoints0, affinePoints1);
        //矩阵仿射变换
        Mat dst = new();
        Cv2.WarpPerspective(
            source,
            dst,
            trans,
            new Size() { Height = source.Rows, Width = source.Cols }
        );
        return dst;
    }

    /// <summary>
    /// 透视变换
    /// </summary>
    /// <param name="source"></param>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Mat WarpPerspective2(this Mat source, Point[] points)
    {
        //设置原图变换顶点
        List<Point2f> affinePoints0 = [points[0], points[1], points[2], points[3]];
        //设置目标图像变换顶点
        List<Point2f> affinePoints1 =
        [
            new Point(0, 0),
            new Point(source.Width, 0),
            new Point(source.Width, source.Height),
            new Point(0, source.Height)
        ];
        //计算变换矩阵
        Mat trans = Cv2.GetAffineTransform(affinePoints0, affinePoints1);

        //矩阵仿射变换
        Mat dst = new();
        Cv2.WarpAffine(
            source,
            dst,
            trans,
            new Size() { Height = source.Rows, Width = source.Cols }
        );
        return dst;
    }

    ///  <summary>
    /// 区域是否涂卡
    ///  </summary>
    ///  <param name="source"></param>
    ///  <param name="rect"></param>
    ///  <param name="max"></param>
    ///  <returns></returns>
    public static bool IsSmearCard(this Mat source, Rect rect, double max = 0.25)
    {
        Mat matTemp = new(source, rect);
        int count = Cv2.CountNonZero(matTemp);
        int total = matTemp.Cols * matTemp.Rows;
        double rate = 1.0f * (total - count) / total;
        return rate > max;
    }

    /// <summary>
    /// 获取黑色块的占比
    /// </summary>
    /// <param name="source"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static double GetSmearRate(this Mat source, Rect rect)
    {
        Mat matTemp = new(source, rect);
        int count = Cv2.CountNonZero(matTemp);
        int total = matTemp.Cols * matTemp.Rows;
        double rate = 1.0f * (total - count) / total;
        return double.Parse(rate.ToString("0.00"));
    }

    /// <summary>
    /// 轮廓识别，使用最外轮廓发抽取轮廓RETR_EXTERNAL，轮廓识别方法为CHAIN_APPROX_SIMPLE
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Point[][] FindContours(this Mat source)
    {
        Cv2.FindContours(
            source,
            out var contours,
            out var hierarchy,
            RetrievalModes.List,
            ContourApproximationModes.ApproxSimple
        );
        return contours;
    }

    public static bool IsGreatArc(Point[] contours)
    {
        var minXmaxY = (X: -1, Y: 0);
        var minXminY = (X: -1, Y: 0);
        var maxXmaxY = (X: -1, Y: 0);
        var maxXminY = (X: -1, Y: 0);

        int minX = -1, minY = -1, maxX = -1, maxY = -1;
        foreach (var item in contours)
        {
            if (item.X < minX || minX == -1)
                minX = item.X;
            if (item.X > maxX || maxX == -1)
                maxX = item.X;
            if (item.Y < minY || minY == -1)
                minY = item.Y;
            if (item.Y > maxY || maxY == -1)
                maxY = item.Y;
        }
        foreach (var item in contours)
        {
            if (Math.Abs(item.X - minX) + Math.Abs(item.Y - maxY) < Math.Abs(minXmaxY.X - minX) + Math.Abs(minXmaxY.Y - maxY) || minXmaxY.X == -1)
            {
                minXmaxY.X = item.X;
                minXmaxY.Y = item.Y;
            }
            if (Math.Abs(item.X - minX) + Math.Abs(item.Y - minY) < Math.Abs(minXminY.X - minX) + Math.Abs(minXminY.Y - minY) || minXminY.X == -1)
            {
                minXminY.X = item.X;
                minXminY.Y = item.Y;
            }
            if (Math.Abs(item.X - maxX) + Math.Abs(item.Y - maxY) < Math.Abs(maxXmaxY.X - maxX) + Math.Abs(maxXmaxY.Y - maxY) || maxXmaxY.X == -1)
            {
                maxXmaxY.X = item.X;
                maxXmaxY.Y = item.Y;
            }
            if (Math.Abs(item.X - maxX) + Math.Abs(item.Y - minY) < Math.Abs(maxXminY.X - maxX) + Math.Abs(maxXminY.Y - minY) || maxXminY.X == -1)
            {
                maxXminY.X = item.X;
                maxXminY.Y = item.Y;
            }
        }
        if (Math.Abs(minXminY.X - minXmaxY.X) < 30 && Math.Abs(maxXminY.X - maxXmaxY.X) < 30 && minXmaxY.X > 5 && minXminY.Y > 5 && maxXminY.X - minXminY.X > 50 && minXmaxY.Y - minXminY.Y > 50)
        {
            if (Math.Abs(minXminY.Y - maxXminY.Y) < 30 && Math.Abs(minXmaxY.Y - maxXmaxY.Y) < 30)
            {
                return true;
            }

        }

        return false;
    }

    public static Mat Crop(this Mat source, Point[] contour)
    {
        var rect = Cv2.BoundingRect(contour);
        return source.Clone(rect);
    }

    /// <summary>
    /// 寻找边界
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static List<Point[]> GetBoundarys(this Mat source, int width, string size = "0,90,3,true")
    {
        //进行高斯滤波
        Mat blurred = new();
        Cv2.GaussianBlur(source, blurred, new Size(3, 3), 0);
        //进行canny边缘检测
        Mat canny = new();
        //Cv2.Canny(blurred, canny, 0, 180);
        string[] str = size.Split(",");

        //  edges 为计算得到的边缘图像。
        //  image 为 8 位输入图像。
        //  threshold1 表示处理过程中的第一个阈值。
        //  threshold2 表示处理过程中的第二个阈值。
        //  apertureSize 表示 Sobel 算子的孔径大小。
        //  L2gradient 为计算图像梯度幅度（gradient magnitude）的标识。其默认值为 False。如果为 True，则使用更精确的 L2 范数进行计算（即两个方向的导数的平方和再开方），否则使用 L1 范数（直接将两个方向导数的绝对值相加）。
        Cv2.Canny(blurred, canny, int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]), bool.Parse(str[3]));
        Show(canny, "canny");

        //寻找矩形边界
        // image，灰度图片输入
        // contours，轮廓结果输出
        // mode，轮廓检索模式
        //      External，只检测外层轮廓
        //      List，提取所有轮廓，并放置在list中，检测的轮廓不建立等级关系
        //      CComp，提取所有轮廓，并将轮廓组织成双层结构(two - level hierarchy),顶层为连通域的外围边界，次层位内层边界
        //      Tree，提取所有轮廓并重新建立网状轮廓结构
        //      FloodFill，官网没有介绍，应该是洪水填充法
        // method，轮廓近似方法
        //      ApproxNone，获取每个轮廓的每个像素，相邻的两个点的像素位置差不超过1
        //      ApproxSimple，压缩水平方向，垂直方向，对角线方向的元素，值保留该方向的重点坐标，如果一个矩形轮廓只需4个点来保存轮廓信息
        //      ApproxTC89L1，使用Teh - Chinl链逼近算法中的一种
        //      ApproxTC89KCOS，使用Teh - Chinl链逼近算法中的一种
        Cv2.FindContours(canny, out Point[][] contours, out HierarchyIndex[] hierarchly, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        List<Point[]> result = [];
        if (contours.Length == 1)
        {
            result.Add(contours[0]);
        }
        else
        {
            for (int i = 0; i < contours.Length; i++)
            {
                if (contours[i].Length < 4)
                {
                    continue;
                }

                double tem = Cv2.ArcLength(contours[i], true);
                // 周长太小或太大，不作为结果
                if (tem < width * 2 || tem > width * 5)
                {
                    continue;
                }

                var rect = Cv2.BoundingRect(contours[i]);
                // 最长边太小，不作为结果
                if (rect.Width < width * .8 || rect.Height < (width / 7.0 / 3.0 * .5))
                {
                    continue;
                }

                var area = rect.Width * rect.Height;
                double contourArea = Cv2.ContourArea(contours[i]);
                if (Math.Abs(contourArea - area) > 0.2 * area)
                {
                    // 面积相差太大，不是矩形，不作为结果
                    continue;
                }
                // if (tem < width * 2 || tem > width * 5)
                // {
                //     // 面积太小或太大，不作为结果
                //     continue;
                // }


                result.Add(contours[i]);

            }
        }
        return result;
    }

    /// <summary>
    /// Point转换为PointF类型
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static Point2f PointToPointF(Point p)
    {
        return new Point2f(p.X, p.Y);
    }


    /// <summary>
    /// 转换到ByteArray
    ///
    /// 转换Bitmap到ZXing的BinaryBitmap
    ///     var bytes = source.ToBytes();
    ///     LuminanceSource s = new RGBLuminanceSource(bytes, source.Width, source.Height, BitmapFormat.Gray8);
    ///     BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(s));
    ///     QRCodeReader reader = new();
    ///     var result = reader.decode(bitmap);
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this Mat source)
    {
        // 确保图像数据是连续的
        source.ThrowIfDisposed();

        // 获取图像的字节数
        int size = source.Rows * source.Cols * source.Channels();

        // 创建字节数组并复制数据
        byte[] byteArray = new byte[size];
        Marshal.Copy(source.Data, byteArray, 0, size);
        return byteArray;
    }


}
