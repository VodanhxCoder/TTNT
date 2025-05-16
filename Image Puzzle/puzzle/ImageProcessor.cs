using System;
using System.Drawing;

public class ImageProcessor
{
    // Hàm lấy vùng crop chính giữa ảnh
    public static Rectangle GetCenterCropArea(Bitmap image, int targetWidth, int targetHeight)
    {
        int x = (image.Width - targetWidth) / 2;
        int y = (image.Height - targetHeight) / 2;
        return new Rectangle(x, y, targetWidth, targetHeight);
    }

    public static Bitmap ResizeImage(Bitmap originalImage, int width, int height)
    {
        Bitmap resized = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(resized))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, width, height);
        }
        return resized;
    }


    // Hàm crop ảnh theo vùng cropArea
    public static Bitmap CropImage(Bitmap original, Rectangle cropArea)
    {
        return original.Clone(cropArea, original.PixelFormat);
    }

    // Hàm cắt ảnh thành 9 ô vuông 3x3
    public static Bitmap[] CropImageToTiles(Bitmap image)
    {
        int rows = 3;
        int cols = 3;
        int tileWidth = image.Width / cols;
        int tileHeight = image.Height / rows;

        Bitmap[] tiles = new Bitmap[rows * cols];
        int index = 0;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Rectangle rect = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                tiles[index++] = image.Clone(rect, image.PixelFormat);
            }
        }

        return tiles;
    }

    // Hàm xử lý tổng thể
    public static (Bitmap resizedImage, Bitmap[] tiles) ProcessImageWithResized(string imagePath, int cropWidth, int cropHeight)
    {
        Bitmap resized = null;
        Bitmap[] tiles;

        using (Bitmap original = new Bitmap(imagePath))
        {
            Bitmap toProcess = original;
            if (original.Width < cropWidth || original.Height < cropHeight)
            {
                resized = ResizeImage(original, Math.Max(cropWidth, original.Width), Math.Max(cropHeight, original.Height));
                toProcess = resized;
            }

            Rectangle cropArea = GetCenterCropArea(toProcess, cropWidth, cropHeight);
            using (Bitmap cropped = CropImage(toProcess, cropArea))
            {
                tiles = CropImageToTiles(cropped);
            }
        }

        return (resized, tiles);
    }


}
