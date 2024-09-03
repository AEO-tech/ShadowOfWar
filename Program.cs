using ClickableTransparentOverlay;
using ImGuiNET;
using Swed64;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
namespace ShadowOfWar
{
    public class Renderer : Overlay
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        public bool arrows = false;
        public bool health = false;
        public bool focus = false;
        public bool execution = false;
        public bool wrath = false;

        bool ShowWindow = true;
        bool confirm = false;

        private Vector4 textColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        private Vector4 backgroundColor = new Vector4(0.07f, 0.11f, 0.18f, 255f);
        private Vector4 border = new Vector4(0.07f, 0.11f, 0.18f, 255f);
        private float borderSize = 0.1f;

        protected override void Render()
        {
            if (GetAsyncKeyState(0x2D) < 0)
            {
                ShowWindow = !ShowWindow;
                Thread.Sleep(200);
            }

            Swed swed = new("ShadowOfWar");
            IntPtr moduleBase = swed.GetModuleBase("ShadowOfWar.exe");
            IntPtr Arrow = swed.ReadPointer(moduleBase, 0x02D21140, 0x28, 0x468, 0x18, 0x8, 0x0) + 0xA8;
            IntPtr Health = swed.ReadPointer(moduleBase, 0x02D21140, 0x28, 0x468, 0x20) + 0x42C;
            IntPtr Focus = swed.ReadPointer(moduleBase, 0x02D21140, 0x28, 0x468, 0x8, 0x0, 0x0, 0x0) + 0x224;
            IntPtr Execution = swed.ReadPointer(moduleBase, 0x02D1C9F8, 0x70, 0x48, 0x10) + 0x2C8;
            IntPtr Wrath = swed.ReadPointer(moduleBase, 0x02D21140, 0xE8, 0x28, 0x18, 0x8) + 0x308;

            if (ShowWindow)
            {
                Process[] pname = Process.GetProcessesByName("ShadowOfWar");
                if (pname.Length == 1)
                {
                    ImGui.Begin("ERROR");
                    ImGui.SetWindowSize(new Vector2(255, 95));
                    ImGui.SetWindowPos(new Vector2(0, 0));
                    ImGui.Text("Shadow of War not running.\nClose hacks and then open the game.\nConfirm to open the hacks anyway.");
                    if (ImGui.Button("Confirm"))
                    {
                        confirm = true;
                    }
                }
                else confirm = true;

                if (confirm == true)
                {
                    ImGui.Begin("Shadow of War");
                    ImGui.SetWindowSize(new Vector2(300, 215));

                    if (ImGui.BeginTabBar("Shadow of War"))
                    {
                        if (ImGui.BeginTabItem("Cheats"))
                        {
                            ImGui.Checkbox("GodMode", ref health);
                            ImGui.Checkbox("Infinite Arrows", ref arrows);
                            ImGui.Checkbox("Infinite Focus", ref focus);
                            ImGui.Checkbox("Infinite Execution",ref execution);
                            ImGui.Button(("give max Wrath"));
                                wrath = true;
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("Stats"))
                        {
                            ImGui.Text("Player Stats:");
                            ImGui.Text($"Health = {swed.ReadFloat(Health).ToString()}");
                            ImGui.Text($"Arrows = {swed.ReadInt(Arrow).ToString()}");
                            ImGui.Text($"Focus = {swed.ReadFloat(Focus).ToString()}");
                            ImGui.Text($"Execution = {swed.ReadFloat(Execution).ToString()}");
                            ImGui.Text($"Wrath = {swed.ReadFloat(Wrath).ToString()}");
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("Customize"))
                        {
                            ImGui.Text("Select Background Color:");
                            ImGui.ColorEdit4("Background", ref backgroundColor);

                            ImGui.PushStyleColor(ImGuiCol.WindowBg, backgroundColor);

                            ImGui.Text("Select Text Color:");
                            ImGui.ColorEdit4("TextColor", ref textColor);

                            ImGui.PushStyleColor(ImGuiCol.Text, textColor);

                            ImGui.Text("Select Border Color:");
                            ImGui.ColorEdit4("BorderColor", ref border);

                            ImGui.PushStyleColor(ImGuiCol.Border, border);

                            ImGui.Text("Adjust Border Size:");
                            ImGui.SliderFloat("Border Size", ref borderSize, 0.0f, 10.0f, "%.1f");

                            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, borderSize);

                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }
                    ImGui.End();

                    if (arrows)
                        swed.WriteBytes(Arrow, "90 90 90 90 90 90");

                    if (health)
                        swed.WriteFloat(Health, 100000F);

                    if (focus)
                        swed.WriteFloat(Focus, 10000F);

                    if (execution)
                        swed.WriteFloat(Execution, 500F);

                    if (wrath)
                    {
                        swed.WriteFloat(Wrath, 99F);
                        wrath = false;
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            Renderer renderer = new Renderer();
            Thread program = new Thread(renderer.Start().Wait);
            program.Start();
        }
    }
}