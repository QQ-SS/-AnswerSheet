using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;

var imgWidth = 2684;
var col = 15;
var row = 6;

Mat source = Cv2.ImRead("img/1.webp")
    // ?.Resize(.5)?.Show()
    ?.Resize(width: imgWidth)?.Show()
    ?.CorrectDistortion(out var code, out var qrWidth)?.Show()
    ?? throw new Exception("srcImg is null!");

Mat binary = source.Show("source")
    .Gray().Show()
    // .Morphology(6).Show()
    .Binary().Show()
    ;

// 填写区宽度 7 倍于二维码宽度
List<Point[]> contours = binary.GetBoundarys(qrWidth * 7);

// 绘制识别结果
for (int i = 0; i < contours.Count; i++)
{
    Scalar[] color = [Scalar.Red, Scalar.Green, Scalar.Blue, Scalar.Gold, Scalar.RandomColor(), Scalar.RandomColor(), Scalar.RandomColor(), Scalar.RandomColor()];
    if (contours[i].Length > 100)
    {
        Cv2.DrawContours(
            source,
            contours,
            contourIdx: i,
            color: color[i % color.Length],
            thickness: 8,
            lineType: LineTypes.Link8,
            hierarchy: null,
            maxLevel: 0);
    }
}

source.Show("Result");

// 识别答题区域
if (contours.Count == 4)
{
    Question result = new()
    {
        CardNo = code,
        Questions = [],
    };

    var matQuestions = binary.Crop(contours[0]).Show("Questions");
    var matSubject = binary.Crop(contours[1]).Show("Subject");
    var matGrade = binary.Crop(contours[2]).Show("Grade");
    // var _matQuestions = binary.WarpPerspective(contours[0]).Show("_Questions");
    // var _matSubject = binary.WarpPerspective(contours[1]).Show("_Subject");
    // var _matGrade = binary.WarpPerspective(contours[2]).Show("_Grade");



    Console.WriteLine(JsonSerializer.Serialize(result));
}


Cv2.WaitKey(0);
Cv2.DestroyAllWindows(); //销毁所有窗口
