using System.Drawing;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;

public static partial class QrCode
{
    /// <summary>
    /// 修正二维码的畸变
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Mat CorrectDistortion(this Mat source, out string code, out int width)
    {

        // var reader = new BarcodeReader();
        // reader.Options.PossibleFormats = [BarcodeFormat.QR_CODE];
        // reader.AutoRotate = true;
        // reader.Options.TryHarder = true;
        // reader.Options.PureBarcode = false;
        // var result = reader.Decode(source);
        // code = result?.Text;
        // 检测二维码
        QRCodeDetector decoder = new();
        // decoder.SetEpsX(0.1f);
        // decoder.SetEpsY(0.1f);
        code = decoder.DetectAndDecode(source, out var srcPoints);

        width = 0;
        if (!string.IsNullOrWhiteSpace(code) && srcPoints != null)
        {
            for (int i = 0; i < srcPoints.Length; i++)
            {
                // 绘制二维码的四个角点
                Point pt1 = new(srcPoints[i].X, srcPoints[i].Y);
                Point pt2 = new(srcPoints[(i + 1) % srcPoints.Count()].X, srcPoints[(i + 1) % srcPoints.Count()].Y);
                Cv2.Line(source, pt1, pt2, Scalar.Red, 8);
                if (i == 0)
                {
                    width = CvUtils.GetSpace(pt1, pt2);
                }
            }

            // 计算二维码偏移角度
            var angle = GetAngle(srcPoints);
            return source.Rotate(angle);
        }

        return source;
    }


    /// <summary>
    /// 使用方位角法计算角度
    /// </summary>
    public static double AzimuthAngle(double x1, double y1, double x2, double y2)
    {
        // 计算横纵坐标的差值
        double dx = x2 - x1;
        double dy = y2 - y1;

        // 求斜率，注意处理dx为0的情况以避免除以0的错误
        double k = dx == 0 ? double.PositiveInfinity : dy / dx;

        // 计算弧度值，使用Math.Atan函数，当dx为0时，需要特殊处理
        double angle = dx == 0 ? (dy > 0 ? Math.PI / 2 : -Math.PI / 2) : Math.Atan(k);

        // return angle;
        // 将弧度值转换为角度值
        return angle * 180 / Math.PI;
    }

    /// <summary>
    /// 给定四个角点，计算角度
    /// </summary>
    /// <param name="locs"></param>
    /// <returns></returns>
    public static double GetAngle(Point2f[] locs)
    {
        // Sort locations from bottom to top, left to right
        Array.Sort(
            locs,
            (p1, p2) =>
            {
                int compare = p1.Y.CompareTo(p2.Y);
                if (compare == 0)
                {
                    compare = p1.X.CompareTo(p2.X);
                }
                return compare;
            }
        );

        return AzimuthAngle(locs[2].X, locs[2].Y, locs[3].X, locs[3].Y);
    }

}
