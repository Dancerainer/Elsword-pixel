using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        void ApplyVelderChaseCamera(ref float cameraX, ref float targetY) {
            float x, y;
            if (!game.VelderChaseCamera(out x, out y)) return;
            // Preserve the source's leftward focus and keep the player visible
            // in this version's narrower pixel viewport.
            x = Mathf.Clamp(x, game.player.x - 205, game.player.x + 205);
            cameraX = Mathf.Clamp(x, 240, Mathf.Max(240, game.RoomWidth - 240));
            targetY = y;
        }

        void RenderRegionalWarning(bool peita) {
            bool palace = game.VelderPalaceAlarm;
            string warning=peita ? game.PeitaArenaAlarm : "";
            if(palace)warning="DANGER   GLITTER VANGUARD";
            if(game.RegionShakeRequired)warning="ALTERNATE LEFT AND RIGHT TO BREAK FREE";
            TextValue("Feita Boss Warning",warning);
            if (labels.TryGetValue("Feita Boss Warning", out var text))
                text.color = C(palace ? 0xff50ceu : 0xffa449u);
        }
    }
}
