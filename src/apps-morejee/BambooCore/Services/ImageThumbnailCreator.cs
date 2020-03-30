using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace BambooCore
{
    /// <summary>
    /// 图片缩略图生成器
    /// </summary>
    public class ImageThumbnailCreator
    {
        /// <summary>
        /// 为一个本地路径的图片生成多个版本的缩略图，比如 dir/aa.jpg， 将会生成 dir/aa_128.jpg, dir/aa_256.jpg
        /// 缩略图将会保留图片的长宽比，用128x128, 256x256的尺寸容纳。如果不是正方形的图片会有白边。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool SaveImageThumbnails(string path)
        {
            bool bOk1 = SaveImageThumbnailCenterCrop(path, 128);
            bool bOk2 = SaveImageThumbnailCenterCrop(path, 256);
            return bOk1 && bOk2;
        }

        /// <summary>
        /// 获取一个图片路径的缩略图路径，路径支持本地完整路径，本地相对路径，网络路径。 不校验文件是否存在
        /// </summary>
        /// <param name="srcPath">原图路径</param>
        /// <param name="borderSize">缩略图边长，比如128</param>
        /// <returns></returns>
        public static string GetThumbnailPath(string srcPath, int borderSize)
        {
            string savePath = srcPath;
            string extension = "";
            int index = srcPath.LastIndexOf('.');
            if (index > 0)
            {
                savePath = srcPath.Substring(0, index);
                extension = srcPath.Substring(index);
            }
            savePath = string.Format("{0}_{1}{2}", savePath, borderSize, extension);
            return savePath;
        }

        /// <summary>
        /// 为一个本地图片生成一个指定大小的缩略图文件，包含全图内容，保持长宽比等比缩放
        /// </summary>
        /// <param name="path"></param>
        /// <param name="borderSize"></param>
        /// <returns></returns>
        public static bool SaveImageThumbnail(string path, int borderSize)
        {
            return SaveImageThumbnail(path, GetThumbnailPath(path, borderSize), borderSize, borderSize);
        }
        /// <summary>
        /// 为一个本地图片生成一个指定大小的缩略图，从图片中心开始裁剪出一个最大的正方形区域
        /// </summary>
        /// <param name="path"></param>
        /// <param name="borderSize"></param>
        /// <returns></returns>
        public static bool SaveImageThumbnailCenterCrop(string path, int borderSize)
        {
            return SaveImageThumbnailCenterCrop(path, GetThumbnailPath(path, borderSize), borderSize, borderSize);
        }

        public static bool SaveImageThumbnail(string path, string thumbnailPath, int width, int height)
        {
            try
            {
                using (Image<Rgba32> image = Image.Load(path))
                {
                    if (image == null)
                        return false;
                    ResizeOptions options = new ResizeOptions();
                    options.Mode = ResizeMode.BoxPad;
                    options.Size = new Size(width, height);
                    image.Mutate(x => x.Resize(options));
                    image.Save(thumbnailPath);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool SaveImageThumbnailCenterCrop(string path, string thumbnailPath, int width, int height)
        {
            try
            {
                using (Image<Rgba32> image = Image.Load(path))
                {
                    if (image == null)
                        return false;
                    int border = Math.Min(image.Width, image.Height);
                    int x, y;
                    x = y = 0;
                    if (image.Width > border)
                        x = (image.Width - border) / 2;
                    else
                        y = (image.Height - border) / 2;
                    image.Mutate(img => img.Crop(new Rectangle(x, y, border, border)).Resize(width, height));
                    image.Save(thumbnailPath);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
