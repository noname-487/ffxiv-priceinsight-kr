using System;
using ImGuiNET;

namespace PriceInsight; 

class ConfigUI : IDisposable {
    private readonly PriceInsightPlugin plugin;

    private bool settingsVisible = false;

    public bool SettingsVisible {
        get => settingsVisible;
        set => settingsVisible = value;
    }

    public ConfigUI(PriceInsightPlugin plugin) {
        this.plugin = plugin;
    }

    public void Dispose() {
    }

    public void Draw() {
        if (!SettingsVisible) {
            return;
        }

        var conf = plugin.Configuration;
        if (ImGui.Begin("Price Insight 설정", ref settingsVisible,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)) {
            var configValue = conf.RefreshWithAlt;
            if (ImGui.Checkbox("Alt키를 눌러 가격을 새로고침", ref configValue)) {
                conf.RefreshWithAlt = configValue;
                conf.Save();
            }
            
            configValue = conf.PrefetchInventory;
            if (ImGui.Checkbox("인벤토리 내 물품 가격을 미리 로드", ref configValue)) {
                conf.PrefetchInventory = configValue;
                conf.Save();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("로그인 시 인벤토리, 초코보 가방, 집사 소지품의 가격을 미리 로드.\n경고: \"지역\" 설정 활성화시 네트워크 부하가 많이 발생합니다.");

            configValue = conf.UseCurrentWorld;
            if (ImGui.Checkbox("현재 서버를 고향 서버로 설정", ref configValue)) {
                conf.UseCurrentWorld = configValue;
                conf.Save();
                plugin.ClearCache();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("현재 접속중인 서버를 \"고향 서버\"로 간주합니다.\n데이터 센터 이동하여 가격을 확인할때 유용합니다.");

            configValue = conf.ForceIpv4;
            if (ImGui.Checkbox("Universalis 연결시 강제로 Ipv4를 적용", ref configValue)) {
                conf.ForceIpv4 = configValue;
                conf.Save();
                plugin.UniversalisClient.ForceIpv4(configValue);
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("VPN 사용시 필요할 수 있습니다.\n연결 문제가 발생시에 해당 기능을 활성화해주세요.");

            ImGui.Separator();
            ImGui.PushID(0);
            
            ImGui.Text("가장 낮은 가격 표시 설정");
            
            configValue = conf.ShowRegion;
            if (ImGui.Checkbox("지역", ref configValue)) {
                conf.ShowRegion = configValue;
                conf.Save();
                plugin.ClearCache();
            }
            TooltipRegion();

            configValue = conf.ShowDatacenter;
            if (ImGui.Checkbox("데이터 센터", ref configValue)) {
                conf.ShowDatacenter = configValue;
                conf.Save();
                plugin.ClearCache();
            }

            configValue = conf.ShowWorld;
            if (ImGui.Checkbox("고향 서버", ref configValue)) {
                conf.ShowWorld = configValue;
                conf.Save();
            }
            
            ImGui.PopID();
            ImGui.Separator();
            ImGui.PushID(1);
            
            ImGui.Text("최근에 거래된 가격 표시 설정");

            configValue = conf.ShowMostRecentPurchaseRegion;
            if (ImGui.Checkbox("지역", ref configValue)) {
                conf.ShowMostRecentPurchaseRegion = configValue;
                conf.Save();
                plugin.ClearCache();
            }
            TooltipRegion();

            configValue = conf.ShowMostRecentPurchase;
            if (ImGui.Checkbox("데이터 센터", ref configValue)) {
                conf.ShowMostRecentPurchase = configValue;
                conf.Save();
                plugin.ClearCache();
            }

            configValue = conf.ShowMostRecentPurchaseWorld;
            if (ImGui.Checkbox("고향 서버", ref configValue)) {
                conf.ShowMostRecentPurchaseWorld = configValue;
                conf.Save();
            }
            
            ImGui.PopID();
            ImGui.Separator();
            
            configValue = conf.ShowDailySaleVelocity;
            if (ImGui.Checkbox("일일 거래량 표시", ref configValue)) {
                conf.ShowDailySaleVelocity = configValue;
                conf.Save();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("지난 20건의 거래를 기준으로 하루 평균 거래량을 표시해줍니다.");

            configValue = conf.ShowAverageSalePrice;
            if (ImGui.Checkbox("평균 가격 표시", ref configValue)) {
                conf.ShowAverageSalePrice = configValue;
                conf.Save();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("지난 20건의 개러를 기준으로 평균 가격을 표시해줍니다.");
            
            configValue = conf.ShowStackSalePrice;
            if (ImGui.Checkbox("묶음 가격 표시", ref configValue)) {
                conf.ShowStackSalePrice = configValue;
                conf.Save();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("거래소 가격을 기준으로 총 금액을 표시해줍니다.");
            
            configValue = conf.ShowAge;
            if (ImGui.Checkbox("데이터 경과일 표시", ref configValue)) {
                conf.ShowAge = configValue;
                conf.Save();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("가격 정보가 마지막으로 갱신된 시점을 표시합니다.\n툴팁이 길어지는 것을 방지하기 위해 끌 수 있습니다.");
            
            configValue = conf.ShowDatacenterOnCrossWorlds;
            if (ImGui.Checkbox("데이터 센터의 다른 월드 표시", ref configValue)) {
                conf.ShowDatacenterOnCrossWorlds = configValue;
                conf.Save();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("서버가 포함된 데이터 센터의 다른 서버의 가격을 표시합니다.\n툴팁이 길어지는 것을 방지하기 위해 끌 수 있습니다.");
            
            configValue = conf.ShowBothNqAndHq;
            if (ImGui.Checkbox("항상 NQ와 HQ 가격을 표시", ref configValue)) {
                conf.ShowBothNqAndHq = configValue;
                conf.Save();
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("아이템의 NQ와 HQ가격을 모두 표시합니다.\n비활성화 되어있다면 현재 품질에 대한 가격만을 표시합니다 (Ctrl 키로 표시하는 품질을 전환합니다.).");
        }

        ImGui.End();
    }

    private static void TooltipRegion() {
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("서버 이동이 가능한 모든 서버를 포함합니다.");
    }
}