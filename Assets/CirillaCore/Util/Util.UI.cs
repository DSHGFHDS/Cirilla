
using UnityEngine;

namespace Cirilla
{
    public partial class Util
    {
        public static Sprite ByteToSprite(byte[] datas, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(datas);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        }

        public static byte[] SpriteToByte(Sprite spr)
        {
            return spr.texture.EncodeToPNG();
        }
    }
}