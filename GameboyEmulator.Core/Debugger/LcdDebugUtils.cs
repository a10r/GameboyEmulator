using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Utils;
using GameboyEmulator.Core.Video;
using System;

namespace GameboyEmulator.Core.Debugger
{
    public static class LcdDebugUtils
    {
        public static void Overlay(this Bitmap bottom, Bitmap top, int x, int y)
        {
            for (int iy = 0; iy < top.Height; iy++)
            {
                for (int ix = 0; ix < top.Width; ix++)
                {
                    bottom[x + ix, y + iy] = top[ix, iy];
                }
            }
        }

        public static Bitmap RenderTile(IMemoryBlock vram, int tileIndex)
        {
            if (tileIndex < 0 || tileIndex >= 384)
                throw new ArgumentException("Invalid tile index.", nameof(tileIndex));

            var shades = new[]
            {
                new Pixel(255, 255, 255),
                new Pixel(192, 192, 192),
                new Pixel(96, 96, 96),
                new Pixel(0, 0, 0),
            };

            var tile = new Bitmap(8, 8);

            var tileOffset = tileIndex << 4;

            for (int y = 0; y < 8; y++)
            {
                var lower = vram[tileOffset + y * 2];
                var upper = vram[tileOffset + y * 2 + 1];

                for (int x = 0; x < 8; x++)
                {
                    var shadeId = (upper.GetBit(7 - x) ? 2 : 0) + (lower.GetBit(7 - x) ? 1 : 0);
                    var shade = shades[shadeId];

                    tile[x, y] = shade;
                }
            }

            return tile;
        }

        public static Bitmap RenderTileset(IMemoryBlock vram)
        {
            const int TILE_COUNT = 384;
            const int TILE_SIZE = 8;
            const int TILES_PER_ROW = 16;

            var tileset = new Bitmap(TILES_PER_ROW * TILE_SIZE, (TILE_COUNT / TILES_PER_ROW) * TILE_SIZE);

            for (int i = 0; i < TILE_COUNT; i++)
            {
                var tilesetX = (i % TILES_PER_ROW) * TILE_SIZE;
                var tilesetY = (i / TILES_PER_ROW) * TILE_SIZE;
                tileset.Overlay(RenderTile(vram, i), tilesetX, tilesetY);
            }

            return tileset;
        }
    }
}
